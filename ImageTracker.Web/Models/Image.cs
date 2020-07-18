using System.Collections.Generic;

namespace ImageTracker.Web.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<ImageTag> ImageTags { get; set; }
    }
}
