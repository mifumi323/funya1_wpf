using System.ComponentModel;
using System.Runtime.CompilerServices;

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
        private int width;
        public int Width { get => width; set { width = value; OnPropertyChanged(); } }
        private int height;
        public int Height { get => height; set { height = value; OnPropertyChanged(); } }
        private int[,] data = new int[40, 40]; // 0 To 39, 0 To 39
        public int[,] Data { get => data; set { data = value; OnPropertyChanged(); } }
        private int totalFood;
        public int TotalFood { get => totalFood; set { totalFood = value; OnPropertyChanged(); } }
        private RangeArray<ItemData> food = new(1, 5, i => new ItemData());
        public RangeArray<ItemData> Food { get => food; set { food = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
