using NUnit.Framework;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefStringTraitsUTF8" /> struct.</summary>
public static unsafe partial class CefStringTraitsUTF8Tests
{
    /// <summary>Validates that the <see cref="CefStringTraitsUTF8" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefStringTraitsUTF8>(), Is.EqualTo(sizeof(CefStringTraitsUTF8)));
    }

    /// <summary>Validates that the <see cref="CefStringTraitsUTF8" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefStringTraitsUTF8).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefStringTraitsUTF8" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        Assert.That(sizeof(CefStringTraitsUTF8), Is.EqualTo(1));
    }
}
