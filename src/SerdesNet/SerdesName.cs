using System.Globalization;

namespace SerdesNet;

/// <summary>
/// A name to use for a value when annotating a (de)serialization operation
/// </summary>
public struct SerdesName
{
    /// <summary>
    /// The integer value to use if the value is not -1
    /// </summary>
    public int N;

    /// <summary>
    /// The name to use when N is -1
    /// </summary>
    public string Name;

    /// <summary>
    /// Implicit conversion from int to SerdesName.
    /// </summary>
    public static implicit operator SerdesName(int value) => new() { N = value };

    /// <summary>
    /// Implicit conversion from string to SerdesName.
    /// </summary>
    public static implicit operator SerdesName(string value) => new() { N = -1, Name = value };

    /// <summary>
    ///
    /// </summary>
    public override string ToString() => N == -1 ? Name : N.ToString(CultureInfo.InvariantCulture);
}
