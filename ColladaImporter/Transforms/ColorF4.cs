using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using static ColladaSharp.Extensions;

namespace ColladaSharp
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct ColorF4
    {
        public float R, G, B, A;
        
        public ColorF4(float intensity) { R = G = B = intensity; A = 1.0f; }
        public ColorF4(float intensity, float alpha) { R = G = B = intensity; A = alpha; }
        public ColorF4(float r, float g, float b) { R = r; G = g; B = b; A = 1.0f; }
        public ColorF4(float r, float g, float b, float a) { R = r; G = g; B = b; A = a; }
        public ColorF4(string s)
        {
            R = G = B = 0.0f;
            A = 1.0f;

            char[] delims = new char[] { ',', 'R', 'G', 'B', 'A', ':', ' ', '[', ']' };
            string[] arr = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);

            if (arr.Length >= 4)
            {
                float.TryParse(arr[0], NumberStyles.Any, CultureInfo.InvariantCulture, out R);
                float.TryParse(arr[1], NumberStyles.Any, CultureInfo.InvariantCulture, out G);
                float.TryParse(arr[2], NumberStyles.Any, CultureInfo.InvariantCulture, out B);
                float.TryParse(arr[3], NumberStyles.Any, CultureInfo.InvariantCulture, out A);
            }
        }

        public bool Equals(ColorF4 other, float precision)
            =>  Math.Abs(R - other.R) < precision &&
                Math.Abs(G - other.G) < precision &&
                Math.Abs(B - other.B) < precision &&
                Math.Abs(A - other.A) < precision;
        
        public static implicit operator ColorF4(Color p)
            => new ColorF4(p.R * ByteToFloat, p.G * ByteToFloat, p.B * ByteToFloat, p.A * ByteToFloat);
        public static explicit operator Color(ColorF4 p)
            => Color.FromArgb(p.A.ToByte(), p.R.ToByte(), p.G.ToByte(), p.B.ToByte());
        
        public override string ToString()
            => string.Format("[R:{0},G:{1},B:{2},A:{3}]", R, G, B, A);
    }
}
