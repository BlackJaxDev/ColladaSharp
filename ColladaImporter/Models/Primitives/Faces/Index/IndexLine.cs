namespace ColladaSharp.Models
{
    public class IndexLine
    {
        public IndexLine() { }
        public IndexLine(IndexPoint point1, IndexPoint point2)
        {
            Point0 = point1;
            Point1 = point2;
        }
        public IndexPoint Point0 { get; private set; }
        public IndexPoint Point1 { get; private set; }
    }
}
