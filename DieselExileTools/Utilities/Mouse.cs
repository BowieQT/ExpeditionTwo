using System.Runtime.InteropServices;
using System.Security.Cryptography;
using SVector2 = System.Numerics.Vector2;
using ExileCore2;
using ExCore2 = global::ExileCore2;


namespace DieselExileTools.ExileCore2;

public static partial class DXT
{
    public class Mouse {
        public enum MouseEvents {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll")]
        public static extern bool BlockInput(bool block);

        

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out System.Drawing.Point point);
        public static System.Drawing.Point GetCursorPosition() {
            System.Drawing.Point point;
            GetCursorPos(out point);
            return point;
        }


        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);
        public static void moveMouse(SVector2 pos) {
            ExCore2.Input.SetCursorPos(pos);
        }
        public static void moveMouse(System.Drawing.Point pos) {
            ExCore2.Input.SetCursorPos(new SVector2(pos.X, pos.Y));
        }

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        public static void LeftDown() {
            mouse_event((int)MouseEvents.LeftDown, 0, 0, 0, 0);
        }
        public static void LeftUp() {
            mouse_event((int)MouseEvents.LeftUp, 0, 0, 0, 0);
        }
        public static void RightDown() {
            mouse_event((int)MouseEvents.RightDown, 0, 0, 0, 0);
        }
        public static void RightUp() {
            mouse_event((int)MouseEvents.RightUp, 0, 0, 0, 0);
        }

        public enum Buttons {
            Left = 1,
            Right = 2,
            Middle = 4,
            XButton1 = 8,
            XButton2 = 16
        }

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
        public static bool IsLeftButtonDown() => (GetAsyncKeyState(0x01) & 0x8000) != 0;
        public static bool IsRightButtonDown() => (GetAsyncKeyState(0x02) & 0x8000) != 0;
        public static bool IsMiddleButtonDown() => (GetAsyncKeyState(0x04) & 0x8000) != 0;
        /// <summary> check if any mouse button is currently held down (Left, Right, Middle) </summary>
        public static bool IsAnyButtonDown() => (GetAsyncKeyState(0x01) & 0x8000) != 0 || (GetAsyncKeyState(0x02) & 0x8000) != 0 || (GetAsyncKeyState(0x04) & 0x8000) != 0;
        public static bool IsMouseButtonDown(Buttons button) => (GetAsyncKeyState((int)button) & 0x8000) != 0;


    }
}
