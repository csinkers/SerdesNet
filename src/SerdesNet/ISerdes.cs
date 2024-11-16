using System;
using System.Collections.Generic;

namespace SerdesNet;

/// <summary>
/// Interface for classes that serialize or deserialize data.
/// </summary>
public interface ISerdes : IDisposable
{
    /// <summary>
    /// Used to check what kind of serdes is being used, i.e. a serializer or deserializer, and whether annotations are being written.
    /// </summary>
    SerializerFlags Flags { get; }

    /// <summary>
    /// The current offset in the stream (or window, when using a WindowingProxySerializer).
    /// Useful for recording offsets that will need to be overwritten later via Seek calls.
    /// </summary>
    long Offset { get; }

    /// <summary>
    /// The number of bytes remaining in the stream (or in the window into the stream when using a WindowingProxySerializer).
    /// </summary>
    long BytesRemaining { get; }

    /// <summary>
    /// When writing annotations, calls to this method will result in a comment being inserted into the output.
    /// Typically ignored by all serializers other than the AnnotationProxySerializer.
    /// </summary>
    /// <param name="comment">The comment to add.</param>
    /// <param name="inline">When true, comments will be added at the end of the current line. When false, the comment will be placed on its own line.</param>
    void Comment(string comment, bool inline = false);

    /// <summary>
    /// When writing annotations, calls to this method will result in a new annotation block/scope being opened.
    /// Typically ignored by all serializers other than the AnnotationProxySerializer.
    /// </summary>
    /// <param name="name">The name of the block</param>
    void Begin(string name = null);

    /// <summary>
    /// When writing annotations, calls to this method will result in the current annotation block/scope being closed.
    /// Typically ignored by all serializers other than the AnnotationProxySerializer.
    /// </summary>
    void End();

    /// <summary>
    /// When writing annotations, calls to this method will result in a new line being added to the textual output.
    /// Typically ignored by all serializers other than the AnnotationProxySerializer.
    /// </summary>
    void NewLine();

    /// <summary>
    /// Seeks to the specified offset in the stream / window.
    /// Useful when an earlier offset needs to be overwritten with a value that was not yet known when it was first visited.
    /// </summary>
    /// <param name="offset">The offset to seek to</param>
    void Seek(long offset);

    /// <summary>
    /// Used to raise assertions in a configurable way during (de)serialization.
    /// When constructing a serializer, a callback can be passed in to handle assertion failures.
    /// This could be used to throw an exception, log a message, or ignore the assertion.
    /// </summary>
    /// <param name="condition">The result of the assertion condition. When this is false, the assertion callback (if any) will be invoked</param>
    /// <param name="message">The message to pass to the callback when the assertion fails.</param>
    void Assert(bool condition, string message);

    /// <summary>
    /// Used to (de)serialize a block of bytes that should all have the same value.
    /// If a deserializer encounters a byte that does not match the expected value, it will call the assertion callback.
    /// </summary>
    /// <param name="count">The number of bytes in the padding block</param>
    /// <param name="value">The value to pad with</param>
    void Pad(int count, byte value = 0);

    /// <summary>
    /// Used to (de)serialize a block of bytes that should all have the same value.
    /// If a deserializer encounters a byte that does not match the expected value, it will call the assertion callback.
    /// </summary>
    /// <param name="name">The name of the padding block to use when annotating</param>
    /// <param name="count">The number of bytes in the padding block</param>
    /// <param name="value">The value to pad with</param>
    void Pad(string name, int count, byte value = 0);

    /// <summary>
    /// (De)serializes a signed 8-bit integer (i.e. a signed byte in the range -128..127).
    /// </summary>
    /// <param name="n">The index of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    sbyte Int8(int n, sbyte value);

    /// <summary>
    /// (De)serializes a (little-endian) signed 16-bit integer (i.e. a signed short in the range -32768..32767).
    /// </summary>
    /// <param name="n">The index of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    short Int16(int n, short value);

    /// <summary>
    /// (De)serializes a (little-endian) signed 32-bit integer (i.e. a signed int in the range -2147483648..2147483647).
    /// </summary>
    /// <param name="n">The index of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    int Int32(int n, int value);

    /// <summary>
    /// (De)serializes a (little-endian) signed 64-bit integer (i.e. a signed long in the range -9223372036854775808..9223372036854775807).
    /// </summary>
    /// <param name="n">The index of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    long Int64(int n, long value);

    /// <summary>
    /// (De)serializes an (little-endian) unsigned 8-bit integer (i.e. an unsigned byte in the range 0..255).
    /// </summary>
    /// <param name="n">The index of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    byte UInt8(int n, byte value);

    /// <summary>
    /// (De)serializes an (little-endian) unsigned 16-bit integer (i.e. an unsigned short in the range 0..65535).
    /// </summary>
    /// <param name="n">The index of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    ushort UInt16(int n, ushort value);

    /// <summary>
    /// (De)serializes an (little-endian) unsigned 32-bit integer (i.e. an unsigned int in the range 0..4294967295).
    /// </summary>
    /// <param name="n">The index of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    uint UInt32(int n, uint value);

    /// <summary>
    /// (De)serializes an (little-endian) unsigned 64-bit integer (i.e. an unsigned long in the range 0..18446744073709551615).
    /// </summary>
    /// <param name="n">The index of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    ulong UInt64(int n, ulong value);

    /// <summary>
    /// (De)serializes a (little-endian) signed 8-bit integer (i.e. a signed byte in the range -128..127).
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    sbyte Int8(string name, sbyte value);

    /// <summary>
    /// (De)serializes a (little-endian) signed 16-bit integer (i.e. a signed short in the range -32768..32767).
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    short Int16(string name, short value);

    /// <summary>
    /// (De)serializes a (little-endian) signed 32-bit integer (i.e. a signed int in the range -2147483648..2147483647).
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    int Int32(string name, int value);

    /// <summary>
    /// (De)serializes a (little-endian) signed 64-bit integer (i.e. a signed long in the range -9223372036854775808..9223372036854775807).
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    long Int64(string name, long value);

    /// <summary>
    /// (De)serializes an (little-endian) unsigned 8-bit integer (i.e. an unsigned byte in the range 0..255).
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    byte UInt8(string name, byte value);

    /// <summary>
    /// (De)serializes an (little-endian) unsigned 16-bit integer (i.e. an unsigned short in the range 0..65535).
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    ushort UInt16(string name, ushort value);

    /// <summary>
    /// (De)serializes an (little-endian) unsigned 32-bit integer (i.e. an unsigned int in the range 0..4294967295).
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    uint UInt32(string name, uint value);

    /// <summary>
    /// (De)serializes an (little-endian) unsigned 64-bit integer (i.e. an unsigned long in the range 0..18446744073709551615).
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    ulong UInt64(string name, ulong value);

    /// <summary>
    /// (De)serializes an enum value with an underlying type of byte.
    /// </summary>
    /// <typeparam name="T">The type of enum to (de)serialize</typeparam>
    /// <param name="n">The index of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    T EnumU8<T>(int n, T value) where T : unmanaged, Enum;

    /// <summary>
    /// (De)serializes an enum value with an underlying type of ushort (little-endian).
    /// </summary>
    /// <typeparam name="T">The type of enum to (de)serialize</typeparam>
    /// <param name="n">The index of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    T EnumU16<T>(int n, T value) where T : unmanaged, Enum;

    /// <summary>
    /// (De)serializes an enum value with an underlying type of uint (little-endian).
    /// </summary>
    /// <typeparam name="T">The type of enum to (de)serialize</typeparam>
    /// <param name="n">The index of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    T EnumU32<T>(int n, T value) where T : unmanaged, Enum;

    /// <summary>
    /// (De)serializes an enum value with an underlying type of byte.
    /// </summary>
    /// <typeparam name="T">The type of enum to (de)serialize</typeparam>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    T EnumU8<T>(string name, T value) where T : unmanaged, Enum;

    /// <summary>
    /// (De)serializes an enum value with an underlying type of ushort (little-endian).
    /// </summary>
    /// <typeparam name="T">The type of enum to (de)serialize</typeparam>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    T EnumU16<T>(string name, T value) where T : unmanaged, Enum;

    /// <summary>
    /// (De)serializes an enum value with an underlying type of uint (little-endian).
    /// </summary>
    /// <typeparam name="T">The type of enum to (de)serialize</typeparam>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    T EnumU32<T>(string name, T value) where T : unmanaged, Enum;

    /// <summary>
    /// (De)serializes a 16-byte GUID value.
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    Guid Guid(string name, Guid value);

    /// <summary>
    /// (De)serializes a fixed-size byte array.
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The bytes to write when serializing</param>
    /// <param name="length">The number of bytes to read/write</param>
    /// <returns>The bytes that were (de)serialized</returns>
    byte[] Bytes(string name, byte[] value, int length);

#if NETSTANDARD2_1_OR_GREATER
        /// <summary>
        /// (De)serializes a span of bytes.
        /// </summary>
        /// <param name="name">The name of the value being written, only used for annotation</param>
        /// <param name="value">The bytes to (de)serialize</param>
        void Bytes(string name, Span<byte> value);
#endif

    /// <summary>
    /// (De)serializes a null-terminated string.
    /// The encoding is determined by the stringToBytes / bytesToString method that was passed to the constructor.
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The string to write when serializing</param>
    /// <returns>The string that was (de)serialized</returns>
    string NullTerminatedString(string name, string value);

    /// <summary>
    /// (De)serializes a fixed-length (in bytes) string.
    /// The encoding is determined by the stringToBytes / bytesToString method that was passed to the constructor.
    /// Any extra bytes should be set to 0.
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The string to write when serializing</param>
    /// <param name="length">The maximum number of bytes the string can occupy</param>
    /// <returns>The string that was (de)serialized</returns>
    string FixedLengthString(string name, string value, int length);

    /// <summary>
    /// (De)serializes a list of values.
    /// </summary>
    /// <typeparam name="TTarget">The type of value being (de)serialized</typeparam>
    /// <param name="name">The name of the list of values, only used for annotation</param>
    /// <param name="list">The list of values to serialize. When deserializing, this list will be populated and returned if non-null, otherwise a new list will be created</param>
    /// <param name="count">The number of elements to (de)serialize</param>
    /// <param name="serdes">The function to use to (de)serialize each element in the list</param>
    /// <param name="initialiser">An optional initializer to use when list is null during deserialization. If no initializer is supplied, then a new System.Collections.Generic.List will be created</param>
    /// <returns>The list of values. This may be the same object as list</returns>
    IList<TTarget> List<TTarget>(
        string name,
        IList<TTarget> list,
        int count,
        SerdesMethod<TTarget> serdes,
        Func<int, IList<TTarget>> initialiser = null);

    /// <summary>
    /// (De)serializes a list of values.
    /// </summary>
    /// <typeparam name="TTarget">The type of value being (de)serialized</typeparam>
    /// <param name="name">The name of the list of values, only used for annotation</param>
    /// <param name="list">The list of values to serialize. When deserializing, this list will be populated and returned if non-null, otherwise a new list will be created</param>
    /// <param name="count">The number of elements to (de)serialize</param>
    /// <param name="offset">The position in the list to start populating/reading from. When deserializing and the list has fewer elements than offset, default-initialised values will be added until the length reaches the offset</param>
    /// <param name="serdes">The function to use to (de)serialize each element in the list</param>
    /// <param name="initialiser">An optional initializer to use when list is null during deserialization. If no initializer is supplied, then a new System.Collections.Generic.List will be created</param>
    /// <returns>The list of values. This may be the same object as list</returns>
    IList<TTarget> List<TTarget>(
        string name,
        IList<TTarget> list,
        int count,
        int offset,
        SerdesMethod<TTarget> serdes,
        Func<int, IList<TTarget>> initialiser = null);

    /// <summary>
    /// (De)serializes a list of values.
    /// </summary>
    /// <typeparam name="TTarget">The type of value being (de)serialized</typeparam>
    /// <typeparam name="TContext">The type of context data that will be passed through to the serdes function.</typeparam>
    /// <param name="name">The name of the list of values, only used for annotation</param>
    /// <param name="list">The list of values to serialize. When deserializing, this list will be populated and returned if non-null, otherwise a new list will be created</param>
    /// <param name="context">The context data to pass through to the serdes function</param>
    /// <param name="count">The number of elements to (de)serialize</param>
    /// <param name="serdes">The function to use to (de)serialize each element in the list</param>
    /// <param name="initialiser">An optional initializer to use when list is null during deserialization. If no initializer is supplied, then a new System.Collections.Generic.List will be created</param>
    /// <returns>The list of values. This may be the same object as list</returns>
    IList<TTarget> ListWithContext<TTarget, TContext>(
        string name,
        IList<TTarget> list,
        TContext context,
        int count,
        SerdesContextMethod<TTarget, TContext> serdes,
        Func<int, IList<TTarget>> initialiser = null);

    /// <summary>
    /// (De)serializes a list of values.
    /// </summary>
    /// <typeparam name="TTarget">The type of value being (de)serialized</typeparam>
    /// <typeparam name="TContext">The type of context data that will be passed through to the serdes function.</typeparam>
    /// <param name="name">The name of the list of values, only used for annotation</param>
    /// <param name="list">The list of values to serialize. When deserializing, this list will be populated and returned if non-null, otherwise a new list will be created</param>
    /// <param name="context">The context data to pass through to the serdes function</param>
    /// <param name="count">The number of elements to (de)serialize</param>
    /// <param name="offset">The position in the list to start populating/reading from. When deserializing and the list has fewer elements than offset, default-initialised values will be added until the length reaches the offset</param>
    /// <param name="serdes">The function to use to (de)serialize each element in the list</param>
    /// <param name="initialiser">An optional initializer to use when list is null during deserialization. If no initializer is supplied, then a new System.Collections.Generic.List will be created</param>
    /// <returns>The list of values. This may be the same object as list</returns>
    IList<TTarget> ListWithContext<TTarget, TContext>(
        string name,
        IList<TTarget> list,
        TContext context,
        int count,
        int offset,
        SerdesContextMethod<TTarget, TContext> serdes,
        Func<int, IList<TTarget>> initialiser = null);
}

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
    /// (De)serializes a (big-endian) signed 16-bit integer (i.e. a signed short in the range -32768..32767).
    /// </summary>
    /// <param name="s">The serdes instance</param>
    /// <param name="n">The index of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static short Int16BE(this ISerdes s, int n, short value) => SwapBytes16(s.Int16(n, SwapBytes16(value)));

    /// <summary>
    /// (De)serializes a (big-endian) signed 32-bit integer (i.e. a signed int in the range -2147483648..2147483647).
    /// </summary>
    /// <param name="s">The serdes instance</param>
    /// <param name="n">The index of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static int Int32BE(this ISerdes s, int n, int value) => SwapBytes32(s.Int32(n, SwapBytes32(value)));

    /// <summary>
    /// (De)serializes a (big-endian) signed 64-bit integer (i.e. a signed long in the range -9223372036854775808..9223372036854775807).
    /// </summary>
    /// <param name="s">The serdes instance</param>
    /// <param name="n">The index of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static long Int64BE(this ISerdes s, int n, long value) => SwapBytes64(s.Int64(n, SwapBytes64(value)));

    /// <summary>
    /// (De)serializes an (big-endian) unsigned 16-bit integer (i.e. an unsigned short in the range 0..65535).
    /// </summary>
    /// <param name="s">The serdes instance</param>
    /// <param name="n">The index of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static ushort UInt16BE(this ISerdes s, int n, ushort value) => SwapBytes16(s.UInt16(n, SwapBytes16(value)));

    /// <summary>
    /// (De)serializes an (big-endian) unsigned 32-bit integer (i.e. an unsigned int in the range 0..4294967295).
    /// </summary>
    /// <param name="s">The serdes instance</param>
    /// <param name="n">The index of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static uint UInt32BE(this ISerdes s, int n, uint value) => SwapBytes32(s.UInt32(n, SwapBytes32(value)));

    /// <summary>
    /// (De)serializes an (big-endian) unsigned 64-bit integer (i.e. an unsigned long in the range 0..18446744073709551615).
    /// </summary>
    /// <param name="s">The serdes instance</param>
    /// <param name="n">The index of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static ulong UInt64BE(this ISerdes s, int n, ulong value) => SwapBytes64(s.UInt64(n, SwapBytes64(value)));

    /// <summary>
    /// (De)serializes an enum value with an underlying type of ushort (big-endian).
    /// </summary>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static short Int16BE(this ISerdes s, string name, short value) => SwapBytes16(s.Int16(name, SwapBytes16(value)));

    /// <summary>
    /// (De)serializes an enum value with an underlying type of uint (big-endian).
    /// </summary>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static int Int32BE(this ISerdes s, string name, int value) => SwapBytes32(s.Int32(name, SwapBytes32(value)));

    /// <summary>
    /// (De)serializes an enum value with an underlying type of ulong (big-endian).
    /// </summary>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static long Int64BE(this ISerdes s, string name, long value) => SwapBytes64(s.Int64(name, SwapBytes64(value)));

    /// <summary>
    /// (De)serializes an enum value with an underlying type of ushort (big-endian).
    /// </summary>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static ushort UInt16BE(this ISerdes s, string name, ushort value) => SwapBytes16(s.UInt16(name, SwapBytes16(value)));

    /// <summary>
    /// (De)serializes an enum value with an underlying type of uint (big-endian).
    /// </summary>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static uint UInt32BE(this ISerdes s, string name, uint value) => SwapBytes32(s.UInt32(name, SwapBytes32(value)));

    /// <summary>
    /// (De)serializes an enum value with an underlying type of ulong (big-endian).
    /// </summary>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static ulong UInt64BE(this ISerdes s, string name, ulong value) => SwapBytes64(s.UInt64(name, SwapBytes64(value)));

    /// <summary>
    /// (De)serializes an enum value with an underlying type of ushort (big-endian).
    /// </summary>
    /// <typeparam name="T">The enum type to (de)serialize</typeparam>
    /// <param name="s">The serdes instance</param>
    /// <param name="n">The index of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static T EnumU16BE<T>(this ISerdes s, int n, T value)  where T : unmanaged, Enum
        => (T)(object)SwapBytes16(s.UInt16(n, SwapBytes16((ushort)(object)value)));

    /// <summary>
    /// (De)serializes an enum value with an underlying type of uint (big-endian).
    /// </summary>
    /// <typeparam name="T">The enum type to (de)serialize</typeparam>
    /// <param name="s">The serdes instance</param>
    /// <param name="n">The index of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static T EnumU32BE<T>(this ISerdes s, int n, T value)  where T : unmanaged, Enum
        => (T)(object)SwapBytes32(s.UInt32(n, SwapBytes32((uint)(object)value)));

    /// <summary>
    /// (De)serializes an enum value with an underlying type of ushort (big-endian).
    /// </summary>
    /// <typeparam name="T">The enum type to (de)serialize</typeparam>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static T EnumU16BE<T>(this ISerdes s, string name, T value)  where T : unmanaged, Enum
        => (T)(object)SwapBytes16(s.UInt16(name, SwapBytes16((ushort)(object)value)));

    /// <summary>
    /// (De)serializes an enum value with an underlying type of uint (big-endian).
    /// </summary>
    /// <typeparam name="T">The enum type to (de)serialize</typeparam>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    public static T EnumU32BE<T>(this ISerdes s, string name, T value)  where T : unmanaged, Enum
        => (T)(object)SwapBytes32(s.UInt32(name, SwapBytes32((uint)(object)value)));

    // ReSharper restore InconsistentNaming

    /// <summary>
    /// Invokes a serdes method inside an indexed scope.
    /// </summary>
    /// <typeparam name="T">The type of object being (de)serialized</typeparam>
    /// <param name="s">The serdes instance</param>
    /// <param name="n">The index of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <param name="serdesMethod">The method responsible for the actual (de)serialization</param>
    /// <returns></returns>
    public static T Object<T>(this ISerdes s, int n, T value, SerdesMethod<T> serdesMethod)
    {
        s.Begin(s.IsCommenting() ? $"{n}" : null); // Avoid allocating a string when we're not annotating.
        var result = serdesMethod(n, value, s);
        s.End();
        return result;
    }

    /// <summary>
    /// Invokes a serdes method inside a named scope.
    /// </summary>
    /// <typeparam name="T">The type of object being (de)serialized</typeparam>
    /// <param name="s">The serdes instance</param>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <param name="serdesMethod">The method responsible for the actual (de)serialization</param>
    /// <returns></returns>
    public static T Object<T>(this ISerdes s, string name, T value, NamedSerdesMethod<T> serdesMethod)
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
        string name,
        T value,
        TContext context,
        NamedSerdesContextMethod<T, TContext> serdesMethod)
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
        string name,
        TContext context,
        Action<TContext, ISerdes> serdesMethod)
    {
        s.Begin(name);
        serdesMethod(context, s);
        s.End();
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
}