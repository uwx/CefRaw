namespace RawCef.Native;

public unsafe partial struct _cef_task_manager_t
{
    [NativeTypeName("cef_base_ref_counted_t")]
    public _cef_base_ref_counted_t @base;

    [NativeTypeName("size_t (*)(struct _cef_task_manager_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_task_manager_t*, nuint> get_tasks_count;

    [NativeTypeName("int (*)(struct _cef_task_manager_t *, size_t *, int64_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_task_manager_t*, nuint*, long*, int> get_task_ids_list;

    [NativeTypeName("int (*)(struct _cef_task_manager_t *, int64_t, struct _cef_task_info_t *) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_task_manager_t*, long, _cef_task_info_t*, int> get_task_info;

    [NativeTypeName("int (*)(struct _cef_task_manager_t *, int64_t) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_task_manager_t*, long, int> kill_task;

    [NativeTypeName("int64_t (*)(struct _cef_task_manager_t *, int) __attribute__((stdcall))")]
    public delegate* unmanaged[Stdcall]<_cef_task_manager_t*, int, long> get_task_id_for_browser_id;
}
