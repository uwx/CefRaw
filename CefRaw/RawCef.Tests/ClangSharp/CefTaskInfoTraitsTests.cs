using NUnit.Framework;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefTaskInfoTraits" /> struct.</summary>
public static unsafe partial class CefTaskInfoTraitsTests
{
    /// <summary>Validates that the <see cref="CefTaskInfoTraits" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefTaskInfoTraits>(), Is.EqualTo(sizeof(CefTaskInfoTraits)));
    }

    /// <summary>Validates that the <see cref="CefTaskInfoTraits" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefTaskInfoTraits).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefTaskInfoTraits" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        Assert.That(sizeof(CefTaskInfoTraits), Is.EqualTo(1));
    }
}
