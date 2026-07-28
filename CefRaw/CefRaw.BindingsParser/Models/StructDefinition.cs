namespace CefRaw.BindingsParser.Models;

/// <summary>Represents a &lt;struct&gt; element.</summary>
public class StructDefinition : IEquatable<StructDefinition>
{
    public string Name { get; set; } = "";
    public string Access { get; set; } = "public";
    public bool Unsafe { get; set; }

    /// <summary>All &lt;field&gt; child elements.</summary>
    public List<FieldDefinition> Fields { get; set; } = [];

    public bool Equals(StructDefinition? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Access == other.Access && Unsafe == other.Unsafe && Fields.SequenceEqual(other.Fields);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((StructDefinition)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Access, Unsafe, Fields.Aggregate(0, HashCode.Combine));
    }

    public static bool operator ==(StructDefinition? left, StructDefinition? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(StructDefinition? left, StructDefinition? right)
    {
        return !Equals(left, right);
    }
}
