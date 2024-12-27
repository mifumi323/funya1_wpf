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
            cleater.LoadSettings();

            SetScreenSize(cleater.Options.ScreenSize);
            WindowState = cleater.Options.WindowState;
            Width = cleater.Options.WindowWidth;
            Height = cleater.Options.WindowHeight;

            frameCounter1 = new(1000.0 / cleater.Options.Interval);
            frameCounter2 = new(2);
            Foods = new(1, 5);
            Foods[1] = Food1;
            Foods[2] = Food2;
            Foods[3] = Food3;
            Foods[4] = Food4;
            Foods[5] = Food5;

            cleater.GameStart();
            UpdateMenuItems();
            ShowMessage("ふにゃ", MessageMode.Info, "Enter/クリックで進む");
        }

        public ActionCommand MenuStage_Click => new(stageNumber =>
        {
            if (!cleater.Results.StageSelect)
            {
                return;
            }
            int stage = (int)stageNumber!;
            cleater.GameStart();
            cleater.StartStage(stage);
        });

        public ActionCommand MenuReverse_Click => new(_ =>
        {
            if (!cleater.Results.Reverse)
            {
                return;
            }
            cleater.Options.Reverse = !cleater.Options.Reverse;
            UpdateMenuReverse();
        });

        public ActionCommand MenuPause_Click => new(_ =>
        {
            if (cleater.GameState == GameState.Playing)
            {
                cleater.Pause();
            }
            else if (cleater.GameState == GameState.Paused)
            {
                cleater.ResumeGame();
            }
        });

        public ActionCommand GameExit_Click => new(_ =>
        {
            Close();
        });

        public ActionCommand MenuStart_Click => new(_ =>
        {
            cleater.GameStart();
        });

        public ActionCommand Death_Click => new(_ =>
        {
            if (cleater.GameState == GameState.Playing)
            {
                CloseMessage();
                cleater.Die();
            }
        });

        public ActionCommand MenuMusic_Click => new(_ =>
        {
            cleater.Pause();
            cleater.StopMusic(); // Pauseメソッドで音楽が止まるのはプレイ中だけ。
            var editing = cleater.MusicOptions.Clone();
            var formMusic = new FormMusic
            {
                Owner = this,
                DataContext = editing,
            };
            var result = formMusic.ShowDialog();
            if (result == true)
            {
                cleater.MusicOptions = editing;
            }
        });

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                if (cleater.GameState == GameState.Playing)
                {
                    cleater.Pause();
                }
            }
            else
            {
                cleater.Options.WindowState = WindowState;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            cleater.SaveSettings();
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

        public ActionCommand MenuOpen_Click => new(_ =>
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
        });

        public ActionCommand Speed_Click => new(interval =>
        {
            if (!cleater.Results.SpeedSet)
            {
                return;
            }
            cleater.Options.Interval = int.Parse((string)interval!);
            frameCounter1.Fps = 1000.0 / cleater.Options.Interval;
            UpdateMenuSpeed();
        });

        public ActionCommand Gravity_Click => new(gravity =>
        {
            if (!cleater.Results.GravitySet)
            {
                return;
            }
            cleater.Options.Gravity = int.Parse((string)gravity!);
            UpdateMenuGravity();
        });

        private void Mine_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // TODO: 実装
        }

        public ActionCommand MenuAbout_Click => new(_ =>
        {
            // TODO: 実装
        });

        public ActionCommand HelpContents_Click => new(_ =>
        {
            // TODO: 実装
        });

        public void UpdateMenuItems()
        {
            UpdateMenuStage();
            UpdateMenuSpeed();
            UpdateMenuGravity();
            UpdateMenuReverse();
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
                    var newItem = new MenuItem
                    {
                        Header = map.Title,
                        Command = MenuStage_Click,
                        CommandParameter = i,
                        IsChecked = i == cleater.CurrentStage
                    };
                    MenuStage.Items.Add(newItem);
                }
            }
            else
            {
                MenuStage.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateMenuSpeed()
        {
            if (cleater.Results.SpeedSet)
            {
                MenuSpeed.Visibility = Visibility.Visible;
                foreach (MenuItem item in MenuSpeed.Items)
                {
                    item.IsChecked = item.CommandParameter.Equals(cleater.Options.Interval.ToString());
                }
            }
            else
            {
                MenuSpeed.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateMenuGravity()
        {
            if (cleater.Results.GravitySet)
            {
                MenuGravity.Visibility = Visibility.Visible;
                foreach (MenuItem item in MenuGravity.Items)
                {
                    item.IsChecked = item.CommandParameter.Equals(cleater.Options.Gravity.ToString());
                }
            }
            else
            {
                MenuGravity.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateMenuReverse()
        {
            if (cleater.Results.Reverse)
            {
                MenuReverse.Visibility = Visibility.Visible;
                MenuReverse.IsChecked = cleater.Options.Reverse;
            }
            else
            {
                MenuReverse.Visibility = Visibility.Collapsed;
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
                cleater.Sleep();
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

        public ActionCommand FixedScreen_Click => new(_ =>
        {
            SetScreenSize(ScreenSize.Fixed);
        });

        public ActionCommand SizableScreen_Click => new(_ =>
        {
            SetScreenSize(ScreenSize.Sizable);
        });

        private void SetScreenSize(ScreenSize screenSize)
        {
            cleater.Options.ScreenSize = screenSize;
            switch (screenSize)
            {
                case ScreenSize.Fixed:
                    FixedScreen.IsChecked = true;
                    SizableScreen.IsChecked = false;
                    WindowState = WindowState.Normal;
                    ResizeMode = ResizeMode.CanMinimize;
                    SizeToContent = SizeToContent.WidthAndHeight;
                    Screen.Width = Client.Width;
                    Screen.Height = Client.Height;
                    ScreenRow.Height = new GridLength(1, GridUnitType.Auto);
                    break;
                case ScreenSize.Sizable:
                    FixedScreen.IsChecked = false;
                    SizableScreen.IsChecked = true;
                    ResizeMode = ResizeMode.CanResize;
                    SizeToContent = SizeToContent.Manual;
                    Screen.Width = double.NaN;
                    Screen.Height = double.NaN;
                    ScreenRow.Height = new GridLength(1, GridUnitType.Star);
                    break;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            cleater.Options.WindowWidth = Width;
            cleater.Options.WindowHeight = Height;
        }
    }
}