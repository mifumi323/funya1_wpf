namespace funya1_wpf
{
    public class Vb6TextReader(string text)
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
            value = int.TryParse(s.Trim(), out int i) ? i : 0;
            return true;
        }
    }
}
