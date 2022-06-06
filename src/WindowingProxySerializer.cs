using System;
using System.Collections.Generic;

namespace SerdesNet
{
    /// <summary>
    /// Delegates reading/writing to an underlying serializer while
    /// manipulating Offset and BytesRemaining to simulate a subset
    /// of the underlying stream/buffer.
    /// </summary>
    public class WindowingProxySerializer : ISerializer
    {
        readonly ISerializer _s;
        readonly int? _size;
        readonly long _relativeOffset;

        public WindowingProxySerializer(ISerializer s, int? size)
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

        public void Dispose() { }
        public SerializerFlags Flags => _s.Flags;
        public long Offset => _s.Offset - _relativeOffset;
        public long BytesRemaining => _size.HasValue ? _size.Value - Offset : _s.BytesRemaining;
        public void Comment(string comment, bool inline) => _s.Comment(comment);
        public void Begin(string name = null) => _s.Begin(name);
        public void End() => _s.End();
        public void NewLine() => _s.NewLine();
        public void Seek(long offset) => _s.Seek(offset + _relativeOffset);
        public void Assert(bool condition, string message) => _s.Assert(condition, message);

        public void Pad(int count, byte value) => _s.Pad(count, value);
        public void Pad(string name, int count, byte value) => _s.Pad(name, count, value);

        public sbyte Int8(int n, sbyte value, sbyte defaultValue = 0) => _s.Int8(n, value, defaultValue);
        public short Int16(int n, short value, short defaultValue = 0) => _s.Int16(n, value, defaultValue);
        public int Int32(int n, int value, int defaultValue = 0) => _s.Int32(n, value, defaultValue);
        public long Int64(int n, long value, long defaultValue = 0) => _s.Int64(n, value, defaultValue);
        public byte UInt8(int n, byte value, byte defaultValue = 0) => _s.UInt8(n, value, defaultValue);
        public ushort UInt16(int n, ushort value, ushort defaultValue = 0) => _s.UInt16(n, value, defaultValue);
        public uint UInt32(int n, uint value, uint defaultValue = 0) => _s.UInt32(n, value, defaultValue);
        public ulong UInt64(int n, ulong value, ulong defaultValue = 0) => _s.UInt64(n, value, defaultValue);

        public sbyte Int8(string name, sbyte value, sbyte defaultValue = 0) => _s.Int8(name, value, defaultValue);
        public short Int16(string name, short value, short defaultValue = 0) => _s.Int16(name, value, defaultValue);
        public int Int32(string name, int value, int defaultValue = 0) => _s.Int32(name, value, defaultValue);
        public long Int64(string name, long value, long defaultValue = 0) => _s.Int64(name, value, defaultValue);
        public byte UInt8(string name, byte value, byte defaultValue = 0) => _s.UInt8(name, value, defaultValue);
        public ushort UInt16(string name, ushort value, ushort defaultValue = 0) => _s.UInt16(name, value, defaultValue);
        public uint UInt32(string name, uint value, uint defaultValue = 0) => _s.UInt32(name, value, defaultValue);
        public ulong UInt64(string name, ulong value, ulong defaultValue = 0) => _s.UInt64(name, value, defaultValue);

        public T EnumU8<T>(int n, T value) where T : unmanaged, Enum => _s.EnumU8(n, value);
        public T EnumU16<T>(int n, T value) where T : unmanaged, Enum => _s.EnumU16(n, value);
        public T EnumU32<T>(int n, T value) where T : unmanaged, Enum => _s.EnumU32(n, value);
        public T EnumU8<T>(string name, T value) where T : unmanaged, Enum => _s.EnumU8(name, value);
        public T EnumU16<T>(string name, T value) where T : unmanaged, Enum => _s.EnumU16(name, value);
        public T EnumU32<T>(string name, T value) where T : unmanaged, Enum => _s.EnumU32(name, value);

        public Guid Guid(string name, Guid value) => _s.Guid(name, value);
        public byte[] Bytes(string name, byte[] value, int length) => _s.Bytes(name, value, length);
        public string NullTerminatedString(string name, string value) => _s.NullTerminatedString(name, value);
        public string FixedLengthString(string name, string value, int length) => _s.FixedLengthString(name, value, length);

        public IList<TTarget> List<TTarget>(string name,
            IList<TTarget> list,
            int count,
            SerdesMethod<TTarget> serdes,
            Func<int, IList<TTarget>> initialiser = null) => _s.List(name, list, count, serdes, initialiser);

        public IList<TTarget> List<TTarget>(string name,
            IList<TTarget> list,
            int count,
            int offset,
            SerdesMethod<TTarget> serdes,
            Func<int, IList<TTarget>> initialiser = null) => _s.List(name, list, count, offset, serdes, initialiser);

        public IList<TTarget> List<TTarget, TContext>(string name,
            IList<TTarget> list,
            TContext context,
            int count,
            SerdesContextMethod<TTarget, TContext> serdes,
            Func<int, IList<TTarget>> initialiser = null) => _s.List(name, list, context, count, serdes, initialiser);

        public IList<TTarget> List<TTarget, TContext>(string name,
            IList<TTarget> list,
            TContext context,
            int count,
            int offset,
            SerdesContextMethod<TTarget, TContext> serdes,
            Func<int, IList<TTarget>> initialiser = null) => _s.List(name, list, context, count, offset, serdes, initialiser);
    }
}