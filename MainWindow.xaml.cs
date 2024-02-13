using System;
using System.Windows;
using LexiExtract;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace TypingSpeedApplication
{
    public partial class MainWindow : Window
    {
        private string _lightTheme = "#f0ece4";
        private string _darkTheme = "#1e1e1e";
        BrushConverter _brushConverter;
        DispatcherTimer _dispatcherTimer;
        List<Language> _languages;

        string _currentLanguage;

        public static string[] _resultWordArray;
        public static string[] _sourceWords;


        List<Label> _words1;
        List<Label> _words2;

        int _numberOfTrueWords = 0;
        int _currentWordIndex = 0;
        int _numbeOfWrongWords = 0;
        int _numberOfFalseKeyStrokes = 0;
        int _numberOfTrueKeyStrokes = 0;

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

            _languages = typeof(Language).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Language))).Select(t => (Language)Activator.CreateInstance(t)).ToList();


            _words1 = new List<Label>();
            _words2 = new List<Label>();


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


        private async void cBoxLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            _currentLanguage = cBoxLanguages.SelectedValue.ToString();

            _sourceWords = await _languages.Where(x => x.GetType().Name == _currentLanguage).First().GetAllWordsAsync();

            // method
            RefreshGame();

            Mouse.OverrideCursor = null;

        }



        private void RefreshStack(StackPanel panel, List<Label> labels)
        {
            panel.Children.Clear();
            labels.Clear();

            _resultWordArray = _sourceWords.GetRandomWords(40);


            int currentLength = 10;

            for (int i = 0; i < _resultWordArray.Length; i++)
            {
                Label lbl = new Label();
                lbl.Background = i == 0 && labels == _words1 ? Brushes.LightGray : Brushes.Transparent;
                lbl.Content = _resultWordArray[i];

                lbl.Style = (Style)FindResource("MainLabelTheme");


                lbl.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                lbl.Arrange(new Rect(lbl.DesiredSize));

                currentLength += Convert.ToInt32(lbl.ActualWidth) + (int)lbl.Margin.Left;

                if (currentLength > stckPanel1.Width)
                    break;

                labels.Add(lbl);
                panel.Children.Insert(i, lbl);

            }

        }

        private void RefreshGame()
        {
            _dispatcherTimer.Stop();

            lblTimer.Content = "60";
            _second = 60;

            RefreshStack(stckPanel1, _words1);
            RefreshStack(stckPanel2, _words2);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshGame();
        }

        string _currentTextOfTextbox;
        bool _isStartedBefore = false;
        bool _isTextBoxChangedCanFire;
        int _numberOfKeyStrokes = 0;
        string _targetText;

        private void tboxWrite_TextChanged(object sender, TextChangedEventArgs e)
        {
            _currentTextOfTextbox = tboxWrite.Text;

            if (!_isStartedBefore && !string.IsNullOrEmpty(tboxWrite.Text))
            {
                _isStartedBefore = true;
                _dispatcherTimer.Start();
            }
            else if (tboxWrite.Text != string.Empty && !_isTextBoxChangedCanFire || tboxWrite.Text == " ")
            {
                tboxWrite.Text = string.Empty;
                return;
            }

            _numberOfKeyStrokes++;

            int currentLength = _currentTextOfTextbox.Length;

            if (currentLength <= _targetText.Length)
            {

                string currentTargetText = _targetText.Substring(0, currentLength);

                if (currentTargetText != _currentTextOfTextbox)
                    CurrentTextFalse(_words1[_currentWordIndex]);
                else
                    CurrentTextRight(_words1[_currentWordIndex]);

            }
            else
                CurrentTextFalse(_words1[_currentWordIndex]);


        }
        private void tboxWrite_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            _targetText = _words1[_currentWordIndex].Content.ToString();

            if (e.Key == Key.Escape)
            {
                RefreshGame();
                return;
            }

            if (e.Key == Key.Space && tboxWrite.Text != string.Empty)
            {
                int lastIndex = _words1.Count;

                if (_currentWordIndex == lastIndex - 1)
                {
                    _currentWordIndex = 0;
                    GetAnotherStack(_words2);
                    return;
                }

                _isTextBoxChangedCanFire = false;

                if (_currentTextOfTextbox == _targetText)
                    MarkedAsCorrect(_words1[_currentWordIndex], _words1[_currentWordIndex + 1]);
                else
                    MarkedAsIncorrect(_words1[_currentWordIndex], _words1[_currentWordIndex + 1]);

            }
            else
                _isTextBoxChangedCanFire = true;

        }

        private void GetAnotherStack(List<Label> words2)
        {
            stckPanel1.Children.Clear();
            stckPanel2.Children.Clear();
            _words1 = new List<Label>();

            for (int i = 0; i < words2.Count; i++)
            {
                stckPanel1.Children.Insert(i, words2[i]);
                _words1.Add(words2[i]);
            }

            tboxWrite.Text = String.Empty;
            RefreshStack(stckPanel2, _words2);
        }

        private void MarkedAsCorrect(Label label, Label nextLabel)
        {
            _numberOfTrueWords++;

            label.Background = Brushes.Transparent;
            label.Foreground = Brushes.Green;
            nextLabel.Background = Brushes.LightGray;

            _currentWordIndex++;
        }

        private void MarkedAsIncorrect(Label label, Label nextLabel)
        {
            _numbeOfWrongWords++;

            label.Background = Brushes.Transparent;
            label.Foreground = Brushes.Red;
            nextLabel.Background = Brushes.LightGray;

            _currentWordIndex++;
        }

        private void CurrentTextFalse(Label label)
        {
            label.Background = Brushes.Red;
            _numberOfFalseKeyStrokes++;
        }
        private void CurrentTextRight(Label label)
        {
            label.Background = Brushes.LightGray;
            _numberOfTrueKeyStrokes++;
        }


    }
}
