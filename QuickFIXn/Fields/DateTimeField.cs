using System;
using QuickFix.Fields.Converters;

namespace QuickFix.Fields;

public class DateTimeField : FieldBase<DateTime>
{
    protected readonly TimePrecision TimePrecision = TimePrecision.Millisecond;

    public DateTimeField(int tag)
        : base(tag, new DateTime()) {}

    public DateTimeField(int tag, DateTime dt)
        : base(tag, dt) {}

    public DateTimeField(int tag, DateTime dt, bool showMilliseconds)
        : this(tag, dt, showMilliseconds ? TimePrecision.Millisecond : TimePrecision.Second ) { }

    public DateTimeField(int tag, DateTime dt, TimePrecision timeFormatPrecision)
        : base(tag, dt)
    {
        TimePrecision = timeFormatPrecision;
    }

    protected override string MakeString()
    {
        return DateTimeConverter.ToFIX(Value, TimePrecision);
    }
}
