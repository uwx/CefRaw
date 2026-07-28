using NUnit.Framework;
using System;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="_cef_component_t" /> struct.</summary>
public static unsafe partial class _cef_component_tTests
{
    /// <summary>Validates that the <see cref="_cef_component_t" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<_cef_component_t>(), Is.EqualTo(sizeof(_cef_component_t)));
    }

    /// <summary>Validates that the <see cref="_cef_component_t" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(_cef_component_t).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="_cef_component_t" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        if (Environment.Is64BitProcess)
        {
            Assert.That(sizeof(_cef_component_t), Is.EqualTo(72));
        }
        else
        {
            Assert.That(sizeof(_cef_component_t), Is.EqualTo(36));
        }
    }
}
