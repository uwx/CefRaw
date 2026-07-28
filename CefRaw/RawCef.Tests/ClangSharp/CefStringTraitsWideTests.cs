using NUnit.Framework;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefStringTraitsWide" /> struct.</summary>
public static unsafe partial class CefStringTraitsWideTests
{
    /// <summary>Validates that the <see cref="CefStringTraitsWide" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefStringTraitsWide>(), Is.EqualTo(sizeof(CefStringTraitsWide)));
    }

    /// <summary>Validates that the <see cref="CefStringTraitsWide" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefStringTraitsWide).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefStringTraitsWide" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        Assert.That(sizeof(CefStringTraitsWide), Is.EqualTo(1));
    }
}
