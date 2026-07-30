using RawCef.Native;

namespace RawCef;

/// <summary>
/// Wraps a library-owned <see cref="_cef_base_scoped_t"/> pointer.
/// Calls <c>del</c> on dispose if the function pointer is non-null.
/// Implements <see cref="IDisposable"/> with a double-dispose guard.
/// </summary>
/// <remarks>
/// Library-owned scoped objects are only valid during their callback.
/// Their native memory may be freed (stack unwind, thread exit, or
/// CEF shutdown) after the callback returns. The finalizer therefore
/// must NOT touch native memory — doing so would dereference a
/// dangling pointer and cause an <see cref="AccessViolationException"/>.
/// Explicit <see cref="Dispose"/> is safe within the callback scope.
/// </remarks>
public unsafe class CefBaseScopedRef : ICefBaseScoped, IDisposable
{
    private _cef_base_scoped_t* _ptr;
    private int _disposed;

    /// <summary>
    /// Wraps <paramref name="ptr"/> without taking ownership.
    /// </summary>
    public CefBaseScopedRef(_cef_base_scoped_t* ptr)
    {
        _ptr = ptr;
    }

    /// <inheritdoc />
    public _cef_base_scoped_t* NativePtr => _ptr;

    /// <inheritdoc />
    public nuint Size
    {
        get => _ptr->size;
        set => _ptr->size = value;
    }

    /// <inheritdoc />
    public void Del()
    {
        if (_ptr is not null && _ptr->del is not null)
            _ptr->del(_ptr);
    }

    /// <summary>
    /// Calls <c>del</c> on the native struct if the pointer is non-null.
    /// Safe to call multiple times. Should be called within the callback
    /// scope while the native pointer is still valid.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        DisposeNative();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the native resources. Only safe when the native pointer
    /// is still valid (i.e., during the callback, not from a finalizer).
    /// </summary>
    private void DisposeNative()
    {
        if (_ptr is not null && !Cef.IsShutdown)
        {
            if (_ptr->del is not null)
                _ptr->del(_ptr);
            _ptr = null;
        }
    }

    /// <summary>
    /// Finalizer — does NOT call native <c>del</c>. Library-owned scoped
    /// objects are only valid during their callback; the native memory
    /// may already be freed by CEF. Accessing <c>_ptr</c> here would
    /// likely cause an <see cref="AccessViolationException"/>.
    /// </summary>
    ~CefBaseScopedRef()
    {
        // Mark disposed so that double-dispose guards work, but do NOT
        // touch native memory — the pointer is almost certainly dangling.
        Interlocked.Exchange(ref _disposed, 1);
    }
}
