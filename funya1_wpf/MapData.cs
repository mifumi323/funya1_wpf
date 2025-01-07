using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;

namespace funya1_wpf
{
    public class MapData : INotifyPropertyChanged
    {
        private string title = "";
        public string Title { get => title; set { title = value; OnPropertyChanged(); } }
        private int startX;
        public int StartX { get => startX; set { startX = value; OnPropertyChanged(); } }
        private int startY;
        public int StartY { get => startY; set { startY = value; OnPropertyChanged(); } }
        private int maxX;
        public int MaxX
        {
            get => maxX;
            set
            {
                maxX = value;
                if (maxX < 2)
                {
                    maxX = 2;
                }
                if (maxX > 39)
                {
                    maxX = 39;
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(Width));
            }
        }
        private int maxY;
        public int MaxY
        {
            get => maxY;
            set
            {
                maxY = value;
                if (maxY < 2)
                {
                    maxY = 2;
                }
                if (maxY > 39)
                {
                    maxY = 39;
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(Height));
            }
        }
        private int[,] data = new int[40, 40]; // 0 To 39, 0 To 39
        public int[,] Data { get => data; set { data = value; OnPropertyChanged(); } }
        private int totalFood;
        public int TotalFood { get => totalFood; set { totalFood = value; OnPropertyChanged(); } }
        private RangeArray<ItemData> food = new(1, 5, i => new ItemData());
        public RangeArray<ItemData> Food { get => food; set { food = value; OnPropertyChanged(); } }

        public int Width { get => MaxX + 1; set => MaxX = value - 1; }
        public int Height { get => MaxY + 1; set => MaxY = value - 1; }

        public void ImportLine(int y, string line)
        {
            for (int x = 0; x < line.Length; x++)
            {
                int n =
                    string.IsNullOrEmpty(line) ? 0 :
                    x >= line.Length ? 0 :
                    line[x] < '0' ? 0 :
                    line[x] > '9' ? 0 :
                    line[x] - '0';
                Data[x, y] = n;
            }
        }

        public void DrawTerrain(Panel Stage, CroppedBitmap?[] croppedBitmaps)
        {
            int terrainWidth = 32 * Width;
            Stage.Width = terrainWidth;
            int terrainHeight = 32 * Height;
            Stage.Height = terrainHeight;

            var terrainImage = new RenderTargetBitmap(terrainWidth, terrainHeight, 96, 96, PixelFormats.Pbgra32);
            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                for (int x = 0; x <= MaxX; x++)
                {
                    for (int y = 0; y <= MaxY; y++)
                    {
                        CroppedBitmap? imageSource = croppedBitmaps[Data[x, y]];
                        if (imageSource != null)
                        {
                            dc.DrawImage(imageSource, new System.Windows.Rect(x * 32, y * 32, 32, 32));
                        }
                    }
                }
            }
            terrainImage.Render(dv);

            Stage.Background = new ImageBrush(terrainImage);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
