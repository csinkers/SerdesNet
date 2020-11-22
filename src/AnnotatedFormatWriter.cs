using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SerdesNet
{
    public class AnnotatedFormatWriter : ISerializer
    {
        readonly TextWriter _tw;
        Stack<int> _versionStack;
        int _indent;

        public AnnotatedFormatWriter(TextWriter textWriter, AnnotatedFormatWriter existing = null)
        {
            _tw = textWriter;
            if (existing != null)
                _indent = existing._indent + 4;
        }

        void DoIndent() => _tw.Write(new string(' ', _indent));
        public SerializerMode Mode => SerializerMode.WritingAnnotated;
        public void PushVersion(int version) => (_versionStack = _versionStack ?? new Stack<int>()).Push(version);
        public int PopVersion() => _versionStack == null || _versionStack.Count == 0 ? 0 : _versionStack.Pop();
        public void Comment(string msg) { DoIndent(); _tw.WriteLine("// {0}", msg); }
        void Begin() => _indent += 4;
        void End() => _indent -= 4;
        public void NewLine() => _tw.WriteLine();
        public long Offset { get; private set; }
        public long BytesRemaining => long.MaxValue;
        public void Seek(long newOffset) { _tw.WriteLine("{1:X} Seek to {0:X} for overwrite", newOffset, Offset); Offset = newOffset; }
        public void Check() { }
        public bool IsComplete() => false;

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

        public sbyte Int8(string name, sbyte existing, sbyte _)
        {
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} y)", Offset, name, existing);
            Offset += 1L;
            return existing;
        }
        public short Int16(string name, short existing, short _)
        {
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} s)", Offset, name, existing);
            Offset += 2L;
            return existing;
        }
        public int Int32(string name, int existing, int _)
        {
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X})", Offset, name, existing);
            Offset += 4L;
            return existing;
        }
        public long Int64(string name, long existing, long _)
        {
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} L)", Offset, name, existing);
            Offset += 8L;
            return existing;
        }

        public byte UInt8(string name, byte existing, byte _)
        {
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} uy)", Offset, name, existing);
            Offset += 1L;
            return existing;
        }
        public ushort UInt16(string name, ushort existing, ushort _)
        {
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} us)", Offset, name, existing);
            Offset += 2L;
            return existing;
        }
        public uint UInt32(string name, uint existing, uint _)
        {
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} u)", Offset, name, existing);
            Offset += 4L;
            return existing;
        }
        public ulong UInt64(string name, ulong existing, ulong _)
        {
            var v = existing;
            DoIndent();
            _tw.WriteLine("{0:X} {1} = 0x{2:X}`{3:X8} UL ({4})", Offset, name, (v & 0xffffffff00000000UL) >> 32, v & 0xffffffffUL, v);
            Offset += 8L;
            return existing;
        }

        public T EnumU8<T>(string name, T existing) where T : struct, Enum
        {
            var label = Enum.GetName(typeof(T), existing);
            var value = (byte)(object)existing;
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} uy) // {3}", Offset, name, value, label);
            Offset += 1L;
            return existing;
        }

        public T EnumU16<T>(string name, T existing) where T : struct, Enum
        {
            var label = Enum.GetName(typeof(T), existing);
            var value = (ushort)(object)existing;
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} us) // {3}", Offset, name, value, label);
            Offset += 2L;
            return existing;
        }

        public T EnumU32<T>(string name, T existing) where T : struct, Enum
        {
            var label = Enum.GetName(typeof(T), existing);
            var value = (uint)(object)existing;
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} u) // {3}", Offset, name, value, label);
            Offset += 4L;
            return existing;
        }

        public Guid Guid(string name, Guid existing)
        {
            var v = existing;
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2:B}", Offset, name, v);
            Offset += 16L;
            return existing;
        }

        public byte[] ByteArray(string name, byte[] existing, int n)
        {
            var v = existing;
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2}", Offset, name, Util.ConvertToHexString(v));
            Offset += v.Length;
            return existing;
        }

        public byte[] ByteArrayHex(string name, byte[] existing, int n)
        {
            var v = existing;
            DoIndent();
            _tw.Write("{0:X} {1} = ", Offset, name);

            Begin();
            var payloadOffset = 0;
            var sb = new StringBuilder(16);
            foreach (var b in v)
            {
                if (payloadOffset % 16 == 0)
                {
                    _tw.Write(' ');
                    _tw.Write(sb.ToString());
                    sb.Clear();
                    _tw.WriteLine();
                    DoIndent();
                    _tw.Write("{0:X4}: ", payloadOffset);
                }
                else if (payloadOffset % 8 == 0) _tw.Write('-');
                else if (payloadOffset % 2 == 0) _tw.Write(' ');

                _tw.Write("{0:X2}", b);

                if (b >= (byte)' ' && b <= 0x7e) sb.Append(Convert.ToChar(b));
                else sb.Append('.');

                payloadOffset += 1;
            }

            if (sb.Length > 0)
            {
                var missingChars = 16 - sb.Length;
                var spaceCount = missingChars * 2 + missingChars / 2 + 1;
                _tw.Write(new string(Enumerable.Repeat(' ', spaceCount).ToArray()));
                _tw.Write(sb.ToString());
            }

            _tw.WriteLine();
            End();
            Offset += v.Length;
            return existing;
        }

        public string NullTerminatedString(string name, string existing)
        {
            var v = existing;
            DoIndent();
            _tw.WriteLine("{0:X} {1} = \"{2}\"", Offset, name, v);

            var bytes = Encoding.GetEncoding(850).GetBytes(v);
            Offset += bytes.Length + 1; // add a byte for the null terminator
            return existing;
        }

        public string FixedLengthString(string name, string existing, int length)
        {
            var v = existing;
            DoIndent();
            _tw.WriteLine("{0:X} {1} = \"{2}\"", Offset, name, v);

            var bytes = Encoding.GetEncoding(850).GetBytes(v);
            if (bytes.Length > length + 1) throw new InvalidOperationException("Tried to write overlength string");

            Offset += length; // add a byte for the null terminator
            return existing;
        }

        public void RepeatU8(string name, byte v, int length)
        {
            DoIndent();
            _tw.WriteLine(
                "{0:X} {1} = [{2} bytes (0x{2:X}) of 0x{3:X}]",
                Offset,
                name,
                length,
                v
            );
            Offset += length;
        }

        public T Object<T>(string name, T existing, Func<int, T, ISerializer, T> serdes)
        {
            _indent += 4;
            DoIndent();
            _tw.WriteLine("// {0}", name);
            var result = serdes(0, existing, this);
            _indent -= 4;
            return result;
        }

        public T Object<T, TContext>(string name, T existing, TContext context, Func<int, T, TContext, ISerializer, T> serdes)
        {
            _indent += 4;
            DoIndent();
            _tw.WriteLine("// {0}", name);
            var result = serdes(0, existing, context, this);
            _indent -= 4;
            return result;
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
            list = list ?? initialiser?.Invoke(count) ?? new List<TTarget>();
            _indent += 4;
            DoIndent();
            _tw.WriteLine("[ // {0}", name);
            for (int i = offset; i < offset + count; i++)
                serializer(i, list[i], this);
            _tw.WriteLine(" ]");
            _indent -= 4;
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
            list = list ?? initialiser?.Invoke(count) ?? new List<TTarget>();
            _indent += 4;
            DoIndent();
            _tw.WriteLine("[ // {0}", name);
            for (int i = offset; i < offset + count; i++)
                serializer(i, list[i], context, this);
            _tw.WriteLine(" ]");
            _indent -= 4;
            return list;
        }

        protected virtual void Dispose(bool disposing) { }
        public void Dispose() => Dispose(true);
    }
}
