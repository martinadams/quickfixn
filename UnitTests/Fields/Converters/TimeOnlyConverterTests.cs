using System;
using NUnit.Framework;
using QuickFix.Fields;
using QuickFix.Fields.Converters;

namespace UnitTests.Fields.Converters;

public class TimeOnlyConverterTests
{
    [Test]
    public void ConvertFromStringTest()
    {
        Assert.That(TimeOnlyConverter.Convert("13:30:45"), Is.EqualTo(new TimeOnly(13, 30, 45)));
        Assert.That(TimeOnlyConverter.Convert("13:30:45.999"), Is.EqualTo(new TimeOnly(13, 30, 45, 999)));
        Assert.That(TimeOnlyConverter.Convert("13:30:45.999050"), Is.EqualTo(new TimeOnly(13, 30, 45, 999, 50)));
        Assert.That(TimeOnlyConverter.Convert("13:30:45.999050333"), Is.EqualTo(new TimeOnly(13, 30, 45, 999, 50)));
    }

    [Test]
    public void ConvertToStringTest()
    {
        TimeOnly tOnly = new TimeOnly(13, 30, 45, 999, 50);
        Assert.That(TimeOnlyConverter.Convert(tOnly, TimePrecision.Second), Is.EqualTo("13:30:45"));
        Assert.That(TimeOnlyConverter.Convert(tOnly, TimePrecision.Millisecond), Is.EqualTo("13:30:45.999"));
        Assert.That(TimeOnlyConverter.Convert(tOnly, TimePrecision.Microsecond), Is.EqualTo("13:30:45.999050"));
        Assert.That(TimeOnlyConverter.Convert(tOnly, TimePrecision.Nanosecond), Is.EqualTo("13:30:45.999050000"));
    }
}
