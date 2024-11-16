using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Xunit;

namespace SerdesNet.Tests
{
    public class BreakpointProxySerdesTests
    {
        static byte[] SeqBytes => Enumerable.Range(0, 16).Select(x => (byte)x).ToArray();

        static ISerdes Reader(byte[] bytes) => new ReaderSerdes(bytes, Encoding.Latin1.GetString);

        [Fact]
        public void Constructor_InitializesCorrectly()
        {
            var inner = new ReaderSerdes(SeqBytes, Encoding.Latin1.GetString);
            var outer = new BreakpointProxySerdes(inner);
            Assert.NotNull(outer);
        }

        [Fact]
        public void Flags_ReturnsUnderlyingSerdesFlags()
        {
            var inner = new ReaderSerdes(SeqBytes, Encoding.Latin1.GetString);
            var outer = new BreakpointProxySerdes(inner);
            Assert.Equal(SerializerFlags.Read, outer.Flags);
        }

        [Fact]
        public void Offset_ReturnsCorrectOffset()
        {
            var inner = new ReaderSerdes(SeqBytes, Encoding.Latin1.GetString);
            var outer = new BreakpointProxySerdes(inner);

            Assert.Equal(0, outer.Offset);
            outer.UInt8(0, 0);
            Assert.Equal(1, outer.Offset);
        }

        [Fact]
        public void BytesRemaining_ReturnsCorrectBytesRemaining()
        {
            var inner = new ReaderSerdes(SeqBytes, Encoding.Latin1.GetString);
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
            inner.Setup(x => x.Int8(It.IsAny<int>(), It.IsAny<sbyte>()))
                .Returns((int _, sbyte v) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(1, outer.Int8(0, 1));
            inner.Verify(x => x.Int8(0, 1), Times.Once);
        }

        [Fact]
        public void Int16_CallsUnderlyingSerdesInt16()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.Int16(It.IsAny<int>(), It.IsAny<short>()))
                .Returns((int _, short v) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(1, outer.Int16(0, 1));
            inner.Verify(x => x.Int16(0, 1), Times.Once);
        }

        [Fact]
        public void Int32_CallsUnderlyingSerdesInt32()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.Int32(It.IsAny<int>(), It.IsAny<int>()))
                .Returns((int _, int v) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(1, outer.Int32(0, 1));
            inner.Verify(x => x.Int32(0, 1), Times.Once);
        }

        [Fact]
        public void Int64_CallsUnderlyingSerdesInt64()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.Int64(It.IsAny<int>(), It.IsAny<long>()))
                .Returns((int _, long v) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(1, outer.Int64(0, 1));
            inner.Verify(x => x.Int64(0, 1), Times.Once);
        }

        [Fact]
        public void UInt8_CallsUnderlyingSerdesUInt8()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.UInt8(It.IsAny<int>(), It.IsAny<byte>()))
                .Returns((int _, byte v) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(1, outer.UInt8(0, 1));
            inner.Verify(x => x.UInt8(0, 1), Times.Once);
        }

        [Fact]
        public void UInt16_CallsUnderlyingSerdesUInt16()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.UInt16(It.IsAny<int>(), It.IsAny<ushort>()))
                .Returns((int _, ushort v) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(1, outer.UInt16(0, 1));
            inner.Verify(x => x.UInt16(0, 1), Times.Once);
        }

        [Fact]
        public void UInt32_CallsUnderlyingSerdesUInt32()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.UInt32(It.IsAny<int>(), It.IsAny<uint>()))
                .Returns((int _, uint v) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(1u, outer.UInt32(0, 1));
            inner.Verify(x => x.UInt32(0, 1), Times.Once);
        }

        [Fact]
        public void UInt64_CallsUnderlyingSerdesUInt64()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.UInt64(It.IsAny<int>(), It.IsAny<ulong>()))
                .Returns((int _, ulong v) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(1u, outer.UInt64(0, 1));
            inner.Verify(x => x.UInt64(0, 1), Times.Once);
        }

        [Fact]
        public void EnumU8_CallsUnderlyingSerdesEnumU8()
        {
            var inner = new Mock<ISerdes>();
            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(TestEnum.Value1, outer.EnumU8(0, TestEnum.Value1));
            inner.Verify(x => x.EnumU8(0, TestEnum.Value1), Times.Once);
        }

        [Fact]
        public void EnumU16_CallsUnderlyingSerdesEnumU16()
        {
            var inner = new Mock<ISerdes>();
            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(TestEnum.Value1, outer.EnumU16(0, TestEnum.Value1));
            inner.Verify(x => x.EnumU16(0, TestEnum.Value1), Times.Once);
        }

        [Fact]
        public void EnumU32_CallsUnderlyingSerdesEnumU32()
        {
            var inner = new Mock<ISerdes>();
            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal(TestEnum.Value1, outer.EnumU32(0, TestEnum.Value1));
            inner.Verify(x => x.EnumU32(0, TestEnum.Value1), Times.Once);
        }

        [Fact]
        public void Guid_CallsUnderlyingSerdesGuid()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.Guid(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns((string _, Guid v) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            var guid = Guid.NewGuid();
            Assert.Equal(guid, outer.Guid("test", guid));
            inner.Verify(x => x.Guid("test", guid), Times.Once);
        }

        [Fact]
        public void Bytes_CallsUnderlyingSerdesBytes()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.Bytes(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<int>()))
                .Returns((string _, byte[] v, int _) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            var bytes = new byte[] { 1, 2, 3 };
            Assert.Equal(bytes, outer.Bytes("test", bytes, 3));
            inner.Verify(x => x.Bytes("test", bytes, 3), Times.Once);
        }

        [Fact]
        public void NullTerminatedString_CallsUnderlyingSerdesNullTerminatedString()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.NullTerminatedString(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string _, string v) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal("test", outer.NullTerminatedString("test", "test"));
            inner.Verify(x => x.NullTerminatedString("test", "test"), Times.Once);
        }

        [Fact]
        public void FixedLengthString_CallsUnderlyingSerdesFixedLengthString()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.FixedLengthString(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns((string _, string v, int _) => v);

            var outer = new BreakpointProxySerdes(inner.Object);
            Assert.Equal("test", outer.FixedLengthString("test", "test", 4));
            inner.Verify(x => x.FixedLengthString("test", "test", 4), Times.Once);
        }

        [Fact]
        public void List_CallsUnderlyingSerdesList()
        {
            var inner = new Mock<ISerdes>();
            inner.Setup(x => x.List(
                    It.IsAny<string>(),
                    It.IsAny<IList<int>>(),
                    It.IsAny<int>(),
                    It.IsAny<SerdesMethod<int>>(),
                    It.IsAny<Func<int, IList<int>>>())
                )
                .Returns((string _, IList<int> list, int _, SerdesMethod<int> _, Func<int, IList<int>> _) => list);

            var outer = new BreakpointProxySerdes(inner.Object);
            var list = new List<int> { 1, 2, 3 };
            Assert.Equal(list, outer.List("test", list, 3, (_, v, _) => v));
            inner.Verify(x => x.List("test", list, 3, It.IsAny<SerdesMethod<int>>(), null), Times.Once);
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

        enum TestEnum
        {
            Value1
        }
    }
}
