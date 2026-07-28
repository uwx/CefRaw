using CefRaw.BindingsParser;

string xml = """
<?xml version="1.0"?>
<bindings>
  <namespace name="Test">
    <enumeration name="cef_drag_operations_mask_t" access="public">
      <type>int</type>
      <enumerator name="DRAG_OPERATION_NONE" access="public">
        <type primitive="False">int</type>
        <value>
          <code>0</code>
        </value>
      </enumerator>
      <enumerator name="DRAG_OPERATION_COPY" access="public">
        <type primitive="False">int</type>
        <value>
          <code>1</code>
        </value>
      </enumerator>
      <enumerator name="DRAG_OPERATION_EVERY" access="public">
        <type primitive="False">int</type>
        <value>
          <unchecked>
            <value>
              <cast>int</cast>
              <value>
                <code>0xffffffff</code>
              </value>
            </value>
          </unchecked>
        </value>
      </enumerator>
    </enumeration>
  </namespace>
</bindings>
""";

var root = BindingsParser.ParseString(xml);
var en = root.Namespace.Enumerations[0];

Console.WriteLine($"Enum: {en.Name}, Type: {en.Type}");
Console.WriteLine($"Enumerators count: {en.Enumerators.Count}");

foreach (var e in en.Enumerators)
{
    Console.WriteLine($"  {e.Name}:");
    Console.WriteLine($"    Type: {e.Type}, IsPrimitive: {e.IsPrimitive}");
    Console.WriteLine($"    Value.Code: {e.Value?.Code}");
    Console.WriteLine($"    Value.IsUnchecked: {e.Value?.IsUnchecked}");
    Console.WriteLine($"    Value.IsDeref: {e.Value?.IsDeref}");
}

// Assertions
var every = en.Enumerators.First(e => e.Name == "DRAG_OPERATION_EVERY");
if (every.Value?.Code != "0xffffffff")
{
    Console.Error.WriteLine($"FAIL: Expected 0xffffffff, got '{every.Value?.Code}'");
    return 1;
}
if (!every.Value.IsUnchecked)
{
    Console.Error.WriteLine("FAIL: Expected IsUnchecked=true");
    return 1;
}

Console.WriteLine("\nAll checks passed!");
return 0;
