using System.Windows.Media.Imaging;

namespace funya1_wpf
{
    public static class BitmapSourceExtension
    {
        public static byte[] GetPixelData(this BitmapSource source)
        {
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;
            byte[] pixelData = new byte[source.PixelHeight * stride];
            source.CopyPixels(pixelData, stride, 0);
            return pixelData;
        }

        public static bool IsValidMapChipSize(this BitmapSource source)
        {
            return source.PixelWidth >= 96 && source.PixelHeight >= 32;
        }
    }
}
