namespace ColladaSharp.Models
{
    public class VertexShaderDesc
    {
        public static readonly int MaxMorphs = 0;
        public static readonly int MaxColors = 2;
        public static readonly int MaxTexCoords = 8;
        public static readonly int MaxOtherBuffers = 10;
        public static readonly int TotalBufferCount = (MaxMorphs + 1) * 6 + MaxColors + MaxTexCoords + MaxOtherBuffers;

        public VertexShaderDesc() { }

        //Note: if there's only one bone, we can just multiply the model matrix by the bone's frame matrix. No need for weighting.
        public bool IsWeighted => BoneCount > 1;
        public bool IsSingleBound => BoneCount == 1;
        public bool HasSkinning => BoneCount > 0;

        public bool HasNormals { get; set; } = false;
        public bool HasBinormals { get; set; } = false;
        public bool HasTangents { get; set; } = false;
        public bool HasTexCoords => TexcoordCount > 0;
        public bool HasColors => ColorCount > 0;

        public int MorphCount { get; set; } = 0;
        public int TexcoordCount { get; set; } = 0;
        public int ColorCount { get; set; } = 0;
        public int BoneCount { get; set; } = 0;

        public static VertexShaderDesc PosColor(int colorCount = 1)
            => new VertexShaderDesc() { ColorCount = colorCount };

        public static VertexShaderDesc PosTex(int texCoordCount = 1)
            => new VertexShaderDesc() { TexcoordCount = texCoordCount };

        public static VertexShaderDesc PosNormTex(int texCoordCount = 1)
            => new VertexShaderDesc() { TexcoordCount = texCoordCount, HasNormals = true };

        public static VertexShaderDesc PosNorm()
            => new VertexShaderDesc() { HasNormals = true };

        public static VertexShaderDesc JustPositions()
            => new VertexShaderDesc();
    }
}
