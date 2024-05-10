using System.Text.RegularExpressions;
using System.Text;

namespace DHungBooks.Helper
{
    public class Utilities
    {
        public static string StripHTML(string input)
        {
            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    return Regex.Replace(input, "<.*?>", String.Empty);
                }
            }
            catch
            {
                return null;
            }
            return null;
        }
        public static bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
        public static void CreateIfMissing(string path)
        {
            bool exist = Directory.Exists(path);
            if (!exist)
                Directory.CreateDirectory(path);
        }
        public static string GetRandomKey(int length = 5)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder randomKey = new StringBuilder();

            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                randomKey.Append(chars[random.Next(chars.Length)]);
            }

            return randomKey.ToString();
        }
        public static async Task<string> UploadFile(Microsoft.AspNetCore.Http.IFormFile file, string sDirectory, string newName)
        {
            try
            {
                if (newName == null) newName = file.FileName;
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", sDirectory);
                CreateIfMissing(path);
                string pathFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", sDirectory, newName);
                var supportedTypes = new[] { "jpg", "png", "jpeg", "PNG", "JPG", "JPEG", "gif" };
                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt.ToLower()))
                {
                    return null;
                }
                else
                {
                    using (var stream = new FileStream(pathFile, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return newName;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}

