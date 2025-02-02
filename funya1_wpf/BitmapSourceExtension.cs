using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace funya1_wpf
{
    public static class BitmapSourceExtension
    {
        public static Color GetAverageColor(this BitmapSource source)
        {
            var bgr32Image = new FormatConvertedBitmap(source, PixelFormats.Bgr32, null, 0);
            var pixelData = bgr32Image.GetPixelData();
            int rSum = 0, gSum = 0, bSum = 0;
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                bSum += pixelData[i];
                gSum += pixelData[i + 1];
                rSum += pixelData[i + 2];
            }
            var coefficient = 1.0 / (pixelData.Length / 4);
            return Color.FromArgb(255, (byte)(rSum * coefficient), (byte)(gSum * coefficient), (byte)(bSum * coefficient));
        }

        public static byte[] GetPixelData(this BitmapSource source)
        {
            int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;
            byte[] pixelData = new byte[source.PixelHeight * stride];
            source.CopyPixels(pixelData, stride, 0);
            return pixelData;
        }
    }
}
