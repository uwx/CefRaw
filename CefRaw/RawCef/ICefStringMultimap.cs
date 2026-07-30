using RawCef.Native;

namespace RawCef;

/// <summary>
/// Methods for <see cref="ICefStringMultimap"/> that wrap the CEF string multimap API.
/// A multimap allows multiple values per key.
/// </summary>
public unsafe partial interface ICefStringMultimap
{
    /// <summary>
    /// Gets the total number of key/value pairs in the multimap.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Returns the number of values associated with the specified <paramref name="key"/>.
    /// </summary>
    int FindCount(string key);

    /// <summary>
    /// Returns the value at <paramref name="valueIndex"/> for the specified <paramref name="key"/>.
    /// </summary>
    string? Enumerate(string key, int valueIndex);

    /// <summary>
    /// Gets the key at the specified <paramref name="index"/>.
    /// </summary>
    string? GetKey(int index);

    /// <summary>
    /// Gets the value at the specified <paramref name="index"/>.
    /// </summary>
    string? GetValue(int index);

    /// <summary>
    /// Appends a key/value pair to the multimap.
    /// </summary>
    void Append(string key, string value);

    /// <summary>
    /// Removes all key/value pairs from the multimap.
    /// </summary>
    void Clear();
}
