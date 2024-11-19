using Connexien.Lib.Models;
using System.Collections.Generic;
using System.IO;

namespace Connexien.Lib.ProfileImages
{
    public static class ProfileImageUtilities
    {
        public static ResizedImage[] GetResizedImageFiles(string fileName)
        {
            List<ResizedImage> result = new List<ResizedImage>();
            List<ImageSize> imageSizes = new List<ImageSize>()
            {
                new ImageSize() { Height = 80, Width = 80 },
                new ImageSize() { Height = 80, Width = 200 },
                new ImageSize() { Height = 150, Width = 150 }
            };

            foreach (var size in imageSizes)
            {
                result.Add(
                    new ResizedImage()
                    {
                        FileName = $"{Path.GetFileNameWithoutExtension(fileName)}-{size.Width}x{size.Height}{Path.GetExtension(fileName)}",
                        Height = size.Height,
                        Width = size.Width
                    });
            }
            
            return result.ToArray();
        }
    }
}
