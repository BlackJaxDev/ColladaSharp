using System.Collections.Generic;

namespace ColladaSharp.Models
{
    public class IndexTriangle : IndexPolygon
    {
        public IndexTriangle() { }

        public override EFaceType Type => EFaceType.Triangles;

        public IndexPoint Point0 => _points[0];
        public IndexPoint Point1 => _points[1];
        public IndexPoint Point2 => _points[2];

        // Counter-clockwise winding
        //     2
        //    / \
        //   /   \
        //  0-----1

        /// <summary>
        /// Creates an index triangle with counter-clockwise winding.
        /// </summary>
        /// <param name="point0">On an equilateral triangle, this is the bottom left point.</param>
        /// <param name="point1">On an equilateral triangle, this is the bottom right point.</param>
        /// <param name="point2">On an equilateral triangle, this is the top point.</param>
        public IndexTriangle(IndexPoint point0, IndexPoint point1, IndexPoint point2)
        {
            _points.Add(point0);
            _points.Add(point1);
            _points.Add(point2);
        }

        public override List<IndexTriangle> ToTriangles()
            => new List<IndexTriangle>() { this };

        public override bool Equals(object obj)
            => !(obj is IndexTriangle t) ? false : t.Point0 == Point0 && t.Point1 == Point1 && t.Point2 == Point2;
        
        public override int GetHashCode()
            => base.GetHashCode();
    }
}
