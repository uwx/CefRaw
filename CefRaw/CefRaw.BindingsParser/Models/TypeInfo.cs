namespace CefRaw.BindingsParser.Models;

/// <summary>
/// Models a &lt;type&gt; element. Holds the native C type (attribute) and the
/// mapped C# type (inner text).
/// </summary>
public class TypeInfo : IEquatable<TypeInfo>
{
    /// <summary>The native C type from the <c>native</c> attribute, e.g. <c>"wchar_t *"</c>.</summary>
    public string? Native { get; set; }

    /// <summary>The inner text — the C# mapped type, e.g. <c>"ushort*"</c>, <c>"nuint"</c>.</summary>
    public string CSharpType { get; set; } = "";

    public bool Equals(TypeInfo? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Native == other.Native && CSharpType == other.CSharpType;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((TypeInfo)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Native, CSharpType);
    }

    public static bool operator ==(TypeInfo? left, TypeInfo? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(TypeInfo? left, TypeInfo? right)
    {
        return !Equals(left, right);
    }
}
