using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using BackTrack.Properties;

namespace BackTrack
{
    static class Program
    {
        private static string _logPath;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            _logPath = args.Length > 0 ? args[0] : Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, "log");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CustomApplicationContext(_logPath));
        }
    }

    public class CustomApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;
        private readonly Tracker _tracker;

        public CustomApplicationContext(string logPath)
        {
            // Initialize Tray Icon
            _trayIcon = new NotifyIcon
            {
                Icon = Resources.AppIcon,
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Exit", Exit)
                }),
                Text = "BackTrack",
                Visible = true
            };

            _tracker = new Tracker(logPath);
        }

        void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            _trayIcon.Visible = false;
            _tracker.Stop();
            Application.Exit();
        }
    }
}
