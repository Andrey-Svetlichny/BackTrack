using System;

namespace Visualizer
{
    class DateTimeToIntConverter
    {
        private readonly DateTime _minDateTime;
        private readonly int _minInt;
        private readonly double _scale;

        public DateTimeToIntConverter(DateTime minDateTime, DateTime maxDateTime, int minInt, int maxInt)
        {
            _minDateTime = minDateTime;
            _minInt = minInt;
            _scale = 1.0 * (maxInt - minInt) / (maxDateTime - minDateTime).Ticks;
        }

        public int Convert(DateTime value)
        {
            var x = _minInt + (value - _minDateTime).Ticks * _scale;
            return System.Convert.ToInt32(x);
        }

        public DateTime ConvertBack(int value)
        {
            var ticks = (value - _minInt) / _scale;
            var dateTime = _minDateTime.AddTicks((long) ticks);
            return dateTime;
        }
    }
}
