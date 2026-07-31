namespace RawCef.BindingsParser.Models;

/// <summary>Represents an &lt;enumerator&gt; element inside an &lt;enumeration&gt;.</summary>
public class EnumeratorDefinition : IEquatable<EnumeratorDefinition>
{
    public string Name { get; set; } = "";
    public string Access { get; set; } = "public";

    /// <summary>The type for this enumerator (e.g. <c>int</c>).</summary>
    public string Type { get; set; } = "";

    /// <summary><c>true</c> when the <c>primitive</c> attribute on the <c>&lt;type&gt;</c> is <c>"True"</c>.</summary>
    public bool IsPrimitive { get; set; }

    /// <summary>Optional explicit value expression.</summary>
    public EnumeratorValue? Value { get; set; }

    public bool Equals(EnumeratorDefinition? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name
            && Access == other.Access
            && Type == other.Type
            && IsPrimitive == other.IsPrimitive
            && Equals(Value, other.Value);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((EnumeratorDefinition)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Access, Type, IsPrimitive, Value);
    }

    public static bool operator ==(EnumeratorDefinition? left, EnumeratorDefinition? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(EnumeratorDefinition? left, EnumeratorDefinition? right)
    {
        return !Equals(left, right);
    }
}
