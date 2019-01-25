using ColladaSharp.Tools;
using ColladaSharp.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ColladaSharp.Models
{
    public partial class PrimitiveData
    {
        public VertexTriangle GetFace(int index)
        {
            IndexTriangle t = _triangles[index];
            FacePoint fp0 = _facePoints[t.Point0];
            FacePoint fp1 = _facePoints[t.Point1];
            FacePoint fp2 = _facePoints[t.Point2];
            Vertex v0 = new Vertex(fp0, _buffers);
            Vertex v1 = new Vertex(fp1, _buffers);
            Vertex v2 = new Vertex(fp2, _buffers);
            return new VertexTriangle(v0, v1, v2);
        }
        public void GenerateBinormalTangentBuffers(int positionIndex, int uvIndex, bool addBinormals, bool addTangents)
        {
            IDataBuffer[] pBuffs = GetAllBuffersOfType(EBufferType.Position);
            if (pBuffs.Length == 0)
            {
                Collada.WriteLine("No position buffers found.");
                return;
            }
            if (!pBuffs.IndexInRange(positionIndex))
            {
                Collada.WriteLine("Position index out of range of available position buffers.");
                return;
            }
            //IDataBuffer[] nBuffs = GetAllBuffersOfType(EBufferType.Normal);
            //if (nBuffs.Length == 0)
            //{
            //    Console.WriteLine("No normal buffers found.");
            //    return;
            //}
            //if (!nBuffs.IndexInRange(normalIndex))
            //{
            //    Console.WriteLine("Normal index out of range of available normal buffers.");
            //    return;
            //}
            IDataBuffer[] tBuffs = GetAllBuffersOfType(EBufferType.TexCoord);
            if (tBuffs.Length == 0)
            {
                Collada.WriteLine("No texcoord buffers found.");
                return;
            }
            if (!tBuffs.IndexInRange(uvIndex))
            {
                Collada.WriteLine("UV index out of range of available texcoord buffers.");
                return;
            }

            Vec3 pos1, pos2, pos3;
            //Vec3 n0, n1, n2;
            Vec2 uv1, uv2, uv3;

            IDataBuffer pBuff = pBuffs[positionIndex];
            //VertexBuffer nBuff = pBuffs[normalIndex];
            IDataBuffer tBuff = tBuffs[uvIndex];
            int pointCount = _triangles.Count * 3;
            List<Vec3> binormals = new List<Vec3>(pointCount);
            List<Vec3> tangents = new List<Vec3>(pointCount);
            
            for (int i = 0; i < _triangles.Count; ++i)
            {
                IndexTriangle t = _triangles[i];

                FacePoint fp0 = _facePoints[t.Point0];
                FacePoint fp1 = _facePoints[t.Point1];
                FacePoint fp2 = _facePoints[t.Point2];

                pos1 = (Vec3)pBuff[fp0.BufferIndices[pBuff.Index]];
                pos2 = (Vec3)pBuff[fp1.BufferIndices[pBuff.Index]];
                pos3 = (Vec3)pBuff[fp2.BufferIndices[pBuff.Index]];
                
                uv1 = (Vec2)tBuff[fp0.BufferIndices[tBuff.Index]];
                uv2 = (Vec2)tBuff[fp1.BufferIndices[tBuff.Index]];
                uv3 = (Vec2)tBuff[fp2.BufferIndices[tBuff.Index]];
                
                Vec3 deltaPos1 = pos2 - pos1;
                Vec3 deltaPos2 = pos3 - pos1;

                Vec2 deltaUV1 = uv2 - uv1;
                Vec2 deltaUV2 = uv3 - uv1;

                Vec3 tangent;
                Vec3 binormal;

                float m = deltaUV1.X * deltaUV2.Y - deltaUV1.Y * deltaUV2.X;
                if (m == 0.0f)
                {
                    tangent = Vec3.UnitY;
                    binormal = Vec3.UnitY;
                }
                else
                {
                    float r = 1.0f / m;
                    tangent = (deltaPos1 * deltaUV2.Y - deltaPos2 * deltaUV1.Y) * r;
                    binormal = (deltaPos2 * deltaUV1.X - deltaPos1 * deltaUV2.X) * r;
                }

                binormals.Add(binormal);
                binormals.Add(binormal);
                binormals.Add(binormal);

                tangents.Add(tangent);
                tangents.Add(tangent);
                tangents.Add(tangent);
            }

            AddBuffer(binormals, EBufferType.Binormal);
            AddBuffer(tangents, EBufferType.Tangent);
            _bufferInfo.HasBinormals = true;
            _bufferInfo.HasTangents = true;
        }
        private void SetInfluences(params InfluenceDef[] influences)
        {
            Remapper remap = new Remapper();
            remap.Remap(influences);
            for (int i = 0; i < remap.RemapTable.Length; ++i)
                _facePoints[i].InfluenceIndex = remap.RemapTable[i];
            _influences = new InfluenceDef[remap.ImplementationLength];
            for (int i = 0; i < remap.ImplementationLength; ++i)
                _influences[i] = influences[remap.ImplementationTable[i]];

            HashSet<string> utilized = new HashSet<string>();
            foreach (InfluenceDef inf in _influences)
                if (inf != null)
                    for (int i = 0; i < inf.WeightCount; ++i)
                        utilized.Add(inf.Weights[i].Bone);
            _utilizedBones = utilized.ToArray();
        }

        private void CreateFacePoints(int pointCount)
        {
            _facePoints = new List<FacePoint>();
            for (int i = 0; i < pointCount; ++i)
                _facePoints.Add(new FacePoint(i, this));
        }

        #region Buffers
        public IDataBuffer this[EBufferType type]
        {
            get => _buffers.FirstOrDefault(x => x.Type == type);
            set
            {
                value.Type = type;
                var buf = _buffers.FirstOrDefault(x => x.Type == type);
                if (buf != null)
                {
                    value.Index = buf.Index;
                    _buffers[buf.Index] = value;
                }
                else
                {
                    value.Index = _buffers.Count;
                    _buffers.Add(value);
                }
            }
        }

        public IDataBuffer this[string name]
        {
            get => _buffers.FirstOrDefault(x => x.Name == name);
            set
            {
                var buf = _buffers.FirstOrDefault(x => x.Name == name);
                if (buf != null)
                {
                    value.Index = buf.Index;
                    _buffers[buf.Index] = value;
                }
                else
                {
                    value.Index = _buffers.Count;
                    _buffers.Add(value);
                }
            }
        }

        public IDataBuffer this[int index]
        {
            get
            {
                if (index < 0 || index >= _buffers.Count)
                    throw new IndexOutOfRangeException();
                return _buffers[index];
            }
            set
            {
                if (index >= 0 && index < _buffers.Count)
                {
                    value.Index = index;
                    _buffers[index] = value;
                }
                else
                    throw new IndexOutOfRangeException();
            }
        }

        public IDataBuffer[] GetAllBuffersOfType(EBufferType type)
            => _buffers.Where(x => x.Type == type).ToArray();
        
        public DataBuffer<T> AddBuffer<T>(
            IList<T> bufferData,
            EBufferType type,
            bool remap = false,
            bool integral = false) where T : unmanaged
        {
            if (_buffers == null)
                _buffers = new List<IDataBuffer>();

            int bufferIndex = _buffers.Count;
            DataBuffer<T> buffer = new DataBuffer<T>(bufferData.ToArray(), false, integral)
            {
                Type = type,
                Index = bufferIndex,
            };
            if (remap)
            {
                Remapper remapper = new Remapper();
                remapper.Remap(bufferData);
                for (int i = 0; i < bufferData.Count; ++i)
                    _facePoints[i].BufferIndices.Add(remapper.RemapTable[i]);
            }
            else
            {
                for (int i = 0; i < bufferData.Count; ++i)
                    _facePoints[i].BufferIndices.Add(i);
            }
            _buffers.Add(buffer);
            return buffer;
        }
        #endregion

        #region Indices
        public int[] GetIndices()
        {
            switch (_type)
            {
                case EPrimitiveType.Triangles:
                    return _triangles?.SelectMany(x => new int[] { x.Point0, x.Point1, x.Point2 }).ToArray();
                case EPrimitiveType.Lines:
                    return _lines?.SelectMany(x => new int[] { x.Point0, x.Point1 }).ToArray();
                case EPrimitiveType.Points:
                    return _points?.Select(x => (int)x).ToArray();
            }
            return null;
        }
        private Remapper SetTriangleIndices(List<Vertex> vertices, bool remap = true)
        {
            if (vertices.Count % 3 != 0)
                throw new Exception("Vertex list needs to be a multiple of 3.");

            _triangles = new List<IndexTriangle>();
            if (remap)
            {
                Remapper remapper = new Remapper();
                remapper.Remap(vertices, null);
                for (int i = 0; i < remapper.RemapTable.Length;)
                {
                    _triangles.Add(new IndexTriangle(
                        remapper.RemapTable[i++],
                        remapper.RemapTable[i++],
                        remapper.RemapTable[i++]));
                }
                return remapper;
            }
            else
            {
                for (int i = 0; i < vertices.Count;)
                    _triangles.Add(new IndexTriangle(i++, i++, i++));
                return null;
            }
        }
        private Remapper SetLineIndices(List<Vertex> vertices, bool remap = true)
        {
            if (vertices.Count % 2 != 0)
                throw new Exception("Vertex list needs to be a multiple of 2.");

            _lines = new List<IndexLine>();
            if (remap)
            {
                Remapper remapper = new Remapper();
                remapper.Remap(vertices, null);
                for (int i = 0; i < remapper.RemapTable.Length;)
                {
                    _lines.Add(new IndexLine(
                        remapper.RemapTable[i++],
                        remapper.RemapTable[i++]));
                }
                return remapper;
            }
            else
            {
                for (int i = 0; i < vertices.Count;)
                    _lines.Add(new IndexLine(i++, i++));
                return null;
            }
        }
        private Remapper SetPointIndices(List<Vertex> vertices, bool remap = true)
        {
            _points = new List<IndexPoint>();
            if (remap)
            {
                Remapper remapper = new Remapper();
                remapper.Remap(vertices, null);
                for (int i = 0; i < remapper.RemapTable.Length;)
                    _points.Add(new IndexPoint(remapper.RemapTable[i++]));
                return remapper;
            }
            else
            {
                for (int i = 0; i < vertices.Count;)
                    _points.Add(new IndexPoint(i++));
                return null;
            }
        }
        #endregion
    }
}
