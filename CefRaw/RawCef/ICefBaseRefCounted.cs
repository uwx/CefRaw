using RawCef.Native;

namespace RawCef;

/// <summary>
/// Interface for reference-counted CEF types backed by <c>_cef_base_ref_counted_t</c>.
/// </summary>
public unsafe interface ICefBaseRefCounted
{
    /// <summary>
    /// Gets the native pointer to the underlying <see cref="_cef_base_ref_counted_t"/>.
    /// </summary>
    _cef_base_ref_counted_t* NativePtr { get; }

    /// <summary>
    /// Gets or sets the <c>size</c> field of the base structure.
    /// </summary>
    nuint Size { get; set; }

    /// <summary>
    /// Increments the reference count.
    /// </summary>
    void AddRef();

    /// <summary>
    /// Decrements the reference count. Returns 1 if the object was deleted, 0 otherwise.
    /// </summary>
    int Release();

    /// <summary>
    /// Returns 1 if the reference count is exactly 1.
    /// </summary>
    int HasOneRef();

    /// <summary>
    /// Returns 1 if the reference count is at least 1.
    /// </summary>
    int HasAtLeastOneRef();
}
