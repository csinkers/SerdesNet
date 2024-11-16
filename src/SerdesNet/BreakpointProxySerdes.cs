﻿using System;
using System.Collections.Generic;

namespace SerdesNet;

/// <summary>
/// Debugging serializer for investigating unintentional overwrites
/// After setting BreakRange to the byte range of interest the Hit
/// event will fire when bytes in the range are read or written.
/// </summary>
public sealed class BreakpointProxySerdes : ISerdes // For debugging unintentional overwrites
{
    readonly ISerdes _s;
    public event EventHandler<(long start, long finish)> Hit;
    public (long from, long to)? BreakRange { get; set; }
    public BreakpointProxySerdes(ISerdes s) => _s = s ?? throw new ArgumentNullException(nameof(s));
    public void Dispose() { }
    public SerializerFlags Flags => _s.Flags;
    public long Offset => _s.Offset;
    public long BytesRemaining => _s.BytesRemaining;
    public void Comment(string comment, bool inline) => _s.Comment(comment);
    public void Begin(string name = null) => _s.Begin(name);
    public void End() => _s.End();
    public void NewLine() => _s.NewLine();

    void CheckV(Action action)
    {
        var start = _s.Offset;
        action();

        if (BreakRange == null)
            return;

        var finish = _s.Offset;
        if (start <= BreakRange.Value.to && finish >= BreakRange.Value.from)
            Hit?.Invoke(this, (start, finish));
    }

    T CheckT<T>(Func<T> func)
    {
        var start = _s.Offset;
        var result = func();

        if (BreakRange != null)
        {
            var finish = _s.Offset;
            if (start <= BreakRange.Value.to && finish >= BreakRange.Value.from)
                Hit?.Invoke(this, (start, finish));
        }

        return result;
    }

    public void Seek(long offset) => _s.Seek(offset);
    public void Assert(bool condition, string message) => _s.Assert(condition, message);

    public void Pad(int count, byte value = 0) => _s.Pad(count, value);
    public void Pad(string name, int count, byte value) => CheckV(() => _s.Pad(name, count, value));
#pragma warning disable CA1720 // Identifier contains type name

    public sbyte Int8(int n, sbyte value) => CheckT(() => _s.Int8(n, value));
    public short Int16(int n, short value) => CheckT(() => _s.Int16(n, value));
    public int Int32(int n, int value) => CheckT(() => _s.Int32(n, value));
    public long Int64(int n, long value) => CheckT(() => _s.Int64(n, value));
    public byte UInt8(int n, byte value) => CheckT(() => _s.UInt8(n, value));
    public ushort UInt16(int n, ushort value) => CheckT(() => _s.UInt16(n, value));
    public uint UInt32(int n, uint value) => CheckT(() => _s.UInt32(n, value));
    public ulong UInt64(int n, ulong value) => CheckT(() => _s.UInt64(n, value));

    public sbyte Int8(string name, sbyte value) => CheckT(() => _s.Int8(name, value));
    public short Int16(string name, short value) => CheckT(() => _s.Int16(name, value));
    public int Int32(string name, int value) => CheckT(() => _s.Int32(name, value));
    public long Int64(string name, long value) => CheckT(() => _s.Int64(name, value));
    public byte UInt8(string name, byte value) => CheckT(() => _s.UInt8(name, value));
    public ushort UInt16(string name, ushort value) => CheckT(() => _s.UInt16(name, value));
    public uint UInt32(string name, uint value) => CheckT(() => _s.UInt32(name, value));
    public ulong UInt64(string name, ulong value) => CheckT(() => _s.UInt64(name, value));

    public T EnumU8<T>(int n, T value) where T : unmanaged, Enum => CheckT(() => _s.EnumU8(n, value));
    public T EnumU16<T>(int n, T value) where T : unmanaged, Enum => CheckT(() => _s.EnumU16(n, value));
    public T EnumU32<T>(int n, T value) where T : unmanaged, Enum => CheckT(() => _s.EnumU32(n, value));

    public T EnumU8<T>(string name, T value) where T : unmanaged, Enum => CheckT(() => _s.EnumU8(name, value));
    public T EnumU16<T>(string name, T value) where T : unmanaged, Enum => CheckT(() => _s.EnumU16(name, value));
    public T EnumU32<T>(string name, T value) where T : unmanaged, Enum => CheckT(() => _s.EnumU32(name, value));

    public Guid Guid(string name, Guid value) => CheckT(() => _s.Guid(name, value));
    public byte[] Bytes(string name, byte[] value, int length) => CheckT(() => _s.Bytes(name, value, length));

#if NETSTANDARD2_1_OR_GREATER
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

    public string NullTerminatedString(string name, string value) => CheckT(() => _s.NullTerminatedString(name, value));
    public string FixedLengthString(string name, string value, int length) => CheckT(() => _s.FixedLengthString(name, value, length));

#pragma warning restore CA1720 // Identifier contains type name
    public IList<TTarget> List<TTarget>(
        string name,
        IList<TTarget> list,
        int count,
        SerdesMethod<TTarget> serdes,
        Func<int, IList<TTarget>> initialiser = null) 
        => CheckT(() => _s.List(name, list, count, serdes, initialiser));

    public IList<TTarget> List<TTarget>(
        string name,
        IList<TTarget> list,
        int count,
        int offset,
        SerdesMethod<TTarget> serdes,
        Func<int, IList<TTarget>> initialiser = null) 
        => CheckT(() => _s.List(name, list, count, offset, serdes, initialiser));

    public IList<TTarget> ListWithContext<TTarget, TContext>(
        string name,
        IList<TTarget> list,
        TContext context,
        int count,
        SerdesContextMethod<TTarget, TContext> serdes,
        Func<int, IList<TTarget>> initialiser = null) 
        => CheckT(() => _s.ListWithContext(name, list, context, count, serdes, initialiser));

    public IList<TTarget> ListWithContext<TTarget, TContext>(
        string name,
        IList<TTarget> list,
        TContext context,
        int count,
        int offset,
        SerdesContextMethod<TTarget, TContext> serdes,
        Func<int, IList<TTarget>> initialiser = null) 
        => CheckT(() => _s.ListWithContext(name, list, context, count, offset, serdes, initialiser));
}