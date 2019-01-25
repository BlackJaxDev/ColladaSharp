using System;

namespace ColladaSharp.Definition
{
    [Flags]
    public enum EIgnoreFlags : ulong
    {
        None        = 0b_0000_0000,
        Asset       = 0b_0000_0001,
        Extra       = 0b_0000_0010,
        Controllers = 0b_0000_0100,
        Geometry    = 0b_0000_1000,
        Animations  = 0b_0001_0000,
        Cameras     = 0b_0010_0000,
        Lights      = 0b_0100_0000,
    }
}
