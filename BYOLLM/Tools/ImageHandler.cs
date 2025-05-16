using FileSignatures;
using Microsoft.AspNetCore.StaticFiles;
//using MimeDetective;

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
                    File.WriteAllBytes(path, imageBytes);
                    //var contentType = new FileExtensionContentTypeProvider().TryGetContentType(path, out var type) ? type : "image/png";
                    var inspector = new FileFormatInspector();
                    var format = inspector.DetermineFileFormat(new MemoryStream(imageBytes));
                    return format.ToString();
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
