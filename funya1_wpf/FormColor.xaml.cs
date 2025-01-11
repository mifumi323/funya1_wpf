using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace funya1_wpf
{
    /// <summary>
    /// FormColor.xaml の相互作用ロジック
    /// </summary>
    public partial class FormColor : Window
    {
        public byte R
        {
            get { return (byte)GetValue(RProperty); }
            set { SetValue(RProperty, value); }
        }
        public static readonly DependencyProperty RProperty =
            DependencyProperty.Register("R", typeof(byte), typeof(FormColor), new PropertyMetadata((byte)0, static (s, e) => (s as FormColor)?.OnColorChanged()));

        public byte G
        {
            get { return (byte)GetValue(GProperty); }
            set { SetValue(GProperty, value); }
        }
        public static readonly DependencyProperty GProperty =
            DependencyProperty.Register("G", typeof(byte), typeof(FormColor), new PropertyMetadata((byte)0, static (s, e) => (s as FormColor)?.OnColorChanged()));

        public byte B
        {
            get { return (byte)GetValue(BProperty); }
            set { SetValue(BProperty, value); }
        }
        public static readonly DependencyProperty BProperty =
            DependencyProperty.Register("B", typeof(byte), typeof(FormColor), new PropertyMetadata((byte)0, static (s, e) => (s as FormColor)?.OnColorChanged()));

        public Color Color
        {
            get => Color.FromRgb(R, G, B);
            set { R = value.R; G = value.G; B = value.B; }
        }

        public FormColor(Color color)
        {
            InitializeComponent();
            Color = color;
            CancelButton.Background = new SolidColorBrush(Color);
            CancelButton.Foreground = new SolidColorBrush(TextColor(Color));
            OnColorChanged();
        }

        private void OnColorChanged()
        {
            OkButton.Background = new SolidColorBrush(Color);
            OkButton.Foreground = new SolidColorBrush(TextColor(Color));
            RSlider.Background = new LinearGradientBrush(Color.FromRgb(0, G, B), Color.FromRgb(255, G, B), 0);
            GSlider.Background = new LinearGradientBrush(Color.FromRgb(R, 0, B), Color.FromRgb(R, 255, B), 0);
            BSlider.Background = new LinearGradientBrush(Color.FromRgb(R, G, 0), Color.FromRgb(R, G, 255), 0);
        }

        private static Color TextColor(Color color)
        {
            return (color.R + color.G + color.B) / 3 < 128 ? Colors.White : Colors.Black;
        }

        public ICommand OkButton_Click => new ActionCommand(friction =>
        {
            DialogResult = true;
            Close();
        });
    }
}
