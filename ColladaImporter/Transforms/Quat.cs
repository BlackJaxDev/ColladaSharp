using System;
using System.Runtime.InteropServices;
using static ColladaSharp.Extensions;
using static System.Math;

namespace ColladaSharp.Transforms
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Quat : IEquatable<Quat>
    {
        public static readonly Quat Identity = new Quat(0.0f, 0.0f, 0.0f, 1.0f);

        public float X, Y, Z, W;

        public Quat(Vec3 v, float w)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = w;
        }
        public Quat(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public float* Data { get { fixed (void* p = &this) return (float*)p; } }
        public Vec3 Xyz
        {
            get => new Vec3(X, Y, Z);
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }
        public Vec4 Xyzw
        {
            get => new Vec4(X, Y, Z, W);
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
                W = value.W;
            }
        }

        public float Length => (float)Sqrt(LengthSquared);
        public float LengthSquared => Xyzw.LengthSquared;

        public float this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }
        public void ToAxisAngleDeg(out Vec3 axis, out float degrees)
        {
            ToAxisAngleRad(out axis, out float radians);
            degrees = RadToDeg(radians);
        }
        public void ToAxisAngleRad(out Vec3 axis, out float radians)
        {
            Vec4 result = ToAxisAngleRad();
            axis = result.Xyz;
            radians = result.W;
        }
        private Vec4 ToAxisAngleRad()
        {
            Quat q = this;
            q.Normalize();
            float den = (float)Sqrt(1.0f - q.W * q.W);
            return new Vec4(den > Epsilon ? q.Xyz / den : Vec3.UnitX, 2.0f * (float)Acos(q.W));
        }

        /// <summary>
        /// Returns a euler rotation in the order of yaw, pitch, roll.
        /// </summary>
        public void ToYawPitchRoll(out float yaw, out float pitch, out float roll)
        {
            Normalize();
            float sqx = X * X;
            float sqy = Y * Y;
            float sqz = Z * Z;
            float sqw = W * W;
            float unit = sqx + sqy + sqz + sqw;
            float test = (X * Y + Z * W) / unit;
            if (test.EqualTo(0.5f, 0.001f))
            {
                //North pole singularity
                yaw = 2.0f * (float)Atan2(X, W);
                pitch = (float)PI / 2.0f;
                roll = 0.0f;
            }
            else if (test.EqualTo(-0.5f, 0.001f))
            {
                //South pole singularity
                yaw = -2.0f * (float)Atan2(X, W);
                pitch = (float)-PI / 2.0f;
                roll = 0.0f;
            }
            else
            {
                yaw = (float)Atan2(2.0f * Y * W - 2.0f * X * Z, 1.0f - 2.0f * sqy - 2.0f * sqz);
                roll = (float)Asin(2.0f * X * Y + 2.0f * Z * W);
                pitch = (float)Atan2(2.0f * X * W - 2.0f * Y * Z, 1.0f - 2.0f * sqx - 2.0f * sqz);

                //yaw = (float)Atan2(2.0f * Y * W - 2.0f * X * Z, sqx - sqy - sqz + sqw);
                //pitch = (float)Asin(2.0f * test / unit);
                //roll = (float)Atan2(2.0f * X * W - 2.0f * Y * Z, -sqx + sqy - sqz + sqw);
            }
            RadToDeg(pitch);
            RadToDeg(yaw);
            RadToDeg(roll);
        }
        public void Normalize(bool safe = true)
        {
            float lengthSq = LengthSquared;
            if (safe && lengthSq.IsZero()) return;
            Xyzw *= 1.0f / (float)Sqrt(lengthSq);
        }
        public Quat Normalized(bool safe = true)
        {
            Quat q = this;
            q.Normalize(safe);
            return q;
        }
        public void Invert() => W = -W;
        public Quat Inverted()
        {
            var q = this;
            q.Invert();
            return q;
        }
        public static Quat Invert(Quat q, bool safe = true)
        {
            float lengthSq = q.LengthSquared;
            if (!safe || !lengthSq.IsZero())
                return new Quat(q.Xyz / -lengthSq, q.W / lengthSq);
            else
                return q;
        }
        /// <summary>
        /// Makes this quaternion the opposite version of itself.
        /// There are two rotations about the same axis that equal the same rotation,
        /// but from different directions.
        /// </summary>
        public void Conjugate() => Xyz = -Xyz;
        /// <summary>
        /// Returns the opposite version of this quaternion.
        /// There are two rotations about the same axis that equal the same rotation,
        /// but from different directions.
        /// </summary>
        public Quat Conjugated()
        {
            var q = this;
            q.Conjugate();
            return q;
        }
        /// <summary>
        /// Returns a quat that rotates from the first quat to the second quat: from * returned = to
        /// </summary>
        public static Quat DeltaRotation(Quat from, Quat to)
            => from.Inverted() * to;
        /// <summary>
        /// Returns a quaternion containing the rotation from one vector direction to another.
        /// </summary>
        /// <param name="initialVector">The starting vector</param>
        /// <param name="finalVector">The ending vector</param>
        public static Quat BetweenVectors(Vec3 initialVector, Vec3 finalVector)
        {
            Vec3.AxisAngleBetween(initialVector, finalVector, out Vec3 axis, out float angle);
            return FromAxisAngleDeg(axis, angle);
        }
        public static Quat LookAt(Vec3 sourcePoint, Vec3 destPoint, Vec3 initialDirection)
            => BetweenVectors(initialDirection, destPoint - sourcePoint);
        public static Quat FromAxisAngleDeg(Vec3 axis, float degrees)
            => FromAxisAngleRad(axis, DegToRad(degrees));
        public static Quat FromAxisAngleRad(Vec3 axis, float radians)
        {
            if (axis.LengthSquared == 0.0f)
                return Identity;

            Quat result = Identity;

            radians *= 0.5f;
            axis.Normalize();
            result.Xyz = axis * (float)Sin(radians);
            result.W = (float)Cos(radians);

            return result.Normalized();
        }
        
        public static Quat FromMatrix(Matrix4 matrix)
        {
            Quat result = new Quat();
            float trace = matrix.Trace;

            if (trace > 0)
            {
                float s = (float)Sqrt(trace + 1.0f) * 2.0f;
                float invS = 1.0f / s;

                result.W = s * 0.25f;
                result.X = (matrix.Row2.Y - matrix.Row1.Z) * invS;
                result.Y = (matrix.Row0.Z - matrix.Row2.X) * invS;
                result.Z = (matrix.Row1.X - matrix.Row0.Y) * invS;
            }
            else
            {
                float m00 = matrix.Row0.X, m11 = matrix.Row1.Y, m22 = matrix.Row2.Z;

                if (m00 > m11 && m00 > m22)
                {
                    float s = (float)Sqrt(1.0f + m00 - m11 - m22) * 2.0f;
                    float invS = 1.0f / s;

                    result.W = (matrix.Row2.Y - matrix.Row1.Z) * invS;
                    result.X = s * 0.25f;
                    result.Y = (matrix.Row0.Y + matrix.Row1.X) * invS;
                    result.Z = (matrix.Row0.Z + matrix.Row2.X) * invS;
                }
                else if (m11 > m22)
                {
                    float s = (float)Sqrt(1.0f + m11 - m00 - m22) * 2.0f;
                    float invS = 1.0f / s;

                    result.W = (matrix.Row0.Z - matrix.Row2.X) * invS;
                    result.X = (matrix.Row0.Y + matrix.Row1.X) * invS;
                    result.Y = s * 0.25f;
                    result.Z = (matrix.Row1.Z + matrix.Row2.Y) * invS;
                }
                else
                {
                    float s = (float)Sqrt(1.0f + m22 - m00 - m11) * 2.0f;
                    float invS = 1.0f / s;

                    result.W = (matrix.Row1.X - matrix.Row0.Y) * invS;
                    result.X = (matrix.Row0.Z + matrix.Row2.X) * invS;
                    result.Y = (matrix.Row1.Z + matrix.Row2.Y) * invS;
                    result.Z = s * 0.25f;
                }
            }
            return result;
        }
        //Optimized form for vec3's (W == 0)
        public Vec3 Transform(Vec3 v)
        {
            Vec3 t = 2.0f * (Xyz ^ v);
            return v + W * t + (Xyz ^ t);
        }
        //Slower, traditional form
        public Vec4 Transform(Vec4 vec)
        {
            Quat v = new Quat(vec.X, vec.Y, vec.Z, vec.W);
            v = this * v * Conjugated();
            return new Vec4(v.X, v.Y, v.Z, v.W);
        }
        public static Vec3 operator *(Quat quat, Vec3 vec)
            => quat.Transform(vec);
        public static Vec3 operator *(Vec3 vec, Quat quat)
            => quat.Transform(vec);
        public static Vec4 operator *(Quat quat, Vec4 vec)
            => quat.Transform(vec);
        public static Vec4 operator *(Vec4 vec, Quat quat)
            => quat.Transform(vec);
        public static Quat operator +(Quat left, Quat right)
        {
            left.Xyzw += right.Xyzw;
            return left;
        }
        public static Quat operator -(Quat left, Quat right)
        {
            left.Xyzw -= right.Xyzw;
            return left;
        }
        public static Quat operator *(Quat left, Quat right)
            => new Quat(
                right.W * left.Xyz + left.W * right.Xyz + (left.Xyz ^ right.Xyz),
                left.W * right.W - (left.Xyz | right.Xyz));
        
        public static Quat operator *(Quat quaternion, float scale)
            => new Quat(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
        public static Quat operator *(float scale, Quat quaternion)
            => new Quat(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
        
        public static bool operator ==(Quat left, Quat right)
            => left.Equals(right);
        public static bool operator !=(Quat left, Quat right)
            => !left.Equals(right);

        private static readonly string ListSeparator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        public override string ToString()
            => String.Format("({0}{4} {1}{4} {2}{4} {3})", X, Y, Z, W, ListSeparator);
        public string ToString(bool includeParentheses, bool includeSeparator)
           => String.Format("{5}{0}{4} {1}{4} {2}{4} {3}{6}", X, Y, Z, W,
               includeSeparator ? ListSeparator : "", includeParentheses ? "(" : "", includeParentheses ? ")" : "");

        public static Quat Parse(string value)
        {
            value = value.Trim();

            if (value.StartsWith("("))
                value = value.Substring(1);
            if (value.EndsWith(")"))
                value = value.Substring(0, value.Length - 1);

            string[] parts = value.Split(' ');
            if (parts[0].EndsWith(ListSeparator))
                parts[0].Substring(0, parts[0].Length - 1);
            if (parts[1].EndsWith(ListSeparator))
                parts[1].Substring(0, parts[1].Length - 1);
            if (parts[2].EndsWith(ListSeparator))
                parts[2].Substring(0, parts[2].Length - 1);

            return new Quat(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
        }
        public override bool Equals(object other)
        {
            if (other is Quat == false)
                return false;
            return this == (Quat)other;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (Xyz.GetHashCode() * 397) ^ W.GetHashCode();
            }
        }
        public bool Equals(Quat other)
        {
            return Xyz == other.Xyz && W == other.W;
        }

        public string WriteToString()
            => ToString(false, false);
        public void ReadFromString(string str)
            => this = Parse(str);
    }
}
