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
        public string Search { get; set; }
        public int Page { get; set; }
        public int NextPage { get { return Page + 1; } }
        public IList<Image> Images { get; set; }
    }
}
