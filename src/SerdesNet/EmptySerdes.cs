using System;

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
    public void Begin(SerdesName name = default) { } 
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
    public void Pad(SerdesName name, int count, byte value) => throw new NotSupportedException();

    /// <inheritdoc />
    public sbyte Int8(SerdesName name, sbyte value) => throw new NotSupportedException();
    /// <inheritdoc />
    public short Int16(SerdesName name, short value) => throw new NotSupportedException();
    /// <inheritdoc />
    public int Int32(SerdesName name, int value) => throw new NotSupportedException();
    /// <inheritdoc />
    public long Int64(SerdesName name, long value) => throw new NotSupportedException();
    /// <inheritdoc />
    public byte UInt8(SerdesName name, byte value) => throw new NotSupportedException();
    /// <inheritdoc />
    public ushort UInt16(SerdesName name, ushort value) => throw new NotSupportedException();
    /// <inheritdoc />
    public uint UInt32(SerdesName name, uint value) => throw new NotSupportedException();
    /// <inheritdoc />
    public ulong UInt64(SerdesName name, ulong value) => throw new NotSupportedException();

    /// <inheritdoc />
    public Guid Guid(SerdesName name, Guid value) => throw new NotSupportedException();
    /// <inheritdoc />
    public byte[] Bytes(SerdesName name, byte[] value, int length) => throw new NotSupportedException();

#if NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc />
    public void Bytes(SerdesName name, Span<byte> value) => throw new NotSupportedException();
#endif

    /// <inheritdoc />
    public string NullTerminatedString(SerdesName name, string value) => throw new NotSupportedException();
    /// <inheritdoc />
    public string FixedLengthString(SerdesName name, string value, int length) => throw new NotSupportedException();
}