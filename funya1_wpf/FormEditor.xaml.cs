using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace funya1_wpf
{
    /// <summary>
    /// FormEditor.xaml の相互作用ロジック
    /// </summary>
    public partial class FormEditor : Window
    {
        private bool IsChanged = false;
        public StageData StageData = new(null!);
        public Resources resources;
        private readonly Options options;

        public int Friction
        {
            get => StageData.Friction;
            set
            {
                if (StageData.Friction == value)
                {
                    return;
                }

                StageData.Friction = value;
                IsChanged = true;
            }
        }

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
            };
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

        public ICommand Open_Click => new ActionCommand(_ =>
        {
            // TODO: ファイルを開く
        });

        public ICommand Save_Click => new ActionCommand(_ =>
        {
            // TODO: ファイルを保存
        });

        public ICommand SaveAs_Click => new ActionCommand(_ =>
        {
            // TODO: 名前をつけて保存
        });

        public ICommand Exit_Click => new ActionCommand(_ => Close());

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
