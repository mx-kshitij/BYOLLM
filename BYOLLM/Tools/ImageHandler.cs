using System.Drawing;
using System.Drawing.Imaging;

namespace Odin
{
    public class ImageHandler
    {
        public static string? SaveBase64ToImage(string base64String, string path)
        {
            try
            {
                if (base64String.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
                {
                    int commaIndex = base64String.IndexOf(',');
                    base64String = base64String.Substring(commaIndex + 1);
                    byte[] imageBytes = Convert.FromBase64String(base64String);
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        Image image = Image.FromStream(ms, true);
                        string mime = ImageCodecInfo.GetImageEncoders().First(codec => codec.FormatID == image.RawFormat.Guid).MimeType;
                        image.Save($"{path}");
                        return mime;
                    }
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
