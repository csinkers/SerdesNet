using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace SerdesNet;

/// <summary>
/// A serializer that writes data to a <see cref="BinaryWriter"/>
/// </summary>
public class WriterSerdes : ISerdes
{
    readonly Func<string, byte[]> _stringToBytes;
    readonly BinaryWriter _bw;
    readonly Action<string> _assertionFailed;
    readonly Action _disposeAction;

    /// <summary>
    /// A serializer that writes data to a <see cref="BinaryWriter"/>
    /// </summary>
    /// <param name="bw">The binary writer to write to</param>
    /// <param name="stringToBytes">A method to convert a string into a sequence of bytes using the desired encoding</param>
    /// <param name="assertionFailed">An optional callback to be invoked when an assertion failure occurs</param>
    /// <param name="disposeAction">An optional callback to be invoked when the <see cref="WriterSerdes"/> is disposed</param>
    public WriterSerdes(BinaryWriter bw,
        Func<string, byte[]> stringToBytes,
        Action<string> assertionFailed = null,
        Action disposeAction = null)
    {
        _assertionFailed = assertionFailed;
        _disposeAction = disposeAction;
        _stringToBytes = stringToBytes ?? throw new ArgumentNullException(nameof(stringToBytes));
        _bw = bw ?? throw new ArgumentNullException(nameof(bw));
    }

    /// <summary>
    /// Create a new <see cref="WriterSerdes"/> for a given <see cref="MemoryStream"/> using the specified encoding
    /// </summary>
    public WriterSerdes(MemoryStream stream, Encoding encoding)
        : this(new BinaryWriter(stream, encoding, true), encoding.GetBytes)
    {
    }

    /// <inheritdoc />
    public SerializerFlags Flags => SerializerFlags.Write;
    /// <inheritdoc />
    public long BytesRemaining => int.MaxValue;
    /// <inheritdoc />
    public void Comment(string msg, bool inline) { }
    /// <inheritdoc />
    public void Begin(string name = null) { }
    /// <inheritdoc />
    public void End() { }
    /// <inheritdoc />
    public void NewLine() { }

    /// <inheritdoc />
    public long Offset => _bw.BaseStream.Position;

    /// <inheritdoc />
    public void Seek(long newOffset) => _bw.Seek((int)newOffset, SeekOrigin.Begin);

    /// <inheritdoc />
    public void Pad(int bytes, byte value) => Pad(null, bytes, value);

    /// <inheritdoc />
    public void Pad(string name, int count, byte value)
    {
        for (int i = 0; i < count; i++)
            _bw.Write(value);
    }

    /// <inheritdoc />
    public sbyte Int8(int n, sbyte value) { _bw.Write(value);  return value; }
    /// <inheritdoc />
    public short Int16(int n, short value) { _bw.Write(value);  return value; }
    /// <inheritdoc />
    public int Int32(int n, int value) { _bw.Write(value);  return value; }
    /// <inheritdoc />
    public long Int64(int n, long value) { _bw.Write(value);  return value; }
    /// <inheritdoc />
    public byte UInt8(int n, byte value) { _bw.Write(value);  return value; }
    /// <inheritdoc />
    public ushort UInt16(int n, ushort value) { _bw.Write(value);  return value; }
    /// <inheritdoc />
    public uint UInt32(int n, uint value) { _bw.Write(value);  return value; }
    /// <inheritdoc />
    public ulong UInt64(int n, ulong value) { _bw.Write(value);  return value; }

    /// <inheritdoc />
    public sbyte Int8(string name, sbyte value) { _bw.Write(value);  return value; }
    /// <inheritdoc />
    public short Int16(string name, short value) { _bw.Write(value);  return value; }
    /// <inheritdoc />
    public int Int32(string name, int value) { _bw.Write(value);  return value; }
    /// <inheritdoc />
    public long Int64(string name, long value) { _bw.Write(value);  return value; }
    /// <inheritdoc />
    public byte UInt8(string name, byte value) { _bw.Write(value);  return value; }
    /// <inheritdoc />
    public ushort UInt16(string name, ushort value) { _bw.Write(value);  return value; }
    /// <inheritdoc />
    public uint UInt32(string name, uint value) { _bw.Write(value);  return value; }
    /// <inheritdoc />
    public ulong UInt64(string name, ulong value) { _bw.Write(value);  return value; }

    /// <inheritdoc />
    public T EnumU8<T>(int n, T value) where T : unmanaged, Enum => EnumU8(null, value);
    /// <inheritdoc />
    public T EnumU8<T>(string name, T value) where T : unmanaged, Enum
    {
        _bw.Write(SerdesUtil.EnumToByte(value));
        return value;
    }

    /// <inheritdoc />
    public T EnumU16<T>(int n, T value) where T : unmanaged, Enum => EnumU16(null, value);
    /// <inheritdoc />
    public T EnumU16<T>(string name, T value) where T : unmanaged, Enum
    {
        _bw.Write(SerdesUtil.EnumToUShort(value));
        return value;
    }

    /// <inheritdoc />
    public T EnumU32<T>(int n, T value) where T : unmanaged, Enum => EnumU32(null, value);
    /// <inheritdoc />
    public T EnumU32<T>(string name, T value) where T : unmanaged, Enum
    {
        _bw.Write(SerdesUtil.EnumToUInt(value));
        return value;
    }

    /// <inheritdoc />
    public Guid Guid(string name, Guid value)
    {
        _bw.Write(value.ToByteArray());
        return value;
    }

    /// <inheritdoc />
    public byte[] Bytes(string name, byte[] value, int n)
    {
        if (value is { Length: > 0 })
            _bw.Write(value, 0, n);
        return value;
    }

#if NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc />
    public void Bytes(string name, Span<byte> value)
    {
        if (value.Length > 0)
            _bw.Write(value);
    }
#endif

    /// <inheritdoc />
    public string NullTerminatedString(string name, string value)
    {
        value ??= string.Empty;
        var bytes = _stringToBytes(value);
        _bw.Write(bytes);
        _bw.Write((byte)0);
        return value;
    }

    /// <inheritdoc />
    public string FixedLengthString(string name, string value, int length)
    {
        value ??= string.Empty;
        var bytes = _stringToBytes(value);
        if (bytes.Length > length + 1)
            _assertionFailed("Tried to write over-length string");

        _bw.Write(bytes);

        for (int i = bytes.Length; i < length; i++)
            _bw.Write((byte)0);

        return value;
    }

    /// <inheritdoc />
    public IList<TTarget> List<TTarget>(
        string name, IList<TTarget> list, int count,
        SerdesMethod<TTarget> serializer,
        Func<int, IList<TTarget>> initialiser = null)
        => List(name, list, count, 0, serializer, initialiser);

    /// <inheritdoc />
    public IList<TTarget> ListWithContext<TTarget, TContext>(
        string name, IList<TTarget> list, TContext context, int count,
        SerdesContextMethod<TTarget, TContext> serializer,
        Func<int, IList<TTarget>> initialiser = null)
        => ListWithContext(name, list, context, count, 0, serializer, initialiser);

    /// <inheritdoc />
    public IList<TTarget> List<TTarget>(
        string name,
        IList<TTarget> list,
        int count,
        int offset,
        SerdesMethod<TTarget> serializer,
        Func<int, IList<TTarget>> initialiser = null)
    {
        list ??= initialiser?.Invoke(count) ?? new TTarget[count];
        for (int i = offset; i < count + offset; i++)
            serializer(i, list[i], this);
        return list;
    }

    /// <inheritdoc />
    public IList<TTarget> ListWithContext<TTarget, TContext>(
        string name,
        IList<TTarget> list,
        TContext context,
        int count,
        int offset,
        SerdesContextMethod<TTarget, TContext> serializer,
        Func<int, IList<TTarget>> initialiser = null)
    {
        list ??= initialiser?.Invoke(count) ?? new TTarget[count];
        for (int i = offset; i < count + offset; i++)
            serializer(i, list[i], context, this);
        return list;
    }

    void ISerdes.Assert(bool condition, string message) => Assert(condition, message);
    void Assert(bool condition, string message = null, [CallerMemberName] string function = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        if (condition)
            return;

        var formatted = $"Assertion failed: {message} at {function} in {file}:{line}";
        _assertionFailed?.Invoke(formatted);
    }

    /// <summary>
    /// The actual implementation of <see cref="IDisposable.Dispose"/>
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            _disposeAction?.Invoke();
    }

    /// <inheritdoc />
    public void Dispose() => Dispose(true);
}