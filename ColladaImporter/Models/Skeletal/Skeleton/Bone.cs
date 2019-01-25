using ColladaSharp.Transforms;
using System.Collections;
using System.Collections.Generic;

namespace ColladaSharp.Models
{
    public class Bone : IReadOnlyList<Bone>
    {
        public Bone(string name, TRSTransform bindState)
        {
            Name = name;
            _bindState = bindState.HardCopy();
        }

        private Bone _parent;
        private List<Bone> _children = new List<Bone>();
        private TRSTransform _bindState;
        
        public Bone Parent
        {
            get => _parent;
            set
            {
                if (_parent != null)
                {
                    Skeleton = null;
                    _parent._children.Remove(this);
                }
                _parent = value;
                if (_parent != null)
                {
                    Skeleton = _parent.Skeleton;
                    _parent._children.Add(this);
                }
            }
        }
        
        public string Name { get; set; }
        public Skeleton Skeleton { get; private set; }

        /// <summary>
        /// This is the matrix transform from the origin of the model.
        /// </summary>
        public Matrix4 BindMatrix { get; private set; } = Matrix4.Identity;
        /// <summary>
        /// This is the inverse matrix transform from the origin of the model.
        /// </summary>
        public Matrix4 InverseBindMatrix { get; private set; } = Matrix4.Identity;
        /// <summary>
        /// This is the local transformation of this bone relative to its parent's origin.
        /// </summary>
        public TRSTransform BindState
        {
            get => _bindState;
            set
            {
                _bindState = value;
                RecalcBindMatrix(Skeleton);
            }
        }

        internal void RecalcBindMatrix(Skeleton skeleton)
        {
            Skeleton = skeleton;
            if (_parent == null)
                RecalcBindMatrix(skeleton, Matrix4.Identity, Matrix4.Identity);
            else
                RecalcBindMatrix(skeleton, _parent.BindMatrix, _parent.InverseBindMatrix);
        }
        private void RecalcBindMatrix(Skeleton skeleton, Matrix4 parentMatrix, Matrix4 inverseParentMatrix)
        {
            BindMatrix = parentMatrix * _bindState.Matrix;
            InverseBindMatrix = _bindState.InverseMatrix * inverseParentMatrix;
            
            foreach (Bone bone in _children)
                bone.RecalcBindMatrix(skeleton, BindMatrix, InverseBindMatrix);
        }

        public int Count => ((IReadOnlyList<Bone>)_children).Count;
        public Bone this[int index] => ((IReadOnlyList<Bone>)_children)[index];

        public IEnumerator<Bone> GetEnumerator() => ((IReadOnlyList<Bone>)_children).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IReadOnlyList<Bone>)_children).GetEnumerator();
    }
}