using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using RawCef.Native;

namespace RawCef;

/// <summary>
/// Abstract base class for client-side implementations of scoped CEF types.
/// Allocates an extended native struct (<c>[CEF struct | GCHandle]</c>) and
/// wires the <c>del</c> function pointer to a static bridge method.
/// </summary>
public abstract unsafe class CefBaseScoped : ICefBaseScoped
{
    private _cef_base_scoped_t* _nativePtr;
    private GCHandle _handle;
    private int _disposed;

    /// <summary>
    /// Initializes the base by allocating native memory for the full CEF struct
    /// plus a trailing <see cref="GCHandle"/> slot.
    /// </summary>
    /// <param name="structSize">
    /// Total size of the concrete CEF struct (e.g. <c>sizeof(_cef_window_info_t)</c>).
    /// </param>
    protected CefBaseScoped(nuint structSize)
    {
        nuint totalSize = structSize + (nuint)IntPtr.Size;
        _nativePtr = (_cef_base_scoped_t*)NativeMemory.AllocZeroed(totalSize);
        _nativePtr->size = structSize;

        _handle = GCHandle.Alloc(this);
        nint* handleSlot = (nint*)((byte*)_nativePtr + structSize);
        *handleSlot = GCHandle.ToIntPtr(_handle);
    }

    /// <inheritdoc />
    public _cef_base_scoped_t* NativePtr => _nativePtr;

    /// <inheritdoc />
    public nuint Size
    {
        get => _nativePtr->size;
        set => _nativePtr->size = value;
    }

    /// <summary>
    /// Calls the destructor, freeing the native memory and GCHandle.
    /// </summary>
    public virtual void Del()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        OnDel();
        if (_handle.IsAllocated)
            _handle.Free();
        NativeMemory.Free(_nativePtr);
        _nativePtr = null;
    }

    /// <summary>
    /// Called before the native memory is freed. Override to release resources.
    /// </summary>
    protected virtual void OnDel() { }

    /// <summary>
    /// Called during construction to assign type-specific function pointers on
    /// the native struct. The base implementation wires the <c>del</c> bridge
    /// method. Override to add your own function-pointer assignments, and be sure
    /// to call <c>base.InitializeNativeStruct()</c> first.
    /// </summary>
    protected virtual void InitializeNativeStruct()
    {
#if OS_WIN
        _nativePtr->del = &BridgeDel;
#elif OS_MAC || OS_LINUX
        _nativePtr->del = &BridgeDel;
#endif
    }

    /// <summary>
    /// Retrieves the managed object of type <typeparamref name="T"/> from a native
    /// CEF struct pointer. The GCHandle is read from the slot immediately after
    /// the struct (offset = <c>self->size</c>).
    /// </summary>
    protected static T? GetManaged<T>(void* self) where T : CefBaseScoped
    {
        if (self is null) return null;
        nuint structSize = ((_cef_base_scoped_t*)self)->size;
        nint* handleSlot = (nint*)((byte*)self + structSize);
        GCHandle handle = GCHandle.FromIntPtr(*handleSlot);
        return handle.Target as T;
    }

    // ── Bridge methods ────────────────────────────────────────────────

#if OS_WIN
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
#else
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
#endif
    private static void BridgeDel(_cef_base_scoped_t* self)
    {
        GetManaged<CefBaseScoped>(self)?.Del();
    }
}
