using NUnit.Framework;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefRect" /> struct.</summary>
public static unsafe partial class CefRectTests
{
    /// <summary>Validates that the <see cref="CefRect" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefRect>(), Is.EqualTo(sizeof(CefRect)));
    }

    /// <summary>Validates that the <see cref="CefRect" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefRect).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefRect" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        Assert.That(sizeof(CefRect), Is.EqualTo(16));
    }
}
