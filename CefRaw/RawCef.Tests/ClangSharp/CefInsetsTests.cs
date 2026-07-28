using NUnit.Framework;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefInsets" /> struct.</summary>
public static unsafe partial class CefInsetsTests
{
    /// <summary>Validates that the <see cref="CefInsets" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefInsets>(), Is.EqualTo(sizeof(CefInsets)));
    }

    /// <summary>Validates that the <see cref="CefInsets" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefInsets).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefInsets" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        Assert.That(sizeof(CefInsets), Is.EqualTo(16));
    }
}
