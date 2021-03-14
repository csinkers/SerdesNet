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
                TMemory value,
                Func<string, TPersistent, ISerializer, TPersistent> serializer,
                IConverter<TPersistent, TMemory> converter) =>
            converter.FromNumeric(serializer(name, converter.ToNumeric(value), this));

        public T TransformEnumU8<T>(string name, T value, IConverter<byte, T> converter) 
            => converter.FromNumeric(UInt8(name, converter.ToNumeric(value), 0));
        public T TransformEnumU16<T>(string name, T value, IConverter<ushort, T> converter)
            => converter.FromNumeric(UInt16(name, converter.ToNumeric(value), 0));
        public T TransformEnumU32<T>(string name, T value, IConverter<uint, T> converter) 
            => converter.FromNumeric(UInt32(name, converter.ToNumeric(value), 0));

        public sbyte Int8(string name, sbyte value, sbyte defaultValue)
        {
            var offset = LocalOffset;
            value =  _s.Int8(name, value, defaultValue);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} y)", offset, name, value);
            return value;
        }

        public short Int16(string name, short value, short defaultValue)
        {
            var offset = LocalOffset;
            value =  _s.Int16(name, value, defaultValue);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} s)", offset, name, value);
            return value;
        }

        public int Int32(string name, int value, int defaultValue)
        {
            var offset = LocalOffset;
            value =  _s.Int32(name, value, defaultValue);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X})", offset, name, value);
            return value;
        }

        public long Int64(string name, long value, long defaultValue)
        {
            var offset = LocalOffset;
            value =  _s.Int64(name, value, defaultValue);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} L)", offset, name, value);
            return value;
        }

        public byte UInt8(string name, byte value, byte defaultValue)
        {
            var offset = LocalOffset;
            value =  _s.UInt8(name, value, defaultValue);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} uy)", offset, name, value);
            return value;
        }

        public ushort UInt16(string name, ushort value, ushort defaultValue)
        {
            var offset = LocalOffset;
            value =  _s.UInt16(name, value, defaultValue);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} us)", offset, name, value);
            return value;
        }

        public uint UInt32(string name, uint value, uint defaultValue)
        {
            var offset = LocalOffset;
            value =  _s.UInt32(name, value, defaultValue);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} u)", offset, name, value);
            return value;
        }

        public ulong UInt64(string name, ulong value, ulong defaultValue)
        {
            var offset = LocalOffset;
            value =  _s.UInt64(name, value, defaultValue);
            DoIndent();
            // _tw.WriteLine("{0:X} {1} = {2} (0x{3:X}`{4:X8} UL)", offset, name, value, (value & 0xffffffff00000000UL) >> 32, value & 0xffffffffUL);
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} UL)", offset, name, value);
            return value;
        }

        public T EnumU8<T>(string name, T value) where T : unmanaged, Enum
        {
            var offset = LocalOffset;
            value =  _s.EnumU8(name, value);
            var label = Enum.GetName(typeof(T), value);
            var uintValue = SerdesUtil.EnumToUInt(value);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} uy) // {3}", offset, name, uintValue, label);
            return value;
        }

        public T EnumU16<T>(string name, T value) where T : unmanaged, Enum
        {
            var offset = LocalOffset;
            value =  _s.EnumU16(name, value);
            var label = Enum.GetName(typeof(T), value);
            var uintValue = SerdesUtil.EnumToUInt(value);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} us) // {3}", offset, name, uintValue, label);
            return value;
        }

        public T EnumU32<T>(string name, T value) where T : unmanaged, Enum
        {
            var offset = LocalOffset;
            value =  _s.EnumU32(name, value);
            var label = Enum.GetName(typeof(T), value);
            var uintValue = SerdesUtil.EnumToUInt(value);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2} (0x{2:X} u) // {3}", offset, name, uintValue, label);
            return value;
        }

        public Guid Guid(string name, Guid value)
        {
            var offset = LocalOffset;
            value =  _s.Guid(name, value);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = {2:B}", offset, name, value);
            return value;
        }

        public byte[] Bytes(string name, byte[] value, int n)
        {

            var offset = LocalOffset;
            value =  _s.Bytes(name, value, n);
            DoIndent();
            _tw.Write("{0:X} {1} = ", offset, name);

            if (n <= 16)
                _tw.WriteLine(SerdesUtil.ConvertToHexString(value, n));
            else
                PrintByteArrayHex(value, n);
            return value;
        }

        void PrintByteArrayHex(byte[] value, int n)
        {
            _indent += 4;
            var payloadOffset = 0;
            var sb = new StringBuilder(16);
            for(int i = 0; i < n; i++)
            {
                byte b = value[i];
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

        public string NullTerminatedString(string name, string value)
        {
            value ??= string.Empty;
            var offset = LocalOffset;
            value =  _s.NullTerminatedString(name, value);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = \"{2}\"", offset, name, value);
            return value;
        }

        public string FixedLengthString(string name, string value, int length)
        {
            value ??= string.Empty;
            var offset = LocalOffset;
            value = _s.FixedLengthString(name, value, length);
            DoIndent();
            _tw.WriteLine("{0:X} {1} = \"{2}\"", offset, name, value);

            var bytes = _stringToBytes(value);
            if (bytes.Length > length + 1) throw new InvalidOperationException("Tried to write overlength string");
            return value;
        }

        public void RepeatU8(string name, byte value, int length)
        {
            var offset = LocalOffset;
            _s.RepeatU8(name, value, length);
            DoIndent();
            _tw.WriteLine(
                "{0:X} {1} = [{2} bytes (0x{2:X}) of 0x{3:X}]",
                offset,
                name,
                length,
                value
            );
        }

        public T Object<T>(string name, T value, Func<int, T, ISerializer, T> serdes)
        {
            Begin(name);
            var result = serdes(0, value, this);
            End();
            return result;
        }

        public T Object<T, TContext>(string name, T value, TContext context, Func<int, T, TContext, ISerializer, T> serdes)
        {
            Begin(name);
            var result = serdes(0, value, context, this);
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
