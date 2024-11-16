using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace SerdesNet;

public class WriterSerdes(
    BinaryWriter bw,
    Func<string, byte[]> stringToBytes,
    Action<string> assertionFailed = null,
    Action disposeAction = null)
    : ISerdes
{
    readonly Func<string, byte[]> _stringToBytes = stringToBytes ?? throw new ArgumentNullException(nameof(stringToBytes));
    readonly BinaryWriter _bw = bw ?? throw new ArgumentNullException(nameof(bw));

    public SerializerFlags Flags => SerializerFlags.Write;
    public long BytesRemaining => int.MaxValue;
    public void Comment(string msg, bool inline) { }
    public void Begin(string name = null) { }
    public void End() { }
    public void NewLine() { }

    public long Offset => _bw.BaseStream.Position;

    public void Seek(long newOffset)
    {
        _bw.Seek((int)newOffset, SeekOrigin.Begin);
    }

    public void Pad(int bytes, byte value) => Pad(null, bytes, value);
    public void Pad(string name, int count, byte value)
    {
        for (int i = 0; i < count; i++)
            _bw.Write(value);
    }

    public sbyte Int8(int n, sbyte value) { _bw.Write(value);  return value; }
    public short Int16(int n, short value) { _bw.Write(value);  return value; }
    public int Int32(int n, int value) { _bw.Write(value);  return value; }
    public long Int64(int n, long value) { _bw.Write(value);  return value; }
    public byte UInt8(int n, byte value) { _bw.Write(value);  return value; }
    public ushort UInt16(int n, ushort value) { _bw.Write(value);  return value; }
    public uint UInt32(int n, uint value) { _bw.Write(value);  return value; }
    public ulong UInt64(int n, ulong value) { _bw.Write(value);  return value; }

    public sbyte Int8(string name, sbyte value) { _bw.Write(value);  return value; }
    public short Int16(string name, short value) { _bw.Write(value);  return value; }
    public int Int32(string name, int value) { _bw.Write(value);  return value; }
    public long Int64(string name, long value) { _bw.Write(value);  return value; }
    public byte UInt8(string name, byte value) { _bw.Write(value);  return value; }
    public ushort UInt16(string name, ushort value) { _bw.Write(value);  return value; }
    public uint UInt32(string name, uint value) { _bw.Write(value);  return value; }
    public ulong UInt64(string name, ulong value) { _bw.Write(value);  return value; }

    public T EnumU8<T>(int n, T value) where T : unmanaged, Enum => EnumU8(null, value);
    public T EnumU8<T>(string name, T value) where T : unmanaged, Enum
    {
        _bw.Write(SerdesUtil.EnumToByte(value));
        return value;
    }

    public T EnumU16<T>(int n, T value) where T : unmanaged, Enum => EnumU16(null, value);
    public T EnumU16<T>(string name, T value) where T : unmanaged, Enum
    {
        _bw.Write(SerdesUtil.EnumToUShort(value));
        return value;
    }

    public T EnumU32<T>(int n, T value) where T : unmanaged, Enum => EnumU32(null, value);
    public T EnumU32<T>(string name, T value) where T : unmanaged, Enum
    {
        _bw.Write(SerdesUtil.EnumToUInt(value));
        return value;
    }

    public Guid Guid(string name, Guid value)
    {
        _bw.Write(value.ToByteArray());
        return value;
    }

    public byte[] Bytes(string name, byte[] value, int n)
    {
        if (value != null && value.Length > 0)
            _bw.Write(value, 0, n);
        return value;
    }

#if NETSTANDARD2_1_OR_GREATER
        public void Bytes(string name, Span<byte> value)
        {
            if (value.Length > 0)
                _bw.Write(value);
        }
#endif

    public string NullTerminatedString(string name, string value)
    {
        value ??= string.Empty;
        var bytes = _stringToBytes(value);
        _bw.Write(bytes);
        _bw.Write((byte)0);
        return value;
    }

    public string FixedLengthString(string name, string value, int length)
    {
        value ??= string.Empty;
        var bytes = _stringToBytes(value);
        if (bytes.Length > length + 1)
            assertionFailed("Tried to write over-length string");

        _bw.Write(bytes);

        for (int i = bytes.Length; i < length; i++)
            _bw.Write((byte)0);

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
        list ??= initialiser?.Invoke(count) ?? new TTarget[count];
        for (int i = offset; i < count + offset; i++)
            serializer(i, list[i], this);
        return list;
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
        assertionFailed?.Invoke(formatted);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            disposeAction?.Invoke();
    }
    public void Dispose() => Dispose(true);
}