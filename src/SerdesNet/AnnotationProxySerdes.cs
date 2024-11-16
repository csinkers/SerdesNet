using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace SerdesNet;

/// <summary>
/// Delegates binary reading/writing to an underlying serializer while writing
/// an annotated record of the data to a text writer.
/// </summary>
public class AnnotationProxySerdes : ISerdes
{
    readonly TextWriter _tw;
    readonly Func<string, byte[]> _stringToBytes;
    readonly ISerdes _s;
    readonly Stack<long> _offsetStack = new Stack<long>();
    readonly bool _useRelativeOffsets;
    int _indent;

    public AnnotationProxySerdes(ISerdes s, TextWriter tw, Func<string, byte[]> stringToBytes, bool useRelativeOffsets = true)
    {
        _s = s ?? throw new ArgumentNullException(nameof(s));
        _tw = tw ?? throw new ArgumentNullException(nameof(tw));
        _stringToBytes = stringToBytes ?? throw new ArgumentNullException(nameof(stringToBytes));
        _offsetStack.Push(0);
        _useRelativeOffsets = useRelativeOffsets;
    }

    public void Dispose() => _s.Dispose();
    public SerializerFlags Flags => _s.Flags | SerializerFlags.Comments;
    public long Offset => _s.Offset;
    long LocalOffset => _useRelativeOffsets ? Offset - _offsetStack.Peek() : Offset;
    public long BytesRemaining => _s.BytesRemaining;

    public void Seek(long offset)
    {
        DoIndent();
        _tw.Write("{1:X} Seek to {0:X} for overwrite", offset, LocalOffset);
        _s.Seek(offset);
    }

    public void Assert(bool condition, string message) => _s.Assert(condition, message);
    public void Pad(int length, byte value) => _s.Pad(length, value); // Don't write anything to the annotation stream for unnamed padding
    public void Pad(string name, int length, byte value)
    {
        var offset = LocalOffset;
        _s.Pad(name, length, value);
        DoIndent();
        _tw.Write(
            "{0:X} {1} = [{2} bytes (0x{2:X}) of 0x{3:X}]",
            offset,
            name,
            length,
            value);
    }
    void DoIndent()
    {
        _tw.WriteLine();
        _tw.Write(new string(' ', _indent));
    }

    public void Comment(string msg, bool inline)
    {
        if (!inline)
        {
            DoIndent();
            _tw.Write("// {0}", msg);
        }
        else _tw.Write(" // {0}", msg);
    }

    public void Begin(string name)
    {
        DoIndent();
        _tw.Write($"{LocalOffset:X} {name}: {{");
        _indent += 4;
        _offsetStack.Push(Offset);
    }

    public void End()
    {
        _offsetStack.Pop();
        _indent -= 4;
        DoIndent();
        _tw.Write("}");
    }

    public void NewLine() => _tw.WriteLine();
    public sbyte Int8(int n, sbyte value)
    {
        var offset = LocalOffset;
        value = _s.Int8(n, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} y)", offset, n, value);
        return value;
    }

    public short Int16(int n, short value)
    {
        var offset = LocalOffset;
        value = _s.Int16(n, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} s)", offset, n, value);
        return value;
    }

    public int Int32(int n, int value)
    {
        var offset = LocalOffset;
        value = _s.Int32(n, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X})", offset, n, value);
        return value;
    }

    public long Int64(int n, long value)
    {
        var offset = LocalOffset;
        value = _s.Int64(n, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} L)", offset, n, value);
        return value;
    }

    public byte UInt8(int n, byte value)
    {
        var offset = LocalOffset;
        value = _s.UInt8(n, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} uy)", offset, n, value);
        return value;
    }

    public ushort UInt16(int n, ushort value)
    {
        var offset = LocalOffset;
        value = _s.UInt16(n, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} us)", offset, n, value);
        return value;
    }

    public uint UInt32(int n, uint value)
    {
        var offset = LocalOffset;
        value = _s.UInt32(n, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} u)", offset, n, value);
        return value;
    }

    public ulong UInt64(int n, ulong value)
    {
        var offset = LocalOffset;
        value = _s.UInt64(n, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} UL)", offset, n, value);
        return value;
    }

    public sbyte Int8(string name, sbyte value)
    {
        var offset = LocalOffset;
        value = _s.Int8(name, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} y)", offset, name, value);
        return value;
    }

    public short Int16(string name, short value)
    {
        var offset = LocalOffset;
        value = _s.Int16(name, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} s)", offset, name, value);
        return value;
    }

    public int Int32(string name, int value)
    {
        var offset = LocalOffset;
        value = _s.Int32(name, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X})", offset, name, value);
        return value;
    }

    public long Int64(string name, long value)
    {
        var offset = LocalOffset;
        value = _s.Int64(name, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} L)", offset, name, value);
        return value;
    }

    public byte UInt8(string name, byte value)
    {
        var offset = LocalOffset;
        value = _s.UInt8(name, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} uy)", offset, name, value);
        return value;
    }

    public ushort UInt16(string name, ushort value)
    {
        var offset = LocalOffset;
        value = _s.UInt16(name, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} us)", offset, name, value);
        return value;
    }

    public uint UInt32(string name, uint value)
    {
        var offset = LocalOffset;
        value = _s.UInt32(name, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} u)", offset, name, value);
        return value;
    }

    public ulong UInt64(string name, ulong value)
    {
        var offset = LocalOffset;
        value = _s.UInt64(name, value);
        DoIndent();
        // _tw.Write("{0:X} {1} = {2} (0x{3:X}`{4:X8} UL)", offset, name, value, (value & 0xffffffff00000000UL) >> 32, value & 0xffffffffUL);
        _tw.Write("{0:X} {1} = {2} (0x{2:X} UL)", offset, name, value);
        return value;
    }

    public T EnumU8<T>(int n, T value) where T : unmanaged, Enum
        => EnumU8(n.ToString(CultureInfo.InvariantCulture), value);
    public T EnumU8<T>(string name, T value) where T : unmanaged, Enum
    {
        var offset = LocalOffset;
        value = _s.EnumU8(name, value);
        var uintValue = SerdesUtil.EnumToUInt(value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} uy) // {3}", offset, name, uintValue, value.ToString());
        return value;
    }

    public T EnumU16<T>(int n, T value) where T : unmanaged, Enum
        => EnumU16(n.ToString(CultureInfo.InvariantCulture), value);
    public T EnumU16<T>(string name, T value) where T : unmanaged, Enum
    {
        var offset = LocalOffset;
        value = _s.EnumU16(name, value);
        var uintValue = SerdesUtil.EnumToUInt(value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} us) // {3}", offset, name, uintValue, value.ToString());
        return value;
    }

    public T EnumU32<T>(int n, T value) where T : unmanaged, Enum
        => EnumU32(n.ToString(CultureInfo.InvariantCulture), value);
    public T EnumU32<T>(string name, T value) where T : unmanaged, Enum
    {
        var offset = LocalOffset;
        value = _s.EnumU32(name, value);
        var uintValue = SerdesUtil.EnumToUInt(value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} u) // {3}", offset, name, uintValue, value.ToString());
        return value;
    }

    public Guid Guid(string name, Guid value)
    {
        var offset = LocalOffset;
        value = _s.Guid(name, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2:B}", offset, name, value);
        return value;
    }

    public byte[] Bytes(string name, byte[] value, int n)
    {
        var offset = LocalOffset;
        value = _s.Bytes(name, value, n);
        DoIndent();
        _tw.Write("{0:X} {1} = ", offset, name);

        var span = value.AsSpan().Slice(0, n);
        if (n <= 16)
            _tw.Write(SerdesUtil.ConvertToHexString(span));
        else
            PrintByteArrayHex(span);
        return value;
    }

#if NETSTANDARD2_1_OR_GREATER
        public void Bytes(string name, Span<byte> value)
        {
            int n = value.Length;
            var offset = LocalOffset;
            _s.Bytes(name, value);
            DoIndent();
            _tw.Write("{0:X} {1} = ", offset, name);

            if (n <= 16)
                _tw.Write(SerdesUtil.ConvertToHexString(value));
            else
                PrintByteArrayHex(value);
        }
#endif

    void PrintByteArrayHex(ReadOnlySpan<byte> value)
    {
        _indent += 4;
        var payloadOffset = 0;
        var sb = new StringBuilder(16);
        for (int i = 0; i < value.Length; i++)
        {
            byte b = value[i];
            if (payloadOffset % 16 == 0)
            {
                _tw.Write(' ');
                _tw.Write(sb.ToString());
                sb.Clear();
                DoIndent();
                _tw.Write("{0:X4}: ", payloadOffset);
            }
            else if (payloadOffset % 8 == 0) _tw.Write('-');
            else if (payloadOffset % 2 == 0) _tw.Write(' ');

            _tw.Write("{0:X2}", b);

            if (b >= (byte)' ' && b <= 0x7e) sb.Append(Convert.ToChar(b));
            else sb.Append('.');

            payloadOffset += 1;
        }

        if (sb.Length > 0)
        {
            var missingChars = 16 - sb.Length;
            var spaceCount = missingChars * 2 + missingChars / 2 + 1;
            _tw.Write(new string(Enumerable.Repeat(' ', spaceCount).ToArray()));
            _tw.Write(sb.ToString());
        }

        _indent -= 4;
    }

    public string NullTerminatedString(string name, string value)
    {
        value ??= string.Empty;
        var offset = LocalOffset;
        value = _s.NullTerminatedString(name, value);
        DoIndent();
        _tw.Write("{0:X} {1} = \"{2}\"", offset, name, value);
        return value;
    }

    public string FixedLengthString(string name, string value, int length)
    {
        value ??= string.Empty;
        var offset = LocalOffset;
        value = _s.FixedLengthString(name, value, length);
        DoIndent();
        _tw.Write("{0:X} {1} = \"{2}\"", offset, name, value);

        var bytes = _stringToBytes(value);
        if (bytes.Length > length + 1) throw new InvalidOperationException("Tried to write overlength string");
        return value;
    }

    public IList<TTarget> List<TTarget>(
        string name, IList<TTarget> list, int count,
        SerdesMethod<TTarget> serializer,
        Func<int, IList<TTarget>> initialiser = null)
        => List(name, list, count, 0, serializer, initialiser);

    public IList<TTarget> ListWithContext<TTarget, TContext>(
        string name, IList<TTarget> list, TContext context, int count,
        SerdesContextMethod<TTarget, TContext> serializer,
        Func<int, IList<TTarget>> initialiser = null)
        => ListWithContext(name, list, context, count, 0, serializer, initialiser);

    public IList<TTarget> List<TTarget>(
        string name,
        IList<TTarget> list,
        int count,
        int offset,
        SerdesMethod<TTarget> serializer,
        Func<int, IList<TTarget>> initialiser = null)
    {
        Begin(name);
        TTarget InterceptedSerializer(int i, TTarget x, ISerdes _) => serializer(i, x, this);
        var result = _s.List(name, list, count, offset, InterceptedSerializer, initialiser);
        End();
        return result;
    }

    public IList<TTarget> ListWithContext<TTarget, TContext>(
        string name,
        IList<TTarget> list,
        TContext context,
        int count,
        int offset,
        SerdesContextMethod<TTarget, TContext> serializer,
        Func<int, IList<TTarget>> initialiser = null)
    {
        Begin(name);
        TTarget InterceptedSerializer(int i, TTarget x, TContext c, ISerdes _) => serializer(i, x, c, this);
        var result = _s.ListWithContext(name, list, context, count, offset, InterceptedSerializer, initialiser);
        End();
        return result;
    }
}