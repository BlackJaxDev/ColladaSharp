using System;
using System.Globalization;
using System.Runtime.InteropServices;
using XMLSchemaDefinition;

namespace ColladaSharp.Transforms
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Vec2 : IEquatable<Vec2>, IParsable
    {
        public float X, Y;

        public float* Data { get { fixed (Vec2* p = &this) return (float*)p; } }

        public Vec2(float value)
        {
            X = value;
            Y = value;
        }
        public Vec2(float x, float y)
        {
            X = x;
            Y = y;
        }
        public Vec2(Vec3 v, bool divideByZ)
        {
            if (divideByZ)
            {
                X = v.X / v.Z;
                Y = v.Y / v.Z;
            }
            else
            {
                X = v.X;
                Y = v.Y;
            }
        }
        public Vec2(string s, params char[] delimiters)
        {
            X = Y = 0.0f;

            char[] delims = delimiters != null && delimiters.Length > 0 ? delimiters : new char[] { ',', '(', ')', ' ' };
            string[] arr = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);

            if (arr.Length >= 2)
            {
                float.TryParse(arr[0], NumberStyles.Any, CultureInfo.InvariantCulture, out X);
                float.TryParse(arr[1], NumberStyles.Any, CultureInfo.InvariantCulture, out Y);
            }
        }

        public float this[int index]
        {
            get
            {
                if (index < 0 || index > 1)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                return Data[index];
            }
            set
            {
                if (index < 0 || index > 1)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                Data[index] = value;
            }
        }

        public static readonly Vec2 UnitX = new Vec2(1.0f, 0.0f);
        public static readonly Vec2 UnitY = new Vec2(0.0f, 1.0f);
        public static readonly Vec2 Zero = new Vec2(0.0f);
        public static readonly Vec2 One = new Vec2(1.0f);

        public static Vec2 operator -(Vec2 v)
            => new Vec2(-v.X, -v.Y);
        public static Vec2 operator -(Vec2 left, Vec2 right)
            => new Vec2(left.X - right.X, left.Y - right.Y);

        public static bool operator ==(Vec2 left, Vec2 right) => left.Equals(right);
        public static bool operator !=(Vec2 left, Vec2 right) => !left.Equals(right);
        
        public override string ToString() => ToString();
        public string ToString(string openingBracket = "(", string closingBracket = ")", string separator = ", ")
            => string.Format("{3}{0}{2}{1}{4}", X, Y, separator, openingBracket, closingBracket);

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Vec2))
                return false;
            return Equals((Vec2)obj);
        }
        public bool Equals(Vec2 other)
            => X == other.X &&
               Y == other.Y;
        public bool Equals(Vec2 other, float precision)
            => Math.Abs(X - other.X) < precision &&
               Math.Abs(Y - other.Y) < precision;

        public string WriteToString()
            => ToString("", "", " ");
        public void ReadFromString(string str)
            => this = new Vec2(str);
    }
}
