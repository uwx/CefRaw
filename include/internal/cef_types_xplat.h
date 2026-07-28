// cef_types_xplat.h — Cross-platform header for ClangSharpPInvokeGenerator
//
// Pre-defines the include guards for all platform-specific headers so they are
// never included, then typedefs all OS-specific handle types to void*.
// Pass this file to ClangSharp instead of the real platform headers to
// generate C# code with no platform dependencies (void* everywhere).
//
// Usage with ClangSharpPInvokeGenerator:
//   - Add this file's directory to include paths
//   - Include this file instead of cef_types_win.h / cef_types_linux.h / cef_types_mac.h

#ifndef CEF_INCLUDE_INTERNAL_CEF_TYPES_XPLAT_H_
#define CEF_INCLUDE_INTERNAL_CEF_TYPES_XPLAT_H_

// Pre-define all platform header guards to prevent their real content
#define CEF_INCLUDE_INTERNAL_CEF_TYPES_WIN_H_
#define CEF_INCLUDE_INTERNAL_CEF_TYPES_LINUX_H_
#define CEF_INCLUDE_INTERNAL_CEF_TYPES_MAC_H_

// Include the shared dependencies that all platform headers pull in
#include "include/internal/cef_string.h"
#include "include/internal/cef_types_color.h"
#include "include/internal/cef_types_geometry.h"
#include "include/internal/cef_types_osr.h"
#include "include/internal/cef_types_runtime.h"

// ── Windows types → void* ──────────────────────────────────────────────
typedef void* HWND;
typedef void* HINSTANCE;
typedef void* HMENU;
typedef void* HCURSOR;
typedef void* HICON;
typedef void* HANDLE;
typedef void* HDC;
typedef void* HBITMAP;
typedef void* HFONT;
typedef void* HMODULE;
typedef void* HMONITOR;
typedef void* HPALETTE;
typedef void* HPEN;
typedef void* HRGN;
typedef void* HRSRC;
typedef void* HWINSTA;
typedef void* HKL;
typedef void* HACCEL;
typedef void* HBRUSH;
typedef void* HSTR;
typedef void* HTASK;
typedef unsigned int   UINT;
typedef unsigned long  DWORD;
typedef long           LONG;
typedef long long      LONGLONG;
typedef unsigned long  ULONG;
typedef unsigned short WORD;
typedef int            BOOL;
typedef void*          WPARAM;
typedef void*          LPARAM;
typedef long long      LRESULT;
typedef unsigned short ATOM;

// MSG (tagMSG) — forward-declare as opaque struct with void* size
typedef struct tagMSG {
    void* hwnd;
    unsigned int message;
    void* wParam;
    void* lParam;
    unsigned int time;
    int x;
    int y;
} MSG;

// ── CEF handle types (platform-agnostic) ───────────────────────────────
typedef void* cef_cursor_handle_t;
typedef void* cef_event_handle_t;
typedef void* cef_window_handle_t;
typedef void* cef_shared_texture_handle_t;

#define kNullCursorHandle NULL
#define kNullEventHandle NULL
#define kNullWindowHandle NULL

#ifdef __cplusplus
extern "C" {
#endif

// ── cef_main_args_t (Windows layout, HINSTANCE → void*) ────────────────
typedef struct _cef_main_args_t {
    void* instance;
} cef_main_args_t;

// ── cef_window_info_t (Windows layout, all handles → void*) ────────────
typedef struct _cef_window_info_t {
    size_t size;

    // Standard parameters required by CreateWindowEx()
    unsigned long ex_style;
    cef_string_t window_name;
    unsigned long style;
    cef_rect_t bounds;
    cef_window_handle_t parent_window;
    void* menu;

    int windowless_rendering_enabled;
    int shared_texture_enabled;
    int external_begin_frame_enabled;

    cef_window_handle_t window;
    cef_runtime_style_t runtime_style;
} cef_window_info_t;

// ── cef_accelerated_paint_info_t (Windows layout) ──────────────────────
typedef struct _cef_accelerated_paint_info_t {
    size_t size;
    cef_shared_texture_handle_t shared_texture_handle;
    cef_color_type_t format;
    cef_accelerated_paint_info_common_t extra;
} cef_accelerated_paint_info_t;

#ifdef __cplusplus
}
#endif

#endif  // CEF_INCLUDE_INTERNAL_CEF_TYPES_XPLAT_H_
