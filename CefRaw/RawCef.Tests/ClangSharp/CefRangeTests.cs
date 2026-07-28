using NUnit.Framework;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefRange" /> struct.</summary>
public static unsafe partial class CefRangeTests
{
    /// <summary>Validates that the <see cref="CefRange" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefRange>(), Is.EqualTo(sizeof(CefRange)));
    }

    /// <summary>Validates that the <see cref="CefRange" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefRange).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefRange" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        Assert.That(sizeof(CefRange), Is.EqualTo(8));
    }
}
