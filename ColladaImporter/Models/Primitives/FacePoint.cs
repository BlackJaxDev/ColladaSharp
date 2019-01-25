using System.Collections.Generic;

namespace ColladaSharp.Models
{
    public class FacePoint
    {
        public FacePoint(int index, PrimitiveData data)
        {
            Index = index;
            Owner = data;
        }

        public int Index { get; set; }
        public int InfluenceIndex { get; set; }
        public List<int> BufferIndices { get; } = new List<int>();
        public PrimitiveData Owner { get; }

        public InfluenceDef GetInfluence()
            => Owner?.Influences != null && Owner.Influences.IndexInRange(InfluenceIndex) ? Owner.Influences[InfluenceIndex] : null;
        
        public override int GetHashCode() => ToString().GetHashCode();
        public override string ToString()
        {
            string fp = "FP" + Index;
            if (BufferIndices.Count > 0)
                fp += ": ";
            for (int i = 0; i < BufferIndices.Count; ++i)
                fp += "(" + i + ": " + BufferIndices[i] + ")";
            return fp;
        }
    }
}
