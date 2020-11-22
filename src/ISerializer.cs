using System;
using System.Collections.Generic;
using System.Globalization;

namespace SerdesNet
{
    public interface ISerializer : IDisposable
    {
        SerializerMode Mode { get; }
        long Offset { get; } // For recording offsets to be overwritten later
        long BytesRemaining { get; }
        void Comment(string comment); // Only affects annotating writers
        // void Begin(string name = null); // Only affects annotating writers
        // void End(); // Only affects annotating writers
        void NewLine(); // Only affects annotating writers
        void Seek(long offset); // For overwriting pre-recorded offsets
        void Check(); // Ensure offset matches stream position
        bool IsComplete(); // Ensure offset matches stream position
        void PushVersion(int version);
        int PopVersion();

        sbyte Int8(string name, sbyte existing, sbyte defaultValue = 0);
        short Int16(string name, short existing, short defaultValue = 0);
        int Int32(string name, int existing, int defaultValue = 0);
        long Int64(string name, long existing, long defaultValue = 0);
        byte UInt8(string name, byte existing, byte defaultValue = 0);
        ushort UInt16(string name, ushort existing, ushort defaultValue = 0);
        uint UInt32(string name, uint existing, uint defaultValue = 0);
        ulong UInt64(string name, ulong existing, ulong defaultValue = 0);

        T EnumU8<T>(string name, T existing) where T : struct, Enum;
        T EnumU16<T>(string name, T existing) where T : struct, Enum;
        T EnumU32<T>(string name, T existing) where T : struct, Enum;
        T Transform<TNumeric, T>(
            string name,
            T existing,
            Func<string, TNumeric, ISerializer, TNumeric> serializer,
            IConverter<TNumeric, T> converter);

        T TransformEnumU8<T>(string name, T existing, IConverter<byte, T> converter);
        T TransformEnumU16<T>(string name, T existing, IConverter<ushort, T> converter);
        T TransformEnumU32<T>(string name, T existing, IConverter<uint, T> converter);

        Guid Guid(string name, Guid existing);
        byte[] ByteArray(string name, byte[] existing, int length);
        byte[] ByteArrayHex(string name, byte[] existing, int length);
        string NullTerminatedString(string name, string existing);
        string FixedLengthString(string name, string existing, int length);
        void RepeatU8(string name, byte value, int count); // Either writes a block of padding or verifies the consistency of one while reading
        T Object<T>(string name, T existing, Func<int, T, ISerializer, T> serdes);
        T Object<T, TContext>(string name, T existing, TContext context, Func<int, T, TContext, ISerializer, T> serdes);
        IList<TTarget> List<TTarget>(
            string name,
            IList<TTarget> list,
            int count,
            Func<int, TTarget, ISerializer, TTarget> serdes,
            Func<int, IList<TTarget>> initialiser = null);

        IList<TTarget> List<TTarget>(
            string name,
            IList<TTarget> list,
            int count,
            int offset,
            Func<int, TTarget, ISerializer, TTarget> serdes,
            Func<int, IList<TTarget>> initialiser = null);

        IList<TTarget> List<TTarget, TContext>(
            string name,
            IList<TTarget> list,
            TContext context,
            int count,
            Func<int, TTarget, TContext, ISerializer, TTarget> serdes,
            Func<int, IList<TTarget>> initialiser = null);

        IList<TTarget> List<TTarget, TContext>(
            string name,
            IList<TTarget> list,
            TContext context,
            int count,
            int offset,
            Func<int, TTarget, TContext, ISerializer, TTarget> serdes,
            Func<int, IList<TTarget>> initialiser = null);
    }

    public static class S
    {
        public static Func<int, T, ISerializer, T> Object<T>(Func<int, T, ISerializer, T> serdes) 
            => (i,x,s) 
                => s.Object($"{i}", x, serdes);

        public static sbyte Int8(string name, sbyte existing, ISerializer s) => s.Int8(name, existing);
        public static short Int16(string name, short existing, ISerializer s) => s.Int16(name, existing);
        public static int Int32(string name, int existing, ISerializer s) => s.Int32(name, existing);
        public static long Int64(string name, long existing, ISerializer s) => s.Int64(name, existing);
        public static byte UInt8(string name, byte existing, ISerializer s) => s.UInt8(name, existing);
        public static ushort UInt16(string name, ushort existing, ISerializer s) => s.UInt16(name, existing);
        public static uint UInt32(string name, uint existing, ISerializer s) => s.UInt32(name, existing);
        public static ulong UInt64(string name, ulong existing, ISerializer s) => s.UInt64(name, existing);

        public static Guid Guid(string name, Guid existing, ISerializer s) => s.Guid(name, existing);
        public static string NullTerminatedString(string name, string existing, ISerializer s) => s.NullTerminatedString(name, existing);

        public static T EnumU8<T>(int i, T v, ISerializer s) where T : struct, Enum 
            => s.EnumU8(i.ToString(CultureInfo.InvariantCulture), v);
        public static T EnumU16<T>(int i, T v, ISerializer s) where T : struct, Enum 
            => s.EnumU16(i.ToString(CultureInfo.InvariantCulture), v);
        public static T EnumU32<T>(int i, T v, ISerializer s) where T : struct, Enum 
            => s.EnumU32(i.ToString(CultureInfo.InvariantCulture), v);
    }

    public static class SerializerExtensions
    {
        public static int PeekVersion(this ISerializer s)
        {
            var version = s.PopVersion();
            s.PushVersion(version);
            return version;
        }

        static short SwapBytes16(short x) { unchecked { return (short) SwapBytes16((ushort) x); } }
        static ushort SwapBytes16(ushort x)
        {
            // swap adjacent 8-bit blocks
            ushort a = (ushort)((x & 0xFF00) >> 8);
            ushort b = (ushort)((x & 0x00FF) << 8);
            return (ushort)(a | b);
        }

        static int SwapBytes32(int x) { unchecked { return (int) SwapBytes32((uint) x); } }
        static uint SwapBytes32(uint x)
        {
            // swap adjacent 16-bit blocks
            x = ((x & 0xFFFF0000) >> 16) | ((x & 0x0000FFFF) << 16);
            // swap adjacent 8-bit blocks
            return ((x & 0xFF00FF00) >> 8) | ((x & 0x00FF00FF) << 8);
        }

        static long SwapBytes64(long x) { unchecked { return (long) SwapBytes64((ulong) x); } }
        static ulong SwapBytes64(ulong x)
        {
            // swap adjacent 32-bit blocks
            x = (x >> 32) | (x << 32);
            // swap adjacent 16-bit blocks
            x = ((x & 0xFFFF0000FFFF0000) >> 16) | ((x & 0x0000FFFF0000FFFF) << 16);
            // swap adjacent 8-bit blocks
            return ((x & 0xFF00FF00FF00FF00) >> 8) | ((x & 0x00FF00FF00FF00FF) << 8);
        }

        public static short Int16BE(this ISerializer s, string name, short existing) => SwapBytes16(s.Int16(name, SwapBytes16(existing)));
        public static int Int32BE(this ISerializer s, string name, int existing) => SwapBytes32(s.Int32(name, SwapBytes32(existing)));
        public static long Int64BE(this ISerializer s, string name, long existing) => SwapBytes64(s.Int64(name, SwapBytes64(existing)));

        public static ushort UInt16BE(this ISerializer s, string name, ushort existing) => SwapBytes16(s.UInt16(name, SwapBytes16(existing)));
        public static uint UInt32BE(this ISerializer s, string name, uint existing) => SwapBytes32(s.UInt32(name, SwapBytes32(existing)));
        public static ulong UInt64BE(this ISerializer s, string name, ulong existing) => SwapBytes64(s.UInt64(name, SwapBytes64(existing)));

        public static T EnumU16BE<T>(this ISerializer s, string name, T existing)  where T : struct, Enum

            => (T)(object)SwapBytes16(s.UInt16(name, SwapBytes16((ushort)(object)existing)));
        public static T EnumU32BE<T>(this ISerializer s, string name, T existing)  where T : struct, Enum

            => (T)(object)SwapBytes32(s.UInt32(name, SwapBytes32((uint)(object)existing)));
    }
}
