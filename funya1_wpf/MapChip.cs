using System.Windows.Media;

namespace funya1_wpf
{
    public class MapChip(byte[] pixelData)
    {
        public byte[] PixelData => pixelData;

        public Color GetAverageColor()
        {
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
    }
}
