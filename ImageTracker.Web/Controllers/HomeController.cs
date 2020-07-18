using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ImageTracker.Web.VMs;
using System.IO;
using ImageTracker.Web.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ImageTracker.Web.Models;
using System.Threading.Tasks;
using System;

namespace ImageTracker.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ImageTrackerContext _db;

        public HomeController(ILogger<HomeController> logger, ImageTrackerContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index(string search, int page = 1)
        {
            var skip = (page - 1) * 100;
            IQueryable<Image> query;
            if (search == null)
            {
                query = _db.Images.Where(i => 1 == 1);
            } else
            {
                search = $"%{search}%";
                FormattableString searchString = $@"SELECT Images.* FROM Images
                    LEFT JOIN ImageTags ON Images.Id = ImageTags.ImageId
                    LEFT JOIN Tags ON Tags.Id = ImageTags.TagId
                    LEFT JOIN categories ON Images.CategoryId = Categories.Id
                    WHERE path LIKE {search}
                    OR Tags.Name LIKE {search}
                    OR Categories.Name LIKE {search}";
                query = _db.Images.FromSqlInterpolated(searchString);
            }
            var images = await query.OrderBy(i => i.Id).Skip(skip).Take(100).ToListAsync();

            return View(new HomeVM {
                Images = images,
                Page = page,
                Search = string.IsNullOrWhiteSpace(search) ? null : search
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddDirectory()
        {
            var directory = HttpContext.Request.Form["directory"];
            var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var match = await _db.Images.FirstOrDefaultAsync(i => i.Path == file);
                if (match == null)
                {
                    _db.Images.Add(new Image { Path = file });
                }
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ShowFile(int id)
        {
            var image = await _db.Images.FindAsync(id);
            var path = image.Path;
            if (System.IO.File.Exists(path))
            {
                var ext = Path.GetExtension(image.Path);
                var name = Path.GetFileName(image.Path);
                var mime = ext == ".jpg" ? "image/jpeg" : "image/png";
                return PhysicalFile(image.Path, mime, name);
            } else
            {
                return StatusCode(204);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
