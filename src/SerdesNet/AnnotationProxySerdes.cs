using System;
using System.Collections.Generic;
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
    readonly ISerdes _s;
    readonly Stack<long> _offsetStack = new();
    readonly bool _useRelativeOffsets;
    int _indent;

    /// <summary>
    /// Creates a new AnnotationProxySerdes.
    /// </summary>
    /// <param name="s">The underlying serdes to delegate to</param>
    /// <param name="tw">The text writer that will record the annotations</param>
    /// <param name="useRelativeOffsets">Whether to use absolute or relative offsets when emitting fields inside a block/object scope</param>
    /// <exception cref="ArgumentNullException"></exception>
    public AnnotationProxySerdes(ISerdes s, TextWriter tw, bool useRelativeOffsets = true)
    {
        _s = s ?? throw new ArgumentNullException(nameof(s));
        _tw = tw ?? throw new ArgumentNullException(nameof(tw));
        _offsetStack.Push(0);
        _useRelativeOffsets = useRelativeOffsets;
    }

    /// <inheritdoc />
    public void Dispose() => _s.Dispose();

    /// <inheritdoc />
    public SerializerFlags Flags => _s.Flags | SerializerFlags.Comments;

    /// <inheritdoc />
    public long Offset => _s.Offset;
    long LocalOffset => _useRelativeOffsets ? Offset - _offsetStack.Peek() : Offset;

    /// <inheritdoc />
    public long BytesRemaining => _s.BytesRemaining;

    /// <inheritdoc />
    public void Seek(long offset)
    {
        DoIndent();
        _tw.Write("{1:X} Seek to {0:X} for overwrite", offset, LocalOffset);
        _s.Seek(offset);
    }

    /// <inheritdoc />
    public void Assert(bool condition, string message) => _s.Assert(condition, message);

    /// <inheritdoc />
    public void Pad(int length, byte value) => _s.Pad(length, value); // Don't write anything to the annotation stream for unnamed padding

    /// <inheritdoc />
    public void Pad(SerdesName name, int length, byte value)
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

    /// <inheritdoc />
    public void Comment(string msg, bool inline)
    {
        if (!inline)
        {
            DoIndent();
            _tw.Write("// {0}", msg);
        }
        else _tw.Write(" // {0}", msg);
    }

    /// <inheritdoc />
    public void Begin(SerdesName name)
    {
        DoIndent();
        _tw.Write($"{LocalOffset:X} {name}: {{");
        _indent += 4;
        _offsetStack.Push(Offset);
    }

    /// <inheritdoc />
    public void End()
    {
        _offsetStack.Pop();
        _indent -= 4;
        DoIndent();
        _tw.Write("}");
    }

    /// <inheritdoc />
    public void NewLine() => _tw.WriteLine();

    /// <inheritdoc />
    public sbyte Int8(SerdesName name, sbyte value)
    {
        var offset = LocalOffset;
        value = _s.Int8(name, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} y)", offset, name, value);
        return value;
    }

    /// <inheritdoc />
    public short Int16(SerdesName name, short value)
    {
        var offset = LocalOffset;
        value = _s.Int16(name, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} s)", offset, name, value);
        return value;
    }

    /// <inheritdoc />
    public int Int32(SerdesName name, int value)
    {
        var offset = LocalOffset;
        value = _s.Int32(name, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X})", offset, name, value);
        return value;
    }

    /// <inheritdoc />
    public long Int64(SerdesName name, long value)
    {
        var offset = LocalOffset;
        value = _s.Int64(name, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} L)", offset, name, value);
        return value;
    }

    /// <inheritdoc />
    public byte UInt8(SerdesName name, byte value)
    {
        var offset = LocalOffset;
        value = _s.UInt8(name, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} uy)", offset, name, value);
        return value;
    }

    /// <inheritdoc />
    public ushort UInt16(SerdesName name, ushort value)
    {
        var offset = LocalOffset;
        value = _s.UInt16(name, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} us)", offset, name, value);
        return value;
    }

    /// <inheritdoc />
    public uint UInt32(SerdesName name, uint value)
    {
        var offset = LocalOffset;
        value = _s.UInt32(name, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2} (0x{2:X} u)", offset, name, value);
        return value;
    }

    /// <inheritdoc />
    public ulong UInt64(SerdesName name, ulong value)
    {
        var offset = LocalOffset;
        value = _s.UInt64(name, value);
        DoIndent();
        // _tw.Write("{0:X} {1} = {2} (0x{3:X}`{4:X8} UL)", offset, name, value, (value & 0xffffffff00000000UL) >> 32, value & 0xffffffffUL);
        _tw.Write("{0:X} {1} = {2} (0x{2:X} UL)", offset, name, value);
        return value;
    }

    /// <inheritdoc />
    public Guid Guid(SerdesName name, Guid value)
    {
        var offset = LocalOffset;
        value = _s.Guid(name, value);
        DoIndent();
        _tw.Write("{0:X} {1} = {2:B}", offset, name, value);
        return value;
    }

    /// <inheritdoc />
    public byte[] Bytes(SerdesName name, byte[] value, int n)
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
        public void Bytes(SerdesName name, Span<byte> value)
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
        foreach (var b in value)
        {
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

            sb.Append(b is >= (byte)' ' and <= 0x7e ? Convert.ToChar(b) : '.');

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
}