namespace ColladaSharp.Models
{
    public class VertexLine : VertexPrimitive
    {
        public Vertex Vertex0 => _vertices[0];
        public Vertex Vertex1 => _vertices[1];

        public override EFaceType Type => EFaceType.Triangles;

        //private List<VertexTriangle> _triangles = new List<VertexTriangle>();

        /// <summary>
        ///    2
        ///   / \
        ///  /   \
        /// 0-----1
        /// </summary>
        public VertexLine(Vertex v0, Vertex v1) : base(v0, v1)
        {
            //Vertex0.AddLine(this);
            //Vertex1.AddLine(this);
        }

        internal void AddFace(VertexTriangle face)
        {
            //_triangles.Add(face);
        }

        //internal void Unlink()
        //{
        //    Vertex0.RemoveLine(this);
        //    Vertex1.RemoveLine(this);
        //}
    }
}
