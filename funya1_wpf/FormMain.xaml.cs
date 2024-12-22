using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace funya1_wpf
{
    /// <summary>
    /// Interaction logic for FormMain.xaml
    /// </summary>
    public partial class FormMain : Window
    {
        private readonly Cleater cleater;

        readonly ElapsedFrameCounter frameCounter1;
        readonly ElapsedFrameCounter frameCounter2;

        public RangeArray<Image> Foods;

        private int CountDown = 10;
        private DispatcherTimer? MessageTimer = null;
        private readonly Queue<Message> MessageQueue = new();
        public Action<bool>? OnMessageClose;

        private Point PreviousMousePosition;
        private DateTime LastMouseMoved = DateTime.Now;
        private readonly DispatcherTimer? MouseHideTimer = new() { Interval = TimeSpan.FromSeconds(0.5) };

        public FormMain()
        {
            InitializeComponent();
            MouseHideTimer.Tick += MouseHideTimer_Tick;
            MouseHideTimer.Start();

            cleater = new Cleater(this);
            frameCounter1 = new(1000 / cleater.Options.Interval);
            frameCounter2 = new(2);
            Foods = new(1, 5);
            Foods[1] = Food1;
            Foods[2] = Food2;
            Foods[3] = Food3;
            Foods[4] = Food4;
            Foods[5] = Food5;

            cleater.GameStart();
            CloseMessage();
            ShowMessage("ふにゃ", MessageMode.Info, "Enter/クリックで進む");
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
            if (MessageButton.Visibility == Visibility.Visible)
            {
                return;
            }
            cleater.OnKeyDown(e.Key);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (MessageButton.Visibility == Visibility.Visible)
            {
                return;
            }
            cleater.OnKeyUp(e.Key);
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            cleater.Pause();
            var dialog = new OpenFileDialog()
            {
                Filter = "ふにゃステージファイル|*.stg",
                Title = "ふにゃステージファイルを開く",
            };
            if (dialog.ShowDialog() == true)
            {
                cleater.StageFile = dialog.FileName;
            }
            else
            {
                cleater.StageFile = "";
            }
            cleater.GameStart();
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
            if (cleater.Results.StageSelect)
            {
                MenuStage.Visibility = Visibility.Visible;
                MenuStage.Items.Clear();
                for (int i = 1; i <= cleater.StageCount; i++)
                {
                    var map = cleater.Map[i];
                    var newItem = new MenuItem()
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
                cleater.MainLoop();
            }
            var frames2 = frameCounter2.GetElapsedFrames(true);
            if (frames2 > 0)
            {
                // TODO: 眠る処理
            }
        }

        public void ShowMessage(string MessageText, MessageMode Mode, string SubMessage = "")
        {
            var message = new Message() { Content = MessageText, Mode = Mode, SubContent = SubMessage };
            if (MessageButton.Visibility == Visibility.Visible)
            {
                MessageQueue.Enqueue(message);
            }
            else
            {
                MessageButton.Visibility = Visibility.Visible;
                CancelButton.Visibility = Visibility.Visible;
                SetMessage(message);
            }
        }

        private void SetMessage(Message message)
        {
            LabelMsg.Text = message.Content;
            LabelSub.Text = message.SubContent;
            switch (message.Mode)
            {
                case MessageMode.Info:
                    MessageImage.Source = cleater.Resources.Stand;
                    break;
                case MessageMode.Clear:
                    MessageImage.Source = cleater.Resources.Happy;
                    break;
                case MessageMode.Dying:
                    MessageImage.Source = cleater.Resources.Death;
                    break;
                case MessageMode.GameOver:
                    MessageImage.Source = cleater.Resources.Death;
                    LabelMsg.Text = "Continue? 10";
                    StartContinueTimer();
                    break;
                default:
                    break;
            }
        }

        public void StartContinueTimer()
        {
            CountDown = 10;
            MessageTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1),
            };
            MessageTimer.Tick += MessageTimer_Tick;
            MessageTimer.Start();
        }

        public void EndContinueTimer()
        {
            if (MessageTimer == null)
            {
                return;
            }
            MessageTimer.Stop();
            MessageTimer = null;
        }

        private void MessageTimer_Tick(object? sender, EventArgs e)
        {
            CountDown--;
            LabelMsg.Text = $"Continue? {CountDown}";
            if (CountDown == 0)
            {
                OnCancel();
                EndContinueTimer();
            }
        }

        private void MessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageQueue.TryDequeue(out var message))
            {
                SetMessage(message);
            }
            else
            {
                if (MessageTimer != null)
                {
                    cleater.Results.GetTotal -= 10;
                    cleater.Rest = cleater.RestMax;
                    cleater.StartStage(cleater.CurrentStage);
                }
                CloseMessage();
                if (OnMessageClose != null)
                {
                    OnMessageClose.Invoke(true);
                    OnMessageClose = null;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            OnCancel();
        }

        private void OnCancel()
        {
            if (MessageQueue.TryDequeue(out var message))
            {
                SetMessage(message);
            }
            else
            {
                CloseMessage();
                if (OnMessageClose != null)
                {
                    OnMessageClose.Invoke(false);
                    OnMessageClose = null;
                }
            }
        }

        public void CloseMessage()
        {
            MessageButton.Visibility = Visibility.Collapsed;
            CancelButton.Visibility = Visibility.Collapsed;
            MessageQueue.Clear();
            EndContinueTimer();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);
            if (PreviousMousePosition != position)
            {
                PreviousMousePosition = position;
                LastMouseMoved = DateTime.Now;
                Client.Cursor = Cursors.Arrow;
            }
        }

        private void MouseHideTimer_Tick(object? sender, EventArgs e)
        {
            if ((DateTime.Now - LastMouseMoved).TotalSeconds > 3)
            {
                Client.Cursor = Cursors.None;
            }
        }

        private void FixedScreen_Click(object sender, RoutedEventArgs e)
        {
            FixedScreen.IsChecked = true;
            SizableScreen.IsChecked = false;
            WindowState = WindowState.Normal;
            ResizeMode = ResizeMode.CanMinimize;
            SizeToContent = SizeToContent.WidthAndHeight;
            Screen.Width = Client.Width;
            Screen.Height = Client.Height;
            ScreenRow.Height = new GridLength(1, GridUnitType.Auto);
        }

        private void SizableScreen_Click(object sender, RoutedEventArgs e)
        {
            FixedScreen.IsChecked = false;
            SizableScreen.IsChecked = true;
            ResizeMode = ResizeMode.CanResize;
            SizeToContent = SizeToContent.Manual;
            Screen.Width = double.NaN;
            Screen.Height = double.NaN;
            ScreenRow.Height = new GridLength(1, GridUnitType.Star);
        }
    }
}