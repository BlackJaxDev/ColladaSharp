using System.ComponentModel;

namespace ColladaSharp
{
    public class ColladaExportOptions
    {
        //[Category("Primitives")]
        //public bool ReverseWinding { get; set; } = false;
        [Category("Primitives")]
        public bool IgnoreBinormals { get; set; } = true;
        [Category("Primitives")]
        public bool IgnoreTangents { get; set; } = true;
        /// <summary>
        /// Some implementations of texture coordinates (like OpenGL)
        /// have the origin at the bottom left instead of the top left (like DirectX).
        /// </summary>
        [Description("Some implementations of texture coordinates (like OpenGL) " +
            "have the origin at the bottom left instead of the top left (like DirectX).")]
        [Category("Import")]
        public bool InvertTexCoordY { get; set; }
    }
}
