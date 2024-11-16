# SerdesNet

A .NET serialisation library where the same function can both serialize and deserialize (i.e. Serdes = SERialize-DESerialize)

The basic usage pattern is:

```csharp
class SomeObject
{
	public int Field1;
	public string Field2;
	public ushort Field3;
}

static SomeObject Serdes(SomeObject existing, ISerdes s) // This method handles both serialization and deserialization.
{
	existing ??= new SomeType(); // If we're reading then typically 'existing' will be null, when writing it will be the object to write.

	// When writing, each of these will basically be equivalent to
	// "existing.Field1 = existing.Field1", but with the side-effect
	// of serializing the value to the underlying stream.

	// When reading, the values passed in will just be the defaults for the type and the return value will be the deserialized value.

	existing.Field1 = s.Int32(nameof(existing.Field1), existing.Field1);
	existing.Field2 = s.NullTerminatedString(nameof(existing.Field2), existing.Field2);
	existing.Field3 = s.UInt16(nameof(existing.Field3), existing.Field3);

	return existing;
}
```

The serdes methods like `Int32(string name, int value)`, `UInt16(string name, ushort value)` etc should maintain these invariants:
- When serializing, the return value should be `value` (i.e. serializing an object should not change it).
- When deserializing, `value` should be ignored and the return value should be the deserialized value.
- The `name` parameter is only used to identify the field when using an annotation serdes for debugging. For the basic reader/writer serializers it is ignored.
- For most types, an integer can be used rather than a name. This is helpful for things like arrays / lists.


In cases where using the same code for reading and writing is impractical the flags can be checked to see if we are reading or writing. For example:
```csharp

static SomeObject Serdes(SomeObject existing, ISerdes s)
{
	if (s.IsReading)
	{
		// Don't need to use 'existing' at all when reading
		var result = new SomeObject();
		result.Field1 = s.Int32(nameof(result.Field1), 0);
		result.Field2 = s.NullTerminatedString(nameof(result.Field2), "");
		result.Field3 = s.UInt16(nameof(result.Field3), 0);
		return result;
	}
	else
	{
		// Don't care about any return value when writing
		if (existing == null) throw new ArgumentNullException(nameof(existing));
		s.Int32(nameof(existing.Field1), existing.Field1);
		s.NullTerminatedString(nameof(existing.Field2), existing.Field2);
		s.UInt16(nameof(existing.Field3), existing.Field3);
		return existing;
	}
}
```

## Features
- **ReaderSerdes** and **WriterSerdes**: Basic implementations of `ISerdes` for reading and writing with a stream.
- **AnnotationProxySerdes**: Delegates reading/writing to an underlying serdes and generates a textual representation of the read/write operations for debugging.
- **BreakpointProxySerdes**: For debugging. When a certain offset range is read/written then an event will be raised.
- **EmptySerdes**: A null implementation of `ISerdes` which represents a non-existent file. Will error on most calls.
- **WindowingProxySerdes**: Delegates reading/writing to an underlying serdes to simulate a subset of the underlying stream.

