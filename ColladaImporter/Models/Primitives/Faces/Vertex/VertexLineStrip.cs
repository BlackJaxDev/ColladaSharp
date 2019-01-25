namespace ColladaSharp.Models
{
    public class VertexLineStrip : VertexPrimitive
    {
        public override EFaceType Type => ClosedLoop ? EFaceType.LineLoop : EFaceType.LineStrip;

        public bool ClosedLoop { get; set; }

        public VertexLineStrip(bool closedLoop, params Vertex[] vertices)
            : base(vertices) => ClosedLoop = closedLoop;
        
        public VertexLine[] ToLines()
        {
            int count = _vertices.Count;
            if (!ClosedLoop && count > 0)
                --count;
            VertexLine[] lines = new VertexLine[count];
            for (int i = 0; i < count; ++i)
            {
                Vertex next = i + 1 == _vertices.Count ? _vertices[0] : _vertices[i + 1];
                lines[i] = new VertexLine(_vertices[i], next);
            }
            return lines;
        }
    }
}
