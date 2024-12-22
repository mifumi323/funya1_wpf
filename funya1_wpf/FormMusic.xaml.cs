using System.Windows;

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
    }
}
