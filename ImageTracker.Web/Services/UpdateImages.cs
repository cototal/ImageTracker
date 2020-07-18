using ImageTracker.Web.Data;
using ImageTracker.Web.Models;
using ImageTracker.Web.VMs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTracker.Web.Services
{
    public class UpdateImages
    {
        private readonly ImageTrackerContext _db;

        public List<Category> PendingCategories { get; private set; } = new List<Category>();
        public List<Tag> PendingTags { get; private set; } = new List<Tag>();

        public UpdateImages(ImageTrackerContext db)
        {
            _db = db;
        }

        public async Task<List<Image>> UpdateFromForm(Dictionary<int, ImageFormVM> imageForms)
        {
            var keys = imageForms.Keys.ToArray();
            var images = await _db.Images
                .Include(i => i.Category)
                .Include(i => i.ImageTags).ThenInclude(it => it.Tag)
                .Where(i => keys.Contains(i.Id)).ToListAsync();
            // TODO: Handle renaming
            foreach (var imageFormEntry in imageForms)
            {
                var imgForm = imageFormEntry.Value;
                var img = images.Find(i => i.Id == imgForm.Id);
                if (img == null)
                {
                    continue;
                }
                img.Name = imgForm.Name;
                await SetCategory(img, imgForm);
                await SetTags(img, imgForm);
                if (string.IsNullOrWhiteSpace(imgForm.Tags) && img.ImageTags != null)
                {
                    _db.ImageTags.RemoveRange(img.ImageTags);
                }
            }
            await _db.SaveChangesAsync();
            PendingCategories = new List<Category>();
            PendingTags = new List<Tag>();
            return images;
        }

        private async Task SetTags(Image img, ImageFormVM imgForm)
        {
            if (string.IsNullOrWhiteSpace(imgForm.Tags))
            {
                if (img.ImageTags == null)
                {
                    return;
                } else
                {
                    _db.ImageTags.RemoveRange(img.ImageTags);
                }
            } else
            {
                var currentTags = img.ImageTags == null ? new string[] { } : img.ImageTags.Select(it => it.Tag.Name.ToLower());
                var formTags = imgForm.Tags.Split(",").Select(t => t.Trim().ToLower());
                var tagsToAdd = new List<string>();
                var tagsToRemove = new List<string>();
                foreach (var tag in currentTags)
                {
                    if (!formTags.Contains(tag))
                    {
                        tagsToRemove.Add(tag);
                    }
                }
                foreach (var tag in formTags)
                {
                    if (!currentTags.Contains(tag))
                    {
                        tagsToAdd.Add(tag);
                    }
                }
                foreach (var tag in tagsToAdd)
                {
                    var tagRec = PendingTags.Find(t => t.Name.ToLower() == tag);
                    if (tagRec == null)
                    {
                        tagRec = await _db.Tags.FirstOrDefaultAsync(t => t.Name == tag);
                        if (tagRec == null)
                        {
                            tagRec = new Tag { Name = tag };
                            PendingTags.Add(tagRec);
                            _db.Tags.Add(tagRec);
                        }
                    }
                    var imgTag = new ImageTag { Image = img, Tag = tagRec };
                    _db.ImageTags.Add(imgTag);
                }
                foreach (var tag in tagsToRemove)
                {
                    var imgTag = await _db.ImageTags.Include(it => it.Tag).FirstOrDefaultAsync(it => it.ImageId == img.Id && it.Tag.Name == tag);
                    if (imgTag != null)
                    {
                        _db.ImageTags.Remove(imgTag);
                    }
                }
            }
        }

        private async Task SetCategory(Image img, ImageFormVM imgForm)
        {
            if (string.IsNullOrWhiteSpace(imgForm.Category))
            {
                img.Category = null;
            }
            else
            {
                var category = PendingCategories.Find(c => c.Name.ToLower() == imgForm.Category.ToLower());
                if (category != null)
                {
                    img.Category = category;
                    return;
                }
                category = await _db.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == imgForm.Category.ToLower());
                if (category == null)
                {
                    category = new Category { Name = imgForm.Category };
                    PendingCategories.Add(category);
                    _db.Categories.Add(category);
                }
                img.Category = category;
            }
        }
    }
}
