using System;
using System.Globalization;

namespace SerdesNet
{
    public class MaxToNullConverter : 
        IConverter<byte,   byte?>,
        IConverter<sbyte,  sbyte?>,
        IConverter<ushort, ushort?>,
        IConverter<short,  short?>,
        IConverter<uint,   uint?>,
        IConverter<int,    int?>,
        IConverter<ulong,  ulong?>,
        IConverter<long,   long?>
    {
        public static readonly MaxToNullConverter Instance = new MaxToNullConverter();
        MaxToNullConverter() { }
        public static byte? Serdes(string name, byte? value, Func<string, byte, byte, byte> serializer)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            return Instance.FromNumeric(serializer(name, Instance.ToNumeric(value), 0));
        }

        public static sbyte? Serdes(string name, sbyte? value, Func<string, sbyte, sbyte, sbyte> serializer)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            return Instance.FromNumeric(serializer(name, Instance.ToNumeric(value), 0));
        }

        public static ushort? Serdes(string name, ushort? value, Func<string, ushort, ushort, ushort> serializer)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            return Instance.FromNumeric(serializer(name, Instance.ToNumeric(value), 0));
        }

        public static short? Serdes(string name, short? value, Func<string, short, short, short> serializer)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            return Instance.FromNumeric(serializer(name, Instance.ToNumeric(value), 0));
        }

        public static uint? Serdes(string name, uint? value, Func<string, uint, uint, uint> serializer)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            return Instance.FromNumeric(serializer(name, Instance.ToNumeric(value), 0));
        }

        public static int? Serdes(string name, int? value, Func<string, int, int, int> serializer)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            return Instance.FromNumeric(serializer(name, Instance.ToNumeric(value), 0));
        }

        public static ulong? Serdes(string name, ulong? value, Func<string, ulong, ulong, ulong> serializer)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            return Instance.FromNumeric(serializer(name, Instance.ToNumeric(value), 0));
        }

        public static long? Serdes(string name, long? value, Func<string, long, long, long> serializer)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            return Instance.FromNumeric(serializer(name, Instance.ToNumeric(value), 0));
        }

        public byte ToNumeric(byte? memory) => memory ?? byte.MaxValue;
        public byte? FromNumeric(byte persistent) => persistent == byte.MaxValue ? (byte?)null : persistent;
        public string ToSymbolic(byte? memory) => memory?.ToString(CultureInfo.InvariantCulture);
        byte? IConverter<byte, byte?>.FromSymbolic(string symbolic) => symbolic == null ? null : (byte?)byte.Parse(symbolic, CultureInfo.InvariantCulture);
        public sbyte ToNumeric(sbyte? memory) => memory ?? sbyte.MaxValue;
        public sbyte? FromNumeric(sbyte persistent) => persistent == sbyte.MaxValue ? (sbyte?)null : persistent;
        public string ToSymbolic(sbyte? memory) => memory?.ToString(CultureInfo.InvariantCulture);
        sbyte? IConverter<sbyte, sbyte?>.FromSymbolic(string symbolic) => symbolic == null ? null : (sbyte?)sbyte.Parse(symbolic, CultureInfo.InvariantCulture);
        public ushort ToNumeric(ushort? memory) => memory ?? ushort.MaxValue;
        public ushort? FromNumeric(ushort persistent) => persistent == ushort.MaxValue ? (ushort?)null : persistent;
        public string ToSymbolic(ushort? memory) => memory?.ToString(CultureInfo.InvariantCulture);
        ushort? IConverter<ushort, ushort?>.FromSymbolic(string symbolic) => symbolic == null ? null : (ushort?)ushort.Parse(symbolic, CultureInfo.InvariantCulture);
        public short ToNumeric(short? memory) => memory ?? short.MaxValue;
        public short? FromNumeric(short persistent) => persistent == short.MaxValue ? (short?)null : persistent;
        public string ToSymbolic(short? memory) => memory?.ToString(CultureInfo.InvariantCulture);
        short? IConverter<short, short?>.FromSymbolic(string symbolic) => symbolic == null ? null : (short?)short.Parse(symbolic, CultureInfo.InvariantCulture);
        public uint ToNumeric(uint? memory) => memory ?? uint.MaxValue;
        public uint? FromNumeric(uint persistent) => persistent == uint.MaxValue ? (uint?)null : persistent;
        public string ToSymbolic(uint? memory) => memory?.ToString(CultureInfo.InvariantCulture);
        uint? IConverter<uint, uint?>.FromSymbolic(string symbolic) => symbolic == null ? null : (uint?)uint.Parse(symbolic, CultureInfo.InvariantCulture);
        public int ToNumeric(int? memory) => memory ?? int.MaxValue;
        public int? FromNumeric(int persistent) => persistent == int.MaxValue ? (int?)null : persistent;
        public string ToSymbolic(int? memory) => memory?.ToString(CultureInfo.InvariantCulture);
        int? IConverter<int, int?>.FromSymbolic(string symbolic) => symbolic == null ? null : (int?)int.Parse(symbolic, CultureInfo.InvariantCulture);
        public ulong ToNumeric(ulong? memory) => memory ?? ulong.MaxValue;
        public ulong? FromNumeric(ulong persistent) => persistent == ulong.MaxValue ? (ulong?)null : persistent;
        public string ToSymbolic(ulong? memory) => memory?.ToString(CultureInfo.InvariantCulture);
        ulong? IConverter<ulong, ulong?>.FromSymbolic(string symbolic) => symbolic == null ? null : (ulong?)ulong.Parse(symbolic, CultureInfo.InvariantCulture);
        public long ToNumeric(long? memory) => memory ?? long.MaxValue;
        public long? FromNumeric(long persistent) => persistent == long.MaxValue ? (long?)null : persistent;
        public string ToSymbolic(long? memory) => memory?.ToString(CultureInfo.InvariantCulture);
        long? IConverter<long, long?>.FromSymbolic(string symbolic) => symbolic == null ? null : (long?)long.Parse(symbolic, CultureInfo.InvariantCulture);
    }

    public sealed class MaxToNullConverter<T> :
        IConverter<byte, T?>,
        IConverter<sbyte, T?>,
        IConverter<ushort, T?>,
        IConverter<short, T?>,
        IConverter<uint, T?>,
        IConverter<int, T?>,
        IConverter<ulong, T?>,
        IConverter<long, T?>
        where T : unmanaged, Enum
    {
        public static readonly MaxToNullConverter<T> Instance = new MaxToNullConverter<T>();
        MaxToNullConverter() { }
        T? IConverter<  byte, T?>.FromNumeric(  byte x) => x == byte.MaxValue ? null   : (T?)(T)(object)x;
        T? IConverter< sbyte, T?>.FromNumeric( sbyte x) => x == sbyte.MaxValue ? null  : (T?)(T)(object)x;
        T? IConverter<ushort, T?>.FromNumeric(ushort x) => x == ushort.MaxValue ? null : (T?)(T)(object)x;
        T? IConverter< short, T?>.FromNumeric( short x) => x == short.MaxValue ? null  : (T?)(T)(object)x;
        T? IConverter<  uint, T?>.FromNumeric(  uint x) => x == uint.MaxValue ? null   : (T?)(T)(object)x;
        T? IConverter<   int, T?>.FromNumeric(   int x) => x == int.MaxValue ? null    : (T?)(T)(object)x;
        T? IConverter< ulong, T?>.FromNumeric( ulong x) => x == ulong.MaxValue ? null  : (T?)(T)(object)x;
        T? IConverter<  long, T?>.FromNumeric(  long x) => x == long.MaxValue ? null   : (T?)(T)(object)x;

        byte   IConverter<  byte, T?>.ToNumeric(T? x) => x == null ? byte.MaxValue   : (  byte)(object)x.Value;
        sbyte  IConverter< sbyte, T?>.ToNumeric(T? x) => x == null ? sbyte.MaxValue  : ( sbyte)(object)x.Value;
        ushort IConverter<ushort, T?>.ToNumeric(T? x) => x == null ? ushort.MaxValue : (ushort)(object)x.Value;
        short  IConverter< short, T?>.ToNumeric(T? x) => x == null ? short.MaxValue  : ( short)(object)x.Value;
        uint   IConverter<  uint, T?>.ToNumeric(T? x) => x == null ? uint.MaxValue   : (  uint)(object)x.Value;
        int    IConverter<   int, T?>.ToNumeric(T? x) => x == null ? int.MaxValue    : (   int)(object)x.Value;
        ulong  IConverter< ulong, T?>.ToNumeric(T? x) => x == null ? ulong.MaxValue  : ( ulong)(object)x.Value;
        long   IConverter<  long, T?>.ToNumeric(T? x) => x == null ? long.MaxValue   : (  long)(object)x.Value;

        public string ToSymbolic(T? x) => x == null ? null : Enum.GetName(typeof(T), x.Value);
        public T? FromSymbolic(string x) => x == null ? null : (T?)(T)Enum.Parse(typeof(T), x);
    }
}
