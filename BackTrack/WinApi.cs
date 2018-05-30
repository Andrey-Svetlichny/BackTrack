using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;


namespace BackTrack
{
    class WinApi
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("Kernel32.dll")]
        private static extern uint GetLastError();


        [DllImport("Kernel32.dll")]
        private static extern uint GetTickCount();

        internal struct LASTINPUTINFO
        {
            public uint cbSize;

            public uint dwTime;
        }

        public static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            var sb = new StringBuilder(nChars);
            IntPtr hWnd = GetForegroundWindow();

            if (GetWindowText(hWnd, sb, nChars) > 0)
            {
                return sb.ToString();
            }
            return null;
        }

        public static string GetActiveWindowProcessName()
        {
            IntPtr hWnd = GetForegroundWindow();
            uint procId;
            GetWindowThreadProcessId(hWnd, out procId);
            var proc = Process.GetProcessById((int)procId);
            return proc.ProcessName;

            // win64
            //return proc.MainModule.ToString();
        }

        public static long GetLastInputTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            if (!GetLastInputInfo(ref lastInPut))
            {
                throw new Exception(GetLastError().ToString());
            }

            return lastInPut.dwTime;
        }

        /*
         * Idle time, ms
         */
        public static long GetIdleTime()
        {
            return GetTickCount() - GetLastInputTime();
        }
    }
}
