namespace ColladaSharp.Models
{
    public class Skeleton
    {
        public Skeleton() : base() { }
        public Skeleton(params Bone[] rootBones) : base()
            => RootBones = rootBones;

        private Bone[] _rootBones;
        public Bone[] RootBones
        {
            get => _rootBones;
            set
            {
                _rootBones = value;
                RecalcBindMatrices();
            }
        }

        public void RecalcBindMatrices()
        {
            if (RootBones != null)
                foreach (Bone b in RootBones)
                    b.RecalcBindMatrix(this);
        }
    }
}
