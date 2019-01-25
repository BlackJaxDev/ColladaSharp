using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ColladaSharp.Models
{
    public enum EBufferType
    {
        Position        = 0,
        Normal          = 1,
        Binormal        = 2,
        Tangent         = 3,
        MatrixIds       = 4,
        MatrixWeights   = 5,
        Color           = 6,
        TexCoord        = 7,
        Other           = 8,
    }
    public interface IDataBuffer
    {
        int Index { get; set; }
        string Name { get; set; }
        EBufferType Type { get; set; }
        bool Integral { get; set; }
        bool Normalize { get; set; }

        int DataLength { get; }
        int Stride { get; }
        int Count { get; }
        Type ElementType { get; }
        
        object this[int index] { get; set; }
    }
    public class DataBuffer<T> : IDataBuffer, IEnumerable<T> where T : unmanaged
    {
        Type IDataBuffer.ElementType => typeof(T);
        public int DataLength => _elements.Length * Stride;
        public int Stride => Marshal.SizeOf<T>();
        public int Count => _elements.Length;

        public int Index { get; set; }
        public string Name { get; set; }
        public EBufferType Type { get; set; }
        public bool Integral { get; set; }
        public bool Normalize { get; set; }
        
        public DataBuffer()
            : this(0, false, false) { }
        public DataBuffer(
            int elementCount,
            bool normalize = false,
            bool integral = false)
        {
            Normalize = normalize;
            Integral = integral;
            _elements = new T[elementCount];
        }
        public DataBuffer(
            T[] elements,
            bool normalize = false,
            bool integral = false)
        {
            Normalize = normalize;
            Integral = integral;
            _elements = elements;
        }

        private T[] _elements;

        public T this[int index]
        {
            get => _elements[index];
            set => _elements[index] = value;
        }

        object IDataBuffer.this[int index]
        {
            get => _elements[index];
            set => _elements[index] = (T)value;
        }

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_elements).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)_elements).GetEnumerator();
    }
}
