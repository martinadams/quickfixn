using System;

#pragma warning disable 0628 // Suppress "new protected member declared in sealed class" warning.

namespace QuickFix.DataDictionary;

[Obsolete("This class will be removed in 1.16 (because it's unused)")]
public sealed class MissingRequiredFieldException : ApplicationException
{
    [Obsolete("This class will be removed in 1.16 (because it's unused)")]
    public MissingRequiredFieldException() { }

    [Obsolete("This class will be removed in 1.16 (because it's unused)")]
    public MissingRequiredFieldException(int field)
        : base($"Missing required field: {field}") { }

    [Obsolete("This class will be removed in 1.16 (because it's unused)")]
    public MissingRequiredFieldException(string message)
        : base(message) { }

    [Obsolete("This class will be removed in 1.16 (because it's unused)")]
    public MissingRequiredFieldException(string message, System.Exception inner)
        : base(message, inner) { }
}
