using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace SerdesNet.Tests
{
    public class AnnotationTests
    {
        static string Write(Action<ISerdes> action, Action<string> assertHandler = null)
        {
            using var byteStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(byteStream);
            using var binarySerializer = new WriterSerdes(
                binaryWriter,
                Encoding.UTF8.GetBytes,
                assertHandler ?? (m => throw new InvalidOperationException(m)));

            using var textStream = new MemoryStream();
            using var textWriter = new StreamWriter(textStream);
            using var annotatingSerializer = new AnnotationProxySerdes(binarySerializer, textWriter, Encoding.ASCII.GetBytes);

            action(annotatingSerializer);

            textWriter.Flush();
            textStream.Position = 0;
            using var sr = new StreamReader(textStream, null, true, -1, true);
            return sr.ReadToEnd().Trim();
        }

        [Fact]
        public void Int8Test()
        {
            Assert.Equal("0  = 0 (0x0 y)", Write(s => s.Int8("", 0)));
            Assert.Equal("0  = 1 (0x1 y)", Write(s => s.Int8("", 1)));
            Assert.Equal("0  = -1 (0xFF y)", Write(s => s.Int8("", -1)));
            Assert.Equal("0 name = 0 (0x0 y)", Write(s => s.Int8("name", 0)));
            Assert.Equal("0 name = 1 (0x1 y)", Write(s => s.Int8("name", 1)));
            Assert.Equal("0 name = -1 (0xFF y)", Write(s => s.Int8("name", -1)));
        }

        [Fact]
        public void UInt8Test()
        {
            Assert.Equal("0 name = 0 (0x0 uy)", Write(s => s.UInt8("name", 0)));
            Assert.Equal("0 name = 1 (0x1 uy)", Write(s => s.UInt8("name", 1)));
            Assert.Equal("0 name = 127 (0x7F uy)", Write(s => s.UInt8("name", 127)));
            Assert.Equal("0 name = 128 (0x80 uy)", Write(s => s.UInt8("name", 128)));
            Assert.Equal("0 name = 255 (0xFF uy)", Write(s => s.UInt8("name", 255)));
        }

        [Fact]
        public void Int16Test()
        {
            Assert.Equal("0 name = 0 (0x0 s)", Write(s => s.Int16("name", 0)));
            Assert.Equal("0 name = 1 (0x1 s)", Write(s => s.Int16("name", 1)));
            Assert.Equal("0 name = 32767 (0x7FFF s)", Write(s => s.Int16("name", 32767)));
            Assert.Equal("0 name = -32768 (0x8000 s)", Write(s => s.Int16("name", -32768)));
            Assert.Equal("0 name = -1 (0xFFFF s)", Write(s => s.Int16("name", -1)));
        }

        [Fact]
        public void UInt16Test()
        {
            Assert.Equal("0 name = 0 (0x0 us)", Write(s => s.UInt16("name", 0)));
            Assert.Equal("0 name = 1 (0x1 us)", Write(s => s.UInt16("name", 1)));
            Assert.Equal("0 name = 32767 (0x7FFF us)", Write(s => s.UInt16("name", 32767)));
            Assert.Equal("0 name = 32768 (0x8000 us)", Write(s => s.UInt16("name", 32768)));
            Assert.Equal("0 name = 65535 (0xFFFF us)", Write(s => s.UInt16("name", 65535)));
        }

        [Fact]
        public void Int32Test()
        {
            Assert.Equal("0 name = 0 (0x0)", Write(s => s.Int32("name", 0)));
            Assert.Equal("0 name = 1 (0x1)", Write(s => s.Int32("name", 1)));
            Assert.Equal("0 name = 2147483647 (0x7FFFFFFF)", Write(s => s.Int32("name", 0x7fffffff)));
            Assert.Equal("0 name = -2147483648 (0x80000000)", Write(s => s.Int32("name", -0x80000000)));
            Assert.Equal("0 name = -1 (0xFFFFFFFF)", Write(s => s.Int32("name", -1)));
        }

        [Fact]
        public void UInt32Test()
        {
            Assert.Equal("0 name = 0 (0x0 u)", Write(s => s.UInt32("name", 0u)));
            Assert.Equal("0 name = 1 (0x1 u)", Write(s => s.UInt32("name", 1u)));
            Assert.Equal("0 name = 2147483647 (0x7FFFFFFF u)", Write(s => s.UInt32("name", 0x7fffffffu)));
            Assert.Equal("0 name = 2147483648 (0x80000000 u)", Write(s => s.UInt32("name", 0x80000000)));
            Assert.Equal("0 name = 4294967295 (0xFFFFFFFF u)", Write(s => s.UInt32("name", uint.MaxValue)));
        }

        [Fact]
        public void Int64Test()
        {
            Assert.Equal("0 name = 0 (0x0 L)", Write(s => s.Int64("name", 0)));
            Assert.Equal("0 name = 1 (0x1 L)", Write(s => s.Int64("name", 1)));
            Assert.Equal("0 name = 9223372036854775807 (0x7FFFFFFFFFFFFFFF L)", Write(s => s.Int64("name", 0x7fffffff_ffffffff)));
            Assert.Equal("0 name = -9223372036854775808 (0x8000000000000000 L)", Write(s => s.Int64("name", -0x80000000_00000000)));
            Assert.Equal("0 name = -1 (0xFFFFFFFFFFFFFFFF L)", Write(s => s.Int64("name", -1)));
        }

        [Fact]
        public void UInt64Test()
        {
            Assert.Equal("0 name = 0 (0x0 UL)", Write(s => s.UInt64("name", 0uL)));
            Assert.Equal("0 name = 1 (0x1 UL)", Write(s => s.UInt64("name", 1uL)));
            Assert.Equal("0 name = 9223372036854775807 (0x7FFFFFFFFFFFFFFF UL)", Write(s => s.UInt64("name", 0x7fffffff_ffffffffuL)));
            Assert.Equal("0 name = 9223372036854775808 (0x8000000000000000 UL)", Write(s => s.UInt64("name", 0x80000000_00000000uL)));
            Assert.Equal("0 name = 18446744073709551615 (0xFFFFFFFFFFFFFFFF UL)", Write(s => s.UInt64("name", ulong.MaxValue)));
        }

        [Fact]
        public void FixedLengthStringTest()
        {
            Assert.Equal("0 name = \"A\"", Write(s => s.FixedLengthString("name", "A", 1)));
            Assert.Throws<InvalidOperationException>(() =>
                Write(s => s.FixedLengthString("name", "Too long", 1)));
        }

        [Fact]
        public void NullTerminatedStringTest()
        {
            Assert.Equal("0 name = \"A\"", Write(s => s.NullTerminatedString("name", "A")));
        }

        [Fact]
        public void GuidTest()
        {
            Assert.Equal("0 name = {fa6fa50d-be6f-4736-87df-e17280f14248}",
                Write(s => s.Guid("name", Guid.Parse("{FA6FA50D-BE6F-4736-87DF-E17280F14248}"))));
        }

        [Fact]
        public void ListTest()
        {
            static byte UInt8Serdes(int i, byte v, ISerdes s) => s.UInt8(i.ToString(), v);
            Assert.Equal(@"0 name: {
    0 0 = 1 (0x1 uy)
    1 1 = 2 (0x2 uy)
    2 2 = 3 (0x3 uy)
}",
                Write(s => s.List(
                    "name",
                    new byte[] { 1, 2, 3 },
                    3,
                    UInt8Serdes,
                    _ => throw new InvalidOperationException())));

            Assert.Equal(@"0 name: {
    0 1 = 1 (0x1 uy)
    1 2 = 2 (0x2 uy)
    2 3 = 3 (0x3 uy)
}",
                Write(s => s.List(
                    "name",
                    new byte[] { 0, 1, 2, 3 },
                    3,
                    1,
                    UInt8Serdes,
                    _ => throw new InvalidOperationException())));
        }

        [Fact]
        public void RepeatTest()
        {
            Assert.Equal("0 name = [4 bytes (0x4) of 0x0]", Write(s => s.Pad("name", 4)));
            Assert.Equal("0 name = [4 bytes (0x4) of 0x1]", Write(s => s.Pad("name", 4, 1)));
            Assert.Equal("0 name = [1 bytes (0x1) of 0x1]", Write(s => s.Pad("name", 1, 1)));
        }

        [Fact]
        public void ByteArrayTest()
        {
            Assert.Equal("0 name = 00010203", Write(s => s.Bytes("name", new byte[] { 0, 1, 2, 3 }, 4)));
            Assert.Equal(
                @"0 name =  
    0000: 0001 0203 0405 0607-0809 0A0B 0C0D 0E0F ................
    0010: 1011 1213 1415 1617-1819 1A1B 1C1D 1E1F ................
    0020: 2021 2223 2425 2627-2829 2A2B 2C2D 2E2F  !""#$%&'()*+,-./
    0030: 3031 3233 3435 3637-3839 3A3B 3C3D 3E3F 0123456789:;<=>?
    0040: 4041 4243 4445 4647-4849 4A4B 4C4D 4E4F @ABCDEFGHIJKLMNO
    0050: 5051 5253 5455 5657-5859 5A5B 5C5D 5E5F PQRSTUVWXYZ[\]^_
    0060: 6061 6263 6465 6667-6869 6A6B 6C6D 6E6F `abcdefghijklmno
    0070: 7071 7273 7475 7677-7879 7A7B 7C7D 7E7F pqrstuvwxyz{|}~.
    0080: 8081 8283 8485 8687-8889 8A8B 8C8D 8E8F ................
    0090: 9091 9293 9495 9697-9899 9A9B 9C9D 9E9F ................
    00A0: A0A1 A2A3 A4A5 A6A7-A8A9 AAAB ACAD AEAF ................
    00B0: B0B1 B2B3 B4B5 B6B7-B8B9 BABB BCBD BEBF ................
    00C0: C0C1 C2C3 C4C5 C6C7-C8C9 CACB CCCD CECF ................
    00D0: D0D1 D2D3 D4D5 D6D7-D8D9 DADB DCDD DEDF ................
    00E0: E0E1 E2E3 E4E5 E6E7-E8E9 EAEB ECED EEEF ................
    00F0: F0F1 F2F3 F4F5 F6F7-F8F9 FAFB FCFD FEFF ................",
                Write(s => s.Bytes("name", Enumerable.Range(0, 256).Select(x => (byte)x).ToArray(), 256)));
        }

        [Fact]
        public void OffsetTests()
        {
            Write(x => Assert.Equal(SerializerFlags.Write | SerializerFlags.Comments, x.Flags));
            Write(x => Assert.Equal(int.MaxValue, x.BytesRemaining));
            Write(x => Assert.NotEqual(0, x.BytesRemaining));

            static void Block1(ISerdes s)
            {
                Assert.Equal(0, s.Offset);
                s.Comment("x"); Assert.Equal(0, s.Offset);
                s.NewLine(); Assert.Equal(0, s.Offset);

                s.UInt8("name", 0); Assert.Equal(1, s.Offset);
                s.UInt16("name", 0x201); Assert.Equal(3, s.Offset);
                s.UInt32("name", 0x6050403u); Assert.Equal(7, s.Offset);
            }

            static void Block2(ISerdes s)
            {
                s.Seek(0);
                Assert.Equal(0, s.Offset);
                s.UInt64("name", 0x807060504030201uL);
                Assert.Equal(8, s.Offset);
            }

            Assert.Equal(@"// x

0 name = 0 (0x0 uy)
1 name = 513 (0x201 us)
3 name = 100992003 (0x6050403 u)", Write(Block1));
            Assert.Equal(@"0 Seek to 0 for overwrite
0 name = 578437695752307201 (0x807060504030201 UL)", Write(Block2));
            Assert.Equal(@"// x

0 name = 0 (0x0 uy)
1 name = 513 (0x201 us)
3 name = 100992003 (0x6050403 u)
7 Seek to 0 for overwrite
0 name = 578437695752307201 (0x807060504030201 UL)", Write(s => { Block1(s); Block2(s); }));
        }

        [Fact]
        public void EnumTests()
        {
            Assert.Equal(@"0 name: {
    0 0 = 0 (0x0 uy) // None
    1 1 = 1 (0x1 uy) // Some
    2 2 = 2 (0x2 uy) // Both
    3 3 = 3 (0x3 uy) // Many
    4 4 = 255 (0xFF uy) // All
}",
                Write(s => s.List("name", new[]
                {
                    ByteEnum.None,
                    ByteEnum.Some,
                    ByteEnum.Both,
                    ByteEnum.Many,
                    ByteEnum.All
                }, 5, (n, v, s2) => s2.EnumU8(n, v))));

            Assert.Equal(@"0 name: {
    0 0 = 0 (0x0 us) // None
    2 1 = 1 (0x1 us) // Some
    4 2 = 2 (0x2 us) // Both
    6 3 = 3 (0x3 us) // Many
    8 4 = 65535 (0xFFFF us) // All
}",
                Write(s => s.List("name", new List<UShortEnum> 
                {
                    UShortEnum.None,
                    UShortEnum.Some,
                    UShortEnum.Both,
                    UShortEnum.Many,
                    UShortEnum.All,
                }, 5, (n, v, s2) => s2.EnumU16(n, v))));

            Assert.Equal(@"0 name: {
    0 0 = 0 (0x0 u) // None
    4 1 = 1 (0x1 u) // Some
    8 2 = 2 (0x2 u) // Both
    C 3 = 3 (0x3 u) // Many
    10 4 = 4294967295 (0xFFFFFFFF u) // All
}",
                Write(s => s.List("name", new List<UIntEnum>
                {
                    UIntEnum.None, 
                    UIntEnum.Some, 
                    UIntEnum.Both, 
                    UIntEnum.Many, 
                    UIntEnum.All, 
                }, 5, (n, v, s2) => s2.EnumU32(n, v))));
        }

        [Fact]
        public void ObjectTests()
        {
            Assert.Equal(@"0 name: {
    0 SByte = -1 (0xFF y)
    1 Byte = 1 (0x1 uy)
    2 Short = -2 (0xFFFE s)
    4 UShort = 2 (0x2 us)
    6 Int = -3 (0xFFFFFFFD)
    A Uint = 3 (0x3 u)
    E Long = -4 (0xFFFFFFFFFFFFFFFC L)
    16 ULong = 4 (0x4 UL)
    1E ByteEnum = 1 (0x1 uy) // Some
    1F UShortEnum = 2 (0x2 us) // Both
    21 UIntEnum = 3 (0x3 u) // Many
    25 Fixed = ""Fixed""
    2A NullTerm = ""Null""
    2F Guid = {fa6fa50d-be6f-4736-87df-e17280f14248}
}",
                Write(s => s.Object("name", Example.TestInstance, Example.Serdes)));
        }
    }
}