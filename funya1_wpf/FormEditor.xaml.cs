using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        private RenderTargetBitmap terrainImage = new(1, 1, 96, 96, PixelFormats.Pbgra32);
        private bool drawing = false;

        public StageData StageData
        {
            get => (StageData)GetValue(StageDataProperty);
            set => SetValue(StageDataProperty, value);
        }
        public static readonly DependencyProperty StageDataProperty =
            DependencyProperty.Register("StageData", typeof(StageData), typeof(FormEditor), new PropertyMetadata(null));

        public RangeArray<MapData> Maps
        {
            get => (RangeArray<MapData>)GetValue(MapsProperty);
            set => SetValue(MapsProperty, value);
        }
        public static readonly DependencyProperty MapsProperty =
            DependencyProperty.Register("Maps", typeof(RangeArray<MapData>), typeof(FormEditor), new PropertyMetadata(null));

        public KeyValuePair<int, MapData> SelectedMap
        {
            get => (KeyValuePair<int, MapData>)GetValue(SelectedMapProperty);
            set => SetValue(SelectedMapProperty, value);
        }
        public static readonly DependencyProperty SelectedMapProperty =
            DependencyProperty.Register("SelectedMap", typeof(KeyValuePair<int, MapData>), typeof(FormEditor), new PropertyMetadata(default(KeyValuePair<int, MapData>)));

        public int SelectedChip
        {
            get => (int)GetValue(SelectedChipProperty);
            set => SetValue(SelectedChipProperty, value);
        }
        public static readonly DependencyProperty SelectedChipProperty =
            DependencyProperty.Register("SelectedChip", typeof(int), typeof(FormEditor), new PropertyMetadata(0));

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
            StageData.Map[1].Title = "マップ1";
            StageData.Map[1].MaxX = 9;
            StageData.Map[1].MaxY = 9;
            Maps = StageData.GetValidMaps();
            SelectedMap = Maps.First();
            IsChanged = false;
        }

        private void StageCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            drawing = true;
            StageCanvas_MouseMove(sender, e);
        }

        private void StageCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && drawing)
            {
                var x = (int)e.GetPosition(StageCanvas).X / 32;
                var y = (int)e.GetPosition(StageCanvas).Y / 32;
                if (x >= 0 && x < SelectedMap.Value.Width && y >= 0 && y < SelectedMap.Value.Height)
                {
                    SelectedMap.Value.Data[x, y] = SelectedChip;
                    var dv = new DrawingVisual();
                    using (var dc = dv.RenderOpen())
                    {
                        SelectedMap.Value.DrawTile(StageData.croppedBitmaps, dc, x, y);
                    }
                    terrainImage.Render(dv);
                    IsChanged = true;
                }
            }
            else
            {
                drawing = false;
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                var x = (int)e.GetPosition(StageCanvas).X / 32;
                var y = (int)e.GetPosition(StageCanvas).Y / 32;
                if (x >= 0 && x < SelectedMap.Value.Width && y >= 0 && y < SelectedMap.Value.Height)
                {
                    SelectChip(SelectedMap.Value.Data[x, y]);
                }
            }
        }

        private void SelectChip(int newChip)
        {
            SelectedChip = newChip;
            Canvas.SetLeft(ChipSelector, 32 * newChip);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (IsChanged)
            {
                var result = MessageBox.Show(this, $"\"{StageData.StageFile}\"の内容は変更されているらしいです。\n保存するんですか？", "終了の前にちょっと確認させていただきたく存じ上げます", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Save();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    DialogResult = false;
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
                    Maps = StageData.GetValidMaps();
                    SelectedMap = Maps.First();
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

        public ICommand Exit_Click => new ActionCommand(_ =>
        {
            DialogResult = false;
            Close();
        });

        public ICommand ExitAll_Click => new ActionCommand(_ =>
        {
            DialogResult = true;
            Close();
        });

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

        private void MapList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.RemovedItems)
            {
                if (item is KeyValuePair<int, MapData> map)
                {
                    map.Value.PropertyChanged -= SelectedMap_PropertyChanged;
                }
            }
            foreach (var item in e.AddedItems)
            {
                if (item is KeyValuePair<int, MapData> map)
                {
                    map.Value.PropertyChanged += SelectedMap_PropertyChanged;
                }
            }
            SelectedMap.Value.DrawTerrainInPanel(StageCanvas, StageData.croppedBitmaps, true);
            terrainImage = (StageCanvas.Background as ImageBrush)?.ImageSource as RenderTargetBitmap ?? terrainImage;
        }

        private void SelectedMap_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MapData.Width))
            {
                StageCanvas.Width = SelectedMap.Value.Width * 32;
            }
            else if (e.PropertyName == nameof(MapData.Height))
            {
                StageCanvas.Height = SelectedMap.Value.Height * 32;
            }
        }

        private void ChipContainer_MouseDown(object sender, MouseButtonEventArgs e) => ChipContainer_MouseMove(sender, e);

        private void ChipContainer_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var x = (int)e.GetPosition(ChipContainer).X / 32;
                if (x >= 0 && x < 10)
                {
                    SelectChip(x);
                }
            }
        }
    }
}
