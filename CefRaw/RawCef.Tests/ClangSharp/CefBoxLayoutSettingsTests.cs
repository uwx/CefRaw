using NUnit.Framework;
using System;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefBoxLayoutSettings" /> struct.</summary>
public static unsafe partial class CefBoxLayoutSettingsTests
{
    /// <summary>Validates that the <see cref="CefBoxLayoutSettings" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefBoxLayoutSettings>(), Is.EqualTo(sizeof(CefBoxLayoutSettings)));
    }

    /// <summary>Validates that the <see cref="CefBoxLayoutSettings" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefBoxLayoutSettings).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefBoxLayoutSettings" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        if (Environment.Is64BitProcess)
        {
            Assert.That(sizeof(CefBoxLayoutSettings), Is.EqualTo(56));
        }
        else
        {
            Assert.That(sizeof(CefBoxLayoutSettings), Is.EqualTo(52));
        }
    }
}
