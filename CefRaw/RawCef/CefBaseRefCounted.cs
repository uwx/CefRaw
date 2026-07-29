using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using RawCef.Native;

namespace RawCef;

/// <summary>
/// Abstract base class for client-side implementations of reference-counted CEF types.
/// Allocates an extended native struct (<c>[CEF struct | GCHandle]</c>) with
/// <c>size</c> set to the struct size. The four ref-counting function pointers
/// (<c>add_ref</c>, <c>release</c>, <c>has_one_ref</c>, <c>has_at_least_one_ref</c>)
/// are wired to static <see cref="UnmanagedCallersOnlyAttribute"/> bridge methods.
/// </summary>
public abstract unsafe class CefBaseRefCounted : ICefBaseRefCounted
{
    private _cef_base_ref_counted_t* _nativePtr;
    private GCHandle _handle;
    private int _refCount;

    /// <summary>
    /// Initializes the base by allocating native memory for the full CEF struct
    /// plus a trailing <see cref="GCHandle"/> slot.
    /// </summary>
    /// <param name="structSize">
    /// Total size of the concrete CEF struct (e.g. <c>sizeof(_cef_browser_t)</c>).
    /// Must include the base struct.
    /// </param>
    protected CefBaseRefCounted(nuint structSize)
    {
        nuint totalSize = structSize + (nuint)IntPtr.Size;
        _nativePtr = (_cef_base_ref_counted_t*)NativeMemory.AllocZeroed(totalSize);
        _nativePtr->size = structSize;

        _handle = GCHandle.Alloc(this);
        nint* handleSlot = (nint*)((byte*)_nativePtr + structSize);
        *handleSlot = GCHandle.ToIntPtr(_handle);

        _refCount = 1;
    }

    /// <inheritdoc />
    public _cef_base_ref_counted_t* NativePtr => _nativePtr;

    /// <inheritdoc />
    public nuint Size
    {
        get => _nativePtr->size;
        set => _nativePtr->size = value;
    }

    /// <inheritdoc />
    public void AddRef()
    {
        Interlocked.Increment(ref _refCount);
    }

    /// <inheritdoc />
    public int Release()
    {
        if (Interlocked.Decrement(ref _refCount) == 0)
        {
            OnRelease();
            if (_handle.IsAllocated)
                _handle.Free();
            NativeMemory.Free(_nativePtr);
            _nativePtr = null;
            return 1;
        }
        return 0;
    }

    /// <inheritdoc />
    public int HasOneRef()
    {
        return Volatile.Read(ref _refCount) == 1 ? 1 : 0;
    }

    /// <inheritdoc />
    public int HasAtLeastOneRef()
    {
        return Volatile.Read(ref _refCount) >= 1 ? 1 : 0;
    }

    /// <summary>
    /// Called when the reference count reaches zero, before native memory is freed.
    /// Override to release child references.
    /// </summary>
    protected virtual void OnRelease() { }

    /// <summary>
    /// Called during construction to assign type-specific function pointers on
    /// the native struct. The base implementation wires the four ref-counting
    /// bridge methods. Override to add your own function-pointer assignments,
    /// and be sure to call <c>base.InitializeNativeStruct()</c> first.
    /// </summary>
    protected virtual void InitializeNativeStruct()
    {
#if OS_WIN
        _nativePtr->add_ref = &BridgeAddRef;
        _nativePtr->release = &BridgeRelease;
        _nativePtr->has_one_ref = &BridgeHasOneRef;
        _nativePtr->has_at_least_one_ref = &BridgeHasAtLeastOneRef;
#elif OS_MAC || OS_LINUX
        _nativePtr->add_ref = &BridgeAddRef;
        _nativePtr->release = &BridgeRelease;
        _nativePtr->has_one_ref = &BridgeHasOneRef;
        _nativePtr->has_at_least_one_ref = &BridgeHasAtLeastOneRef;
#endif
    }

    /// <summary>
    /// Retrieves the managed object of type <typeparamref name="T"/> from a native
    /// CEF struct pointer. The GCHandle is read from the slot immediately after
    /// the struct (offset = <c>self->size</c>).
    /// </summary>
    protected static T? GetManaged<T>(void* self) where T : CefBaseRefCounted
    {
        if (self is null) return null;
        nuint structSize = ((_cef_base_ref_counted_t*)self)->size;
        nint* handleSlot = (nint*)((byte*)self + structSize);
        GCHandle handle = GCHandle.FromIntPtr(*handleSlot);
        return handle.Target as T;
    }

    // ── Ref-counting bridge methods ──────────────────────────────────

#if OS_WIN
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static void BridgeAddRef(_cef_base_ref_counted_t* self)
    {
        GetManaged<CefBaseRefCounted>(self)?.AddRef();
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static int BridgeRelease(_cef_base_ref_counted_t* self)
    {
        return GetManaged<CefBaseRefCounted>(self)?.Release() ?? 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static int BridgeHasOneRef(_cef_base_ref_counted_t* self)
    {
        return GetManaged<CefBaseRefCounted>(self)?.HasOneRef() ?? 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static int BridgeHasAtLeastOneRef(_cef_base_ref_counted_t* self)
    {
        return GetManaged<CefBaseRefCounted>(self)?.HasAtLeastOneRef() ?? 0;
    }
#elif OS_MAC || OS_LINUX
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void BridgeAddRef(_cef_base_ref_counted_t* self)
    {
        GetManaged<CefBaseRefCounted>(self)?.AddRef();
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static int BridgeRelease(_cef_base_ref_counted_t* self)
    {
        return GetManaged<CefBaseRefCounted>(self)?.Release() ?? 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static int BridgeHasOneRef(_cef_base_ref_counted_t* self)
    {
        return GetManaged<CefBaseRefCounted>(self)?.HasOneRef() ?? 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static int BridgeHasAtLeastOneRef(_cef_base_ref_counted_t* self)
    {
        return GetManaged<CefBaseRefCounted>(self)?.HasAtLeastOneRef() ?? 0;
    }
#endif
}