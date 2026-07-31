using RawCef;
using RawCef.Native;

namespace RawCef.Tests;

/// <summary>
/// Tests for basic CEF utility types that do NOT allocate through CEF's
/// PartitionAlloc. These tests are safe to run without <c>cef_initialize</c>.
///
/// Tests that require CEF allocation (e.g. <see cref="CefStringListRef"/>,
/// <see cref="CefStringMapRef"/>, <see cref="CefStringMultimapRef"/>,
/// <see cref="CefString"/> setter with copy, <see cref="CefStringRef.AllocUserfree"/>,
/// and <see cref="Cef"/> lifecycle calls) live in
/// <see cref="CefIntegrationTests"/> which has a proper CEF initialization fixture.
/// </summary>
public unsafe class BasicCefFunctionalityTests
{
    // ── CefString (read-only, no native allocation) ──────────────────

    [Fact]
    public void CefString_ConstructedWithNonNullPtr_HasEmptyDefault()
    {
        _cef_string_utf16_t nativeStr = default;
        var cefStr = new CefString(&nativeStr);

        string? value = cefStr.Value;
        Assert.Equal(string.Empty, value);
    }

    [Fact]
    public void CefString_ConstructedWithNullStrField_ReturnsEmpty()
    {
        // Zeroed struct has str=null, length=0.
        _cef_string_utf16_t nativeStr = default;
        var cefStr = new CefString(&nativeStr);

        Assert.Equal(string.Empty, cefStr.Value);
    }

    // ── CefStringRef.ToString (pure managed read, no allocation) ─────

    [Fact]
    public void CefStringRef_ToString_WithNullPtr_ReturnsNull()
    {
        string? result = CefStringRef.ToString(null);
        Assert.Null(result);
    }

    [Fact]
    public void CefStringRef_ToStringAndFree_WithNullPtr_ReturnsNull()
    {
        string? result = CefStringRef.ToStringAndFree(null);
        Assert.Null(result);
    }

    [Fact]
    public void CefStringRef_AllocUserfree_WithNull_ReturnsNull()
    {
        _cef_string_utf16_t* result = CefStringRef.AllocUserfree(null);
        Assert.True(result == null);
    }

    // ── CefStringRef.FillFromPinned (copy=0, no allocation) ──────────

    [Fact]
    public void CefStringRef_FillFromPinned_RoundTrips()
    {
        _cef_string_utf16_t nativeStr = default;
        string input = "test string";

        fixed (char* p = input)
        {
            CefStringRef.FillFromPinned(&nativeStr, p, input.Length);
        }

        string? result = CefStringRef.ToString(&nativeStr);
        Assert.Equal(input, result);
    }

    [Fact]
    public void CefStringRef_FillFromPinned_EmptyString()
    {
        _cef_string_utf16_t nativeStr = default;
        string input = "";

        fixed (char* p = input)
        {
            CefStringRef.FillFromPinned(&nativeStr, p, 0);
        }

        string? result = CefStringRef.ToString(&nativeStr);
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void CefStringRef_FillFromPinned_ThenOverwrite_RoundTrips()
    {
        _cef_string_utf16_t nativeStr = default;

        // Fill once
        string first = "first";
        fixed (char* p = first)
        {
            CefStringRef.FillFromPinned(&nativeStr, p, first.Length);
        }
        Assert.Equal("first", CefStringRef.ToString(&nativeStr));

        // Overwrite with different string
        string second = "second value";
        fixed (char* p = second)
        {
            CefStringRef.FillFromPinned(&nativeStr, p, second.Length);
        }
        Assert.Equal("second value", CefStringRef.ToString(&nativeStr));
    }

    [Fact]
    public void CefStringRef_FillFromPinned_Unicode_RoundTrips()
    {
        _cef_string_utf16_t nativeStr = default;
        string input = "こんにちは 🌍";

        fixed (char* p = input)
        {
            CefStringRef.FillFromPinned(&nativeStr, p, input.Length);
        }

        string? result = CefStringRef.ToString(&nativeStr);
        Assert.Equal(input, result);
    }

    // ── Cef.IsShutdown (pure managed flag) ───────────────────────────

    [Fact]
    public void Cef_Initially_IsNotShutdown()
    {
        Assert.False(Cef.IsShutdown);
    }

    // ── CefStringRef.ToString with zeroed struct ─────────────────────

    [Fact]
    public void CefStringRef_ToString_WithZeroedStruct_ReturnsEmpty()
    {
        _cef_string_utf16_t nativeStr = default;

        string? result = CefStringRef.ToString(&nativeStr);
        Assert.Equal(string.Empty, result);
    }
}
