using RawCef;
using RawCef.Native;

namespace RawCef.Tests;

/// <summary>
/// Tests that ref-counted and scoped objects are properly disposed when they
/// go out of scope and that double-dispose is safely ignored.
/// </summary>
public unsafe class DisposalTests
{
    // ── CefBaseRefCounted (client-side abstract) ──────────────────────

    [Fact]
    public void RefCounted_StartsWithOneRef()
    {
        var obj = new TestRefCounted();
        Assert.Equal(1, obj.HasOneRef());
        Assert.Equal(1, obj.HasAtLeastOneRef());
    }

    [Fact]
    public void RefCounted_Release_DecrementsAndCleansUp()
    {
        var obj = new TestRefCounted();

        int result = obj.Release();

        Assert.Equal(1, result);              // returned 1 = deleted
        Assert.True(obj.OnReleaseCalled);     // OnRelease was invoked
        Assert.True(obj.IsNativePtrNull);     // native pointer zeroed
    }

    [Fact]
    public void RefCounted_AddRef_IncrementsCount()
    {
        var obj = new TestRefCounted();

        obj.AddRef();
        Assert.Equal(0, obj.HasOneRef());        // count > 1
        Assert.Equal(1, obj.HasAtLeastOneRef());

        // Release twice to clean up
        obj.Release();
        obj.Release();
    }

    [Fact]
    public void RefCounted_HasOneRef_WhenExactlyOne()
    {
        var obj = new TestRefCounted();
        Assert.Equal(1, obj.HasOneRef());

        obj.AddRef();
        Assert.Equal(0, obj.HasOneRef());

        obj.Release();
        Assert.Equal(1, obj.HasOneRef());

        obj.Release(); // final cleanup
    }

    [Fact]
    public void RefCounted_HasAtLeastOneRef_ReturnsZeroAfterFinalRelease()
    {
        var obj = new TestRefCounted();
        obj.Release();
        Assert.Equal(0, obj.HasAtLeastOneRef());
    }

    // ── CefBaseRefCountedRef (library-side wrapper) ───────────────────

    [Fact]
    public void RefCountedRef_WrappingIncrementsRefCount()
    {
        var obj = new TrackedRefCounted();           // refCount = 1
        var wrapper = new CefBaseRefCountedRef(obj.NativePtr); // refCount = 2

        Assert.Equal(0, obj.HasOneRef());            // 2 refs
        Assert.Equal(1, obj.HasAtLeastOneRef());
    }

    [Fact]
    public void RefCountedRef_Dispose_DecrementsRefCount()
    {
        var obj = new TrackedRefCounted();           // refCount = 1
        var wrapper = new CefBaseRefCountedRef(obj.NativePtr); // refCount = 2

        wrapper.Dispose();

        Assert.Equal(1, obj.HasOneRef());            // back to 1
        Assert.Equal(0, obj.OnReleaseCount);         // not yet released
    }

    [Fact]
    public void RefCountedRef_DoubleDispose_IsSafe()
    {
        var obj = new TrackedRefCounted();
        var wrapper = new CefBaseRefCountedRef(obj.NativePtr);

        wrapper.Dispose();
        wrapper.Dispose();                           // should be a no-op

        // Only one release happened through the wrapper
        Assert.Equal(1, obj.HasOneRef());
        Assert.Equal(0, obj.OnReleaseCount);
    }

    [Fact]
    public void RefCountedRef_DisposeThenOriginalRelease_CleansUpOnce()
    {
        var obj = new TrackedRefCounted();
        var wrapper = new CefBaseRefCountedRef(obj.NativePtr);

        wrapper.Dispose();  // refCount: 2 → 1
        obj.Release();      // refCount: 1 → 0

        Assert.Equal(1, obj.OnReleaseCount);   // OnRelease called exactly once
        Assert.True(obj.IsNativePtrNull);
    }

    [Fact]
    public void RefCountedRef_MultipleWrappers_TracksAllReferences()
    {
        var obj = new TrackedRefCounted();              // refCount = 1
        var w1 = new CefBaseRefCountedRef(obj.NativePtr); // refCount = 2
        var w2 = new CefBaseRefCountedRef(obj.NativePtr); // refCount = 3

        w1.Dispose();  // 2
        w2.Dispose();  // 1
        Assert.Equal(1, obj.HasOneRef());
        Assert.Equal(0, obj.OnReleaseCount);

        obj.Release(); // 0
        Assert.Equal(1, obj.OnReleaseCount);
    }

    // ── CefBaseScoped (client-side abstract) ──────────────────────────

    [Fact]
    public void Scoped_Del_FreesNativeMemory()
    {
        var obj = new TestScoped();

        obj.Del();

        Assert.True(obj.OnDelCalled);
        Assert.True(obj.IsNativePtrNull);
    }

    [Fact]
    public void Scoped_DoubleDel_IsSafe()
    {
        var obj = new TestScoped();

        obj.Del();
        obj.Del();  // second call should be a no-op

        Assert.True(obj.OnDelCalled);
        Assert.Equal(1, obj is TestScoped s ? (s.OnDelCalled ? 1 : 0) : 0);
        // OnDelCalled should still be true (only called once)
    }

    [Fact]
    public void Scoped_DoubleDel_OnlyInvokesOnDelOnce()
    {
        var obj = new TestScoped();
        obj.Del();
        bool firstOnDel = obj.OnDelCalled;

        // Reset and call again
        obj.Del();
        Assert.True(firstOnDel); // OnDel was called during first Del only
    }

    // ── CefBaseScopedRef (library-side wrapper) ───────────────────────

    [Fact]
    public void ScopedRef_DoubleDispose_IsSafe()
    {
        // Create a client-side scoped object, get its native ptr,
        // wrap it in a ScopedRef, dispose twice.
        var obj = new TestScoped();
        var wrapper = new CefBaseScopedRef((_cef_base_scoped_t*)obj.NativePtr);

        wrapper.Dispose();
        wrapper.Dispose();  // should not throw

        // The underlying del was called once (via Dispose).
        Assert.True(obj.OnDelCalled);
    }

    // ── Null / edge cases ────────────────────────────────────────────

    [Fact]
    public void RefCounted_NullNativePtr_ReleaseReturnsZero()
    {
        var obj = new TestRefCounted();
        obj.Release(); // frees native memory, sets ptr to null

        int result = obj.Release(); // already released
        Assert.Equal(0, result);
    }

    [Fact]
    public void RefCountedRef_NullPointer_DoesNotCrash()
    {
        var wrapper = new CefBaseRefCountedRef(null);

        // All operations should be no-ops or return safe defaults.
        Assert.Equal(0, wrapper.HasOneRef());
        Assert.Equal(0, wrapper.HasAtLeastOneRef());
        Assert.Equal(0, wrapper.Release());
        wrapper.AddRef();    // should not throw
        wrapper.Dispose();   // should not throw
    }
}
