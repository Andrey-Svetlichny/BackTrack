using System;

namespace BackTrack
{
    class ActiveApp
    {
        public DateTime DateTime { get; set; }

        public string ProcessName { get; set; }

        public string WindowTitle { get; set; }

        public bool IsIdle { get; set; }

        public string ToCsv()
        {
            var idle = IsIdle ? "T" : "F";
            return $"{DateTime}; {idle}; {EncodeCsv(ProcessName)}; {EncodeCsv(WindowTitle)}";
        }

        private string EncodeCsv(string s)
        {
            if (s == null)
                return null;

            char[] csvTokens = new[] { '\"', ',', '\n', '\r' };
            if (s.IndexOfAny(csvTokens) >= 0)
            {
                s = "\"" + s.Replace("\"", "\"\"") + "\"";
            }

            return s;
        }
    }
}
