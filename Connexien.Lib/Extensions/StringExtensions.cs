using System.Diagnostics.Contracts;
using System.IO;

namespace Connexien.Lib.Extensions
{
    public static class StringExtensions
    {
        public static string Left(this string thisString, int count)
        {

            Contract.Requires(thisString != null);
            Contract.Requires(count >= 0);
            Contract.Requires(count <= thisString.Length);

            Contract.Ensures(Contract.Result<string>() != null);
            Contract.Ensures(Contract.Result<string>().Length <= count);

            if (thisString.Length <= count)
            {
                return thisString;
            }
            
            return thisString.Substring(0, count);
        }

        public static string GetSizedImageName(this string imageName, int width, int height)
        {
            return $"{Path.GetFileNameWithoutExtension(imageName)}-{width}x{height}{Path.GetExtension(imageName)}";
        }
    }
}
