using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using XMLSchemaDefinition;
using static ColladaSharp.Extensions;

namespace ColladaSharp.Transforms
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Vec3 : IEquatable<Vec3>, IParsable
    {
        public float X, Y, Z;

        public float* Data { get { fixed (Vec3* p = &this) return (float*)p; } }

        public Vec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Vec3(float value)
            : this(value, value, value) { }
        public Vec3(Vec2 v)
            : this(v.X, v.Y, 0.0f) { }
        public Vec3(Vec2 v, float z)
            : this(v.X, v.Y, z) { }
        public Vec3(float x, Vec2 v)
            : this(x, v.X, v.Y) { }
        public Vec3(Vec4 v, bool divideByW)
        {
            if (divideByW)
            {
                X = v.X / v.W;
                Y = v.Y / v.W;
                Z = v.Z / v.W;
            }
            else
            {
                X = v.X;
                Y = v.Y;
                Z = v.Z;
            }
        }
        public Vec3(string s, params char[] delimiters)
        {
            X = Y = Z = 0.0f;

            char[] delims = delimiters != null && delimiters.Length > 0 ? delimiters : new char[] { ',', '(', ')', ' ' };
            string[] arr = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);

            if (arr.Length >= 3)
            {
                float.TryParse(arr[0], NumberStyles.Any, CultureInfo.InvariantCulture, out X);
                float.TryParse(arr[1], NumberStyles.Any, CultureInfo.InvariantCulture, out Y);
                float.TryParse(arr[2], NumberStyles.Any, CultureInfo.InvariantCulture, out Z);
            }
        }

        public float this[int index]
        {
            get
            {
                if (index < 0 || index > 2)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                return Data[index];
            }
            set
            {
                if (index < 0 || index > 2)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                Data[index] = value;
            }
        }

        public static readonly Vec3 UnitX = new Vec3(1.0f, 0.0f, 0.0f);
        public static readonly Vec3 UnitY = new Vec3(0.0f, 1.0f, 0.0f);
        public static readonly Vec3 UnitZ = new Vec3(0.0f, 0.0f, 1.0f);
        public static readonly Vec3 Zero = new Vec3(0.0f);
        public static readonly Vec3 One = new Vec3(1.0f);

        //        |
        // normal |  /
        // l x r, | / right
        // -r x l |/_______ 
        //            left
        public Vec3 Cross(Vec3 right)
            => new Vec3(
                Y * right.Z - Z * right.Y,
                Z * right.X - X * right.Z,
                X * right.Y - Y * right.X);

        public float Dot(Vec3 other)
            => X * other.X + Y * other.Y + Z * other.Z;

        public float LengthSquared => Dot(this);
        public float Length => (float)Math.Sqrt(LengthSquared);

        public void Normalize(bool safe = true)
        {
            float lengthSq = LengthSquared;
            this *= 1.0f / (float)Math.Sqrt(lengthSq);
        }
        public Vec3 Normalized()
        {
            Vec3 value = this;
            value.Normalize();
            return value;
        }

        public static void AxisAngleBetween(Vec3 initialVector, Vec3 finalVector, out Vec3 axis, out float angle)
        {
            initialVector.Normalize();
            finalVector.Normalize();

            float dot = initialVector | finalVector;

            //dot is the cosine adj/hyp ratio between the two vectors, so
            //dot == 1 is same direction
            //dot == -1 is opposite direction
            //dot == 0 is a 90 degree angle

            if (dot > 0.999f)
            {
                axis = Vec3.UnitZ;
                angle = 0.0f;
            }
            else if (dot < -0.999f)
            {
                axis = Vec3.UnitZ;
                angle = 180.0f;
            }
            else
            {
                axis = initialVector ^ finalVector;
                angle = RadToDeg((float)Math.Acos(dot));
            }
        }

        #region Operators
        public static Vec3 operator +(float left, Vec3 right)
            => right + left;
        public static Vec3 operator +(Vec3 left, float right)
            => new Vec3(
                left.X + right,
                left.Y + right,
                left.Z + right);

        public static Vec3 operator -(float left, Vec3 right)
            => new Vec3(
                left - right.X,
                left - right.Y,
                left - right.Z);

        public static Vec3 operator -(Vec3 left, float right)
            => new Vec3(
                left.X - right,
                left.Y - right,
                left.Z - right);

        public static Vec3 operator +(Vec3 left, Vec3 right)
            => new Vec3(
                left.X + right.X,
                left.Y + right.Y,
                left.Z + right.Z);

        public static Vec3 operator -(Vec3 left, Vec3 right)
            => new Vec3(
                left.X - right.X,
                left.Y - right.Y,
                left.Z - right.Z);

        public static Vec3 operator -(Vec3 vec)
            => new Vec3(-vec.X, -vec.Y, -vec.Z);
        public static Vec3 operator *(Vec3 vec, float scale)
            => new Vec3(
                vec.X * scale,
                vec.Y * scale,
                vec.Z * scale);

        public static Vec3 operator *(float scale, Vec3 vec)
            => vec * scale;
        public static Vec3 operator *(Vec3 vec, Vec3 scale)
            => new Vec3(
                vec.X * scale.X,
                vec.Y * scale.Y,
                vec.Z * scale.Z);

        public static bool operator ==(Vec3 left, Vec3 right)
            => left.Equals(right);
        public static bool operator !=(Vec3 left, Vec3 right)
            => !left.Equals(right);
        public static bool operator <(Vec3 left, Vec3 right)
            =>
                left.X < right.X &&
                left.Y < right.Y &&
                left.Z < right.Z;
        public static bool operator >(Vec3 left, Vec3 right)
            =>
                left.X > right.X &&
                left.Y > right.Y &&
                left.Z > right.Z;

        public static bool operator <=(Vec3 left, Vec3 right)
            =>
                left.X <= right.X &&
                left.Y <= right.Y &&
                left.Z <= right.Z;

        public static bool operator >=(Vec3 left, Vec3 right)
            =>
                left.X >= right.X &&
                left.Y >= right.Y &&
                left.Z >= right.Z;
        public static Vec3 operator /(Vec3 vec, float scale)
        {
            scale = 1.0f / scale;
            return new Vec3(
                vec.X * scale,
                vec.Y * scale,
                vec.Z * scale);
        }
        public static Vec3 operator /(float scale, Vec3 vec)
            => new Vec3(
                scale / vec.X,
                scale / vec.Y,
                scale / vec.Z);

        public static Vec3 operator /(Vec3 left, Vec3 right)
            => new Vec3(
                left.X / right.X,
                left.Y / right.Y,
                left.Z / right.Z);

        //Cross Product:
        //        |
        // normal |  /
        // l x r, | / right
        // -r x l |/_______ 
        //            left

        /// <summary>
        /// Cross Product
        /// </summary>
        public static Vec3 operator ^(Vec3 vec1, Vec3 vec2)
            => vec1.Cross(vec2);

        /// <summary>
        /// Dot product; 
        /// 1 is same direction (0 degrees difference),
        /// -1 is opposite direction (180 degrees difference), 
        /// 0 is perpendicular (a 90 degree angle)
        /// </summary>
        public static float operator |(Vec3 vec1, Vec3 vec2)
            => vec1.Dot(vec2);

        //public static Vec3 operator *(Matrix4 left, Vec3 right)
        //    => TransformPerspective(left, right);

        //public static Vec3 operator *(Vec3 left, Matrix4 right)
        //    => TransformPerspective(left, right);

        public static explicit operator Vec3(Color c)
            => new Vec3(c.R * ByteToFloat, c.G * ByteToFloat, c.B * ByteToFloat);
        public static explicit operator Color(Vec3 v)
            => Color.FromArgb(v.X.ToByte(), v.Y.ToByte(), v.Z.ToByte());

        public static implicit operator Vec3(Vec2 v) => new Vec3(v.X, v.Y, 0.0f);
        public static explicit operator Vec3(ColorF4 v) => new Vec3(v.R, v.G, v.B);
        public static implicit operator Vec3(float v) => new Vec3(v);

        #endregion

        public override string ToString() => ToString();
        public string ToString(string openingBracket = "(", string closingBracket = ")", string separator = ", ")
            => string.Format("{4}{0}{3}{1}{3}{2}{5}", X, Y, Z, separator, openingBracket, closingBracket);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Vec3))
                return false;

            return Equals((Vec3)obj);
        }
        public bool Equals(Vec3 other)
            =>
                X == other.X &&
                Y == other.Y &&
                Z == other.Z;

        public bool Equals(Vec3 other, float precision)
            =>
                Math.Abs(X - other.X) < precision &&
                Math.Abs(Y - other.Y) < precision &&
                Math.Abs(Z - other.Z) < precision;

        public string WriteToString() => ToString();
        public void ReadFromString(string str) => this = new Vec3(str);
        
        public static Vec3 TransformVector(Vec3 vec, Matrix4 mat)
            => new Vec3(
                vec.Dot(new Vec3(mat.Column0, false)),
                vec.Dot(new Vec3(mat.Column1, false)),
                vec.Dot(new Vec3(mat.Column2, false)));
        
        public static Vec3 TransformPosition(Vec3 pos, Matrix4 mat)
            => new Vec3(
                pos.Dot(new Vec3(mat.Column0, false)) + mat.Row3.X,
                pos.Dot(new Vec3(mat.Column1, false)) + mat.Row3.Y,
                pos.Dot(new Vec3(mat.Column2, false)) + mat.Row3.Z);
    }
}
