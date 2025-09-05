using System;

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
    void Begin(SerdesName name = default);

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
    void Pad(SerdesName name, int count, byte value = 0);

    /// <summary>
    /// (De)serializes a (little-endian) signed 8-bit integer (i.e. a signed byte in the range -128..127).
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    sbyte Int8(SerdesName name, sbyte value);

    /// <summary>
    /// (De)serializes a (little-endian) signed 16-bit integer (i.e. a signed short in the range -32768..32767).
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    short Int16(SerdesName name, short value);

    /// <summary>
    /// (De)serializes a (little-endian) signed 32-bit integer (i.e. a signed int in the range -2147483648..2147483647).
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    int Int32(SerdesName name, int value);

    /// <summary>
    /// (De)serializes a (little-endian) signed 64-bit integer (i.e. a signed long in the range -9223372036854775808..9223372036854775807).
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    long Int64(SerdesName name, long value);

    /// <summary>
    /// (De)serializes an (little-endian) unsigned 8-bit integer (i.e. an unsigned byte in the range 0..255).
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    byte UInt8(SerdesName name, byte value);

    /// <summary>
    /// (De)serializes an (little-endian) unsigned 16-bit integer (i.e. an unsigned short in the range 0..65535).
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    ushort UInt16(SerdesName name, ushort value);

    /// <summary>
    /// (De)serializes an (little-endian) unsigned 32-bit integer (i.e. an unsigned int in the range 0..4294967295).
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    uint UInt32(SerdesName name, uint value);

    /// <summary>
    /// (De)serializes an (little-endian) unsigned 64-bit integer (i.e. an unsigned long in the range 0..18446744073709551615).
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    ulong UInt64(SerdesName name, ulong value);

    /// <summary>
    /// (De)serializes a 16-byte GUID value.
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The value to write when serializing</param>
    /// <returns>The value that was (de)serialized</returns>
    Guid Guid(SerdesName name, Guid value);

    /// <summary>
    /// (De)serializes a fixed-size byte array.
    /// </summary>
    /// <param name="name">The name of the value being written, only used for annotation</param>
    /// <param name="value">The bytes to write when serializing</param>
    /// <param name="length">The number of bytes to read/write</param>
    /// <returns>The bytes that were (de)serialized</returns>
    byte[] Bytes(SerdesName name, byte[] value, int length);

#if NETSTANDARD2_1_OR_GREATER
        /// <summary>
        /// (De)serializes a span of bytes.
        /// </summary>
        /// <param name="name">The name of the value being written, only used for annotation</param>
        /// <param name="value">The bytes to (de)serialize</param>
        void Bytes(SerdesName name, Span<byte> value);
#endif
}