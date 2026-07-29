using RawCef.Native;

namespace RawCef;

/// <summary>
/// Methods for <see cref="ICefStringList"/> that wrap the CEF string list API.
/// </summary>
public unsafe partial interface ICefStringList
{
    /// <summary>
    /// Gets the number of strings in the list.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the string value at the specified <paramref name="index"/>.
    /// </summary>
    string? GetValue(int index);

    /// <summary>
    /// Appends a string to the end of the list.
    /// </summary>
    void Append(string? value);

    /// <summary>
    /// Removes all strings from the list.
    /// </summary>
    void Clear();

    /// <summary>
    /// Creates a copy of this list. The returned list must be disposed separately.
    /// </summary>
    ICefStringList Copy();
}
