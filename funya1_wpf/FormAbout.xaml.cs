using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace funya1_wpf
{
    /// <summary>
    /// FormAbout.xaml の相互作用ロジック
    /// </summary>
    public partial class FormAbout : Window
    {
        public required Resources resources { get; init; }
        private readonly ElapsedFrameCounter frameCounter1;
        private readonly Random random = new();

        private Status status = Status.Standing;
        private int frame = 0;

        public FormAbout()
        {
            InitializeComponent();
            frameCounter1 = new(20);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            var frames1 = frameCounter1.GetElapsedFrames(true);
            if (frames1 > 0)
            {
                for (int i = 0; i < frames1; i++)
                {
                    MainLoop();
                }
            }
        }

        private void MainLoop()
        {
            if (frame > 0 && frame % 40 == 0)
            {
                var r = random.Next(15);
                switch (r)
                {
                    case 0:
                        status = Status.Standing;
                        break;
                    case 1:
                        status = Status.Sitting;
                        ChangeMineImage(resources.Sit);
                        break;
                    case 2:
                        status = Status.WalkingL;
                        ChangeMineImage(resources.WalkL);
                        break;
                    case 3:
                        status = Status.WalkingR;
                        ChangeMineImage(resources.WalkR);
                        break;
                    case 4:
                        status = Status.JumpingUp;
                        ChangeMineImage(resources.Jump);
                        break;
                    case 5:
                        status = Status.JumpingUp;
                        ChangeMineImage(resources.JumpL);
                        break;
                    case 6:
                        status = Status.JumpingUp;
                        ChangeMineImage(resources.JumpR);
                        break;
                    case 7:
                        status = Status.JumpingDown;
                        ChangeMineImage(resources.Fall);
                        break;
                    case 8:
                        status = Status.JumpingDown;
                        ChangeMineImage(resources.FallL);
                        break;
                    case 9:
                        status = Status.JumpingDown;
                        ChangeMineImage(resources.FallR);
                        break;
                    case 10:
                        status = Status.RunningL;
                        break;
                    case 11:
                        status = Status.RunningR;
                        break;
                    case 12:
                        status = Status.Slepping;
                        ChangeMineImage(resources.Sleep);
                        break;
                    case 13:
                        status = Status.Smile;
                        ChangeMineImage(resources.Happy);
                        break;
                    case 14:
                        status = Status.Smile;
                        ChangeMineImage(resources.Death);
                        break;
                }
            }
            switch (status)
            {
                case Status.Standing:
                    ChangeMineImage(random.Next(100) < 2 ? resources.Wink : resources.Stand);
                    break;
                case Status.RunningL:
                    ChangeMineImage(resources.RunL[frame % 3]);
                    break;
                case Status.RunningR:
                    ChangeMineImage(resources.RunR[frame % 3]);
                    break;
                case Status.Slepping:
                    ChangeMineImage(frame % 40 < 20 ? resources.Sleep : resources.Sleep2);
                    break;
            }
            frame++;
        }

        private void ChangeMineImage(ImageSource image)
        {
            if (image != MineImage.Source)
            {
                MineImage.Source = image;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}
