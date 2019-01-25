namespace ColladaSharp.Models
{
    public class BoneWeight
    {
        public static float ComparisonTolerance = 0.00001f;
        
        public string Bone { get; set; }
        public float Weight { get; set; }
        
        public BoneWeight() : this(null, 1.0f) { }
        public BoneWeight(string bone) : this(bone, 1.0f) { }
        public BoneWeight(string bone, float weight) { Bone = bone; Weight = weight; }

        public void ClampWeight() => Weight = Weight.Clamp(0.0f, 1.0f);

        public override bool Equals(object obj)
            => Equals(obj as BoneWeight, ComparisonTolerance);
        public bool Equals(BoneWeight other, float weightTolerance)
            => other != null ? (other.Bone == Bone && Weight.EqualTo(other.Weight, weightTolerance)) : false;

        public static bool operator ==(BoneWeight left, BoneWeight right)
        {
            if (left is null)
                return right is null;
            return left.Equals(right);
        }
        public static bool operator !=(BoneWeight left, BoneWeight right)
        {
            if (left is null)
                return !(right is null);
            return !left.Equals(right);
        }
        public override int GetHashCode() => base.GetHashCode();
    }
}
