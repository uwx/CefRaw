#include "include/base/cef_build.h"

#undef OS_WIN
#undef OS_MAC
#undef OS_LINUX
#undef OS_APPLE
#undef OS_POSIX

#if defined(CLANGSHARP_WIN)
#define OS_WIN 1
#elif defined(CLANGSHARP_MAC)
#define OS_MAC 1
#elif defined(CLANGSHARP_LINUX)
#define OS_LINUX 1
#endif

#if defined(OS_MAC) || defined(OS_IOS)
#define OS_APPLE 1
#endif

#if defined(OS_AIX) || defined(OS_ANDROID) || defined(OS_ASMJS) ||  \
    defined(OS_FREEBSD) || defined(OS_IOS) || defined(OS_LINUX) ||  \
    defined(OS_CHROMEOS) || defined(OS_MAC) || defined(OS_NACL) ||  \
    defined(OS_NETBSD) || defined(OS_OPENBSD) || defined(OS_QNX) || \
    defined(OS_SOLARIS)
#define OS_POSIX 1
#endif

// Compiler detection. Note: clang masquerades as GCC on POSIX and as MSVC on
// Windows.
#if defined(__GNUC__)
#define COMPILER_GCC 1
#elif defined(_MSC_VER)
#define COMPILER_MSVC 1
#else
#error Please add support for your compiler in build/build_config.h
#endif

// Type detection for wchar_t.
#if defined(OS_WIN)
#define WCHAR_T_IS_16_BIT
#elif defined(OS_FUCHSIA)
#define WCHAR_T_IS_32_BIT
#elif defined(OS_POSIX) && defined(COMPILER_GCC) && defined(__WCHAR_MAX__) && \
    (__WCHAR_MAX__ == 0x7fffffff || __WCHAR_MAX__ == 0xffffffff)
#define WCHAR_T_IS_32_BIT
#elif defined(OS_POSIX) && defined(COMPILER_GCC) && defined(__WCHAR_MAX__) && \
    (__WCHAR_MAX__ == 0x7fff || __WCHAR_MAX__ == 0xffff)
// On Posix, we'll detect short wchar_t, but projects aren't guaranteed to
// compile in this mode (in particular, Chrome doesn't). This is intended for
// other projects using base who manage their own dependencies and make sure
// short wchar works for them.
#define WCHAR_T_IS_16_BIT
#else
// #error Please add support for your compiler in include/base/cef_build.h
#define WCHAR_T_IS_32_BIT
#endif

#include "include/capi/cef_app_capi.h"
#include "include/capi/cef_client_capi.h"
#include "include/capi/cef_browser_capi.h"
#include "include/capi/cef_browser_process_handler_capi.h"