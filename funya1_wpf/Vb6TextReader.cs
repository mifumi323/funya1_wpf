using System.Globalization;
using System.Text.RegularExpressions;

namespace funya1_wpf
{
    public partial class Vb6TextReader(string text)
    {
        private int index = 0;

        /// <summary>
        /// 文字列をカンマまたは改行区切りで読み込みます。
        /// </summary>
        public bool TryInputString(out string value)
        {
            if (index >= text.Length)
            {
                value = "";
                return false;
            }
            int start = index;
            int newLinePosition = text.IndexOf("\r\n", start);
            if (newLinePosition == -1)
            {
                newLinePosition = text.Length;
            }
            int commaPosition = text.IndexOf(',', start);
            if (commaPosition == -1)
            {
                commaPosition = text.Length;
            }
            int end;
            if (newLinePosition < commaPosition)
            {
                end = newLinePosition;
                index = newLinePosition + 2;
            }
            else
            {
                end = commaPosition;
                index = commaPosition + 1;
            }
            value = text[start..end];
            return true;
        }

        /// <summary>
        /// 文字列を整数として読み込みます。
        /// </summary>
        public bool TryInputInt(out int value)
        {
            if (!TryInputString(out string s))
            {
                value = 0;
                return false;
            }

            // VB6 の仕様に合わせて、&H で始まる文字列を16進数として解釈
            var match = HexadecimalRegex().Match(s.Trim());
            if (match.Success)
            {
                value = int.Parse(match.Groups[1].Value, NumberStyles.HexNumber);
                return true;
            }

            value = int.TryParse(s.Trim(), out int i) ? i : 0;
            return true;
        }

        [GeneratedRegex(@"^&H([0-9A-F]{1,8})&?$", RegexOptions.IgnoreCase, "ja-JP")]
        private static partial Regex HexadecimalRegex();
    }
}
