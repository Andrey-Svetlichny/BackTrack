namespace Visualizer.model
{
    public class Process
    {
        public string Name { get; set; }

        public int Color { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
