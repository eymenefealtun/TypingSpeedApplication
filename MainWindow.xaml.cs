using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace TypingSpeedApplication
{
    public partial class MainWindow : Window
    {
        private string _lightTheme = "#f0ece4";
        private string _darkTheme = "#1e1e1e";
        BrushConverter _brushConverter;
        DispatcherTimer _dispatcherTimer;

        int _second = 7;
        public int Second
        {
            get
            {
                return this._second;
            }
            set
            {
                _second = value;
                lblTimer.Content = _second.ToString();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _brushConverter = new BrushConverter();


            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);



            _dispatcherTimer.Start();

        }

        private void dispatcherTimer_Tick(object? sender, EventArgs e)
        {
            Second--;

            if (Second == 0)
            {
                _dispatcherTimer.Stop();
                lblTimer.Content = "60";
            }
        }

        private void btnBackgroungTheme_Click(object sender, RoutedEventArgs e)
        {
            this.Background = btnBackgroungTheme.IsChecked == true ? (SolidColorBrush)_brushConverter.ConvertFromString(_lightTheme) : (SolidColorBrush)_brushConverter.ConvertFromString(_darkTheme);
        }


    }
}
