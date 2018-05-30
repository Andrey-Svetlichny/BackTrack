using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Visualizer.model;

namespace Visualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Image image;
        List<ActiveApp> apps;
        static DateTimeToIntConverter converter;
        private static TextBlock textBlock;
        TimeChart timeChart;

        public MainWindow()
        {
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


                image.MouseMove += new MouseEventHandler(i_MouseMove);

            };

        }

        // приложения, соответствующие каждой точке графика по горизонтали
        int[] _appIndex;

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


        private static int oldX = -1;
        void i_MouseMove(object sender, MouseEventArgs e)
        {

            int x = (int)e.GetPosition(image).X;
            int y = (int)e.GetPosition(image).Y;

            if (x >= timeChart.Width)
            {
                return;
            }

            if (oldX != -1)
            {
                var oldAppId = _appIndex[oldX];
                var oldApp = apps[oldAppId];
                timeChart.DrawLine(oldX, 0, timeChart.Height-10, oldApp.Process.Color);
            }

            oldX = x;
            timeChart.DrawLine(x, 0, timeChart.Height-10, Constants.GetColor(1));

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


            if (e.LeftButton == MouseButtonState.Pressed)
            {
                timeChart.DrawPixel(x, y);

            }


/*
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                ErasePixel(e);
            }
*/
        }



    }
}
