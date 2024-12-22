using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace funya1_wpf
{
    /// <summary>
    /// FormMusic.xaml の相互作用ロジック
    /// </summary>
    public partial class FormMusic : Window
    {
        public FormMusic()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var OpenButton = (Button)sender;
            var dialog = new OpenFileDialog()
            {
                Filter = "音楽ファイル|*.mid;*.mp3|全てのファイル|*.*",
                Title = "音楽ファイルを開く",
                FileName = OpenButton.Tag as string,
            };
            if (dialog.ShowDialog() == true)
            {
                OpenButton.Tag = dialog.FileName;
            }
        }
    }
}
