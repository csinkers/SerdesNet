using System;

namespace SerdesNet;

/// <summary>
/// Flags for describing the behaviors of a serdes instance.
/// </summary>
[Flags]
public enum SerializerFlags
{
    /// <summary>
    /// This flag should be set for serdes instances that read data.
    /// </summary>
    Read = 1,

    /// <summary>
    /// This flag should be set for serdes instances that write data.
    /// </summary>
    Write = 2,

    /// <summary>
    /// This flag should be set for serdes instances that collect annotations / comments in a separate textual output stream.
    /// </summary>
    Comments = 4,
}