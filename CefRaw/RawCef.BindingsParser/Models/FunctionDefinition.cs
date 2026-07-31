namespace RawCef.BindingsParser.Models;

/// <summary>Represents a &lt;function&gt; inside a &lt;class&gt;.</summary>
public class FunctionDefinition : IEquatable<FunctionDefinition>
{
    public string Name { get; set; } = "";
    public string Access { get; set; } = "public";
    public string? Lib { get; set; }
    public string? Convention { get; set; }
    public bool Static { get; set; }
    public bool Unsafe { get; set; }

    /// <summary>The return type — first &lt;type&gt; child (text content).</summary>
    public string ReturnType { get; set; } = "";

    /// <summary>All &lt;param&gt; children.</summary>
    public List<ParamDefinition> Parameters { get; set; } = [];


    public bool Equals(FunctionDefinition? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Access == other.Access && Lib == other.Lib && Convention == other.Convention && Static == other.Static && Unsafe == other.Unsafe && ReturnType == other.ReturnType && Parameters.SequenceEqual(other.Parameters);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((FunctionDefinition)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Access, Lib, Convention, Static, Unsafe, ReturnType, Parameters.Aggregate(0, HashCode.Combine));
    }

    public static bool operator ==(FunctionDefinition? left, FunctionDefinition? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(FunctionDefinition? left, FunctionDefinition? right)
    {
        return !Equals(left, right);
    }
}
