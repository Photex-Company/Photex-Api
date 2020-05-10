using System;

namespace Photex.Core.Contracts.Models
{
    public class ImageInCatalogueModel
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
