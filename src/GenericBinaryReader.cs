using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace SerdesNet
{
    public class GenericBinaryReader : ISerializer
    {
        readonly Action<string> _assertionFailed;
        readonly Func<byte[], string> _bytesToString;
        readonly Stack<int> _versionStack = new Stack<int>();
        readonly BinaryReader _br;
        readonly long _maxOffset;
        long _offset;

        public GenericBinaryReader(BinaryReader br, long maxLength, Func<byte[], string> bytesToString, Action<string> assertionFailed = null)
        {
            _br = br ?? throw new ArgumentNullException(nameof(br));
            _assertionFailed = assertionFailed;
            _bytesToString = bytesToString ?? throw new ArgumentNullException(nameof(bytesToString));
            _offset = br.BaseStream.Position;
            _maxOffset = _offset + maxLength;
        }

        public SerializerMode Mode => SerializerMode.Reading;
        public void PushVersion(int version) => _versionStack.Push(version);
        public int PopVersion() => _versionStack.Count == 0 ? 0 : _versionStack.Pop();
        public long BytesRemaining => _maxOffset - _offset;
        public void Comment(string msg) { }
        public void NewLine() { }
        public long Offset
        {
            get
            {
                Check();
                return _offset;
            }
        }

        public void Check()
        {
            Assert(_offset == _br.BaseStream.Position);
            if (_br.BaseStream.Position > _maxOffset)
                _assertionFailed("Buffer overrun in binary reader");
        }

        public bool IsComplete() => _br.BaseStream.Position >= _maxOffset;

        public void Seek(long newOffset)
        {
            _br.BaseStream.Seek(newOffset, SeekOrigin.Begin);
            _offset = newOffset;
        }

        public sbyte Int8(string name, sbyte existing, sbyte _ = 0) { _offset += 1L; return _br.ReadSByte(); }
        public short Int16(string name, short existing, short _ = 0) { _offset += 2L; return _br.ReadInt16(); }
        public int Int32(string name, int existing, int _ = 0) { _offset += 4L; return _br.ReadInt32(); }
        public long Int64(string name, long existing, long _ = 0) { _offset += 8L; return _br.ReadInt64(); }
        public byte UInt8(string name, byte existing, byte _ = 0) { _offset += 1L; return _br.ReadByte(); }
        public ushort UInt16(string name, ushort existing, ushort _ = 0) { _offset += 2L; return _br.ReadUInt16(); }
        public uint UInt32(string name, uint existing, uint _ = 0) { _offset += 4L; return _br.ReadUInt32(); }
        public ulong UInt64(string name, ulong existing, ulong _ = 0) { _offset += 8L; return _br.ReadUInt64(); }
        public T EnumU8<T>(string name, T existing) where T : struct, Enum { _offset += 1L; return (T)(object)_br.ReadByte(); }
        public T EnumU16<T>(string name, T existing) where T : struct, Enum { _offset += 2L; return (T)(object)_br.ReadUInt16(); }
        public T EnumU32<T>(string name, T existing) where T : struct, Enum { _offset += 4L; return (T)(object)_br.ReadUInt32(); }

        public Guid Guid(string name, Guid existing)
        {
            _offset += 16L;
            var bytes = _br.ReadBytes(16);
            if (bytes.Length < 16)
                throw new EndOfStreamException();
            return new Guid(bytes);
        }

        public T Object<T>(string name, T existing, Func<int, T, ISerializer, T> serdes) => serdes(0, existing, this);
        public T Object<T, TContext>(string name, T existing, TContext context, Func<int, T, TContext, ISerializer, T> serdes) => serdes(0, existing, context, this);

        public TMemory Transform<TPersistent, TMemory>(
                string name,
                TMemory existing,
                Func<string, TPersistent, ISerializer, TPersistent> serializer,
                IConverter<TPersistent, TMemory> converter) =>
            converter.FromNumeric(serializer(name, converter.ToNumeric(existing), this));

        public T TransformEnumU8<T>(string name, T existing, IConverter<byte, T> converter) 
            => converter.FromNumeric(UInt8(name, converter.ToNumeric(existing)));
        public T TransformEnumU16<T>(string name, T existing, IConverter<ushort, T> converter) 
            => converter.FromNumeric(UInt16(name, converter.ToNumeric(existing)));
        public T TransformEnumU32<T>(string name, T existing, IConverter<uint, T> converter) 
            => converter.FromNumeric(UInt32(name, converter.ToNumeric(existing)));

        public byte[] ByteArray(string name, byte[] existing, int n)
        {
            var v = _br.ReadBytes(n);
            if (v.Length < n)
                throw new EndOfStreamException();
            _offset += v.Length;
            return v;
        }

        public byte[] ByteArrayHex(string name, byte[] existing, int n)
        {
            var v = _br.ReadBytes(n);
            if (v.Length < n)
                throw new EndOfStreamException();
            _offset += v.Length;
            return v;
        }

        public string NullTerminatedString(string name, string existing)
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

        public string FixedLengthString(string name, string existing, int length)
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

        void Assert(bool result, string message = null, [CallerMemberName] string function = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (result)
                return;

            var formatted = $"Assertion failed: {message} at {function} in {file}:{line}";
            _assertionFailed?.Invoke(formatted);
        }
    }
}
