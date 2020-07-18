using System.Collections.Generic;

namespace ImageTracker.Web.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}
