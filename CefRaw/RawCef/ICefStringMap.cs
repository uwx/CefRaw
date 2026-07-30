using RawCef.Native;

namespace RawCef;

/// <summary>
/// Methods for <see cref="ICefStringMap"/> that wrap the CEF string map API.
/// </summary>
public unsafe partial interface ICefStringMap
{
    /// <summary>
    /// Gets the number of key/value pairs in the map.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Finds the value associated with the specified <paramref name="key"/>.
    /// Returns <c>null</c> if the key is not found.
    /// </summary>
    string? Find(string key);

    /// <summary>
    /// Gets the key at the specified <paramref name="index"/>.
    /// </summary>
    string? GetKey(int index);

    /// <summary>
    /// Gets the value at the specified <paramref name="index"/>.
    /// </summary>
    string? GetValue(int index);

    /// <summary>
    /// Appends a key/value pair to the map. If the key already exists,
    /// the existing value is replaced.
    /// </summary>
    void Append(string key, string value);

    /// <summary>
    /// Removes all key/value pairs from the map.
    /// </summary>
    void Clear();
}
