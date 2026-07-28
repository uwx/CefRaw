namespace CefRaw.BindingsParser.Models;

/// <summary>Root model representing the entire &lt;bindings&gt; document.</summary>
public class BindingsRoot : IEquatable<BindingsRoot>
{
    public BindingsNamespace Namespace { get; set; } = new();

    public bool Equals(BindingsRoot? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Namespace.Equals(other.Namespace);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((BindingsRoot)obj);
    }

    public override int GetHashCode()
    {
        return Namespace.GetHashCode();
    }

    public static bool operator ==(BindingsRoot? left, BindingsRoot? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(BindingsRoot? left, BindingsRoot? right)
    {
        return !Equals(left, right);
    }
}
