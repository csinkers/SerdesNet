using System.IO;
using System.Text;
using Moq;
using Xunit;

namespace SerdesNet.Tests;

public class SerializerExtensionsTests
{
    [Fact]
    public void IsReading_ShouldReturnTrue_WhenReadFlagIsSet()
    {
        // Arrange
        var mockSerdes = new Mock<ISerdes>();
        mockSerdes.Setup(s => s.Flags).Returns(SerializerFlags.Read);

        // Act
        var result = mockSerdes.Object.IsReading();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsReading_ShouldReturnFalse_WhenReadFlagIsNotSet()
    {
        // Arrange
        var mockSerdes = new Mock<ISerdes>();
        mockSerdes.Setup(s => s.Flags).Returns(SerializerFlags.Write);

        // Act
        var result = mockSerdes.Object.IsReading();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsWriting_ShouldReturnTrue_WhenWriteFlagIsSet()
    {
        // Arrange
        var mockSerdes = new Mock<ISerdes>();
        mockSerdes.Setup(s => s.Flags).Returns(SerializerFlags.Write);

        // Act
        var result = mockSerdes.Object.IsWriting();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsWriting_ShouldReturnFalse_WhenWriteFlagIsNotSet()
    {
        // Arrange
        var mockSerdes = new Mock<ISerdes>();
        mockSerdes.Setup(s => s.Flags).Returns(SerializerFlags.Read);

        // Act
        var result = mockSerdes.Object.IsWriting();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsCommenting_ShouldReturnTrue_WhenCommentsFlagIsSet()
    {
        // Arrange
        var mockSerdes = new Mock<ISerdes>();
        mockSerdes.Setup(s => s.Flags).Returns(SerializerFlags.Comments);

        // Act
        var result = mockSerdes.Object.IsCommenting();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsCommenting_ShouldReturnFalse_WhenCommentsFlagIsNotSet()
    {
        // Arrange
        var mockSerdes = new Mock<ISerdes>();
        mockSerdes.Setup(s => s.Flags).Returns(SerializerFlags.Read);

        // Act
        var result = mockSerdes.Object.IsCommenting();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Assert_ShouldInvokeAssert_WhenConditionIsFalse()
    {
        // Arrange
        var mockSerdes = new Mock<ISerdes>();
        var context = "context";
        var message = "Assertion failed";
        mockSerdes.Setup(s => s.Assert(It.IsAny<bool>(), It.IsAny<string>()));

        // Act
        mockSerdes.Object.Assert(false, context, _ => message);

        // Assert
        mockSerdes.Verify(s => s.Assert(false, message), Times.Once);
    }

    [Fact]
    public void Assert_ShouldNotInvokeAssert_WhenConditionIsTrue()
    {
        // Arrange
        var mockSerdes = new Mock<ISerdes>();
        var context = "context";
        var message = "Assertion failed";
        mockSerdes.Setup(s => s.Assert(It.IsAny<bool>(), It.IsAny<string>()));

        // Act
        mockSerdes.Object.Assert(true, context, _ => message);

        // Assert
        mockSerdes.Verify(s => s.Assert(It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Int16BE_ShouldSerializeCorrectly()
    {
        // Arrange
        short value = 0x1234;
        byte[] expected = [0x12, 0x34];
        var ms = new MemoryStream();
        var w = new WriterSerdes(ms, Encoding.UTF8);

        // Act
        var result = w.Int16BE(0, value);
        var bytes = ms.ToArray();

        // Assert
        Assert.Equal(value, result);
        Assert.Equal(expected, bytes);
    }

    [Fact]
    public void Int32BE_ShouldSerializeCorrectly()
    {
        // Arrange
        int value = 0x12345678;
        byte[] expected = [0x12, 0x34, 0x56, 0x78];
        var ms = new MemoryStream();
        var w = new WriterSerdes(ms, Encoding.UTF8);

        // Act
        var result = w.Int32BE(0, value);
        var bytes = ms.ToArray();

        // Assert
        Assert.Equal(value, result);
        Assert.Equal(expected, bytes);
    }

    [Fact]
    public void Int64BE_ShouldSerializeCorrectly()
    {
        // Arrange
        long value = 0x123456789ABCDEF0;
        byte[] expected = [0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0];
        var ms = new MemoryStream();
        var w = new WriterSerdes(ms, Encoding.UTF8);

        // Act
        var result = w.Int64BE(0, value);
        var bytes = ms.ToArray();

        // Assert
        Assert.Equal(value, result);
        Assert.Equal(expected, bytes);
    }

    [Fact]
    public void UInt16BE_ShouldSerializeCorrectly()
    {
        // Arrange
        ushort value = 0x1234;
        byte[] expected = [0x12, 0x34];
        var ms = new MemoryStream();
        var w = new WriterSerdes(ms, Encoding.UTF8);

        // Act
        var result = w.UInt16BE(0, value);
        var bytes = ms.ToArray();

        // Assert
        Assert.Equal(value, result);
        Assert.Equal(expected, bytes);
    }

    [Fact]
    public void UInt32BE_ShouldSerializeCorrectly()
    {
        // Arrange
        uint value = 0x12345678;
        byte[] expected = [0x12, 0x34, 0x56, 0x78];
        var ms = new MemoryStream();
        var w = new WriterSerdes(ms, Encoding.UTF8);

        // Act
        var result = w.UInt32BE(0, value);
        var bytes = ms.ToArray();

        // Assert
        Assert.Equal(value, result);
        Assert.Equal(expected, bytes);
    }

    [Fact]
    public void UInt64BE_ShouldSerializeCorrectly()
    {
        // Arrange
        ulong value = 0x123456789ABCDEF0;
        byte[] expected = [0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0];
        var ms = new MemoryStream();
        var w = new WriterSerdes(ms, Encoding.UTF8);

        // Act
        var result = w.UInt64BE(0, value);
        var bytes = ms.ToArray();

        // Assert
        Assert.Equal(value, result);
        Assert.Equal(expected, bytes);
    }

    [Fact]
    public void Int16BE_WithName_ShouldSerializeCorrectly()
    {
        // Arrange
        short value = 0x1234;
        byte[] expected = [0x12, 0x34];
        var ms = new MemoryStream();
        var w = new WriterSerdes(ms, Encoding.UTF8);

        // Act
        var result = w.Int16BE("test", value);
        var bytes = ms.ToArray();

        // Assert
        Assert.Equal(value, result);
        Assert.Equal(expected, bytes);
    }

    [Fact]
    public void Int32BE_WithName_ShouldSerializeCorrectly()
    {
        // Arrange
        int value = 0x12345678;
        byte[] expected = [0x12, 0x34, 0x56, 0x78];
        var ms = new MemoryStream();
        var w = new WriterSerdes(ms, Encoding.UTF8);

        // Act
        var result = w.Int32BE("test", value);
        var bytes = ms.ToArray();

        // Assert
        Assert.Equal(value, result);
        Assert.Equal(expected, bytes);
    }

    [Fact]
    public void Int64BE_WithName_ShouldSerializeCorrectly()
    {
        // Arrange
        long value = 0x123456789ABCDEF0;
        byte[] expected = [0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0];
        var ms = new MemoryStream();
        var w = new WriterSerdes(ms, Encoding.UTF8);

        // Act
        var result = w.Int64BE("test", value);
        var bytes = ms.ToArray();

        // Assert
        Assert.Equal(value, result);
        Assert.Equal(expected, bytes);
    }

    [Fact]
    public void UInt16BE_WithName_ShouldSerializeCorrectly()
    {
        // Arrange
        ushort value = 0x1234;
        byte[] expected = [0x12, 0x34];
        var ms = new MemoryStream();
        var w = new WriterSerdes(ms, Encoding.UTF8);

        // Act
        var result = w.UInt16BE("test", value);
        var bytes = ms.ToArray();

        // Assert
        Assert.Equal(value, result);
        Assert.Equal(expected, bytes);
    }

    [Fact]
    public void UInt32BE_WithName_ShouldSerializeCorrectly()
    {
        // Arrange
        uint value = 0x12345678;
        byte[] expected = [0x12, 0x34, 0x56, 0x78];
        var ms = new MemoryStream();
        var w = new WriterSerdes(ms, Encoding.UTF8);

        // Act
        var result = w.UInt32BE("test", value);
        var bytes = ms.ToArray();

        // Assert
        Assert.Equal(value, result);
        Assert.Equal(expected, bytes);
    }

    [Fact]
    public void UInt64BE_WithName_ShouldSerializeCorrectly()
    {
        // Arrange
        ulong value = 0x123456789ABCDEF0;
        byte[] expected = [0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0];
        var ms = new MemoryStream();
        var w = new WriterSerdes(ms, Encoding.UTF8);

        // Act
        var result = w.UInt64BE("test", value);
        var bytes = ms.ToArray();

        // Assert
        Assert.Equal(value, result);
        Assert.Equal(expected, bytes);
    }

    [Fact]
    public void EnumU16BE_ShouldSerializeCorrectly()
    {
        // Arrange
        var value = TestEnumU16.Value1;
        byte[] expected = [0x12, 0x34];
        var ms = new MemoryStream();
        var w = new WriterSerdes(ms, Encoding.UTF8);

        // Act
        var result = w.EnumU16BE(0, value);
        var bytes = ms.ToArray();

        // Assert
        Assert.Equal(value, result);
        Assert.Equal(expected, bytes);
    }

    [Fact]
    public void EnumU32BE_ShouldSerializeCorrectly()
    {
        // Arrange
        var value = TestEnumU32.Value1;
        byte[] expected = [0x12, 0x34, 0x56, 0x78];
        var ms = new MemoryStream();
        var w = new WriterSerdes(ms, Encoding.UTF8);

        // Act
        var result = w.EnumU32BE(0, value);
        var bytes = ms.ToArray();

        // Assert
        Assert.Equal(value, result);
        Assert.Equal(expected, bytes);
    }

    [Fact]
    public void EnumU16BE_WithName_ShouldSerializeCorrectly()
    {
        // Arrange
        var value = TestEnumU16.Value1;
        byte[] expected = [0x12, 0x34];
        var ms = new MemoryStream();
        var w = new WriterSerdes(ms, Encoding.UTF8);

        // Act
        var result = w.EnumU16BE("test", value);
        var bytes = ms.ToArray();

        // Assert
        Assert.Equal(value, result);
        Assert.Equal(expected, bytes);
    }

    [Fact]
    public void EnumU32BE_WithName_ShouldSerializeCorrectly()
    {
        // Arrange
        var value = TestEnumU32.Value1;
        byte[] expected = [0x12, 0x34, 0x56, 0x78];
        var ms = new MemoryStream();
        var w = new WriterSerdes(ms, Encoding.UTF8);

        // Act
        var result = w.EnumU32BE("test", value);
        var bytes = ms.ToArray();

        // Assert
        Assert.Equal(value, result);
        Assert.Equal(expected, bytes);
    }

    enum TestEnumU16 : ushort
    {
        Value1 = 0x1234,
        Value2 = 0x5678
    }

    enum TestEnumU32 : uint
    {
        Value1 = 0x12345678,
        Value2 = 0x9abcdef0
    }

    [Fact]
    public void Object_Serialization_Deserialization_Test()
    {
        var testObject = new TestObject { Id = 42, Name = "Test" };
        var encoding = Encoding.UTF8;
        byte[] serializedData;

        // Serialize
        using (var ms = new MemoryStream())
        {
            var writer = new WriterSerdes(ms, encoding);
            writer.Object(0, testObject, SerializeTestObject);
            serializedData = ms.ToArray();
        }

        // Deserialize
        TestObject deserializedObject;
        using (var ms = new MemoryStream(serializedData))
        {
            var reader = new ReaderSerdes(new BinaryReader(ms), serializedData.Length, encoding.GetString);
            deserializedObject = reader.Object(0, default(TestObject), SerializeTestObject);
        }

        // Assert
        Assert.Equal(testObject.Id, deserializedObject.Id);
        Assert.Equal(testObject.Name, deserializedObject.Name);
    }

    [Fact]
    public void Object_WithContext_Serialization_Deserialization_Test()
    {
        var testObject = new TestObject { Id = 42, Name = "Test" };
        var context = new TestContext { Prefix = "Context_" };
        var encoding = Encoding.UTF8;
        byte[] serializedData;

        // Serialize
        using (var ms = new MemoryStream())
        {
            var writer = new WriterSerdes(ms, encoding);
            writer.Object("testObject", testObject, context, SerdesTestObjectWithContext);
            serializedData = ms.ToArray();
        }

        // Deserialize
        TestObject deserializedObject;
        using (var ms = new MemoryStream(serializedData))
        {
            var reader = new ReaderSerdes(new BinaryReader(ms), serializedData.Length, encoding.GetString);
            deserializedObject = reader.Object("testObject", default(TestObject), context, SerdesTestObjectWithContext);
        }

        // Assert
        Assert.Equal(testObject.Id, deserializedObject.Id);
        Assert.Equal(testObject.Name, deserializedObject.Name);
    }

    [Fact]
    public void Object_WithContext_InPlace_Serialization_Deserialization_Test()
    {
        var testObject = new TestObject { Id = 42, Name = "Test" };
        var encoding = Encoding.UTF8;
        byte[] serializedData;

        // Serialize
        using (var ms = new MemoryStream())
        {
            var writer = new WriterSerdes(new BinaryWriter(ms), encoding.GetBytes);
            writer.Object("testObject", testObject, SerdesTestObjectInPlace);
            serializedData = ms.ToArray();
        }

        // Deserialize
        TestObject deserializedObject = new TestObject();
        using (var ms = new MemoryStream(serializedData))
        {
            var reader = new ReaderSerdes(new BinaryReader(ms), serializedData.Length, encoding.GetString);
            reader.Object("testObject", deserializedObject, SerdesTestObjectInPlace);
        }

        // Assert
        Assert.Equal(testObject.Id, deserializedObject.Id);
        Assert.Equal(testObject.Name, deserializedObject.Name);
    }

    void SerdesTestObjectInPlace(TestObject obj, ISerdes s)
    {
        obj.Id = s.Int32(nameof(obj.Id), obj.Id);
        obj.Name = s.NullTerminatedString(nameof(obj.Name), obj.Name);
    }

    static TestObject SerializeTestObject(SerdesName name, TestObject obj, ISerdes s)
    {
        obj ??= new TestObject();
        obj.Id = s.Int32(nameof(obj.Id), obj.Id);
        obj.Name = s.NullTerminatedString(nameof(obj.Name), obj.Name);
        return obj;
    }

    static TestObject SerdesTestObjectWithContext(SerdesName name, TestObject obj, TestContext context, ISerdes s)
    {
        obj ??= new TestObject();
        obj.Id = s.Int32(nameof(obj.Id), obj.Id);
        obj.Name = s.NullTerminatedString(nameof(obj.Name), context.Prefix + obj.Name);
        return obj;
    }

    class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    class TestContext
    {
        public string Prefix { get; set; }
    }
}
