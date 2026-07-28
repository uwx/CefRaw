using NUnit.Framework;
using System;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefScreenInfo" /> struct.</summary>
public static unsafe partial class CefScreenInfoTests
{
    /// <summary>Validates that the <see cref="CefScreenInfo" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefScreenInfo>(), Is.EqualTo(sizeof(CefScreenInfo)));
    }

    /// <summary>Validates that the <see cref="CefScreenInfo" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefScreenInfo).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefScreenInfo" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        if (Environment.Is64BitProcess)
        {
            Assert.That(sizeof(CefScreenInfo), Is.EqualTo(56));
        }
        else
        {
            Assert.That(sizeof(CefScreenInfo), Is.EqualTo(52));
        }
    }
}
