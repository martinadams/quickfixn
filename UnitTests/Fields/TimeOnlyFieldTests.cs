using System;
using NUnit.Framework;
using QuickFix.Fields;
using QuickFix.Fields.Converters;

namespace UnitTests.Fields;

[TestFixture]
public class TimeOnlyFieldTests
{
    [Test]
    public void DefaultCtorTest()
    {
        TimeOnlyField tof = new(Tags.MDEntryTime);
        Assert.That(tof.Value, Is.EqualTo(new TimeOnly(0, 0, 0, 0)));
        Assert.That(tof.ToString(), Is.EqualTo("00:00:00.000"));
        Assert.That(tof.ToStringField(), Is.EqualTo("273=00:00:00.000"));
    }

    [Test]
    public void CtorDefaultPrecisionTest()
    {
        // Default display precision is Milliseconds
        TimeOnlyField tof = new(Tags.MDEntryTime, new TimeOnly(13, 30, 45, 999));
        Assert.That(tof.Value, Is.EqualTo(new TimeOnly(13, 30, 45, 999)));
        Assert.That(tof.ToString(), Is.EqualTo("13:30:45.999"));
        Assert.That(tof.ToStringField(), Is.EqualTo("273=13:30:45.999"));
    }

    [Test]
    public void CtorWithPrecisionTest()
    {
        TimeOnlyField tofSec =
            new(Tags.MDEntryTime, new TimeOnly(13, 30, 45, 999, 50), TimePrecision.Second);
        TimeOnlyField tofMilli =
            new(Tags.MDEntryTime, new TimeOnly(13, 30, 45, 999, 50), TimePrecision.Millisecond);
        TimeOnlyField tofMicro =
            new(Tags.MDEntryTime, new TimeOnly(13, 30, 45, 999, 50), TimePrecision.Microsecond);
        TimeOnlyField tofNano =
            new(Tags.MDEntryTime, new TimeOnly(13, 30, 45, 999, 50), TimePrecision.Nanosecond);

        Assert.That(tofSec.ToString(),   Is.EqualTo("13:30:45"));
        Assert.That(tofMilli.ToString(), Is.EqualTo("13:30:45.999"));
        Assert.That(tofMicro.ToString(), Is.EqualTo("13:30:45.999050"));
        Assert.That(tofNano.ToString(),  Is.EqualTo("13:30:45.999050000"));
    }

    [Test]
    public void LegacyTest()
    {
#pragma warning disable CS0618
        DateTime dt = new(2025, 10, 31, 17, 30, 59, 333, DateTimeKind.Utc);
        TimeOnlyField tof = new(Tags.MDEntryTime, dt);

        Assert.That(tof.ToString(), Is.EqualTo("17:30:59.333"));
        Assert.That(tof.ToStringField(), Is.EqualTo("273=17:30:59.333"));

        tof = new(273, dt, false);
        Assert.That(tof.ToString(), Is.EqualTo("17:30:59"));
        Assert.That(tof.ToStringField(), Is.EqualTo("273=17:30:59"));

        tof = new(273, dt, true);
        Assert.That(tof.ToString(), Is.EqualTo("17:30:59.333"));
        Assert.That(tof.ToStringField(), Is.EqualTo("273=17:30:59.333"));

        tof = new(273, dt, TimePrecision.Second);
        Assert.That(tof.ToString(), Is.EqualTo("17:30:59"));
        Assert.That(tof.ToStringField(), Is.EqualTo("273=17:30:59"));

        tof = new(273, dt, TimePrecision.Millisecond);
        Assert.That(tof.ToString(), Is.EqualTo("17:30:59.333"));
        Assert.That(tof.ToStringField(), Is.EqualTo("273=17:30:59.333"));

        tof = new(273, dt, TimePrecision.Nanosecond);
        Assert.That(tof.ToString(), Is.EqualTo("17:30:59.333000000"));
        Assert.That(tof.ToStringField(), Is.EqualTo("273=17:30:59.333000000"));
#pragma warning restore CS0618
    }
}
