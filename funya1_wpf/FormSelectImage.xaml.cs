using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace funya1_wpf
{
    /// <summary>
    /// FormSelectImage.xaml の相互作用ロジック
    /// </summary>
    public partial class FormSelectImage : Window
    {
        private record ImageItem(string Path, BitmapSource Image, string Title, bool IsValid)
        {
            public string DisplayTitle => IsValid ? Title : $"{Title} (使えません)";
        }

        private ImageItem[] ImageItems
        {
            get => (ImageItem[])GetValue(ImageItemsProperty);
            set => SetValue(ImageItemsProperty, value);
        }
        public static readonly DependencyProperty ImageItemsProperty =
            DependencyProperty.Register("ImageItems", typeof(ImageItem[]), typeof(FormSelectImage), new PropertyMetadata(Array.Empty<ImageItem>()));

        private ImageItem SelectedImage
        {
            get => (ImageItem)GetValue(SelectedImageProperty);
            set => SetValue(SelectedImageProperty, value);
        }
        public static readonly DependencyProperty SelectedImageProperty =
            DependencyProperty.Register("SelectedImage", typeof(ImageItem), typeof(FormSelectImage), new PropertyMetadata(null));

        public string ImagePath => SelectedImage.Path;

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OkButton_Click.Execute(null);
        }

        public FormSelectImage(string baseDirectory, string imagePath, Resources resources)
        {
            InitializeComponent();

            ImageItems = [
                new("", resources.BlockData1, "(サンプル画像)", true),
                ..Directory.EnumerateFiles(baseDirectory)
                .Where(file => Path.GetExtension(file).ToLower() is ".bmp" or ".gif" or ".jpg" or ".jpeg" or ".png")
                .Select(file => {
                    try {
                        using var stream = File.OpenRead(file);
                        var image = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                        var fileName = Path.GetFileName(file);
                        return new ImageItem(fileName, image, fileName, image.IsValidMapChipSize());
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                })
                .Where(item => item != null)!
            ];
            SelectedImage = ImageItems.FirstOrDefault(item => item.Path == imagePath) ?? ImageItems[0];
        }

        public ICommand OkButton_Click => new ActionCommand(friction =>
        {
            if (SelectedImage == null || !SelectedImage.IsValid)
            {
                MessageBox.Show("有効な画像を選択してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DialogResult = true;
            Close();
        });
    }
}
