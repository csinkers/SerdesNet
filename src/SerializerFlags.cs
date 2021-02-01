using System;

namespace SerdesNet
{
    [Flags]
    public enum SerializerFlags
    {
        Read = 1,
        Write = 2,
        Comments = 4,
    }
}
