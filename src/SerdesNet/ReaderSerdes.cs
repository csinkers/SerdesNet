using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace SerdesNet
{
    public class ReaderSerdes : ISerdes
    {
        readonly Action<string> _assertionFailed;
        readonly Action _disposeAction;
        readonly Func<byte[], string> _bytesToString;
        readonly BinaryReader _br;
        readonly long _maxOffset;

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

        public SerializerFlags Flags => SerializerFlags.Read;
        public long BytesRemaining => _maxOffset - Offset;
        public void Comment(string msg, bool inline) { }
        public void Begin(string name = null) { }
        public void End() { }
        public void NewLine() { }
        public long Offset => _br.BaseStream.Position;

        public void Seek(long newOffset)
        {
            _br.BaseStream.Seek(newOffset, SeekOrigin.Begin);
        }

        public void Pad(int count, byte value) => Pad(null, count, value);
        public void Pad(string name, int count, byte value)
        {
            var bytes = _br.ReadBytes(count);
            if (bytes.Length < count)
                throw new EndOfStreamException();

            foreach (var b in bytes)
                if (b != value)
                    Assert(false, $"Unexpected value \"{b}\" found in repeating byte pattern (expected {value}");
        }

        public sbyte Int8(int n, sbyte value) => _br.ReadSByte();
        public short Int16(int n, short value) => _br.ReadInt16();
        public int Int32(int n, int value) => _br.ReadInt32();
        public long Int64(int n, long value) => _br.ReadInt64();
        public byte UInt8(int n, byte value) => _br.ReadByte();
        public ushort UInt16(int n, ushort value) => _br.ReadUInt16();
        public uint UInt32(int n, uint value) => _br.ReadUInt32();
        public ulong UInt64(int n, ulong value) => _br.ReadUInt64();

        public sbyte Int8(string name, sbyte value) => _br.ReadSByte();
        public short Int16(string name, short value) => _br.ReadInt16();
        public int Int32(string name, int value) => _br.ReadInt32();
        public long Int64(string name, long value) => _br.ReadInt64();
        public byte UInt8(string name, byte value) => _br.ReadByte();
        public ushort UInt16(string name, ushort value) => _br.ReadUInt16();
        public uint UInt32(string name, uint value) => _br.ReadUInt32();
        public ulong UInt64(string name, ulong value) => _br.ReadUInt64();

        public T EnumU8<T>(int n, T value) where T : unmanaged, Enum => SerdesUtil.ByteToEnum<T>(_br.ReadByte());
        public T EnumU16<T>(int n, T value) where T : unmanaged, Enum => SerdesUtil.UShortToEnum<T>((_br.ReadUInt16()));
        public T EnumU32<T>(int n, T value) where T : unmanaged, Enum => SerdesUtil.UIntToEnum<T>(_br.ReadUInt32());

        public T EnumU8<T>(string name, T value) where T : unmanaged, Enum => SerdesUtil.ByteToEnum<T>(_br.ReadByte());
        public T EnumU16<T>(string name, T value) where T : unmanaged, Enum => SerdesUtil.UShortToEnum<T>((_br.ReadUInt16()));
        public T EnumU32<T>(string name, T value) where T : unmanaged, Enum => SerdesUtil.UIntToEnum<T>(_br.ReadUInt32());

        public Guid Guid(string name, Guid value)
        {
            var bytes = _br.ReadBytes(16);
            if (bytes.Length < 16)
                throw new EndOfStreamException();
            return new Guid(bytes);
        }

        public byte[] Bytes(string name, byte[] value, int n)
        {
            var v = _br.ReadBytes(n);
            if (v.Length < n)
                throw new EndOfStreamException();

            return v;
        }

#if NETSTANDARD2_1_OR_GREATER
        public void Bytes(string name, Span<byte> value)
        {
            int n = _br.Read(value);
            if (value.Length < n)
                throw new EndOfStreamException();
        }
#endif

        public string NullTerminatedString(string name, string value)
        {
            var bytes = new List<byte>();
            for (;;)
            {
                var b = _br.ReadByte();
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
            return str.TrimEnd('\0');
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
            SerdesMethod<TTarget> serdes,
            Func<int, IList<TTarget>> initialiser = null)
        {
            list ??= initialiser?.Invoke(count) ?? new List<TTarget>();
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

        public IList<TTarget> ListWithContext<TTarget, TContext>(
            string name,
            IList<TTarget> list,
            TContext context,
            int count,
            int offset,
            SerdesContextMethod<TTarget, TContext> serdes,
            Func<int, IList<TTarget>> initialiser = null)
        {
            list ??= initialiser?.Invoke(count) ?? new List<TTarget>();
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

        void ISerdes.Assert(bool condition, string message) => Assert(condition, message);
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