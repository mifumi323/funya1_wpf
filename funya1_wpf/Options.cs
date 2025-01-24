using System.Windows;

namespace funya1_wpf
{
    public class Options
    {
        public ScreenSize ScreenSize = ScreenSize.Fixed;
        public WindowState WindowState = WindowState.Normal;
        public double WindowWidth = double.NaN;
        public double WindowHeight = double.NaN;

        public WindowState StageMakerState = WindowState.Normal;
        public double StageMakerWidth = double.NaN;
        public double StageMakerHeight = double.NaN;
        public int StageMakerZoom = 100;

        public bool ToolBarVisible = true;

        public int Interval = 50;
        public int Gravity = 2;
        public bool Reverse = false;
    }
}
