namespace RawCef.BindingsParser.Models;

/// <summary>Represents an &lt;enumeration&gt; element.</summary>
public class EnumerationDefinition : IEquatable<EnumerationDefinition>
{
    public string Name { get; set; } = "";
    public string Access { get; set; } = "public";

    /// <summary>The underlying type of the enumeration (e.g. <c>int</c>).</summary>
    public string Type { get; set; } = "";

    /// <summary>All &lt;enumerator&gt; child elements.</summary>
    public List<EnumeratorDefinition> Enumerators { get; set; } = [];

    public bool Equals(EnumerationDefinition? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name
            && Access == other.Access
            && Type == other.Type
            && Enumerators.SequenceEqual(other.Enumerators);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((EnumerationDefinition)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Access, Type, Enumerators.Aggregate(0, HashCode.Combine));
    }

    public static bool operator ==(EnumerationDefinition? left, EnumerationDefinition? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(EnumerationDefinition? left, EnumerationDefinition? right)
    {
        return !Equals(left, right);
    }
}
