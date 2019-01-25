using System.Collections.Generic;
using System.Linq;

namespace ColladaSharp.Models
{
    public enum EFaceType
    {
        Points,
        Lines,
        LineStrip,
        LineLoop,
        Triangles,
        TriangleStrip,
        TriangleFan,
        Quads,
        QuadStrip,
        Ngon
    }
    public abstract class IndexPrimitive
    {
        public IndexPrimitive(params IndexPoint[] points) => _points = points.ToList();

        protected List<IndexPoint> _points = new List<IndexPoint>();

        public abstract EFaceType Type { get; }
        public IReadOnlyCollection<IndexPoint> Points => _points;
    }
    public abstract class IndexPolygon : IndexPrimitive
    {
        public IndexPolygon(params IndexPoint[] points) : base(points) { }

        public abstract List<IndexTriangle> ToTriangles();

        public bool ContainsEdge(IndexLine edge, out bool polygonIsCCW)
        {
            for (int i = 0; i < _points.Count; ++i)
            {
                if (_points[i] == edge.Point0)
                {
                    if (i + 1 < _points.Count && _points[i + 1] == edge.Point1)
                    {
                        polygonIsCCW = true;
                        return true;
                    }
                    else if (i - 1 >= 0 && _points[i - 1] == edge.Point1)
                    {
                        polygonIsCCW = false;
                        return true;
                    }
                }
            }
            polygonIsCCW = true;
            return false;
        }
    }
}
