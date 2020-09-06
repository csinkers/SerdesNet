using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SerdesNet
{
    public class JsonWriter : ISerializer
    {
        readonly Stack<int> _versionStack = new Stack<int>();
        readonly TextWriter _tw;
        readonly Encoding _binaryStringEncoding;
        readonly bool _elideDefaults;
        readonly bool _compact;
        readonly Action<string> _assertionFailed;
        int _indent;
        bool _first = true;

        public JsonWriter(
            TextWriter textWriter,
            JsonWriter existing = null,
            Encoding binaryStringEncoding = null,
            Action<string> assertionFailed = null,
            bool elideDefaults = true,
            bool compact = false)
        {
            _tw = textWriter;
            _binaryStringEncoding = binaryStringEncoding ?? Encoding.GetEncoding(850);
            _elideDefaults = elideDefaults;
            _compact = compact;
            _assertionFailed = assertionFailed ?? (x => throw new InvalidOperationException(x));
            if (existing != null)
                _indent = existing._indent;
        }

        void DoIndent() { if (!_compact) _tw.Write(new string(' ', _indent)); }
        void Comma()
        {
            if (_first) return;
            if (_compact)
                _tw.Write(", ");
            else
            {
                _tw.WriteLine(',');
                DoIndent();
            }
        }

        public SerializerMode Mode => SerializerMode.WritingJson;
        public void PushVersion(int version) => _versionStack.Push(version);
        public int PopVersion() => _versionStack.Count == 0 ? 0 : _versionStack.Pop();

        public void Comment(string msg)
        {
            _tw.Write("/* {0} */", msg);
            if (_compact)
                _tw.WriteLine();
            DoIndent();
        }
        void Begin(string name)
        {
            Comma();

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            if (_compact)
                _tw.Write("{ ");
            else
                _tw.WriteLine('{');
            _first = true;
            _indent += 4;
            DoIndent();
        }

        void End()
        {
            _indent -= 4;
            if (!_first)
            {
                if (!_compact)
                {
                    _tw.WriteLine();
                    DoIndent();
                }
                else _tw.Write(' ');
            }

            _tw.Write('}');
            _first = false;
        }

        public void NewLine() { if (!_compact) _tw.WriteLine(); }
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
                Func<string, TPersistent, ISerializer, TPersistent> serializer,
                IConverter<TPersistent, TMemory> converter) =>
            converter.FromNumeric(serializer(name, converter.ToNumeric(existing), this));

        public T TransformEnumU8<T>(string name, T existing, IConverter<byte, T> converter)
        {
            var symbolic = converter.ToSymbolic(existing);
            if (_elideDefaults && symbolic == null && name != null)
                return existing;

            Comma();

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
            if (_elideDefaults && symbolic == null && name != null)
                return existing;

            Comma();

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
            if (_elideDefaults && symbolic == null && name != null)
                return existing;

            Comma();

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

        public sbyte Int8(string name, sbyte existing, sbyte defaultValue)
        {
            if (_elideDefaults && existing == defaultValue && name != null)
                return existing;

            Comma();

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write(existing);
            _first = false;
            Offset += 1L;
            return existing;
        }

        public short Int16(string name, short existing, short defaultValue)
        {
            if (_elideDefaults && existing == defaultValue && name != null)
                return existing;

            Comma();

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write(existing);
            _first = false;
            Offset += 2L;
            return existing;
        }

        public int Int32(string name, int existing, int defaultValue)
        {
            if (_elideDefaults && existing == defaultValue && name != null)
                return existing;

            Comma();

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write(existing);
            _first = false;
            Offset += 4L;
            return existing;
        }

        public long Int64(string name, long existing, long defaultValue)
        {
            if (_elideDefaults && existing == defaultValue && name != null)
                return existing;

            Comma();

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write(existing);
            _first = false;
            Offset += 8L;
            return existing;
        }

        public byte UInt8(string name, byte existing, byte defaultValue)
        {
            if (_elideDefaults && existing == defaultValue && name != null)
                return existing;

            Comma();

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write(existing);
            _first = false;
            Offset += 1L;
            return existing;
        }

        public ushort UInt16(string name, ushort existing, ushort defaultValue)
        {
            if (_elideDefaults && existing == defaultValue && name != null)
                return existing;

            Comma();

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write(existing);
            _first = false;
            Offset += 2L;
            return existing;
        }

        public uint UInt32(string name, uint existing, uint defaultValue)
        {
            if (_elideDefaults && existing == defaultValue && name != null)
                return existing;

            Comma();

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write(existing);
            _first = false;
            Offset += 4L;
            return existing;
        }

        public ulong UInt64(string name, ulong existing, ulong defaultValue)
        {
            if (_elideDefaults && existing == defaultValue && name != null)
                return existing;

            Comma();

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write(existing);
            _first = false;
            Offset += 8L;
            return existing;
        }

        public T EnumU8<T>(string name, T existing) where T : struct, Enum
        {
            if (_elideDefaults && (byte)(object)existing == 0 && name != null)
                return existing;

            Comma();

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write("\"{0}\"", existing.ToString());
            _first = false;
            Offset += 1L;
            return existing;
        }

        public T EnumU16<T>(string name, T existing) where T : struct, Enum
        {
            if (_elideDefaults && (ushort)(object)existing == 0 && name != null)
                return existing;

            Comma();

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write("\"{0}\"", existing.ToString());
            _first = false;
            Offset += 2L;
            return existing;
        }

        public T EnumU32<T>(string name, T existing) where T : struct, Enum
        {
            if (_elideDefaults && (uint)(object)existing == 0 && name != null)
                return existing;

            Comma();

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write("\"{0}\"", existing.ToString());
            _first = false;
            Offset += 4L;
            return existing;
        }

        public Guid Guid(string name, Guid existing)
        {
            Comma();

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write("\"{0}\"", existing);
            _first = false;
            Offset += 16L;
            return existing;
        }

        public byte[] ByteArray(string name, byte[] existing, int n)
        {
            Comma();

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write("\"{0}\"", Util.ConvertToHexString(existing));
            _first = false;
            Offset += existing.Length;
            return existing;
        }

        public byte[] ByteArrayHex(string name, byte[] existing, int n)
        {
            Comma();

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
            Comma();

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write("\"{0}\"", existing);
            _first = false;

            var bytes = _binaryStringEncoding.GetBytes(existing);
            Offset += bytes.Length + 1; // add a byte for the null terminator
            return existing;
        }

        public string FixedLengthString(string name, string existing, int length)
        {
            Comma();

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write("\"{0}\"", existing);
            _first = false;

            var bytes = _binaryStringEncoding.GetBytes(existing);
            if (bytes.Length > length) _assertionFailed("Tried to write overlength string");

            Offset += length;
            return existing;
        }

        public void RepeatU8(string name, byte v, int length)
        {
            Comma();

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write(
                "\"[{0} bytes (0x{0:X}) of 0x{1:X}]\"",
                length,
                v);

            _first = false;
            Offset += length;
        }

        public T Object<T>(string name, T existing, Func<int, T, ISerializer, T> serdes)
        {
            Comma();

            Begin(name);
            var result = serdes(0, existing, this);
            End();
            _first = false;
            return result;
        }

        public IList<TTarget> List<TTarget>(
            string name,
            IList<TTarget> list,
            int count,
            Func<int, TTarget, ISerializer, TTarget> serdes,
            Func<int, IList<TTarget>> initialiser = null)
        {
            Comma();

            if(name != null)
                _tw.Write("\"{0}\": ", name);

            if (_compact)
                _tw.Write("[ ");
            else
                _tw.WriteLine("[");

            if (count > 0)
            {
                _indent += 4;
                _first = true;
                DoIndent();
                    for (int i = 0; i < count; i++)
                    serdes(i, list[i], this);

                _indent -= 4;
                if (!_compact)
                    _tw.WriteLine();
            }

            DoIndent();
            _tw.Write("]");
            _first = false;
            return list;
        }

        public IList<TTarget> List<TTarget>(
            string name,
            IList<TTarget> list,
            int count,
            int offset,
            Func<int, TTarget, ISerializer, TTarget> serializer,
            Func<int, IList<TTarget>> initialiser = null)
        {
            Comma();

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            if (_compact)
                _tw.Write("[ ");
            else
                _tw.WriteLine("[");

            if (count - offset > 0)
            {
                _indent += 4;
                _first = true;
                DoIndent();
                for (int i = offset; i < offset + count; i++)
                    serializer(i, list[i], this);

                _indent -= 4;
                if (!_compact)
                    _tw.WriteLine();
            }

            DoIndent();
            _tw.Write("]");
            _first = false;
            return list;
        }

        public string Raw(string name, string content)
        {
            Comma();

            if (name != null)
                _tw.Write("\"{0}\": ", name);

            _tw.Write(content);
            _first = false;
            return content;
        }
    }
}
