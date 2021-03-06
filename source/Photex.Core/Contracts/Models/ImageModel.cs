﻿using System;
using System.Collections.Generic;

namespace Photex.Core.Contracts.Models
{
    public class ImageModel
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public IDictionary<string, IDictionary<string, string>> Metadata { get; set; }
    }
}
