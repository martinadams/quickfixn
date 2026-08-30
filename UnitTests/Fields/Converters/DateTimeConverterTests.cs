using System;
using NUnit.Framework;
using QuickFix;
using QuickFix.Fields;
using QuickFix.Fields.Converters;
using UnitTests.TestHelpers;

namespace UnitTests.Fields.Converters;

[TestFixture]
public class DateTimeConverterTests {

    [SetUp]
    public void SetUp() {
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
    }

    [TearDown]
    public void TearDown() {
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
    }

    [Test]
    public void ParseToDateTimeTest() {
        DateTimeOffset? dto;

        Assert.That(DateTimeConverter.ParseToDateTime("20100912-04:22:01"),
            Is.EqualTo(TimeHelper.MakeDateTime(2010, 9, 12, 4, 22, 1, 0, 0, 0)));
        Assert.That(DateTimeConverter.ParseToDateTime("20100912-04:22:01.123"),
            Is.EqualTo(TimeHelper.MakeDateTime(2010, 9, 12, 4, 22, 1, 123, 0, 0)));
        Assert.That(DateTimeConverter.ParseToDateTime("20100912-04:22:01.123456", out dto),
            Is.EqualTo(TimeHelper.MakeDateTime(2010, 9, 12, 4, 22, 1, 123, 456, 0)));
        Assert.That(dto, Is.Null); // no timezone info

        // DateTime only supports tick resolution, which is 100ns
        Assert.That(DateTimeConverter.ParseToDateTime("20100912-04:22:01.123456789", out dto),
            Is.EqualTo(TimeHelper.MakeDateTime(2010, 9, 12, 4, 22, 1, 123, 456, 700)));
        Assert.That(dto, Is.Null); // no timezone info

        Assert.That(DateTimeConverter.ParseToDateTime("20100912-04:22:01.123456789Z", out dto),
            Is.EqualTo(TimeHelper.MakeDateTime(2010, 9, 12, 4, 22, 1, 123, 456, 700)));
        Assert.That(dto?.DateTime, Is.EqualTo(TimeHelper.MakeDateTime(2010, 9, 12, 4, 22, 1, 123, 456, 700)));
        Assert.That(dto?.Offset, Is.EqualTo(new TimeSpan(hours: 0, minutes: 0, seconds: 0))); // timezone is UTC

        // Timezones
        Assert.That(DateTimeConverter.ParseToDateTime("20100912-04:22:01.123456789+01:30"),
            Is.EqualTo(TimeHelper.MakeDateTime(2010, 9, 12, 2, 52, 1, 123, 456, 700)));
        Assert.That(DateTimeConverter.ParseToDateTime("20100912-04:22:01.123456789-01:30", out dto),
            Is.EqualTo(TimeHelper.MakeDateTime(2010, 9, 12, 5, 52, 1, 123, 456, 700)));

        Assert.That(dto?.DateTime, Is.EqualTo(TimeHelper.MakeDateTime(2010, 9, 12, 4, 22, 1, 123, 456, 700)));
        Assert.That(dto?.Offset, Is.EqualTo(new TimeSpan(hours: 0, minutes: -90, seconds: 0)));
    }

    [Test]
    public void ParseToDateTimeTest_Exceptions() {
        Assert.Throws(typeof(FieldConvertError), delegate { DateTimeConverter.ParseToDateTime(""); });
        Assert.Throws(typeof(FieldConvertError), delegate { DateTimeConverter.ParseToDateTime("20021201"); });
        Assert.Throws(typeof(FieldConvertError), delegate { DateTimeConverter.ParseToDateTime("20021201-11:03"); });
    }

    [Test]
    public void ParseToDateOnlyTest_ReturnsDateOnly() {
        ReadOnlySpan<char> ros = "20100912";
#pragma warning disable CS0618
        DateOnly rv = DateTimeConverter.ParseToDateOnly(ros);
#pragma warning restore CS0618
        Assert.That(rv, Is.EqualTo(new DateOnly(2010, 9, 12)));
    }

    [Test]
    public void ParseToDateOnlyTest_ReturnsDateTime() {
#pragma warning disable CS0618
        DateTime rv = DateTimeConverter.ParseToDateOnly("20100912");
#pragma warning restore CS0618
        Assert.That(rv, Is.EqualTo(new DateTime(2010, 9, 12, 0, 0, 0, DateTimeKind.Utc)));
    }

    [Test]
    public void ParseToDateOnlyTest_Exceptions() {
#pragma warning disable CS0618
        Assert.Throws(typeof(FieldConvertError), delegate { DateTimeConverter.ParseToDateOnly(""); });
        Assert.Throws(typeof(FieldConvertError), delegate { DateTimeConverter.ParseToDateOnly("20021201-11:03:00"); });
#pragma warning restore CS0618
    }

    [Test]
    public void ParseToTimeOnlyTest_ReturnsTimeOnly() {
        var targetTicks = new TimeSpan(4, 22, 1).Ticks;
        Assert.That(targetTicks, Is.EqualTo(157210000000)); // for human reader reference

        TimeSpan? timeSpan;

#pragma warning disable CS0618
        TimeOnly rv = DateTimeConverter.ParseToTimeOnly("04:22:01", out timeSpan);
        Assert.That(rv.Ticks, Is.EqualTo(targetTicks));
        Assert.That(timeSpan, Is.Null);

        Assert.That(DateTimeConverter.ParseToTimeOnly("04:22:01.123", out _).Ticks, Is.EqualTo(targetTicks + 1230000));
        Assert.That(DateTimeConverter.ParseToTimeOnly("04:22:01.123456", out _).Ticks, Is.EqualTo(targetTicks + 1234560));

        rv = DateTimeConverter.ParseToTimeOnly("04:22:01-03:00", out timeSpan);
        Assert.That(rv.Ticks, Is.EqualTo(targetTicks));
        Assert.That(timeSpan, Is.EqualTo(new TimeSpan(-3, 0, 0)));
#pragma warning restore CS0618
    }

    [Test]
    public void ParseToTimeOnlyTest_ReturnsDateTime() {
        var targetTicks = new DateTime(1980, 1, 1, 4, 22, 1, 0, DateTimeKind.Utc).Ticks;
        Assert.That(targetTicks, Is.EqualTo(624511453210000000)); // for human reader reference

#pragma warning disable CS0618
        DateTime rv = DateTimeConverter.InternalParseToTimeOnly("04:22:01");

        Assert.That(rv.Ticks, Is.EqualTo(targetTicks));
        Assert.That(DateTimeConverter.InternalParseToTimeOnly("04:22:01.123").Ticks, Is.EqualTo(targetTicks + 1230000));
        Assert.That(DateTimeConverter.InternalParseToTimeOnly("04:22:01.123456").Ticks, Is.EqualTo(targetTicks + 1234560));
#pragma warning restore CS0618
    }

    [Test]
    public void ParseToTimeOnlyTest_Exceptions() {
#pragma warning disable CS0618
        Assert.Throws(typeof(FieldConvertError), delegate { DateTimeConverter.InternalParseToTimeOnly(""); });
        Assert.Throws(typeof(FieldConvertError), delegate { DateTimeConverter.InternalParseToTimeOnly("20021201-11:03:00"); });
#pragma warning restore CS0618
    }

    [Test]
    public void ToFIXTest() {
        var dtSec = DateTime.SpecifyKind(TimeHelper.MakeDateTime(2002, 12, 01, 11, 03, 05, 0, 0, 0), DateTimeKind.Utc);
        var dtMs = DateTime.SpecifyKind(TimeHelper.MakeDateTime(2002, 12, 01, 11, 03, 05, 123, 0, 0), DateTimeKind.Utc);
        var dtMicro = DateTime.SpecifyKind(TimeHelper.MakeDateTime(2002, 12, 01, 11, 03, 05, 0, 654, 0), DateTimeKind.Utc);
        var dtNanoFull = DateTime.SpecifyKind(TimeHelper.MakeDateTime(2002, 12, 01, 11, 03, 05, 123, 654, 700), DateTimeKind.Utc);
        var dtNano100 = DateTime.SpecifyKind(TimeHelper.MakeDateTime(2002, 12, 01, 11, 03, 05, 000, 000, 100), DateTimeKind.Utc);

        // ToFIX(DateTime dt, TimeStampPrecision precision)
        Assert.That(DateTimeConverter.ToFIX(dtSec, TimePrecision.Second), Is.EqualTo("20021201-11:03:05"));
        Assert.That(DateTimeConverter.ToFIX(dtMs, TimePrecision.Second), Is.EqualTo("20021201-11:03:05"));
        Assert.That(DateTimeConverter.ToFIX(dtMicro, TimePrecision.Second), Is.EqualTo("20021201-11:03:05"));
        Assert.That(DateTimeConverter.ToFIX(dtNanoFull, TimePrecision.Second), Is.EqualTo("20021201-11:03:05"));
        Assert.That(DateTimeConverter.ToFIX(dtNano100, TimePrecision.Second), Is.EqualTo("20021201-11:03:05"));

        Assert.That(DateTimeConverter.ToFIX(dtSec, TimePrecision.Millisecond), Is.EqualTo("20021201-11:03:05.000"));
        Assert.That(DateTimeConverter.ToFIX(dtMs, TimePrecision.Millisecond), Is.EqualTo("20021201-11:03:05.123"));
        Assert.That(DateTimeConverter.ToFIX(dtMicro, TimePrecision.Millisecond), Is.EqualTo("20021201-11:03:05.000"));
        Assert.That(DateTimeConverter.ToFIX(dtNanoFull, TimePrecision.Millisecond), Is.EqualTo("20021201-11:03:05.123"));
        Assert.That(DateTimeConverter.ToFIX(dtNano100, TimePrecision.Millisecond), Is.EqualTo("20021201-11:03:05.000"));

        Assert.That(DateTimeConverter.ToFIX(dtSec, TimePrecision.Microsecond), Is.EqualTo("20021201-11:03:05.000000"));
        Assert.That(DateTimeConverter.ToFIX(dtMs, TimePrecision.Microsecond), Is.EqualTo("20021201-11:03:05.123000"));
        Assert.That(DateTimeConverter.ToFIX(dtMicro, TimePrecision.Microsecond), Is.EqualTo("20021201-11:03:05.000654"));
        Assert.That(DateTimeConverter.ToFIX(dtNanoFull, TimePrecision.Microsecond), Is.EqualTo("20021201-11:03:05.123654"));
        Assert.That(DateTimeConverter.ToFIX(dtNano100, TimePrecision.Microsecond), Is.EqualTo("20021201-11:03:05.000000"));

        Assert.That(DateTimeConverter.ToFIX(dtSec, TimePrecision.Nanosecond), Is.EqualTo("20021201-11:03:05.000000000"));
        Assert.That(DateTimeConverter.ToFIX(dtMs, TimePrecision.Nanosecond), Is.EqualTo("20021201-11:03:05.123000000"));
        Assert.That(DateTimeConverter.ToFIX(dtMicro, TimePrecision.Nanosecond), Is.EqualTo("20021201-11:03:05.000654000"));
        Assert.That(DateTimeConverter.ToFIX(dtNanoFull, TimePrecision.Nanosecond), Is.EqualTo("20021201-11:03:05.123654700"));
        Assert.That(DateTimeConverter.ToFIX(dtNano100, TimePrecision.Nanosecond), Is.EqualTo("20021201-11:03:05.000000100"));
    }

    [Test]
    public void ToFIXDateOnlyTest() {
#pragma warning disable CS0618
        // ToFIXDateOnly(DateOnly)
        Assert.That(DateTimeConverter.ToFIXDateOnly(new DateOnly(2002, 12, 01)), Is.EqualTo("20021201"));

        // ToFIXDateOnly(DateTime)
        var dt = DateTime.SpecifyKind(
            TimeHelper.MakeDateTime(2002, 12, 01, 11, 03, 05, 123, 654, 700),
            DateTimeKind.Utc);
        Assert.That(DateTimeConverter.ToFIXDateOnly(dt), Is.EqualTo("20021201"));
#pragma warning restore CS0618
    }

    [Test]
    public void ToFIXTimeOnlyTest_TimeOnlyParam() {
        var tSec = TimeHelper.MakeTimeOnly(11, 03, 05, 0, 0, 0);
        var tMs = TimeHelper.MakeTimeOnly(11, 03, 05, 123, 0, 0);
        var tMicro = TimeHelper.MakeTimeOnly(11, 03, 05, 0, 654, 0);
        var tNanoFull = TimeHelper.MakeTimeOnly(11, 03, 05, 123, 654, 700);
        var tNano100 = TimeHelper.MakeTimeOnly(11, 03, 05, 000, 000, 100);

#pragma warning disable CS0618
        // ToFIXTimeOnly(TimeOnly time, TimeStampPrecision precision)
        Assert.That(DateTimeConverter.ToFIXTimeOnly(tSec, TimePrecision.Second), Is.EqualTo("11:03:05"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(tMs, TimePrecision.Second), Is.EqualTo("11:03:05"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(tMicro, TimePrecision.Second), Is.EqualTo("11:03:05"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(tNanoFull, TimePrecision.Second), Is.EqualTo("11:03:05"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(tNano100, TimePrecision.Second), Is.EqualTo("11:03:05"));

        Assert.That(DateTimeConverter.ToFIXTimeOnly(tSec, TimePrecision.Millisecond), Is.EqualTo("11:03:05.000"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(tMs, TimePrecision.Millisecond), Is.EqualTo("11:03:05.123"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(tMicro, TimePrecision.Millisecond), Is.EqualTo("11:03:05.000"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(tNanoFull, TimePrecision.Millisecond), Is.EqualTo("11:03:05.123"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(tNano100, TimePrecision.Millisecond), Is.EqualTo("11:03:05.000"));

        Assert.That(DateTimeConverter.ToFIXTimeOnly(tSec, TimePrecision.Microsecond), Is.EqualTo("11:03:05.000000"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(tMs, TimePrecision.Microsecond), Is.EqualTo("11:03:05.123000"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(tMicro, TimePrecision.Microsecond), Is.EqualTo("11:03:05.000654"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(tNanoFull, TimePrecision.Microsecond), Is.EqualTo("11:03:05.123654"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(tNano100, TimePrecision.Microsecond), Is.EqualTo("11:03:05.000000"));

        Assert.That(DateTimeConverter.ToFIXTimeOnly(tSec, TimePrecision.Nanosecond), Is.EqualTo("11:03:05.000000000"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(tMs, TimePrecision.Nanosecond), Is.EqualTo("11:03:05.123000000"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(tMicro, TimePrecision.Nanosecond), Is.EqualTo("11:03:05.000654000"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(tNanoFull, TimePrecision.Nanosecond), Is.EqualTo("11:03:05.123654700"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(tNano100, TimePrecision.Nanosecond), Is.EqualTo("11:03:05.000000100"));
#pragma warning restore CS0618
    }

    [Test]
    public void ToFIXTimeOnlyTest_DateTimeParam() {
        var dtSec = DateTime.SpecifyKind(TimeHelper.MakeDateTime(2002, 12, 01, 11, 03, 05, 0, 0, 0), DateTimeKind.Utc);
        var dtMs = DateTime.SpecifyKind(TimeHelper.MakeDateTime(2002, 12, 01, 11, 03, 05, 123, 0, 0), DateTimeKind.Utc);
        var dtMicro = DateTime.SpecifyKind(TimeHelper.MakeDateTime(2002, 12, 01, 11, 03, 05, 0, 654, 0), DateTimeKind.Utc);
        var dtNanoFull = DateTime.SpecifyKind(TimeHelper.MakeDateTime(2002, 12, 01, 11, 03, 05, 123, 654, 700), DateTimeKind.Utc);
        var dtNano100 = DateTime.SpecifyKind(TimeHelper.MakeDateTime(2002, 12, 01, 11, 03, 05, 000, 000, 100), DateTimeKind.Utc);

#pragma warning disable CS0618
        // string ToFIXTimeOnly(DateTime dt, TimePrecision precision)
        Assert.That(DateTimeConverter.ToFIXTimeOnly(dtSec, TimePrecision.Second), Is.EqualTo("11:03:05"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(dtMs, TimePrecision.Second), Is.EqualTo("11:03:05"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(dtMicro, TimePrecision.Second), Is.EqualTo("11:03:05"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(dtNanoFull, TimePrecision.Second), Is.EqualTo("11:03:05"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(dtNano100, TimePrecision.Second), Is.EqualTo("11:03:05"));

        Assert.That(DateTimeConverter.ToFIXTimeOnly(dtSec, TimePrecision.Millisecond), Is.EqualTo("11:03:05.000"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(dtMs, TimePrecision.Millisecond), Is.EqualTo("11:03:05.123"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(dtMicro, TimePrecision.Millisecond), Is.EqualTo("11:03:05.000"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(dtNanoFull, TimePrecision.Millisecond), Is.EqualTo("11:03:05.123"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(dtNano100, TimePrecision.Millisecond), Is.EqualTo("11:03:05.000"));

        Assert.That(DateTimeConverter.ToFIXTimeOnly(dtSec, TimePrecision.Microsecond), Is.EqualTo("11:03:05.000000"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(dtMs, TimePrecision.Microsecond), Is.EqualTo("11:03:05.123000"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(dtMicro, TimePrecision.Microsecond), Is.EqualTo("11:03:05.000654"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(dtNanoFull, TimePrecision.Microsecond), Is.EqualTo("11:03:05.123654"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(dtNano100, TimePrecision.Microsecond), Is.EqualTo("11:03:05.000000"));

        Assert.That(DateTimeConverter.ToFIXTimeOnly(dtSec, TimePrecision.Nanosecond), Is.EqualTo("11:03:05.000000000"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(dtMs, TimePrecision.Nanosecond), Is.EqualTo("11:03:05.123000000"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(dtMicro, TimePrecision.Nanosecond), Is.EqualTo("11:03:05.000654000"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(dtNanoFull, TimePrecision.Nanosecond), Is.EqualTo("11:03:05.123654700"));
        Assert.That(DateTimeConverter.ToFIXTimeOnly(dtNano100, TimePrecision.Nanosecond), Is.EqualTo("11:03:05.000000100"));
#pragma warning restore CS0618
    }
}
