namespace RawCef.BindingsParser.Models;

/// <summary>Represents a &lt;field&gt; inside a &lt;struct&gt;.</summary>
public class FieldDefinition : IEquatable<FieldDefinition>
{
    public string Name { get; set; } = "";
    public string Access { get; set; } = "public";

    /// <summary>The &lt;type&gt; child — see <see cref="TypeInfo"/>.</summary>
    public TypeInfo Type { get; set; } = new();

    public bool Equals(FieldDefinition? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Access == other.Access && Type.Equals(other.Type);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((FieldDefinition)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Access, Type);
    }

    public static bool operator ==(FieldDefinition? left, FieldDefinition? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(FieldDefinition? left, FieldDefinition? right)
    {
        return !Equals(left, right);
    }
}
