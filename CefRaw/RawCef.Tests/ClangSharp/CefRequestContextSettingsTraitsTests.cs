using NUnit.Framework;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefRequestContextSettingsTraits" /> struct.</summary>
public static unsafe partial class CefRequestContextSettingsTraitsTests
{
    /// <summary>Validates that the <see cref="CefRequestContextSettingsTraits" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefRequestContextSettingsTraits>(), Is.EqualTo(sizeof(CefRequestContextSettingsTraits)));
    }

    /// <summary>Validates that the <see cref="CefRequestContextSettingsTraits" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefRequestContextSettingsTraits).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefRequestContextSettingsTraits" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        Assert.That(sizeof(CefRequestContextSettingsTraits), Is.EqualTo(1));
    }
}
