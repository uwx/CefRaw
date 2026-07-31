namespace RawCef.BindingsParser.Models;

/// <summary>Represents a &lt;constant&gt; inside a &lt;class&gt;.</summary>
public class ConstantDefinition : IEquatable<ConstantDefinition>
{
    public string Name { get; set; } = "";
    public string Access { get; set; } = "public";

    /// <summary>
    /// The &lt;type&gt; child text content, e.g. <c>"delegate*&lt;...&gt;"</c>.
    /// </summary>
    public string Type { get; set; } = "";

    /// <summary>
    /// Whether the <c>primitive</c> attribute is <c>"True"</c>.
    /// </summary>
    public bool IsPrimitive { get; set; }

    /// <summary>The parsed &lt;value&gt; child.</summary>
    public ConstantValue? Value { get; set; }

    public bool Equals(ConstantDefinition? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Access == other.Access && Type == other.Type && IsPrimitive == other.IsPrimitive && Equals(Value, other.Value);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ConstantDefinition)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Access, Type, IsPrimitive, Value);
    }

    public static bool operator ==(ConstantDefinition? left, ConstantDefinition? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ConstantDefinition? left, ConstantDefinition? right)
    {
        return !Equals(left, right);
    }
}
