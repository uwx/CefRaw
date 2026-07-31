using RawCef;
using RawCef.Native;

namespace RawCef.Tests;

/// <summary>
/// Integration tests that require a fully initialized CEF runtime.
/// These tests call CEF functions that allocate memory through
/// PartitionAlloc, which needs <c>cef_initialize</c> to have run first.
///
/// Uses <see cref="CefInitializationFixture"/> to set up the CEF
/// runtime once for all tests in this class.
/// </summary>
public unsafe class CefIntegrationTests : IClassFixture<CefInitializationFixture>
{
    private readonly CefInitializationFixture _fixture;

    public CefIntegrationTests(CefInitializationFixture fixture)
    {
        _fixture = fixture;
    }

    // ── CefString (allocating setter) ─────────────────────────────────

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

    // ── CefStringRef.AllocUserfree ───────────────────────────────────

    [Fact]
    public void CefStringRef_AllocUserfree_RoundTrips()
    {
        string input = "alloc test";

        _cef_string_utf16_t* userfree = CefStringRef.AllocUserfree(input);
        Assert.True(userfree != null);

        string? result = CefStringRef.ToStringAndFree(userfree);
        Assert.Equal(input, result);
    }

    // Note: Cef lifecycle tests (InitializeLibrary / Shutdown) are intentionally
    // omitted here. CEF supports only one init/shutdown per process, and calling
    // Shutdown would destroy the runtime for all subsequent tests in this class.
    // The initial-state check lives in BasicCefFunctionalityTests.

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
        list.Dispose();
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
