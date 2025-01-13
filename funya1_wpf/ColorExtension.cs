using System.Windows.Media;

namespace funya1_wpf
{
    public static class ColorExtension
    {
        /// <summary>指定された色から遠い色を取得します。</summary>
        public static Color FarColor(this Color color)
        {
            return IsDark(color) ? Colors.White : Colors.Black;
        }

        /// <summary>暗い色なら true を返します。</summary>
        public static bool IsDark(this Color color)
        {
            return color.R + color.G + color.B < 128 * 3;
        }
    }
}
