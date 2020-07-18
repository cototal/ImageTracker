using ImageTracker.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTracker.Web.VMs
{
    public class HomeVM
    {
        public HomeVM(string search, int page, IList<Image> images)
        {
            Search = string.IsNullOrWhiteSpace(search) ? null : search;
            Page = page;
            NextPage = page + 1;
            Images = images;
            var dict = new Dictionary<int, ImageFormVM>();
            foreach (var image in Images)
            {
                dict[image.Id] = ImageFormVM.FromImage(image);
            }
            ImageForms = dict;


        }
        public string Search { get; set; }
        public int Page { get; set; }
        public int NextPage { get; private set; }
        public IList<Image> Images { get; set; }

        public Dictionary<int, ImageFormVM> ImageForms { get; private set; }
    }
}
