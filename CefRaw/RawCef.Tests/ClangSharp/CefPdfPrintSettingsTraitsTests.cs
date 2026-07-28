using NUnit.Framework;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefPdfPrintSettingsTraits" /> struct.</summary>
public static unsafe partial class CefPdfPrintSettingsTraitsTests
{
    /// <summary>Validates that the <see cref="CefPdfPrintSettingsTraits" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefPdfPrintSettingsTraits>(), Is.EqualTo(sizeof(CefPdfPrintSettingsTraits)));
    }

    /// <summary>Validates that the <see cref="CefPdfPrintSettingsTraits" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefPdfPrintSettingsTraits).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefPdfPrintSettingsTraits" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        Assert.That(sizeof(CefPdfPrintSettingsTraits), Is.EqualTo(1));
    }
}
