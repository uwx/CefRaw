using NUnit.Framework;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefURLPartsTraits" /> struct.</summary>
public static unsafe partial class CefURLPartsTraitsTests
{
    /// <summary>Validates that the <see cref="CefURLPartsTraits" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefURLPartsTraits>(), Is.EqualTo(sizeof(CefURLPartsTraits)));
    }

    /// <summary>Validates that the <see cref="CefURLPartsTraits" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefURLPartsTraits).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefURLPartsTraits" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        Assert.That(sizeof(CefURLPartsTraits), Is.EqualTo(1));
    }
}
