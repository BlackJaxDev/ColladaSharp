using ColladaSharp.Tools;
using ColladaSharp.Transforms;
using System.Collections.Generic;
using System.Linq;

namespace ColladaSharp.Models
{
    public partial class PrimitiveData
    {
        public static PrimitiveData FromQuads(VertexShaderDesc info, params VertexQuad[] quads)
            => FromQuadList(info, quads);
        public static PrimitiveData FromQuadList(VertexShaderDesc info, IEnumerable<VertexQuad> quads)
            => FromTriangleList(info, quads.SelectMany(x => x.ToTriangles()));
        public static PrimitiveData FromTriangleStrips(VertexShaderDesc info, params VertexTriangleStrip[] strips)
            => FromTriangleStripList(info, strips);
        public static PrimitiveData FromTriangleStripList(VertexShaderDesc info, IEnumerable<VertexTriangleStrip> strips)
            => FromTriangleList(info, strips.SelectMany(x => x.ToTriangles()));
        public static PrimitiveData FromTriangleFans(VertexShaderDesc info, params VertexTriangleFan[] fans)
            => FromTriangleFanList(info, fans);
        public static PrimitiveData FromTriangleFanList(VertexShaderDesc info, IEnumerable<VertexTriangleFan> fans)
            => FromTriangleList(info, fans.SelectMany(x => x.ToTriangles()));
        public static PrimitiveData FromTriangles(VertexShaderDesc info, params VertexTriangle[] triangles)
            => FromTriangleList(info, triangles);
        public static PrimitiveData FromTriangleList(VertexShaderDesc info, IEnumerable<VertexTriangle> triangles)
        {
            //TODO: convert triangles to tristrips and use primitive restart to render them all in one call
            return new PrimitiveData(info, triangles.SelectMany(x => x.Vertices), EPrimitiveType.Triangles);
        }
        public static PrimitiveData FromLineStrips(VertexShaderDesc info, params VertexLineStrip[] lines)
            => FromLineStripList(info, lines);
        public static PrimitiveData FromLineStripList(VertexShaderDesc info, IEnumerable<VertexLineStrip> lines)
            => FromLineList(info, lines.SelectMany(x => x.ToLines()));
        public static PrimitiveData FromLines(VertexShaderDesc info, params VertexLine[] lines)
            => FromLineList(info, lines);
        public static PrimitiveData FromLineList(VertexShaderDesc info, IEnumerable<VertexLine> lines)
            => new PrimitiveData(info, lines.SelectMany(x => x.Vertices), EPrimitiveType.Lines);
        public static PrimitiveData FromPoints(params Vec3[] points)
            => FromPointList(points);
        public static PrimitiveData FromPointList(IEnumerable<Vec3> points)
            => new PrimitiveData(VertexShaderDesc.JustPositions(), points.Select(x => new Vertex(x)), EPrimitiveType.Points);
        
        public PrimitiveData() { }
        public PrimitiveData(VertexShaderDesc info, IEnumerable<Vertex> points, EPrimitiveType type)
        {
            _bufferInfo = info;
            _type = type;
            _influences = null;

            List<Vertex> vertices = points.ToList();
            Remapper remapper = null;
            switch (_type)
            {
                case EPrimitiveType.Triangles:
                    remapper = SetTriangleIndices(vertices);
                    break;
                case EPrimitiveType.Lines:
                    remapper = SetLineIndices(vertices);
                    break;
                default:
                //case EPrimitiveType.Points:
                    remapper = SetPointIndices(vertices);
                    break;
            }

            int[] firstAppearanceArray = null;
            if (remapper == null)
            {
                firstAppearanceArray = new int[vertices.Count];
                for (int i = 0; i < vertices.Count; ++i)
                    firstAppearanceArray[i] = i;
            }
            else
                firstAppearanceArray = remapper.ImplementationTable;

            CreateFacePoints(firstAppearanceArray.Length);

            if (info.HasSkinning)
                SetInfluences(firstAppearanceArray.Select(x => vertices[x].Influence).ToArray());

            for (int i = 0; i < info.MorphCount + 1; ++i)
            {
                var data = firstAppearanceArray.Select(x => vertices[x].Position).ToList();
                AddBuffer(data, EBufferType.Position);
            }
            if (info.HasNormals)
                for (int i = 0; i < info.MorphCount + 1; ++i)
                {
                    var data = firstAppearanceArray.Select(x => vertices[x].Normal).ToList();
                    AddBuffer(data, EBufferType.Normal);
                }
            if (info.HasBinormals)
                for (int i = 0; i < info.MorphCount + 1; ++i)
                {
                    var data = firstAppearanceArray.Select(x => vertices[x].Binormal).ToList();
                    AddBuffer(data, EBufferType.Binormal);
                }
            if (info.HasTangents)
                for (int i = 0; i < info.MorphCount + 1; ++i)
                {
                    var data = firstAppearanceArray.Select(x => vertices[x].Tangent).ToList();
                    AddBuffer(data, EBufferType.Tangent);
                }
            for (int i = 0; i < info.ColorCount; ++i)
            {
                var data = firstAppearanceArray.Select(x => vertices[x].Color).ToList();
                AddBuffer(data, EBufferType.Color);
            }
            for (int i = 0; i < info.TexcoordCount; ++i)
            {
                var data = firstAppearanceArray.Select(x => vertices[x].TexCoord).ToList();
                AddBuffer(data, EBufferType.TexCoord);
            }
        }
    }
    public enum ECulling
    {
        /// <summary>
        /// No faces will be invisible.
        /// </summary>
        None,
        /// <summary>
        /// Back faces will be invisible.
        /// </summary>
        Back,
        /// <summary>
        /// Front faces will be invisible.
        /// </summary>
        Front,
        /// <summary>
        /// All faces will be invisible.
        /// </summary>
        Both
    }
    public enum EPrimitiveType
    {
        Points = 0,
        Lines = 1,
        Triangles = 2,
    }
}
