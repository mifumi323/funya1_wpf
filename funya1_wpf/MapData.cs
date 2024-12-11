namespace funya1_wpf
{
    public class MapData
    {
        public string Title = "";
        public int StartX;
        public int StartY;
        public int Width;
        public int Height;
        public int[,] Data = new int[40, 40]; // 0 To 39, 0 To 39
        public int TotalFood;
        public ItemData[] Food = new ItemData[6]; // 1 To 5(0は使用しない)
    }
}
