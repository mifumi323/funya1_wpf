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
                if (friction > 20)
                {
                    friction = 20;
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
        public BitmapSource image = null!;
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
                if (Map[StageNumber].Width < 2 || 39 < Map[StageNumber].Width)
                {
                    return false;
                }
                if (!reader.TryInputInt(out Map[StageNumber].Height))
                {
                    return false;
                }
                if (Map[StageNumber].Height < 2 || 39 < Map[StageNumber].Height)
                {
                    return false;
                }
                if (!reader.TryInputInt(out Map[StageNumber].StartX))
                {
                    return false;
                }
                if (Map[StageNumber].StartX <= 0 || Map[StageNumber].Width <= Map[StageNumber].StartX)
                {
                    return false;
                }
                if (!reader.TryInputInt(out Map[StageNumber].StartY))
                {
                    return false;
                }
                if (Map[StageNumber].StartY <= 0 || Map[StageNumber].Height <= Map[StageNumber].StartY)
                {
                    return false;
                }
                if (!reader.TryInputInt(out Map[StageNumber].TotalFood))
                {
                    return false;
                }
                if (Map[StageNumber].TotalFood < 1 || 5 < Map[StageNumber].TotalFood)
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
            if (!reader.TryInputInt(out var endingType))
            {
                return false;
            }
            EndingType = endingType;

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

        public void Save()
        {
            // TODO: セーブ
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
