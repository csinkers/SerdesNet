using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace SerdesNet.Tests
{
    public class BinaryWriterTests
    {
        static byte[] Write(Action<ISerializer> action, Action<string> assertHandler = null)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            var s = new GenericBinaryWriter(
                bw,
                Encoding.UTF8.GetBytes,
                assertHandler ?? (m => throw new InvalidOperationException(m)));
            action(s);
            return ms.ToArray();
        }

        [Fact]
        public void Int8Test()
        {
            Assert.Equal(new byte[] { 0 }, Write(s => s.Int8("", 0)));
            Assert.Equal(new byte[] { 1 }, Write(s => s.Int8("", 1)));
            Assert.Equal(new byte[] { 127 }, Write(s => s.Int8("", 127)));
            Assert.Equal(new byte[] { 128 }, Write(s => s.Int8("", -128)));
            Assert.Equal(new byte[] { 255 }, Write(s => s.Int8("", -1)));
        }

        [Fact]
        public void UInt8Test()
        {
            Assert.Equal(new byte[] { 0 }, Write(s => s.UInt8("", 0)));
            Assert.Equal(new byte[] { 1 }, Write(s => s.UInt8("", 1)));
            Assert.Equal(new byte[] { 127 }, Write(s => s.UInt8("", 127)));
            Assert.Equal(new byte[] { 128 }, Write(s => s.UInt8("", 128)));
            Assert.Equal(new byte[] { 255 }, Write(s => s.UInt8("", 255)));
        }

        [Fact]
        public void Int16Test()
        {
            Assert.Equal(new byte[] { 0, 0 }, Write(s => s.Int16("", 0)));
            Assert.Equal(new byte[] { 1, 0 }, Write(s => s.Int16("", 1)));
            Assert.Equal(new byte[] { 255, 127 }, Write(s => s.Int16("", 32767)));
            Assert.Equal(new byte[] { 0, 128 }, Write(s => s.Int16("", -32768)));
            Assert.Equal(new byte[] { 255, 255 }, Write(s => s.Int16("", -1)));
        }

        [Fact]
        public void UInt16Test()
        {
            Assert.Equal(new byte[] { 0, 0 }, Write(s => s.UInt16("", 0)));
            Assert.Equal(new byte[] { 1, 0 }, Write(s => s.UInt16("", 1)));
            Assert.Equal(new byte[] { 255, 127 }, Write(s => s.UInt16("", 32767)));
            Assert.Equal(new byte[] { 0, 128 }, Write(s => s.UInt16("", 32768)));
            Assert.Equal(new byte[] { 255, 255 }, Write(s => s.UInt16("", 65535)));
        }

        [Fact]
        public void Int32Test()
        {
            Assert.Equal(new byte[] { 0, 0, 0, 0 }, Write(s => s.Int32("", 0)));
            Assert.Equal(new byte[] { 1, 0, 0, 0 }, Write(s => s.Int32("", 1)));
            Assert.Equal(new byte[] { 255, 255, 255, 127 }, Write(s => s.Int32("", 0x7fffffff)));
            Assert.Equal(new byte[] { 0, 0, 0, 128 }, Write(s => s.Int32("", -0x80000000)));
            Assert.Equal(new byte[] { 255, 255, 255, 255 }, Write(s => s.Int32("", -1)));
        }

        [Fact]
        public void UInt32Test()
        {
            Assert.Equal(new byte[] { 0, 0, 0, 0 }, Write(s => s.UInt32("", 0u)));
            Assert.Equal(new byte[] { 1, 0, 0, 0 }, Write(s => s.UInt32("", 1u)));
            Assert.Equal(new byte[] { 255, 255, 255, 127 }, Write(s => s.UInt32("", 0x7fffffffu)));
            Assert.Equal(new byte[] { 0, 0, 0, 128 }, Write(s => s.UInt32("", 0x80000000)));
            Assert.Equal(new byte[] { 255, 255, 255, 255 }, Write(s => s.UInt32("", uint.MaxValue)));
        }

        [Fact]
        public void Int64Test()
        {
            Assert.Equal(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, Write(s => s.Int64("", 0)));
            Assert.Equal(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0 }, Write(s => s.Int64("", 1)));
            Assert.Equal(new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 }, Write(s => s.Int64("", 0x7fffffff_ffffffff)));
            Assert.Equal(new byte[] { 0, 0, 0, 0, 0, 0, 0, 128 }, Write(s => s.Int64("", -0x80000000_00000000)));
            Assert.Equal(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 }, Write(s => s.Int64("", -1)));
        }

        [Fact]
        public void UInt64Test()
        {
            Assert.Equal(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, Write(s => s.UInt64("", 0uL)));
            Assert.Equal(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0 }, Write(s => s.UInt64("", 1uL)));
            Assert.Equal(new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 }, Write(s => s.UInt64("", 0x7fffffff_ffffffffuL)));
            Assert.Equal(new byte[] { 0, 0, 0, 0, 0, 0, 0, 128 }, Write(s => s.UInt64("", 0x80000000_00000000uL)));
            Assert.Equal(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 }, Write(s => s.UInt64("", ulong.MaxValue)));
        }

        [Fact]
        public void FixedLengthStringTest()
        {
            Assert.Equal(new byte[] { 65 },
                Write(s => s.FixedLengthString("", "A", 1)));
            Assert.Throws<InvalidOperationException>(() =>
                Write(s => s.FixedLengthString("", "Too long", 1)));
        }

        [Fact]
        public void NullTerminatedStringTest()
        {
            Assert.Equal(new byte[] { 65, 0 }, Write(s => s.NullTerminatedString("", "A")));
        }

        [Fact]
        public void GuidTest()
        {
            Assert.Equal(new byte[]
                {
                    0x0d, 0xa5, 0x6f, 0xfa,
                    0x6f, 0xbe,
                    0x36, 0x47,
                    0x87, 0xdf,
                    0xe1, 0x72, 0x80, 0xf1, 0x42, 0x48
                },
                Write(s => s.Guid("", Guid.Parse("{FA6FA50D-BE6F-4736-87DF-E17280F14248}"))));
        }

        [Fact]
        public void ListTest()
        {
            static byte UInt8Serdes(int i, byte v, ISerializer s) => s.UInt8("", v);
            Assert.Equal(new byte[] { 1, 2, 3 },
                Write(s => s.List(
                        "",
                        new byte[] { 1, 2, 3 },
                        3,
                        UInt8Serdes,
                        x => throw new InvalidOperationException())));

            Assert.Equal(new byte[] { 1, 2, 3 },
                Write(s => s.List(
                        "",
                        new byte[] { 0, 1, 2, 3 },
                        3,
                        1,
                        UInt8Serdes,
                        x => throw new InvalidOperationException())));
        }

        [Fact]
        public void RepeatTest()
        {
            Assert.Equal(new byte[] { 0, 0, 0, 0 }, Write(s => s.RepeatU8("", 0, 4)));
            Assert.Equal(new byte[] { 1, 1, 1, 1 }, Write(s => s.RepeatU8("", 1, 4)));
            Assert.Equal(new byte[] { 1 }, Write(s => s.RepeatU8("", 1, 1)));
        }

        [Fact]
        public void ByteArrayTest()
        {
            Assert.Equal(new byte[] { 0, 1, 2, 3 },
                Write(s => s.ByteArray("", new byte[] { 0, 1, 2, 3 }, 4)));

            Assert.Equal(new byte[] { 0, 1, 2, 3 }, 
                Write(s =>s.ByteArrayHex("", new byte[] { 0, 1, 2, 3 }, 4)));
        }

        [Fact]
        public void OffsetTests()
        {
            Write(x => Assert.Equal(SerializerMode.Writing, x.Mode));
            Write(x => Assert.Equal(long.MaxValue, x.BytesRemaining));
            Write(x => Assert.False(x.IsComplete()));

            static void Block1(ISerializer s)
            {
                Assert.Equal(0, s.Offset);
                s.Comment("x"); Assert.Equal(0, s.Offset);
                s.NewLine(); Assert.Equal(0, s.Offset);
                s.PushVersion(1); Assert.Equal(0, s.Offset);
                Assert.Equal(1, s.PopVersion());
                Assert.Equal(0, s.Offset);

                s.UInt8("", 0); Assert.Equal(1, s.Offset);
                s.UInt16("", 0x201); Assert.Equal(3, s.Offset);
                s.UInt32("", 0x6050403u); Assert.Equal(7, s.Offset);
            }

            static void Block2(ISerializer s)
            {
                s.Seek(0);
                Assert.Equal(0, s.Offset);
                s.UInt64("", 0x807060504030201uL);
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
                Write(s => s.List("", new[]
                {
                    ByteEnum.None,
                    ByteEnum.Some,
                    ByteEnum.Both,
                    ByteEnum.Many,
                    ByteEnum.All
                }, 5, S.EnumU8)));

            Assert.Equal(new byte[] {  0, 0, 1, 0, 2, 0, 3, 0, 0xff, 0xff },
                Write(s => s.List("", new List<UShortEnum> 
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
                Write(s => s.List("", new List<UIntEnum>
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
                Write(s => s.List("", new[]
                {
                    (ByteEnum?)null,
                    ByteEnum.None,
                    ByteEnum.Some,
                    ByteEnum.Both,
                    ByteEnum.AlmostAll,
                }, 5, (i, v, s2) => s2.TransformEnumU8("", v, ZeroToNullConverter<ByteEnum>.Instance))));

            Assert.Equal(new byte[] { 0, 1, 2, 3, 0xff },
                Write(s => s.List("", new[]
                {
                    ByteEnum.None,
                    ByteEnum.Some,
                    ByteEnum.Both,
                    ByteEnum.Many,
                    (ByteEnum?)null
                }, 5,
                (i, v, s2) => s2.TransformEnumU8("", v, MaxToNullConverter<ByteEnum>.Instance))));

            Assert.Equal(new byte[] { 0, 0, 1, 0, 2, 0, 3, 0, 0xff, 0xff },
                Write(s => s.List("", new[]
                {
                    (UShortEnum?) null,
                    UShortEnum.None,
                    UShortEnum.Some,
                    UShortEnum.Both,
                    UShortEnum.AlmostAll,
                }, 5,
                (i, v, s2) => s2.TransformEnumU16("", v, ZeroToNullConverter<UShortEnum>.Instance))));

            Assert.Equal(new byte[] { 0, 0, 1, 0, 2, 0, 3, 0, 0xff, 0xff },
                Write(s => s.List("", new[]
                {
                    UShortEnum.None,
                    UShortEnum.Some,
                    UShortEnum.Both,
                    UShortEnum.Many,
                    (UShortEnum?)null
                }, 5,
                (i, v, s2) => s2.TransformEnumU16("", v, MaxToNullConverter<UShortEnum>.Instance)))
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
                Write(s => s.List("", new[]
                {
                    (UIntEnum?)null,
                    UIntEnum.None,
                    UIntEnum.Some,
                    UIntEnum.Both,
                    UIntEnum.AlmostAll,
                }, 5,
                (i, v, s2) => s2.TransformEnumU32("", v, ZeroToNullConverter<UIntEnum>.Instance))));

            Assert.Equal(
                new byte[]
                {
                    0, 0, 0, 0,
                    1, 0, 0, 0,
                    2, 0, 0, 0,
                    3, 0, 0, 0,
                    0xff, 0xff, 0xff, 0xff,
                },
                Write(s => s.List("", new[]
                {
                    UIntEnum.None,
                    UIntEnum.Some,
                    UIntEnum.Both,
                    UIntEnum.Many,
                    (UIntEnum?)null
                }, 5, (i, v, s2) => s2.TransformEnumU32("", v, MaxToNullConverter<UIntEnum>.Instance))));
        }

        [Fact]
        public void TransformTests()
        {
            Assert.Equal(new byte[] { 0, 1, 2, 255, 254 }, Write(s =>
              {
                  s.Transform<byte, byte?>("", null, S.UInt8, ZeroToNullConverter.Instance);
                  s.Transform<byte, byte?>("", 0, S.UInt8, ZeroToNullConverter.Instance);
                  s.Transform<byte, byte?>("", 1, S.UInt8, ZeroToNullConverter.Instance);
                  s.Transform<byte, byte?>("", byte.MaxValue - 1, S.UInt8, ZeroToNullConverter.Instance);
                  s.Transform<byte, byte?>("", byte.MaxValue - 2, S.UInt8, ZeroToNullConverter.Instance);
              }));

            Assert.Equal(new byte[] { 0, 1, 2, 255, 254 }, Write(s =>
            {
                s.Transform<byte, byte?>("", 0, S.UInt8, MaxToNullConverter.Instance);
                s.Transform<byte, byte?>("", 1, S.UInt8, MaxToNullConverter.Instance);
                s.Transform<byte, byte?>("", 2, S.UInt8, MaxToNullConverter.Instance);
                s.Transform<byte, byte?>("", null, S.UInt8, MaxToNullConverter.Instance);
                s.Transform<byte, byte?>("", byte.MaxValue - 1, S.UInt8, MaxToNullConverter.Instance);
            }));

            Assert.Equal(new byte[] { 0, 0, 1, 0, 2, 0, 255, 255, 254, 255 }, Write(s =>
            {
                s.Transform<ushort, ushort?>("", null, S.UInt16, ZeroToNullConverter.Instance);
                s.Transform<ushort, ushort?>("", 0, S.UInt16, ZeroToNullConverter.Instance);
                s.Transform<ushort, ushort?>("", 1, S.UInt16, ZeroToNullConverter.Instance);
                s.Transform<ushort, ushort?>("", ushort.MaxValue - 1, S.UInt16, ZeroToNullConverter.Instance);
                s.Transform<ushort, ushort?>("", ushort.MaxValue - 2, S.UInt16, ZeroToNullConverter.Instance);
            }));

            Assert.Equal(new byte[] { 0, 0, 1, 0, 2, 0, 255, 255, 254, 255 }, Write(s =>
            {
                s.Transform<ushort, ushort?>("", 0, S.UInt16, MaxToNullConverter.Instance);
                s.Transform<ushort, ushort?>("", 1, S.UInt16, MaxToNullConverter.Instance);
                s.Transform<ushort, ushort?>("", 2, S.UInt16, MaxToNullConverter.Instance);
                s.Transform<ushort, ushort?>("", null, S.UInt16, MaxToNullConverter.Instance);
                s.Transform<ushort, ushort?>("", ushort.MaxValue - 1, S.UInt16, MaxToNullConverter.Instance);
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
                    s.Transform<uint, uint?>("", null, S.UInt32, ZeroToNullConverter.Instance);
                    s.Transform<uint, uint?>("", 0, S.UInt32, ZeroToNullConverter.Instance);
                    s.Transform<uint, uint?>("", 1, S.UInt32, ZeroToNullConverter.Instance);
                    s.Transform<uint, uint?>("", uint.MaxValue - 1, S.UInt32, ZeroToNullConverter.Instance);
                    s.Transform<uint, uint?>("", uint.MaxValue - 2, S.UInt32, ZeroToNullConverter.Instance);
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
                    s.Transform<uint, uint?>("", 0, S.UInt32, MaxToNullConverter.Instance);
                    s.Transform<uint, uint?>("", 1, S.UInt32, MaxToNullConverter.Instance);
                    s.Transform<uint, uint?>("", 2, S.UInt32, MaxToNullConverter.Instance);
                    s.Transform<uint, uint?>("", null, S.UInt32, MaxToNullConverter.Instance);
                    s.Transform<uint, uint?>("", uint.MaxValue - 1, S.UInt32, MaxToNullConverter.Instance);
                }));
        }

        [Fact]
        public void ObjectTests()
        {
            Assert.Equal(Example.ExampleBuffer,
                Write(s => s.Object("", Example.TestInstance, Example.Serdes)));
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

            s.UInt8("", 1);  // 0->1
            s.Check();
            bw.Write((byte)5); // 1->2 (stream only)
            Assert.Throws<InvalidOperationException>(() => s.Check());
            s.Seek(ms.Position); // 1->2 (s only)
            s.Check();
            s.UInt8("", 3); // 2->3
            s.Check();
            s.UInt8("", 4);
            Assert.Equal(new byte[] { 1, 5, 3, 4 }, ms.ToArray());
        }
    }
}
