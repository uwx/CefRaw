namespace CefRaw.BindingsParser.Models;

/// <summary>Represents a &lt;namespace&gt; element containing structs and a class.</summary>
public class BindingsNamespace : IEquatable<BindingsNamespace>
{
    public string Name { get; set; } = "";

    /// <summary>All &lt;struct&gt; elements (including duplicates).</summary>
    public List<StructDefinition> Structs { get; set; } = [];

    /// <summary>The single &lt;class&gt; element with functions and constants.</summary>
    public ClassDefinition? Class { get; set; }

    public bool Equals(BindingsNamespace? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Structs.SequenceEqual(other.Structs) && Equals(Class, other.Class);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((BindingsNamespace)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Structs.Aggregate(0, HashCode.Combine), Class);
    }

    public static bool operator ==(BindingsNamespace? left, BindingsNamespace? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(BindingsNamespace? left, BindingsNamespace? right)
    {
        return !Equals(left, right);
    }
}
