using System;
using System.Collections.Generic;

namespace SerdesNet
{
    public delegate T SerdesMethod<T>(int n, T value, ISerializer s);
    public delegate T SerdesContextMethod<T, in TContext>(int n, T value, TContext context, ISerializer s);
    public delegate T NamedSerdesMethod<T>(string name, T value, ISerializer s);
    public delegate T NamedSerdesContextMethod<T, in TContext>(string name, T value, TContext context, ISerializer s);

    public interface ISerializer : IDisposable
    {
        SerializerFlags Flags { get; }
        long Offset { get; } // For recording offsets to be overwritten later
        long BytesRemaining { get; }
        void Comment(string comment, bool inline = false); // Only affects annotating writers
        void Begin(string name = null); // Only affects annotating writers
        void End(); // Only affects annotating writers
        void NewLine(); // Only affects annotating writers
        void Seek(long offset); // For overwriting pre-recorded offsets
        void Assert(bool condition, string message);

        void Pad(int count, byte value = 0);
        void Pad(string name, int count, byte value = 0);

        sbyte Int8(int n, sbyte value, sbyte defaultValue = 0);
        short Int16(int n, short value, short defaultValue = 0);
        int Int32(int n, int value, int defaultValue = 0);
        long Int64(int n, long value, long defaultValue = 0);
        byte UInt8(int n, byte value, byte defaultValue = 0);
        ushort UInt16(int n, ushort value, ushort defaultValue = 0);
        uint UInt32(int n, uint value, uint defaultValue = 0);
        ulong UInt64(int n, ulong value, ulong defaultValue = 0);

        sbyte Int8(string name, sbyte value, sbyte defaultValue = 0);
        short Int16(string name, short value, short defaultValue = 0);
        int Int32(string name, int value, int defaultValue = 0);
        long Int64(string name, long value, long defaultValue = 0);
        byte UInt8(string name, byte value, byte defaultValue = 0);
        ushort UInt16(string name, ushort value, ushort defaultValue = 0);
        uint UInt32(string name, uint value, uint defaultValue = 0);
        ulong UInt64(string name, ulong value, ulong defaultValue = 0);

        T EnumU8<T>(int n, T value) where T : unmanaged, Enum;
        T EnumU16<T>(int n, T value) where T : unmanaged, Enum;
        T EnumU32<T>(int n, T value) where T : unmanaged, Enum;

        T EnumU8<T>(string name, T value) where T : unmanaged, Enum;
        T EnumU16<T>(string name, T value) where T : unmanaged, Enum;
        T EnumU32<T>(string name, T value) where T : unmanaged, Enum;

        Guid Guid(string name, Guid value);
        byte[] Bytes(string name, byte[] value, int length);
        string NullTerminatedString(string name, string value);
        string FixedLengthString(string name, string value, int length);

        IList<TTarget> List<TTarget>(
            string name,
            IList<TTarget> list,
            int count,
            SerdesMethod<TTarget> serdes,
            Func<int, IList<TTarget>> initialiser = null);

        IList<TTarget> List<TTarget>(
            string name,
            IList<TTarget> list,
            int count,
            int offset,
            SerdesMethod<TTarget> serdes,
            Func<int, IList<TTarget>> initialiser = null);

        IList<TTarget> List<TTarget, TContext>(
            string name,
            IList<TTarget> list,
            TContext context,
            int count,
            SerdesContextMethod<TTarget, TContext> serdes,
            Func<int, IList<TTarget>> initialiser = null);

        IList<TTarget> List<TTarget, TContext>(
            string name,
            IList<TTarget> list,
            TContext context,
            int count,
            int offset,
            SerdesContextMethod<TTarget, TContext> serdes,
            Func<int, IList<TTarget>> initialiser = null);
    }

    public static class S // Helpers exposing the common ISerializer methods as SerdesMethods for passing to ISerializer.List, Object etc
    {
        public static SerdesMethod<T> Object<T>(SerdesMethod<T> serdes) => (i, x, s) => s.Object(i, x, serdes);

        public static sbyte    Int8(int n, sbyte value, ISerializer s) =>  s.Int8(n, value);
        public static short   Int16(int n, short value, ISerializer s) => s.Int16(n, value);
        public static int     Int32(int n, int   value, ISerializer s) => s.Int32(n, value);
        public static long    Int64(int n, long  value, ISerializer s) => s.Int64(n, value);

        public static byte    UInt8(int n, byte   value, ISerializer s) =>  s.UInt8(n, value);
        public static ushort UInt16(int n, ushort value, ISerializer s) => s.UInt16(n, value);
        public static uint   UInt32(int n, uint   value, ISerializer s) => s.UInt32(n, value);
        public static ulong  UInt64(int n, ulong  value, ISerializer s) => s.UInt64(n, value);

        public static sbyte    Int8(string name, sbyte value, ISerializer s) =>  s.Int8(name, value);
        public static short   Int16(string name, short value, ISerializer s) => s.Int16(name, value);
        public static int     Int32(string name, int   value, ISerializer s) => s.Int32(name, value);
        public static long    Int64(string name, long  value, ISerializer s) => s.Int64(name, value);

        public static byte    UInt8(string name, byte   value, ISerializer s) =>  s.UInt8(name, value);
        public static ushort UInt16(string name, ushort value, ISerializer s) => s.UInt16(name, value);
        public static uint   UInt32(string name, uint   value, ISerializer s) => s.UInt32(name, value);
        public static ulong  UInt64(string name, ulong  value, ISerializer s) => s.UInt64(name, value);

        public static T  EnumU8<T>(int i, T v, ISerializer s) where T : unmanaged, Enum =>  s.EnumU8(i, v);
        public static T EnumU16<T>(int i, T v, ISerializer s) where T : unmanaged, Enum => s.EnumU16(i, v);
        public static T EnumU32<T>(int i, T v, ISerializer s) where T : unmanaged, Enum => s.EnumU32(i, v);

        public static Guid Guid(string name, Guid value, ISerializer s) => s.Guid(name, value);
        public static string NullTerminatedString(string name, string value, ISerializer s) => s.NullTerminatedString(name, value);
    }

    public static class SerializerExtensions
    {
        public static bool IsReading(this ISerializer s) => (s.Flags & SerializerFlags.Read) != 0;
        public static bool IsWriting(this ISerializer s) => (s.Flags & SerializerFlags.Write) != 0;
        public static bool IsCommenting(this ISerializer s) => (s.Flags & SerializerFlags.Comments) != 0;

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

        // ReSharper disable InconsistentNaming
        public static short Int16BE(this ISerializer s, int n, short value) => SwapBytes16(s.Int16(n, SwapBytes16(value)));
        public static int   Int32BE(this ISerializer s, int n, int   value) => SwapBytes32(s.Int32(n, SwapBytes32(value)));
        public static long  Int64BE(this ISerializer s, int n, long  value) => SwapBytes64(s.Int64(n, SwapBytes64(value)));

        public static ushort UInt16BE(this ISerializer s, int n, ushort value) => SwapBytes16(s.UInt16(n, SwapBytes16(value)));
        public static uint   UInt32BE(this ISerializer s, int n, uint   value) => SwapBytes32(s.UInt32(n, SwapBytes32(value)));
        public static ulong  UInt64BE(this ISerializer s, int n, ulong  value) => SwapBytes64(s.UInt64(n, SwapBytes64(value)));

        public static short Int16BE(this ISerializer s, string name, short value) => SwapBytes16(s.Int16(name, SwapBytes16(value)));
        public static int   Int32BE(this ISerializer s, string name, int   value) => SwapBytes32(s.Int32(name, SwapBytes32(value)));
        public static long  Int64BE(this ISerializer s, string name, long  value) => SwapBytes64(s.Int64(name, SwapBytes64(value)));

        public static ushort UInt16BE(this ISerializer s, string name, ushort value) => SwapBytes16(s.UInt16(name, SwapBytes16(value)));
        public static uint   UInt32BE(this ISerializer s, string name, uint   value) => SwapBytes32(s.UInt32(name, SwapBytes32(value)));
        public static ulong  UInt64BE(this ISerializer s, string name, ulong  value) => SwapBytes64(s.UInt64(name, SwapBytes64(value)));

        public static T EnumU16BE<T>(this ISerializer s, int n, T value)  where T : unmanaged, Enum
            => (T)(object)SwapBytes16(s.UInt16(n, SwapBytes16((ushort)(object)value)));

        public static T EnumU32BE<T>(this ISerializer s, int n, T value)  where T : unmanaged, Enum
            => (T)(object)SwapBytes32(s.UInt32(n, SwapBytes32((uint)(object)value)));

        public static T EnumU16BE<T>(this ISerializer s, string name, T value)  where T : unmanaged, Enum
            => (T)(object)SwapBytes16(s.UInt16(name, SwapBytes16((ushort)(object)value)));

        public static T EnumU32BE<T>(this ISerializer s, string name, T value)  where T : unmanaged, Enum
           => (T)(object)SwapBytes32(s.UInt32(name, SwapBytes32((uint)(object)value)));

        // ReSharper restore InconsistentNaming

        public static T Object<T>(this ISerializer s, int n, T value, SerdesMethod<T> serdes)
        {
            s.Begin($"{n}");
            var result = serdes(n, value, s);
            s.End();
            return result;
        }
        public static T Object<T>(this ISerializer s, string name, T value, NamedSerdesMethod<T> serdes)
        {
            s.Begin(name);
            var result = serdes(name, value, s);
            s.End();
            return result;
        }

        public static T Object<T, TContext>(this ISerializer s, string name, T value, TContext context, NamedSerdesContextMethod<T, TContext> serdes)
        {
            s.Begin(name);
            var result = serdes(name, value, context, s);
            s.End();
            return result;
        }

        public static void Object<TContext>(this ISerializer s, string name, TContext context, Action<TContext, ISerializer> serdes)
        {
            s.Begin(name);
            serdes(context, s);
            s.End();
        }

        public static T Transform<TNumeric, T>(
            this ISerializer s,
            int n,
            T value,
            SerdesMethod<TNumeric> serializer,
            IConverter<TNumeric, T> converter)
            => converter.FromNumeric(serializer(n, converter.ToNumeric(value), s));

        public static T Transform<TNumeric, T>(
            this ISerializer s,
            string name,
            T value,
            NamedSerdesMethod<TNumeric> serializer,
            IConverter<TNumeric, T> converter)
            => converter.FromNumeric(serializer(name, converter.ToNumeric(value), s));

        public static T TransformEnumU8<T>(
            this ISerializer s,
            string name,
            T value,
            IConverter<byte, T> converter)
            => converter.FromNumeric(s.UInt8(name, converter.ToNumeric(value)));

        public static T TransformEnumU16<T>(
            this ISerializer s,
            string name,
            T value,
            IConverter<ushort, T> converter)
            => converter.FromNumeric(s.UInt16(name, converter.ToNumeric(value)));

        public static T TransformEnumU32<T>(
            this ISerializer s,
            string name,
            T value,
            IConverter<uint, T> converter)
            => converter.FromNumeric(s.UInt32(name, converter.ToNumeric(value)));
    }
}
