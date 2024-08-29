using System;

namespace FTN.Common
{
    public enum PhaseCode : short
    {
        Unknown = 0x0,
        s1 = 0x1,//
        s12 = 0x2,//
        s12N = 0x3,//
        s1N = 0x4,//
        s2 = 0x5,//
        s2N = 0x6,//
        BC = 0x7,//
        BN = 0x8,//
        B = 0x9,//
        AB = 0xA,//
        ABN = 0xB,//
        ABC = 0xC,//
        ABCN = 0xD,//
        AC = 0xE,//
        ACN = 0xF,//
        AN = 0x10,//
        A = 0x11,//
        C = 0x12,//
        CN = 0x13,//
        N = 0x14,//
        BCN=0x15
    }

}
