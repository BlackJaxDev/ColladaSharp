namespace ColladaSharp.Models
{
    public class VertexTriangle : VertexPolygon
    {
        public Vertex Vertex0 => _vertices[0];
        public Vertex Vertex1 => _vertices[1];
        public Vertex Vertex2 => _vertices[2];

        //private VertexLine _e01, _e12, _e20;

        public override EFaceType Type => EFaceType.Triangles;

        /// <summary>
        ///    2
        ///   / \
        ///  /   \
        /// 0-----1
        /// </summary>
        public VertexTriangle(Vertex v0, Vertex v1, Vertex v2) : base(v0, v1, v2)
        {
            //_e01 = v0.LinkTo(v1);
            //_e12 = v1.LinkTo(v2);
            //_e20 = v2.LinkTo(v0);

            //_e01.AddFace(this);
            //_e12.AddFace(this);
            //_e20.AddFace(this);
        }

        public override VertexTriangle[] ToTriangles()
            => new VertexTriangle[] { this };
        public override VertexLine[] ToLines() 
            => new VertexLine[] 
            {
                new VertexLine(Vertex0, Vertex1),
                new VertexLine(Vertex1, Vertex2),
                new VertexLine(Vertex2, Vertex0),
            };
    }
}
