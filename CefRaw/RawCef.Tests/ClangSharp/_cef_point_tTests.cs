using NUnit.Framework;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="_cef_point_t" /> struct.</summary>
public static unsafe partial class _cef_point_tTests
{
    /// <summary>Validates that the <see cref="_cef_point_t" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<_cef_point_t>(), Is.EqualTo(sizeof(_cef_point_t)));
    }

    /// <summary>Validates that the <see cref="_cef_point_t" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(_cef_point_t).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="_cef_point_t" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        Assert.That(sizeof(_cef_point_t), Is.EqualTo(8));
    }
}
