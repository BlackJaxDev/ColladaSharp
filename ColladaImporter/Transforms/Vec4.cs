using System;
using System.Globalization;
using System.Runtime.InteropServices;
using XMLSchemaDefinition;

namespace ColladaSharp.Transforms
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Vec4 : IEquatable<Vec4>, IParsable
    {
        public float X, Y, Z, W;

        public float* Data { get { fixed (Vec4* p = &this) return (float*)p; } }

        public Vec4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
        public Vec4(float value)
            : this(value, value, value, value) { }
        public Vec4(Vec2 v1, Vec2 v2)
            : this(v1.X, v2.Y, v2.X, v2.Y) { }
        public Vec4(Vec2 v, float z, float w)
            : this(v.X, v.Y, z, w) { }
        public Vec4(float x, float y, Vec2 v)
            : this(x, y, v.X, v.Y) { }
        public Vec4(float x, Vec3 v)
            : this(x, v.X, v.Y, v.Z) { }
        public Vec4(Vec3 v, float w)
            : this(v.X, v.Y, v.Z, w) { }
        public Vec4(string s, params char[] delimiters)
        {
            X = Y = Z = W = 0.0f;

            char[] delims = delimiters != null && delimiters.Length > 0 ? delimiters : new char[] { ',', '(', ')', ' ' };
            string[] arr = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);

            if (arr.Length >= 4)
            {
                float.TryParse(arr[0], NumberStyles.Any, CultureInfo.InvariantCulture, out X);
                float.TryParse(arr[1], NumberStyles.Any, CultureInfo.InvariantCulture, out Y);
                float.TryParse(arr[2], NumberStyles.Any, CultureInfo.InvariantCulture, out Z);
                float.TryParse(arr[3], NumberStyles.Any, CultureInfo.InvariantCulture, out W);
            }
        }

        public float this[int index]
        {
            get
            {
                if (index < 0 || index > 3)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                return Data[index];
            }
            set
            {
                if (index < 0 || index > 3)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                Data[index] = value;
            }
        }

        public static readonly Vec4 UnitX = new Vec4(1.0f, 0.0f, 0.0f, 0.0f);
        public static readonly Vec4 UnitY = new Vec4(0.0f, 1.0f, 0.0f, 0.0f);
        public static readonly Vec4 UnitZ = new Vec4(0.0f, 0.0f, 1.0f, 0.0f);
        public static readonly Vec4 UnitW = new Vec4(0.0f, 0.0f, 0.0f, 1.0f);
        public static readonly Vec4 Zero = new Vec4(0.0f);
        public static readonly Vec4 One = new Vec4(1.0f);
        
        public float LengthSquared => X * X + Y * Y + Z * Z + W * W;
        public float Length => (float)Math.Sqrt(LengthSquared);

        public Vec3 Xyz => new Vec3(X, Y, Z);

        public static Vec4 operator -(Vec4 v) 
            => new Vec4(-v.X, -v.Y, -v.Z, -v.W);
        public static Vec4 operator +(Vec4 left, Vec4 right)
            => new Vec4(
                left.X + right.X,
                left.Y + right.Y,
                left.Z + right.Z,
                left.W + right.W);
        public static Vec4 operator -(Vec4 left, Vec4 right)
            => new Vec4(
                left.X - right.X,
                left.Y - right.Y,
                left.Z - right.Z,
                left.W - right.W);
        public static Vec4 operator *(Vec4 vec, float scale)
            => new Vec4(
                vec.X * scale,
                vec.Y * scale,
                vec.Z * scale,
                vec.W * scale);
        public static Vec4 operator /(Vec4 vec, float scale)
            => vec * (1.0f / scale);
        public static Vec4 operator *(float scale, Vec4 vec)
            => vec * scale;
        public static Vec4 operator *(Vec4 vec, Vec4 scale)
            => new Vec4(
                vec.X * scale.X,
                vec.Y * scale.Y,
                vec.Z * scale.Z,
                vec.W * scale.W);

        public static bool operator ==(Vec4 left, Vec4 right)
            => left.Equals(right);
        public static bool operator !=(Vec4 left, Vec4 right)
            => !left.Equals(right);

        public override string ToString() => ToString();
        public string ToString(string openingBracket = "(", string closingBracket = ")", string separator = ", ")
           => string.Format("{5}{0}{4}{1}{4}{2}{4}{3}{6}", X, Y, Z, W, separator, openingBracket, closingBracket);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                hashCode = (hashCode * 397) ^ W.GetHashCode();
                return hashCode;
            }
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Vec4))
                return false;

            return Equals((Vec4)obj);
        }
        public bool Equals(Vec4 other)
        {
            return
                X == other.X &&
                Y == other.Y &&
                Z == other.Z &&
                W == other.W;
        }
        public bool Equals(Vec4 other, float precision)
        {
            return
                Math.Abs(X - other.X) < precision &&
                Math.Abs(Y - other.Y) < precision &&
                Math.Abs(Z - other.Z) < precision &&
                Math.Abs(W - other.W) < precision;
        }
        public string WriteToString()
            => ToString("", "", " ");
        public void ReadFromString(string str)
            => this = new Vec4(str);
    }
}
