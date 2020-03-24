namespace Photex.Core.Contracts.Requests
{
    public class UpdateImageRequest
    {
        public string Description { get; set; }
        public bool DescriptionSpecified { get; set; }
        public string Catalogue { get; set; }
        public bool CatalogueSpecified { get; set; }
    }
}
