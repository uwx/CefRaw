using NUnit.Framework;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefSettingsTraits" /> struct.</summary>
public static unsafe partial class CefSettingsTraitsTests
{
    /// <summary>Validates that the <see cref="CefSettingsTraits" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefSettingsTraits>(), Is.EqualTo(sizeof(CefSettingsTraits)));
    }

    /// <summary>Validates that the <see cref="CefSettingsTraits" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefSettingsTraits).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefSettingsTraits" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        Assert.That(sizeof(CefSettingsTraits), Is.EqualTo(1));
    }
}
