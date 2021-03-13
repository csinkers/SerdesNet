using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace SerdesNet
{
    static class SerdesUtil
    {
        const string HexChars = "0123456789ABCDEF";
        public static string ConvertToHexString(byte[] bytes) => ConvertToHexString(bytes, bytes.Length);
        public static string ConvertToHexString(byte[] bytes, int n)
        {
            if (bytes == null) return "";
            var result = new StringBuilder(n * 2);
            for(int i = 0; i < n; i++)
            {
                byte b = bytes[i];
                result.Append(HexChars[b >> 4]);
                result.Append(HexChars[b & 0xf]);
            }

            return result.ToString();
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
                    sizeof(T) == 1   ? Unsafe.As<T, byte>(ref value)
                    : sizeof(T) == 2 ? Unsafe.As<T, ushort>(ref value)
                    : throw new InvalidOperationException($"Type {typeof(T)} is of non-enum type, or has an unsupported underlying type");
            }
        }
        public static uint EnumToUInt<T>(T value) where T : unmanaged, Enum
        {
            unsafe
            {
                return
                    sizeof(T) == 1   ? Unsafe.As<T, byte>(ref value)
                    : sizeof(T) == 2 ? Unsafe.As<T, ushort>(ref value)
                    : sizeof(T) == 4 ? Unsafe.As<T, uint>(ref value)
                    : throw new InvalidOperationException($"Type {typeof(T)} is of non-enum type, or has an unsupported underlying type");
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
}
