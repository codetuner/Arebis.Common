using System;

#if PORTABLE
namespace System
{
    /// <summary>
    /// Replacement for the SerializableAttribute that is missing in the
    /// portable library subset. Fools the BinaryFormatter.
    /// </summary>
    internal class SerializableAttribute : Attribute
    { }
}
#endif