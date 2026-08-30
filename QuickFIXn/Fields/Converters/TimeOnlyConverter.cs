using System;
using System.Globalization;

namespace QuickFix.Fields.Converters;

/// <summary>
/// Convert TimeOnly to/from string
/// </summary>
public class TimeOnlyConverter
{
    private const string SecondsFormat = "HH:mm:ss";
    private const string MillisecFormat = "HH:mm:ss.fff";
    private const string MicrosecFormat = "HH:mm:ss.ffffff";
    // (There is no nanoseconds format because C# does not support nano precision in TimeOnly.)

    private static readonly string[] Formats = [
        SecondsFormat,
        MillisecFormat,
        MicrosecFormat
    ];

    /// <summary>
    /// Convert string to TimeOnly.
    /// Any nanoseconds are truncated, as C# doesn't support them in TimeOnly.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    /// <exception cref="FieldConvertError"></exception>
    public static TimeOnly Convert(string str)
    {
        try
        {
            if (str.Length == 18)
                str = str.Substring(0, 15);
            return TimeOnly.ParseExact(str, Formats, CultureInfo.InvariantCulture);
        }
        catch (FormatException e)
        {
            throw new FieldConvertError($"Could not convert string to TimeOnly ({str})", e);
        }
        catch (ArgumentNullException e)
        {
            throw new FieldConvertError("Could not convert null string to decimal", e);
        }
    }

    /// <summary>
    /// Convert TimeOnly to string, with precision specified by parameter.
    /// TimeOnly doesn't actually support nanosecond precision,
    /// so if <paramref name="precision"/> is <code>Nanosecond</code>, the returned value will always end in "000".
    /// </summary>
    /// <param name="timeOnly"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string Convert(TimeOnly timeOnly, TimePrecision precision)
    {
        return precision switch
        {
            TimePrecision.Second => timeOnly.ToString(SecondsFormat, CultureInfo.InvariantCulture),
            TimePrecision.Millisecond => timeOnly.ToString(MillisecFormat, CultureInfo.InvariantCulture),
            TimePrecision.Microsecond => timeOnly.ToString(MicrosecFormat, CultureInfo.InvariantCulture),
            TimePrecision.Nanosecond => timeOnly.ToString(MicrosecFormat, CultureInfo.InvariantCulture) + "000",
            _ => throw new ArgumentOutOfRangeException(nameof(precision))
        };
    }
}
