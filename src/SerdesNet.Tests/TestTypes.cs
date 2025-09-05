using System;

namespace SerdesNet.Tests;

public enum ByteEnum : byte
{
    None,
    Some,
    Both,
    Many,
    AlmostAll = 254,
    All = 255
}

public enum UShortEnum : ushort
{
    None,
    Some,
    Both,
    Many,
    AlmostAll = 0xfffe,
    All = 0xffff
}

public enum UIntEnum : uint
{
    None = 0,
    Some,
    Both,
    Many,
    AlmostAll = 0xffff_fffe,
    All = 0xffff_ffff
}

public class Example
{
    public static readonly byte[] ExampleBuffer =
    [
        255, 1, 254, 255, 2, 0,
        253,255,255,255,
        3,0,0,0,
        252,255,255,255,255,255,255,255,
        4,0,0,0,0,0,0,0,
        1,
        2,0,
        3,0,0,0,
        0x4, 0, 0, 0,
        0x55, 0x74, 0x66, 0x38,

        0x0d, 0xa5, 0x6f, 0xfa,
        0x6f, 0xbe,
        0x36, 0x47,
        0x87, 0xdf,
        0xe1, 0x72, 0x80, 0xf1, 0x42, 0x48
    ];

    public void Verify(Action<string> failed)
    {
        if (SByte      != -1) failed($"SByte was expected to be -1, but was {SByte}");
        if (Byte       !=  1) failed($"Byte was expected to be 1, but was {Byte}");
        if (Short      != -2) failed($"Short was expected to be -2, but was {Short}");
        if (UShort     !=  2) failed($"UShort was expected to be 2, but was {UShort}");
        if (Int        != -3) failed($"Int was expected to be -3, but was {Int}");
        if (Uint       !=  3) failed($"Uint was expected to be 3, but was {Uint}");
        if (Long       != -4) failed($"Long was expected to be -4, but was {Long}");
        if (ULong      !=  4) failed($"ULong was expected to be 4, but was {ULong}");
        if (ByteEnum   != ByteEnum.Some) failed($"ByteEnum was expected to be Some, but was {ByteEnum}");
        if (UShortEnum != UShortEnum.Both) failed($"UShortEnum was expected to be Both, but was {UShortEnum}");
        if (UIntEnum   != UIntEnum.Many) failed($"UIntEnum was expected to be Many, but was {UIntEnum}");
        if (Utf8       != "Utf8") failed($"Utf8 was expected to be \"Utf8\", but was \"{Utf8}\"");
        var guid = Guid.Parse("{FA6FA50D-BE6F-4736-87DF-E17280F14248}");
        if(Guid       != guid) failed($"Guid was expected to be {guid}, but was {Guid}");
    }

    public static readonly Example TestInstance = new()
    {
        SByte = -1,
        Byte = 1,
        Short = -2,
        UShort = 2,
        Int = -3,
        Uint = 3,
        Long = -4,
        ULong = 4,
        ByteEnum = ByteEnum.Some,
        UShortEnum = UShortEnum.Both,
        UIntEnum = UIntEnum.Many,
        Utf8 = "Utf8",
        Guid = Guid.Parse("{FA6FA50D-BE6F-4736-87DF-E17280F14248}"),
    };

    const int FixedLength = 5;
    public sbyte      SByte      { get; private set; }
    public byte       Byte       { get; private set; }
    public short      Short      { get; private set; }
    public ushort     UShort     { get; private set; }
    public int        Int        { get; private set; }
    public uint       Uint       { get; private set; }
    public long       Long       { get; private set; }
    public ulong      ULong      { get; private set; }
    public ByteEnum   ByteEnum   { get; private set; }
    public UShortEnum UShortEnum { get; private set; }
    public UIntEnum   UIntEnum   { get; private set; }
    public string     Utf8       { get; private set; }
    public Guid       Guid       { get; private set; }

    public static Example Serdes(SerdesName _, Example e, ISerdes s)
    {
        e ??= new Example();
        e.SByte      =    s.Int8(nameof(SByte), e.SByte);
        e.Byte       =   s.UInt8(nameof(Byte), e.Byte);
        e.Short      =   s.Int16(nameof(Short), e.Short);
        e.UShort     =  s.UInt16(nameof(UShort), e.UShort);
        e.Int        =   s.Int32(nameof(Int), e.Int);
        e.Uint       =  s.UInt32(nameof(Uint), e.Uint);
        e.Long       =   s.Int64(nameof(Long), e.Long);
        e.ULong      =  s.UInt64(nameof(ULong), e.ULong);
        e.ByteEnum   =  s.EnumU8(nameof(ByteEnum), e.ByteEnum);
        e.UShortEnum = s.EnumU16(nameof(UShortEnum), e.UShortEnum);
        e.UIntEnum   = s.EnumU32(nameof(UIntEnum), e.UIntEnum);
        e.Utf8       = s.String32Utf8(nameof(Utf8), e.Utf8);
        e.Guid       = s.Guid(nameof(Guid), e.Guid);
        return e;
    }
}
