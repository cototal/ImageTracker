using ImageTracker.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTracker.Web.VMs
{
    public class ImageFormVM
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Category { get; set; }
        public string Tags { get; set; }
        public bool Rename { get; set; }

        public static ImageFormVM FromImage(Image image)
        {
            var tags = image.ImageTags == null ? null : string.Join(", ", image.ImageTags.Select(it => it.Tag.Name));
            return new ImageFormVM
            {
                Id = image.Id,
                Name = image.Name,
                Path = image.Path,
                Category = image.Category?.Name,
                Tags = tags,
                Rename = false
            };
        }
    }
}
