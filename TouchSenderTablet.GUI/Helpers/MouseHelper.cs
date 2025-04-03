namespace TouchSenderTablet.GUI.Helpers;

using System;
using System.Runtime.InteropServices;

/// <summary>
/// マウス操作をエミュレートするヘルパークラス
/// </summary>
public static partial class MouseHelper
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名スタイル")]
    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public uint type;
        public MOUSEINPUT mi;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名スタイル")]
    [StructLayout(LayoutKind.Sequential)]
    private struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    /// <summary>
    /// https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/nf-winuser-sendinput
    /// SendInput 関数は、INPUT 構造体のイベントをキーボードまたはマウス入力ストリームに順次挿入します。
    /// これらのイベントは、ユーザー (キーボードまたはマウスを使用) または SendInput へのkeybd_event、
    /// mouse_event、またはその他の呼び出しの呼び出しによって挿入された他のキーボードまたはマウス入力イベントに分散されません。
    /// この関数は、キーボードの現在の状態をリセットしません。 関数が呼び出されたときに既に押されているキーは、
    /// この関数によって生成されるイベントに干渉する可能性があります。
    /// </summary>
    /// <param name="nInputs"></param>
    /// <param name="pInputs"></param>
    /// <param name="cbSize"></param>
    /// <returns></returns>
    [LibraryImport("user32.dll", SetLastError = true)]
    private static partial uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    private const uint INPUT_MOUSE = 0;
    private const uint MOUSEEVENTF_MOVE = 0x0001;
    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;

    /// <summary>
    /// マウスカーソルを相対的に移動させる
    /// </summary>
    /// <param name="dx">X方向の移動量（ピクセル）</param>
    /// <param name="dy">Y方向の移動量（ピクセル）</param>
    public static void MoveCursor(int dx, int dy)
    {
        INPUT input = new()
        {
            type = INPUT_MOUSE,
            mi = new MOUSEINPUT { dx = dx, dy = dy, dwFlags = MOUSEEVENTF_MOVE }
        };
        SendInput(1, [input], Marshal.SizeOf<INPUT>());
    }

    /// <summary>
    /// 左クリックのDOWNを実行
    /// </summary>
    public static void LeftClickDown()
    {
        INPUT input = new()
        {
            type = INPUT_MOUSE,
            mi = new MOUSEINPUT { dwFlags = MOUSEEVENTF_LEFTDOWN }
        };
        SendInput(1, [input], Marshal.SizeOf<INPUT>());
    }

    /// <summary>
    /// 左クリックのUPを実行
    /// </summary>
    public static void LeftClickUp()
    {
        INPUT input = new()
        {
            type = INPUT_MOUSE,
            mi = new MOUSEINPUT { dwFlags = MOUSEEVENTF_LEFTUP }
        };
        SendInput(1, [input], Marshal.SizeOf<INPUT>());
    }
}
