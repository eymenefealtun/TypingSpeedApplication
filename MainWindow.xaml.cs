using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TypingSpeedApplication
{
    public partial class MainWindow : Window
    {
        private string _lightTheme = "#f0ece4";
        private string _darkTheme = "#1e1e1e";
        BrushConverter _brushConverter;

        public MainWindow()
        {
            InitializeComponent();
            _brushConverter = new BrushConverter();

            Label label1 = new Label();
            label1.Content = "My Words";

            Label label2 = new Label();
            label2.Content = "My Second Words";

            stckPanel1.Children.Clear();

            stckPanel1.Children.Add(label1);
            stckPanel1.Children.Add(label2);
        }

        private void btnBackgroungTheme_Click(object sender, RoutedEventArgs e)
        {
            this.Background = btnBackgroungTheme.IsChecked == true ? (SolidColorBrush)_brushConverter.ConvertFromString(_lightTheme) : (SolidColorBrush)_brushConverter.ConvertFromString(_darkTheme);
        }


    }
}
