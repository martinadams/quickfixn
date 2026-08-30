using System;

namespace QuickFix.DataDictionary;

[Obsolete("This class will be removed in 1.16 (because it's unused)")]
public class InvalidMessageTypeException : ApplicationException
{
    [Obsolete("This class will be removed in 1.16 (because it's unused)")]
    public InvalidMessageTypeException() { }

    [Obsolete("This class will be removed in 1.16 (because it's unused)")]
    public InvalidMessageTypeException(string message)
        : base(message) { }

    [Obsolete("This class will be removed in 1.16 (because it's unused)")]
    public InvalidMessageTypeException(string message, System.Exception inner)
        : base(message, inner) { }
}
