using System;

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
    public void Begin(SerdesName name = default) => s.Begin(name);
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
    public void Pad(SerdesName name, int count, byte value) => CheckV(() => s.Pad(name, count, value));


    /// <inheritdoc />
    public sbyte Int8(SerdesName name, sbyte value) => CheckT(() => s.Int8(name, value));
    /// <inheritdoc />
    public short Int16(SerdesName name, short value) => CheckT(() => s.Int16(name, value));
    /// <inheritdoc />
    public int Int32(SerdesName name, int value) => CheckT(() => s.Int32(name, value));
    /// <inheritdoc />
    public long Int64(SerdesName name, long value) => CheckT(() => s.Int64(name, value));
    /// <inheritdoc />
    public byte UInt8(SerdesName name, byte value) => CheckT(() => s.UInt8(name, value));
    /// <inheritdoc />
    public ushort UInt16(SerdesName name, ushort value) => CheckT(() => s.UInt16(name, value));
    /// <inheritdoc />
    public uint UInt32(SerdesName name, uint value) => CheckT(() => s.UInt32(name, value));
    /// <inheritdoc />
    public ulong UInt64(SerdesName name, ulong value) => CheckT(() => s.UInt64(name, value));

    /// <inheritdoc />
    public Guid Guid(SerdesName name, Guid value) => CheckT(() => s.Guid(name, value));
    /// <inheritdoc />
    public byte[] Bytes(SerdesName name, byte[] value, int length) => CheckT(() => s.Bytes(name, value, length));

#if NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc />
    public void Bytes(SerdesName name, Span<byte> value)
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
    public string NullTerminatedString(SerdesName name, string value) => CheckT(() => s.NullTerminatedString(name, value));
    /// <inheritdoc />
    public string FixedLengthString(SerdesName name, string value, int length) => CheckT(() => s.FixedLengthString(name, value, length));
}