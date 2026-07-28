using NUnit.Framework;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefTime" /> struct.</summary>
public static unsafe partial class CefTimeTests
{
    /// <summary>Validates that the <see cref="CefTime" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefTime>(), Is.EqualTo(sizeof(CefTime)));
    }

    /// <summary>Validates that the <see cref="CefTime" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefTime).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefTime" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        Assert.That(sizeof(CefTime), Is.EqualTo(32));
    }
}
