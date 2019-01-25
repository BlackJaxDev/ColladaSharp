using System;
using System.Linq;
using XMLSchemaDefinition;

namespace ColladaSharp.Definition
{
    public partial class Collada
    {
        public class ElementColladaURI : BaseElementString
        {
            public ColladaURI Value { get; set; }
            public override void ReadFromString(string str)
            {
                Value = new ColladaURI();
                Value.ReadFromString(str);
            }
            public override string WriteToString()
                => Value.ToString();
        }
        public class ElementBoolArray : BaseElementString
        {
            public bool[] Values { get; set; }
            public override void ReadFromString(string str)
                => Values = str.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Select(x => bool.Parse(x)).ToArray();
            public override string WriteToString()
                => string.Join(" ", Values);
        }
        public class ElementStringArray : BaseElementString
        {
            public string[] Values { get; set; }
            public override void ReadFromString(string str)
                => Values = str.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            public override string WriteToString()
                => string.Join(" ", Values);
        }
        public class ElementIntArray : BaseElementString
        {
            public int[] Values { get; set; }
            public override void ReadFromString(string str)
                => Values = str.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();
            public override string WriteToString()
                => string.Join(" ", Values);
        }
        public class ElementFloatArray : BaseElementString
        {
            public float[] Values { get; set; }
            public override void ReadFromString(string str)
                => Values = str.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Select(x => float.Parse(x)).ToArray();
            public override string WriteToString()
                => string.Join(" ", Values);
        }
    }
}
