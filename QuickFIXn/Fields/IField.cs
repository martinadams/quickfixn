using System;

namespace QuickFix.Fields;

/// <summary>
/// Interface for all field classes
/// </summary>
public interface IField
{
    public int Tag { get; set; }

    /// <summary>
    /// returns full fix string: tag=val
    /// </summary>
    public string ToStringField();

    /// <summary>
    /// returns field value (not tag) formatted for FIX
    /// </summary>
    public string ToString();

    /// <summary>
    /// length of formatted field (including the trailing SOH) e.g. tag=val\001
    /// </summary>
    public int GetLength();

    /// <summary>
    /// Sum of bytes; used in calculating checksum
    /// </summary>
    public int GetTotal();

    [Obsolete("Use capitalized ToStringField() instead. This function will be removed in 1.16.")]
    public string toStringField();
    [Obsolete("Use capitalized GetLength() instead. This function will be removed in 1.16.")]
    public int getLength();
    [Obsolete("Use capitalized GetTotal() instead. This function will be removed in 1.16.")]
    public int getTotal();
}
