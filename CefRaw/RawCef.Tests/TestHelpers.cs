using RawCef;
using RawCef.Native;

namespace RawCef.Tests;

// ── Concrete test implementations ────────────────────────────────────

/// <summary>
/// Minimal concrete <see cref="CefBaseRefCounted"/> for testing ref-counting,
/// disposal, and bridge-method dispatch.
/// </summary>
internal sealed unsafe class TestRefCounted : CefBaseRefCounted
{
    public bool OnReleaseCalled { get; private set; }
    public bool IsNativePtrNull => NativePtr == null;

    public TestRefCounted() : base((nuint)sizeof(_cef_base_ref_counted_t))
    {
        InitializeNativeStruct();
    }

    protected override void OnRelease()
    {
        OnReleaseCalled = true;
    }

    /// <summary>Exposes the internal ref count for assertions.</summary>
    public int CurrentRefCount
    {
        get
        {
            // We can observe ref count indirectly via HasOneRef / HasAtLeastOneRef
            // but for precise testing we release and check.
            return HasOneRef() == 1 ? 1 : HasAtLeastOneRef() == 1 ? 2 : 0;
        }
    }
}

/// <summary>
/// Minimal concrete <see cref="CefBaseScoped"/> for testing scoped-object
/// deletion and double-del guards.
/// </summary>
internal sealed unsafe class TestScoped : CefBaseScoped
{
    public bool OnDelCalled { get; private set; }
    public bool IsNativePtrNull => NativePtr == null;

    public TestScoped() : base((nuint)sizeof(_cef_base_scoped_t))
    {
        InitializeNativeStruct();
    }

    protected override void OnDel()
    {
        OnDelCalled = true;
    }
}

/// <summary>
/// A ref-counted type that tracks how many times <see cref="OnRelease"/> fires.
/// Used to detect double-dispose bugs.
/// </summary>
internal sealed unsafe class TrackedRefCounted : CefBaseRefCounted
{
    public int OnReleaseCount { get; private set; }
    public bool IsNativePtrNull => NativePtr == null;

    public TrackedRefCounted() : base((nuint)sizeof(_cef_base_ref_counted_t))
    {
        InitializeNativeStruct();
    }

    protected override void OnRelease()
    {
        OnReleaseCount++;
    }
}
