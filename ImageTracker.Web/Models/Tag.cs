using System.Collections.Generic;

namespace ImageTracker.Web.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ImageTag> ImageTags { get; set; }
    }
}
