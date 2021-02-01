using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SerdesNet
{
    /// <summary>
    /// Delegates binary reading/writing to an underlying serializer while writing
    /// an annotated record of the data to a text writer.
    /// </summary>
    public class AnnotationFacadeSerializer : ISerializer
    {
        readonly TextWriter _tw;
        readonly Func<string, byte[]> _stringToBytes;
        readonly ISerializer _s;
        readonly Stack<long> _offsetStack = new Stack<long>();
        readonly bool _useRelativeOffsets;
        int _indent;

        public AnnotationFacadeSerializer(ISerializer s, TextWriter tw, Func<string, byte[]> stringToBytes, bool useRelativeOffsets = true)
        {
            _s = s ?? throw new ArgumentNullException(nameof(s));
            _tw = tw ?? throw new ArgumentNullException(nameof(tw));
            _stringToBytes = stringToBytes ?? throw new ArgumentNullException(nameof(stringToBytes));
            _offsetStack.Push(0);
            _useRelativeOffsets = useRelativeOffsets;
        }

        public void Dispose() => _s.Dispose();
        public SerializerFlags Flags => _s.Flags | SerializerFlags.Comments;
        public long Offset => _s.Offset;
        long LocalOffset => _useRelativeOffsets ? Offset - _offsetStack.Peek() : Offset;
        public long BytesRemaining => _s.BytesRemaining;

        public void Seek(long offset)
        {
             _tw.WriteLine("{1:X} Seek to {0:X} for overwrite", offset, LocalOffset);
            _s.Seek(offset);
        }

        public void Check() => _s.Check();
        public void Assert(bool condition, string message) => _s.Assert(condition, message);
        public bool IsComplete() => _s.IsComplete();
        public void Pad(int bytes) => _s.Pad(bytes); // Don't write anything to the annotation stream for padding
        void DoIndent() => _tw.Write(new string(' ', _indent));
        public void Comment(string msg) { DoIndent(); _tw.WriteLine("// {0}", msg); }
        public void Begin(string name)
        {
            DoIndent();
            _tw.WriteLine($"{LocalOffset:X} {name}: {{");
            _indent += 4;
            _offsetStack.Push(Offset);
        }

        public void End()
        {
            _offsetStack.Pop();
            _indent -= 4;
            DoIndent();
            _tw.WriteLine("}");
        }

        public void NewLine() => _tw.WriteLine();

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

        public sbyte Int8(string name, sbyte existing, sbyte defaultValue)
        {
            var offset = LocalOffset;
            var v =  _s.Int8(name, existing, defaultValue);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} y)", offset, name, v);
            return v;
        }

        public short Int16(string name, short existing, short defaultValue)
        {
            var offset = LocalOffset;
            var v =  _s.Int16(name, existing, defaultValue);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} s)", offset, name, v);
            return v;
        }

        public int Int32(string name, int existing, int defaultValue)
        {
            var offset = LocalOffset;
            var v =  _s.Int32(name, existing, defaultValue);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X})", offset, name, v);
            return v;
        }

        public long Int64(string name, long existing, long defaultValue)
        {
            var offset = LocalOffset;
            var v =  _s.Int64(name, existing, defaultValue);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} L)", offset, name, v);
            return v;
        }

        public byte UInt8(string name, byte existing, byte defaultValue)
        {
            var offset = LocalOffset;
            var v =  _s.UInt8(name, existing, defaultValue);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} uy)", offset, name, v);
            return v;
        }

        public ushort UInt16(string name, ushort existing, ushort defaultValue)
        {
            var offset = LocalOffset;
            var v =  _s.UInt16(name, existing, defaultValue);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} us)", offset, name, v);
            return v;
        }

        public uint UInt32(string name, uint existing, uint defaultValue)
        {
            var offset = LocalOffset;
            var v =  _s.UInt32(name, existing, defaultValue);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} u)", offset, name, v);
            return v;
        }

        public ulong UInt64(string name, ulong existing, ulong defaultValue)
        {
            var offset = LocalOffset;
            var v =  _s.UInt64(name, existing, defaultValue);
            DoIndent();
            // _tw.WriteLine("{0:X} {1} = {2} (0x{3:X}`{4:X8} UL)", offset, name, v, (v & 0xffffffff00000000UL) >> 32, v & 0xffffffffUL);
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} UL)", offset, name, v);
            return v;
        }

        public T EnumU8<T>(string name, T existing) where T : struct, Enum
        {
            var offset = LocalOffset;
            var v =  _s.EnumU8(name, existing);
            var label = Enum.GetName(typeof(T), v);
            var value = (byte)(object)v;
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} uy) // {3}", offset, name, value, label);
            return v;
        }

        public T EnumU16<T>(string name, T existing) where T : struct, Enum
        {
            var offset = LocalOffset;
            var v =  _s.EnumU16(name, existing);
            var label = Enum.GetName(typeof(T), v);
            var value = (ushort)(object)v;
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} us) // {3}", offset, name, value, label);
            return v;
        }

        public T EnumU32<T>(string name, T existing) where T : struct, Enum
        {
            var offset = LocalOffset;
            var v =  _s.EnumU32(name, existing);
            var label = Enum.GetName(typeof(T), v);
            var value = (uint)(object)v;
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} u) // {3}", offset, name, value, label);
            return v;
        }

        public Guid Guid(string name, Guid existing)
        {
            var offset = LocalOffset;
            var v =  _s.Guid(name, existing);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2:B}", offset, name, v);
            return v;
        }

        public byte[] ByteArray(string name, byte[] existing, int n)
        {

            var offset = LocalOffset;
            var v =  _s.ByteArray(name, existing, n);
            DoIndent();
            _tw.Write("{0:X} {1} = ", offset, name);

            if (n <= 16)
                _tw.WriteLine(Util.ConvertToHexString(v));
            else
                PrintByteArrayHex(v);
            return v;
        }

        void PrintByteArrayHex(byte[] v)
        {
            _indent += 4;
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
            _indent -= 4;
        }

        public string NullTerminatedString(string name, string existing)
        {
            var offset = LocalOffset;
            var v =  _s.NullTerminatedString(name, existing);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = \"{2}\"", offset, name, v);
            return v;
        }

        public string FixedLengthString(string name, string existing, int length)
        {
            var offset = LocalOffset;
            var v = _s.FixedLengthString(name, existing, length);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = \"{2}\"", offset, name, v);

            var bytes = _stringToBytes(v);
            if (bytes.Length > length + 1) throw new InvalidOperationException("Tried to write overlength string");
            return v;
        }

        public void RepeatU8(string name, byte v, int length)
        {
            var offset = LocalOffset;
            _s.RepeatU8(name, v, length);
            DoIndent();
            _tw.WriteLine(
                "{0:X} {1} = [{2} bytes (0x{2:X}) of 0x{3:X}]",
                offset,
                name,
                length,
                v
            );
        }

        public T Object<T>(string name, T existing, Func<int, T, ISerializer, T> serdes)
        {
            Begin(name);
            var result = serdes(0, existing, this);
            End();
            return result;
        }

        public T Object<T, TContext>(string name, T existing, TContext context, Func<int, T, TContext, ISerializer, T> serdes)
        {
            Begin(name);
            var result = serdes(0, existing, context, this);
            End();
            return result;
        }

        public void Object(string name, Action<ISerializer> serdes)
        {
            Begin(name);
            serdes(this);
            End();
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
            Begin(name);
            TTarget InterceptedSerializer(int i, TTarget x, ISerializer _) => serializer(i, x, this);
            var result = _s.List(name, list, count, offset, InterceptedSerializer, initialiser);
            End();
            return result;
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
            Begin(name);
            TTarget InterceptedSerializer(int i, TTarget x, TContext c, ISerializer _) => serializer(i, x, c, this);
            var result = _s.List(name, list, context, count, offset, InterceptedSerializer, initialiser);
            End();
            return result;
        }
    }
}
