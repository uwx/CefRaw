using RawCef;
using RawCef.Native;

namespace RawCef.Tests;

/// <summary>
/// Tests that the safe binding wrappers correctly handle edge cases
/// such as null pointers, post-dispose access, and cross-type reference
/// tracking.
/// </summary>
public unsafe class SafeBindingsTests
{
    // ── CefBaseRefCountedRef safety ──────────────────────────────────

    [Fact]
    public void RefCountedRef_AfterDispose_HasOneRefReturnsZero()
    {
        var obj = new TrackedRefCounted();
        var wrapper = new CefBaseRefCountedRef(obj.NativePtr);
        wrapper.Dispose();

        // After dispose, the internal _ptr is nulled; methods return safe defaults.
        Assert.Equal(0, wrapper.HasOneRef());
    }

    [Fact]
    public void RefCountedRef_AfterDispose_HasAtLeastOneRefReturnsZero()
    {
        var obj = new TrackedRefCounted();
        var wrapper = new CefBaseRefCountedRef(obj.NativePtr);
        wrapper.Dispose();

        Assert.Equal(0, wrapper.HasAtLeastOneRef());
    }

    [Fact]
    public void RefCountedRef_AfterDispose_ReleaseReturnsZero()
    {
        var obj = new TrackedRefCounted();
        var wrapper = new CefBaseRefCountedRef(obj.NativePtr);
        wrapper.Dispose();

        int result = wrapper.Release();
        Assert.Equal(0, result);
    }

    [Fact]
    public void RefCountedRef_AfterDispose_AddRefDoesNotThrow()
    {
        var obj = new TrackedRefCounted();
        var wrapper = new CefBaseRefCountedRef(obj.NativePtr);
        wrapper.Dispose();

        // Should not throw — null check inside AddRef.
        wrapper.AddRef();
    }

    // Note: RefCountedRef_DisposeAfterOriginalReleased is intentionally omitted.
    // The Ref wrapper stores a copy of the native pointer at construction time.
    // If the original object is released (freeing native memory), the wrapper's
    // _ptr becomes a dangling pointer — the null check passes because the pointer
    // value is non-null, but dereferencing it causes an AccessViolation.
    // This is by design: wrappers must be disposed BEFORE the original object.

    // ── GCHandle round-trip ──────────────────────────────────────────

    [Fact]
    public void RefCounted_BridgeMethods_RoundTripManagedObject()
    {
        // Verify that BridgeAddRef/BridgeRelease correctly find the
        // managed object through the GCHandle stored after the struct.
        var obj = new TestRefCounted();
        _cef_base_ref_counted_t* ptr = obj.NativePtr;

        // Simulate what native code does: call add_ref through function pointer.
        ptr->add_ref(ptr);
        Assert.Equal(0, obj.HasOneRef()); // refCount should be 2 now

        ptr->release(ptr);
        Assert.Equal(1, obj.HasOneRef()); // back to 1

        ptr->release(ptr); // final release
        Assert.True(obj.OnReleaseCalled);
    }

    [Fact]
    public void RefCounted_BridgeHasOneRef_ReturnsCorrectValue()
    {
        var obj = new TestRefCounted();
        _cef_base_ref_counted_t* ptr = obj.NativePtr;

        Assert.Equal(1, ptr->has_one_ref(ptr));

        ptr->add_ref(ptr);
        Assert.Equal(0, ptr->has_one_ref(ptr));

        ptr->release(ptr);
        ptr->release(ptr);
    }

    [Fact]
    public void RefCounted_BridgeHasAtLeastOneRef_ReturnsCorrectValue()
    {
        var obj = new TestRefCounted();
        _cef_base_ref_counted_t* ptr = obj.NativePtr;

        // Ref count = 1 → at least one ref = 1
        Assert.Equal(1, ptr->has_at_least_one_ref(ptr));

        ptr->add_ref(ptr);
        // Ref count = 2 → at least one ref = 1
        Assert.Equal(1, ptr->has_at_least_one_ref(ptr));

        ptr->release(ptr);
        // Ref count = 1 → at least one ref = 1
        Assert.Equal(1, ptr->has_at_least_one_ref(ptr));

        ptr->release(ptr);
        // Ref count = 0 → native memory freed; no more bridge calls.
        // The zero-ref case is tested via the managed object in
        // DisposalTests.RefCounted_HasAtLeastOneRef_ReturnsZeroAfterFinalRelease.
    }

    // ── CefBaseScopedRef safety ──────────────────────────────────────

    [Fact]
    public void ScopedRef_Del_DelegatesToBridge()
    {
        var obj = new TestScoped();
        _cef_base_scoped_t* ptr = (_cef_base_scoped_t*)obj.NativePtr;

        // Call del through the function pointer (simulates CEF calling it).
        ptr->del(ptr);

        Assert.True(obj.OnDelCalled);
    }

    [Fact]
    public void ScopedRef_AfterDispose_DelDoesNotThrow()
    {
        var obj = new TestScoped();
        var wrapper = new CefBaseScopedRef((_cef_base_scoped_t*)obj.NativePtr);

        wrapper.Dispose();
        wrapper.Del(); // after dispose, _ptr is null, Del is a no-op
    }

    // ── Interface contract ───────────────────────────────────────────

    [Fact]
    public void RefCountedRef_ImplementsICefBaseRefCounted()
    {
        var obj = new TrackedRefCounted();
        var wrapper = new CefBaseRefCountedRef(obj.NativePtr);

        ICefBaseRefCounted iface = wrapper;
        Assert.NotNull(iface);
        Assert.NotEqual(IntPtr.Zero, (nint)iface.NativePtr);
    }

    [Fact]
    public void ScopedRef_ImplementsICefBaseScoped()
    {
        var obj = new TestScoped();
        var wrapper = new CefBaseScopedRef((_cef_base_scoped_t*)obj.NativePtr);

        ICefBaseScoped iface = wrapper;
        Assert.NotNull(iface);
    }

    // ── Concrete subclass allocation ─────────────────────────────────

    [Fact]
    public void RefCounted_NativeMemory_IsAllocatedWithCorrectSize()
    {
        var obj = new TestRefCounted();
        _cef_base_ref_counted_t* ptr = obj.NativePtr;

        nuint expectedSize = (nuint)sizeof(_cef_base_ref_counted_t);
        Assert.NotEqual((nuint)0, ptr->size);
        Assert.Equal(expectedSize, ptr->size);
    }

    [Fact]
    public void Scoped_NativeMemory_IsAllocatedWithCorrectSize()
    {
        var obj = new TestScoped();
        _cef_base_scoped_t* ptr = (_cef_base_scoped_t*)obj.NativePtr;

        Assert.NotEqual((nuint)0, ptr->size);
        Assert.Equal((nuint)sizeof(_cef_base_scoped_t), ptr->size);
    }

    // ── Size property ────────────────────────────────────────────────

    [Fact]
    public void RefCounted_Size_CanBeReadAndWritten()
    {
        var obj = new TestRefCounted();
        nuint original = obj.Size;

        obj.Size = 12345;
        Assert.Equal((nuint)12345, obj.Size);

        obj.Size = original; // restore
        obj.Release();
    }

    [Fact]
    public void Scoped_Size_CanBeReadAndWritten()
    {
        var obj = new TestScoped();
        nuint original = obj.Size;

        obj.Size = 9999;
        Assert.Equal((nuint)9999, obj.Size);

        obj.Size = original;
        obj.Del();
    }
}
