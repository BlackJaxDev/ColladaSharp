namespace ColladaSharp.Models
{
    public class IndexPoint
    {
        public IndexPoint() { }
        public IndexPoint(int vertexIndex) => VertexIndex = vertexIndex;
        
        public int VertexIndex { get; }

        public override string ToString()
            => VertexIndex.ToString();
        
        public static implicit operator IndexPoint(int i) => new IndexPoint(i);
        public static implicit operator int(IndexPoint i) => i.VertexIndex;
    }
}
