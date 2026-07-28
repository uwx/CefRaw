using NUnit.Framework;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefLinuxWindowPropertiesTraits" /> struct.</summary>
public static unsafe partial class CefLinuxWindowPropertiesTraitsTests
{
    /// <summary>Validates that the <see cref="CefLinuxWindowPropertiesTraits" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefLinuxWindowPropertiesTraits>(), Is.EqualTo(sizeof(CefLinuxWindowPropertiesTraits)));
    }

    /// <summary>Validates that the <see cref="CefLinuxWindowPropertiesTraits" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefLinuxWindowPropertiesTraits).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefLinuxWindowPropertiesTraits" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        Assert.That(sizeof(CefLinuxWindowPropertiesTraits), Is.EqualTo(1));
    }
}
