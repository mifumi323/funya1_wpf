using System.IO;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace funya1_wpf
{
    public class StageData(BitmapSource BlockData1)
    {
        public string StageFile = "";

        public RangeArray<MapData> Map = new(0, 30, i => new MapData()); // 1~30: 通常マップ、0: 演出用マップ
        public string[,] MapText = new string[31, 40]; // 0 To 30, 0 To 39

        public int StageCount;
        public int Friction;
        public int RestMax;
        public int EndingType;

        public Color StageColor = Color.FromRgb(0, 0, 0);
        public BitmapSource Image = null!;
        public CroppedBitmap?[] croppedBitmaps = new CroppedBitmap?[10];

        public void SetStage()
        {
            foreach (var map in Map)
            {
                var StageNumber = map.Key;
                var MapValue = map.Value;
                for (int x = 0; x <= MapValue.Width; x++)
                {
                    for (int y = 0; y <= MapValue.Height; y++)
                    {
                        int n =
                            string.IsNullOrEmpty(MapText[StageNumber, y]) ? 0 :
                            x >= MapText[StageNumber, y].Length ? 0 :
                            MapText[StageNumber, y][x] < '0' ? 0 :
                            MapText[StageNumber, y][x] > '9' ? 0 :
                            MapText[StageNumber, y][x] - '0';
                        MapValue.Data[x, y] = n;
                    }
                }
            }
        }

        public bool LoadFile()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var fileText = File.ReadAllText(StageFile, Encoding.GetEncoding(932));
            var reader = new Vb6TextReader(fileText);
            if (!reader.TryInputInt(out Friction))
            {
                return false;
            }
            if (!reader.TryInputString(out var MapBank))
            {
                return false;
            }
            var MapBankPath = Path.Combine(Path.GetDirectoryName(StageFile)!, MapBank);
            if (File.Exists(MapBankPath))
            {
                using var stream = File.OpenRead(MapBankPath);
                Image = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                LoadCroppedBitmaps();
            }
            else
            {
                LoadSampleImage();
            }
            if (!reader.TryInputInt(out StageCount))
            {
                return false;
            }
            if (StageCount < 1 || StageCount > 30)
            {
                return false;
            }
            if (!reader.TryInputInt(out RestMax))
            {
                return false;
            }
            if (!reader.TryInputInt(out int ColorValue))
            {
                return false;
            }
            StageColor = Color.FromRgb((byte)(ColorValue & 0xFF), (byte)((ColorValue >> 8) & 0xFF), (byte)((ColorValue >> 16) & 0xFF));
            for (int StageNumber = 1; StageNumber <= StageCount; StageNumber++)
            {
                if (!reader.TryInputString(out Map[StageNumber].Title))
                {
                    return false;
                }
                if (!reader.TryInputInt(out Map[StageNumber].Width))
                {
                    return false;
                }
                if (!reader.TryInputInt(out Map[StageNumber].Height))
                {
                    return false;
                }
                if (!reader.TryInputInt(out Map[StageNumber].StartX))
                {
                    return false;
                }
                if (!reader.TryInputInt(out Map[StageNumber].StartY))
                {
                    return false;
                }
                if (!reader.TryInputInt(out Map[StageNumber].TotalFood))
                {
                    return false;
                }
                for (int i = 1; i <= Map[StageNumber].TotalFood; i++)
                {
                    if (!reader.TryInputInt(out Map[StageNumber].Food[i].x))
                    {
                        return false;
                    }
                    if (!reader.TryInputInt(out Map[StageNumber].Food[i].y))
                    {
                        return false;
                    }
                }
                for (int y = 0; y <= Map[StageNumber].Height; y++)
                {
                    if (!reader.TryInputString(out MapText[StageNumber, y]))
                    {
                        return false;
                    }
                }
            }
            if (!reader.TryInputInt(out EndingType))
            {
                return false;
            }

            return true;
        }

        public void ResetStage()
        {
            foreach (var map in Map)
            {
                for (int x = 0; x <= 39; x++)
                {
                    for (int y = 0; y <= 39; y++)
                    {
                        map.Value.Data[x, y] = 0;
                    }
                }
            }
        }

        public void LoadSampleImage()
        {
            Image = BlockData1;
            LoadCroppedBitmaps();
        }

        public void LoadCroppedBitmaps()
        {
            for (int i = 0; i < 10; i++)
            {
                croppedBitmaps[i] = (i + 1) * 32 <= Image.PixelWidth ? new(Image, new(i * 32, 0, 32, 32)) : null;
            }
        }
    }
}
