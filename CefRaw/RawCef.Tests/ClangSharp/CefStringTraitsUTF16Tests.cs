using NUnit.Framework;
using System.Runtime.InteropServices;

namespace RawCef.Native.UnitTests;

/// <summary>Provides validation of the <see cref="CefStringTraitsUTF16" /> struct.</summary>
public static unsafe partial class CefStringTraitsUTF16Tests
{
    /// <summary>Validates that the <see cref="CefStringTraitsUTF16" /> struct is blittable.</summary>
    [Test]
    public static void IsBlittableTest()
    {
        Assert.That(Marshal.SizeOf<CefStringTraitsUTF16>(), Is.EqualTo(sizeof(CefStringTraitsUTF16)));
    }

    /// <summary>Validates that the <see cref="CefStringTraitsUTF16" /> struct has the right <see cref="LayoutKind" />.</summary>
    [Test]
    public static void IsLayoutSequentialTest()
    {
        Assert.That(typeof(CefStringTraitsUTF16).IsLayoutSequential, Is.True);
    }

    /// <summary>Validates that the <see cref="CefStringTraitsUTF16" /> struct has the correct size.</summary>
    [Test]
    public static void SizeOfTest()
    {
        Assert.That(sizeof(CefStringTraitsUTF16), Is.EqualTo(1));
    }
}
