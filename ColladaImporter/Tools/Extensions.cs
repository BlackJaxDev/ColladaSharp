using System;
using System.Collections.Generic;

namespace ColladaSharp
{
    public static class Extensions
    {
        public const float Epsilon = 0.000001f;
        public const float ByteToFloat = 1.0f / 255.0f;
        public const float PIf = 3.1415926535897931f;

        private static readonly float DegToRadMultf = PIf / 180.0f;
        private static readonly float RadToDegMultf = 180.0f / PIf;

        public static float DegToRad(float degrees) => degrees * DegToRadMultf;
        public static float RadToDeg(float radians) => radians * RadToDegMultf;

        public static bool IsZero(this float value, float errorMargin = Epsilon)
            => Math.Abs(value) < errorMargin;
        public static T AsEnum<T>(this string str) where T : Enum
            => (T)Enum.Parse(typeof(T), str);
        public static bool IndexInRange<T>(this IList<T> list, int value)
            => value >= 0 && value < list.Count;
        public static int Clamp(this int value, int min, int max)
            => value <= min ? min : value >= max ? max : value;
        public static float Clamp(this float value, float min, float max)
            => value <= min ? min : value >= max ? max : value;
        public static bool EqualTo(this float value, float other, float tolerance = 0.0001f)
            => Math.Abs(value - other) < tolerance;
        public static byte ToByte(this float value)
        {
            //Casting a decimal to an integer floors the value
            //So multiply by 256 to get the proper value.
            //1.0f is the edge case, so clamp to ensure no rounding issues 
            //and handle edge case appropriately
            float f2 = value.Clamp(0.0f, 1.0f);
            return (byte)Math.Floor(f2 == 1.0f ? 255.0f : f2 * 256.0f);
        }
    }
}
