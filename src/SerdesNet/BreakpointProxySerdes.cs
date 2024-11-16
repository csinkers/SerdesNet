using System;
using System.Collections.Generic;

namespace SerdesNet;

/// <summary>
/// Debugging serializer for investigating unintentional overwrites
/// After setting BreakRange to the byte range of interest the Hit
/// event will fire when bytes in the range are read or written.
/// </summary>
public sealed class BreakpointProxySerdes(ISerdes s) : ISerdes // For debugging unintentional overwrites
{
    /// <summary>
    /// Event fired when the byte range is read from or written to
    /// </summary>
    public event EventHandler<(long start, long finish)> Hit;

    /// <summary>
    /// The range of byte offsets to break on
    /// </summary>
    public (long from, long to)? BreakRange { get; set; }

    /// <inheritdoc />
    public void Dispose() { }
    /// <inheritdoc />
    public SerializerFlags Flags => s.Flags;
    /// <inheritdoc />
    public long Offset => s.Offset;
    /// <inheritdoc />
    public long BytesRemaining => s.BytesRemaining;
    /// <inheritdoc />
    public void Comment(string comment, bool inline) => s.Comment(comment);
    /// <inheritdoc />
    public void Begin(string name = null) => s.Begin(name);
    /// <inheritdoc />
    public void End() => s.End();
    /// <inheritdoc />
    public void NewLine() => s.NewLine();

    void CheckV(Action action)
    {
        var start = s.Offset;
        action();

        if (BreakRange == null)
            return;

        var finish = s.Offset;
        if (start <= BreakRange.Value.to && finish > BreakRange.Value.from)
            Hit?.Invoke(this, (start, finish));
    }

    T CheckT<T>(Func<T> func)
    {
        var start = s.Offset;
        var result = func();

        if (BreakRange != null)
        {
            var finish = s.Offset;
            if (start <= BreakRange.Value.to && finish > BreakRange.Value.from)
                Hit?.Invoke(this, (start, finish));
        }

        return result;
    }

    /// <inheritdoc />
    public void Seek(long offset) => s.Seek(offset);
    /// <inheritdoc />
    public void Assert(bool condition, string message) => s.Assert(condition, message);

    /// <inheritdoc />
    public void Pad(int count, byte value = 0) => CheckV(() => s.Pad(count, value));
    /// <inheritdoc />
    public void Pad(string name, int count, byte value) => CheckV(() => s.Pad(name, count, value));

    /// <inheritdoc />
    public sbyte Int8(int n, sbyte value) => CheckT(() => s.Int8(n, value));
    /// <inheritdoc />
    public short Int16(int n, short value) => CheckT(() => s.Int16(n, value));
    /// <inheritdoc />
    public int Int32(int n, int value) => CheckT(() => s.Int32(n, value));
    /// <inheritdoc />
    public long Int64(int n, long value) => CheckT(() => s.Int64(n, value));
    /// <inheritdoc />
    public byte UInt8(int n, byte value) => CheckT(() => s.UInt8(n, value));
    /// <inheritdoc />
    public ushort UInt16(int n, ushort value) => CheckT(() => s.UInt16(n, value));
    /// <inheritdoc />
    /// <inheritdoc />
    public uint UInt32(int n, uint value) => CheckT(() => s.UInt32(n, value));
    /// <inheritdoc />
    public ulong UInt64(int n, ulong value) => CheckT(() => s.UInt64(n, value));

    /// <inheritdoc />
    public sbyte Int8(string name, sbyte value) => CheckT(() => s.Int8(name, value));
    /// <inheritdoc />
    public short Int16(string name, short value) => CheckT(() => s.Int16(name, value));
    /// <inheritdoc />
    public int Int32(string name, int value) => CheckT(() => s.Int32(name, value));
    /// <inheritdoc />
    public long Int64(string name, long value) => CheckT(() => s.Int64(name, value));
    /// <inheritdoc />
    public byte UInt8(string name, byte value) => CheckT(() => s.UInt8(name, value));
    /// <inheritdoc />
    public ushort UInt16(string name, ushort value) => CheckT(() => s.UInt16(name, value));
    /// <inheritdoc />
    public uint UInt32(string name, uint value) => CheckT(() => s.UInt32(name, value));
    /// <inheritdoc />
    public ulong UInt64(string name, ulong value) => CheckT(() => s.UInt64(name, value));

    /// <inheritdoc />
    public T EnumU8<T>(int n, T value) where T : unmanaged, Enum => CheckT(() => s.EnumU8(n, value));
    /// <inheritdoc />
    public T EnumU16<T>(int n, T value) where T : unmanaged, Enum => CheckT(() => s.EnumU16(n, value));
    /// <inheritdoc />
    public T EnumU32<T>(int n, T value) where T : unmanaged, Enum => CheckT(() => s.EnumU32(n, value));

    /// <inheritdoc />
    public T EnumU8<T>(string name, T value) where T : unmanaged, Enum => CheckT(() => s.EnumU8(name, value));
    /// <inheritdoc />
    public T EnumU16<T>(string name, T value) where T : unmanaged, Enum => CheckT(() => s.EnumU16(name, value));
    /// <inheritdoc />
    public T EnumU32<T>(string name, T value) where T : unmanaged, Enum => CheckT(() => s.EnumU32(name, value));

    /// <inheritdoc />
    public Guid Guid(string name, Guid value) => CheckT(() => s.Guid(name, value));
    /// <inheritdoc />
    public byte[] Bytes(string name, byte[] value, int length) => CheckT(() => s.Bytes(name, value, length));

#if NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc />
    public void Bytes(string name, Span<byte> value)
    {
        // Can't use CheckT with Span
        var start = _s.Offset;
        _s.Bytes(name, value);

        if (BreakRange != null)
        {
            var finish = _s.Offset;
            if (start <= BreakRange.Value.to && finish >= BreakRange.Value.from)
                Hit?.Invoke(this, (start, finish));
        }
    }
#endif

    /// <inheritdoc />
    public string NullTerminatedString(string name, string value) => CheckT(() => s.NullTerminatedString(name, value));
    /// <inheritdoc />
    public string FixedLengthString(string name, string value, int length) => CheckT(() => s.FixedLengthString(name, value, length));

    /// <inheritdoc />
    public IList<TTarget> List<TTarget>(
        string name,
        IList<TTarget> list,
        int count,
        SerdesMethod<TTarget> serdes,
        Func<int, IList<TTarget>> initialiser = null) 
        => CheckT(() => s.List(name, list, count, serdes, initialiser));

    /// <inheritdoc />
    public IList<TTarget> List<TTarget>(
        string name,
        IList<TTarget> list,
        int count,
        int offset,
        SerdesMethod<TTarget> serdes,
        Func<int, IList<TTarget>> initialiser = null) 
        => CheckT(() => s.List(name, list, count, offset, serdes, initialiser));

    /// <inheritdoc />
    public IList<TTarget> ListWithContext<TTarget, TContext>(
        string name,
        IList<TTarget> list,
        TContext context,
        int count,
        SerdesContextMethod<TTarget, TContext> serdes,
        Func<int, IList<TTarget>> initialiser = null) 
        => CheckT(() => s.ListWithContext(name, list, context, count, serdes, initialiser));

    /// <inheritdoc />
    public IList<TTarget> ListWithContext<TTarget, TContext>(
        string name,
        IList<TTarget> list,
        TContext context,
        int count,
        int offset,
        SerdesContextMethod<TTarget, TContext> serdes,
        Func<int, IList<TTarget>> initialiser = null) 
        => CheckT(() => s.ListWithContext(name, list, context, count, offset, serdes, initialiser));
}