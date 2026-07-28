using NUnit.Framework;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefTouchEvent" /> struct.</summary>
public static unsafe partial class CefTouchEventTests
{
    /// <summary>Validates that the <see cref="CefTouchEvent" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefTouchEvent>(), Is.EqualTo(sizeof(CefTouchEvent)));
    }

    /// <summary>Validates that the <see cref="CefTouchEvent" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefTouchEvent).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefTouchEvent" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        Assert.That(sizeof(CefTouchEvent), Is.EqualTo(40));
    }
}
