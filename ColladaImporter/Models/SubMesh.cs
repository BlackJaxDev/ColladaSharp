namespace ColladaSharp.Models
{
    public class SubMesh
    {
        public string Name { get; set; }
        public PrimitiveData Primitives { get; set; }
        public (float, PrimitiveData)[] Morphs { get; set; }
        public Material Material { get; set; }

        public SubMesh() { }
        public SubMesh(
            string name,
            PrimitiveData primitives,
            Material material)
        {
            Name = name;
            Primitives = primitives;
            Material = material;
        }
    }
}
