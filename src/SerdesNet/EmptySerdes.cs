using System;
using System.Collections.Generic;

namespace SerdesNet;

/// <summary>
/// Stub serializer for representing an empty file
/// </summary>
public class EmptySerdes : ISerdes
{
    /// <inheritdoc />
    public SerializerFlags Flags => SerializerFlags.Read;
    /// <inheritdoc />
    public long Offset => 0;
    /// <inheritdoc />
    public long BytesRemaining => 0;
    /// <inheritdoc />
    public void Dispose() { } 
    /// <inheritdoc />
    public void Comment(string comment, bool inline) { } 
    /// <inheritdoc />
    public void Begin(string name = null) { } 
    /// <inheritdoc />
    public void End() { } 
    /// <inheritdoc />
    public void NewLine() { } 
    /// <inheritdoc />
    public void Seek(long offset) => throw new NotSupportedException();
    /// <inheritdoc />
    public void Assert(bool condition, string message) { } 
    /// <inheritdoc />
    public void Pad(int count, byte value) => throw new NotSupportedException();
    /// <inheritdoc />
    public void Pad(string name, int count, byte value) => throw new NotSupportedException();

    /// <inheritdoc />
    public sbyte Int8(int n, sbyte value) => throw new NotSupportedException();
    /// <inheritdoc />
    public short Int16(int n, short value) => throw new NotSupportedException();
    /// <inheritdoc />
    public int Int32(int n, int value) => throw new NotSupportedException();
    /// <inheritdoc />
    public long Int64(int n, long value) => throw new NotSupportedException();
    /// <inheritdoc />
    public byte UInt8(int n, byte value) => throw new NotSupportedException();
    /// <inheritdoc />
    public ushort UInt16(int n, ushort value) => throw new NotSupportedException();
    /// <inheritdoc />
    public uint UInt32(int n, uint value) => throw new NotSupportedException();
    /// <inheritdoc />
    public ulong UInt64(int n, ulong value) => throw new NotSupportedException();

    /// <inheritdoc />
    public sbyte Int8(string name, sbyte value) => throw new NotSupportedException();
    /// <inheritdoc />
    public short Int16(string name, short value) => throw new NotSupportedException();
    /// <inheritdoc />
    public int Int32(string name, int value) => throw new NotSupportedException();
    /// <inheritdoc />
    public long Int64(string name, long value) => throw new NotSupportedException();
    /// <inheritdoc />
    public byte UInt8(string name, byte value) => throw new NotSupportedException();
    /// <inheritdoc />
    public ushort UInt16(string name, ushort value) => throw new NotSupportedException();
    /// <inheritdoc />
    public uint UInt32(string name, uint value) => throw new NotSupportedException();
    /// <inheritdoc />
    public ulong UInt64(string name, ulong value) => throw new NotSupportedException();

    /// <inheritdoc />
    public T EnumU8<T>(int n, T value) where T : unmanaged, Enum => throw new NotSupportedException();
    /// <inheritdoc />
    public T EnumU16<T>(int n, T value) where T : unmanaged, Enum => throw new NotSupportedException();
    /// <inheritdoc />
    public T EnumU32<T>(int n, T value) where T : unmanaged, Enum => throw new NotSupportedException();

    /// <inheritdoc />
    public T EnumU8<T>(string name, T value) where T : unmanaged, Enum => throw new NotSupportedException();
    /// <inheritdoc />
    public T EnumU16<T>(string name, T value) where T : unmanaged, Enum => throw new NotSupportedException();
    /// <inheritdoc />
    public T EnumU32<T>(string name, T value) where T : unmanaged, Enum => throw new NotSupportedException();

    /// <inheritdoc />
    public Guid Guid(string name, Guid value) => throw new NotSupportedException();
    /// <inheritdoc />
    public byte[] Bytes(string name, byte[] value, int length) => throw new NotSupportedException();

#if NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc />
    public void Bytes(string name, Span<byte> value) => throw new NotSupportedException();
#endif

    /// <inheritdoc />
    public string NullTerminatedString(string name, string value) => throw new NotSupportedException();
    /// <inheritdoc />
    public string FixedLengthString(string name, string value, int length) => throw new NotSupportedException();
    /// <inheritdoc />
    public IList<TTarget> List<TTarget>(
        string name, 
        IList<TTarget> list,
        int count,
        SerdesMethod<TTarget> serdes,
        Func<int, IList<TTarget>> initialiser = null) => throw new NotSupportedException();

    /// <inheritdoc />
    public IList<TTarget> List<TTarget>(
        string name,
        IList<TTarget> list,
        int count,
        int offset,
        SerdesMethod<TTarget> serdes,
        Func<int, IList<TTarget>> initialiser = null) => throw new NotSupportedException();

    /// <inheritdoc />
    public IList<TTarget> ListWithContext<TTarget, TContext>(
        string name,
        IList<TTarget> list,
        TContext context,
        int count,
        SerdesContextMethod<TTarget, TContext> serdes,
        Func<int, IList<TTarget>> initialiser = null) => throw new NotSupportedException();

    /// <inheritdoc />
    public IList<TTarget> ListWithContext<TTarget, TContext>(
        string name,
        IList<TTarget> list,
        TContext context,
        int count,
        int offset,
        SerdesContextMethod<TTarget, TContext> serdes,
        Func<int, IList<TTarget>> initialiser = null) => throw new NotSupportedException();
}