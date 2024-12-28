using System.Windows;

namespace funya1_wpf
{
    /// <summary>
    /// FormAbout.xaml の相互作用ロジック
    /// </summary>
    public partial class FormAbout : Window
    {
        public FormAbout()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
