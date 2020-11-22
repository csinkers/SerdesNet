using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SerdesNet
{
    public class GenericBinaryWriter : ISerializer
    {
        readonly Action<string> _assertionFailed;
        readonly Func<string, byte[]> _stringToBytes;
        readonly BinaryWriter _bw;
        Stack<int> _versionStack;
        long _offset;

        public GenericBinaryWriter(BinaryWriter bw, Func<string, byte[]> stringToBytes, Action<string> assertionFailed = null)
        {
            _bw = bw ?? throw new ArgumentNullException(nameof(bw));
            _stringToBytes = stringToBytes ?? throw new ArgumentNullException(nameof(stringToBytes));
            _assertionFailed = assertionFailed;
        }

        public SerializerMode Mode => SerializerMode.Writing;
        public void PushVersion(int version) => (_versionStack = _versionStack ?? new Stack<int>()).Push(version);
        public int PopVersion() => _versionStack == null || _versionStack.Count == 0 ? 0 : _versionStack.Pop();
        public long BytesRemaining => long.MaxValue;
        public void Comment(string msg) { }
        public void NewLine() { }
        public void Check() => Assert(_offset == _bw.BaseStream.Position);
        public bool IsComplete() => false;
        public T Object<T>(string name, T existing, Func<int, T, ISerializer, T> serdes) => serdes(0, existing, this);
        public T Object<T, TContext>(string name, T existing, TContext context, Func<int, T, TContext, ISerializer, T> serdes) => serdes(0, existing, context, this);

        public TMemory Transform<TPersistent, TMemory>(
                string name,
                TMemory existing,
                Func<string, TPersistent, ISerializer, TPersistent> serializer,
                IConverter<TPersistent, TMemory> converter) =>
            converter.FromNumeric(serializer(name, converter.ToNumeric(existing), this));

        public T TransformEnumU8<T>(string name, T existing, IConverter<byte, T> converter) 
            => converter.FromNumeric(UInt8(name, converter.ToNumeric(existing), 0));
        public T TransformEnumU16<T>(string name, T existing, IConverter<ushort, T> converter) 
            => converter.FromNumeric(UInt16(name, converter.ToNumeric(existing), 0));
        public T TransformEnumU32<T>(string name, T existing, IConverter<uint, T> converter) 
            => converter.FromNumeric(UInt32(name, converter.ToNumeric(existing), 0));

        public long Offset
        {
            get
            {
                Assert(_offset == _bw.BaseStream.Position);
                return _offset;
            }
        }

        public void Seek(long newOffset)
        {
            _bw.Seek((int)newOffset, SeekOrigin.Begin);
            _offset = newOffset;
        }

        public sbyte Int8(string name, sbyte existing, sbyte _ = 0) { _bw.Write(existing); _offset += 1L; return existing; }
        public short Int16(string name, short existing, short _ = 0) { _bw.Write(existing); _offset += 2L; return existing; }
        public int Int32(string name, int existing, int _ = 0) { _bw.Write(existing); _offset += 4L; return existing; }
        public long Int64(string name, long existing, long _ = 0) { _bw.Write(existing); _offset += 8L; return existing; }
        public byte UInt8(string name, byte existing, byte _ = 0) { _bw.Write(existing); _offset += 1L; return existing; }
        public ushort UInt16(string name, ushort existing, ushort _ = 0) { _bw.Write(existing); _offset += 2L; return existing; }
        public uint UInt32(string name, uint existing, uint _ = 0) { _bw.Write(existing); _offset += 4L; return existing; }
        public ulong UInt64(string name, ulong existing, ulong _ = 0) { _bw.Write(existing); _offset += 8L; return existing; }
        public T EnumU8<T>(string name, T existing) where T : struct, Enum
        {
            _bw.Write((byte)(object)existing);
            _offset += 1L;
            return existing;
        }

        public T EnumU16<T>(string name, T existing) where T : struct, Enum
        {
            _bw.Write((ushort)(object)existing);
            _offset += 2L;
            return existing;
        }

        public T EnumU32<T>(string name, T existing) where T : struct, Enum
        {
            _bw.Write((uint)(object)existing);
            _offset += 4L;
            return existing;
        }

        public Guid Guid(string name, Guid existing)
        {
            var v = existing;
            _bw.Write(v.ToByteArray());
            _offset += 16L;
            return existing;
        }

        public byte[] ByteArray(string name, byte[] existing, int n)
        {
            var v = existing;
            _bw.Write(v);
            _offset += v.Length;
            return existing;
        }

        public byte[] ByteArrayHex(string name, byte[] existing, int n)
        {
            var v = existing;
            _bw.Write(v);
            _offset += v.Length;
            return existing;
        }

        public string NullTerminatedString(string name, string existing)
        {
            var v = existing;
            var bytes = _stringToBytes(v);
            _bw.Write(bytes);
            _bw.Write((byte)0);
            _offset += bytes.Length + 1; // add 2 bytes for the null terminator
            return existing;
        }

        public string FixedLengthString(string name, string existing, int length)
        {
            var bytes = _stringToBytes(existing ?? "");
            if (bytes.Length > length + 1) _assertionFailed("Tried to write overlength string");
            _bw.Write(bytes);
            _bw.Write(Enumerable.Repeat((byte)0, length - bytes.Length).ToArray());
            _offset += length; // Pad out to the full length
            return existing;
        }

        public void RepeatU8(string name, byte v, int length)
        {
            _bw.Write(Enumerable.Repeat(v, length).ToArray());
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
            Func<int, TTarget, ISerializer, TTarget> serializer,
            Func<int, IList<TTarget>> initialiser = null)
        {
            list = list ?? initialiser?.Invoke(count) ?? new TTarget[count];
            for (int i = offset; i < count + offset; i++)
                serializer(i, list[i], this);
            return list;
        }

        public IList<TTarget> List<TTarget, TContext>(
            string name,
            IList<TTarget> list,
            TContext context,
            int count,
            int offset,
            Func<int, TTarget, TContext, ISerializer, TTarget> serializer,
            Func<int, IList<TTarget>> initialiser = null)
        {
            list = list ?? initialiser?.Invoke(count) ?? new TTarget[count];
            for (int i = offset; i < count + offset; i++)
                serializer(i, list[i], context, this);
            return list;
        }

        void Assert(bool result, string message = null, [CallerMemberName] string function = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (result)
                return;

            var formatted = $"Assertion failed: {message} at {function} in {file}:{line}";
            _assertionFailed?.Invoke(formatted);
        }

        protected virtual void Dispose(bool disposing) { }
        public void Dispose() => Dispose(true);
    }
}
