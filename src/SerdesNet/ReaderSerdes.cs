using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace SerdesNet;

/// <summary>
/// A serializer that reads data from a <see cref="BinaryReader"/> or byte array.
/// </summary>
public class ReaderSerdes : ISerdes
{
    readonly Action<string> _assertionFailed;
    readonly Action _disposeAction;
    readonly Func<byte[], string> _bytesToString;
    readonly BinaryReader _br;
    readonly long _maxOffset;

    /// <summary>
    /// Create a new ReaderSerdes based on a BinaryReader
    /// </summary>
    /// <param name="br">The binary reader to use for reading</param>
    /// <param name="maxLength">The maximum offset that should be accessed</param>
    /// <param name="bytesToString">A method to convert a sequence of bytes to a string using the desired encoding</param>
    /// <param name="assertionFailed">An optional callback to be invoked when an assertion failure occurs</param>
    /// <param name="disposeAction">An optional callback to be invoked when the <see cref="ReaderSerdes"/> is disposed</param>
    public ReaderSerdes(
        BinaryReader br,
        long maxLength,
        Func<byte[], string> bytesToString,
        Action<string> assertionFailed = null,
        Action disposeAction = null)
    {
        _br = br ?? throw new ArgumentNullException(nameof(br));
        _assertionFailed = assertionFailed;
        _disposeAction = disposeAction;
        _bytesToString = bytesToString ?? throw new ArgumentNullException(nameof(bytesToString));
        _maxOffset = Offset + maxLength;
    }

    /// <summary>
    /// Create a new ReaderSerdes based on a byte array
    /// </summary>
    /// <param name="bytes">The byte array to read from</param>
    /// <param name="bytesToString"></param>
    /// <param name="assertionFailed">An optional callback to be invoked when an assertion failure occurs</param>
    /// <param name="disposeAction">An optional callback to be invoked when the <see cref="ReaderSerdes"/> is disposed</param>
    public ReaderSerdes(
        byte[] bytes,
        Func<byte[], string> bytesToString,
        Action<string> assertionFailed = null,
        Action disposeAction = null)
    {
        var ms = new MemoryStream(bytes);
        _br = new BinaryReader(ms);
        _maxOffset = bytes.Length;
        _bytesToString = bytesToString ?? throw new ArgumentNullException(nameof(bytesToString));
        _assertionFailed = assertionFailed;
        _disposeAction = disposeAction;
    }

    /// <inheritdoc />
    public SerializerFlags Flags => SerializerFlags.Read;
    /// <inheritdoc />
    public long BytesRemaining => _maxOffset - Offset;
    /// <inheritdoc />
    public void Comment(string msg, bool inline) { }
    /// <inheritdoc />
    public void Begin(SerdesName name = default) { }
    /// <inheritdoc />
    public void End() { }
    /// <inheritdoc />
    public void NewLine() { }
    /// <inheritdoc />
    public long Offset => _br.BaseStream.Position;

    /// <inheritdoc />
    public void Seek(long newOffset) => _br.BaseStream.Seek(newOffset, SeekOrigin.Begin);

    /// <inheritdoc />
    public void Pad(int count, byte value) => Pad(null, count, value);

    /// <inheritdoc />
    public void Pad(SerdesName name, int count, byte value)
    {
        var bytes = _br.ReadBytes(count);
        if (bytes.Length < count)
            throw new EndOfStreamException();

        foreach (var b in bytes)
            if (b != value)
                Assert(false, $"Unexpected value \"{b}\" found in repeating byte pattern (expected {value}");
    }

    /// <inheritdoc />
    public sbyte Int8(SerdesName name, sbyte value) => _br.ReadSByte();
    /// <inheritdoc />
    public short Int16(SerdesName name, short value) => _br.ReadInt16();
    /// <inheritdoc />
    public int Int32(SerdesName name, int value) => _br.ReadInt32();
    /// <inheritdoc />
    public long Int64(SerdesName name, long value) => _br.ReadInt64();
    /// <inheritdoc />
    public byte UInt8(SerdesName name, byte value) => _br.ReadByte();
    /// <inheritdoc />
    public ushort UInt16(SerdesName name, ushort value) => _br.ReadUInt16();
    /// <inheritdoc />
    public uint UInt32(SerdesName name, uint value) => _br.ReadUInt32();
    /// <inheritdoc />
    public ulong UInt64(SerdesName name, ulong value) => _br.ReadUInt64();

    /// <inheritdoc />
    public Guid Guid(SerdesName name, Guid value)
    {
        var bytes = _br.ReadBytes(16);
        if (bytes.Length < 16)
            throw new EndOfStreamException();
        return new Guid(bytes);
    }

    /// <inheritdoc />
    public byte[] Bytes(SerdesName name, byte[] value, int n)
    {
        var v = _br.ReadBytes(n);
        if (v.Length < n)
            throw new EndOfStreamException();

        return v;
    }

#if NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc />
    public void Bytes(SerdesName name, Span<byte> value)
    {
        int n = _br.Read(value);
        if (value.Length < n)
            throw new EndOfStreamException();
    }
#endif

    /// <inheritdoc />
    public string NullTerminatedString(SerdesName name, string value)
    {
        var bytes = new List<byte>();
        for (;;)
        {
            var b = _br.ReadByte();
            if (b == 0)
                break;

            bytes.Add(b);
        }

        return _bytesToString([.. bytes]);
    }

    /// <inheritdoc />
    public string FixedLengthString(SerdesName name, string value, int length)
    {
        var bytes = _br.ReadBytes(length);
        if (bytes.Length < length)
            throw new EndOfStreamException();

        var str = _bytesToString(bytes);
        return str.TrimEnd('\0');
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
    /// The actual implementation of the Dispose method.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            _disposeAction?.Invoke();
    }

    /// <inheritdoc />
    public void Dispose() => Dispose(true);
}