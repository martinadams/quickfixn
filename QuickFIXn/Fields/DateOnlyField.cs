using System;
using QuickFix.Fields.Converters;

namespace QuickFix.Fields;

public class DateOnlyField : FieldBase<DateOnly>
{
    /// <summary>
    /// Create a DateOnlyField set to Jan 1, 0001.
    /// </summary>
    /// <param name="tag"></param>
    public DateOnlyField(int tag)
        : this(tag, new DateOnly()) { }

    public DateOnlyField(int tag, DateOnly dateOnly)
        : base(tag, dateOnly) { }



    protected override string MakeString()
    {
        return DateOnlyConverter.Convert(Value);
    }

    /// <summary>
    /// This converts the dt parameter into a DateTime, losing any time info.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="dt"></param>
    [Obsolete("This ctor will be removed in 1.16.  You should use the DateOnly-based constructors.")]
    public DateOnlyField(int tag, DateTime dt)
        : this(tag, DateOnly.FromDateTime(dt)) { }

    [Obsolete("This ctor will be removed in 1.16 because it's unused (and doesn't even make sense)")]
    public DateOnlyField(int tag, DateTime dt, bool showMilliseconds)
        : this(tag, dt) { }

    [Obsolete("This ctor will be removed in 1.16 because it's unused (and doesn't even make sense)")]
    public DateOnlyField(int tag, DateTime dt, TimePrecision timeFormatPrecision)
        : this(tag, dt) { }
}
