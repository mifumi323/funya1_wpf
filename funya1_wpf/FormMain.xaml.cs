using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace funya1_wpf
{
    /// <summary>
    /// Interaction logic for FormMain.xaml
    /// </summary>
    public partial class FormMain : Window
    {
        private Cleater cleater;

        private DispatcherTimer Timer1;
        private DispatcherTimer Timer2;

        public RangeArray<Image> Foods;

        public FormMain()
        {
            InitializeComponent();
            cleater = new Cleater(this);
            Timer1 = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(cleater.Options.Interval),
            };
            Timer2 = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(500),
            };
            Timer1.Tick += Timer1_Tick;
            Timer1.Start();
            Timer2.Tick += Timer2_Tick;
            Foods = new(1, 5);
            Foods[1] = Food1;
            Foods[2] = Food2;
            Foods[3] = Food3;
            Foods[4] = Food4;
            Foods[5] = Food5;

            cleater.GameStart();
        }

        private void MenuStage_Click(object sender, RoutedEventArgs e)
        {
            int stage = (int)((MenuItem)sender).Tag;
            cleater.GameStart();
            cleater.StartStage(stage);
        }

        private void MenuReverse_Click(object sender, RoutedEventArgs e)
        {
            cleater.Options.Reverse = !cleater.Options.Reverse;
            ((MenuItem)sender).IsChecked = cleater.Options.Reverse;
        }

        private void MenuPause_Click(object sender, RoutedEventArgs e)
        {
            if (cleater.GameState == GameState.Playing)
            {
                cleater.Pause();
            }
            else if (cleater.GameState == GameState.Paused)
            {
                cleater.ResumeGame();
            }
        }

        private void GameExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuStart_Click(object sender, RoutedEventArgs e)
        {
            cleater.GameStart();
        }

        private void Death_Click(object sender, RoutedEventArgs e)
        {
            if (cleater.GameState == GameState.Playing)
            {
                cleater.Die();
            }
        }

        private void Timer2_Tick(object? sender, EventArgs e)
        {
            // TODO: 実装
        }

        private void MenuMusic_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 実装
        }

        private void Timer1_Tick(object? sender, EventArgs e)
        {
            cleater.MainLoop();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                if (cleater.GameState == GameState.Playing)
                {
                    cleater.Pause();
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // TODO: 実装
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            cleater.OnKeyDown(e.Key);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            // TODO: 実装
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 実装
        }

        private void Speed_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 実装
        }

        private void Gravity_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 実装
        }

        private void Mine_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // TODO: 実装
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 実装
        }

        private void HelpContents_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 実装
        }

        public void UpdateMenuStage()
        {
            if (cleater.Secrets.StageSelect)
            {
                MenuStage.Visibility = Visibility.Visible;
                MenuStage.Items.Clear();
                for (int i = 1; i <= cleater.StageCount; i++)
                {
                    var map = cleater.Map[i];
                    MenuItem newItem = new MenuItem()
                    {
                        Header = map.Title,
                        Tag = i,
                    };
                    newItem.Click += MenuStage_Click;
                    MenuStage.Items.Add(newItem);
                }
            }
            else
            {
                MenuStage.Visibility = Visibility.Collapsed;
            }
        }
    }
}