using System;
using System.IO;
using System.Threading;

namespace BackTrack
{
    class Tracker
    {
        const int IdleThreshold = 60 * 1000; // 60 sec
        private readonly string _logPath;
        private readonly Timer _timer;
        private ActiveApp _lastActivity;

        public Tracker(string logPath)
        {
            _logPath = logPath;
            _timer = new Timer(e =>
            {
                try
                {
                    Track();
                }
                catch (Exception ex)
                {
                    OnException(ex);
                }
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        public void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void Track()
        {
            long idleTime = WinApi.GetIdleTime();
            if (idleTime > IdleThreshold)
            {
                if (_lastActivity == null || !_lastActivity.IsIdle)
                {
                    var activity = new ActiveApp
                    {
                        DateTime = DateTime.Now.AddMilliseconds(-idleTime),
                        IsIdle = true
                    };
                    _lastActivity = activity;
                    WriteActivity(activity);
                }
            }
            else
            {
                var activity = new ActiveApp
                {
                    DateTime = DateTime.Now,
                    ProcessName = WinApi.GetActiveWindowProcessName(),
                    WindowTitle = WinApi.GetActiveWindowTitle()
                };

                if (_lastActivity == null || _lastActivity.ProcessName != activity.ProcessName ||
                    _lastActivity.WindowTitle != activity.WindowTitle)
                {
                    _lastActivity = activity;
                    WriteActivity(activity);
                }
            }
        }

        private void WriteActivity(ActiveApp activity)
        {
            using (var streamWriter = File.AppendText(_logPath))
            {
                streamWriter.WriteLine(activity.ToCsv());
            }
        }

        private void OnException(Exception ex)
        {
        }
    }
}
