using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ColladaSharp.Models
{
    public abstract class VertexPrimitive : IEnumerable<Vertex>
    {
        public abstract EFaceType Type { get; }
        public IReadOnlyList<Vertex> Vertices => _vertices;

        protected List<Vertex> _vertices = new List<Vertex>();

        public VertexPrimitive(List<Vertex> vertices) => _vertices = vertices;
        public VertexPrimitive(IEnumerable<Vertex> vertices) => _vertices = vertices.ToList();
        public VertexPrimitive(params Vertex[] vertices) => _vertices = vertices.ToList();
        
        public IEnumerator<Vertex> GetEnumerator() => ((IEnumerable<Vertex>)_vertices).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Vertex>)_vertices).GetEnumerator();
    }
    public class VertexPolygon : VertexPrimitive
    {
        public override EFaceType Type => EFaceType.Ngon;
        public VertexPolygon(params Vertex[] vertices) : base(vertices)
        {
            if (vertices.Length < 3)
                throw new InvalidOperationException("Not enough vertices for a polygon.");
        }
        public VertexPolygon(IEnumerable<Vertex> vertices) : base(vertices)
        {
            if (Vertices.Count < 3)
                throw new InvalidOperationException("Not enough vertices for a polygon.");
        }
        public virtual VertexTriangle[] ToTriangles()
        {
            // Example polygons:

            //   4----3
            //  /      \
            // 5        2
            //  \      /
            //   0----1
            // Converted: 012, 023, 034, 045

            // 3---2
            // |   |
            // 0---1
            // Converted: 012, 023

            int triangleCount = Vertices.Count - 2;
            if (triangleCount < 1)
                return new VertexTriangle[0];
            VertexTriangle[] list = new VertexTriangle[triangleCount];
            for (int i = 0; i < triangleCount; ++i)
                list[i] = new VertexTriangle(Vertices[0].HardCopy(), Vertices[i + 1].HardCopy(), Vertices[i + 2].HardCopy());
            return list;
        }
        public virtual VertexLine[] ToLines()
        {
            VertexLine[] lines = new VertexLine[Vertices.Count];
            for (int i = 0; i < Vertices.Count - 1; ++i)
                lines[i] = new VertexLine(Vertices[i].HardCopy(), Vertices[i + 1].HardCopy());
            lines[Vertices.Count - 1] = new VertexLine(Vertices[Vertices.Count - 1].HardCopy(), Vertices[0].HardCopy());
            return lines;
        }
    }
}
