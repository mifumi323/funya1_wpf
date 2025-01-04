using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace funya1_wpf
{
    /// <summary>
    /// FormEditor.xaml の相互作用ロジック
    /// </summary>
    public partial class FormEditor : Window
    {
        private bool IsChanged = false;
        public Resources resources;
        private readonly Options options;

        public StageData StageData
        {
            get => (StageData)GetValue(StageDataProperty);
            set => SetValue(StageDataProperty, value);
        }
        public static readonly DependencyProperty StageDataProperty =
            DependencyProperty.Register("StageData", typeof(StageData), typeof(FormEditor), new PropertyMetadata(null));


        public FormEditor(Resources resources, Options options)
        {
            InitializeComponent();

            this.resources = resources;
            this.options = options;

            WindowState = options.StageMakerState;
            Width = options.StageMakerWidth;
            Height = options.StageMakerHeight;

            NewData();
        }

        private void NewData()
        {
            StageData = new StageData(resources.BlockData1)
            {
                StageFile = "",
                StageCount = 1,
                Friction = 10,
                RestMax = 10,
                EndingType = 0,
                StageColor = Color.FromRgb(0, 0, 0),
            };
            StageData.LoadSampleImage();
            IsChanged = false;
        }

        private void StageCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void StageCanvas_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void StageCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (IsChanged)
            {
                var result = MessageBox.Show(this, $"\"{StageData.StageFile}\"の内容は変更されているらしいです。\n保存するんですか？", "終了の前にちょっと確認させていただきたく存じ上げます", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // TODO: セーブ
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        public ICommand New_Click => new ActionCommand(_ => NewData());

        public ICommand Open_Click => new ActionCommand(_ => Open());

        private void Open()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "ふにゃステージファイル|*.stg",
                DefaultExt = ".stg",
                DefaultDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "funyaMaker"),
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var stageData = new StageData(resources.BlockData1)
                    {
                        StageFile = dialog.FileName,
                    };
                    stageData.LoadFile();
                    IsChanged = false;
                    StageData = stageData;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public ICommand Save_Click => new ActionCommand(_ => Save());

        private void Save()
        {
            if (string.IsNullOrEmpty(StageData.StageFile))
            {
                SaveAs();
            }
            else
            {
                StageData.Save();
            }
        }

        public ICommand SaveAs_Click => new ActionCommand(_ => SaveAs());

        private void SaveAs()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "ふにゃステージファイル|*.stg",
                DefaultExt = ".stg",
                FileName = StageData.StageFile,
                DefaultDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "funyaMaker"),
            };
            if (dialog.ShowDialog() == true)
            {
                StageData.StageFile = dialog.FileName;
                StageData.Save();
            }
        }

        public ICommand Exit_Click => new ActionCommand(_ => Close());

        public ICommand FrictionPreset_Click => new ActionCommand(friction =>
        {
            StageData.Friction = int.Parse((friction as string)!);
        });

        private void FrictionPreset_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            foreach (var item in FrictionPreset.Items)
            {
                if (item is MenuItem menuItem)
                {
                    menuItem.IsChecked = menuItem.CommandParameter.ToString() == StageData.Friction.ToString();
                }
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState != WindowState.Minimized)
            {
                options.StageMakerState = WindowState;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            options.StageMakerWidth = Width;
            options.StageMakerHeight = Height;
        }
    }
}
