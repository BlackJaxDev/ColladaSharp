using System.Collections.Generic;

namespace ColladaSharp.Models
{
    public class IndexQuad : IndexPolygon
    {
        public IndexQuad() { }

        readonly bool _forwardSlash = false;

        public override EFaceType Type => EFaceType.Quads;
        public IndexPoint Point0 => _points[0];
        public IndexPoint Point1 => _points[1];
        public IndexPoint Point2 => _points[2];
        public IndexPoint Point3 => _points[3];

        public IndexQuad(IndexPoint point0, IndexPoint point1, IndexPoint point2, IndexPoint point3, bool forwardSlash = false)
            : base(point0, point1, point2, point3) => _forwardSlash = forwardSlash;
        
        public override List<IndexTriangle> ToTriangles()
        {
            // Counter-clockwise winding
            // 3--2        2       3--2      3          3--2
            // |  |  =    /|  and  | /   OR  |\    and   \ |
            // |  |      / |       |/        | \          \|
            // 0--1     0--1       0         0--1          1

            List<IndexTriangle> triangles = new List<IndexTriangle>();
            if (_forwardSlash)
            {
                triangles.Add(new IndexTriangle(Point0, Point1, Point2));
                triangles.Add(new IndexTriangle(Point0, Point2, Point3));
            }
            else
            {
                triangles.Add(new IndexTriangle(Point0, Point1, Point3));
                triangles.Add(new IndexTriangle(Point3, Point1, Point2));
            }
            return triangles;
        }
    }
}
