﻿using System.ComponentModel;
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
        private int maxX;
        public int MaxX { get => maxX; set { maxX = value; OnPropertyChanged(); OnPropertyChanged(nameof(Width)); } }
        private int maxY;
        public int MaxY { get => maxY; set { maxY = value; OnPropertyChanged(); OnPropertyChanged(nameof(Height)); } }
        private int[,] data = new int[40, 40]; // 0 To 39, 0 To 39
        public int[,] Data { get => data; set { data = value; OnPropertyChanged(); } }
        private int totalFood;
        public int TotalFood { get => totalFood; set { totalFood = value; OnPropertyChanged(); } }
        private RangeArray<ItemData> food = new(1, 5, i => new ItemData());
        public RangeArray<ItemData> Food { get => food; set { food = value; OnPropertyChanged(); } }

        public int Width { get => MaxX + 1; set => MaxX = value - 1; }
        public int Height { get => MaxY + 1; set => MaxY = value - 1; }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
