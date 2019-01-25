namespace ColladaSharp.Models
{
    public class Material
    {
        public string Name { get; set; }
        public TexRef[] Textures { get; set; }
    }
    public abstract class TexRef
    {

    }
    public class TexRefPath : TexRef
    {
        public TexRefPath(string absolutePath) => AbsolutePath = absolutePath;
        public string AbsolutePath { get; set; }
    }
    public class TexRefInternal : TexRef
    {
        public TexRefInternal(byte[] bytes, string format)
        {
            Bytes = bytes;
            Format = format;
        }
        
        public byte[] Bytes { get; set; }
        public string Format { get; set; }
    }
}