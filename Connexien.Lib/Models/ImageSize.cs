
namespace Connexien.Lib.Models
{
    public class ImageSize
    {
        public int Height { get; set; }
        public int Width { get; set; }
    }

    public class ResizedImage : ImageSize
    {
        public string FileName { get; set; }
    }
}
