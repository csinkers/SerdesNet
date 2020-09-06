using System;
using System.IO;
using System.Text;
using Xunit;

namespace SerdesNet.Tests
{
    public class JsonWriterTests
    {
        static string Write(Action<ISerializer> action, bool elideDefaults = false, bool compact = true)
        {
            var ms = new MemoryStream();
            using (var tw = new StreamWriter(ms))
            {
                var s = new JsonWriter(tw, null, Encoding.UTF8, null, elideDefaults, compact);
                action(s);
            }

            var str = Encoding.UTF8.GetString(ms.ToArray());
            return str;
        }

        [Fact]
        public void Int8Test()
        {
            Assert.Equal("0",    Write(s => s.Int8(null, 0   )));
            Assert.Equal("1",    Write(s => s.Int8(null, 1   )));
            Assert.Equal("127",  Write(s => s.Int8(null, 127 )));
            Assert.Equal("-128", Write(s => s.Int8(null, -128)));
            Assert.Equal("-1",   Write(s => s.Int8(null, -1  )));
            Assert.Equal("\"A\": 0",    Write(s => s.Int8("A", 0   )));
            Assert.Equal("\"B\": 1",    Write(s => s.Int8("B", 1   )));
            Assert.Equal("\"C\": 127",  Write(s => s.Int8("C", 127 )));
            Assert.Equal("\"D\": -128", Write(s => s.Int8("D", -128)));
            Assert.Equal("\"E\": -1",   Write(s => s.Int8("E", -1  )));
            Assert.Equal("0, 1", Write(s => { s.Int8(null, 0); s.Int8(null, 1); }));
        }

        [Fact]
        public void UInt8Test()
        {
            Assert.Equal("0",   Write(s => s.UInt8(null, 0  )));
            Assert.Equal("1",   Write(s => s.UInt8(null, 1  )));
            Assert.Equal("127", Write(s => s.UInt8(null, 127)));
            Assert.Equal("128", Write(s => s.UInt8(null, 128)));
            Assert.Equal("255", Write(s => s.UInt8(null, 255)));
            Assert.Equal("\"A\": 0",   Write(s => s.UInt8("A", 0  )));
            Assert.Equal("\"B\": 1",   Write(s => s.UInt8("B", 1  )));
            Assert.Equal("\"C\": 127", Write(s => s.UInt8("C", 127)));
            Assert.Equal("\"D\": 128", Write(s => s.UInt8("D", 128)));
            Assert.Equal("\"E\": 255", Write(s => s.UInt8("E", 255)));
            Assert.Equal("0, 1", Write(s => { s.UInt8(null, 0); s.UInt8(null, 1); }));
        }

        [Fact]
        public void Int16Test()
        {
            Assert.Equal("0",      Write(s => s.Int16(null, 0     )));
            Assert.Equal("1",      Write(s => s.Int16(null, 1     )));
            Assert.Equal("32767",  Write(s => s.Int16(null, 32767 )));
            Assert.Equal("-32768", Write(s => s.Int16(null, -32768)));
            Assert.Equal("-1",     Write(s => s.Int16(null, -1    )));
            Assert.Equal("\"A\": 0",      Write(s => s.Int16("A", 0     )));
            Assert.Equal("\"B\": 1",      Write(s => s.Int16("B", 1     )));
            Assert.Equal("\"C\": 32767",  Write(s => s.Int16("C", 32767 )));
            Assert.Equal("\"D\": -32768", Write(s => s.Int16("D", -32768)));
            Assert.Equal("\"E\": -1",     Write(s => s.Int16("E", -1    )));
            Assert.Equal("0, 1", Write(s => { s.Int16(null, 0); s.Int16(null, 1); }));
        }

        [Fact]
        public void UInt16Test()
        {
            Assert.Equal("0", Write(s => s.UInt16(null, 0    )));
            Assert.Equal("1", Write(s => s.UInt16(null, 1    )));
            Assert.Equal("32767", Write(s => s.UInt16(null, 32767)));
            Assert.Equal("32768", Write(s => s.UInt16(null, 32768)));
            Assert.Equal("65535", Write(s => s.UInt16(null, 65535)));
            Assert.Equal("\"A\": 0", Write(s => s.UInt16("A", 0    )));
            Assert.Equal("\"B\": 1", Write(s => s.UInt16("B", 1    )));
            Assert.Equal("\"C\": 32767", Write(s => s.UInt16("C", 32767)));
            Assert.Equal("\"D\": 32768", Write(s => s.UInt16("D", 32768)));
            Assert.Equal("\"E\": 65535", Write(s => s.UInt16("E", 65535)));
            Assert.Equal("0, 1", Write(s => { s.UInt16(null, 0); s.UInt16(null, 1); }));
        }

        [Fact]
        public void Int32Test()
        {
            Assert.Equal("0",           Write(s => s.Int32(null, 0          )));
            Assert.Equal("1",           Write(s => s.Int32(null, 1          )));
            Assert.Equal("2147483647",  Write(s => s.Int32(null, 0x7fffffff )));
            Assert.Equal("-2147483648", Write(s => s.Int32(null, -0x80000000)));
            Assert.Equal("-1",          Write(s => s.Int32(null, -1         )));
            Assert.Equal("\"A\": 0",           Write(s => s.Int32("A", 0          )));
            Assert.Equal("\"B\": 1",           Write(s => s.Int32("B", 1          )));
            Assert.Equal("\"C\": 2147483647",  Write(s => s.Int32("C", 0x7fffffff )));
            Assert.Equal("\"D\": -2147483648", Write(s => s.Int32("D", -0x80000000)));
            Assert.Equal("\"E\": -1",          Write(s => s.Int32("E", -1         )));
            Assert.Equal("0, 1", Write(s => { s.Int32(null, 0); s.Int32(null, 1); }));
        }

        [Fact]
        public void UInt32Test()
        {
            Assert.Equal("0",          Write(s => s.UInt32(null, 0u           )));
            Assert.Equal("1",          Write(s => s.UInt32(null, 1u           )));
            Assert.Equal("2147483647", Write(s => s.UInt32(null, 0x7fffffffu  )));
            Assert.Equal("2147483648", Write(s => s.UInt32(null, 0x80000000   )));
            Assert.Equal("4294967295", Write(s => s.UInt32(null, uint.MaxValue)));
            Assert.Equal("\"A\": 0",          Write(s => s.UInt32("A", 0u           )));
            Assert.Equal("\"B\": 1",          Write(s => s.UInt32("B", 1u           )));
            Assert.Equal("\"C\": 2147483647", Write(s => s.UInt32("C", 0x7fffffffu  )));
            Assert.Equal("\"D\": 2147483648", Write(s => s.UInt32("D", 0x80000000   )));
            Assert.Equal("\"E\": 4294967295", Write(s => s.UInt32("E", uint.MaxValue)));
            Assert.Equal("0, 1", Write(s => { s.UInt32(null, 0); s.UInt32(null, 1); }));
        }

        [Fact]
        public void Int64Test()
        {
            Assert.Equal("0",                   Write(s => s.Int64(null, 0                   )));
            Assert.Equal("1",                   Write(s => s.Int64(null, 1                   )));
            Assert.Equal("9223372036854775807", Write(s => s.Int64(null, 0x7fffffff_ffffffff )));
            Assert.Equal("-9223372036854775808", Write(s => s.Int64(null, -0x80000000_00000000)));
            Assert.Equal("-1", Write(s => s.Int64(null, -1                  )));
            Assert.Equal("\"A\": 0",                   Write(s => s.Int64("A", 0                   )));
            Assert.Equal("\"B\": 1",                   Write(s => s.Int64("B", 1                   )));
            Assert.Equal("\"C\": 9223372036854775807", Write(s => s.Int64("C", 0x7fffffff_ffffffff )));
            Assert.Equal("\"D\": -9223372036854775808", Write(s => s.Int64("D", -0x80000000_00000000)));
            Assert.Equal("\"E\": -1", Write(s => s.Int64("E", -1                  )));
            Assert.Equal("0, 1", Write(s => { s.Int64(null, 0); s.Int64(null, 1); }));
        }

        [Fact]
        public void UInt64Test()
        {
            Assert.Equal("0",                    Write(s => s.UInt64(null, 0uL                  )));
            Assert.Equal("1",                    Write(s => s.UInt64(null, 1uL                  )));
            Assert.Equal("9223372036854775807",  Write(s => s.UInt64(null, 0x7fffffff_ffffffffuL)));
            Assert.Equal("9223372036854775808",  Write(s => s.UInt64(null, 0x80000000_00000000uL)));
            Assert.Equal("18446744073709551615", Write(s => s.UInt64(null, ulong.MaxValue       )));
            Assert.Equal("\"A\": 0",                    Write(s => s.UInt64("A", 0uL                  )));
            Assert.Equal("\"B\": 1",                    Write(s => s.UInt64("B", 1uL                  )));
            Assert.Equal("\"C\": 9223372036854775807",  Write(s => s.UInt64("C", 0x7fffffff_ffffffffuL)));
            Assert.Equal("\"D\": 9223372036854775808",  Write(s => s.UInt64("D", 0x80000000_00000000uL)));
            Assert.Equal("\"E\": 18446744073709551615", Write(s => s.UInt64("E", ulong.MaxValue       )));
            Assert.Equal("0, 1", Write(s => { s.UInt64(null, 0); s.UInt64(null, 1); }));
        }

        [Fact]
        public void FixedLengthStringTest()
        {
            Assert.Equal("\"A\"",
                Write(s => s.FixedLengthString(null, "A", 1)));

            Assert.Equal("\"A\", \"B\"",
                Write(s =>
                {
                    s.FixedLengthString(null, "A", 1);
                    s.FixedLengthString(null, "B", 1);
                }));

            Assert.Throws<InvalidOperationException>(() =>
                Write(s => s.FixedLengthString(null, "Too long", 1)));
        }

        [Fact]
        public void NullTerminatedStringTest()
        {
            Assert.Equal("\"A\"", Write(s => s.NullTerminatedString(null, "A")));
            Assert.Equal("\"A\", \"B\"", Write(s =>
            {
                s.NullTerminatedString(null, "A");
                s.NullTerminatedString(null, "B");
            }));
        }

        [Fact]
        public void GuidTest()
        {
            Assert.Equal("\"{FA6FA50D-BE6F-4736-87DF-E17280F14248}\"",
                Write(s => s.Guid(null, Guid.Parse("{FA6FA50D-BE6F-4736-87DF-E17280F14248}"))));
        }

        [Fact]
        public void ListTest()
        {
            static byte UInt8Serdes(int i, byte v, ISerializer s) => s.UInt8(null, v);
            Assert.Equal("[ 1, 2, 3 ]",
                Write(s => s.List(
                        null,
                        new byte[] { 1, 2, 3 },
                        3,
                        UInt8Serdes,
                        x => throw new InvalidOperationException())));

            Assert.Equal("[ 1, 2, 3]",
                Write(s => s.List(
                        null,
                        new byte[] { 0, 1, 2, 3 },
                        3,
                        1,
                        UInt8Serdes,
                        x => throw new InvalidOperationException())));
        }

        [Fact]
        public void RepeatTest()
        {
            Assert.Equal("[ 0, 0, 0, 0 ]", Write(s => s.RepeatU8(null, 0, 4)));
            Assert.Equal("[ 1, 1, 1, 1 ]", Write(s => s.RepeatU8(null, 1, 4)));
            Assert.Equal("[ 1 ]", Write(s => s.RepeatU8(null, 1, 1)));
        }

        [Fact]
        public void ByteArrayTest()
        {
            Assert.Equal("[ 0, 1, 2, 3 ]", 
                Write(s => s.ByteArray(null, new byte[] { 0, 1, 2, 3 }, 4)));

            Assert.Equal("0x[00010203]",
                Write(s =>s.ByteArrayHex(null, new byte[] { 0, 1, 2, 3 }, 4)));
        }
/*
        [Fact]
        public void OffsetTests()
        {
            Write(x => Assert.Equal(SerializerMode.Writing, x.Mode));
            Write(x => Assert.Equal(long.MaxValue, x.BytesRemaining));
            Write(x => Assert.False(x.IsComplete()));

            static void Block1(ISerializer s)
            {
                Assert.Equal(0, s.Offset);
                s.Begin(); Assert.Equal(0, s.Offset);
                s.End(); Assert.Equal(0, s.Offset);
                s.Comment("x"); Assert.Equal(0, s.Offset);
                s.NewLine(); Assert.Equal(0, s.Offset);
                s.PushVersion(1); Assert.Equal(0, s.Offset);
                Assert.Equal(1, s.PopVersion());
                Assert.Equal(0, s.Offset);

                s.UInt8(null, 0); Assert.Equal(1, s.Offset);
                s.UInt16(null, 0x201); Assert.Equal(3, s.Offset);
                s.UInt32(null, 0x6050403u); Assert.Equal(7, s.Offset);
            }

            static void Block2(ISerializer s)
            {
                s.Seek(0);
                Assert.Equal(0, s.Offset);
                s.UInt64(null, 0x807060504030201uL);
                Assert.Equal(8, s.Offset);
            }

            Assert.Equal(new byte[] { 0, 1, 2, 3, 4, 5, 6, }, Write(Block1));
            Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, Write(Block2));
            Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, Write(s => { Block1(s); Block2(s); }));
        }

        [Fact]
        public void EnumTests()
        {
            Assert.Equal(new byte[] { 0, 1, 2, 3, 0xff },
                Write(s => s.List(null, new[]
                {
                    ByteEnum.None,
                    ByteEnum.Some,
                    ByteEnum.Both,
                    ByteEnum.Many,
                    ByteEnum.All
                }, 5, S.EnumU8)));

            Assert.Equal(new byte[] {  0, 0, 1, 0, 2, 0, 3, 0, 0xff, 0xff },
                Write(s => s.List(null, new List<UShortEnum> 
                {
                    UShortEnum.None,
                    UShortEnum.Some,
                    UShortEnum.Both,
                    UShortEnum.Many,
                    UShortEnum.All,
                }, 5, S.EnumU16)));

            Assert.Equal(new byte[]
                {
                    0,0,0,0,
                    1,0,0,0,
                    2,0,0,0,
                    3,0,0,0,
                    0xff, 0xff, 0xff, 0xff,
                },
                Write(s => s.List(null, new List<UIntEnum>
                {
                    UIntEnum.None, 
                    UIntEnum.Some, 
                    UIntEnum.Both, 
                    UIntEnum.Many, 
                    UIntEnum.All, 
                }, 5, S.EnumU32)));
        }

        [Fact]
        public void TransformEnumTests()
        {
            Assert.Equal(new byte[] { 0, 1, 2, 3, 0xff },
                Write(s => s.List(null, new[]
                {
                    (ByteEnum?)null,
                    ByteEnum.None,
                    ByteEnum.Some,
                    ByteEnum.Both,
                    ByteEnum.AlmostAll,
                }, 5, (i, v, s2) => s2.TransformEnumU8(null, v, ZeroToNullConverter<ByteEnum>.Instance))));

            Assert.Equal(new byte[] { 0, 1, 2, 3, 0xff },
                Write(s => s.List(null, new[]
                {
                    ByteEnum.None,
                    ByteEnum.Some,
                    ByteEnum.Both,
                    ByteEnum.Many,
                    (ByteEnum?)null
                }, 5,
                (i, v, s2) => s2.TransformEnumU8(null, v, MaxToNullConverter<ByteEnum>.Instance))));

            Assert.Equal(new byte[] { 0, 0, 1, 0, 2, 0, 3, 0, 0xff, 0xff },
                Write(s => s.List(null, new[]
                {
                    (UShortEnum?) null,
                    UShortEnum.None,
                    UShortEnum.Some,
                    UShortEnum.Both,
                    UShortEnum.AlmostAll,
                }, 5,
                (i, v, s2) => s2.TransformEnumU16(null, v, ZeroToNullConverter<UShortEnum>.Instance))));

            Assert.Equal(new byte[] { 0, 0, 1, 0, 2, 0, 3, 0, 0xff, 0xff },
                Write(s => s.List(null, new[]
                {
                    UShortEnum.None,
                    UShortEnum.Some,
                    UShortEnum.Both,
                    UShortEnum.Many,
                    (UShortEnum?)null
                }, 5,
                (i, v, s2) => s2.TransformEnumU16(null, v, MaxToNullConverter<UShortEnum>.Instance)))
            );

            Assert.Equal(
                new byte[]
                {
                    0, 0, 0, 0,
                    1, 0, 0, 0,
                    2, 0, 0, 0,
                    3, 0, 0, 0,
                    0xff, 0xff, 0xff, 0xff,
                },
                Write(s => s.List(null, new[]
                {
                    (UIntEnum?)null,
                    UIntEnum.None,
                    UIntEnum.Some,
                    UIntEnum.Both,
                    UIntEnum.AlmostAll,
                }, 5,
                (i, v, s2) => s2.TransformEnumU32(null, v, ZeroToNullConverter<UIntEnum>.Instance))));

            Assert.Equal(
                new byte[]
                {
                    0, 0, 0, 0,
                    1, 0, 0, 0,
                    2, 0, 0, 0,
                    3, 0, 0, 0,
                    0xff, 0xff, 0xff, 0xff,
                },
                Write(s => s.List(null, new[]
                {
                    UIntEnum.None,
                    UIntEnum.Some,
                    UIntEnum.Both,
                    UIntEnum.Many,
                    (UIntEnum?)null
                }, 5, (i, v, s2) => s2.TransformEnumU32(null, v, MaxToNullConverter<UIntEnum>.Instance))));
        }

        [Fact]
        public void TransformTests()
        {
            Assert.Equal(new byte[] { 0, 1, 2, 255, 254 }, Write(s =>
              {
                  s.Transform<byte, byte?>(null, null, S.UInt8, ZeroToNullConverter.Instance);
                  s.Transform<byte, byte?>(null, 0, S.UInt8, ZeroToNullConverter.Instance);
                  s.Transform<byte, byte?>(null, 1, S.UInt8, ZeroToNullConverter.Instance);
                  s.Transform<byte, byte?>(null, byte.MaxValue - 1, S.UInt8, ZeroToNullConverter.Instance);
                  s.Transform<byte, byte?>(null, byte.MaxValue - 2, S.UInt8, ZeroToNullConverter.Instance);
              }));

            Assert.Equal(new byte[] { 0, 1, 2, 255, 254 }, Write(s =>
            {
                s.Transform<byte, byte?>(null, 0, S.UInt8, MaxToNullConverter.Instance);
                s.Transform<byte, byte?>(null, 1, S.UInt8, MaxToNullConverter.Instance);
                s.Transform<byte, byte?>(null, 2, S.UInt8, MaxToNullConverter.Instance);
                s.Transform<byte, byte?>(null, null, S.UInt8, MaxToNullConverter.Instance);
                s.Transform<byte, byte?>(null, byte.MaxValue - 1, S.UInt8, MaxToNullConverter.Instance);
            }));

            Assert.Equal(new byte[] { 0, 0, 1, 0, 2, 0, 255, 255, 254, 255 }, Write(s =>
            {
                s.Transform<ushort, ushort?>(null, null, S.UInt16, ZeroToNullConverter.Instance);
                s.Transform<ushort, ushort?>(null, 0, S.UInt16, ZeroToNullConverter.Instance);
                s.Transform<ushort, ushort?>(null, 1, S.UInt16, ZeroToNullConverter.Instance);
                s.Transform<ushort, ushort?>(null, ushort.MaxValue - 1, S.UInt16, ZeroToNullConverter.Instance);
                s.Transform<ushort, ushort?>(null, ushort.MaxValue - 2, S.UInt16, ZeroToNullConverter.Instance);
            }));

            Assert.Equal(new byte[] { 0, 0, 1, 0, 2, 0, 255, 255, 254, 255 }, Write(s =>
            {
                s.Transform<ushort, ushort?>(null, 0, S.UInt16, MaxToNullConverter.Instance);
                s.Transform<ushort, ushort?>(null, 1, S.UInt16, MaxToNullConverter.Instance);
                s.Transform<ushort, ushort?>(null, 2, S.UInt16, MaxToNullConverter.Instance);
                s.Transform<ushort, ushort?>(null, null, S.UInt16, MaxToNullConverter.Instance);
                s.Transform<ushort, ushort?>(null, ushort.MaxValue - 1, S.UInt16, MaxToNullConverter.Instance);
            }));

            Assert.Equal(new byte[]
                {
                    0,0,0,0,
                    1,0,0,0,
                    2,0,0,0,
                    255,255,255,255,
                    254,255,255,255
                }, Write(s =>
                {
                    s.Transform<uint, uint?>(null, null, S.UInt32, ZeroToNullConverter.Instance);
                    s.Transform<uint, uint?>(null, 0, S.UInt32, ZeroToNullConverter.Instance);
                    s.Transform<uint, uint?>(null, 1, S.UInt32, ZeroToNullConverter.Instance);
                    s.Transform<uint, uint?>(null, uint.MaxValue - 1, S.UInt32, ZeroToNullConverter.Instance);
                    s.Transform<uint, uint?>(null, uint.MaxValue - 2, S.UInt32, ZeroToNullConverter.Instance);
                }));

            Assert.Equal(new byte[]
                {
                    0,0,0,0,
                    1,0,0,0,
                    2,0,0,0,
                    255,255,255,255,
                    254,255,255,255
                }, Write(s =>
                {
                    s.Transform<uint, uint?>(null, 0, S.UInt32, MaxToNullConverter.Instance);
                    s.Transform<uint, uint?>(null, 1, S.UInt32, MaxToNullConverter.Instance);
                    s.Transform<uint, uint?>(null, 2, S.UInt32, MaxToNullConverter.Instance);
                    s.Transform<uint, uint?>(null, null, S.UInt32, MaxToNullConverter.Instance);
                    s.Transform<uint, uint?>(null, uint.MaxValue - 1, S.UInt32, MaxToNullConverter.Instance);
                }));
        }

        [Fact]
        public void ObjectTests()
        {
            Assert.Equal(Example.ExampleBuffer,
                Write(s => s.Object(null, Example.TestInstance, Example.Serdes)));
        }

        [Fact]
        public void CheckTests()
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            var s = new GenericBinaryWriter(
                bw,
                Encoding.UTF8.GetBytes,
                m => throw new InvalidOperationException(m));

            s.UInt8(null, 1);  // 0->1
            s.Check();
            bw.Write((byte)5); // 1->2 (stream only)
            Assert.Throws<InvalidOperationException>(() => s.Check());
            s.Seek(ms.Position); // 1->2 (s only)
            s.Check();
            s.UInt8(null, 3); // 2->3
            s.Check();
            s.UInt8(null, 4);
            Assert.Equal(new byte[] { 1, 5, 3, 4 }, ms.ToArray());
        }
*/
    }
}
