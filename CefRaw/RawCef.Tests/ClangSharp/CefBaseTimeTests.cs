using NUnit.Framework;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefBaseTime" /> struct.</summary>
public static unsafe partial class CefBaseTimeTests
{
    /// <summary>Validates that the <see cref="CefBaseTime" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefBaseTime>(), Is.EqualTo(sizeof(CefBaseTime)));
    }

    /// <summary>Validates that the <see cref="CefBaseTime" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefBaseTime).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefBaseTime" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        Assert.That(sizeof(CefBaseTime), Is.EqualTo(8));
    }
}
