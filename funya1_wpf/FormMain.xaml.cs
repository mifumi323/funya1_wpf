﻿using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Text;
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
        private Cleater cleater;
        private readonly Music music = new();
        private readonly Results results = new();
        private readonly Options options = new();
        private readonly Resources resources = new();
        private readonly SaveData SaveData;
        private bool saveOnExit = true;

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

        public readonly DispatcherTimer MusicFadeTimer = new() { Interval = TimeSpan.FromMilliseconds(100) };

        public FormMain()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            InitializeComponent();
            MouseHideTimer.Tick += MouseHideTimer_Tick;
            MouseHideTimer.Start();

            cleater = new Cleater(this, music, results, options, resources);
            SaveData = new(music, results, options);
            SaveData.Load();

            MusicFadeTimer.Tick += (_, _) =>
            {
                if (cleater.GameState == GameState.Playing)
                {
                    music.Volume = cleater.Status == Status.Slepping ? Music.SleepVolume : Music.NormalVolume;
                }
                music.OnFrame();
            };
            MusicFadeTimer.Start();

            SetScreenSize(options.ScreenSize);
            WindowState = options.WindowState;
            Width = options.WindowWidth;
            Height = options.WindowHeight;

            frameCounter1 = new(1000.0 / options.Interval);
            frameCounter2 = new(2);
            Foods = new(1, 5);
            Foods[1] = Food1;
            Foods[2] = Food2;
            Foods[3] = Food3;
            Foods[4] = Food4;
            Foods[5] = Food5;

            cleater.GameStart();
            music.Stop();
            UpdateMenuItems();
            ShowMessage("ふにゃ", MessageMode.Info, "Enter/クリックで進む");
            OnMessageClose = _ =>
            {
                if (cleater.GameState == GameState.Playing)
                {
                    music.Play(music.Options.Playing, cleater.Status == Status.Slepping ? Music.SleepVolume : Music.NormalVolume);
                }
            };
        }

        public ActionCommand MenuStage_Click => new(stageNumber =>
        {
            if (!results.StageSelect)
            {
                return;
            }
            int stage = (int)stageNumber!;
            cleater.GameStart();
            cleater.StartStage(stage);
            UpdatePause();
        });

        public ActionCommand MenuReverse_Click => new(_ =>
        {
            if (!results.Reverse)
            {
                return;
            }
            options.Reverse = !options.Reverse;
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
            UpdatePause();
        });

        private void UpdatePause()
        {
            MenuPause.IsChecked = cleater.GameState == GameState.Paused;
            ButtonPause.IsChecked = cleater.GameState == GameState.Paused;
        }

        public ActionCommand GameExit_Click => new(_ =>
        {
            Close();
        });

        public ActionCommand ExitAndDeleteData_Click => new(_ =>
        {
            string[] messages = [
                "ふにゃのセーブデータを削除しますか？",
                "本当に削除してもよいのですね？",
                "削除したデータは二度ともとに戻りませんがよいのですか？",
                "最後に訊きます。本当に削除するのですね？",
            ];
            foreach (var message in messages)
            {
                var result = MessageBox.Show(message, "確認", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            SaveData.Delete();
            saveOnExit = false;
            Close();
        });

        public ActionCommand MenuStart_Click => new(_ =>
        {
            if (cleater.CurrentStage > 1 && cleater.GameState is GameState.Playing or GameState.Paused)
            {
                var result = MessageBox.Show("マップ1からやり直しますか？", "確認", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            cleater.GameStart();
            UpdatePause();
        });

        public ActionCommand Death_Click => new(_ =>
        {
            if (cleater.GameState == GameState.Playing)
            {
                CloseMessage();
                cleater.Die();
                UpdatePause();
            }
        });

        public ActionCommand MenuMusic_Click => new(_ =>
        {
            cleater.Pause();
            music.Stop(); // Pauseメソッドで音楽が止まるのはプレイ中だけ。
            UpdatePause();
            var editing = music.Options.Clone();
            var formMusic = new FormMusic
            {
                Owner = this,
                DataContext = editing,
            };
            var result = formMusic.ShowDialog();
            if (result == true)
            {
                music.Options = editing;
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
                options.WindowState = WindowState;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (saveOnExit)
            {
                SaveData.Save();
            }
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
            UpdatePause();
            var newCleater = new Cleater(this, music, results, options, resources);
            var dialog = new OpenFileDialog()
            {
                Filter = "ふにゃステージファイル|*.stg",
                Title = "ふにゃステージファイルを開く",
                DefaultDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "funyaMaker"),
            };
            string fileName;
            if (dialog.ShowDialog() == true)
            {
                fileName = dialog.FileName;
            }
            else
            {
                fileName = "";
            }
            OpenStageFile(fileName);
        });

        public ActionCommand Speed_Click => new(interval =>
        {
            if (!results.SpeedSet)
            {
                return;
            }
            options.Interval = int.Parse((string)interval!);
            frameCounter1.Fps = 1000.0 / options.Interval;
            UpdateMenuSpeed();
        });

        public ActionCommand Gravity_Click => new(gravity =>
        {
            if (!results.GravitySet)
            {
                return;
            }
            options.Gravity = int.Parse((string)gravity!);
            UpdateMenuGravity();
        });

        private void Mine_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenResultsCommand.Execute(null);
        }

        public ActionCommand MenuAbout_Click => new(_ =>
        {
            cleater.Pause();
            UpdatePause();
            var formAbout = new FormAbout
            {
                Owner = this,
                resources = resources,
            };
            formAbout.ShowDialog();
        });

        public static ActionCommand HelpContents_Click => new(_ =>
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Help", "index.html"),
                UseShellExecute = true,
            });
        });

        public void UpdateMenuItems()
        {
            UpdateMenuStage();
            UpdateMenuSpeed();
            UpdateMenuGravity();
            UpdateMenuReverse();
            UpdateToolBar();
        }

        public void UpdateMenuStage()
        {
            if (results.StageSelect)
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
            if (results.SpeedSet)
            {
                MenuSpeed.Visibility = Visibility.Visible;
                foreach (MenuItem item in MenuSpeed.Items)
                {
                    item.IsChecked = item.CommandParameter.Equals(options.Interval.ToString());
                }
            }
            else
            {
                MenuSpeed.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateMenuGravity()
        {
            if (results.GravitySet)
            {
                MenuGravity.Visibility = Visibility.Visible;
                foreach (MenuItem item in MenuGravity.Items)
                {
                    item.IsChecked = item.CommandParameter.Equals(options.Gravity.ToString());
                }
            }
            else
            {
                MenuGravity.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateMenuReverse()
        {
            if (results.Reverse)
            {
                MenuReverse.Visibility = Visibility.Visible;
                MenuReverse.IsChecked = options.Reverse;
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
                for (int i = 0; i < frames1; i++)
                {
                    cleater.MainLoop();
                }
            }
            var frames2 = frameCounter2.GetElapsedFrames(true);
            if (frames2 > 0)
            {
                for (int i = 0; i < frames2; i++)
                {
                    cleater.Sleep();
                }
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
                    MessageImage.Source = resources.Stand;
                    break;
                case MessageMode.Clear:
                    MessageImage.Source = resources.Happy;
                    break;
                case MessageMode.Dying:
                    MessageImage.Source = resources.Death;
                    break;
                case MessageMode.GameOver:
                    MessageImage.Source = resources.Death;
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
                    results.GetTotal -= 10;
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
            options.ScreenSize = screenSize;
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
            options.WindowWidth = Width;
            options.WindowHeight = Height;
        }

        public ActionCommand OpenResultsCommand => new(_ =>
        {
            cleater.Pause();
            UpdatePause();
            var formResults = new FormResults(results)
            {
                Owner = this,
            };
            formResults.ShowDialog();
        });

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
                var fileName = fileNames[0];
                OpenStageFile(fileName);
            }
        }

        private void OpenStageFile(string fileName)
        {
            var newCleater = new Cleater(this, music, results, options, resources)
            {
                StageFile = fileName
            };
            bool success;
            try
            {
                success = newCleater.GameStart();
            }
            catch (Exception)
            {
                success = false;
            }
            if (success)
            {
                cleater = newCleater;
                UpdateMenuItems();
                UpdatePause();
            }
            else
            {
                MessageBox.Show("ステージファイル読み込みに失敗しました。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ActionCommand MenuStageMaker_Click => new(_ =>
        {
            cleater.Pause();
            UpdatePause();
            var formEditor = new FormEditor(resources, options)
            {
                Owner = this,
            };
            Hide();
            var result = formEditor.ShowDialog();
            if (result == true)
            {
                Close();
            }
            else
            {
                Show();
            }
        });

        public ActionCommand MenuToolBar_Click => new(_ =>
        {
            options.ToolBarVisible = !options.ToolBarVisible;
            UpdateToolBar();
        });

        public static ActionCommand TestException_Click => new(_ =>
        {
            // UI スレッドでの例外テスト
            throw new InvalidOperationException("これはテスト例外です。エラーハンドリングが正しく機能していることを確認するためのものです。");
        });

        public static ActionCommand TestThreadException_Click => new(_ =>
        {
            // 非 UI スレッドでの例外テスト
            Thread thread = new(() =>
            {
                Thread.Sleep(500); // 少し待機
                throw new InvalidOperationException("これは非UIスレッドでのテスト例外です。");
            });
            thread.Start();
        });

        public static ActionCommand TestTaskException_Click => new(_ =>
        {
            // Task での例外テスト
            Task.Run(() =>
            {
                Thread.Sleep(500); // 少し待機
                throw new InvalidOperationException("これはTaskでのテスト例外です。");
            });
        });

        private void UpdateToolBar()
        {
            MainToolBar.Visibility = options.ToolBarVisible ? Visibility.Visible : Visibility.Collapsed;
            MenuToolBar.IsChecked = options.ToolBarVisible;
        }
    }
}