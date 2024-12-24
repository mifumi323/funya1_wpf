using System.Windows;

namespace funya1_wpf
{
    public class Options
    {
        public ScreenSize ScreenSize = ScreenSize.Fixed;
        public WindowState WindowState = WindowState.Normal;
        public double WindowWidth = double.NaN;
        public double WindowHeight = double.NaN;

        public int Interval = 50;
        public int Gravity = 2;
        public bool Reverse = false;
    }
}
