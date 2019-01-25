namespace ColladaSharp.Models
{
    public class IndexLineStrip : IndexPrimitive
    {
        private readonly bool _closed = false;

        public IndexLineStrip() { }
        public IndexLineStrip(bool closed, params IndexPoint[] points) 
            : base(points) { _closed = closed; }

        public override EFaceType Type => _closed ? EFaceType.LineLoop : EFaceType.LineStrip;
    }
}
