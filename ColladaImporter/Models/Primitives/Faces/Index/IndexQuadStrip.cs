using System;
using System.Collections.Generic;

namespace ColladaSharp.Models
{
    public class IndexQuadStrip : IndexPolygon
    {
        public override EFaceType Type => EFaceType.QuadStrip;

        public IndexQuadStrip() { }
        public IndexQuadStrip(params IndexPoint[] points) { }

        public override List<IndexTriangle> ToTriangles()
        {
            //TODO: triangulate quad strip
            throw new NotImplementedException();
        }
    }
}
