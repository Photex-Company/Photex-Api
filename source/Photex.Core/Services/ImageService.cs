using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using ExifLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Photex.Core.Contracts.Metadata;
using Photex.Core.Contracts.Models;
using Photex.Core.Contracts.Requests;
using Photex.Core.Contracts.Settings;
using Photex.Core.Exceptions;
using Photex.Core.Extensions;
using Photex.Core.Interfaces;
using Photex.Database;
using Photex.Database.Entities;

namespace Photex.Core.Services
{
    public class ImageService : IImageService
    {
        private readonly byte[][] _jpgSignatures = new[] { new byte[] { 255, 216, 255, 224 }, new byte[] { 255, 216, 255, 225 } };
        private readonly PhotexDbContext _context;
        private readonly BlobContainerClient _container;
        private readonly string _baseUrl;

        public ImageService(
            PhotexDbContext context,
            IOptions<ImageContainerSettings> options)
        {
            _baseUrl = options?.Value?.BaseUrl ?? throw new ArgumentNullException(nameof(options));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _container = new BlobServiceClient(options.Value.Connection)
                .GetBlobContainerClient(options.Value.Name);
        }

        public async Task DeleteImage(long userId, long imageId)
        {
            var image = await _context.Images
                .Where(x => x.Id == imageId && x.Catalogue.UserId == userId)
                .FirstOrDefaultAsync();

            if (image != null)
            {
                _context.Remove(image);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CatalogueModel>> GetImages(long userId)
        {
            var catalogues = await _context.Catalogues
                               .Include(x => x.Images)
                               .Where(x => x.UserId == userId)
                               .ToListAsync();

            return catalogues.Select(MapToCatalogueModel);
        }

        public async Task UpdateImage(long userId, long imageId, UpdateImageRequest request)
        {
            var image = await _context.Images
                .Include(x => x.Catalogue)
                .Where(x => x.Id == imageId && x.Catalogue.UserId == userId)
                .FirstOrDefaultAsync();

            if (image != null)
            {
                if (request.DescriptionSpecified)
                {
                    image.Description = request.Description;
                }

                if (request.CatalogueSpecified)
                {
                    var existingCatalogue = image.Catalogue;
                    if (!existingCatalogue.Name.Equals(request.Catalogue))
                    {
                        var catalogue = await _context.Catalogues
                            .Include(x => x.Images)
                            .FirstOrDefaultAsync(x =>
                                x.Name.Equals(request.Catalogue, StringComparison.OrdinalIgnoreCase) &&
                                x.UserId == userId);

                        if (catalogue == null)
                        {
                            catalogue = new Catalogue
                            {
                                Name = request.Catalogue,
                                Images = new List<Image>(),
                                UserId = userId
                            };

                            await _context.Catalogues.AddAsync(catalogue);
                        }

                        catalogue.Images.Add(image);
                        existingCatalogue.Images.Remove(image);
                        await _context.SaveChangesAsync();
                    }
                }

                image.DateModified = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UploadImageFromStream(long userId, long catalogueId, string description, Stream imageStream)
        {
            EnsureIsJpg(imageStream);
            var catalogue = await _context.Catalogues
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x =>
                    x.Id == catalogueId &&
                    x.UserId == userId)
                    ?? throw new ArgumentException("Catalogue not found");

            var path = $"{userId}/{Guid.NewGuid().ToString()}.jpg";
            var blobInfo = await _container.UploadBlobAsync(path, imageStream);
            catalogue.Images.Add(new Image
            {
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                Description = description,
                Url = $"{_baseUrl}/{path}"
            });

            await _context.SaveChangesAsync();
        }

        public async Task<CatalogueModel> GetImagesFromCatalogue(long userId, long catalogueId)
        {
            var catalogue = await _context.Catalogues
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.UserId == userId & x.Id == catalogueId);

            if (catalogue == null)
            {
                return null;
            }

            return MapToCatalogueModel(catalogue);
        }

        private void EnsureIsJpg(Stream imageStream)
        {
            var isJpg = false;
            foreach (var signature in _jpgSignatures)
            {
                var buffer = new byte[signature.Length];
                imageStream.Read(buffer, 0, signature.Length);
                imageStream.Seek(0, SeekOrigin.Begin);
                if (buffer.SequenceEqual(signature))
                {
                    isJpg = true;
                }
            }

            if (!isJpg)
            {
                throw new WrongImageFormatException();
            }
        }

        private CatalogueModel MapToCatalogueModel(Catalogue catalogue)
        {
            var images = new List<ImageInCatalogueModel>();
            foreach (var image in catalogue.Images ?? Enumerable.Empty<Image>())
            {
                images.Add(new ImageInCatalogueModel
                {
                    DateCreated = image.DateCreated,
                    DateModified = image.DateModified,
                    Description = image.Description,
                    Id = image.Id,
                    Url = image.Url
                });
            }

            return new CatalogueModel
            {
                Name = catalogue.Name,
                Images = images.ToArray()
            };
        }

        private async Task<IEnumerable<MetadataExtractor.Directory>> GetImageMetadata(Image image)
        {
            using (var ms = new MemoryStream())
            {
                await _container.GetBlobClient(
                    image.Url.Remove(0, _baseUrl.Length + 1)).DownloadToAsync(ms);

                ms.Seek(0, SeekOrigin.Begin);
                return MetadataExtractor.ImageMetadataReader.ReadMetadata(ms);
            }
        }

        public async Task<ImageModel> GetImageFromUser(long userId, long imageId)
        {
            var image = await _context.Images.FirstOrDefaultAsync(x => x.Id == imageId && x.Catalogue.UserId == userId);
            if (image == null)
            {
                return null;
            }

            var metadata = await GetImageMetadata(image);

            return new ImageModel
            {
                DateCreated = image.DateCreated,
                DateModified = image.DateModified,
                Description = image.Description,
                Id = image.Id,
                Url = image.Url,
                Metadata = metadata.SafeToDictionary(
                    d => d.Name,
                    d => d.Tags.SafeToDictionary(
                        t => t.Name,
                        t => t.Description,
                        (current, updated) => current + '|' + updated),
                    (current, updated) => current.SafeConcatDictionaries(
                            updated,
                            (c, u) => c + " | " + u))
            };
        }

        public async Task UpdateMetadata(long userId, long imageId, UpdateMetadataRequest request)
        {
            var image = await _context.Images.FirstOrDefaultAsync(x => x.Id == imageId && x.Catalogue.UserId == userId);
            if (image == null)
            {
                return;
            }

            ImageFile realImage;
            using (var ms = new MemoryStream())
            {
                await _container.GetBlobClient(
                        image.Url.Remove(0, _baseUrl.Length + 1))
                    .DownloadToAsync(ms);

                ms.Seek(0, SeekOrigin.Begin);
                realImage = await ImageFile.FromStreamAsync(ms);
            }

            foreach (var value in request.NewMetadata)
            {
                realImage.Properties.Set(MetadataInfo.MetadataTags[value.Name], value.NewValue);
            }

            using (var ms = new MemoryStream())
            {
                await realImage.SaveAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                await _container.GetBlobClient(
                        image.Url.Remove(0, _baseUrl.Length + 1))
                    .UploadAsync(ms, overwrite: true);
            }

            image.DateModified = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
