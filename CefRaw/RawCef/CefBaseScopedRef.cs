using RawCef.Native;

namespace RawCef;

/// <summary>
/// Wraps a library-owned <see cref="_cef_base_scoped_t"/> pointer.
/// Calls <c>del</c> on dispose if the function pointer is non-null.
/// Implements <see cref="IDisposable"/> with a double-dispose guard.
/// </summary>
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
    /// Safe to call multiple times.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        if (_ptr is not null)
        {
            if (_ptr->del is not null)
                _ptr->del(_ptr);
            _ptr = null;
        }

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizer as safety net.
    /// </summary>
    ~CefBaseScopedRef()
    {
        Dispose();
    }
}
