using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace SerdesNet.Tests
{
    public class BinaryReaderTests
    {
        static ISerializer Read(byte[] buffer, Action<string> assertHandler = null)
        {
            var ms = new MemoryStream(buffer);
            var br = new BinaryReader(ms);
            return new GenericBinaryReader(
                br,
                buffer.Length,
                Encoding.UTF8.GetString,
                assertHandler ?? (m => throw new InvalidOperationException(m)));
        }

        [Fact]
        public void Int8Test()
        {
            Assert.Equal(0, Read(new byte[] {0}).Int8("", 0));
            Assert.Equal(1, Read(new byte[] { 1 }).Int8("", 0));
            Assert.Equal(127, Read(new byte[] { 127 }).Int8("", 0));
            Assert.Equal(-128, Read(new byte[] { 128 }).Int8("", 0));
            Assert.Equal(-1, Read(new byte[] { 255 }).Int8("", 0));
        }

        [Fact]
        public void UInt8Test()
        {
            Assert.Equal(0, Read(new byte[] { 0 }).UInt8("", 0));
            Assert.Equal(1, Read(new byte[] { 1 }).UInt8("", 0));
            Assert.Equal(127, Read(new byte[] { 127 }).UInt8("", 0));
            Assert.Equal(128, Read(new byte[] { 128 }).UInt8("", 0));
            Assert.Equal(255, Read(new byte[] { 255 }).UInt8("", 0));
        }

        [Fact]
        public void Int16Test()
        {
            Assert.Equal(0, Read(new byte[] { 0, 0 }).Int16("", 0));
            Assert.Equal(1, Read(new byte[] { 1, 0 }).Int16("", 0));
            Assert.Equal(32767, Read(new byte[] { 255, 127 }).Int16("", 0));
            Assert.Equal(-32768, Read(new byte[] { 0, 128 }).Int16("", 0));
            Assert.Equal(-1, Read(new byte[] { 255, 255 }).Int16("", 0));
        }

        [Fact]
        public void UInt16Test()
        {
            Assert.Equal(0, Read(new byte[] { 0, 0 }).UInt16("", 0));
            Assert.Equal(1, Read(new byte[] { 1, 0 }).UInt16("", 0));
            Assert.Equal(32767, Read(new byte[] { 255, 127 }).UInt16("", 0));
            Assert.Equal(32768, Read(new byte[] { 0, 128 }).UInt16("", 0));
            Assert.Equal(65535, Read(new byte[] { 255, 255 }).UInt16("", 0));
        }

        [Fact]
        public void Int32Test()
        {
            Assert.Equal(0, Read(new byte[] { 0, 0, 0, 0 }).Int32("", 0));
            Assert.Equal(1, Read(new byte[] { 1, 0, 0, 0 }).Int32("", 0));
            Assert.Equal(0x7fffffff, Read(new byte[] { 255, 255, 255, 127 }).Int32("", 0));
            Assert.Equal(-0x80000000, Read(new byte[] { 0, 0, 0, 128 }).Int32("", 0));
            Assert.Equal(-1, Read(new byte[] { 255, 255, 255, 255 }).Int32("", 0));
        }

        [Fact]
        public void UInt32Test()
        {
            Assert.Equal(0u, Read(new byte[] { 0, 0, 0, 0 }).UInt32("", 0));
            Assert.Equal(1u, Read(new byte[] { 1, 0, 0, 0 }).UInt32("", 0));
            Assert.Equal(0x7fffffffu, Read(new byte[] { 255, 255, 255, 127 }).UInt32("", 0));
            Assert.Equal(0x80000000, Read(new byte[] { 0, 0, 0, 128 }).UInt32("", 0));
            Assert.Equal(uint.MaxValue, Read(new byte[] { 255, 255, 255, 255 }).UInt32("", 0));
        }

        [Fact]
        public void Int64Test()
        {
            Assert.Equal(0, Read(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }).Int64("", 0));
            Assert.Equal(1, Read(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0 }).Int64("", 0));
            Assert.Equal(0x7fffffff_ffffffff, Read(new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 }).Int64("", 0));
            Assert.Equal(-0x80000000_00000000, Read(new byte[] { 0,0,0,0,0,0,0,128 }).Int64("", 0));
            Assert.Equal(-1, Read(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 }).Int64("", 0));
        }

        [Fact]
        public void UInt64Test()
        {
            Assert.Equal(0uL, Read(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }).UInt64("", 0));
            Assert.Equal(1uL, Read(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0 }).UInt64("", 0));
            Assert.Equal(0x7fffffff_ffffffffuL, Read(new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 }).UInt64("", 0));
            Assert.Equal(0x80000000_00000000uL, Read(new byte[] { 0,0,0,0,0,0,0,128 }).UInt64("", 0));
            Assert.Equal(ulong.MaxValue, Read(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 }).UInt64("", 0));
        }

        [Fact]
        public void FixedLengthStringTest()
        {
            Assert.Equal("A", Read(new byte[] { 65 }).FixedLengthString("", "", 1));
            Assert.Throws<EndOfStreamException>(() => 
                Read(new byte[] { 65 })
                    .FixedLengthString("", "", 2));
        }

        [Fact]
        public void NullTerminatedStringTest()
        {
            Assert.Equal("A", Read(new byte[] { 65, 0 }).NullTerminatedString("", ""));
        }

        [Fact]
        public void GuidTest()
        {
            Assert.Equal(Guid.Parse("{FA6FA50D-BE6F-4736-87DF-E17280F14248}"),
                Read(new byte[]
                {
                    0x0d, 0xa5, 0x6f, 0xfa,
                    0x6f, 0xbe,
                    0x36, 0x47,
                    0x87, 0xdf,
                    0xe1, 0x72, 0x80, 0xf1, 0x42, 0x48
                }).Guid("", Guid.Empty));
            Assert.Throws<EndOfStreamException>(() => Read(new byte[] {1, 2, 3, 4}).Guid("", Guid.Empty));
        }

        [Fact]
        public void ListTest()
        {
            static byte UInt8Serdes(int i, byte v, ISerializer s) => s.UInt8("", v);
            Assert.Collection(
                Read(new byte[] { 1, 2, 3 })
                    .List("", null, 3, UInt8Serdes, x => new byte[x]),
                x => Assert.Equal(1, x),
                x => Assert.Equal(2, x),
                x => Assert.Equal(3, x)
            );

            Assert.Collection(
                Read(new byte[] { 1, 2, 3 })
                    .List("", null, 3, UInt8Serdes, _ => new List<byte>()),
                x => Assert.Equal(1, x),
                x => Assert.Equal(2, x),
                x => Assert.Equal(3, x)
            );

            Assert.Throws<EndOfStreamException>(() =>
                Read(new byte[] { 1, 2, 3 })
                    .List("", null, 4, UInt8Serdes, _ => new List<byte>())
            );

            Assert.Collection(
                Read(new byte[] { 1, 2, 3 })
                    .List("", null, 3, 1, UInt8Serdes, _ => new List<byte>()),
                x => Assert.Equal(1, x),
                x => Assert.Equal(1, x),
                x => Assert.Equal(2, x),
                x => Assert.Equal(3, x)
            );

            var l = new List<byte> { 5 };
            Assert.Collection(
                Read(new byte[] { 1, 2, 3 })
                    .List("", l, 3, 1, UInt8Serdes, _ => throw new InvalidOperationException()),
                x => Assert.Equal(5, x),
                x => Assert.Equal(1, x),
                x => Assert.Equal(2, x),
                x => Assert.Equal(3, x)
            );

            var a = new byte[4];
            Assert.Collection(
                Read(new byte[] { 1, 2, 3 })
                    .List("", a, 3, 1, UInt8Serdes, _ => throw new InvalidOperationException()),
                x => Assert.Equal(0, x),
                x => Assert.Equal(1, x),
                x => Assert.Equal(2, x),
                x => Assert.Equal(3, x)
            );
        }

        [Fact]
        public void RepeatTest()
        {
            Read(new byte[] { 0, 0, 0, 0 }).Pad("", 4);
            Read(new byte[] { 1, 1, 1, 1 }).Pad("", 4, 1);
            Read(new byte[] { 1, 1, 1, 1 }).Pad("", 1, 1);
            Assert.Throws<InvalidOperationException>(() =>
                Read(new byte[] { 0, 0, 1, 0 })
                 .Pad("", 4));
            Assert.Throws<EndOfStreamException>(() =>
                Read(new byte[] { 0, 0, 0, 0 })
                 .Pad("", 5));
        }

        [Fact]
        public void ByteArrayTest()
        {
            Assert.Collection(Read(new byte[] { 0, 1, 2, 3 }).Bytes("", null, 4),
                x => Assert.Equal(0, x),
                x => Assert.Equal(1, x),
                x => Assert.Equal(2, x),
                x => Assert.Equal(3, x)
            );
            Assert.Throws<EndOfStreamException>(() =>
                Read(new byte[] { 0, 1, 2, 3 })
                 .Bytes("", null, 5));

            Assert.Collection(Read(new byte[] { 0, 1, 2, 3 }).Bytes("", null, 4),
                x => Assert.Equal(0, x),
                x => Assert.Equal(1, x),
                x => Assert.Equal(2, x),
                x => Assert.Equal(3, x)
            );
            Assert.Throws<EndOfStreamException>(() =>
                Read(new byte[] { 0, 1, 2, 3 })
                 .Bytes("", null, 5));
        }

        [Fact]
        public void OffsetTests()
        {
            Assert.Equal(SerializerFlags.Read, Read(new byte[] { 0 }).Flags);
            var buf = new byte[] {0, 1, 2, 3, 4, 5, 6, 7};
            var s = Read(buf);
            Assert.Equal(buf.Length, s.BytesRemaining);
            Assert.Equal(0, s.Offset);
            s.Comment("x"); Assert.Equal(0, s.Offset);
            s.NewLine(); Assert.Equal(0, s.Offset);

            Assert.Equal(0, s.UInt8("", 0));
            Assert.Equal(1, s.Offset);
            Assert.Equal(0x201, s.UInt16("", 0));
            Assert.Equal(3, s.Offset);
            Assert.Equal(0x6050403u, s.UInt32("", 0));
            Assert.Equal(7, s.Offset);

            s.Seek(0);
            Assert.Equal(0, s.Offset);
            Assert.Equal(0x706050403020100uL, s.UInt64("", 0));
            Assert.Equal(8, s.Offset);
            Assert.Equal(0, s.BytesRemaining);

            s.Seek(0);
            Assert.NotEqual(0, s.BytesRemaining);
        }

        [Fact]
        public void EnumTests()
        {
            Assert.Collection(
                Read(new byte[] { 0, 1, 2, 3, 0xff })
                    .List<ByteEnum>("", null, 5, S.EnumU8),
                x => Assert.Equal(ByteEnum.None, x),
                x => Assert.Equal(ByteEnum.Some, x),
                x => Assert.Equal(ByteEnum.Both, x),
                x => Assert.Equal(ByteEnum.Many, x),
                x => Assert.Equal(ByteEnum.All, x)
            );

            Assert.Collection(
                Read(new byte[] {  0, 0, 1, 0, 2, 0, 3, 0, 0xff, 0xff })
                    .List<UShortEnum>("", null, 5, S.EnumU16),
                x => Assert.Equal(UShortEnum.None, x),
                x => Assert.Equal(UShortEnum.Some, x),
                x => Assert.Equal(UShortEnum.Both, x),
                x => Assert.Equal(UShortEnum.Many, x),
                x => Assert.Equal(UShortEnum.All, x)
            );

            Assert.Collection(
                Read(new byte[]
                    {
                        0,0,0,0,
                        1,0,0,0, 
                        2,0,0,0, 
                        3,0,0,0, 
                        0xff, 0xff, 0xff, 0xff,
                    }).List<UIntEnum>("", null, 5, S.EnumU32),
                x => Assert.Equal(UIntEnum.None, x),
                x => Assert.Equal(UIntEnum.Some, x),
                x => Assert.Equal(UIntEnum.Both, x),
                x => Assert.Equal(UIntEnum.Many, x),
                x => Assert.Equal(UIntEnum.All, x)
            );
        }

        [Fact]
        public void TransformEnumTests()
        {
            var s = Read(new byte[] { 0, 1, 2, 3, 0xff });
            Assert.Collection(
                s.List<ByteEnum?>("", null, 5,
                    (_, v, s2) => s2.TransformEnumU8(
                        "", v,
                        ZeroToNullConverter<ByteEnum>.Instance)),
                x => Assert.Null(x),
                x => Assert.Equal(ByteEnum.None, x),
                x => Assert.Equal(ByteEnum.Some, x),
                x => Assert.Equal(ByteEnum.Both, x),
                x => Assert.Equal(ByteEnum.AlmostAll, x)
            );

            s.Seek(0);
            Assert.Collection(
                s.List<ByteEnum?>("", null, 5,
                    (_, v, s2) => s2.TransformEnumU8(
                        "", v,
                        MaxToNullConverter<ByteEnum>.Instance)),
                x => Assert.Equal(ByteEnum.None, x),
                x => Assert.Equal(ByteEnum.Some, x),
                x => Assert.Equal(ByteEnum.Both, x),
                x => Assert.Equal(ByteEnum.Many, x),
                x => Assert.Null(x)
            );

            s = Read(new byte[] {0, 0, 1, 0, 2, 0, 3, 0, 0xff, 0xff});
            Assert.Collection(
                s.List<UShortEnum?>("", null, 5,
                    (_, v, s2) => s2.TransformEnumU16(
                        "", v,
                        ZeroToNullConverter<UShortEnum>.Instance)),
                x => Assert.Null(x),
                x => Assert.Equal(UShortEnum.None, x),
                x => Assert.Equal(UShortEnum.Some, x),
                x => Assert.Equal(UShortEnum.Both, x),
                x => Assert.Equal(UShortEnum.AlmostAll, x)
            );

            s.Seek(0);
            Assert.Collection(
                s.List<UShortEnum?>("", null, 5,
                    (_, v, s2) => s2.TransformEnumU16(
                        "", v,
                        MaxToNullConverter<UShortEnum>.Instance)),
                x => Assert.Equal(UShortEnum.None, x),
                x => Assert.Equal(UShortEnum.Some, x),
                x => Assert.Equal(UShortEnum.Both, x),
                x => Assert.Equal(UShortEnum.Many, x),
                x => Assert.Null(x)
            );

            s = Read(new byte[]
            {
                0, 0, 0, 0,
                1, 0, 0, 0,
                2, 0, 0, 0,
                3, 0, 0, 0,
                0xff, 0xff, 0xff, 0xff,
            });
            Assert.Collection(
                s.List<UIntEnum?>("", null, 5,
                    (_, v, s2) => s2.TransformEnumU32(
                        "", v,
                        ZeroToNullConverter<UIntEnum>.Instance)),
                x => Assert.Null(x),
                x => Assert.Equal(UIntEnum.None, x),
                x => Assert.Equal(UIntEnum.Some, x),
                x => Assert.Equal(UIntEnum.Both, x),
                x => Assert.Equal(UIntEnum.AlmostAll, x)
            );

            s.Seek(0);
            Assert.Collection(
                s.List<UIntEnum?>("", null, 5,
                    (_, v, s2) => s2.TransformEnumU32(
                        "", v,
                        MaxToNullConverter<UIntEnum>.Instance)),
                x => Assert.Equal(UIntEnum.None, x),
                x => Assert.Equal(UIntEnum.Some, x),
                x => Assert.Equal(UIntEnum.Both, x),
                x => Assert.Equal(UIntEnum.Many, x),
                x => Assert.Null(x)
            );
        }

        [Fact]
        public void TransformTests()
        {
            var s = Read(new byte[] { 0, 1, 2, 255, 254 });
            Assert.Null(s.Transform<byte, byte?>(0, 0, S.UInt8, ZeroToNullConverter.Instance));
            Assert.Equal((byte?)0, s.Transform<byte, byte?>(0, 0, S.UInt8, ZeroToNullConverter.Instance));
            Assert.Equal((byte?)1, s.Transform<byte, byte?>(0, 0, S.UInt8, ZeroToNullConverter.Instance));
            Assert.Equal((byte?)byte.MaxValue-1, s.Transform<byte, byte?>(0, 0, S.UInt8, ZeroToNullConverter.Instance));
            Assert.Equal((byte?)byte.MaxValue-2, s.Transform<byte, byte?>(0, 0, S.UInt8, ZeroToNullConverter.Instance));

            s.Seek(0);
            Assert.Equal((byte?)0, s.Transform<byte, byte?>(0, 0, S.UInt8, MaxToNullConverter.Instance));
            Assert.Equal((byte?)1, s.Transform<byte, byte?>(0, 0, S.UInt8, MaxToNullConverter.Instance));
            Assert.Equal((byte?)2, s.Transform<byte, byte?>(0, 0, S.UInt8, MaxToNullConverter.Instance));
            Assert.Null(s.Transform<byte, byte?>(0, 0, S.UInt8, MaxToNullConverter.Instance));
            Assert.Equal((byte?)byte.MaxValue-1, s.Transform<byte, byte?>(0, 0, S.UInt8, MaxToNullConverter.Instance));

            s = Read(new byte[] { 0, 0, 1, 0, 2, 0, 255, 255, 254, 255 });
            Assert.Null(s.Transform<ushort, ushort?>(0, 0, S.UInt16, ZeroToNullConverter.Instance));
            Assert.Equal((ushort?)0, s.Transform<ushort, ushort?>(0, 0, S.UInt16, ZeroToNullConverter.Instance));
            Assert.Equal((ushort?)1, s.Transform<ushort, ushort?>(0, 0, S.UInt16, ZeroToNullConverter.Instance));
            Assert.Equal((ushort?)ushort.MaxValue-1, s.Transform<ushort, ushort?>(0, 0, S.UInt16, ZeroToNullConverter.Instance));
            Assert.Equal((ushort?)ushort.MaxValue-2, s.Transform<ushort, ushort?>(0, 0, S.UInt16, ZeroToNullConverter.Instance));

            s.Seek(0);
            Assert.Equal((ushort?)0, s.Transform<ushort, ushort?>(0, 0, S.UInt16, MaxToNullConverter.Instance));
            Assert.Equal((ushort?)1, s.Transform<ushort, ushort?>(0, 0, S.UInt16, MaxToNullConverter.Instance));
            Assert.Equal((ushort?)2, s.Transform<ushort, ushort?>(0, 0, S.UInt16, MaxToNullConverter.Instance));
            Assert.Null(s.Transform<ushort, ushort?>(0, 0, S.UInt16, MaxToNullConverter.Instance));
            Assert.Equal((ushort?)ushort.MaxValue-1, s.Transform<ushort, ushort?>(0, 0, S.UInt16, MaxToNullConverter.Instance));

            s = Read(new byte[]
            {
                0,0,0,0,
                1,0,0,0,
                2,0,0,0,
                255,255,255,255,
                254,255,255,255
            });
            Assert.Null(s.Transform<uint, uint?>(0, 0, S.UInt32, ZeroToNullConverter.Instance));
            Assert.Equal((uint?)0, s.Transform<uint, uint?>(0, 0, S.UInt32, ZeroToNullConverter.Instance));
            Assert.Equal((uint?)1, s.Transform<uint, uint?>(0, 0, S.UInt32, ZeroToNullConverter.Instance));
            Assert.Equal((uint?)uint.MaxValue-1, s.Transform<uint, uint?>(0, 0, S.UInt32, ZeroToNullConverter.Instance));
            Assert.Equal((uint?)uint.MaxValue-2, s.Transform<uint, uint?>(0, 0, S.UInt32, ZeroToNullConverter.Instance));

            s.Seek(0);
            Assert.Equal((uint?)0, s.Transform<uint, uint?>(0, 0, S.UInt32, MaxToNullConverter.Instance));
            Assert.Equal((uint?)1, s.Transform<uint, uint?>(0, 0, S.UInt32, MaxToNullConverter.Instance));
            Assert.Equal((uint?)2, s.Transform<uint, uint?>(0, 0, S.UInt32, MaxToNullConverter.Instance));
            Assert.Null(s.Transform<uint, uint?>(0, 0, S.UInt32, MaxToNullConverter.Instance));
            Assert.Equal((uint?)uint.MaxValue-1, s.Transform<uint, uint?>(0, 0, S.UInt32, MaxToNullConverter.Instance));
        }

        [Fact]
        public void ObjectTests()
        {
            var ex = Read(Example.ExampleBuffer).Object<Example>("", null, Example.Serdes);
            ex.Verify(m => throw new InvalidOperationException(m));
        }
    }
}
