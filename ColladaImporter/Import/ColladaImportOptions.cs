using ColladaSharp.Definition;
using ColladaSharp.Transforms;
using System.ComponentModel;

namespace ColladaSharp
{
    public class ColladaImportOptions
    {
        //[Category("Primitives")]
        //public bool ReverseWinding { get; set; } = false;
        //[Category("Primitives")]
        //public float WeightPrecision { get => _weightPrecision; set => _weightPrecision = value.Clamp(0.0000001f, 0.999999f); }
        //[Category("Primitives")]
        //public ETexWrapMode TexCoordWrap { get; set; } = ETexWrapMode.Repeat;
        [Category("Primitives")]
        public bool GenerateBinormals { get; set; } = true;
        [Category("Primitives")]
        public bool GenerateTangents { get; set; } = true;

        /// <summary>
        /// Determines how the model should be scaled, rotated and translated.
        /// </summary>
        [Description("Determines how the model should be scaled, rotated and translated.")]
        [Category("Import")]
        public TRSTransform InitialTransform { get; set; } = TRSTransform.GetIdentity();
        /// <summary>
        /// Dictates what information to ignore.
        /// Ignoring certain elements can give a decent parsing speed boost.
        /// </summary>
        [Description("Dictates what information to ignore. " +
            "Ignoring certain elements can give a decent parsing speed boost.")]
        [Category("Import")]
        public EIgnoreFlags IgnoreFlags { get; set; } = EIgnoreFlags.None;
        /// <summary>
        /// Some implementations of texture coordinates (like OpenGL)
        /// have the origin at the bottom left instead of the top left (like DirectX).
        /// </summary>
        [Description("Some implementations of texture coordinates (like OpenGL) " +
            "have the origin at the bottom left instead of the top left (like DirectX).")]
        [Category("Import")]
        public bool InvertTexCoordY { get; set; }

        //private float _weightPrecision = 0.0001f;
    }
    public enum ETexWrapMode
    {
        /// <summary>
        /// Out-of-range image coordinates are remapped back into range.
        /// </summary>
        Repeat,
        /// <summary>
        ///  Out-of-range image coordinates will return the border color.
        /// </summary>
        ClampToBorder,
        /// <summary>
        /// Out-of-range image coordinates are clamped to the extent of the image.
        /// The border color is not sampled.
        /// </summary>
        ClampToEdge,
        /// <summary>
        /// Out-of-range image coordinates are remapped back into range.
        /// Every repetition is reversed.
        /// </summary>
        MirroredRepeat,
    }
}
