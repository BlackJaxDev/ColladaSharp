using System;
using System.Collections.Generic;
using System.Linq;

namespace ColladaSharp.Models
{
    public class IndexTriangleStrip : IndexPolygon
    {
        public IndexTriangleStrip() { }

        public override EFaceType Type => EFaceType.TriangleStrip;
        
        public IndexTriangleStrip(params IndexPoint[] points)
        {
            if (points.Length < 3)
                throw new Exception("A triangle strip needs 3 or more points.");
            _points = points.ToList();
        }

        public override List<IndexTriangle> ToTriangles()
        {
            // Clockwise 0-1-2, counter-clockwise 1-2-3, clockwise 2-3-4, repeat
            //    1-----3
            //   / \   / \
            //  /   \ /   \
            // 0-----2-----4

            List<IndexTriangle> triangles = new List<IndexTriangle>();
            for (int i = 2; i < _points.Count; ++i)
            {
                bool cw = (i & 1) == 0;
                triangles.Add(new IndexTriangle(
                    _points[i - 2], 
                    _points[cw ? i : i - 1], 
                    _points[cw ? i - 1 : i]));
            }
            return triangles;
        }
    }
}
