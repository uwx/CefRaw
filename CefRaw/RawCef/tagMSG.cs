#if OS_WIN
namespace RawCef;

public unsafe struct tagMSG
{
    public HWND hwnd;
    public uint message;
    public void* wParam;
    public void* lParam;
    public uint time;
    public UIntPtr pt_x;
    public UIntPtr pt_y;
}
#endif