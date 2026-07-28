using NUnit.Framework;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefMediaSinkDeviceInfoTraits" /> struct.</summary>
public static unsafe partial class CefMediaSinkDeviceInfoTraitsTests
{
    /// <summary>Validates that the <see cref="CefMediaSinkDeviceInfoTraits" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefMediaSinkDeviceInfoTraits>(), Is.EqualTo(sizeof(CefMediaSinkDeviceInfoTraits)));
    }

    /// <summary>Validates that the <see cref="CefMediaSinkDeviceInfoTraits" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefMediaSinkDeviceInfoTraits).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefMediaSinkDeviceInfoTraits" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        Assert.That(sizeof(CefMediaSinkDeviceInfoTraits), Is.EqualTo(1));
    }
}
