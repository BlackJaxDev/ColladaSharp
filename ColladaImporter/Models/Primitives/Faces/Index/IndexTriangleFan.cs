using System;
using System.Collections.Generic;

namespace ColladaSharp.Models
{
    public class IndexTriangleFan : IndexPolygon
    {
        public IndexTriangleFan(params IndexPoint[] points) : base(points) { }

        public override EFaceType Type => EFaceType.TriangleFan;
        public override List<IndexTriangle> ToTriangles()
        {
            //TODO: convert triangle fan to triangles
            throw new NotImplementedException();
        }
    }
}
