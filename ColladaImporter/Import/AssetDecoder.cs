using ColladaSharp.Models;
using ColladaSharp.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;
using static ColladaSharp.Definition.Collada;
using static ColladaSharp.Definition.Collada.COLLADA.LibraryControllers.Controller;
using static ColladaSharp.Definition.Collada.COLLADA.LibraryGeometries;
using static ColladaSharp.Definition.Collada.COLLADA.LibraryGeometries.Geometry.Mesh;
using static ColladaSharp.Definition.Collada.COLLADA.LibraryGeometries.Geometry.Mesh.Polylist;
using static ColladaSharp.Definition.Collada.COLLADA.LibraryVisualScenes;
using static ColladaSharp.Definition.Collada.Source;

namespace ColladaSharp
{
    public unsafe partial class Collada
    {
        static PrimitiveData DecodePrimitivesWeighted(
            VisualScene scene,
            Matrix4 bindMatrix,
            Geometry geo,
            Skin skin)
        {
            Matrix4 bindShapeMatrix = skin.BindShapeMatrixElement?.StringContent?.Value ?? Matrix4.Identity;
            InfluenceDef[] infList = CreateInfluences(skin, scene);
            DecodePrimitives(geo, bindMatrix * bindShapeMatrix, infList,
                out VertexShaderDesc info, out List<VertexPrimitive> lines, out List<VertexPolygon> faces);
            return CreateData(info, lines, faces);
        }

        public static InfluenceDef[] CreateInfluences(Skin skin, VisualScene scene)
        {
            Bone[] boneList;
            Bone bone = null;
            int boneCount;

            var joints = skin.JointsElement;
            var influences = skin.VertexWeightsElement;
            var boneCounts = influences.BoneCountsElement;
            var prims = influences.PrimitiveIndicesElement;

            InfluenceDef[] infList = new InfluenceDef[influences.Count];

            //Find joint source
            string[] jointSIDs = null;
            foreach (InputUnshared inp in joints.GetChildren<InputUnshared>())
                if (inp.CommonSemanticType == ESemantic.JOINT && inp.Source.GetElement(inp.Root) is Source src)
                {
                    jointSIDs = src.GetChild<NameArray>().StringContent.Values;
                    break;
                }

            if (jointSIDs == null)
                return null;

            //Populate bone list
            boneCount = jointSIDs.Length;
            boneList = new Bone[boneCount];
            for (int i = 0; i < boneCount; i++)
            {
                string sid = jointSIDs[i];
                var node = scene.FindNode(sid);

                if (node != null && node.UserData is Bone b)
                    boneList[i] = b;
                else
                    WriteLine(string.Format("Bone '{0}' not found", sid));
            }

            //Build input command list
            float[] pWeights = null;
            byte[] pCmd = new byte[influences.InputElements.Length];
            foreach (InputShared inp in influences.InputElements)
            {
                switch (inp.CommonSemanticType)
                {
                    case ESemantic.JOINT:
                        pCmd[inp.Offset] = 1;
                        break;

                    case ESemantic.WEIGHT:
                        pCmd[inp.Offset] = 2;

                        Source src = inp.Source.GetElement<Source>(inp.Root);
                        pWeights = src.GetArrayElement<FloatArray>().StringContent.Values;

                        break;

                    default:
                        pCmd[inp.Offset] = 0;
                        break;
                }
            }

            float weight = 0;
            int[] boneIndices = boneCounts.StringContent.Values;
            int[] primIndices = prims.StringContent.Values;
            for (int i = 0, primIndex = 0; i < influences.Count; i++)
            {
                InfluenceDef inf = new InfluenceDef();
                for (int boneIndex = 0; boneIndex < boneIndices[i]; boneIndex++)
                {
                    for (int cmd = 0; cmd < pCmd.Length; cmd++, primIndex++)
                    {
                        int index = primIndices[primIndex];
                        switch (pCmd[cmd])
                        {
                            case 1: //JOINT
                                bone = boneList[index];
                                break;
                            case 2: //WEIGHT
                                weight = pWeights[index];
                                break;
                        }
                    }
                    inf.AddWeight(new BoneWeight(bone.Name, weight));
                }
                inf.Normalize();
                infList[i] = inf;
            }
            return infList;
        }

        public static PrimitiveData DecodeMorphedPrimitivesUnweighted(Matrix4 bindMatrix, Morph morph)
        {
            if (!(morph.BaseMeshUrl.GetElement(morph.Root) is Geometry baseMesh))
            {
                WriteLine("Morph base mesh '" + morph.BaseMeshUrl.TargetID + "' does not point to a valid geometry entry.");
                return null;
            }

            DecodePrimitives(baseMesh, bindMatrix, null,
                out VertexShaderDesc baseInfo, out List<VertexPrimitive> baseLines, out List<VertexPolygon> baseFaces);

            var targets = morph.TargetsElement;
            InputUnshared morphTargets = null, morphWeights = null;
            foreach (InputUnshared input in targets.InputElements)
            {
                switch (input.CommonSemanticType)
                {
                    case ESemantic.MORPH_TARGET: morphTargets = input; break;
                    case ESemantic.MORPH_WEIGHT: morphWeights = input; break;
                }
            }

            Source targetSource = morphTargets?.Source?.GetElement<Source>(morphTargets.Root);
            Source weightSource = morphWeights?.Source?.GetElement<Source>(morphWeights.Root);

            NameArray nameArray = targetSource?.GetArrayElement<NameArray>();
            FloatArray weightArray = weightSource?.GetArrayElement<FloatArray>();

            string[] geomIds = nameArray?.StringContent?.Values;
            float[] weights = weightArray?.StringContent?.Values;

            if (geomIds == null || weights == null)
            {
                WriteLine("Morph set for '" + morph.BaseMeshUrl.TargetID + "' does not have valid target and weight inputs.");
                return null;
            }
            int count = geomIds.Length;
            if (geomIds.Length != weights.Length)
            {
                WriteLine("Morph set for '" + morph.BaseMeshUrl.TargetID + "' does not have a target count that matches weight count.");
                count = Math.Min(geomIds.Length, weights.Length);
            }

            Geometry geom;
            List<VertexPrimitive>[] morphLines = new List<VertexPrimitive>[count];
            List<VertexPolygon>[] morphFaces = new List<VertexPolygon>[count];

            for (int i = 0; i < count; ++i)
            {
                geom = targets.Root.GetIDEntry<Geometry>(geomIds[i]);
                DecodePrimitives(geom, bindMatrix, null,
                    out VertexShaderDesc info, out List<VertexPrimitive> lines, out List<VertexPolygon> faces);
                morphLines[i] = lines;
                morphFaces[i] = faces;
            }

            //TODO: create data using weights and morphLines or morphFaces array

            return CreateData(baseInfo, baseLines, baseFaces);
        }

        public static PrimitiveData DecodeMorphedPrimitivesWeighted(VisualScene scene, Matrix4 bindMatrix, Morph morphController, Skin skin)
        {
            Matrix4 bindShapeMatrix = skin.BindShapeMatrixElement?.StringContent?.Value ?? Matrix4.Identity;
            InfluenceDef[] infList = CreateInfluences(skin, scene);

            return null;
        }

        public static PrimitiveData DecodePrimitivesUnweighted(Matrix4 bindMatrix, Geometry geo)
        {
            DecodePrimitives(geo, bindMatrix, null, out VertexShaderDesc info, out List<VertexPrimitive> lines, out List<VertexPolygon> faces);
            return CreateData(info, lines, faces);
        }
        public static void DecodePrimitives(
            Geometry geo,
            Matrix4 bindMatrix,
            InfluenceDef[] infList,
            out VertexShaderDesc info,
            out List<VertexPrimitive> lines,
            out List<VertexPolygon> faces)
        {
            info = VertexShaderDesc.JustPositions();
            lines = new List<VertexPrimitive>();
            faces = new List<VertexPolygon>();

            Source src;
            int boneCount = 0;
            if (infList != null)
            {
                HashSet<string> bones = new HashSet<string>();
                foreach (InfluenceDef inf in infList)
                    for (int i = 0; i < inf.WeightCount; ++i)
                        bones.Add(inf.Weights[i].Bone);
                boneCount = bones.Count;
            }
            info.BoneCount = boneCount;

            var m = geo.MeshElement;
            if (m == null)
                return;

            Vertices vertsElem;
            foreach (var prim in m.PrimitiveElements)
            {
                Dictionary<ESemantic, int> semanticCounts = new Dictionary<ESemantic, int>();
                Dictionary<ESemantic, Dictionary<int, Source>> inputSources = new Dictionary<ESemantic, Dictionary<int, Source>>();
                Dictionary<ESemantic, Source> vertexInputSources = new Dictionary<ESemantic, Source>();
                foreach (InputShared inp in prim.InputElements)
                {
                    if (inp.CommonSemanticType == ESemantic.VERTEX)
                    {
                        vertsElem = inp.Source.GetElement<Vertices>(inp.Root);
                        foreach (InputUnshared input in vertsElem.InputElements)
                        {
                            ESemantic semantic = input.CommonSemanticType;
                            if (semanticCounts.ContainsKey(semantic))
                                ++semanticCounts[semantic];
                            else
                                semanticCounts.Add(semantic, 1);

                            src = input.Source.GetElement<Source>(vertsElem.Root);
                            vertexInputSources[input.CommonSemanticType] = src;
                        }
                        continue;
                    }
                    else
                    {
                        ESemantic semantic = inp.CommonSemanticType;
                        if (semanticCounts.ContainsKey(semantic))
                            ++semanticCounts[semantic];
                        else
                            semanticCounts.Add(semantic, 1);

                        src = inp.Source.GetElement<Source>(inp.Root);
                        if (src != null)
                        {
                            if (!inputSources.ContainsKey(semantic))
                                inputSources.Add(semantic, new Dictionary<int, Source>());

                            int set = (int)inp.Set;
                            if (!inputSources[semantic].ContainsKey(set))
                                inputSources[semantic].Add(set, src);
                            else
                                inputSources[semantic][set] = src;
                        }
                    }
                }

                info.MorphCount = 0; //Morphs are stored in separate geometry entries, so they need to be combined later
                info.HasNormals = semanticCounts.ContainsKey(ESemantic.NORMAL) && semanticCounts[ESemantic.NORMAL] > 0;

                bool hasTexBinormal = semanticCounts.ContainsKey(ESemantic.TEXBINORMAL) && semanticCounts[ESemantic.TEXBINORMAL] > 0;
                bool hasBinormal = semanticCounts.ContainsKey(ESemantic.BINORMAL) && semanticCounts[ESemantic.BINORMAL] > 0;
                info.HasBinormals = hasTexBinormal || hasBinormal;

                bool hasTexTangent = semanticCounts.ContainsKey(ESemantic.TEXTANGENT) && semanticCounts[ESemantic.TEXTANGENT] > 0;
                bool hasTangent = semanticCounts.ContainsKey(ESemantic.TANGENT) && semanticCounts[ESemantic.TANGENT] > 0;
                info.HasTangents = hasTexTangent || hasTangent;
                
                info.ColorCount = semanticCounts.ContainsKey(ESemantic.COLOR) ? semanticCounts[ESemantic.COLOR] : 0;
                info.TexcoordCount = semanticCounts.ContainsKey(ESemantic.TEXCOORD) ? semanticCounts[ESemantic.TEXCOORD] : 0;

                int maxSets = Math.Max(info.MorphCount + 1, 
                    Math.Max(info.ColorCount, info.TexcoordCount));

                Vertex[][] vertices = new Vertex[prim.PointCount][];
                int[] indices = prim?.IndicesElement?.StringContent?.Values;
                if (indices == null)
                {
                    WriteLine("Mesh has no face indices. Mesh will be empty.");
                    return;
                }

                Matrix4 invTranspBindMatrix = bindMatrix;
                if (info.HasNormals || info.HasBinormals || info.HasTangents)
                {
                    invTranspBindMatrix.Invert();
                    invTranspBindMatrix.Transpose();
                }

                foreach (var inp in prim.InputElements)
                {
                    int set = (int)inp.Set;
                    int offset = (int)inp.Offset;

                    if (inp.CommonSemanticType == ESemantic.VERTEX)
                    {
                        foreach (ESemantic s in vertexInputSources.Keys)
                        {
                            src = vertexInputSources[s];
                            DecodeSource(src, s, offset, set, maxSets, prim, indices, vertices, infList, bindMatrix, invTranspBindMatrix);
                        }
                    }
                    else
                    {
                        src = inputSources[inp.CommonSemanticType][set];
                        DecodeSource(src, inp.CommonSemanticType, offset, set, maxSets, prim, indices, vertices, infList, bindMatrix, invTranspBindMatrix);
                    }
                }

                int setIndex = 0;
                switch (prim.Type)
                {
                    case EColladaPrimitiveType.Lines:

                        VertexLine[] linesTemp = new VertexLine[vertices.Length / 2];
                        for (int i = 0, x = 0; i < vertices.Length; i += 2, ++x)
                            linesTemp[x] = new VertexLine(vertices[i][setIndex], vertices[i + 1][setIndex]);
                        lines.AddRange(linesTemp);

                        break;

                    case EColladaPrimitiveType.Linestrips:
                        lines.Add(new VertexLineStrip(false, vertices.Select(x => x[setIndex]).ToArray()));
                        break;

                    case EColladaPrimitiveType.Triangles:

                        VertexTriangle[] tris = new VertexTriangle[vertices.Length / 3];

                        for (int i = 0, x = 0; i < vertices.Length; i += 3, ++x)
                            tris[x] = new VertexTriangle(
                                vertices[i][setIndex],
                                vertices[i + 1][setIndex],
                                vertices[i + 2][setIndex]);

                        faces.AddRange(tris);
                        break;

                    case EColladaPrimitiveType.Trifans:
                        faces.Add(new VertexTriangleFan(vertices.Select(x => x[setIndex]).ToArray()));
                        break;

                    case EColladaPrimitiveType.Tristrips:
                        faces.Add(new VertexTriangleStrip(vertices.Select(x => x[setIndex]).ToArray()));
                        break;

                    case EColladaPrimitiveType.Polylist:
                        Polylist polyListPrim = (Polylist)prim;
                        PolyCounts countsElem = polyListPrim.PolyCountsElement;
                        int[] counts = countsElem.StringContent.Values;

                        VertexPolygon[] polys = new VertexPolygon[counts.Length];

                        for (int vtxIndex = 0, polyIndex = 0; polyIndex < counts.Length; ++polyIndex)
                        {
                            int count = counts[polyIndex];
                            Vertex[] verts = new Vertex[count];
                            for (int polyVtxIndex = 0; polyVtxIndex < count; ++polyVtxIndex, ++vtxIndex)
                                verts[polyVtxIndex] = vertices[vtxIndex][setIndex];
                            polys[polyIndex] = new VertexPolygon(verts);
                        }

                        faces.AddRange(polys);
                        break;

                    default:
                    case EColladaPrimitiveType.Polygons:
                        WriteLine($"Primitive type '{prim.Type.ToString()}' not supported. Mesh will be empty.");
                        break;
                }
            }
        }

        private static void DecodeSource(
            Source src,
            ESemantic semantic,
            int offset,
            int set,
            int maxSets,
            BasePrimitive prim,
            int[] indices,
            Vertex[][] vertices,
            InfluenceDef[] infList,
            Matrix4 bindMatrix,
            Matrix4 invTranspBindMatrix)
        {
            var acc = src.TechniqueCommonElement.AccessorElement;
            int stride = (int)acc.Stride;
            int startIndex, pointIndex;

            float[] list = src.GetArrayElement<FloatArray>().StringContent.Values;
            for (int i = 0, x = 0; i < prim.PointCount; ++i, x += prim.InputElements.Length)
            {
                if (vertices[i] == null)
                    vertices[i] = new Vertex[maxSets];

                startIndex = (pointIndex = indices[x + offset]) * stride;

                Vertex vtx = vertices[i][set];
                if (vtx == null)
                    vtx = new Vertex();

                switch (semantic)
                {
                    case ESemantic.POSITION:
                        Vec3 position = new Vec3(
                            list[startIndex],
                            list[startIndex + 1],
                            list[startIndex + 2]);
                        position = Vec3.TransformPosition(position, bindMatrix);
                        vtx.Position = position;
                        if (infList != null)
                            vtx.Influence = infList[pointIndex];
                        break;
                    case ESemantic.NORMAL:
                        Vec3 normal = new Vec3(
                            list[startIndex],
                            list[startIndex + 1],
                            list[startIndex + 2]);
                        vtx.Normal = Vec3.TransformVector(normal, invTranspBindMatrix);
                        break;
                    case ESemantic.BINORMAL:
                    case ESemantic.TEXBINORMAL:
                        Vec3 binormal = new Vec3(
                            list[startIndex],
                            list[startIndex + 1],
                            list[startIndex + 2]);
                        vtx.Binormal = Vec3.TransformVector(binormal, invTranspBindMatrix);
                        break;
                    case ESemantic.TANGENT:
                    case ESemantic.TEXTANGENT:
                        Vec3 tangent = new Vec3(
                            list[startIndex],
                            list[startIndex + 1],
                            list[startIndex + 2]);
                        vtx.Tangent = Vec3.TransformVector(tangent, invTranspBindMatrix);
                        break;
                    case ESemantic.TEXCOORD:
                        vtx.TexCoord = new Vec2(
                            list[startIndex],
                            ImportOptions.InvertTexCoordY ? 
                            1.0f - list[startIndex + 1] : list[startIndex + 1]);
                        break;
                    case ESemantic.COLOR:
                        vtx.Color = new ColorF4(
                            list[startIndex],
                            list[startIndex + 1],
                            list[startIndex + 2],
                            list[startIndex + 3]);
                        break;
                }
                vertices[i][set] = vtx;
            }
        }

        public static PrimitiveData CreateData(VertexShaderDesc info, List<VertexPrimitive> lines, List<VertexPolygon> faces)
        {
            if (faces.Count > 0)
            {
                if (lines.Count > 0)
                    WriteLine("Mesh has both lines and triangles. Only triangles will be shown in this case - PrimitiveData only supports lines OR triangles.");

                return PrimitiveData.FromTriangleList(info, faces.SelectMany(x => x.ToTriangles()));
            }
            else if (lines != null && lines.Count > 0)
            {
                return PrimitiveData.FromLineList(info, lines.SelectMany(
                    x => x is VertexLineStrip strip ? strip.ToLines() : new VertexLine[] { (VertexLine)x }));
            }

            WriteLine("Mesh has no primitives.");

            return null;//PrimitiveData.FromTriangles(VertexShaderDesc.JustPositions());
        }
    }
}
