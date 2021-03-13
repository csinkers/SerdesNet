using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace SerdesNet
{
    public class GenericBinaryWriter : ISerializer
    {
        readonly Action<string> _assertionFailed;
        readonly Action _disposeAction;
        readonly Func<string, byte[]> _stringToBytes;
        readonly BinaryWriter _bw;
        long _offset;

        public GenericBinaryWriter(
            BinaryWriter bw,
            Func<string, byte[]> stringToBytes,
            Action<string> assertionFailed = null,
            Action disposeAction = null)
        {
            _bw = bw ?? throw new ArgumentNullException(nameof(bw));
            _stringToBytes = stringToBytes ?? throw new ArgumentNullException(nameof(stringToBytes));
            _assertionFailed = assertionFailed;
            _disposeAction = disposeAction;
        }

        public SerializerFlags Flags => SerializerFlags.Write;
        public long BytesRemaining => long.MaxValue;
        public void Comment(string msg) { }
        public void Begin(string name = null) { }
        public void End() { }
        public void NewLine() { }
        public void Check() => Assert(_offset == _bw.BaseStream.Position, "Mismatch between internal offset and stream position");
        public bool IsComplete() => false;
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

        public void Pad(int bytes) => RepeatU8(null, 0, bytes);
        public sbyte Int8(string name, sbyte value, sbyte _ = 0) { _bw.Write(value); _offset += 1L; DebugCheck(); return value; }
        public short Int16(string name, short value, short _ = 0) { _bw.Write(value); _offset += 2L; DebugCheck(); return value; }
        public int Int32(string name, int value, int _ = 0) { _bw.Write(value); _offset += 4L; DebugCheck(); return value; }
        public long Int64(string name, long value, long _ = 0) { _bw.Write(value); _offset += 8L; DebugCheck(); return value; }
        public byte UInt8(string name, byte value, byte _ = 0) { _bw.Write(value); _offset += 1L; DebugCheck(); return value; }
        public ushort UInt16(string name, ushort value, ushort _ = 0) { _bw.Write(value); _offset += 2L; DebugCheck(); return value; }
        public uint UInt32(string name, uint value, uint _ = 0) { _bw.Write(value); _offset += 4L; DebugCheck(); return value; }
        public ulong UInt64(string name, ulong value, ulong _ = 0) { _bw.Write(value); _offset += 8L; DebugCheck(); return value; }
        public T EnumU8<T>(string name, T value) where T : unmanaged, Enum
        {
            _bw.Write(SerdesUtil.EnumToByte(value));
            _offset += 1L;
            DebugCheck();
            return value;
        }

        public T EnumU16<T>(string name, T value) where T : unmanaged, Enum
        {
            _bw.Write(SerdesUtil.EnumToUShort(value));
            _offset += 2L;
            DebugCheck();
            return value;
        }

        public T EnumU32<T>(string name, T value) where T : unmanaged, Enum
        {
            _bw.Write(SerdesUtil.EnumToUInt(value));
            _offset += 4L;
            DebugCheck();
            return value;
        }

        public Guid Guid(string name, Guid value)
        {
            _bw.Write(value.ToByteArray());
            _offset += 16L;
            DebugCheck();
            return value;
        }

        public byte[] Bytes(string name, byte[] value, int n)
        {
            if (value != null && value.Length > 0)
                _bw.Write(value, 0, n);
            _offset += value?.Length ?? 0;
            DebugCheck();
            return value;
        }

        public string NullTerminatedString(string name, string value)
        {
            var bytes = _stringToBytes(value);
            _bw.Write(bytes);
            _bw.Write((byte)0);
            _offset += bytes.Length + 1; // add 2 bytes for the null terminator
            DebugCheck();
            return value;
        }

        public string FixedLengthString(string name, string value, int length)
        {
            var bytes = _stringToBytes(value ?? "");
            if (bytes.Length > length + 1) _assertionFailed("Tried to write over-length string");
            _bw.Write(bytes);
            for (int i = bytes.Length; i < length; i++)
                _bw.Write((byte)0);
            _offset += length; // Pad out to the full length
            DebugCheck();
            return value;
        }

        public void RepeatU8(string name, byte v, int length)
        {
            for (int i = 0; i < length; i++)
                _bw.Write(v);
            _offset += length;
            DebugCheck();
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
            DebugCheck();
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
            DebugCheck();
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
            if (disposing)
                _disposeAction?.Invoke();
        }
        public void Dispose() => Dispose(true);

        [Conditional("DEBUG")] void DebugCheck() => Check();
    }
}
