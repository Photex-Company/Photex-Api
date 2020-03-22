using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Photex.Core.Contracts.Models;
using Photex.Core.Contracts.Settings;
using Photex.Core.Interfaces;
using Photex.Database;
using Photex.Database.Entities;

namespace Photex.Core.Services
{
    public class ImageService : IImageService
    {
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

        public async Task<CatalogueModel> GetCatalogue(long userId, string name)
        {
            var catalogue = await _context.Catalogues
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (catalogue == null)
            {
                return null;
            }

            return MapToCatalogueModel(catalogue);
        }

        public async Task<IEnumerable<string>> GetCatalogues(long userId)
            => await _context.Catalogues.Where(x => x.UserId == userId).Select(x => x.Name).ToListAsync();

        public async Task<IEnumerable<CatalogueModel>> GetImages(long userId)
        {
            var catalogues = await _context.Catalogues
                               .Include(x => x.Images)
                               .Where(x => x.UserId == userId)
                               .ToListAsync();

            return catalogues.Select(MapToCatalogueModel);
        }

        public async Task UploadImageFromStream(long userId, string catalogueName, string description, Stream imageStream)
        {
            var path = $"{userId}/{Guid.NewGuid().ToString()}.jpg";
            var blobInfo = await _container.UploadBlobAsync(path, imageStream);
            var catalogue = await _context.Catalogues
                .FirstOrDefaultAsync(x => 
                    x.Name.Equals(catalogueName, StringComparison.OrdinalIgnoreCase) &&
                    x.UserId == userId);

            if(catalogue == null)
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    catalogue = new Catalogue
                    {
                        Name = catalogueName,
                        Images = new List<Image>(),
                        UserId = userId
                    };

                    await _context.Catalogues.AddAsync(catalogue);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }                   
            }

            catalogue.Images.Add(new Image
            {
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                Description = description,
                Url = $"{_baseUrl}/{path}"
            });

            await _context.SaveChangesAsync();
        }

        private CatalogueModel MapToCatalogueModel(Catalogue catalogue)
            => new CatalogueModel
            {
                Name = catalogue.Name,
                Images = catalogue.Images?.Select(image => new ImageModel
                {
                    DateCreated = image.DateCreated,
                    DateModified = image.DateModified,
                    Description = image.Description,
                    Id = image.Id,
                    Url = image.Url
                }).ToArray()
            };
    }
}
