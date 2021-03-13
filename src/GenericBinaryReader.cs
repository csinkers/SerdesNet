using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace SerdesNet
{
    public class GenericBinaryReader : ISerializer
    {
        readonly Action<string> _assertionFailed;
        readonly Action _disposeAction;
        readonly Func<byte[], string> _bytesToString;
        readonly BinaryReader _br;
        readonly long _maxOffset;
        long _offset;

        public GenericBinaryReader(
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
            _offset = br.BaseStream.Position;
            _maxOffset = _offset + maxLength;
        }

        public SerializerFlags Flags => SerializerFlags.Read;
        public long BytesRemaining => _maxOffset - _offset;
        public void Comment(string msg) { }
        public void Begin(string name = null) { }
        public void End() { }
        public void NewLine() { }
        public long Offset { get { Check(); return _offset; } }

        public void Check()
        {
            Assert(_offset == _br.BaseStream.Position);
            Assert(_maxOffset >=  _br.BaseStream.Position, "Buffer overrun in binary reader");
        }

        public bool IsComplete() => _br.BaseStream.Position >= _maxOffset;

        public void Seek(long newOffset)
        {
            _br.BaseStream.Seek(newOffset, SeekOrigin.Begin);
            _offset = newOffset;
        }

        public void Pad(int bytes) => RepeatU8(null, 0, bytes);
        public sbyte Int8(string name, sbyte value, sbyte _ = 0) { _offset += 1L; return _br.ReadSByte(); }
        public short Int16(string name, short value, short _ = 0) { _offset += 2L; return _br.ReadInt16(); }
        public int Int32(string name, int value, int _ = 0) { _offset += 4L; return _br.ReadInt32(); }
        public long Int64(string name, long value, long _ = 0) { _offset += 8L; return _br.ReadInt64(); }
        public byte UInt8(string name, byte value, byte _ = 0) { _offset += 1L; return _br.ReadByte(); }
        public ushort UInt16(string name, ushort value, ushort _ = 0) { _offset += 2L; return _br.ReadUInt16(); }
        public uint UInt32(string name, uint value, uint _ = 0) { _offset += 4L; return _br.ReadUInt32(); }
        public ulong UInt64(string name, ulong value, ulong _ = 0) { _offset += 8L; return _br.ReadUInt64(); }
        public T EnumU8<T>(string name, T value) where T : unmanaged, Enum { _offset += 1L; return SerdesUtil.ByteToEnum<T>(_br.ReadByte()); }
        public T EnumU16<T>(string name, T value) where T : unmanaged, Enum { _offset += 2L; return SerdesUtil.UShortToEnum<T>((_br.ReadUInt16())); }
        public T EnumU32<T>(string name, T value) where T : unmanaged, Enum { _offset += 4L; return SerdesUtil.UIntToEnum<T>(_br.ReadUInt32()); }

        public Guid Guid(string name, Guid value)
        {
            _offset += 16L;
            var bytes = _br.ReadBytes(16);
            if (bytes.Length < 16)
                throw new EndOfStreamException();
            return new Guid(bytes);
        }

        public T Object<T>(string name, T value, Func<int, T, ISerializer, T> serdes) => serdes(0, value, this);
        public T Object<T, TContext>(string name, T value, TContext context, Func<int, T, TContext, ISerializer, T> serdes) => serdes(0, value, context, this);
        public void Object(string name, Action<ISerializer> serdes) => serdes(this);

        public TMemory Transform<TPersistent, TMemory>(
                string name,
                TMemory value,
                Func<string, TPersistent, ISerializer, TPersistent> serializer,
                IConverter<TPersistent, TMemory> converter) =>
            converter.FromNumeric(serializer(name, converter.ToNumeric(value), this));

        public T TransformEnumU8<T>(string name, T value, IConverter<byte, T> converter) 
            => converter.FromNumeric(UInt8(name, converter.ToNumeric(value)));
        public T TransformEnumU16<T>(string name, T value, IConverter<ushort, T> converter) 
            => converter.FromNumeric(UInt16(name, converter.ToNumeric(value)));
        public T TransformEnumU32<T>(string name, T value, IConverter<uint, T> converter) 
            => converter.FromNumeric(UInt32(name, converter.ToNumeric(value)));

        public byte[] Bytes(string name, byte[] value, int n)
        {
            var v = _br.ReadBytes(n);
            if (v.Length < n)
                throw new EndOfStreamException();
            _offset += v.Length;
            return v;
        }

        public string NullTerminatedString(string name, string value)
        {
            var bytes = new List<byte>();
            for (;;)
            {
                var b = _br.ReadByte();
                _offset++;
                if (b == 0)
                    break;
                bytes.Add(b);
            }

            return _bytesToString(bytes.ToArray());
        }

        public string FixedLengthString(string name, string value, int length)
        {
            var bytes = _br.ReadBytes(length);
            if (bytes.Length < length)
                throw new EndOfStreamException();
            var str = _bytesToString(bytes);
            _offset += length;
            Assert(_offset == _br.BaseStream.Position);
            return str.TrimEnd('\0');
        }

        public void RepeatU8(string name, byte v, int length)
        {
            var bytes = _br.ReadBytes(length);
            if (bytes.Length < length)
                throw new EndOfStreamException();

            foreach (var b in bytes)
                if (b != v)
                    Assert(false, $"Unexpected value \"{b}\" found in repeating byte pattern (expected {v}");
            _offset += length;
        }

        public IList<TTarget> List<TTarget>(
            string name, IList<TTarget> list, int count,
            Func<int, TTarget, ISerializer, TTarget> serializer,
            Func<int, IList<TTarget>> initialiser = null)
            => List(name, list, count, 0, serializer, initialiser);

        public IList<TTarget> List<TTarget, TContext>(
            string name, IList<TTarget> list, TContext context, int count,
            Func<int, TTarget, TContext, ISerializer, TTarget> serializer,
            Func<int, IList<TTarget>> initialiser = null)
            => List(name, list, context, count, 0, serializer, initialiser);

        public IList<TTarget> List<TTarget>(
            string name,
            IList<TTarget> list,
            int count,
            int offset,
            Func<int, TTarget, ISerializer, TTarget> serdes,
            Func<int, IList<TTarget>> initialiser = null)
        {
            list = list ?? initialiser?.Invoke(count) ?? new List<TTarget>();
            for (int i = offset; i < offset + count; i++)
            {
                var x = serdes(i, default, this);

                if (list.Count <= i)
                    while (list.Count <= i)
                        list.Add(x);
                else
                    list[i] = x;
            }
            return list;
        }

        public IList<TTarget> List<TTarget, TContext>(
            string name,
            IList<TTarget> list,
            TContext context,
            int count,
            int offset,
            Func<int, TTarget, TContext, ISerializer, TTarget> serdes,
            Func<int, IList<TTarget>> initialiser = null)
        {
            list = list ?? initialiser?.Invoke(count) ?? new List<TTarget>();
            for (int i = offset; i < offset + count; i++)
            {
                var x = serdes(i, default, context, this);

                if (list.Count <= i)
                    while (list.Count <= i)
                        list.Add(x);
                else
                    list[i] = x;
            }
            return list;
        }

        void ISerializer.Assert(bool condition, string message) => Assert(condition, message);
        void Assert(bool condition, string message = null, [CallerMemberName] string function = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (condition)
                return;

            var formatted = $"Assertion failed: {message} at {function} in {file}:{line}";
            _assertionFailed?.Invoke(formatted);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
                _disposeAction?.Invoke();
        }
        public void Dispose() => Dispose(true);
    }
}
