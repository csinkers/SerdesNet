using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SerdesNet
{
    public class JsonWriter : ISerializer
    {
        const string HexChars = "0123456789ABCDEF";

        readonly Stack<int> _versionStack = new Stack<int>();
        readonly TextWriter _tw;
        readonly bool _elideDefaults;
        int _indent;
        bool _first = true;

        public JsonWriter(TextWriter textWriter, bool elideDefaults, JsonWriter existing = null)
        {
            _tw = textWriter;
            _elideDefaults = elideDefaults;
            if (existing != null)
                _indent = existing._indent;
        }

        void DoIndent() => _tw.Write(new string(' ', _indent));
        public SerializerMode Mode => SerializerMode.WritingJson;
        public void PushVersion(int version) => _versionStack.Push(version);
        public int PopVersion() => _versionStack.Count == 0 ? 0 : _versionStack.Pop();
        public void Comment(string msg) { _tw.WriteLine("// {0}", msg); DoIndent(); }
        public void Begin(string name)
        {
            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.WriteLine('{');
            _first = true;
            _indent += 4;
            DoIndent();
        }

        public void End()
        {
            _indent -= 4;
            if (!_first)
            {
                _tw.WriteLine();
                DoIndent();
            }

            _tw.Write('}');
            _first = false;
        }

        public void NewLine() => _tw.WriteLine();
        public long Offset { get; private set; }
        public long BytesRemaining => long.MaxValue;

        public void Seek(long newOffset)
        {
            // _tw.WriteLine("// {1:X} Seek to {0:X} for overwrite", newOffset, Offset);
            Offset = newOffset;
        }
        public void Check() { }
        public bool IsComplete() => false;

        public TMemory Transform<TPersistent, TMemory>(
                string name,
                TMemory existing,
                Func<string, TPersistent, TPersistent> serializer,
                IConverter<TPersistent, TMemory> converter) =>
            converter.FromNumeric(serializer(name, converter.ToNumeric(existing)));

        public T TransformEnumU8<T>(string name, T existing, IConverter<byte, T> converter)
        {
            var symbolic = converter.ToSymbolic(existing);
            if (_elideDefaults && symbolic == null)
                return existing;

            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            if (symbolic == null)
                _tw.Write("null");
            else
                _tw.Write("\"{0}\"", symbolic);

            _first = false;
            Offset += 1L;
            return existing;
        }

        public T TransformEnumU16<T>(string name, T existing, IConverter<ushort, T> converter)
        {
            var symbolic = converter.ToSymbolic(existing);
            if (_elideDefaults && symbolic == null)
                return existing;

            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            if(symbolic == null)
                _tw.Write("null");
            else
                _tw.Write("\"{0}\"", symbolic);

            _first = false;
            Offset += 2L;
            return existing;
        }

        public T TransformEnumU32<T>(string name, T existing, IConverter<uint, T> converter)
        {
            var symbolic = converter.ToSymbolic(existing);
            if (_elideDefaults && symbolic == null)
                return existing;

            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            if(symbolic == null)
                _tw.Write("null");
            else
                _tw.Write("\"{0}\"", symbolic);

            _first = false;
            Offset += 4L;
            return existing;
        }

        public sbyte Int8(string name, sbyte existing)
        {
            if (_elideDefaults && existing == 0)
                return existing;

            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write(existing);
            _first = false;
            Offset += 1L;
            return existing;
        }

        public short Int16(string name, short existing)
        {
            if (_elideDefaults && existing == 0)
                return existing;

            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write(existing);
            _first = false;
            Offset += 2L;
            return existing;
        }

        public int Int32(string name, int existing)
        {
            if (_elideDefaults && existing == 0)
                return existing;

            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write(existing);
            _first = false;
            Offset += 4L;
            return existing;
        }

        public long Int64(string name, long existing)
        {
            if (_elideDefaults && existing == 0)
                return existing;

            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write(existing);
            _first = false;
            Offset += 8L;
            return existing;
        }

        public byte UInt8(string name, byte existing)
        {
            if (_elideDefaults && existing == 0)
                return existing;

            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write(existing);
            _first = false;
            Offset += 1L;
            return existing;
        }

        public ushort UInt16(string name, ushort existing)
        {
            if (_elideDefaults && existing == 0)
                return existing;

            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write(existing);
            _first = false;
            Offset += 2L;
            return existing;
        }

        public uint UInt32(string name, uint existing)
        {
            if (_elideDefaults && existing == 0)
                return existing;

            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write(existing);
            _first = false;
            Offset += 4L;
            return existing;
        }

        public ulong UInt64(string name, ulong existing)
        {
            if (_elideDefaults && existing == 0)
                return existing;

            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write(existing);
            _first = false;
            Offset += 8L;
            return existing;
        }

        public T EnumU8<T>(string name, T existing) where T : struct, Enum
        {
            if (_elideDefaults && (byte)(object)existing == 0)
                return existing;

            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write("\"{0}\"", existing.ToString());
            _first = false;
            Offset += 1L;
            return existing;
        }

        public T EnumU16<T>(string name, T existing) where T : struct, Enum
        {
            if (_elideDefaults && (ushort)(object)existing == 0)
                return existing;

            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write("\"{0}\"", existing.ToString());
            _first = false;
            Offset += 2L;
            return existing;
        }

        public T EnumU32<T>(string name, T existing) where T : struct, Enum
        {
            if (_elideDefaults && (uint)(object)existing == 0)
                return existing;

            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write("\"{0}\"", existing.ToString());
            _first = false;
            Offset += 4L;
            return existing;
        }

        public Guid Guid(string name, Guid existing)
        {
            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write("\"{0}\"", existing);
            _first = false;
            Offset += 16L;
            return existing;
        }

        public static string ConvertToHexString(byte[] bytes)
        {
            var result = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                result.Append(HexChars[b >> 4]);
                result.Append(HexChars[b & 0xf]);
            }

            return result.ToString();
        }

        public byte[] ByteArray(string name, byte[] existing, int n)
        {
            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write("\"{0}\"", ConvertToHexString(existing));
            _first = false;
            Offset += existing.Length;
            return existing;
        }

        public byte[] ByteArray2(string name, byte[] existing, int n, string comment)
        {
            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write("\"{0}\"", ConvertToHexString(existing));
            _first = false;
            Offset += existing.Length;
            return existing;
        }

        public byte[] ByteArrayHex(string name, byte[] existing, int n)
        {
            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": \"", name);

            _indent += 4;
            var payloadOffset = 0;
            var sb = new StringBuilder(16);
            foreach (var b in existing)
            {
                if (payloadOffset % 16 == 0)
                {
                    _tw.Write(' ');
                    _tw.Write(sb.ToString());
                    sb.Clear();
                    _tw.Write("\\n");
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

            _tw.Write("\"");
            _first = false;
            _indent -= 4;
            Offset += existing.Length;
            return existing;
        }

        public string NullTerminatedString(string name, string existing)
        {
            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write("\"{0}\"", existing);
            _first = false;

            var bytes = Encoding.GetEncoding(850).GetBytes(existing);
            Offset += bytes.Length + 1; // add a byte for the null terminator
            return existing;
        }

        public string FixedLengthString(string name, string existing, int length)
        {
            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write("\"{0}\"", existing);
            _first = false;

            var bytes = Encoding.GetEncoding(850).GetBytes(existing);
            if (bytes.Length > length + 1) throw new InvalidOperationException("Tried to write overlength string");

            Offset += length; // add a byte for the null terminator
            return existing;
        }

        public void RepeatU8(string name, byte v, int length)
        {
            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write(
                "\"[{0} bytes (0x{0:X}) of 0x{1:X}]\"",
                length,
                v);

            _first = false;
            Offset += length;
        }

        public void Meta(string name, Action<ISerializer> serializer, Action<ISerializer> deserializer)
        {
            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);
            _first = true;
            serializer(this);
            _first = false;
        }

        public T Meta<T>(string name, T existing, Func<int, T, ISerializer, T> serdes)
        {
            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _first = true;
            var result = serdes(0, existing, this);
            _first = false;
            return result;
        }

        public IList<TTarget> List<TTarget>(string name, IList<TTarget> list, int count, Func<int, TTarget, ISerializer, TTarget> serializer)
        {
            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if(name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.WriteLine("[");
            if (count > 0)
            {
                _indent += 4;
                _first = true;
                DoIndent();
                for (int i = 0; i < count; i++)
                    serializer(i, list[i], this);

                _indent -= 4;
                _tw.WriteLine();
            }

            DoIndent();
            _tw.Write("]");
            _first = false;
            return list;
        }

        public IList<TTarget> List<TTarget>(string name, IList<TTarget> list, int count, int offset, Func<int, TTarget, ISerializer, TTarget> serializer)
        {
            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if(name != null)
                _tw.Write("\"{0}\": ", name);
            _tw.WriteLine("[");
            if (count - offset > 0)
            {
                _indent += 4;
                _first = true;
                DoIndent();
                for (int i = offset; i < offset + count; i++)
                    serializer(i, list[i], this);

                _indent -= 4;
                _tw.WriteLine();
            }

            DoIndent();
            _tw.Write("]");
            _first = false;
            return list;
        }

        public string Raw(string name, string content)
        {
            if (!_first)
            {
                _tw.WriteLine(", ");
                DoIndent();
            }

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write(content);
            _first = false;
            return content;
        }
    }
}
