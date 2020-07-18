namespace ImageTracker.Web.Models
{
    public class ImageTag
    {
        public int Id { get; set; }
        public int ImageId { get; set; }
        public Image Image { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
