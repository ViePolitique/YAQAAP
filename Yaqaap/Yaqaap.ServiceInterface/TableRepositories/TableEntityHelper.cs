using System.Globalization;
using System.Linq;
using System.Text;

namespace Yaqaap.ServiceInterface.TableRepositories
{
    public static class TableEntityHelper
    {
        public static string ToAzureKeyString(string str)
        {
            str = str.Replace("#", "sharp");
            str = str.Replace("/", "slash");
            str = str.Replace("\\", "backslash");
            str = str.Replace("?", "question-mark");

            var sb = new StringBuilder();
            foreach (var c in str
                .Where(c => !char.IsControl(c)))
                sb.Append(c);
            return sb.ToString();
        }

        public static string RemoveDiacritics(string str)
        {
            var normalizedString = str.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}