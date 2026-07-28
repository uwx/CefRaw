namespace RawCef.Native;

public unsafe partial struct tagMSG
{
    public void* hwnd;

    [NativeTypeName("unsigned int")]
    public uint message;

    public void* wParam;

    public void* lParam;

    [NativeTypeName("unsigned int")]
    public uint time;

    public int x;

    public int y;
}
