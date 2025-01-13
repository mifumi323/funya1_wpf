using System.Globalization;
using System.Text;
using System.Windows.Controls;

namespace funya1_wpf
{
    public class ShiftJisTextRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is not string unicodeText)
            {
                return new ValidationResult(false, "文字列を入力してください");
            }
            var sjis = Encoding.GetEncoding(932);
            var bytes = sjis.GetBytes(unicodeText);
            var sjisText = sjis.GetString(bytes);
            return sjisText == unicodeText
                ? ValidationResult.ValidResult
                : new ValidationResult(false, "Shift-JISで表現できない文字が含まれています");
        }
    }
}
