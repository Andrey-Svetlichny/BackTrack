using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Visualizer.Annotations;
using Visualizer.model;
using Visualizer.Model;

namespace Visualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        static Image image;
        List<ActiveApp> apps;
        static DateTimeToIntConverter converter;
        private static TextBlock textBlock;
        TimeChart timeChart;

        public MainWindow()
        {
            TagStrip = new TagStrip();
            InitializeComponent();

            ReadLog();

            image = this.Image;
            textBlock = this.TextBlock;

            Loaded += delegate
            {
                var margin = Border.Margin.Left + Border.Margin.Right + Border.BorderThickness.Left + Border.BorderThickness.Right;
                timeChart = new TimeChart((int)(this.Grid.ActualWidth - margin), (int)50);
                this.Image.Source = timeChart.Bitmap;

                FillAppIndex(apps, timeChart.Width);
                DrawChart(apps);

                image.MouseMove += Image_MouseMove;
                image.MouseDown += Image_MouseDown;
            };
        }

        private int selectionStart, selectionEnd;
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DrawSelection(clear:true);
            int x = (int)e.GetPosition(image).X;
            selectionStart = x;
        }

        // приложения, соответствующие каждой точке графика по горизонтали
        int[] _appIndex;
        private List<ActiveApp> _selectedApps;

        private void FillAppIndex(List<ActiveApp> apps, int width)
        {
            converter = new DateTimeToIntConverter(apps.Min(o => o.DateTime), apps.Max(o => o.DateTime), 0, width);
            _appIndex = new int[width];

            var x1 = 0;
            for (int i = 0; i < apps.Count; i++)
            {
                var x2 = converter.Convert(apps[i].DateTime);
                if (i != 0)
                {
                    for (int x = x1; x < x2; x++)
                    {
                        _appIndex[x] = i - 1;
                    }
                }
                x1 = x2;
            }
        }

        private void DrawChart(List<ActiveApp> apps)
        {
            int x1 = 0, x2 = 1;
            for (; x2 < _appIndex.Length; x2++)
            {
                if (_appIndex[x2] != _appIndex[x1])
                {
                    timeChart.DrawRect(x1, x2, 0, timeChart.Height, apps[_appIndex[x1]].Process.Color);
                    x1 = x2;
                }
            }
            timeChart.DrawRect(x1, x2, 0, timeChart.Height, apps[_appIndex[x1]].Process.Color);
        }

        private void ReadLog()
        {
            var logReader = new LogReader();
            var allApps = logReader.Read(@"..\..\Data\BackTrack.log");

            var date = allApps.Min(a => a.DateTime.Date);
            apps = allApps.Where(o => o.DateTime.Date == date).ToList();
        }


        public TagStrip TagStrip { get; set; }

        void Image_MouseMove(object sender, MouseEventArgs e)
        {
            int x = (int)e.GetPosition(image).X;
            if (x >= timeChart.Width)
            {
                return;
            }

            DrawPointer(x);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                selectionEnd = x;
                DrawSelection();
                ShowSelectionApps();
            }
        }

        public List<ActiveApp> SelectedApps
        {
            get { return _selectedApps; }
            set
            {
                if (Equals(value, _selectedApps)) return;
                _selectedApps = value;
                OnPropertyChanged();
            }
        }

        private void ShowSelectionApps()
        {
            var x1 = selectionStart < selectionEnd ? selectionStart : selectionEnd;
            var x2 = selectionStart < selectionEnd ? selectionEnd : selectionStart;
            var appId1 = _appIndex[x1];
            var appId2 = _appIndex[x2];
            SelectedApps = apps.Skip(appId1).Take(appId2 - appId1 + 1).ToList();
        }

        private void DrawSelection(bool clear = false)
        {
            var x1 = selectionStart < selectionEnd ? selectionStart : selectionEnd;
            var x2 = selectionStart < selectionEnd ? selectionEnd : selectionStart;
            var color = clear ? Constants.GetColor(0) : Constants.GetColor(1);
            timeChart.DrawLineHorizontal(x1, x2, timeChart.Height - 1, color);
        }

        private static int _pointerX = -1;
        private void DrawPointer(int x)
        {
            if (_pointerX != -1)
            {
                var oldAppId = _appIndex[_pointerX];
                var oldApp = apps[oldAppId];
                timeChart.DrawLineVertical(_pointerX, 0, timeChart.Height - 10, oldApp.Process.Color);
            }

            _pointerX = x;
            timeChart.DrawLineVertical(x, 0, timeChart.Height - 10, Constants.GetColor(1));

            var appId = _appIndex[x];
            var app = apps[appId];
            var begin = app.DateTime;
            DateTime? end = null;
            if (appId < apps.Count)
            {
                var nextApp = apps[appId + 1];
                end = nextApp.DateTime;
            }
            textBlock.Text = $"x={x}, dateTime={begin} - {end} {app}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void EnterTag_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) return;

            var x1 = selectionStart < selectionEnd ? selectionStart : selectionEnd;
            var x2 = selectionStart < selectionEnd ? selectionEnd : selectionStart;
            var appId1 = _appIndex[x1];
            var appId2 = _appIndex[x2];
            if (appId2 < apps.Count)
            {
                appId2++;
            }
            TagStrip.Set(apps[appId1].DateTime, apps[appId2].DateTime, TextBoxTag.Text);
        }
    }
}
