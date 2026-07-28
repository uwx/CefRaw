using System.Xml.Linq;
using CefRaw.BindingsParser.Models;

namespace CefRaw.BindingsParser;

/// <summary>
/// Parses a CEF bindings XML file (e.g. <c>bindings_win.xml</c>) into
/// a strongly-typed <see cref="BindingsRoot"/> POCO graph.
/// </summary>
public static class BindingsParser
{
    /// <summary>
    /// Parse the XML document at the given file path.
    /// </summary>
    public static BindingsRoot ParseFile(string filePath)
    {
        var doc = XDocument.Load(filePath);
        return ParseDocument(doc);
    }

    /// <summary>
    /// Parse XML from a <see cref="Stream"/>.
    /// </summary>
    public static BindingsRoot ParseStream(Stream stream)
    {
        var doc = XDocument.Load(stream);
        return ParseDocument(doc);
    }

    /// <summary>
    /// Parse XML from a string.
    /// </summary>
    public static BindingsRoot ParseString(string xml)
    {
        var doc = XDocument.Parse(xml);
        return ParseDocument(doc);
    }

    private static BindingsRoot ParseDocument(XDocument doc)
    {
        var root = doc.Root
            ?? throw new InvalidOperationException("XML document has no root element.");

        if (root.Name.LocalName != "bindings")
            throw new InvalidOperationException(
                $"Expected root element <bindings>, got <{root.Name.LocalName}>.");

        var nsEl = root.Elements("namespace").FirstOrDefault()
            ?? throw new InvalidOperationException("No <namespace> element found under <bindings>.");

        return new BindingsRoot
        {
            Namespace = ParseNamespace(nsEl)
        };
    }

    // ── Namespace ──────────────────────────────────────────────────────

    private static BindingsNamespace ParseNamespace(XElement el)
    {
        var ns = new BindingsNamespace
        {
            Name = Attr(el, "name") ?? ""
        };

        foreach (var child in el.Elements())
        {
            switch (child.Name.LocalName)
            {
                case "struct":
                    ns.Structs.Add(ParseStruct(child));
                    break;
                case "enumeration":
                    ns.Enumerations.Add(ParseEnumeration(child));
                    break;
                case "class":
                    ns.Class = ParseClass(child);
                    break;
            }
        }

        return ns;
    }

    // ── Struct ─────────────────────────────────────────────────────────

    private static StructDefinition ParseStruct(XElement el)
    {
        var s = new StructDefinition
        {
            Name   = Attr(el, "name") ?? "",
            Access = Attr(el, "access") ?? "public",
            Unsafe = BoolAttr(el, "unsafe")
        };

        foreach (var fieldEl in el.Elements("field"))
            s.Fields.Add(ParseField(fieldEl));

        return s;
    }

    // ── Enumeration ────────────────────────────────────────────────────

    private static EnumerationDefinition ParseEnumeration(XElement el)
    {
        var typeEl = el.Element("type");

        var e = new EnumerationDefinition
        {
            Name   = Attr(el, "name") ?? "",
            Access = Attr(el, "access") ?? "public",
            Type   = typeEl?.Value ?? ""
        };

        foreach (var enumeratorEl in el.Elements("enumerator"))
            e.Enumerators.Add(ParseEnumerator(enumeratorEl));

        return e;
    }

    // ── Enumerator ─────────────────────────────────────────────────────

    private static EnumeratorDefinition ParseEnumerator(XElement el)
    {
        var typeEl  = el.Element("type");
        var valueEl = el.Element("value");

        return new EnumeratorDefinition
        {
            Name        = Attr(el, "name") ?? "",
            Access      = Attr(el, "access") ?? "public",
            Type        = typeEl?.Value ?? "",
            IsPrimitive = BoolAttr(typeEl, "primitive"),
            Value       = ParseEnumeratorValue(valueEl)
        };
    }

    // ── Enumerator Value ───────────────────────────────────────────────

    private static EnumeratorValue? ParseEnumeratorValue(XElement? el)
    {
        if (el is null)
            return null;

        // Pattern: <unchecked><value><cast>...</cast><value><code>...</code></value></value></unchecked>
        var uncheckedEl = el.Element("unchecked");
        if (uncheckedEl is not null)
            return ParseEnumeratorValueInner(uncheckedEl, isUnchecked: true);

        // Pattern: <deref><code>...</code></deref>
        var derefEl = el.Element("deref");
        if (derefEl is not null)
        {
            return new EnumeratorValue
            {
                Code        = derefEl.Element("code")?.Value,
                IsDeref     = true,
                IsUnchecked = false
            };
        }

        // Plain <code> inside <value>
        return new EnumeratorValue
        {
            Code        = el.Element("code")?.Value,
            IsDeref     = false,
            IsUnchecked = false
        };
    }

    /// <summary>
    /// Recursively drills into nested <c>&lt;value&gt;</c> / <c>&lt;unchecked&gt;</c> wrappers
    /// to find the innermost <c>&lt;code&gt;</c> element.
    /// </summary>
    private static EnumeratorValue? ParseEnumeratorValueInner(XElement el, bool isUnchecked)
    {
        // Drill through <value> elements until we find <code>
        var codeEl = el.Element("code");
        if (codeEl is not null)
        {
            return new EnumeratorValue
            {
                Code        = codeEl.Value,
                IsUnchecked = isUnchecked,
                IsDeref     = false
            };
        }

        // Handle nested <value> inside <unchecked> (e.g. <value><cast>...</cast><value><code>...</code></value></value>)
        var innerValue = el.Element("value");
        if (innerValue is not null)
        {
            // Check for <unchecked> inside the inner value
            var innerUnchecked = innerValue.Element("unchecked");
            if (innerUnchecked is not null)
                return ParseEnumeratorValueInner(innerUnchecked, isUnchecked: true);

            // Look for <code> inside the inner value
            var innerCode = innerValue.Element("code");
            if (innerCode is not null)
            {
                return new EnumeratorValue
                {
                    Code        = innerCode.Value,
                    IsUnchecked = isUnchecked,
                    IsDeref     = false
                };
            }

            // Drill deeper: <value> inside <value>
            return ParseEnumeratorValueInner(innerValue, isUnchecked);
        }

        return null;
    }

    // ── Field ──────────────────────────────────────────────────────────

    private static FieldDefinition ParseField(XElement el)
    {
        var typeEl = el.Element("type");

        return new FieldDefinition
        {
            Name   = Attr(el, "name") ?? "",
            Access = Attr(el, "access") ?? "public",
            Type   = ParseTypeInfo(typeEl)
        };
    }

    // ── TypeInfo ───────────────────────────────────────────────────────

    private static TypeInfo ParseTypeInfo(XElement? el)
    {
        if (el is null)
            return new TypeInfo();

        return new TypeInfo
        {
            Native     = Attr(el, "native"),
            CSharpType = el.Value
        };
    }

    // ── Class ──────────────────────────────────────────────────────────

    private static ClassDefinition ParseClass(XElement el)
    {
        var c = new ClassDefinition
        {
            Name   = Attr(el, "name") ?? "",
            Access = Attr(el, "access") ?? "public",
            Static = BoolAttr(el, "static"),
            Unsafe = BoolAttr(el, "unsafe")
        };

        foreach (var child in el.Elements())
        {
            switch (child.Name.LocalName)
            {
                case "function":
                    c.Functions.Add(ParseFunction(child));
                    break;
                case "constant":
                    c.Constants.Add(ParseConstant(child));
                    break;
            }
        }

        return c;
    }

    // ── Function ───────────────────────────────────────────────────────

    private static FunctionDefinition ParseFunction(XElement el)
    {
        var f = new FunctionDefinition
        {
            Name       = Attr(el, "name") ?? "",
            Access     = Attr(el, "access") ?? "public",
            Lib        = Attr(el, "lib"),
            Convention = Attr(el, "convention"),
            Static     = BoolAttr(el, "static"),
            Unsafe     = BoolAttr(el, "unsafe"),
            ReturnType = el.Element("type")?.Value ?? ""
        };

        foreach (var paramEl in el.Elements("param"))
            f.Parameters.Add(ParseParam(paramEl));

        return f;
    }

    // ── Param ──────────────────────────────────────────────────────────

    private static ParamDefinition ParseParam(XElement el)
    {
        return new ParamDefinition
        {
            Name = Attr(el, "name") ?? "",
            Type = el.Element("type")?.Value ?? ""
        };
    }

    // ── Constant ───────────────────────────────────────────────────────

    private static ConstantDefinition ParseConstant(XElement el)
    {
        var typeEl  = el.Element("type");
        var valueEl = el.Element("value");

        return new ConstantDefinition
        {
            Name        = Attr(el, "name") ?? "",
            Access      = Attr(el, "access") ?? "public",
            Type        = typeEl?.Value ?? "",
            IsPrimitive = BoolAttr(typeEl, "primitive"),
            Value       = ParseConstantValue(valueEl)
        };
    }

    private static ConstantValue? ParseConstantValue(XElement? el)
    {
        if (el is null)
            return null;

        // Check for <deref> wrapper
        var derefEl = el.Element("deref");
        if (derefEl is not null)
        {
            return new ConstantValue
            {
                Code    = derefEl.Element("code")?.Value,
                IsDeref = true
            };
        }

        // Plain <code> child
        return new ConstantValue
        {
            Code    = el.Element("code")?.Value,
            IsDeref = false
        };
    }

    // ── Helpers ────────────────────────────────────────────────────────

    private static string? Attr(XElement? el, string name)
        => el?.Attribute(name)?.Value;

    private static bool BoolAttr(XElement? el, string name)
        => string.Equals(Attr(el, name), "true", StringComparison.OrdinalIgnoreCase);
}
