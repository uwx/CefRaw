using NUnit.Framework;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefSize" /> struct.</summary>
public static unsafe partial class CefSizeTests
{
    /// <summary>Validates that the <see cref="CefSize" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefSize>(), Is.EqualTo(sizeof(CefSize)));
    }

    /// <summary>Validates that the <see cref="CefSize" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefSize).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefSize" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        Assert.That(sizeof(CefSize), Is.EqualTo(8));
    }
}
