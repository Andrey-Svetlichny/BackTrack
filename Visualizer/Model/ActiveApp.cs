using System;

namespace Visualizer.model
{
    public class ActiveApp
    {
        public DateTime DateTime { get; set; }

        public Process Process { get; set; }

        public string WindowTitle { get; set; }

        public bool IsIdle { get; set; }

        public override string ToString()
        {
            return IsIdle ?
                $"{DateTime}; Idle; ":
                $"{DateTime}; ProcessName = {Process.Name}; WindowTitle = {WindowTitle}";
        }
    }
}
