using System.Text;
using System.Text.RegularExpressions;

namespace DHungBooks.Extensions
{
    public static class Ext
    {
        public static string toVND(this double donGia)
        {
            return donGia.ToString("#,##0") + "VNĐ";
        }
        public static string ToTitleCase(string str)
        {
            string result = str;
            if(!string.IsNullOrEmpty(str)) {
                var words = str.Split(' ');
                for(int i = 0; i < words.Length; i++)
                {
                    var s = words[i];
                    if(s.Length > 0)
                    {
                        words[i] = s[0].ToString().ToUpper() + s.Substring(1);
                    }
                }
                result = string.Join(" ",words);
            }
            return result;
        }

        public static string ToUrlFriendly(this string url)
        {
            // Loại bỏ các ký tự đặc biệt
            string cleanedUrl = Regex.Replace(url, @"[^\w\d\s]", "");

            // Chuyển khoảng trắng thành dấu gạch ngang
            cleanedUrl = cleanedUrl.Replace(" ", "-");

            // Chuyển đổi chữ cái thành chữ thường
            cleanedUrl = cleanedUrl.ToLower();

            // Chuyển đổi ký tự tiếng Việt có dấu thành ký tự không dấu
            cleanedUrl = cleanedUrl.Normalize(NormalizationForm.FormD);
            cleanedUrl = Regex.Replace(cleanedUrl, @"[^a-zA-Z0-9\s-]", "");

            return cleanedUrl;
        }

    }
}
