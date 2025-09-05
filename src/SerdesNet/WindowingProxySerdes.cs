using System;

namespace SerdesNet;

/// <summary>
/// Delegates reading/writing to an underlying serializer while
/// manipulating Offset and BytesRemaining to simulate a subset
/// of the underlying stream/buffer.
/// </summary>
public class WindowingProxySerdes : ISerdes
{
    readonly ISerdes _s;
    readonly int? _size;
    readonly long _relativeOffset;

    /// <summary>
    /// Create a new WindowingProxySerdes
    /// </summary>
    /// <param name="s">The serdes instance to delegate to</param>
    /// <param name="size">The maximum number of bytes that should be read from/written to in this window into the parent stream.</param>
    public WindowingProxySerdes(ISerdes s, int? size)
    {
        _s = s ?? throw new ArgumentNullException(nameof(s));
        _relativeOffset = _s.Offset;

        if (size > _s.BytesRemaining)
        {
            throw new ArgumentOutOfRangeException(nameof(size),
                $"Tried to create a windowing serializer with a size of {size}, but " +
                $"the underlying serializer only has {_s.BytesRemaining} bytes remaining");
        }

        _size = size;
    }

    /// <inheritdoc />
    public void Dispose() { }
    /// <inheritdoc />
    public SerializerFlags Flags => _s.Flags;
    /// <inheritdoc />
    public long Offset => _s.Offset - _relativeOffset;
    /// <inheritdoc />
    public long BytesRemaining => _size.HasValue ? _size.Value - Offset : _s.BytesRemaining;
    /// <inheritdoc />
    public void Comment(string comment, bool inline) => _s.Comment(comment, inline);
    /// <inheritdoc />
    public void Begin(SerdesName name = default) => _s.Begin(name);
    /// <inheritdoc />
    public void End() => _s.End();
    /// <inheritdoc />
    public void NewLine() => _s.NewLine();
    /// <inheritdoc />
    public void Seek(long offset) => _s.Seek(offset + _relativeOffset);
    /// <inheritdoc />
    public void Assert(bool condition, string message) => _s.Assert(condition, message);

    /// <inheritdoc />
    public void Pad(int count, byte value) => _s.Pad(count, value);
    /// <inheritdoc />
    public void Pad(SerdesName name, int count, byte value) => _s.Pad(name, count, value);

    /// <inheritdoc />
    public sbyte Int8(SerdesName name, sbyte value) => _s.Int8(name, value);
    /// <inheritdoc />
    public short Int16(SerdesName name, short value) => _s.Int16(name, value);
    /// <inheritdoc />
    public int Int32(SerdesName name, int value) => _s.Int32(name, value);
    /// <inheritdoc />
    public long Int64(SerdesName name, long value) => _s.Int64(name, value);
    /// <inheritdoc />
    public byte UInt8(SerdesName name, byte value) => _s.UInt8(name, value);
    /// <inheritdoc />
    public ushort UInt16(SerdesName name, ushort value) => _s.UInt16(name, value);
    /// <inheritdoc />
    public uint UInt32(SerdesName name, uint value) => _s.UInt32(name, value);
    /// <inheritdoc />
    public ulong UInt64(SerdesName name, ulong value) => _s.UInt64(name, value);

    /// <inheritdoc />
    public Guid Guid(SerdesName name, Guid value) => _s.Guid(name, value);
    /// <inheritdoc />
    public byte[] Bytes(SerdesName name, byte[] value, int length) => _s.Bytes(name, value, length);

#if NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc />
    public void Bytes(SerdesName name, Span<byte> value) => _s.Bytes(name, value);
#endif
}