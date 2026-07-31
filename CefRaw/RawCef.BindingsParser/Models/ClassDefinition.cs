namespace RawCef.BindingsParser.Models;

/// <summary>Represents the single &lt;class&gt; element inside &lt;namespace&gt;.</summary>
public class ClassDefinition : IEquatable<ClassDefinition>
{
    public string Name { get; set; } = "";
    public string Access { get; set; } = "public";
    public bool Static { get; set; }
    public bool Unsafe { get; set; }

    /// <summary>All &lt;function&gt; child elements.</summary>
    public List<FunctionDefinition> Functions { get; set; } = [];

    /// <summary>All &lt;constant&gt; child elements.</summary>
    public List<ConstantDefinition> Constants { get; set; } = [];

    public bool Equals(ClassDefinition? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Access == other.Access && Static == other.Static && Unsafe == other.Unsafe && Functions.SequenceEqual(other.Functions) && Constants.SequenceEqual(other.Constants);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ClassDefinition)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Access, Static, Unsafe, Functions.Aggregate(0, HashCode.Combine), Constants.Aggregate(0, HashCode.Combine));
    }

    public static bool operator ==(ClassDefinition? left, ClassDefinition? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ClassDefinition? left, ClassDefinition? right)
    {
        return !Equals(left, right);
    }
}
