using System.Collections.Generic;

namespace ColladaSharp.Models
{
    public class Model
    {
        public Model() : base() { }
        public Model(string name) { Name = name; }
        
        public string Name { get; set; }
        public List<SubMesh> Children { get; } = new List<SubMesh>();
        public Skeleton Skeleton { get; set; }
    }
}
