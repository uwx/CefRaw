using NUnit.Framework;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefBrowserSettingsTraits" /> struct.</summary>
public static unsafe partial class CefBrowserSettingsTraitsTests
{
    /// <summary>Validates that the <see cref="CefBrowserSettingsTraits" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefBrowserSettingsTraits>(), Is.EqualTo(sizeof(CefBrowserSettingsTraits)));
    }

    /// <summary>Validates that the <see cref="CefBrowserSettingsTraits" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefBrowserSettingsTraits).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefBrowserSettingsTraits" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        Assert.That(sizeof(CefBrowserSettingsTraits), Is.EqualTo(1));
    }
}
