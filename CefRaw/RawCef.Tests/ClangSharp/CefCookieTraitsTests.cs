using NUnit.Framework;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefCookieTraits" /> struct.</summary>
public static unsafe partial class CefCookieTraitsTests
{
    /// <summary>Validates that the <see cref="CefCookieTraits" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefCookieTraits>(), Is.EqualTo(sizeof(CefCookieTraits)));
    }

    /// <summary>Validates that the <see cref="CefCookieTraits" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefCookieTraits).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefCookieTraits" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        Assert.That(sizeof(CefCookieTraits), Is.EqualTo(1));
    }
}
