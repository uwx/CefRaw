namespace CefRaw.BindingsParser.Models;

/// <summary>
/// Represents the &lt;value&gt; child of an &lt;enumerator&gt;.
/// Supports plain <c>&lt;code&gt;</c>, <c>&lt;unchecked&gt;</c>-wrapped values,
/// and <c>&lt;deref&gt;</c>-wrapped values.
/// </summary>
public class EnumeratorValue : IEquatable<EnumeratorValue>
{
    /// <summary>
    /// The inner code text — the actual value expression (e.g. <c>"0"</c>, <c>"LOGSEVERITY_VERBOSE"</c>, <c>"1 &lt;&lt; 0"</c>).
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// <c>true</c> when the value was wrapped in an <c>&lt;unchecked&gt;</c> element.
    /// </summary>
    public bool IsUnchecked { get; set; }

    /// <summary>
    /// <c>true</c> when the value was wrapped in a <c>&lt;deref&gt;</c> element.
    /// </summary>
    public bool IsDeref { get; set; }

    public bool Equals(EnumeratorValue? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Code == other.Code
            && IsUnchecked == other.IsUnchecked
            && IsDeref == other.IsDeref;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((EnumeratorValue)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Code, IsUnchecked, IsDeref);
    }

    public static bool operator ==(EnumeratorValue? left, EnumeratorValue? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(EnumeratorValue? left, EnumeratorValue? right)
    {
        return !Equals(left, right);
    }
}
