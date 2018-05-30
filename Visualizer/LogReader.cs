using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Visualizer.model;

namespace Visualizer
{
    class LogReader
    {
        class ParsedLine
        {
            public DateTime DateTime { get; set; }

            public string ProcessName { get; set; }

            public string WindowTitle { get; set; }

            public bool IsIdle { get; set; }
        }

        public List<ActiveApp> Read(string path)
        {
            var apps = new List<ActiveApp>();
            var processes = new List<Process>();

            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                var parsedLine = Parse(line);

                var process = processes.FirstOrDefault(o => o.Name == parsedLine.ProcessName);
                if (process == null)
                {
                    process = new Process
                    {
                        Name = parsedLine.ProcessName
                    };
                    processes.Add(process);
                }

                var activeApp = new ActiveApp
                {
                    DateTime = parsedLine.DateTime,
                    Process = process,
                    WindowTitle = parsedLine.WindowTitle,
                    IsIdle = parsedLine.IsIdle
                };
                apps.Add(activeApp);
            }

            SetColors(processes);
            return apps;
        }

        private void SetColors(List<Process> processes)
        {
            for (int i = 0; i < processes.Count; i++)
            {
                // skip first two - black and white
                var n = i % (Constants.GetColorCount() - 2) + 2;
                processes[i].Color = Constants.GetColor(n);
            }
        }

        private ParsedLine Parse(string line)
        {
            const string idle = "Idle";
            var n1 = line.IndexOf("; ", StringComparison.InvariantCulture);
            var dateTimeStr = line.Substring(0, n1);
            var dateTime = DateTime.Parse(dateTimeStr);
            var n2 = line.IndexOf("; ", n1 + 2, StringComparison.InvariantCulture);
            var isIdleStr = line.Substring(n1 + 2, n2 - n1 - 2);
            var isIdle = isIdleStr == idle;
            var n3 = line.IndexOf("; ", n2 + 2, StringComparison.InvariantCulture);
            var processName = line.Substring(n2 + 2, n3 - n2 - 2);
            var windowTitle = line.Substring(n3 + 2);

            return new ParsedLine
            {
                DateTime = dateTime,
                ProcessName = processName,
                WindowTitle = windowTitle,
                IsIdle = isIdle
            };
        }
    }
}
