using System;
using System.Collections.Generic;

namespace ColladaSharp.Models
{
    public class IndexNgon : IndexPolygon
    {
        public override EFaceType Type => EFaceType.Ngon;

        public IndexNgon() { }
        public IndexNgon(params IndexPoint[] points) { }

        public override List<IndexTriangle> ToTriangles()
        {
            //TODO: generate triangles from polygon with an arbitrary amount of edges
            throw new NotImplementedException();
        }
    }
}
