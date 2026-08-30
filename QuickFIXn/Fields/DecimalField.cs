using System;

namespace QuickFix.Fields;

/// <summary>
/// A decimal FIX field
/// </summary>
public class DecimalField : FieldBase<decimal>
{
    public DecimalField(int tag)
        : base(tag, new decimal(0.0)) {}

    public DecimalField(int tag, decimal val)
        : base(tag, val) { }

    protected override string MakeString()
    {
        return Converters.DecimalConverter.Convert(Value);
    }
}
