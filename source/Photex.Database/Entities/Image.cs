using System;

namespace Photex.Database.Entities
{
    public class Image
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public virtual Catalogue Catalogue { get; set; }
    }
}
