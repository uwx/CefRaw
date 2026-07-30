using RawCef.Native;

namespace RawCef;

/// <summary>
/// Wraps a library-owned <see cref="_cef_base_ref_counted_t"/> pointer.
/// Calls <c>add_ref</c> on construction and <c>release</c> on dispose.
/// Implements <see cref="IDisposable"/> with a double-dispose guard.
/// </summary>
public unsafe class CefBaseRefCountedRef : ICefBaseRefCounted, IDisposable
{
    private _cef_base_ref_counted_t* _ptr;
    private int _disposed;

    /// <summary>
    /// Wraps <paramref name="ptr"/>, adding a reference via <c>add_ref</c>.
    /// </summary>
    public CefBaseRefCountedRef(_cef_base_ref_counted_t* ptr)
    {
        _ptr = ptr;
        if (_ptr is not null)
            _ptr->add_ref(_ptr);
    }

    /// <inheritdoc />
    public _cef_base_ref_counted_t* NativePtr => _ptr;

    /// <inheritdoc />
    public nuint Size
    {
        get => _ptr->size;
        set => _ptr->size = value;
    }

    /// <inheritdoc />
    public void AddRef()
    {
        if (_ptr is not null)
            _ptr->add_ref(_ptr);
    }

    /// <inheritdoc />
    public int Release()
    {
        if (_ptr is null) return 0;
        return _ptr->release(_ptr);
    }

    /// <inheritdoc />
    public int HasOneRef()
    {
        if (_ptr is null) return 0;
        return _ptr->has_one_ref(_ptr);
    }

    /// <inheritdoc />
    public int HasAtLeastOneRef()
    {
        if (_ptr is null) return 0;
        return _ptr->has_at_least_one_ref(_ptr);
    }

    /// <summary>
    /// Releases the native reference. Safe to call multiple times.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        DisposeNative();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the native reference. Only safe when CEF has not been
    /// shut down and the native pointer is still valid.
    /// </summary>
    private void DisposeNative()
    {
        if (_ptr is not null && !Cef.IsShutdown)
        {
            _ptr->release(_ptr);
            _ptr = null;
        }
    }

    /// <summary>
    /// Finalizer — calls <c>release</c> only if CEF is still alive.
    /// After <see cref="Cef.Shutdown"/>, native pointers are no longer
    /// valid and accessing them would cause an
    /// <see cref="AccessViolationException"/>.
    /// </summary>
    ~CefBaseRefCountedRef()
    {
        if (Cef.IsShutdown)
        {
            // CEF has been shut down; native pointers are invalid.
            Interlocked.Exchange(ref _disposed, 1);
            return;
        }

        DisposeNative();
    }
}
