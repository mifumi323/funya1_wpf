using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace funya1_wpf
{
    public class StageData(BitmapSource BlockData1) : INotifyPropertyChanged
    {
        public string StageFile = "";

        private RangeArray<MapData> map = new(0, 30, i => new MapData()); // 1~30: 通常マップ、0: 演出用マップ
        public RangeArray<MapData> Map { get => map; set { map = value; OnPropertyChanged(); } }
        private string[,] mapText = new string[31, 40]; // 0 To 30, 0 To 39
        public string[,] MapText { get => mapText; set { mapText = value; OnPropertyChanged(); } }

        private int stageCount;
        public int StageCount { get => stageCount; set { stageCount = value; OnPropertyChanged(); } }
        private int friction;
        public int Friction
        {
            get => friction;
            set
            {
                friction = value;
                if (friction < 0)
                {
                    friction = 0;
                }
                if (friction > 100)
                {
                    friction = 100;
                }
                OnPropertyChanged();
            }
        }
        private int restMax;
        public int RestMax { get => restMax; set { restMax = value; OnPropertyChanged(); } }
        private int endingType;
        public int EndingType { get => endingType; set { endingType = value; OnPropertyChanged(); } }

        private Color stageColor = Color.FromRgb(0, 0, 0);
        public Color StageColor { get => stageColor; set { stageColor = value; OnPropertyChanged(); } }
        private string imagePath = "";
        public string ImagePath
        {
            get => imagePath;
            set
            {
                imagePath = value;
                var MapBankPath = Path.Combine(Path.GetDirectoryName(StageFile)!, imagePath);
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
                OnPropertyChanged();
            }
        }
        private BitmapSource image = null!;
        public BitmapSource Image { get => image; set { image = value; OnPropertyChanged(); } }
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
            if (!reader.TryInputInt(out var friction))
            {
                return false;
            }
            Friction = friction;
            if (!reader.TryInputString(out var MapBank))
            {
                return false;
            }
            ImagePath = MapBank;
            if (!reader.TryInputInt(out var stageCount))
            {
                return false;
            }
            StageCount = stageCount;
            if (StageCount < 1 || StageCount > 30)
            {
                return false;
            }
            if (!reader.TryInputInt(out var restMax))
            {
                return false;
            }
            RestMax = restMax;
            if (RestMax < 1)
            {
                return false;
            }
            if (!reader.TryInputInt(out int ColorValue))
            {
                return false;
            }
            StageColor = IntToColor(ColorValue);
            for (int StageNumber = 1; StageNumber <= StageCount; StageNumber++)
            {
                if (!reader.TryInputString(out var title))
                {
                    return false;
                }
                Map[StageNumber].Title = title;
                if (!reader.TryInputInt(out var width))
                {
                    return false;
                }
                if (width < 2 || 39 < width)
                {
                    return false;
                }
                Map[StageNumber].Width = width;
                if (!reader.TryInputInt(out var height))
                {
                    return false;
                }
                if (height < 2 || 39 < height)
                {
                    return false;
                }
                Map[StageNumber].Height = height;
                if (!reader.TryInputInt(out var startX))
                {
                    return false;
                }
                if (startX <= 0 || width <= startX)
                {
                    return false;
                }
                Map[StageNumber].StartX = startX;
                if (!reader.TryInputInt(out var startY))
                {
                    return false;
                }
                if (startY <= 0 || height <= startY)
                {
                    return false;
                }
                Map[StageNumber].StartY = startY;
                if (!reader.TryInputInt(out var totalFood))
                {
                    return false;
                }
                if (totalFood < 1 || 5 < totalFood)
                {
                    return false;
                }
                Map[StageNumber].TotalFood = totalFood;
                for (int i = 1; i <= totalFood; i++)
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
            if (!reader.TryInputInt(out var endingType))
            {
                return false;
            }
            EndingType = endingType;

            return true;
        }

        private static Color IntToColor(int ColorValue)
        {
            return Color.FromRgb((byte)(ColorValue & 0xFF), (byte)((ColorValue >> 8) & 0xFF), (byte)((ColorValue >> 16) & 0xFF));
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

        public void Save()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Friction.ToString());
            sb.AppendLine(ImagePath);
            sb.AppendLine($"{StageCount},{RestMax}");
            sb.AppendLine(ColorToInt(StageColor).ToString());
            for (int StageNumber = 1; StageNumber <= StageCount; StageNumber++)
            {
                sb.AppendLine(Map[StageNumber].Title);
                sb.AppendLine($"{Map[StageNumber].Width},{Map[StageNumber].Height},{Map[StageNumber].StartX},{Map[StageNumber].StartY}");
                sb.AppendLine(Map[StageNumber].TotalFood.ToString());
                for (int i = 1; i <= Map[StageNumber].TotalFood; i++)
                {
                    sb.AppendLine($"{Map[StageNumber].Food[i].x},{Map[StageNumber].Food[i].y}");
                }
                for (int y = 0; y <= Map[StageNumber].Height; y++)
                {
                    var line = new StringBuilder();
                    for (int x = 0; x <= Map[StageNumber].Width; x++)
                    {
                        var chipNumber = Map[StageNumber].Data[x, y];
                        var chipChar = chipNumber == 0 && x > 0 ? ' ' : (char)('0' + chipNumber);
                        line.Append(chipChar);
                    }
                    sb.AppendLine(line.ToString().TrimEnd());
                }
            }
            sb.AppendLine(EndingType == 0 ? "End" : EndingType.ToString());

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            File.WriteAllText(StageFile, sb.ToString(), Encoding.GetEncoding(932));
        }

        private static int ColorToInt(Color color)
        {
            return color.R + (color.G << 8) + (color.B << 16);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
