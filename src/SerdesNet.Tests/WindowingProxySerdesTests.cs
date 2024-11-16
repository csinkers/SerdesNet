using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Xunit;

namespace SerdesNet.Tests;

public class WindowingProxySerdesTests
{
    static byte[] SeqBytes => Enumerable.Range(0, 16).Select(x => (byte)x).ToArray();
    static byte[] SeqShorts => new byte[] { 0, 0, 1, 0, 2, 0, 3, 0, 4, 0, 5, 0, 6, 0, 7, 0 };
    static byte[] SeqInts => new byte[] { 0, 0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0, 3, 0, 0, 0 };
    static byte[] SeqLongs => new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 };

    static ISerdes Reader(byte[] bytes) => new ReaderSerdes(bytes, Encoding.Latin1.GetString);

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenSerializerIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new WindowingProxySerdes(null, 0));
    }

    [Fact]
    public void Constructor_ThrowsArgumentOutOfRangeException_WhenSizeIsGreaterThanBytesRemaining()
    {
        var inner = Reader(SeqBytes);
        Assert.Throws<ArgumentOutOfRangeException>(() => new WindowingProxySerdes(inner, 32));
    }

    [Fact]
    public void Flags_ReturnsUnderlyingSerializerFlags()
    {
        var inner = Reader(SeqBytes);
        var outer = new WindowingProxySerdes(inner, 8);
        Assert.Equal(SerializerFlags.Read, outer.Flags);
    }

    [Fact]
    public void Offset_ReturnsCorrectOffset()
    {
        var inner = Reader(SeqShorts);
        Assert.Equal(0, inner.UInt16(0, 0));
        Assert.Equal(1, inner.UInt16(0, 0));

        var outer = new WindowingProxySerdes(inner, 2);
        Assert.Equal(0, outer.Offset);
        Assert.Equal(4, inner.Offset);
        Assert.Equal(2, outer.UInt16(0, 0));
    }

    [Fact]
    public void BytesRemaining_ReturnsCorrectBytesRemaining()
    {
        var inner = Reader(SeqShorts);
        Assert.Equal(0, inner.UInt16(0, 0));
        Assert.Equal(1, inner.UInt16(0, 0));
        Assert.Equal(12, inner.BytesRemaining);

        var outer = new WindowingProxySerdes(inner, 4);
        Assert.Equal(4, outer.BytesRemaining);
        Assert.Equal(2, outer.UInt16(0, 0));
        Assert.Equal(2, outer.BytesRemaining);
    }

    [Fact]
    public void Comment_CallsUnderlyingSerializerComment()
    {
        string comment = null;
        bool? wasInline = null;

        var inner = new Mock<ISerdes>();
        inner.Setup(x => x.Offset).Returns(0);
        inner.Setup(x => x.BytesRemaining).Returns(16);
        inner.Setup(x => x.Comment(It.IsAny<string>(), It.IsAny<bool>())).Callback((string c, bool i) =>
        {
            comment = c;
            wasInline = i;
        });

        var outer = new WindowingProxySerdes(inner.Object, 8);
        outer.Comment("test", false);
        Assert.Equal("test", comment);
        Assert.Equal(false, wasInline);

        outer.Comment("test2", true);
        Assert.Equal("test2", comment);
        Assert.Equal(true, wasInline);
    }

    [Fact]
    public void Begin_CallsUnderlyingSerializerBegin()
    {
        var inner = new Mock<ISerdes>();
        inner.Setup(x => x.Offset).Returns(0);
        inner.Setup(x => x.BytesRemaining).Returns(16);

        var outer = new WindowingProxySerdes(inner.Object, 8);
        outer.Begin("test");
        inner.Verify(x => x.Begin("test"), Times.Once);
    }

    [Fact]
    public void End_CallsUnderlyingSerializerEnd()
    {
        var inner = new Mock<ISerdes>();
        inner.Setup(x => x.Offset).Returns(0);
        inner.Setup(x => x.BytesRemaining).Returns(16);

        var outer = new WindowingProxySerdes(inner.Object, 8);
        outer.End();
        inner.Verify(x => x.End(), Times.Once);
    }

    [Fact]
    public void NewLine_CallsUnderlyingSerializerNewLine()
    {
        var inner = new Mock<ISerdes>();
        inner.Setup(x => x.Offset).Returns(0);
        inner.Setup(x => x.BytesRemaining).Returns(16);

        var outer = new WindowingProxySerdes(inner.Object, 8);
        outer.NewLine();
        inner.Verify(x => x.NewLine(), Times.Once);
    }

    [Fact]
    public void Seek_CallsUnderlyingSerializerSeek()
    {
        var inner = new Mock<ISerdes>();
        inner.Setup(x => x.Offset).Returns(4);
        inner.Setup(x => x.BytesRemaining).Returns(12);

        var outer = new WindowingProxySerdes(inner.Object, 8);
        outer.Seek(5);
        inner.Verify(x => x.Seek(9), Times.Once);
    }

    [Fact]
    public void Assert_CallsUnderlyingSerializerAssert()
    {
        var inner = new Mock<ISerdes>();
        inner.Setup(x => x.Offset).Returns(0);
        inner.Setup(x => x.BytesRemaining).Returns(16);

        var outer = new WindowingProxySerdes(inner.Object, 8);
        outer.Assert(true, "test");
        inner.Verify(x => x.Assert(true, "test"), Times.Once);

        outer.Assert(false, "test2");
        inner.Verify(x => x.Assert(false, "test2"), Times.Once);
    }

    [Fact]
    public void Pad_CallsUnderlyingSerializerPad()
    {
        var inner = new Mock<ISerdes>();
        inner.Setup(x => x.Offset).Returns(0);
        inner.Setup(x => x.BytesRemaining).Returns(16);

        var outer = new WindowingProxySerdes(inner.Object, 8);
        outer.Pad(5, 0);
        inner.Verify(x => x.Pad(5, 0), Times.Once);

        outer.Pad(5, 1);
        inner.Verify(x => x.Pad(5, 1), Times.Once);
    }

    [Fact]
    public void Int8_CallsUnderlyingSerializerInt8()
    {
        var inner = Reader(SeqBytes);
        var outer = new WindowingProxySerdes(inner, 8);
        Assert.Equal(0, outer.Int8(0, 1));
        Assert.Equal(1, outer.Int8(0, 1));
    }

    [Fact]
    public void Int16_CallsUnderlyingSerializerInt16()
    {
        var inner = Reader(SeqShorts);
        var outer = new WindowingProxySerdes(inner, 8);
        Assert.Equal(0, outer.Int16(0, 1));
        Assert.Equal(1, outer.Int16(0, 1));
    }

    [Fact]
    public void Int32_CallsUnderlyingSerializerInt32()
    {
        var inner = Reader(SeqInts);
        var outer = new WindowingProxySerdes(inner, 8);
        Assert.Equal(0, outer.Int32(0, 1));
        Assert.Equal(1, outer.Int32(0, 1));
    }

    [Fact]
    public void Int64_CallsUnderlyingSerializerInt64()
    {
        var inner = Reader(SeqLongs);
        var outer = new WindowingProxySerdes(inner, 8);
        Assert.Equal(0, outer.Int64(0, 1));
        Assert.Equal(1, outer.Int64(0, 1));
    }

    [Fact]
    public void UInt8_CallsUnderlyingSerializerUInt8()
    {
        var inner = Reader(SeqBytes);
        var outer = new WindowingProxySerdes(inner, 8);
        Assert.Equal(0, outer.UInt8(0, 1));
        Assert.Equal(1, outer.UInt8(0, 1));
    }

    [Fact]
    public void UInt16_CallsUnderlyingSerializerUInt16()
    {
        var inner = Reader(SeqShorts);
        var outer = new WindowingProxySerdes(inner, 8);
        Assert.Equal(0, outer.UInt16(0, 1));
        Assert.Equal(1, outer.UInt16(0, 1));
    }

    [Fact]
    public void UInt32_CallsUnderlyingSerializerUInt32()
    {
        var inner = Reader(SeqInts);
        var outer = new WindowingProxySerdes(inner, 8);
        Assert.Equal(0u, outer.UInt32(0, 1));
        Assert.Equal(1u, outer.UInt32(0, 1));
    }

    [Fact]
    public void UInt64_CallsUnderlyingSerializerUInt64()
    {
        var inner = Reader(SeqLongs);
        var outer = new WindowingProxySerdes(inner, 8);
        Assert.Equal(0u, outer.UInt64(0, 1));
        Assert.Equal(1u, outer.UInt64(0, 1));
    }

    [Fact]
    public void EnumU8_CallsUnderlyingSerializerEnumU8()
    {
        var inner = Reader(SeqBytes);
        var outer = new WindowingProxySerdes(inner, 8);
        Assert.Equal(TestEnum.Value0, outer.EnumU8(0, TestEnum.Value1));
        Assert.Equal(TestEnum.Value1, outer.EnumU8(0, TestEnum.Value1));
        Assert.Equal(TestEnum.Value2, outer.EnumU8(0, TestEnum.Value1));
    }

    [Fact]
    public void EnumU16_CallsUnderlyingSerializerEnumU16()
    {
        var inner = Reader(SeqShorts);
        var outer = new WindowingProxySerdes(inner, 8);
        Assert.Equal(TestEnum.Value0, outer.EnumU16(0, TestEnum.Value1));
        Assert.Equal(TestEnum.Value1, outer.EnumU16(0, TestEnum.Value1));
        Assert.Equal(TestEnum.Value2, outer.EnumU16(0, TestEnum.Value1));
    }

    [Fact]
    public void EnumU32_CallsUnderlyingSerializerEnumU32()
    {
        var inner = Reader(SeqInts);
        var outer = new WindowingProxySerdes(inner, 8);
        Assert.Equal(TestEnum.Value0, outer.EnumU32(0, TestEnum.Value1));
        Assert.Equal(TestEnum.Value1, outer.EnumU32(0, TestEnum.Value1));
        Assert.Equal(TestEnum.Value2, outer.EnumU32(0, TestEnum.Value1));
    }

    [Fact]
    public void Guid_CallsUnderlyingSerializerGuid()
    {
        var inner = Reader(SeqBytes);
        var outer = new WindowingProxySerdes(inner, 16);
        var guid = Guid.Parse("03020100-0504-0706-0809-0a0b0c0d0e0f");
        Assert.Equal(guid, outer.Guid("test", guid));
    }

    [Fact]
    public void Bytes_CallsUnderlyingSerializerBytes()
    {
        var inner = Reader(SeqBytes);
        var outer = new WindowingProxySerdes(inner, 8);
        var bytes = new byte[] { 0, 1, 2, 3 };
        Assert.Equal(bytes, outer.Bytes("test", null, 4));
    }

    [Fact]
    public void NullTerminatedString_CallsUnderlyingSerializerNullTerminatedString()
    {
        var inner = Reader( Encoding.Latin1.GetBytes("test\0"));
        var outer = new WindowingProxySerdes(inner, 5);
        Assert.Equal("test", outer.NullTerminatedString("foo", "bar"));
    }

    [Fact]
    public void FixedLengthString_CallsUnderlyingSerializerFixedLengthString()
    {
        var inner = Reader(Encoding.Latin1.GetBytes("test string"));
        var outer = new WindowingProxySerdes(inner, (int)inner.BytesRemaining);
        Assert.Equal("test", outer.FixedLengthString("foo", "bar", 4));
    }

    [Fact]
    public void List_CallsUnderlyingSerializerList1()
    {
        var inner = Reader(SeqBytes);
        var outer = new WindowingProxySerdes(inner, 8);
        var list = new List<int>();
        outer.List("test", list, 4, (i, _, s) => s.UInt8(i, 0));
        Assert.Equal(new List<int> { 0, 1, 2, 3 }, list);
    }

    [Fact]
    public void List_CallsUnderlyingSerializerList2()
    {
        var inner = Reader(SeqBytes);
        var outer = new WindowingProxySerdes(inner, 8);
        var list = new List<int>();
        outer.List("test", list, 4, 4, (i, _, s) => s.UInt8(i, 0));
        Assert.Equal(new List<int> { 0, 0, 0, 0, 0, 1, 2, 3 }, list);
    }

    [Fact]
    public void List_CallsUnderlyingSerializerList3()
    {
        var inner = Reader(SeqBytes);
        var outer = new WindowingProxySerdes(inner, 8);
        var list = new List<int>();
        outer.ListWithContext("test", list, 1, 4, (i, _, ctx, s) => s.UInt8(i, 0) + ctx);
        Assert.Equal(new List<int> { 1, 2, 3, 4 }, list);
    }

    [Fact]
    public void List_CallsUnderlyingSerializerList4()
    {
        var inner = Reader(SeqBytes);
        var outer = new WindowingProxySerdes(inner, 8);
        var list = new List<int>() { 0, 0, 0, 0, 0, 0};
        outer.ListWithContext("test", list, 1, 4, 2, (i, _, ctx, s) => s.UInt8(i, 0) + ctx);
        Assert.Equal(new List<int> {0, 0, 1, 2, 3, 4}, list);
    }

    enum TestEnum
    {
        Value0,
        Value1,
        Value2
    }
}