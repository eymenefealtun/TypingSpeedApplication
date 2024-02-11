﻿using System;
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
            RefreshStack(stckPanel1, new List<Label>());
            RefreshStack(stckPanel2, new List<Label>());

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
                lbl.Background = i == 0 ? Brushes.LightGray : Brushes.Transparent;
                lbl.Content = _resultWordArray[i];

                //lbl.Style


                lbl.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                lbl.Arrange(new Rect(lbl.DesiredSize));

                currentLength += Convert.ToInt32(lbl.ActualWidth) + (int)lbl.Margin.Left;

                if (currentLength > stckPanel1.Width)
                    break;

                labels.Add(lbl);
                panel.Children.Insert(i,lbl);

            }

        }


    }
}
