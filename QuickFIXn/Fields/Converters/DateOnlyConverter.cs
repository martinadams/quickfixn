using System;
using System.Globalization;

namespace QuickFix.Fields.Converters;

/// <summary>
/// Convert DateOnly to/from string
/// </summary>
public class DateOnlyConverter
{
    /// <summary>
    /// Convert string to DateOnly
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    /// <exception cref="FieldConvertError"></exception>
    public static DateOnly Convert(string str)
    {
        try
        {
            return DateOnly.ParseExact(str, "yyyyMMdd", CultureInfo.InvariantCulture);
        }
        catch (FormatException e)
        {
            throw new FieldConvertError($"Could not convert string to DateOnly ({str})", e);
        }
        catch (ArgumentNullException e)
        {
            throw new FieldConvertError("Could not convert null string to decimal", e);
        }
    }

    /// <summary>
    /// Converts the specified <see cref="DateOnly"/> to a <see cref="string"/> in the format "yyyyMMdd".
    /// </summary>
    /// <param name="dateOnly">The value to convert.</param>
    /// <returns>A value representing <paramref name="dateOnly"/> in the format "yyyyMMdd".</returns>
    public static string Convert(DateOnly dateOnly)
    {
        return dateOnly.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
    }
}
