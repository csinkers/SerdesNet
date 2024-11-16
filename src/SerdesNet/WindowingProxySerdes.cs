using System;
using System.Collections.Generic;

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
    public void Begin(string name = null) => _s.Begin(name);
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
    public void Pad(string name, int count, byte value) => _s.Pad(name, count, value);

    /// <inheritdoc />
    public sbyte Int8(int n, sbyte value) => _s.Int8(n, value);
    /// <inheritdoc />
    public short Int16(int n, short value) => _s.Int16(n, value);
    /// <inheritdoc />
    public int Int32(int n, int value) => _s.Int32(n, value);
    /// <inheritdoc />
    public long Int64(int n, long value) => _s.Int64(n, value);
    /// <inheritdoc />
    public byte UInt8(int n, byte value) => _s.UInt8(n, value);
    /// <inheritdoc />
    public ushort UInt16(int n, ushort value) => _s.UInt16(n, value);
    /// <inheritdoc />
    public uint UInt32(int n, uint value) => _s.UInt32(n, value);
    /// <inheritdoc />
    public ulong UInt64(int n, ulong value) => _s.UInt64(n, value);

    /// <inheritdoc />
    public sbyte Int8(string name, sbyte value) => _s.Int8(name, value);
    /// <inheritdoc />
    public short Int16(string name, short value) => _s.Int16(name, value);
    /// <inheritdoc />
    public int Int32(string name, int value) => _s.Int32(name, value);
    /// <inheritdoc />
    public long Int64(string name, long value) => _s.Int64(name, value);
    /// <inheritdoc />
    public byte UInt8(string name, byte value) => _s.UInt8(name, value);
    /// <inheritdoc />
    public ushort UInt16(string name, ushort value) => _s.UInt16(name, value);
    /// <inheritdoc />
    public uint UInt32(string name, uint value) => _s.UInt32(name, value);
    /// <inheritdoc />
    public ulong UInt64(string name, ulong value) => _s.UInt64(name, value);

    /// <inheritdoc />
    public T EnumU8<T>(int n, T value) where T : unmanaged, Enum => _s.EnumU8(n, value);
    /// <inheritdoc />
    public T EnumU16<T>(int n, T value) where T : unmanaged, Enum => _s.EnumU16(n, value);
    /// <inheritdoc />
    public T EnumU32<T>(int n, T value) where T : unmanaged, Enum => _s.EnumU32(n, value);
    /// <inheritdoc />
    public T EnumU8<T>(string name, T value) where T : unmanaged, Enum => _s.EnumU8(name, value);
    /// <inheritdoc />
    public T EnumU16<T>(string name, T value) where T : unmanaged, Enum => _s.EnumU16(name, value);
    /// <inheritdoc />
    public T EnumU32<T>(string name, T value) where T : unmanaged, Enum => _s.EnumU32(name, value);

    /// <inheritdoc />
    public Guid Guid(string name, Guid value) => _s.Guid(name, value);
    /// <inheritdoc />
    public byte[] Bytes(string name, byte[] value, int length) => _s.Bytes(name, value, length);

#if NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc />
    public void Bytes(string name, Span<byte> value) => _s.Bytes(name, value);
#endif

    /// <inheritdoc />
    public string NullTerminatedString(string name, string value) => _s.NullTerminatedString(name, value);
    /// <inheritdoc />
    public string FixedLengthString(string name, string value, int length) => _s.FixedLengthString(name, value, length);

    /// <inheritdoc />
    public IList<TTarget> List<TTarget>(string name,
        IList<TTarget> list,
        int count,
        SerdesMethod<TTarget> serdes,
        Func<int, IList<TTarget>> initialiser = null) => _s.List(name, list, count, serdes, initialiser);

    /// <inheritdoc />
    public IList<TTarget> List<TTarget>(string name,
        IList<TTarget> list,
        int count,
        int offset,
        SerdesMethod<TTarget> serdes,
        Func<int, IList<TTarget>> initialiser = null) => _s.List(name, list, count, offset, serdes, initialiser);

    /// <inheritdoc />
    public IList<TTarget> ListWithContext<TTarget, TContext>(string name,
        IList<TTarget> list,
        TContext context,
        int count,
        SerdesContextMethod<TTarget, TContext> serdes,
        Func<int, IList<TTarget>> initialiser = null) => _s.ListWithContext(name, list, context, count, serdes, initialiser);

    /// <inheritdoc />
    public IList<TTarget> ListWithContext<TTarget, TContext>(string name,
        IList<TTarget> list,
        TContext context,
        int count,
        int offset,
        SerdesContextMethod<TTarget, TContext> serdes,
        Func<int, IList<TTarget>> initialiser = null) => _s.ListWithContext(name, list, context, count, offset, serdes, initialiser);
}