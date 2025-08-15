using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace SerdesNet;

internal static class SerdesUtil
{
    const string HexChars = "0123456789ABCDEF";
    public static string ConvertToHexString(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length == 0)
            return "";

        var result = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
        {
            result.Append(HexChars[b >> 4]);
            result.Append(HexChars[b & 0xf]);
        }

        return result.ToString();
    }

    public static sbyte EnumToSByte<T>(T value) where T : unmanaged, Enum
    {
        unsafe
        {
            return
                sizeof(T) == 1 ? Unsafe.As<T, sbyte>(ref value)
                    : throw new InvalidOperationException($"Type {typeof(T)} is of non-enum type, or has an unsupported underlying type");
        }
    }
    public static short EnumToShort<T>(T value) where T : unmanaged, Enum
    {
        unsafe
        {
            return
                sizeof(T) == 1 ? Unsafe.As<T, sbyte>(ref value)
                : sizeof(T) == 2 ? Unsafe.As<T, short>(ref value)
                : throw new InvalidOperationException($"Type {typeof(T)} is of non-enum type, or has an unsupported underlying type");
        }
    }
    public static int EnumToInt<T>(T value) where T : unmanaged, Enum
    {
        unsafe
        {
            return
                sizeof(T) == 1 ? Unsafe.As<T, sbyte>(ref value)
                : sizeof(T) == 2 ? Unsafe.As<T, short>(ref value)
                : sizeof(T) == 4 ? Unsafe.As<T, int>(ref value)
                : throw new InvalidOperationException($"Type {typeof(T)} is of non-enum type, or has an unsupported underlying type");
        }
    }

    public static byte EnumToByte<T>(T value) where T : unmanaged, Enum
    {
        unsafe
        {
            return
                sizeof(T) == 1 ? Unsafe.As<T, byte>(ref value)
                    : throw new InvalidOperationException($"Type {typeof(T)} is of non-enum type, or has an unsupported underlying type");
        }
    }
    public static ushort EnumToUShort<T>(T value) where T : unmanaged, Enum
    {
        unsafe
        {
            return
                sizeof(T) == 1 ? Unsafe.As<T, byte>(ref value)
                : sizeof(T) == 2 ? Unsafe.As<T, ushort>(ref value)
                : throw new InvalidOperationException($"Type {typeof(T)} is of non-enum type, or has an unsupported underlying type");
        }
    }
    public static uint EnumToUInt<T>(T value) where T : unmanaged, Enum
    {
        unsafe
        {
            return
                sizeof(T) == 1 ? Unsafe.As<T, byte>(ref value)
                : sizeof(T) == 2 ? Unsafe.As<T, ushort>(ref value)
                : sizeof(T) == 4 ? Unsafe.As<T, uint>(ref value)
                : throw new InvalidOperationException($"Type {typeof(T)} is of non-enum type, or has an unsupported underlying type");
        }
    }

    public static T SByteToEnum<T>(sbyte value) where T : unmanaged, Enum
    {
        unsafe
        {
            if (sizeof(T) == 1)
                return Unsafe.As<sbyte, T>(ref value);

            if (sizeof(T) == 2)
            {
                short ushortValue = value;
                return Unsafe.As<short, T>(ref ushortValue);
            }

            if (sizeof(T) == 4)
            {
                int uintValue = value;
                return Unsafe.As<int, T>(ref uintValue);
            }

            throw new InvalidOperationException($"Type {typeof(T)} is of non-enum type, or has an unsupported underlying type");
        }
    }

    public static T ShortToEnum<T>(short value) where T : unmanaged, Enum
    {
        unsafe
        {
            if (sizeof(T) == 2)
                return Unsafe.As<short, T>(ref value);

            if (sizeof(T) == 4)
            {
                int uintValue = value;
                return Unsafe.As<int, T>(ref uintValue);
            }

            throw new InvalidOperationException($"Type {typeof(T)} is of non-enum type, or has an unsupported underlying type");
        }
    }

    public static T IntToEnum<T>(int value) where T : unmanaged, Enum
    {
        unsafe
        {
            if (sizeof(T) == 4)
                return Unsafe.As<int, T>(ref value);

            throw new InvalidOperationException($"Type {typeof(T)} is of non-enum type, or has an unsupported underlying type");
        }
    }

    public static T ByteToEnum<T>(byte value) where T : unmanaged, Enum
    {
        unsafe
        {
            if (sizeof(T) == 1)
                return Unsafe.As<byte, T>(ref value);

            if (sizeof(T) == 2)
            {
                ushort ushortValue = value;
                return Unsafe.As<ushort, T>(ref ushortValue);
            }

            if (sizeof(T) == 4)
            {
                uint uintValue = value;
                return Unsafe.As<uint, T>(ref uintValue);
            }

            throw new InvalidOperationException($"Type {typeof(T)} is of non-enum type, or has an unsupported underlying type");
        }
    }

    public static T UShortToEnum<T>(ushort value) where T : unmanaged, Enum
    {
        unsafe
        {
            if (sizeof(T) == 2)
                return Unsafe.As<ushort, T>(ref value);

            if (sizeof(T) == 4)
            {
                uint uintValue = value;
                return Unsafe.As<uint, T>(ref uintValue);
            }

            throw new InvalidOperationException($"Type {typeof(T)} is of non-enum type, or has an unsupported underlying type");
        }
    }

    public static T UIntToEnum<T>(uint value) where T : unmanaged, Enum
    {
        unsafe
        {
            if (sizeof(T) == 4)
                return Unsafe.As<uint, T>(ref value);

            throw new InvalidOperationException($"Type {typeof(T)} is of non-enum type, or has an unsupported underlying type");
        }
    }
}