using System.Collections.Generic;
using Photex.Core.Contracts.Metadata;

namespace Photex.Core.Contracts.Requests
{
    public class UpdateMetadataRequest
    {
        public class MetadataToEdit
        {
            public MetadataInfo.MetadataName Name { get; set; }
            public string NewValue { get; set; }
        }

        public IEnumerable<MetadataToEdit> NewMetadata { get; set; }
    }
}
