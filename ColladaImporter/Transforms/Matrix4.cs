using System;
using System.Runtime.InteropServices;
using XMLSchemaDefinition;
using static System.Math;

namespace ColladaSharp.Transforms
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Matrix4 : IEquatable<Matrix4>, IParsable
    {
        public Vec4 Row0;
        public Vec4 Row1;
        public Vec4 Row2;
        public Vec4 Row3;
        
        public float* Data { get { fixed (Matrix4* p = &this) return (float*)p; } }

        public static readonly Matrix4 Identity = new Matrix4(Vec4.UnitX, Vec4.UnitY, Vec4.UnitZ, Vec4.UnitW);
        public static readonly Matrix4 Zero = new Matrix4(Vec4.Zero, Vec4.Zero, Vec4.Zero, Vec4.Zero);
        public static readonly Matrix4 ZupToYup = new Matrix4(Vec4.UnitX, -Vec4.UnitZ, Vec4.UnitY, Vec4.UnitW);
        public static readonly Matrix4 YupToZup = new Matrix4(Vec4.UnitX, Vec4.UnitZ, -Vec4.UnitY, Vec4.UnitW);
        public static readonly Matrix4 XupToYup = new Matrix4(Vec4.UnitY, -Vec4.UnitX, Vec4.UnitZ, Vec4.UnitW);
        public static readonly Matrix4 YupToXup = new Matrix4(-Vec4.UnitY, Vec4.UnitX, Vec4.UnitZ, Vec4.UnitW);

        public enum ECoordinate
        {
            PosX = 0,
            NegX = 1,
            PosY = 2,
            NegY = 3,
            PosZ = 4,
            NegZ = 5,
        }
        
        public Matrix4(Vec4 row0, Vec4 row1, Vec4 row2, Vec4 row3)
        {
            Row0 = row0;
            Row1 = row1;
            Row2 = row2;
            Row3 = row3;
        }

        public Matrix4(
            float m00, float m01, float m02, float m03,
            float m10, float m11, float m12, float m13,
            float m20, float m21, float m22, float m23,
            float m30, float m31, float m32, float m33)
        {
            Row0 = new Vec4(m00, m01, m02, m03);
            Row1 = new Vec4(m10, m11, m12, m13);
            Row2 = new Vec4(m20, m21, m22, m23);
            Row3 = new Vec4(m30, m31, m32, m33);
        }
        public float Determinant
        {
            get
            {
                float m11 = Row0.X, m12 = Row0.Y, m13 = Row0.Z, m14 = Row0.W,
                      m21 = Row1.X, m22 = Row1.Y, m23 = Row1.Z, m24 = Row1.W,
                      m31 = Row2.X, m32 = Row2.Y, m33 = Row2.Z, m34 = Row2.W,
                      m41 = Row3.X, m42 = Row3.Y, m43 = Row3.Z, m44 = Row3.W;

                return
                    m11 * m22 * m33 * m44 - m11 * m22 * m34 * m43 + m11 * m23 * m34 * m42 - m11 * m23 * m32 * m44
                  + m11 * m24 * m32 * m43 - m11 * m24 * m33 * m42 - m12 * m23 * m34 * m41 + m12 * m23 * m31 * m44
                  - m12 * m24 * m31 * m43 + m12 * m24 * m33 * m41 - m12 * m21 * m33 * m44 + m12 * m21 * m34 * m43
                  + m13 * m24 * m31 * m42 - m13 * m24 * m32 * m41 + m13 * m21 * m32 * m44 - m13 * m21 * m34 * m42
                  + m13 * m22 * m34 * m41 - m13 * m22 * m31 * m44 - m14 * m21 * m32 * m43 + m14 * m21 * m33 * m42
                  - m14 * m22 * m33 * m41 + m14 * m22 * m31 * m43 - m14 * m23 * m31 * m42 + m14 * m23 * m32 * m41;
            }
        }

        /// <summary>
        /// Gets the trace of the matrix, the sum of the values along the diagonal.
        /// </summary>
        public float Trace => Row0.X + Row1.Y + Row2.Z + Row3.W;

        public Matrix4 Inverted()
        {
            Matrix4 m = this;
            if (m.Determinant != 0.0f)
                m.Invert();
            return m;
        }
        public void Invert()
        {
            int[] colIdx = { 0, 0, 0, 0 };
            int[] rowIdx = { 0, 0, 0, 0 };
            int[] pivotIdx = { -1, -1, -1, -1 };

            // convert the matrix to an array for easy looping
            float[,] inverse = {{Row0.X, Row0.Y, Row0.Z, Row0.W},
                                {Row1.X, Row1.Y, Row1.Z, Row1.W},
                                {Row2.X, Row2.Y, Row2.Z, Row2.W},
                                {Row3.X, Row3.Y, Row3.Z, Row3.W}};
            int icol = 0;
            int irow = 0;
            for (int i = 0; i < 4; i++)
            {
                // Find the largest pivot value
                float maxPivot = 0.0f;
                for (int j = 0; j < 4; j++)
                {
                    if (pivotIdx[j] != 0)
                    {
                        for (int k = 0; k < 4; ++k)
                        {
                            if (pivotIdx[k] == -1)
                            {
                                float absVal = Abs(inverse[j, k]);
                                if (absVal > maxPivot)
                                {
                                    maxPivot = absVal;
                                    irow = j;
                                    icol = k;
                                }
                            }
                            else if (pivotIdx[k] > 0)
                            {
                                //Singular
                                return;
                            }
                        }
                    }
                }

                ++(pivotIdx[icol]);

                // Swap rows over so pivot is on diagonal
                if (irow != icol)
                {
                    for (int k = 0; k < 4; ++k)
                    {
                        float f = inverse[irow, k];
                        inverse[irow, k] = inverse[icol, k];
                        inverse[icol, k] = f;
                    }
                }

                rowIdx[i] = irow;
                colIdx[i] = icol;

                float pivot = inverse[icol, icol];
                // check for singular matrix
                if (pivot == 0.0f)
                {
                    throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
                }

                // Scale row so it has a unit diagonal
                float oneOverPivot = 1.0f / pivot;
                inverse[icol, icol] = 1.0f;
                for (int k = 0; k < 4; ++k)
                    inverse[icol, k] *= oneOverPivot;

                // Do elimination of non-diagonal elements
                for (int j = 0; j < 4; ++j)
                {
                    // check this isn't on the diagonal
                    if (icol != j)
                    {
                        float f = inverse[j, icol];
                        inverse[j, icol] = 0.0f;
                        for (int k = 0; k < 4; ++k)
                            inverse[j, k] -= inverse[icol, k] * f;
                    }
                }
            }

            for (int j = 3; j >= 0; --j)
            {
                int ir = rowIdx[j];
                int ic = colIdx[j];
                for (int k = 0; k < 4; ++k)
                {
                    float f = inverse[k, ir];
                    inverse[k, ir] = inverse[k, ic];
                    inverse[k, ic] = f;
                }
            }

            Row0.X = inverse[0, 0];
            Row0.Y = inverse[0, 1];
            Row0.Z = inverse[0, 2];
            Row0.W = inverse[0, 3];
            Row1.X = inverse[1, 0];
            Row1.Y = inverse[1, 1];
            Row1.Z = inverse[1, 2];
            Row1.W = inverse[1, 3];
            Row2.X = inverse[2, 0];
            Row2.Y = inverse[2, 1];
            Row2.Z = inverse[2, 2];
            Row2.W = inverse[2, 3];
            Row3.X = inverse[3, 0];
            Row3.Y = inverse[3, 1];
            Row3.Z = inverse[3, 2];
            Row3.W = inverse[3, 3];
        }
        public Vec4 Column0
        {
            get { return new Vec4(Row0.X, Row1.X, Row2.X, Row3.X); }
            set { Row0.X = value.X; Row1.X = value.Y; Row2.X = value.Z; Row3.X = value.W; }
        }
        public Vec4 Column1
        {
            get { return new Vec4(Row0.Y, Row1.Y, Row2.Y, Row3.Y); }
            set { Row0.Y = value.X; Row1.Y = value.Y; Row2.Y = value.Z; Row3.Y = value.W; }
        }
        public Vec4 Column2
        {
            get { return new Vec4(Row0.Z, Row1.Z, Row2.Z, Row3.Z); }
            set { Row0.Z = value.X; Row1.Z = value.Y; Row2.Z = value.Z; Row3.Z = value.W; }
        }
        public Vec4 Column3
        {
            get { return new Vec4(Row0.W, Row1.W, Row2.W, Row3.W); }
            set { Row0.W = value.X; Row1.W = value.Y; Row2.W = value.Z; Row3.W = value.W; }
        }
        public float M11 { get { return Row0.X; } set { Row0.X = value; } }
        public float M12 { get { return Row0.Y; } set { Row0.Y = value; } }
        public float M13 { get { return Row0.Z; } set { Row0.Z = value; } }
        public float M14 { get { return Row0.W; } set { Row0.W = value; } }
        public float M21 { get { return Row1.X; } set { Row1.X = value; } }
        public float M22 { get { return Row1.Y; } set { Row1.Y = value; } }
        public float M23 { get { return Row1.Z; } set { Row1.Z = value; } }
        public float M24 { get { return Row1.W; } set { Row1.W = value; } }
        public float M31 { get { return Row2.X; } set { Row2.X = value; } }
        public float M32 { get { return Row2.Y; } set { Row2.Y = value; } }
        public float M33 { get { return Row2.Z; } set { Row2.Z = value; } }
        public float M34 { get { return Row2.W; } set { Row2.W = value; } }
        public float M41 { get { return Row3.X; } set { Row3.X = value; } }
        public float M42 { get { return Row3.Y; } set { Row3.Y = value; } }
        public float M43 { get { return Row3.Z; } set { Row3.Z = value; } }
        public float M44 { get { return Row3.W; } set { Row3.W = value; } }

        /// <summary>
        /// Gets or sets the values along the main diagonal of the matrix.
        /// </summary>
        public Vec4 Diagonal
        {
            get => new Vec4(Row0.X, Row1.Y, Row2.Z, Row3.W);
            set
            {
                Row0.X = value.X;
                Row1.Y = value.Y;
                Row2.Z = value.Z;
                Row3.W = value.W;
            }
        }
        public float this[int rowIndex, int columnIndex]
        {
            get
            {
                if (rowIndex > 3 || columnIndex > 3)
                    throw new IndexOutOfRangeException("Cannot access matrix at (" + rowIndex + ", " + columnIndex + ")");
                return Data[(columnIndex << 2) + rowIndex];
            }
            set
            {
                if (rowIndex > 3 || columnIndex > 3)
                    throw new IndexOutOfRangeException("Cannot access matrix at (" + rowIndex + ", " + columnIndex + ")");
                Data[(columnIndex << 2) + rowIndex] = value;
            }
        }

        /// <summary>
        /// Build a rotation matrix from the specified axis/angle rotation.
        /// </summary>
        /// <param name="axis">The axis to rotate about.</param>
        /// <param name="angle">Angle in degrees to rotate counter-clockwise (looking in the direction of the given axis).</param>
        /// <param name="result">A matrix instance.</param>
        public static Matrix4 CreateFromAxisAngleDeg(Vec3 axis, float angle)
            => CreateFromAxisAngleRad(axis, angle * (float)PI / 180.0f);
        /// <summary>
        /// Build a rotation matrix from the specified axis/angle rotation.
        /// </summary>
        /// <param name="axis">The axis to rotate about.</param>
        /// <param name="angle">Angle in radians to rotate counter-clockwise (looking in the direction of the given axis).</param>
        /// <param name="result">A matrix instance.</param>
        public static Matrix4 CreateFromAxisAngleRad(Vec3 axis, float angle)
        {
            // normalize and create a local copy of the vector.
            axis.Normalize();
            float axisX = axis.X, axisY = axis.Y, axisZ = axis.Z;

            // calculate angles
            float cos = (float)Cos(-angle);
            float sin = (float)Sin(-angle);
            float t = 1.0f - cos;

            // do the conversion math once
            float tXX = t * axisX * axisX,
                tXY = t * axisX * axisY,
                tXZ = t * axisX * axisZ,
                tYY = t * axisY * axisY,
                tYZ = t * axisY * axisZ,
                tZZ = t * axisZ * axisZ;

            float sinX = sin * axisX,
                sinY = sin * axisY,
                sinZ = sin * axisZ;

            Matrix4 result;
            result.Row0.X = tXX + cos;
            result.Row0.Y = tXY - sinZ;
            result.Row0.Z = tXZ + sinY;
            result.Row0.W = 0;
            result.Row1.X = tXY + sinZ;
            result.Row1.Y = tYY + cos;
            result.Row1.Z = tYZ - sinX;
            result.Row1.W = 0;
            result.Row2.X = tXZ - sinY;
            result.Row2.Y = tYZ + sinX;
            result.Row2.Z = tZZ + cos;
            result.Row2.W = 0;
            result.Row3 = Vec4.UnitW;
            return result;
        }
        /// <summary>
        /// Builds a rotation matrix for a rotation around the x-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in degrees.</param>
        /// <returns>The resulting Matrix4 instance.</returns>
        public static Matrix4 CreateRotationX(float angle)
        {
            //Convert to radians
            angle *= (float)PI / 180.0f;

            float cos = (float)Cos(angle);
            float sin = (float)Sin(angle);

            Matrix4 result = Identity;
            result.Row1.Y = cos;
            result.Row1.Z = sin;
            result.Row2.Y = -sin;
            result.Row2.Z = cos;
            return result;
        }
        /// <summary>
        /// Builds a rotation matrix for a rotation around the y-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in degrees.</param>
        /// <returns>The resulting Matrix4 instance.</returns>
        public static Matrix4 CreateRotationY(float angle)
        {
            //Convert to radians
            angle *= (float)PI / 180.0f;

            float cos = (float)Cos(angle);
            float sin = (float)Sin(angle);

            Matrix4 result = Identity;
            result.Row0.X = cos;
            result.Row0.Z = -sin;
            result.Row2.X = sin;
            result.Row2.Z = cos;
            return result;
        }
        /// <summary>
        /// Builds a rotation matrix for a rotation around the z-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in degrees.</param>
        /// <returns>The resulting Matrix4 instance.</returns>
        public static Matrix4 CreateRotationZ(float angle)
        {
            //Convert to radians
            angle *= (float)PI / 180.0f;

            float cos = (float)Cos(angle);
            float sin = (float)Sin(angle);

            Matrix4 result = Identity;
            result.Row0.X = cos;
            result.Row0.Y = sin;
            result.Row1.X = -sin;
            result.Row1.Y = cos;
            return result;
        }
        public static Matrix4 CreateTranslation(float x, float y, float z)
        {
            Matrix4 result = Identity;
            result.Row3.X = x;
            result.Row3.Y = y;
            result.Row3.Z = z;
            return result;
        }
        public static Matrix4 CreateTranslation(Vec3 translation)
        {
            return CreateTranslation(translation.X, translation.Y, translation.Z);
        }
        public static Matrix4 CreateScale(float scale)
        {
            return CreateScale(new Vec3(scale));
        }
        public static Matrix4 CreateScale(float x, float y, float z)
        {
            return CreateScale(new Vec3(x, y, z));
        }
        public static Matrix4 CreateScale(Vec3 scale)
        {
            Matrix4 result = Zero;
            result.Diagonal = new Vec4(scale, 1.0f);
            return result;
        }
        public void Transpose() { this = Transposed(); }
        public Matrix4 Transposed()
        {
            return new Matrix4(Column0, Column1, Column2, Column3);
        }
        public static Matrix4 operator *(Matrix4 left, Matrix4 right)
        {
            Matrix4 nm;
            float*
                leftMtx = (float*)&left,
                rightMtx = (float*)&right,
                dPtr = (float*)&nm;

            float val;
            for (int rowIndex = 0; rowIndex < 16; rowIndex += 4)
                for (int colIndex = 0; colIndex < 4; ++colIndex)
                {
                    val = 0.0f;
                    for (int x = rowIndex, y = colIndex; y < 16; ++x, y += 4)
                        val += rightMtx[x] * leftMtx[y];
                    dPtr[rowIndex + colIndex] = val;
                }

            return nm;
        }
        public static bool operator ==(Matrix4 left, Matrix4 right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Matrix4 left, Matrix4 right)
        {
            return !left.Equals(right);
        }
        public override string ToString()
        {
            return String.Format("{0}\n{1}\n{2}\n{3}", Row0, Row1, Row2, Row3);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Row0.GetHashCode();
                hashCode = (hashCode * 397) ^ Row1.GetHashCode();
                hashCode = (hashCode * 397) ^ Row2.GetHashCode();
                hashCode = (hashCode * 397) ^ Row3.GetHashCode();
                return hashCode;
            }
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Matrix4))
                return false;

            return Equals((Matrix4)obj);
        }
        public bool Equals(Matrix4 other)
        {
            return
                Row0 == other.Row0 &&
                Row1 == other.Row1 &&
                Row2 == other.Row2 &&
                Row3 == other.Row3;
        }
        public string WriteToString()
        {
            return
                Row0.WriteToString() + " " +
                Row1.WriteToString() + " " +
                Row2.WriteToString() + " " +
                Row3.WriteToString();
        }
        public void ReadFromString(string str)
        {
            string[] v = str.Split(' ');
            for (int i = 0; i < 16; ++i)
                Data[i] = i < v.Length ? float.Parse(v[i]) : 0.0f;
        }


        /// <summary>
        /// Returns the rotation component of this instance. Quite slow.
        /// </summary>
        /// <param name="rowNormalize">Whether the method should row-normalise (i.e. remove scale from) the Matrix. Pass false if you know it's already normalised.</param>
        public Quat ExtractRotation(bool rowNormalize = true)
        {
            var row0 = Row0.Xyz;
            var row1 = Row1.Xyz;
            var row2 = Row2.Xyz;

            if (rowNormalize)
            {
                row0 = row0.Normalized();
                row1 = row1.Normalized();
                row2 = row2.Normalized();
            }

            // code below adapted from Blender

            Quat q = new Quat();
            double trace = 0.25 * (row0[0] + row1[1] + row2[2] + 1.0);

            if (trace > 0)
            {
                double sq = Sqrt(trace);

                q.W = (float)sq;
                sq = 1.0 / (4.0 * sq);
                q.X = (float)((row1[2] - row2[1]) * sq);
                q.Y = (float)((row2[0] - row0[2]) * sq);
                q.Z = (float)((row0[1] - row1[0]) * sq);
            }
            else if (row0[0] > row1[1] && row0[0] > row2[2])
            {
                double sq = 2.0 * Sqrt(1.0 + row0[0] - row1[1] - row2[2]);

                q.X = (float)(0.25 * sq);
                sq = 1.0 / sq;
                q.W = (float)((row2[1] - row1[2]) * sq);
                q.Y = (float)((row1[0] + row0[1]) * sq);
                q.Z = (float)((row2[0] + row0[2]) * sq);
            }
            else if (row1[1] > row2[2])
            {
                double sq = 2.0 * Sqrt(1.0 + row1[1] - row0[0] - row2[2]);

                q.Y = (float)(0.25 * sq);
                sq = 1.0 / sq;
                q.W = (float)((row2[0] - row0[2]) * sq);
                q.X = (float)((row1[0] + row0[1]) * sq);
                q.Z = (float)((row2[1] + row1[2]) * sq);
            }
            else
            {
                double sq = 2.0 * Sqrt(1.0 + row2[2] - row0[0] - row1[1]);

                q.Z = (float)(0.25 * sq);
                sq = 1.0 / sq;
                q.W = (float)((row1[0] - row0[1]) * sq);
                q.X = (float)((row2[0] + row0[2]) * sq);
                q.Y = (float)((row2[1] + row1[2]) * sq);
            }

            q.Normalize();
            return q;
        }
    }
}
