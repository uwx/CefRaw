namespace CefRaw.BindingsParser.Models;

/// <summary>Represents a &lt;param&gt; inside a &lt;function&gt;.</summary>
public class ParamDefinition : IEquatable<ParamDefinition>
{
    public string Name { get; set; } = "";

    /// <summary>The &lt;type&gt; child text content, e.g. <c>"ushort*"</c>.</summary>
    public string Type { get; set; } = "";

    public bool Equals(ParamDefinition? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Type == other.Type;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ParamDefinition)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Type);
    }

    public static bool operator ==(ParamDefinition? left, ParamDefinition? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ParamDefinition? left, ParamDefinition? right)
    {
        return !Equals(left, right);
    }
}
