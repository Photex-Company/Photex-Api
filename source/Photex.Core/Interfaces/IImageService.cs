using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Photex.Core.Contracts.Models;
using Photex.Core.Contracts.Requests;

namespace Photex.Core.Interfaces
{
    public interface IImageService
    {
        Task<CatalogueModel> GetCatalogue(long userId, string name);
        Task<IEnumerable<CatalogueModel>> GetImages(long userId);
        Task<IEnumerable<string>> GetCatalogues(long userId);
        Task UploadImageFromStream(long userId, string catalogue, string description, Stream imageStream);
        Task DeleteImage(long userId, long imageId);
        Task UpdateImage(long userId, long imageId, UpdateImageRequest request);
    }
}
