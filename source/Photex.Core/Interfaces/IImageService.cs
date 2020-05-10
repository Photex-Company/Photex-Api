using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Photex.Core.Contracts.Models;
using Photex.Core.Contracts.Requests;

namespace Photex.Core.Interfaces
{
    public interface IImageService
    {
        Task<ImageModel> GetImageFromUser(long userId, long imageId);
        Task<CatalogueModel> GetImagesFromCatalogue(long userId, long catalogueId);
        Task<IEnumerable<CatalogueModel>> GetImages(long userId);
        Task UploadImageFromStream(long userId, long catalogueId, string description, Stream imageStream);
        Task DeleteImage(long userId, long imageId);
        Task UpdateImage(long userId, long imageId, UpdateImageRequest request);
        Task UpdateMetadata(long userId, long imageId, UpdateMetadataRequest request);
    }
}
