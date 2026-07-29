using RawCef.Native;

namespace RawCef;

/// <summary>
/// Interface for scoped CEF types backed by <c>_cef_base_scoped_t</c>.
/// </summary>
public unsafe interface ICefBaseScoped
{
    /// <summary>
    /// Gets the native pointer to the underlying <see cref="_cef_base_scoped_t"/>.
    /// </summary>
    _cef_base_scoped_t* NativePtr { get; }

    /// <summary>
    /// Gets or sets the <c>size</c> field of the base structure.
    /// </summary>
    nuint Size { get; set; }

    /// <summary>
    /// Calls the destructor function on the native struct, releasing all native
    /// resources held by this scoped object.
    /// </summary>
    void Del();
}
