using RawCef;
using RawCef.Native;

namespace RawCef.Tests;

/// <summary>
/// Tests for basic CEF utility types: <see cref="CefString"/>, <see cref="CefStringRef"/>,
/// string-list/map wrappers, and the <see cref="Cef"/> lifecycle guard.
///
/// These tests require the CEF native library (libcef.dll) to be present in the
/// output directory. The <see cref="RawCef.Binaries.Win64.Debug"/> project reference
/// ensures the DLLs are copied.
/// </summary>
public unsafe class BasicCefFunctionalityTests
{
    // ── CefString ─────────────────────────────────────────────────────

    [Fact]
    public void CefString_ConstructedWithNonNullPtr_HasEmptyDefault()
    {
        // Allocate a stack CEF string and wrap it.
        _cef_string_utf16_t nativeStr = default;
        var cefStr = new CefString(&nativeStr);

        string? value = cefStr.Value;
        Assert.Equal(string.Empty, value);
    }

    [Fact]
    public void CefString_SetAndGet_RoundTrips()
    {
        _cef_string_utf16_t nativeStr = default;
        var cefStr = new CefString(&nativeStr);

        cefStr.Value = "hello";

        Assert.Equal("hello", cefStr.Value);
    }

    [Fact]
    public void CefString_SetToNull_ReturnsEmpty()
    {
        _cef_string_utf16_t nativeStr = default;
        var cefStr = new CefString(&nativeStr);

        cefStr.Value = "test";
        cefStr.Value = null;

        Assert.Equal(string.Empty, cefStr.Value);
    }

    // ── CefStringRef ─────────────────────────────────────────────────

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
    public void CefStringRef_AllocUserfree_RoundTrips()
    {
        string input = "alloc test";

        _cef_string_utf16_t* userfree = CefStringRef.AllocUserfree(input);
        Assert.True(userfree != null);

        string? result = CefStringRef.ToStringAndFree(userfree);
        Assert.Equal(input, result);
    }

    // ── Cef lifecycle ────────────────────────────────────────────────

    [Fact]
    public void Cef_Initially_IsNotShutdown()
    {
        Assert.False(Cef.IsShutdown);
    }

    [Fact]
    public void Cef_InitializeLibrary_ResetsShutdownFlag()
    {
        // Simulate a prior shutdown by calling Shutdown first
        // (in a test-only context, this is safe because no native CEF is loaded).
        Cef.Shutdown();
        Assert.True(Cef.IsShutdown);

        Cef.InitializeLibrary();
        Assert.False(Cef.IsShutdown);
    }

    [Fact]
    public void Cef_Shutdown_SetsFlag()
    {
        // Ensure we're in a known state
        Cef.InitializeLibrary();
        Assert.False(Cef.IsShutdown);

        Cef.Shutdown();
        Assert.True(Cef.IsShutdown);

        // Reset for other tests
        Cef.InitializeLibrary();
    }

    // ── CefStringListRef ─────────────────────────────────────────────

    [Fact]
    public void CefStringListRef_NewList_IsEmpty()
    {
        using var list = new CefStringListRef();
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void CefStringListRef_AppendAndGet_RoundTrips()
    {
        using var list = new CefStringListRef();
        list.Append("item1");
        list.Append("item2");

        Assert.Equal(2, list.Count);
        Assert.Equal("item1", list.GetValue(0));
        Assert.Equal("item2", list.GetValue(1));
    }

    [Fact]
    public void CefStringListRef_Clear_EmptiesList()
    {
        using var list = new CefStringListRef();
        list.Append("a");
        list.Append("b");
        list.Clear();

        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void CefStringListRef_DoubleDispose_IsSafe()
    {
        var list = new CefStringListRef();
        list.Dispose();
        list.Dispose(); // should not throw
    }

    // ── CefStringMapRef ──────────────────────────────────────────────

    [Fact]
    public void CefStringMapRef_NewMap_IsEmpty()
    {
        using var map = new CefStringMapRef();
        Assert.Equal(0, map.Count);
    }

    [Fact]
    public void CefStringMapRef_AppendAndFind_RoundTrips()
    {
        using var map = new CefStringMapRef();
        map.Append("key1", "value1");
        map.Append("key2", "value2");

        Assert.Equal(2, map.Count);
        Assert.Equal("value1", map.Find("key1"));
        Assert.Equal("value2", map.Find("key2"));
    }

    [Fact]
    public void CefStringMapRef_Clear_EmptiesMap()
    {
        using var map = new CefStringMapRef();
        map.Append("k", "v");
        map.Clear();

        Assert.Equal(0, map.Count);
    }

    [Fact]
    public void CefStringMapRef_DoubleDispose_IsSafe()
    {
        var map = new CefStringMapRef();
        map.Dispose();
        map.Dispose();
    }

    // ── CefStringMultimapRef ─────────────────────────────────────────

    [Fact]
    public void CefStringMultimapRef_NewMap_IsEmpty()
    {
        using var map = new CefStringMultimapRef();
        Assert.Equal(0, map.Count);
    }

    [Fact]
    public void CefStringMultimapRef_AppendAndEnumerate_RoundTrips()
    {
        using var map = new CefStringMultimapRef();
        map.Append("key1", "value1a");
        map.Append("key1", "value1b");

        Assert.Equal(2, map.Count);
        Assert.Equal(2, map.FindCount("key1"));

        Assert.Equal("value1a", map.Enumerate("key1", 0));
        Assert.Equal("value1b", map.Enumerate("key1", 1));
    }

    [Fact]
    public void CefStringMultimapRef_DoubleDispose_IsSafe()
    {
        var map = new CefStringMultimapRef();
        map.Dispose();
        map.Dispose();
    }
}
