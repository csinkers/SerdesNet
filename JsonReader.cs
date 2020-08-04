using System;
using System.Collections.Generic;

namespace SerdesNet
{
    public class JsonReader : ISerializer
    {
        public SerializerMode Mode { get; }
        public long Offset { get; }
        public long BytesRemaining { get; }
        public void Comment(string comment)
        {
            throw new NotImplementedException();
        }

        public void Begin(string name)
        {
            throw new NotImplementedException();
        }

        public void End()
        {
            throw new NotImplementedException();
        }

        public void NewLine()
        {
            throw new NotImplementedException();
        }

        public void Seek(long offset)
        {
            throw new NotImplementedException();
        }

        public void Check()
        {
            throw new NotImplementedException();
        }

        public bool IsComplete()
        {
            throw new NotImplementedException();
        }

        public void PushVersion(int version)
        {
            throw new NotImplementedException();
        }

        public int PopVersion()
        {
            throw new NotImplementedException();
        }

        public sbyte Int8(string name, sbyte existing)
        {
            throw new NotImplementedException();
        }

        public short Int16(string name, short existing)
        {
            throw new NotImplementedException();
        }

        public int Int32(string name, int existing)
        {
            throw new NotImplementedException();
        }

        public long Int64(string name, long existing)
        {
            throw new NotImplementedException();
        }

        public byte UInt8(string name, byte existing)
        {
            throw new NotImplementedException();
        }

        public ushort UInt16(string name, ushort existing)
        {
            throw new NotImplementedException();
        }

        public uint UInt32(string name, uint existing)
        {
            throw new NotImplementedException();
        }

        public ulong UInt64(string name, ulong existing)
        {
            throw new NotImplementedException();
        }

        public T EnumU8<T>(string name, T existing) where T : struct, Enum
        {
            throw new NotImplementedException();
        }

        public T EnumU16<T>(string name, T existing) where T : struct, Enum
        {
            throw new NotImplementedException();
        }

        public T EnumU32<T>(string name, T existing) where T : struct, Enum
        {
            throw new NotImplementedException();
        }

        public TMemory Transform<TPersistent, TMemory>(string name, TMemory existing, Func<string, TPersistent, TPersistent> serializer, IConverter<TPersistent, TMemory> converter)
        {
            throw new NotImplementedException();
        }

        public T TransformEnumU8<T>(string name, T existing, IConverter<byte, T> converter)
        {
            throw new NotImplementedException();
        }

        public T TransformEnumU16<T>(string name, T existing, IConverter<ushort, T> converter)
        {
            throw new NotImplementedException();
        }

        public T TransformEnumU32<T>(string name, T existing, IConverter<uint, T> converter)
        {
            throw new NotImplementedException();
        }

        public Guid Guid(string name, Guid existing)
        {
            throw new NotImplementedException();
        }

        public byte[] ByteArray(string name, byte[] existing, int length)
        {
            throw new NotImplementedException();
        }

        public byte[] ByteArrayHex(string name, byte[] existing, int length)
        {
            throw new NotImplementedException();
        }

        public byte[] ByteArray2(string name, byte[] existing, int length, string coment)
        {
            throw new NotImplementedException();
        }

        public string NullTerminatedString(string name, string existing)
        {
            throw new NotImplementedException();
        }

        public string FixedLengthString(string name, string existing, int length)
        {
            throw new NotImplementedException();
        }

        public void RepeatU8(string name, byte value, int count)
        {
            throw new NotImplementedException();
        }

        public void Meta(string name, Action<ISerializer> reader, Action<ISerializer> writer)
        {
            throw new NotImplementedException();
        }

        public T Meta<T>(string name, T existing, Func<int, T, ISerializer, T> serdes)
        {
            throw new NotImplementedException();
        }

        public IList<TTarget> List<TTarget>(string name, IList<TTarget> list, int count, Func<int, TTarget, ISerializer, TTarget> serdes, Func<int, IList<TTarget>> initialiser = null)
        {
            throw new NotImplementedException();
        }

        public IList<TTarget> List<TTarget>(string name, IList<TTarget> list, int count, int offset, Func<int, TTarget, ISerializer, TTarget> serializer, Func<int, IList<TTarget>> initialiser = null)
        {
            throw new NotImplementedException();
        }
    }
}
