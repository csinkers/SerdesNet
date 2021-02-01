using System;
using System.Collections.Generic;

namespace SerdesNet
{
    /// <summary>
    /// Delegates reading/writing to an underlying serializer while
    /// manipulating Offset and BytesRemaining to simulate a window of the underlying stream/buffer.
    /// </summary>
    public class WindowingFacadeSerializer : ISerializer
    {
        readonly ISerializer _s;
        readonly int? _size;
        readonly long _relativeOffset;

        public WindowingFacadeSerializer(ISerializer s, int? size)
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
        public void Comment(string comment) => _s.Comment(comment);
        public void Begin(string name = null) => _s.Begin(name);
        public void End() => _s.End();
        public void NewLine() => _s.NewLine();
        public void Seek(long offset) => _s.Seek(offset + _relativeOffset);
        public void Check()
        {
            _s.Assert(!_size.HasValue || Offset <= _size, "Buffer overrun in windowing reader");
            _s.Check();
        }

        public void Assert(bool condition, string message) => _s.Assert(condition, message);
        public bool IsComplete() => BytesRemaining <= 0;

        public void Pad(int bytes) => _s.Pad(bytes);
        public sbyte Int8(string name, sbyte existing, sbyte defaultValue = 0) => _s.Int8(name, existing, defaultValue);
        public short Int16(string name, short existing, short defaultValue = 0) => _s.Int16(name, existing, defaultValue);
        public int Int32(string name, int existing, int defaultValue = 0) => _s.Int32(name, existing, defaultValue);
        public long Int64(string name, long existing, long defaultValue = 0) => _s.Int64(name, existing, defaultValue);
        public byte UInt8(string name, byte existing, byte defaultValue = 0) => _s.UInt8(name, existing, defaultValue);
        public ushort UInt16(string name, ushort existing, ushort defaultValue = 0) => _s.UInt16(name, existing, defaultValue);
        public uint UInt32(string name, uint existing, uint defaultValue = 0) => _s.UInt32(name, existing, defaultValue);
        public ulong UInt64(string name, ulong existing, ulong defaultValue = 0) => _s.UInt64(name, existing, defaultValue);
        public T EnumU8<T>(string name, T existing) where T : struct, Enum => _s.EnumU8(name, existing);
        public T EnumU16<T>(string name, T existing) where T : struct, Enum => _s.EnumU16(name, existing);
        public T EnumU32<T>(string name, T existing) where T : struct, Enum => _s.EnumU32(name, existing);
        public T Transform<TNumeric, T>(string name, T existing, Func<string, TNumeric, ISerializer, TNumeric> serializer, IConverter<TNumeric, T> converter) 
            => _s.Transform(name, existing, serializer, converter);
        public T TransformEnumU8<T>(string name, T existing, IConverter<byte, T> converter) => _s.TransformEnumU8(name, existing, converter);
        public T TransformEnumU16<T>(string name, T existing, IConverter<ushort, T> converter) => _s.TransformEnumU16(name, existing, converter);
        public T TransformEnumU32<T>(string name, T existing, IConverter<uint, T> converter) => _s.TransformEnumU32(name, existing, converter);
        public Guid Guid(string name, Guid existing) => _s.Guid(name, existing);
        public byte[] ByteArray(string name, byte[] existing, int length) => _s.ByteArray(name, existing, length);
        public string NullTerminatedString(string name, string existing) => _s.NullTerminatedString(name, existing);
        public string FixedLengthString(string name, string existing, int length) => _s.FixedLengthString(name, existing, length);
        public void RepeatU8(string name, byte value, int count) => _s.RepeatU8(name, value, count);
        public T Object<T>(string name, T existing, Func<int, T, ISerializer, T> serdes) => _s.Object(name, existing, serdes);

        public T Object<T, TContext>(string name, T existing, TContext context, Func<int, T, TContext, ISerializer, T> serdes) 
            => _s.Object(name, existing, context, serdes);

        public void Object(string name, Action<ISerializer> serdes) => serdes(this);

        public IList<TTarget> List<TTarget>(string name,
            IList<TTarget> list,
            int count,
            Func<int, TTarget, ISerializer, TTarget> serdes,
            Func<int, IList<TTarget>> initialiser = null) => _s.List(name, list, count, serdes, initialiser);

        public IList<TTarget> List<TTarget>(string name,
            IList<TTarget> list,
            int count,
            int offset,
            Func<int, TTarget, ISerializer, TTarget> serdes,
            Func<int, IList<TTarget>> initialiser = null) => _s.List(name, list, count, offset, serdes, initialiser);

        public IList<TTarget> List<TTarget, TContext>(string name,
            IList<TTarget> list,
            TContext context,
            int count,
            Func<int, TTarget, TContext, ISerializer, TTarget> serdes,
            Func<int, IList<TTarget>> initialiser = null) => _s.List(name, list, context, count, serdes, initialiser);

        public IList<TTarget> List<TTarget, TContext>(string name,
            IList<TTarget> list,
            TContext context,
            int count,
            int offset,
            Func<int, TTarget, TContext, ISerializer, TTarget> serdes,
            Func<int, IList<TTarget>> initialiser = null) => _s.List(name, list, context, count, offset, serdes, initialiser);
    }
}