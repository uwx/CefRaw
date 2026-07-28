using NUnit.Framework;
using System;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefCursorInfo" /> struct.</summary>
public static unsafe partial class CefCursorInfoTests
{
    /// <summary>Validates that the <see cref="CefCursorInfo" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefCursorInfo>(), Is.EqualTo(sizeof(CefCursorInfo)));
    }

    /// <summary>Validates that the <see cref="CefCursorInfo" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefCursorInfo).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefCursorInfo" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        if (Environment.Is64BitProcess)
        {
            Assert.That(sizeof(CefCursorInfo), Is.EqualTo(32));
        }
        else
        {
            Assert.That(sizeof(CefCursorInfo), Is.EqualTo(24));
        }
    }
}
