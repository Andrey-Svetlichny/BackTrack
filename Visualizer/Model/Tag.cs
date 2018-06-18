using System;

namespace Visualizer.Model
{
    public class Tag
    {
        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        public TagText TagText { get; set; }
    }

    public class TagText
    {
        public string Text { get; set; }
    }
}
