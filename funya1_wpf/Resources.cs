using System.Windows;
using System.Windows.Media.Imaging;

namespace funya1_wpf
{
    public class Resources
    {
        public BitmapSource Banana { get; } = LoadBitmapSource("banana.png");
        public BitmapSource BlockData1 { get; } = LoadBitmapSource("BlockData1.bmp");
        public BitmapSource Death { get; } = LoadBitmapSource("Death.png");
        public BitmapSource Fall { get; } = LoadBitmapSource("Fall.png");
        public BitmapSource FallG { get; } = LoadBitmapSource("FallG.png");
        public BitmapSource FallL { get; } = LoadBitmapSource("FallL.png");
        public BitmapSource FallLG { get; } = LoadBitmapSource("FallLG.png");
        public BitmapSource FallR { get; } = LoadBitmapSource("FallR.png");
        public BitmapSource FallRG { get; } = LoadBitmapSource("FallRG.png");
        public BitmapSource funyaG { get; } = LoadBitmapSource("funyaG.png");
        public BitmapSource Happy { get; } = LoadBitmapSource("Happy.png");
        public BitmapSource Jump { get; } = LoadBitmapSource("Jump.png");
        public BitmapSource JumpG { get; } = LoadBitmapSource("JumpG.png");
        public BitmapSource JumpL { get; } = LoadBitmapSource("JumpL.png");
        public BitmapSource JumpLG { get; } = LoadBitmapSource("JumpLG.png");
        public BitmapSource JumpR { get; } = LoadBitmapSource("JumpR.png");
        public BitmapSource JumpRG { get; } = LoadBitmapSource("JumpRG.png");
        public BitmapSource RunLA { get; } = LoadBitmapSource("RunLA.png");
        public BitmapSource RunLB { get; } = LoadBitmapSource("RunLB.png");
        public BitmapSource RunLC { get; } = LoadBitmapSource("RunLC.png");
        public BitmapSource RunRA { get; } = LoadBitmapSource("RunRA.png");
        public BitmapSource RunRB { get; } = LoadBitmapSource("RunRB.png");
        public BitmapSource RunRC { get; } = LoadBitmapSource("RunRC.png");
        public BitmapSource Sit { get; } = LoadBitmapSource("Sit.png");
        public BitmapSource Sleep { get; } = LoadBitmapSource("Sleep.png");
        public BitmapSource Sleep2 { get; } = LoadBitmapSource("Sleep2.png");
        public BitmapSource Stand { get; } = LoadBitmapSource("Stand.png");
        public BitmapSource WalkL { get; } = LoadBitmapSource("WalkL.png");
        public BitmapSource WalkR { get; } = LoadBitmapSource("WalkR.png");
        public BitmapSource Wink { get; } = LoadBitmapSource("Wink.png");

        private static BitmapFrame LoadBitmapSource(string filename)
        {
            var uri = new Uri($"/Resources/{filename}", UriKind.Relative);
            var info = Application.GetResourceStream(uri);
            return BitmapFrame.Create(info.Stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        }
    }
}
