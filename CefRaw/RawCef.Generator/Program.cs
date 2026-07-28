using CefRaw.BindingsParser;

var winBindings = BindingsParser.ParseFile("./bindings_win.xml");
var macBindings = BindingsParser.ParseFile("./bindings_mac.xml");
var linuxBindings = BindingsParser.ParseFile("./bindings_linux.xml");

foreach (var @struct in winBindings.Namespace.Structs)
{
    
}