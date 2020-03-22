using System.Collections.Generic;

namespace Photex.Database.Entities
{
    public class Catalogue
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long UserId { get; set; }
        public virtual ICollection<Image> Images { get; set; }
    }
}
