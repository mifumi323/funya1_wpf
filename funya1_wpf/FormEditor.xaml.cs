using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
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

        private enum EditMode
        {
            Chip,
            Mine,
            Food,
        }
        private EditMode editMode = EditMode.Chip;
        private int selectedNumber = 0;

        private readonly Effect darkEffect = new DropShadowEffect
        {
            Color = Colors.Black,
            ShadowDepth = 0,
            BlurRadius = 16,
            Opacity = 0.5,
        };
        private readonly Effect lightEffect = new DropShadowEffect
        {
            Color = Colors.White,
            ShadowDepth = 0,
            BlurRadius = 16,
            Opacity = 0.5,
        };

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

        public SolidColorBrush StageColorBrush
        {
            get => (SolidColorBrush)GetValue(StageColorBrushProperty);
            set => SetValue(StageColorBrushProperty, value);
        }
        public static readonly DependencyProperty StageColorBrushProperty =
            DependencyProperty.Register("StageColorBrush", typeof(SolidColorBrush), typeof(FormEditor), new PropertyMetadata(null));

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
                StageCount = 0,
                Friction = 10,
                RestMax = 10,
                EndingType = 0,
                StageColor = Color.FromRgb(0, 0, 0),
            };
            StageData.LoadSampleImage();
            AddMap();
            SelectedMap = Maps.First();
            UpdateColor();
            Select(0, EditMode.Chip);
            UpdateMapSelectUi();
            IsChanged = false;
        }

        private void AddMap()
        {
            StageData.StageCount++;
            StageData.Map[StageData.StageCount].Reset();
            StageData.Map[StageData.StageCount].Title = $"マップ{StageData.StageCount}";
            Maps = StageData.GetValidMaps();
        }

        private void StageCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            drawing = true;
            StageCanvas_MouseMove(sender, e);
        }

        private void StageCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var x = (int)e.GetPosition(StageCanvas).X / 32;
            var y = (int)e.GetPosition(StageCanvas).Y / 32;
            if (editMode == EditMode.Mine && !IsValidMinePosition(x, y))
            {
                StageCanvas.Cursor = Cursors.No;
            }
            else
            {
                StageCanvas.Cursor = Cursors.Pen;
            }

            if (e.LeftButton == MouseButtonState.Pressed && drawing)
            {
                if (x >= 0 && x < SelectedMap.Value.Width && y >= 0 && y < SelectedMap.Value.Height)
                {
                    switch (editMode)
                    {
                        case EditMode.Chip:
                            SelectedMap.Value.Data[x, y] = selectedNumber;
                            var dv = new DrawingVisual();
                            using (var dc = dv.RenderOpen())
                            {
                                SelectedMap.Value.DrawTile(StageData.croppedBitmaps, dc, x, y);
                            }
                            terrainImage.Render(dv);
                            break;
                        case EditMode.Mine:
                            if (IsValidMinePosition(x, y))
                            {
                                SelectedMap.Value.StartX = x;
                                SelectedMap.Value.StartY = y;
                            }
                            MoveCharacters();
                            break;
                        case EditMode.Food:
                            SelectedMap.Value.Food[selectedNumber].x = x;
                            SelectedMap.Value.Food[selectedNumber].y = y;
                            MoveCharacters();
                            break;
                    }
                    IsChanged = true;
                }
            }
            else
            {
                drawing = false;
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (x >= 0 && x < SelectedMap.Value.Width && y >= 0 && y < SelectedMap.Value.Height)
                {
                    var newChip = SelectedMap.Value.Data[x, y];
                    var newMode = EditMode.Chip;
                    if (x == SelectedMap.Value.StartX && y == SelectedMap.Value.StartY)
                    {
                        newChip = 0;
                        newMode = EditMode.Mine;
                    }
                    for (var i = 1; i <= SelectedMap.Value.TotalFood; i++)
                    {
                        if (x == SelectedMap.Value.Food[i].x && y == SelectedMap.Value.Food[i].y)
                        {
                            newChip = i;
                            newMode = EditMode.Food;
                        }
                    }
                    Select(newChip, newMode);
                }
            }
        }

        private bool IsValidMinePosition(int x, int y)
        {
            return x >= 1 && x < SelectedMap.Value.Width - 1 && y >= 1 && y < SelectedMap.Value.Height;
        }

        private void Select(int newChip, EditMode newMode)
        {
            editMode = newMode;
            selectedNumber = newChip;
            switch (newMode)
            {
                case EditMode.Chip:
                    ChipSelector.Visibility = Visibility.Visible;
                    Canvas.SetLeft(ChipSelector, 32 * newChip);
                    SelectMine.IsChecked = false;
                    SelectFood1.IsChecked = false;
                    SelectFood2.IsChecked = false;
                    SelectFood3.IsChecked = false;
                    SelectFood4.IsChecked = false;
                    SelectFood5.IsChecked = false;
                    break;
                case EditMode.Mine:
                    ChipSelector.Visibility = Visibility.Collapsed;
                    SelectMine.IsChecked = true;
                    SelectFood1.IsChecked = false;
                    SelectFood2.IsChecked = false;
                    SelectFood3.IsChecked = false;
                    SelectFood4.IsChecked = false;
                    SelectFood5.IsChecked = false;
                    break;
                case EditMode.Food:
                    ChipSelector.Visibility = Visibility.Collapsed;
                    SelectMine.IsChecked = false;
                    SelectFood1.IsChecked = newChip == 1;
                    SelectFood2.IsChecked = newChip == 2;
                    SelectFood3.IsChecked = newChip == 3;
                    SelectFood4.IsChecked = newChip == 4;
                    SelectFood5.IsChecked = newChip == 5;
                    break;
            }
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
                    UpdateColor();
                    Select(0, EditMode.Chip);
                    UpdateMapSelectUi();
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
            MoveCharacters();
            UpdateFoodsVisibility();
        }

        private void MoveCharacters()
        {
            Canvas.SetLeft(PlaceMine, SelectedMap.Value.StartX * 32);
            Canvas.SetTop(PlaceMine, SelectedMap.Value.StartY * 32);
            Canvas.SetLeft(PlaceFood1, SelectedMap.Value.Food[1].x * 32);
            Canvas.SetTop(PlaceFood1, SelectedMap.Value.Food[1].y * 32);
            Canvas.SetLeft(PlaceFood2, SelectedMap.Value.Food[2].x * 32);
            Canvas.SetTop(PlaceFood2, SelectedMap.Value.Food[2].y * 32);
            Canvas.SetLeft(PlaceFood3, SelectedMap.Value.Food[3].x * 32);
            Canvas.SetTop(PlaceFood3, SelectedMap.Value.Food[3].y * 32);
            Canvas.SetLeft(PlaceFood4, SelectedMap.Value.Food[4].x * 32);
            Canvas.SetTop(PlaceFood4, SelectedMap.Value.Food[4].y * 32);
            Canvas.SetLeft(PlaceFood5, SelectedMap.Value.Food[5].x * 32);
            Canvas.SetTop(PlaceFood5, SelectedMap.Value.Food[5].y * 32);
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
            else if (e.PropertyName == nameof(MapData.TotalFood))
            {
                UpdateFoodsVisibility();
            }
        }

        private void UpdateFoodsVisibility()
        {
            PlaceFood1.Visibility = SelectFood1.Visibility = SelectedMap.Value.TotalFood >= 1 ? Visibility.Visible : Visibility.Collapsed;
            PlaceFood2.Visibility = SelectFood2.Visibility = SelectedMap.Value.TotalFood >= 2 ? Visibility.Visible : Visibility.Collapsed;
            PlaceFood3.Visibility = SelectFood3.Visibility = SelectedMap.Value.TotalFood >= 3 ? Visibility.Visible : Visibility.Collapsed;
            PlaceFood4.Visibility = SelectFood4.Visibility = SelectedMap.Value.TotalFood >= 4 ? Visibility.Visible : Visibility.Collapsed;
            PlaceFood5.Visibility = SelectFood5.Visibility = SelectedMap.Value.TotalFood >= 5 ? Visibility.Visible : Visibility.Collapsed;
            ReduceFood.Visibility = SelectedMap.Value.TotalFood > 1 ? Visibility.Visible : Visibility.Collapsed;
            AddFood.Visibility = SelectedMap.Value.TotalFood < 5 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ChipContainer_MouseDown(object sender, MouseButtonEventArgs e) => ChipContainer_MouseMove(sender, e);

        private void ChipContainer_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var x = (int)e.GetPosition(ChipContainer).X / 32;
                if (x >= 0 && x < 10)
                {
                    Select(x, EditMode.Chip);
                }
            }
        }

        public ICommand BackColor_Click => new ActionCommand(friction =>
        {
            var dialog = new FormColor(StageData.StageColor);
            if (dialog.ShowDialog() == true)
            {
                StageData.StageColor = dialog.Color;
            }
            UpdateColor();
        });

        private void UpdateColor()
        {
            StageColorBrush = new SolidColorBrush(StageData.StageColor);
            StageCanvas.Effect = StageData.StageColor.IsDark() ? lightEffect : darkEffect;
        }

        public ICommand AddFood_Click => new ActionCommand(_ => SelectedMap.Value.TotalFood++);

        public ICommand ReduceFood_Click => new ActionCommand(_ => SelectedMap.Value.TotalFood--);

        public ICommand SelectMine_Click => new ActionCommand(_ => Select(0, EditMode.Mine));

        public ICommand SelectFood_Click => new ActionCommand(number => Select(int.Parse(number as string ?? "1"), EditMode.Food));

        public ICommand MapAdd_Click => new ActionCommand(_ =>
        {
            AddMap();
            SelectedMap = Maps.Last();
            UpdateMapSelectUi();
        });

        public ICommand MapDel_Click => new ActionCommand(_ =>
        {
            var number = SelectedMap.Key;
            var map = SelectedMap.Value;
            for (var i = number + 1; i <= StageData.StageCount; i++)
            {
                StageData.Map[i - 1] = StageData.Map[i];
                StageData.Map[i] = map;
            }
            StageData.StageCount--;
            Maps = StageData.GetValidMaps();
            SelectedMap = Maps.ElementAt(Math.Min(number - 1, StageData.StageCount - 1));
            UpdateMapSelectUi();
        });

        private void UpdateMapSelectUi()
        {
            MapAdd.IsEnabled = StageData.StageCount < 30;
            MapDel.IsEnabled = StageData.StageCount > 1;
        }

        public ICommand LoadChip_Click => new ActionCommand(_ =>
        {
            if (string.IsNullOrEmpty(StageData.StageFile))
            {
                MessageBox.Show(this, "いったん保存した後に変更できます。", "画像切り替え", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var f = new FormSelectImage();
            if (f.ShowDialog() == true)
            {
            }
        });
    }
}
