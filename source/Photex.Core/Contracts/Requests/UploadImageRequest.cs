using Microsoft.AspNetCore.Http;

namespace Photex.Core.Contracts.Requests
{
    public class UploadImageRequest
    {
        public string Catalogue { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
    }
}
