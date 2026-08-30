using System;
using NUnit.Framework;
using QuickFix;

namespace UnitTests;

[TestFixture]
public class SettingsDictionaryTests
{
    [TearDown]
    public void Teardown()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
    }

    [Test]
    public void TestSetGetString()
    {
        SettingsDictionary d = new();
        d.SetString("STRINGKEY1", "STRINGVALUE1");
        d.SetString("STRINGKEY2", "stringvalue2");
        Assert.That(d.GetString("STRINGKEY1"), Is.EqualTo("STRINGVALUE1"));
        Assert.That(d.GetString("STRINGKEY2"), Is.EqualTo("stringvalue2"));
        Assert.Throws<ConfigError>(delegate { d.GetString("STRINGKEY3"); });

        #pragma warning disable CS0618
        Assert.That(d.GetString("STRINGKEY2", true), Is.EqualTo("STRINGVALUE2"));
        #pragma warning restore CS0618
    }

    [Test]
    public void TestGetChar()
    {
        SettingsDictionary d = new();
        d.SetString("CHARKEY1", "x");
        d.SetString("CHAR-TOOLONG", "xxxxxxx");
        d.SetString("CHAR-EMPTY", "");

        Assert.That(d.GetChar("cHaRkEy1"), Is.EqualTo('x'));

        var ex = Assert.Throws<ConfigError>(delegate { d.GetChar("CHAR-TOOLONG"); })!;
        Assert.That(ex.Message, Is.EqualTo("Configuration failed: Value is not a char for key: CHAR-TOOLONG"));

        ex = Assert.Throws<ConfigError>(delegate { d.GetChar("CHAR-EMPTY"); })!;
        Assert.That(ex.Message, Is.EqualTo("Configuration failed: Value is empty for key: CHAR-EMPTY"));

        ex = Assert.Throws<ConfigError>(delegate { d.GetChar("NOTPRESENT"); })!;
        Assert.That(ex.Message, Is.EqualTo("Configuration failed: No value for key: NOTPRESENT"));
    }

    [Test]
    public void TestSetGetLong()
    {
        SettingsDictionary d = new();
        d.SetLong("LONGKEY1", 12);
        d.SetLong("LONGKEY2", 987362);
        d.SetString("BADLONGKEY", "AB12");
        Assert.That(d.GetLong("LONGKEY1"), Is.EqualTo(12));
        Assert.That(d.GetLong("LONGKEY2"), Is.EqualTo(987362));
        Assert.Throws<ConfigError>(delegate { d.GetLong("LONGKEY3"); });
        Assert.Throws<ConfigError>(delegate { d.GetLong("BADLONGKEY"); });
    }

    [Test]
    public void TestSetDouble()
    {
        // make sure that QF/n uses the invariant culture, no matter what the current culture is
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("fr-FR");

        SettingsDictionary d = new();
        d.SetDouble("DOUBLEKEY1", 12.3);
        d.SetDouble("DOUBLEKEY2", 987362.987362);

        Assert.That(d.GetString("DOUBLEKEY1"), Is.EqualTo("12.3"));
        Assert.That(d.GetString("DOUBLEKEY2"), Is.EqualTo("987362.987362"));
    }

    [Test]
    public void TestGetDouble()
    {
        // make sure that QF/n uses the invariant culture, no matter what the current culture is
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("fr-FR");

        SettingsDictionary d = new();
        d.SetString("DOUBLEKEY1", "12.3");
        d.SetString("DOUBLEKEY2", "987362.987362");
        d.SetString("BADDOUBLEKEY", "AB12.3");
        d.SetString("FOREIGNFORMAT", "44,44");

        Assert.That(d.GetDouble("DOUBLEKEY1"), Is.EqualTo(12.3));
        Assert.That(d.GetDouble("DOUBLEKEY2"), Is.EqualTo(987362.987362));
        Assert.Throws<ConfigError>(delegate { d.GetDouble("DOUBLEKEY3"); });
        Assert.Throws<ConfigError>(delegate { d.GetDouble("BADDOUBLEKEY"); });
        Assert.Throws<ConfigError>(delegate { d.GetDouble("BADDOUBLEKEY"); });
    }

    [Test]
    public void TestSetGetBool()
    {
        SettingsDictionary d = new();
        d.SetBool("BOOLKEY1", true);
        d.SetBool("BOOLKEY2", false);
        d.SetString("BADBOOLKEY2", "YO");
        Assert.That(d.GetBool("BOOLKEY1"), Is.True);
        Assert.That(d.GetBool("BOOLKEY2"), Is.False);
        Assert.Throws<ConfigError>(delegate { d.GetBool("BOOLKEY3"); });
        Assert.Throws<ConfigError>(delegate { d.GetBool("BADBOOLKEY2"); });
    }

    [Test]
    public void TestGetInt()
    {
        SettingsDictionary d = new();
        d.SetString("INTKEY1", "100");
        d.SetString("INT-WRONG", "pants");
        d.SetString("INT-EMPTY", "");

        Assert.That(d.GetInt("InTkEy1"), Is.EqualTo(100));

        var ex = Assert.Throws<ConfigError>(delegate { d.GetInt("INT-WRONG"); })!;
        Assert.That(ex.Message, Is.EqualTo("Configuration failed: Incorrect data type"));

        //ex = Assert.Throws<ConfigError>(delegate { d.GetInt("INT-EMPTY"); })!;
        //Assert.That(ex.Message, Is.EqualTo("Value is empty for key: INT-EMPTY"));

        ex = Assert.Throws<ConfigError>(delegate { d.GetInt("NOTPRESENT"); })!;
        Assert.That(ex.Message, Is.EqualTo("Configuration failed: No value for key: NOTPRESENT"));
    }

    [Test]
    public void TestIsBoolPresentAndTrue() {
        SettingsDictionary d = new();
        d.SetBool("BOOLKEY-T", true);
        d.SetBool("BOOLKEY-F", false);
        Assert.That(d.IsBoolPresentAndTrue("BOOLKEY-T"), Is.True);
        Assert.That(d.IsBoolPresentAndTrue("BOOLKEY-F"), Is.False);
        Assert.That(d.IsBoolPresentAndTrue("nonexistent key"), Is.False);
    }

    [Test]
    public void TestGetDay()
    {
        SettingsDictionary d = new();

        d.SetString("DAY1", "SU");
        d.SetString("DAY2", "MO");
        d.SetString("DAY3", "TU");
        d.SetString("DAY4", "WE");
        d.SetString("DAY5", "TH");
        d.SetString("DAY6", "FR");
        d.SetString("DAY7", "SA");
        Assert.That(d.GetDay("DAY1"), Is.EqualTo(DayOfWeek.Sunday));
        Assert.That(d.GetDay("DAY2"), Is.EqualTo(DayOfWeek.Monday));
        Assert.That(d.GetDay("DAY3"), Is.EqualTo(DayOfWeek.Tuesday));
        Assert.That(d.GetDay("DAY4"), Is.EqualTo(DayOfWeek.Wednesday));
        Assert.That(d.GetDay("DAY5"), Is.EqualTo(DayOfWeek.Thursday));
        Assert.That(d.GetDay("DAY6"), Is.EqualTo(DayOfWeek.Friday));
        Assert.That(d.GetDay("DAY7"), Is.EqualTo(DayOfWeek.Saturday));

        d.SetString("DAY_X", "invalid");
        var ex = Assert.Throws(typeof(ArgumentException), delegate { d.GetDay("DAY_X"); });
        Assert.That(ex!.Message, Does.Contain("Cannot recognize this day: 'invalid'"));
    }

    [Test]
    public void TestSetDay() {
        SettingsDictionary d = new();
        d.SetDay("DAY1", DayOfWeek.Monday);
        d.SetDay("DAY4", DayOfWeek.Thursday);

        Assert.That(d.GetString("DAY1"), Is.EqualTo("Monday"));
        Assert.That(d.GetString("DAY4"), Is.EqualTo("Thursday"));

        var ex = Assert.Throws(typeof(ArgumentException), delegate { d.SetDay("X", (DayOfWeek)9); });
        Assert.That(ex!.Message, Does.Contain("Not a valid DayOfWeek value"));
    }

    [Test]
    public void TestMerge()
    {
        SettingsDictionary first = new();
        first.SetString("FIRSTKEY", "FIRSTVALUE");
        first.SetString("THIRDKEY", "FIRST");

        SettingsDictionary second = new();
        second.SetString("SECONDKEY", "SECONDVALUE");
        second.SetString("THIRDKEY", "SECOND");

        first.Merge(second);
        Assert.That(first.GetString("FIRSTKEY"), Is.EqualTo("FIRSTVALUE"));
        Assert.That(first.GetString("SECONDKEY"), Is.EqualTo("SECONDVALUE"));
        Assert.That(first.GetString("THIRDKEY"), Is.EqualTo("FIRST"));
    }

    [Test]
    public void TestValueEquality()
    {
        SettingsDictionary first = new("MyName");
        SettingsDictionary second = new("MyName");
        Assert.That(first.Equals(second), Is.True);

        first.SetString("THIRDKEY", "FIRST");
        second.SetString("THIRDKEY", "SECOND");
        Assert.That(first.Equals(second), Is.False);

        first.SetString("THIRDKEY", "SECOND");
        Assert.That(first.Equals(second), Is.True);

        first.SetString("FIRSTKEY", "FIRSTVALUE");
        second.SetString("SECONDKEY", "SECONDVALUE");
        Assert.That(first.Equals(second), Is.False);

        first.SetString("SECONDKEY", "SECONDVALUE");
        second.SetString("FIRSTKEY", "FIRSTVALUE");
        Assert.That(first.Equals(second), Is.True);

        SettingsDictionary third = new("Name1");
        SettingsDictionary fourth = new("Name2");
        Assert.That(third.Equals(fourth), Is.False);
    }

    [Test]
    public void TestCopyCtor() {
        SettingsDictionary orig = new("orig");
        orig.SetString("uno", "One");
        orig.SetLong("dos", 2);

        SettingsDictionary dupe = new("dupe", orig);

        Assert.That(dupe.Count, Is.EqualTo(2));
        Assert.That(dupe.GetString("uNo"), Is.EqualTo("One"));
        Assert.That(dupe.GetString("DOs"), Is.EqualTo("2"));
    }

    [Test]
    public void TestGetIntArray() {
        SettingsDictionary settings = new("test");

        // empty string becomes empty array
        settings.SetString("myintarray", "");
        Assert.That(settings.GetIntArray("myintarray"), Is.Empty);

        int[] expected = [1, 33, 999];

        // the intended input format
        settings.SetString("myintarray", "1,33,999");
        Assert.That(settings.GetIntArray("myintarray"), Is.EqualTo(expected));

        // trailing & repeated commas ignored
        settings.SetString("myintarray", "1,33,,999,");
        Assert.That(settings.GetIntArray("myintarray"), Is.EqualTo(expected));

        // spaces ignored
        settings.SetString("myintarray", "1, 33, , 999, ");
        Assert.That(settings.GetIntArray("myintarray"), Is.EqualTo(expected));

        // sorted, dupes consolidated
        settings.SetString("myintarray", "33, 1, , 999, 33, 1");
        Assert.That(settings.GetIntArray("myintarray"), Is.EqualTo(expected));

        // key not found
        Assert.Throws<ConfigError>(delegate { settings.GetIntArray("nope"); });

        // invalid values
        settings.SetString("myintarray", "1, 33, , 999, fourteen, ");
        Assert.Throws<ConfigError>(delegate { settings.GetIntArray("myintarray"); });
    }
}
