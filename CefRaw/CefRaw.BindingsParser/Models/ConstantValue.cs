namespace CefRaw.BindingsParser.Models;

/// <summary>
/// Represents the &lt;value&gt; child of a &lt;constant&gt;.
/// Supports both plain <c>&lt;code&gt;</c> and <c>&lt;deref&gt;&lt;code&gt;</c> patterns.
/// </summary>
public class ConstantValue : IEquatable<ConstantValue>
{
    /// <summary>
    /// The inner text of <c>&lt;code&gt;</c> — the actual value expression.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// <c>true</c> when the value was wrapped in a <c>&lt;deref&gt;</c> element.
    /// </summary>
    public bool IsDeref { get; set; }

    public bool Equals(ConstantValue? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Code == other.Code && IsDeref == other.IsDeref;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ConstantValue)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Code, IsDeref);
    }

    public static bool operator ==(ConstantValue? left, ConstantValue? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ConstantValue? left, ConstantValue? right)
    {
        return !Equals(left, right);
    }
}
