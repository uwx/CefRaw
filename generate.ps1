<#
ClangSharpPInvokeGenerator
  ClangSharp P/Invoke Binding Generator

Usage:
  ClangSharpPInvokeGenerator [options]

Options:
  -a, --additional <additional>                                     An argument to pass to Clang when parsing the input files. []
  -c, --config <config>                                             A configuration option that controls how the bindings are generated. Specify 'help' to see the available options. []
  -g, --generate <generate>                                         A feature to generate, e.g. 'aggressive-inlining', 'tests-nunit', or 'generated-code=type'. Append '=false' to opt a feature back out. Specify '--config help' to see the available names. []
  -lg, --log <log>                                                  A diagnostic log to emit during generation: 'exclusions', 'potential-typedef-remappings', or 'visited-files'. []
  -D, --define-macro <define-macro>                                 Define <macro> to <value> (or 1 if <value> omitted). []
  -e, --exclude <exclude>                                           A declaration name to exclude from binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -f, --file <file>                                                 A file to parse and generate bindings for. []
  -F, --file-directory <file-directory>                             The base path for files to parse. []
  -hf, --header-file <header-file>                                  A file which contains the header to prefix every generated file with. []
  -i, --include <include>                                           A declaration name to include in binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -I, --include-directory <include-directory>                       Add directory to include search path. []
  -x, --language <c|c++|objective-c|objective-c++>                  Treat subsequent input files as having type <language>. [default: c++]
  -l, --library-path <library-path>                                 The string to use in the DllImport attribute used when generating bindings. []
  -m, --method-class-name <method-class-name>                       The name of the static class that will contain the generated method bindings. [default: Methods]
  -n, --namespace <namespace>                                       The namespace in which to place the generated bindings. []
  -om, --output-mode <CSharp|Xml>                                   The mode describing how the information collected from the headers are presented in the resultant bindings. [default: CSharp]
  -o, --output <output>                                             The output location to write the generated bindings to. []
  -p, --prefix-strip <prefix-strip>                                 The prefix to strip from the generated method bindings. []
  -tp, --type-prefix-strip <type-prefix-strip>                      The prefix to strip from the generated enum, struct, and union type bindings (and their enum member names). []
  --native-type-names-to-strip <native-type-names-to-strip>         The contents to strip from the generated NativeTypeName attributes. []
  -r, --remap <remap>                                               A declaration name to be remapped to another name during binding generation. []
  -rt, --remap-type <remap-type>                                    A type (record or enum) declaration name to be remapped to another name during binding generation. Takes precedence over --remap and is useful when a type and field share a name. []
  -rf, --remap-field <remap-field>                                  A field declaration name to be remapped to another name during binding generation. Takes precedence over --remap and is useful when a type and field share a name. []
  -rd, --resource-directory <directory>                             The Clang resource directory containing the builtin headers (such as stddef.h). When omitted, an installed and version-matched Clang's resource directory is automatically detected. []
  --no-resource-directory-detection                                 Disable the automatic detection of the Clang resource directory.
  -std, --std <std>                                                 Language standard to compile for. []
  -to, --test-output <test-output>                                  The output location to write the generated tests to. []
  -t, --traverse <traverse>                                         A file name included either directly or indirectly by -f that should be traversed during binding generation. []
  -v, --version                                                     Prints the current version information for the tool and its native dependencies.
  -was, --with-access-specifier <with-access-specifier>             An access specifier to be used with the given qualified or remapped declaration name during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wa, --with-attribute <with-attribute>                            An attribute to be added to the given remapped declaration name during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wb, --with-base <with-base>                                      An additional base type the generated type should derive from during binding generation. Applies to structs and COM interface types. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wcc, --with-callconv <with-callconv>                             A calling convention to be used for the given declaration during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wc, --with-class <with-class>                                    A class to be used for the given remapped constant or function declaration name during binding generation. Supports a trailing `*` wildcard for prefix matching; an exact match takes precedence. []
  -wcond, --with-conditional <symbol>                               A preprocessor symbol used to wrap single-file C# output in a leading '#if <symbol>' and trailing '#endif'. Useful when files can't be conditionally excluded at the project level (e.g. Unity). []
  -wcfv, --with-constant-folded-value <with-constant-folded-value>  Emit the clang-evaluated constant value for the given declaration instead of translating the written initializer expression. Useful when an initializer references companion declarations that aren't themselves generated. Applies to enum members (matched by the qualified `Enum.Member`, so `Enum.*` folds every member) and to macro or const value declarations. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wems, --with-enum-member-strip <with-enum-member-strip>          How to strip a prefix or suffix from the members of the given remapped enum name during binding generation. Mode is one of `none`, `common-prefix`, `common-suffix`, `type-name`, `prefix:<str>`, or `suffix:<str>`. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wem, --with-equality-members <with-equality-members>             Generate IEquatable<T> with field-wise Equals, GetHashCode, and the == and != operators for the given struct. Opt-in and not valid for every native type; a named struct also opts in the nested and base structs it compares. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wg, --with-guid <with-guid>                                      A GUID to be used for the given declaration during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wl, --with-length <with-length>                                  A length to be used for the given declaration during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wlb, --with-library-path <with-library-path>                     A library path to be used for the given declaration during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wmi, --with-manual-import <with-manual-import>                   A remapped function name to be treated as a manual import during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wn, --with-namespace <with-namespace>                            A namespace to be used for the given remapped declaration name during binding generation. Supports a trailing `*` wildcard for prefix matching; an exact match takes precedence. []
  -wp, --with-packing <with-packing>                                Overrides the StructLayoutAttribute.Pack property for the given type. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wro, --with-readonly <with-readonly>                             Add the readonly modifier to a given instance method. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wsle, --with-setlasterror <with-setlasterror>                    Add the SetLastError=true modifier or SetsSystemLastError attribute to a given DllImport or UnmanagedFunctionPointer. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wsgct, --with-suppressgctransition <with-suppressgctransition>   Add the SuppressGCTransition calling convention to a given DllImport or UnmanagedFunctionPointer. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wts, --with-transparent-struct <with-transparent-struct>         A remapped type name to be treated as a transparent wrapper during binding generation. Matched by exact name only. []
  -wt, --with-type <with-type>                                      A type to be used for the given enum declaration, macro constant, or struct field (using the qualified `Type.field`) during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  -wu, --with-using <with-using>                                    A using directive to be included for the given remapped declaration name during binding generation. Supports `*` (any run) and `?` (single character) wildcards; exact matches take precedence. []
  --without-access-specifier <without-access-specifier>             A declaration name to opt back out of a '--with-access-specifier *' catch-all. []
  --without-attribute <without-attribute>                           A declaration name to opt back out of a '--with-attribute *' catch-all. []
  --without-callconv <without-callconv>                             A declaration name to opt back out of a '--with-callconv *' catch-all. []
  --without-constant-folded-value <without-constant-folded-value>   A declaration name to opt back out of a '--with-constant-folded-value *' catch-all. []
  --without-enum-member-strip <without-enum-member-strip>           An enum name to opt back out of a '--with-enum-member-strip *' catch-all. []
  --without-equality-members <without-equality-members>             A struct name to opt back out of a '--with-equality-members *' catch-all. []
  --without-library-path <without-library-path>                     A declaration name to opt back out of a '--with-library-path *' catch-all. []
  --without-readonly <without-readonly>                             A method name to opt back out of a '--with-readonly *' catch-all. []
  --without-setlasterror <without-setlasterror>                     A declaration name to opt back out of a '--with-setlasterror *' catch-all. []
  --without-suppressgctransition <without-suppressgctransition>     A declaration name to opt back out of a '--with-suppressgctransition *' catch-all. []
  --without-type <without-type>                                     A declaration name to opt back out of a '--with-type *' catch-all. []
  --without-using <without-using>                                   A declaration name to opt back out of a '--with-using *' catch-all. []
  -?, -h, --help                                                    Show help and usage information

Wildcards:
Many name-matching options accept glob patterns using `*` (matches any run of characters, including qualification separators) and `?` (matches a single character); `::` and `.` are treated as equivalent separators and matching is case-sensitive. An exact match always wins over a glob, and among globs the most specific (the most literal characters) wins. Many `--with-*` options also accept a bare `*` as a catch-all that applies a rule to everything; for value options it is written `*=value` (e.g. --with-access-specifier *=Internal makes all generated code internal). Each such option has a paired `--without-<name>` option that opts a specific declaration (by exact name or glob) back out of its `*` catch-all (e.g. --with-access-specifier *=Internal --without-access-specifier Foo). To opt everything in and exclude piecemeal, use the include (-i) and exclude (-e) options together; they are already an opt-in/opt-out pair and likewise accept globs.

More information:
See https://github.com/dotnet/ClangSharp/blob/main/docs/generating-bindings-best-practices.md for a guide on structuring a generation project and using these options.

--config, -c	A configuration option that controls how the bindings are generated. Specify 'help' to see the available options.

Options:
  ?, h, help                                  Show help and usage information for -c, --config, --generate, and --log

  # -c, --config now carries only the four mode families below (plus this help). The
  # feature switches moved to --generate <name> and the diagnostics to --log <name>.
  # Boolean --generate features accept an optional '=true'/'=false' ('=true' is implied
  # when omitted), so a later response file can override an earlier one. Valued switches
  # take the value shown as name=<value>.

  # Mode Families (-c <name>=<value>)

  codegen=<value>                             Which .NET/C# level to target: 'compatible' (.NET Standard 2.0), 'default' (current LTS; .NET 8/C# 12), 'latest' (current STS; .NET 10/C# 14), or 'preview'. Defaults to 'default'.
  file=<value>                                How output is split: 'single' (one output file; the default) or 'multi' (approximately one type per file).
  types=<value>                               Which platform defaults to assume: 'windows' or 'unix'. Defaults to the host platform.
  vtbls=<value>                               How VTBLs are generated: 'explicit' (a named field per entry), 'implicit' (the default; reduces metadata bloat), or 'trimmable' (defined but unused in helpers to reduce bloat when trimming).

  # Test Generation (--generate <name>)

  tests-nunit                                 Basic tests validating size, blittability, and associated metadata should be generated for NUnit.
  tests-xunit                                 Basic tests validating size, blittability, and associated metadata should be generated for XUnit.

  # Generation Features (--generate <name>[=false])

  aggressive-inlining                         [MethodImpl(MethodImplOptions.AggressiveInlining)] should be added to generated helper functions.
  anonymous-field-helpers                     The helper ref properties for fields in nested anonymous structs and unions should be generated. On by default; use =false to opt out.
  callconv-member-function                    Instance function pointers should use [CallConvMemberFunction] where applicable.
  com-proxies                                 Types recognized as COM proxies should have bindings generated. These are currently function declarations ending with _UserFree, _UserMarshal, _UserSize, _UserUnmarshal, _Proxy, or _Stub. On by default; use =false to opt out.
  cpp-attributes                              [CppAttributeList("")] should be generated to document the encountered C++ attributes.
  default-remappings                          Default remappings for well known types should be added. This currently includes intptr_t, ptrdiff_t, size_t, ssize_t, uintptr_t, and the exact-width stdint types (int8_t, int16_t, int32_t, int64_t, uint8_t, uint16_t, uint32_t, and uint64_t). When targeting Windows, the pointer-width Windows types (INT_PTR, LONG_PTR, SSIZE_T, DWORD_PTR, SIZE_T, UINT_PTR, and ULONG_PTR) and _GUID are also included. On by default; use =false to opt out.
  disable-runtime-marshalling                 [assembly: DisableRuntimeMarshalling] should be generated.
  doc-includes                                <include> xml documentation tags should be generated for declarations.
  empty-records                               Bindings for records that contain no members should be generated. These are commonly encountered for opaque handle like types such as HWND. On by default; use =false to opt out.
  enum-member-type-name                       The enum type name should be kept at the beginning of its member names. On by default; use =false to strip it.
  enum-operators                              Bindings for operators over enum types should be generated. These are largely unnecessary in C# as the operators are available by default. On by default; use =false to opt out.
  extern-variables                            Top-level extern/extern const global variables should be surfaced as settable pointer fields on a <Class>ManualImports struct for the consumer to resolve (like --with-manual-import). Opt-in; pointer and primitive types only.
  file-scoped-namespaces                      Namespaces should be scoped to the file to reduce nesting.
  fixed-buffer-indexer-overloads              Fixed sized buffer helper types should generate additional uint, nint, and nuint indexer overloads.
  fnptr-codegen                               Generated bindings for latest or preview codegen should use function pointers. On by default; use =false to opt out.
  funcs-with-body                             Bindings for functions with bodies should be generated. On by default; use =false to opt out.
  generated-code=<mode>                       Controls the emission of the GeneratedCode attribute. 'assembly' (default) emits a single '[assembly: GeneratedCode]' when helper types are generated; 'type' instead annotates each generated top-level type; 'none' emits neither.
  generic-pointer-wrapper                     Pointer<T> should be used for limited generic type support.
  guid-member                                 Types with an associated GUID should have a corresponding member generated.
  helper-types                                Code files should be generated for various helper attributes and declared transparent structs.
  macro-bindings                              Bindings for macro-definitions should be generated. This currently only works with value like macros and not function-like ones.
  marker-interfaces                           Bindings for marker interfaces representing native inheritance hierarchies should be generated.
  native-alignment-attribute                  [NativeAlignment(#)] attribute should be generated to document the requested over-alignment (__declspec(align) / DECLSPEC_ALIGN) that .NET cannot honor.
  native-bitfield-attribute                   [NativeBitfield("", offset: #, length: #)] attribute should be generated to document the encountered bitfield layout.
  native-inheritance-attribute                [NativeInheritance("")] attribute should be generated to document the encountered C++ base type.
  nint-codegen                                Generated bindings should use nint/nuint where applicable. On by default; use =false to opt out.
  objective-c-bindings                        Bindings for Objective-C declarations (currently @protocol types) should be generated. This is experimental and requires the Objective-C runtime (libobjc) at runtime.
  setslastsystemerror-attribute               [SetsLastSystemError] attribute should be generated rather than using SetLastError = true.
  template-bindings                           Bindings for template-definitions should be generated. This is currently experimental.
  unmanaged-constants                         Unmanaged constants should be generated using static ref readonly properties. This is currently experimental.
  using-statics-for-enums                     Enum usages should include a corresponding 'using static EnumName;' rather than being fully qualified. On by default; use =false to opt out.
  using-statics-for-guid-members              GUID member usages should include a corresponding 'using static' rather than being fully qualified. On by default; use =false to opt out.
  vtbl-index-attribute                        [VtblIndex(#)] attribute should be generated to document the underlying VTBL index for a helper method.

  # Diagnostic Logs (--log <name>)

  exclusions                                  A list of excluded declaration types should be generated. This will also log if the exclusion was due to an exact or partial match.
  potential-typedef-remappings                A list of potential typedef remappings should be generated. This can help identify missing remappings.
  visited-files                               A list of the visited files should be generated. This can help identify traversal issues.

  # Legacy/Deprecated -c spellings (still accepted, but each emits a deprecation warning)

  -c compatible-codegen                       Use -c codegen=compatible.
  -c default-codegen                          Use -c codegen=default.
  -c latest-codegen                           Use -c codegen=latest.
  -c preview-codegen                          Use -c codegen=preview.
  -c single-file                              Use -c file=single.
  -c multi-file                               Use -c file=multi.
  -c windows-types                            Use -c types=windows.
  -c unix-types                               Use -c types=unix.
  -c explicit-vtbls                           Use -c vtbls=explicit.
  -c implicit-vtbls                           Use -c vtbls=implicit.
  -c trimmable-vtbls                          Use -c vtbls=trimmable.
  -c generate-*                               Use --generate * (e.g. -c generate-helper-types becomes --generate helper-types).
  -c log-*                                    Use --log * (e.g. -c log-visited-files becomes --log visited-files).
  -c exclude-anonymous-field-helpers          Use --generate anonymous-field-helpers=false.
  -c exclude-com-proxies                      Use --generate com-proxies=false.
  -c exclude-default-remappings               Use --generate default-remappings=false.
  -c no-default-remappings                    Use --generate default-remappings=false.
  -c default-remappings                       Use --generate default-remappings (or =true).
  -c exclude-empty-records                    Use --generate empty-records=false.
  -c exclude-enum-operators                   Use --generate enum-operators=false.
  -c exclude-fnptr-codegen                    Use --generate fnptr-codegen=false.
  -c exclude-funcs-with-body                  Use --generate funcs-with-body=false.
  -c exclude-nint-codegen                     Use --generate nint-codegen=false.
  -c exclude-using-statics-for-enums          Use --generate using-statics-for-enums=false.
  -c dont-use-using-statics-for-enums         Use --generate using-statics-for-enums=false.
  -c exclude-using-statics-for-guid-members   Use --generate using-statics-for-guid-members=false.
  -c dont-use-using-statics-for-guid-members  Use --generate using-statics-for-guid-members=false.
  -c strip-enum-member-type-name              Use --generate enum-member-type-name=false.
#>

$commonArgs = @(
    '-c', 'codegen=latest',
    '-c', 'file=single',
    '-c', 'vtbls=trimmable',
   # '--generate', 'tests-nunit',
    '--generate', 'aggressive-inlining',
    '--generate', 'anonymous-field-helpers',
    '--generate', 'unmanaged-constants',
    '--generate', 'marker-interfaces',
    '--generate', 'macro-bindings',
    '--generate', 'helper-types',
    '--generate', 'fnptr-codegen',
    '--generate', 'fixed-buffer-indexer-overloads',
    '--generate', 'file-scoped-namespaces',
    '--include-directory', '.',
    '-n', 'RawCef.Native',
    '-std', 'c++20',
    '-l', 'libcef',
    '-m', 'RawMethods',
    '-om', 'Xml',
    '-f', 'include/capi/cef_app_capi.h',
    '-f', 'include/capi/cef_client_capi.h',
    '-f', 'include/capi/cef_browser_capi.h',
    '-f', 'include/capi/cef_browser_process_handler_capi.h',
    '-f', 'include/cef_api_hash.h',
    '-t', 'include/capi/cef_accessibility_handler_capi.h',
    '-t', 'include/capi/cef_audio_handler_capi.h',
    '-t', 'include/capi/cef_auth_callback_capi.h',
    '-t', 'include/capi/cef_base_capi.h',
    '-t', 'include/capi/cef_callback_capi.h',
    '-t', 'include/capi/cef_command_handler_capi.h',
    '-t', 'include/capi/cef_command_line_capi.h',
    '-t', 'include/capi/cef_component_updater_capi.h',
    '-t', 'include/capi/cef_context_menu_handler_capi.h',
    '-t', 'include/capi/cef_cookie_capi.h',
    '-t', 'include/capi/cef_crash_util_capi.h',
    '-t', 'include/capi/cef_devtools_message_observer_capi.h',
    '-t', 'include/capi/cef_dialog_handler_capi.h',
    '-t', 'include/capi/cef_display_handler_capi.h',
    '-t', 'include/capi/cef_dom_capi.h',
    '-t', 'include/capi/cef_download_handler_capi.h',
    '-t', 'include/capi/cef_download_item_capi.h',
    '-t', 'include/capi/cef_drag_data_capi.h',
    '-t', 'include/capi/cef_drag_handler_capi.h',
    '-t', 'include/capi/cef_file_util_capi.h',
    '-t', 'include/capi/cef_find_handler_capi.h',
    '-t', 'include/capi/cef_focus_handler_capi.h',
    '-t', 'include/capi/cef_frame_capi.h',
    '-t', 'include/capi/cef_frame_handler_capi.h',
    '-t', 'include/capi/cef_i18n_util_capi.h',
    '-t', 'include/capi/cef_image_capi.h',
    '-t', 'include/capi/cef_jsdialog_handler_capi.h',
    '-t', 'include/capi/cef_keyboard_handler_capi.h',
    '-t', 'include/capi/cef_life_span_handler_capi.h',
    '-t', 'include/capi/cef_load_handler_capi.h',
    '-t', 'include/capi/cef_media_router_capi.h',
    '-t', 'include/capi/cef_menu_model_capi.h',
    '-t', 'include/capi/cef_menu_model_delegate_capi.h',
    '-t', 'include/capi/cef_navigation_entry_capi.h',
    '-t', 'include/capi/cef_origin_whitelist_capi.h',
    '-t', 'include/capi/cef_parser_capi.h',
    '-t', 'include/capi/cef_path_util_capi.h',
    '-t', 'include/capi/cef_permission_handler_capi.h',
    '-t', 'include/capi/cef_preference_capi.h',
    '-t', 'include/capi/cef_print_handler_capi.h',
    '-t', 'include/capi/cef_print_settings_capi.h',
    '-t', 'include/capi/cef_process_message_capi.h',
    '-t', 'include/capi/cef_process_util_capi.h',
    '-t', 'include/capi/cef_registration_capi.h',
    '-t', 'include/capi/cef_render_handler_capi.h',
    '-t', 'include/capi/cef_render_process_handler_capi.h',
    '-t', 'include/capi/cef_request_capi.h',
    '-t', 'include/capi/cef_request_context_capi.h',
    '-t', 'include/capi/cef_request_context_handler_capi.h',
    '-t', 'include/capi/cef_request_handler_capi.h',
    '-t', 'include/capi/cef_resource_bundle_capi.h',
    '-t', 'include/capi/cef_resource_bundle_handler_capi.h',
    '-t', 'include/capi/cef_resource_handler_capi.h',
    '-t', 'include/capi/cef_resource_request_handler_capi.h',
    '-t', 'include/capi/cef_response_capi.h',
    '-t', 'include/capi/cef_response_filter_capi.h',
    '-t', 'include/capi/cef_scheme_capi.h',
    '-t', 'include/capi/cef_server_capi.h',
    '-t', 'include/capi/cef_shared_memory_region_capi.h',
    '-t', 'include/capi/cef_shared_process_message_builder_capi.h',
    '-t', 'include/capi/cef_ssl_info_capi.h',
    '-t', 'include/capi/cef_ssl_status_capi.h',
    '-t', 'include/capi/cef_stream_capi.h',
    '-t', 'include/capi/cef_string_visitor_capi.h',
    '-t', 'include/capi/cef_task_capi.h',
    '-t', 'include/capi/cef_task_manager_capi.h',
    '-t', 'include/capi/cef_thread_capi.h',
    '-t', 'include/capi/cef_trace_capi.h',
    '-t', 'include/capi/cef_unresponsive_process_callback_capi.h',
    '-t', 'include/capi/cef_urlrequest_capi.h',
    '-t', 'include/capi/cef_v8_capi.h',
    '-t', 'include/capi/cef_values_capi.h',
    '-t', 'include/capi/cef_waitable_event_capi.h',
    '-t', 'include/capi/cef_x509_certificate_capi.h',
    '-t', 'include/capi/cef_xml_reader_capi.h',
    '-t', 'include/capi/cef_zip_reader_capi.h',
    '-t', 'include/capi/views/cef_view_capi.h',
    '-t', 'include/capi/views/cef_view_delegate_capi.h',
    '-t', 'include/capi/views/cef_window_capi.h',
    '-t', 'include/capi/views/cef_textfield_capi.h',
    '-t', 'include/capi/views/cef_textfield_delegate_capi.h',
    '-t', 'include/capi/views/cef_overlay_controller_capi.h',
    '-t', 'include/capi/views/cef_panel_capi.h',
    '-t', 'include/capi/views/cef_panel_delegate_capi.h',
    '-t', 'include/capi/views/cef_scroll_view_capi.h',
    '-t', 'include/capi/views/cef_layout_capi.h',
    '-t', 'include/capi/views/cef_menu_button_capi.h',
    '-t', 'include/capi/views/cef_menu_button_delegate_capi.h',
    '-t', 'include/capi/views/cef_display_capi.h',
    '-t', 'include/capi/views/cef_fill_layout_capi.h',
    '-t', 'include/capi/views/cef_label_button_capi.h',
    '-t', 'include/capi/views/cef_browser_view_delegate_capi.h',
    '-t', 'include/capi/views/cef_button_capi.h',
    '-t', 'include/capi/views/cef_button_delegate_capi.h',
    '-t', 'include/capi/views/cef_box_layout_capi.h',
    '-t', 'include/capi/views/cef_browser_view_capi.h',
    '-t', 'include/capi/views/cef_window_delegate_capi.h',
    #'-t', 'include/internal/cef_dump_without_crashing_internal.h',
    #'-t', 'include/internal/cef_export.h',
    #'-t', 'include/internal/cef_logging_internal.h',
    #'-t', 'include/internal/cef_ptr.h',
    '-t', 'include/internal/cef_string.h',
    '-t', 'include/internal/cef_string_list.h',
    '-t', 'include/internal/cef_string_map.h',
    '-t', 'include/internal/cef_string_multimap.h',
    '-t', 'include/internal/cef_string_types.h'
    #'-t', 'include/internal/cef_string_wrappers.h',
    #'-t', 'include/internal/cef_thread_internal.h',
    #'-t', 'include/internal/cef_time.h',
    #'-t', 'include/internal/cef_time_wrappers.h',
    #'-t', 'include/internal/cef_trace_event_internal.h',
    #'-t', 'include/internal/cef_types.h',
    #'-t', 'include/internal/cef_types_color.h',
    #'-t', 'include/internal/cef_types_component.h',
    #'-t', 'include/internal/cef_types_content_settings.h',
    #'-t', 'include/internal/cef_types_geometry.h',
    #'-t', 'include/internal/cef_types_osr.h',
    #'-t', 'include/internal/cef_types_runtime.h'
)

$platforms = @(
    @{ Name = 'win';   Types = 'windows'; Defines = @('OS_WIN', 'NOMINMAX', 'WIN32_LEAN_AND_MEAN') },
    @{ Name = 'mac';   Types = 'unix';    Defines = @('OS_MAC') },
    @{ Name = 'linux'; Types = 'unix';    Defines = @('OS_LINUX') }
)

foreach ($p in $platforms) {
    $platformArgs = @(
        '-c', "types=$($p.Types)"
    )
    foreach ($d in $p.Defines) {
        $platformArgs += '-D'
        $platformArgs += $d
    }
    $platformArgs += '-o'
    $platformArgs += "bindings_$($p.Name).xml"
    #$platformArgs += '-to'
    #$platformArgs += "tests_$($p.Name).xml"

    Write-Host "=== Generating for $($p.Name) (types=$($p.Types), defines=$($p.Defines -join ', ')) ==="

    $allArgs = $commonArgs + $platformArgs
    & ClangSharpPInvokeGenerator @allArgs
}