using System.ComponentModel;

namespace ColladaSharp.Transforms
{
    /// <summary>
    /// Represents a [translate -> rotate -> scale] transformation, in that order.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TRSTransform
    {
        public delegate void MatrixChange(Matrix4 oldMatrix, Matrix4 oldInvMatrix);
        public event MatrixChange MatrixChanged;
        
        public static TRSTransform GetIdentity() => new TRSTransform(Vec3.Zero, Quat.Identity, Vec3.One);

        public TRSTransform() 
            : this(Vec3.Zero, Quat.Identity, Vec3.One) { }
        public TRSTransform(Vec3 translation, Quat rotation, Vec3 scale)
            => SetAll(translation, rotation, scale);
                
        private Vec3 _translation = Vec3.Zero;
        private Quat _rotation = Quat.Identity;
        private Vec3 _scale = Vec3.One;
        private Matrix4 _mtx = Matrix4.Identity;
        private Matrix4 _inv = Matrix4.Identity;

        public Matrix4 Matrix
        {
            get => _mtx;
            set
            {
                _mtx = value;
                _inv = _mtx.Inverted();
                DeriveMatrix();
            }
        }
        public Matrix4 InverseMatrix
        {
            get => _inv;
            set
            {
                _inv = value;
                _mtx = _inv.Inverted();
                DeriveMatrix();
            }
        }
        public Vec3 Translation
        {
            get => _translation;
            set
            {
                _translation = value;
                CreateMatrices();
            }
        }
        public Vec3 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                CreateMatrices();
            }
        }
        public Quat Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                CreateMatrices();
            }
        }
        public void SetAll(Vec3 translation, Quat rotation, Vec3 scale)
        {
            _translation = translation;
            _rotation = rotation;
            _scale = scale;
            CreateMatrices();
        }
        public void CreateMatrices()
        {
            Matrix4 oldMatrix = _mtx;
            Matrix4 oldInvMatrix = _inv;

            _rotation.ToAxisAngleRad(out Vec3 rotAxis, out float rotRad);

            _mtx = 
                Matrix4.CreateTranslation(_translation) *
                Matrix4.CreateFromAxisAngleRad(rotAxis, rotRad) *
                Matrix4.CreateScale(_scale);

            _inv =
                Matrix4.CreateScale(1.0f / _scale) *
                Matrix4.CreateFromAxisAngleRad(rotAxis, -rotRad) *
                Matrix4.CreateTranslation(-_translation);

            MatrixChanged?.Invoke(oldMatrix, oldInvMatrix);
        }
        private void DeriveMatrix()
            => DeriveTRS(_mtx, out _translation, out _scale, out _rotation);
        public static void DeriveTRS(Matrix4 matrix, out Vec3 translation, out Vec3 scale, out Quat rotation)
        {
            translation = matrix.Row3.Xyz;
            scale = new Vec3(matrix.Row0.Xyz.Length, matrix.Row1.Xyz.Length, matrix.Row2.Xyz.Length);
            rotation = matrix.ExtractRotation(true);
        }
        public TRSTransform HardCopy()
            => new TRSTransform(Translation, Rotation, Scale);
        public override string ToString()
        {
            return $"{Translation.ToString()} {Rotation.ToString()} {Scale.ToString()}";
        }
    }
}
