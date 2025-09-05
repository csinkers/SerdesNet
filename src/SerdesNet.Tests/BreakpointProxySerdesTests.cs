using System;
using System.Linq;
using Moq;
using Xunit;

namespace SerdesNet.Tests
{
    public class BreakpointProxySerdesTests
    {
        static byte[] SeqBytes => Enumerable.Range(0, 16).Select(x => (byte)x).ToArray();

        static ISerdes Reader(byte[] bytes) => new ReaderSerdes(bytes);

        [Fact]
        public void Constructor_InitializesCorrectly()
        {
            var inner = new ReaderSerdes(SeqBytes);
            var outer = new BreakpointProxySerdes(inner);
            Assert.NotNull(outer);
        }

        [Fact]
        public void Flags_ReturnsUnderlyingSerdesFlags()
        {
            var inner = new ReaderSerdes(SeqBytes);
            var outer = new BreakpointProxySerdes(inner);
            Assert.Equal(SerializerFlags.Read, outer.Flags);
        }

        [Fact]
        public void Offset_ReturnsCorrectOffset()
        {
            var inner = new ReaderSerdes(SeqBytes);
            var outer = new BreakpointProxySerdes(inner);

            Assert.Equal(0, outer.Offset);
            outer.UInt8(0, 0);
            Assert.Equal(1, outer.Offset);
        }

        [Fact]
        public void BytesRemaining_ReturnsCorrectBytesRemaining()
        {
            var inner = new ReaderSerdes(SeqBytes);
            var outer = new BreakpointProxySerdes(inner);
            Assert.Equal(16, outer.BytesRemaining);
            outer.UInt8(0, 0);
            Assert.Equal(15, outer.BytesRemaining);
        }

        [Fact]
        public void Comment_CallsUnderlyingSerdesComment()
        {
            var inner = new Mock<ISerdes>();
            var outer = new BreakpointProxySerdes(inner.Object);
            outer.Comment("test", false);
            inner.Verify(x => x.Comment("test", false), Times.Once);
        }

        [Fact]
        public void Begin_CallsUnderlyingSerdesBegin()
        {
            var inner = new Mock<ISerdes>();
            var outer = new BreakpointProxySerdes(inner.Object);
            outer.Begin("test");
        }

        [Fact]
        public void End_CallsUnderlyingSerdesEnd()
        {
            var inner = new Mock<ISerdes>();
            var outer = new BreakpointProxySerdes(inner.Object);
            outer.End();
            inner.Verify(x => x.End(), Times.Once);
        }

        [Fact]
        public void NewLine_CallsUnderlyingSerdesNewLine()
        {
            var inner = new Mock<ISerdes>();
            var outer = new BreakpointProxySerdes(inner.Object);
            outer.NewLine();
            inner.Verify(x => x.NewLine(), Times.Once);
        }

        [Fact]
        public void Seek_CallsUnderlyingSerdesSeek()
        {
            var inner = new Mock<ISerdes>();
            var outer = new BreakpointProxySerdes(inner.Object);
            outer.Seek(5);
            inner.Verify(x => x.Seek(5), Times.Once);
        }

        [Fact]
        public void Assert_CallsUnderlyingSerdesAssert()
        {
            var inner = new Mock<ISerdes>();
            var outer = new BreakpointProxySerdes(inner.Object);
            outer.Assert(true, "test");
            inner.Verify(x => x.Assert(true, "test"), Times.Once);
        }

        [Fact]
        public void Pad_CallsUnderlyingSerdesPad()
        {
            var inner = new Mock<ISerdes>();
            var outer = new BreakpointProxySerdes(inner.Object);
            outer.Pad(5);
            inner.Verify(x => x.Pad(5, 0), Times.Once);
        }

        [Fact]
        public void Int8_CallsUnderlyingSerdesInt8()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.Int8(It.IsAny<SerdesName>(), It.IsAny<sbyte>()))
                .Returns((SerdesName _, sbyte v) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(1, outer.Int8(0, 1));
            inner.Verify(x => x.Int8(0, 1), Times.Once);
        }

        [Fact]
        public void Int16_CallsUnderlyingSerdesInt16()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.Int16(It.IsAny<SerdesName>(), It.IsAny<short>()))
                .Returns((SerdesName _, short v) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(1, outer.Int16(0, 1));
            inner.Verify(x => x.Int16(0, 1), Times.Once);
        }

        [Fact]
        public void Int32_CallsUnderlyingSerdesInt32()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.Int32(It.IsAny<SerdesName>(), It.IsAny<int>()))
                .Returns((SerdesName _, int v) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(1, outer.Int32(0, 1));
            inner.Verify(x => x.Int32(0, 1), Times.Once);
        }

        [Fact]
        public void Int64_CallsUnderlyingSerdesInt64()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.Int64(It.IsAny<SerdesName>(), It.IsAny<long>()))
                .Returns((SerdesName _, long v) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(1, outer.Int64(0, 1));
            inner.Verify(x => x.Int64(0, 1), Times.Once);
        }

        [Fact]
        public void UInt8_CallsUnderlyingSerdesUInt8()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.UInt8(It.IsAny<SerdesName>(), It.IsAny<byte>()))
                .Returns((SerdesName _, byte v) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(1, outer.UInt8(0, 1));
            inner.Verify(x => x.UInt8(0, 1), Times.Once);
        }

        [Fact]
        public void UInt16_CallsUnderlyingSerdesUInt16()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.UInt16(It.IsAny<SerdesName>(), It.IsAny<ushort>()))
                .Returns((SerdesName _, ushort v) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(1, outer.UInt16(0, 1));
            inner.Verify(x => x.UInt16(0, 1), Times.Once);
        }

        [Fact]
        public void UInt32_CallsUnderlyingSerdesUInt32()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.UInt32(It.IsAny<SerdesName>(), It.IsAny<uint>()))
                .Returns((SerdesName _, uint v) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(1u, outer.UInt32(0, 1));
            inner.Verify(x => x.UInt32(0, 1), Times.Once);
        }

        [Fact]
        public void UInt64_CallsUnderlyingSerdesUInt64()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.UInt64(It.IsAny<SerdesName>(), It.IsAny<ulong>()))
                .Returns((SerdesName _, ulong v) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(1u, outer.UInt64(0, 1));
            inner.Verify(x => x.UInt64(0, 1), Times.Once);
        }

        [Fact]
        public void EnumU8_CallsUnderlyingSerdesU8()
        {
            var inner = new Mock<ISerdes>();
            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(TestEnumU8.Value1, outer.EnumU8(0, TestEnumU8.Value1));
            inner.Verify(x => x.UInt8(0, (byte)TestEnumU8.Value1), Times.Once);
        }

        [Fact]
        public void EnumU16_CallsUnderlyingSerdesU16()
        {
            var inner = new Mock<ISerdes>();
            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(TestEnumU16.Value1, outer.EnumU16(0, TestEnumU16.Value1));
            inner.Verify(x => x.UInt16(0, (ushort)TestEnumU16.Value1), Times.Once);
        }

        [Fact]
        public void EnumU32_CallsUnderlyingSerdesU32()
        {
            var inner = new Mock<ISerdes>();
            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(TestEnumU32.Value1, outer.EnumU32(0, TestEnumU32.Value1));
            inner.Verify(x => x.UInt32(0, (uint)TestEnumU32.Value1), Times.Once);
        }

        [Fact]
        public void Guid_CallsUnderlyingSerdesGuid()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.Guid(It.IsAny<SerdesName>(), It.IsAny<Guid>()))
                .Returns((SerdesName _, Guid v) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            var guid = Guid.NewGuid();
            Assert.Equal(guid, outer.Guid("test", guid));
            inner.Verify(x => x.Guid("test", guid), Times.Once);
        }

        [Fact]
        public void Bytes_CallsUnderlyingSerdesBytes()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.Bytes(It.IsAny<SerdesName>(), It.IsAny<byte[]>(), It.IsAny<int>()))
                .Returns((SerdesName _, byte[] v, int _) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            var bytes = new byte[] { 1, 2, 3 };
            Assert.Equal(bytes, outer.Bytes((SerdesName)"test", bytes, 3));
            inner.Verify(x => x.Bytes((SerdesName)"test", bytes, 3), Times.Once);
        }

        [Fact]
        public void HitEvent_FiresWhenBreakRangeIsAccessed()
        {
            var inner = Reader(SeqBytes);
            var outer = new BreakpointProxySerdes(inner) { BreakRange = (2, 4) };
            int hitCount = 0;
            outer.Hit += (_, _) => hitCount++;

            outer.Int8(0, 1); Assert.Equal(0, hitCount); // 0
            outer.Int8(0, 1); Assert.Equal(0, hitCount); // 1
            outer.Int8(0, 1); Assert.Equal(1, hitCount); // 2
            outer.Int8(0, 1); Assert.Equal(2, hitCount); // 3
            outer.Int8(0, 1); Assert.Equal(3, hitCount); // 4
            outer.Int8(0, 1); Assert.Equal(3, hitCount); // 5
        }

        [Fact]
        public void HitEvent_FiresWhenBreakRangeIsAccessed2()
        {
            var inner = Reader(SeqBytes);
            var outer = new BreakpointProxySerdes(inner) { BreakRange = (2, 4) };
            int hitCount = 0;
            outer.Hit += (_, _) => hitCount++;

            outer.Pad(1); Assert.Equal(0, hitCount); // 0
            outer.Pad(1); Assert.Equal(0, hitCount); // 1
            outer.Pad(1); Assert.Equal(1, hitCount); // 2
            outer.Pad(1); Assert.Equal(2, hitCount); // 3
            outer.Pad(1); Assert.Equal(3, hitCount); // 4
            outer.Pad(1); Assert.Equal(3, hitCount); // 5
        }

        enum TestEnumU8 : byte { Value1 }
        enum TestEnumU16 : ushort { Value1 }
        enum TestEnumU32 : uint { Value1 }
    }
}
