namespace RawCef.Native;

public unsafe partial struct CefStringTraitsWide
{
    public static void clear([NativeTypeName("CefStringTraitsWide::struct_type *")] _cef_string_wide_t* s)
    {
        cef_string_wide_clear(s);
    }

    public static int set([NativeTypeName("const char_type *")] uint* src, [NativeTypeName("size_t")] nuint src_size, [NativeTypeName("CefStringTraitsWide::struct_type *")] _cef_string_wide_t* output, int copy)
    {
        return cef_string_wide_set(src, src_size, output, copy);
    }

    public static int compare([NativeTypeName("const struct_type *")] _cef_string_wide_t* s1, [NativeTypeName("const struct_type *")] _cef_string_wide_t* s2)
    {
        return cef_string_wide_cmp(s1, s2);
    }

    [return: NativeTypeName("CefStringTraitsWide::userfree_struct_type")]
    public static _cef_string_wide_t* userfree_alloc()
    {
        return cef_string_userfree_wide_alloc();
    }

    public static void userfree_free([NativeTypeName("CefStringTraitsWide::userfree_struct_type")] _cef_string_wide_t* ufs)
    {
        cef_string_userfree_wide_free(ufs);
    }

    public static bool from_ascii([NativeTypeName("const char *")] sbyte* str, [NativeTypeName("size_t")] nuint len, [NativeTypeName("CefStringTraitsWide::struct_type *")] _cef_string_wide_t* s)
    {
        return (cef_string_ascii_to_wide(str, len, s)) != 0 ? true : false;
    }

    [return: NativeTypeName("std::string")]
    public static basic_string<sbyte, char_traits<sbyte>, allocator<sbyte>> to_string([NativeTypeName("const struct_type *")] _cef_string_wide_t* s)
    {
        _cef_string_utf8_t cstr = new _cef_string_utf8_t();

        cstr = default;
        _ = cef_string_wide_to_utf8(s->str, s->length, &cstr);
        basic_string<sbyte, char_traits<sbyte>, allocator<sbyte>> str = new basic_string();

        if (cstr.length > 0)
        {
            str.op_Assign(&);
        }

        cef_string_utf8_clear(&cstr);
        return str;
    }

    public static bool from_string([NativeTypeName("const std::string::value_type *")] sbyte* data, [NativeTypeName("size_t")] nuint length, [NativeTypeName("CefStringTraitsWide::struct_type *")] _cef_string_wide_t* s)
    {
        return (cef_string_utf8_to_wide(data, length, s)) != 0 ? true : false;
    }

    public static bool from_string([NativeTypeName("const std::string &")] basic_string<sbyte, char_traits<sbyte>, allocator<sbyte>>* str, [NativeTypeName("CefStringTraitsWide::struct_type *")] _cef_string_wide_t* s)
    {
        return from_string(str->data(), str->length(), s);
    }

    [return: NativeTypeName("std::wstring")]
    public static basic_string<uint, char_traits<uint>, allocator<uint>> to_wstring([NativeTypeName("const struct_type *")] _cef_string_wide_t* s)
    {
        return ;
    }

    public static bool from_wstring([NativeTypeName("const std::wstring::value_type *")] uint* data, [NativeTypeName("size_t")] nuint length, [NativeTypeName("CefStringTraitsWide::struct_type *")] _cef_string_wide_t* s)
    {
        return (cef_string_wide_set(data, length, s, (true) ? 1 : 0)) != 0 ? true : false;
    }

    public static bool from_wstring([NativeTypeName("const std::wstring &")] basic_string<uint, char_traits<uint>, allocator<uint>>* str, [NativeTypeName("CefStringTraitsWide::struct_type *")] _cef_string_wide_t* s)
    {
        return from_wstring(str->data(), str->length(), s);
    }

    [return: NativeTypeName("std::u16string")]
    public static basic_string<ushort, char_traits<ushort>, allocator<ushort>> to_string16([NativeTypeName("const struct_type *")] _cef_string_wide_t* s)
    {
        return ;
    }

    public static bool from_string16([NativeTypeName("const std::u16string::value_type *")] ushort* data, [NativeTypeName("size_t")] nuint length, [NativeTypeName("CefStringTraitsWide::struct_type *")] _cef_string_wide_t* s)
    {
        return (cef_string_wide_set((uint*)(data), length, s, (true) ? 1 : 0)) != 0 ? true : false;
    }

    public static bool from_string16([NativeTypeName("const std::u16string &")] basic_string<ushort, char_traits<ushort>, allocator<ushort>>* str, [NativeTypeName("CefStringTraitsWide::struct_type *")] _cef_string_wide_t* s)
    {
        return from_string16(str->data(), str->length(), s);
    }
}
