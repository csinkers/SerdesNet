using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace SerdesNet.Tests;

public class WriterSerdesTests
{
    static byte[] Write(Action<ISerdes> action, Action<string> assertHandler = null)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        using var s = new WriterSerdes(
            bw,
            Encoding.UTF8.GetBytes,
            assertHandler ?? (m => throw new InvalidOperationException(m)));
        action(s);
        bw.Flush();
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
        {
            Write(s => s.FixedLengthString("", "Too long", 1));
        });
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
        static byte UInt8Serdes(int i, byte v, ISerdes s) => s.UInt8("", v);
        Assert.Equal(new byte[] { 1, 2, 3 },
            Write(s => s.List(
                "",
                new byte[] { 1, 2, 3 },
                3,
                UInt8Serdes,
                _ => throw new InvalidOperationException())));

        Assert.Equal(new byte[] { 1, 2, 3 },
            Write(s => s.List(
                "",
                new byte[] { 0, 1, 2, 3 },
                3,
                1,
                UInt8Serdes,
                _ => throw new InvalidOperationException())));
    }

    [Fact]
    public void ListTest2()
    {
        Assert.Equal(new byte[] { 2, 3, 4 },
            Write(s => s.ListWithContext(
                "",
                new byte[] { 1, 2, 3 },
                1,
                3,
                static (_, v, ctx, s) => s.UInt8("", (byte)(v + ctx)),
                _ => throw new InvalidOperationException())));

        Assert.Equal(new byte[] { 2, 3, 4 },
            Write(s => s.ListWithContext(
                "",
                new byte[] { 0, 1, 2, 3 },
                1,
                3,
                1,
                static (_, v, ctx, s) => s.UInt8("", (byte)(v + ctx)),
                _ => throw new InvalidOperationException())));
    }

    [Fact]
    public void RepeatTest()
    {
        Assert.Equal(new byte[] { 0, 0, 0, 0 }, Write(s => s.Pad(4)));
        Assert.Equal(new byte[] { 1, 1, 1, 1 }, Write(s => s.Pad(4, 1)));
        Assert.Equal(new byte[] { 1 }, Write(s => s.Pad(1, 1)));
    }

    [Fact]
    public void ByteArrayTest()
    {
        Assert.Equal(new byte[] { 0, 1, 2, 3 },
            Write(s => s.Bytes("", [0, 1, 2, 3], 4)));
    }

    [Fact]
    public void OffsetTests()
    {
        Write(x => Assert.Equal(SerializerFlags.Write, x.Flags));
        Write(x => Assert.Equal(int.MaxValue, x.BytesRemaining));
        Write(x => Assert.NotEqual(0, x.BytesRemaining));

        static void Block1(ISerdes s)
        {
            Assert.Equal(0, s.Offset);
            s.Comment("x"); Assert.Equal(0, s.Offset);
            s.NewLine(); Assert.Equal(0, s.Offset);

            s.UInt8("", 0); Assert.Equal(1, s.Offset);
            s.UInt16("", 0x201); Assert.Equal(3, s.Offset);
            s.UInt32("", 0x6050403u); Assert.Equal(7, s.Offset);
        }

        static void Block2(ISerdes s)
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
            Write(s => s.List("", [
                ByteEnum.None,
                ByteEnum.Some,
                ByteEnum.Both,
                ByteEnum.Many,
                ByteEnum.All
            ], 5, (n, v, s2) => s2.EnumU8(n, v))));

        Assert.Equal(new byte[] { 0, 0, 1, 0, 2, 0, 3, 0, 0xff, 0xff },
            Write(s => s.List("", new List<UShortEnum>
            {
                UShortEnum.None,
                UShortEnum.Some,
                UShortEnum.Both,
                UShortEnum.Many,
                UShortEnum.All,
            }, 5, (n, v, s2) => s2.EnumU16(n, v))));

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
            }, 5, (n, v, s2) => s2.EnumU32(n, v))));
    }

    [Fact]
    public void ObjectTests()
    {
        Assert.Equal(Example.ExampleBuffer,
            Write(s => s.Object("test", Example.TestInstance, Example.Serdes)));
    }

    [Fact]
    public void Assert_ShouldInvokeAssertionFailedCallback_WhenConditionIsFalse()
    {
        // Arrange
        var assertionFailedCalled = false;
        string assertionMessage = null;
        Action<string> assertionFailed = message =>
        {
            assertionFailedCalled = true;
            assertionMessage = message;
        };
        var writer = new BinaryWriter(new MemoryStream());
        var serdes = new WriterSerdes(writer, Encoding.UTF8.GetBytes, assertionFailed);

        // Act
        serdes.Assert(false, 0, x => "Test assertion" + x);

        // Assert
        Assert.True(assertionFailedCalled);
        Assert.Contains("Test assertion0", assertionMessage);
    }

    [Fact]
    public void Assert_ShouldNotInvokeAssertionFailedCallback_WhenConditionIsTrue()
    {
        // Arrange
        var assertionFailedCalled = false;
        string assertionMessage = null;
        Action<string> assertionFailed = message =>
        {
            assertionFailedCalled = true;
            assertionMessage = message;
        };
        var writer = new BinaryWriter(new MemoryStream());
        var serdes = new WriterSerdes(writer, Encoding.UTF8.GetBytes, assertionFailed);

        // Act
        serdes.Assert(true, 0, x => "Test assertion" + x);

        // Assert
        Assert.False(assertionFailedCalled);
        Assert.Null(assertionMessage);
    }
}