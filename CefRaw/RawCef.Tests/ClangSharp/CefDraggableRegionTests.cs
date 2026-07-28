using NUnit.Framework;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefDraggableRegion" /> struct.</summary>
public static unsafe partial class CefDraggableRegionTests
{
    /// <summary>Validates that the <see cref="CefDraggableRegion" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefDraggableRegion>(), Is.EqualTo(sizeof(CefDraggableRegion)));
    }

    /// <summary>Validates that the <see cref="CefDraggableRegion" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefDraggableRegion).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefDraggableRegion" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        Assert.That(sizeof(CefDraggableRegion), Is.EqualTo(20));
    }
}
