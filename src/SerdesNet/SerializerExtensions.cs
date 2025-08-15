using System;
using System.Collections.Generic;

namespace SerdesNet;

/// <summary>
/// Various extension methods to supplement the functionality of ISerdes instances
/// </summary>
public static class SerializerExtensions
{
    /// <summary>
    /// Helper method to determine if the serdes instance reads data.
    /// </summary>
    public static bool IsReading(this ISerdes s) => (s.Flags & SerializerFlags.Read) != 0;

    /// <summary>
    /// Helper method to determine if the serdes instance writes data.
    /// </summary>
    public static bool IsWriting(this ISerdes s) => (s.Flags & SerializerFlags.Write) != 0;

    /// <summary>
    /// Helper method to determine if the serdes instance generates comments.
    /// </summary>
    public static bool IsCommenting(this ISerdes s) => (s.Flags & SerializerFlags.Comments) != 0;

    /// <summary>
    /// Allows raising assertions without allocating a string when the condition is true.
    /// Useful for cases where generating the assertion string would have a performance impact.
    /// </summary>
    /// <typeparam name="T">The type of context required for building the message</typeparam>
    /// <param name="s">The serializer</param>
    /// <param name="condition">The assertion condition</param>
    /// <param name="context">The context required for building the message</param>
    /// <param name="messageBuilder">A function which generates the assertion message based on the context</param>
    public static void Assert<T>(this ISerdes s, bool condition, T context, Func<T, string> messageBuilder)
    {
        if (!condition)
            s.Assert(false, messageBuilder(context));
    }

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// (De)serializes an enum value with an underlying type of ushort (big-endian).
    /// </summary>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static short Int16BE(this ISerdes s, SerdesName name, short value) => SwapBytes16(s.Int16(name, SwapBytes16(value)));

    /// <summary>
    /// (De)serializes an enum value with an underlying type of uint (big-endian).
    /// </summary>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static int Int32BE(this ISerdes s, SerdesName name, int value) => SwapBytes32(s.Int32(name, SwapBytes32(value)));

    /// <summary>
    /// (De)serializes an enum value with an underlying type of ulong (big-endian).
    /// </summary>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static long Int64BE(this ISerdes s, SerdesName name, long value) => SwapBytes64(s.Int64(name, SwapBytes64(value)));

    /// <summary>
    /// (De)serializes an enum value with an underlying type of ushort (big-endian).
    /// </summary>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static ushort UInt16BE(this ISerdes s, SerdesName name, ushort value) => SwapBytes16(s.UInt16(name, SwapBytes16(value)));

    /// <summary>
    /// (De)serializes an enum value with an underlying type of uint (big-endian).
    /// </summary>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static uint UInt32BE(this ISerdes s, SerdesName name, uint value) => SwapBytes32(s.UInt32(name, SwapBytes32(value)));

    /// <summary>
    /// (De)serializes an enum value with an underlying type of ulong (big-endian).
    /// </summary>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static ulong UInt64BE(this ISerdes s, SerdesName name, ulong value) => SwapBytes64(s.UInt64(name, SwapBytes64(value)));

    /// <summary>
    /// (De)serializes an enum value with an underlying type of ushort (big-endian).
    /// </summary>
    /// <typeparam name="T">The enum type to (de)serialize</typeparam>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static T EnumU16BE<T>(this ISerdes s, SerdesName name, T value)  where T : unmanaged, Enum
        => (T)(object)SwapBytes16(s.UInt16(name, SwapBytes16((ushort)(object)value)));

    /// <summary>
    /// (De)serializes an enum value with an underlying type of uint (big-endian).
    /// </summary>
    /// <typeparam name="T">The enum type to (de)serialize</typeparam>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static T EnumU32BE<T>(this ISerdes s, SerdesName name, T value)  where T : unmanaged, Enum
        => (T)(object)SwapBytes32(s.UInt32(name, SwapBytes32((uint)(object)value)));

    // ReSharper restore InconsistentNaming

    /// <summary>
    /// Invokes a serdes method inside a named scope.
    /// </summary>
    /// <typeparam name="T">The type of object being (de)serialized</typeparam>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <param name="serdesMethod">The method responsible for the actual (de)serialization</param>
    /// <returns></returns>
    public static T Object<T>(this ISerdes s, SerdesName name, T value, SerdesMethod<T> serdesMethod)
    {
        s.Begin(name);
        var result = serdesMethod(name, value, s);
        s.End();
        return result;
    }

    /// <summary>
    /// Invokes a serdes method inside a named scope with a context value.
    /// </summary>
    /// <typeparam name="T">The type of object being (de)serialized</typeparam>
    /// <typeparam name="TContext">The type of context data that will be passed through to the serdes function.</typeparam>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <param name="context">The context data to pass through to the serdes function</param>
    /// <param name="serdesMethod">The method responsible for the actual (de)serialization</param>
    /// <returns></returns>
    public static T Object<T, TContext>(
        this ISerdes s,
        SerdesName name,
        T value,
        TContext context,
        SerdesContextMethod<T, TContext> serdesMethod)
    {
        s.Begin(name);
        var result = serdesMethod(name, value, context, s);
        s.End();
        return result;
    }

    /// <summary>
    /// Invokes a non-specific serdes method inside a named scope with a context value.
    /// </summary>
    /// <typeparam name="TContext">The type of context data that will be passed through to the serdes function.</typeparam>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="context">The context data to pass through to the serdes function</param>
    /// <param name="serdesMethod">The method responsible for the actual (de)serialization</param>
    public static void Object<TContext>(
        this ISerdes s,
        SerdesName name,
        TContext context,
        Action<TContext, ISerdes> serdesMethod)
    {
        s.Begin(name);
        serdesMethod(context, s);
        s.End();
    }

    /// <summary>
    /// (De)serializes an enum value with an underlying type of byte.
    /// </summary>
    /// <typeparam name="T">The type of enum to (de)serialize</typeparam>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static T Enum8<T>(this ISerdes s, SerdesName name, T value) where T : unmanaged, Enum
    {
        var numeric = SerdesUtil.EnumToSByte(value);
        numeric = s.Int8(name, numeric);

        if (s.IsCommenting())
            s.Comment(value.ToString(), true);

        return SerdesUtil.SByteToEnum<T>(numeric);
    }

    /// <summary>
    /// (De)serializes an enum value with an underlying type of ushort (little-endian).
    /// </summary>
    /// <typeparam name="T">The type of enum to (de)serialize</typeparam>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static T Enum16<T>(this ISerdes s, SerdesName name, T value) where T : unmanaged, Enum
    {
        var numeric = SerdesUtil.EnumToShort(value);
        numeric = s.Int16(name, numeric);

        if (s.IsCommenting())
            s.Comment(value.ToString(), true);

        return SerdesUtil.ShortToEnum<T>(numeric);
    }

    /// <summary>
    /// (De)serializes an enum value with an underlying type of uint (little-endian).
    /// </summary>
    /// <typeparam name="T">The type of enum to (de)serialize</typeparam>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static T Enum32<T>(this ISerdes s, SerdesName name, T value) where T : unmanaged, Enum
    {
        var numeric = SerdesUtil.EnumToInt(value);
        numeric = s.Int32(name, numeric);

        if (s.IsCommenting())
            s.Comment(value.ToString(), true);

        return SerdesUtil.IntToEnum<T>(numeric);
    }

    /// <summary>
    /// (De)serializes an enum value with an underlying type of byte.
    /// </summary>
    /// <typeparam name="T">The type of enum to (de)serialize</typeparam>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static T EnumU8<T>(this ISerdes s, SerdesName name, T value) where T : unmanaged, Enum
    {
        var numeric = SerdesUtil.EnumToByte(value);
        numeric = s.UInt8(name, numeric);

        if (s.IsCommenting())
            s.Comment(value.ToString(), true);

        return SerdesUtil.ByteToEnum<T>(numeric);
    }

    /// <summary>
    /// (De)serializes an enum value with an underlying type of ushort (little-endian).
    /// </summary>
    /// <typeparam name="T">The type of enum to (de)serialize</typeparam>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static T EnumU16<T>(this ISerdes s, SerdesName name, T value) where T : unmanaged, Enum
    {
        var numeric = SerdesUtil.EnumToUShort(value);
        numeric = s.UInt16(name, numeric);

        if (s.IsCommenting())
            s.Comment(value.ToString(), true);

        return SerdesUtil.UShortToEnum<T>(numeric);
    }

    /// <summary>
    /// (De)serializes an enum value with an underlying type of uint (little-endian).
    /// </summary>
    /// <typeparam name="T">The type of enum to (de)serialize</typeparam>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static T EnumU32<T>(this ISerdes s, SerdesName name, T value) where T : unmanaged, Enum
    {
        var numeric = SerdesUtil.EnumToUInt(value);
        numeric = s.UInt32(name, numeric);

        if (s.IsCommenting())
            s.Comment(value.ToString(), true);

        return SerdesUtil.UIntToEnum<T>(numeric);
    }

    static short SwapBytes16(short x)
    {
        unchecked
        {
            return (short)SwapBytes16((ushort)x);
        }
    }

    static ushort SwapBytes16(ushort x)
    {
        // swap adjacent 8-bit blocks
        ushort a = (ushort)((x & 0xFF00) >> 8);
        ushort b = (ushort)((x & 0x00FF) << 8);
        return (ushort)(a | b);
    }

    static int SwapBytes32(int x)
    {
        unchecked
        {
            return (int)SwapBytes32((uint)x);
        }
    }

    static uint SwapBytes32(uint x)
    {
        x = ((x & 0xFFFF0000) >> 16) | ((x & 0x0000FFFF) << 16); // swap adjacent 16-bit blocks
        return ((x & 0xFF00FF00) >> 8) | ((x & 0x00FF00FF) << 8); // swap adjacent 8-bit blocks
    }

    static long SwapBytes64(long x) { unchecked { return (long) SwapBytes64((ulong) x); } }
    static ulong SwapBytes64(ulong x)
    {
        x = (x >> 32) | (x << 32); // swap adjacent 32-bit blocks
        x = ((x & 0xFFFF0000FFFF0000) >> 16) | ((x & 0x0000FFFF0000FFFF) << 16); // swap adjacent 16-bit blocks
        return ((x & 0xFF00FF00FF00FF00) >> 8) | ((x & 0x00FF00FF00FF00FF) << 8); // swap adjacent 8-bit blocks
    }

    /// <summary>
    /// (De)serializes a list of values.
    /// </summary>
    /// <typeparam name="TTarget">The type of value being (de)serialized</typeparam>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the list of values, only used for annotation</param>
    /// <param name="list">The list of values to serialize. When deserializing, this list will be populated and returned if non-null, otherwise a new list will be created</param>
    /// <param name="count">The number of elements to (de)serialize</param>
    /// <param name="serdes">The function to use to (de)serialize each element in the list</param>
    /// <param name="initialiser">An optional initializer to use when list is null during deserialization. If no initializer is supplied, then a new System.Collections.Generic.List will be created</param>
    /// <returns>The list of values. This may be the same object as list</returns>
    public static IList<TTarget> List<TTarget>(
        this ISerdes s,
        SerdesName name,
        IList<TTarget> list,
        int count,
        SerdesMethod<TTarget> serdes,
        Func<int, IList<TTarget>> initialiser = null)
        => s.List(name, list, count, 0, serdes, initialiser);

    /// <summary>
    /// (De)serializes a list of values.
    /// </summary>
    /// <typeparam name="TTarget">The type of value being (de)serialized</typeparam>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the list of values, only used for annotation</param>
    /// <param name="list">The list of values to serialize. When deserializing, this list will be populated and returned if non-null, otherwise a new list will be created</param>
    /// <param name="count">The number of elements to (de)serialize</param>
    /// <param name="offset">The position in the list to start populating/reading from. When deserializing and the list has fewer elements than offset, default-initialised values will be added until the length reaches the offset</param>
    /// <param name="serdes">The function to use to (de)serialize each element in the list</param>
    /// <param name="initialiser">An optional initializer to use when list is null during deserialization. If no initializer is supplied, then a new System.Collections.Generic.List will be created</param>
    /// <returns>The list of values. This may be the same object as list</returns>
    public static IList<TTarget> List<TTarget>(
        this ISerdes s,
        SerdesName name,
        IList<TTarget> list,
        int count,
        int offset,
        SerdesMethod<TTarget> serdes,
        Func<int, IList<TTarget>> initialiser = null)
    {
        s.Begin(name);

        list ??= initialiser?.Invoke(count) ?? (List<TTarget>)[];

        for (int i = offset; i < offset + count; i++)
        {
            while (list.Count <= i)
                list.Add(default);

            var x = serdes(i, list[i], s);
            list[i] = x;
        }

        s.End();
        return list;
    }

    /// <summary>
    /// (De)serializes a list of values.
    /// </summary>
    /// <typeparam name="TTarget">The type of value being (de)serialized</typeparam>
    /// <typeparam name="TContext">The type of context data that will be passed through to the serdes function.</typeparam>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the list of values, only used for annotation</param>
    /// <param name="list">The list of values to serialize. When deserializing, this list will be populated and returned if non-null, otherwise a new list will be created</param>
    /// <param name="context">The context data to pass through to the serdes function</param>
    /// <param name="count">The number of elements to (de)serialize</param>
    /// <param name="serdes">The function to use to (de)serialize each element in the list</param>
    /// <param name="initialiser">An optional initializer to use when list is null during deserialization. If no initializer is supplied, then a new System.Collections.Generic.List will be created</param>
    /// <returns>The list of values. This may be the same object as list</returns>
    public static IList<TTarget> ListWithContext<TTarget, TContext>(
        this ISerdes s,
        SerdesName name,
        IList<TTarget> list,
        TContext context,
        int count,
        SerdesContextMethod<TTarget, TContext> serdes,
        Func<int, IList<TTarget>> initialiser = null)
    => s.ListWithContext(name, list, context, count, 0, serdes, initialiser);

    /// <summary>
    /// (De)serializes a list of values.
    /// </summary>
    /// <typeparam name="TTarget">The type of value being (de)serialized</typeparam>
    /// <typeparam name="TContext">The type of context data that will be passed through to the serdes function.</typeparam>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the list of values, only used for annotation</param>
    /// <param name="list">The list of values to serialize. When deserializing, this list will be populated and returned if non-null, otherwise a new list will be created</param>
    /// <param name="context">The context data to pass through to the serdes function</param>
    /// <param name="count">The number of elements to (de)serialize</param>
    /// <param name="offset">The position in the list to start populating/reading from. When deserializing and the list has fewer elements than offset, default-initialised values will be added until the length reaches the offset</param>
    /// <param name="serdes">The function to use to (de)serialize each element in the list</param>
    /// <param name="initialiser">An optional initializer to use when list is null during deserialization. If no initializer is supplied, then a new System.Collections.Generic.List will be created</param>
    /// <returns>The list of values. This may be the same object as list</returns>
    public static IList<TTarget> ListWithContext<TTarget, TContext>(
        this ISerdes s,
        SerdesName name,
        IList<TTarget> list,
        TContext context,
        int count,
        int offset,
        SerdesContextMethod<TTarget, TContext> serdes,
        Func<int, IList<TTarget>> initialiser = null)
    {
        s.Begin(name);

        list ??= initialiser?.Invoke(count) ?? (List<TTarget>)[];

        for (int i = offset; i < offset + count; i++)
        {
            while (list.Count <= i)
                list.Add(default);

            var x = serdes(i, list[i], context, s);
            list[i] = x;
        }

        s.End();
        return list;
    }
}