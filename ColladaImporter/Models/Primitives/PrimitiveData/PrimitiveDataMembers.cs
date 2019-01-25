using System.Collections.Generic;

namespace ColladaSharp.Models
{
    public partial class PrimitiveData
    {
        public bool HasSkinning => _utilizedBones == null ? false : _utilizedBones.Length > 0;
        public List<IDataBuffer> Buffers { get => _buffers; set => _buffers = value; }
        public InfluenceDef[] Influences { get => _influences; set => _influences = value; }
        public string[] UtilizedBones { get => _utilizedBones; set => _utilizedBones = value; }
        public List<FacePoint> FacePoints { get => _facePoints; set => _facePoints = value; }
        public List<IndexTriangle> Triangles { get => _triangles; set => _triangles = value; }
        public List<IndexLine> Lines { get => _lines; set => _lines = value; }
        public List<IndexPoint> Points { get => _points; set => _points = value; }
        public EPrimitiveType Type { get => _type; set => _type = value; }
        public VertexShaderDesc BufferInfo => _bufferInfo;
        
        internal string[] _utilizedBones;
        internal InfluenceDef[] _influences;
        internal VertexShaderDesc _bufferInfo;
        internal List<IDataBuffer> _buffers = null;
        internal List<FacePoint> _facePoints = null;
        internal List<IndexPoint> _points = null;
        internal List<IndexLine> _lines = null;
        internal List<IndexTriangle> _triangles = null;
        internal EPrimitiveType _type = EPrimitiveType.Triangles;
    }
}
