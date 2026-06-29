using System.Runtime.InteropServices;

namespace DieselExileTools.ExileCore2;
public static partial class DXT {
    public class Keyboard
    {
        public enum Keys {

            // Common Keys
            Back = 8,           // The BACKSPACE key.
            Tab = 9,            // The TAB key.
            LineFeed = 10,      // The LINEFEED key.
            Clear = 12,         // The CLEAR key.
            Return = 13,        // The RETURN key. (Same as Enter)
            Enter = 13,         // The ENTER key.
            ShiftKey = 16,      // The SHIFT key.
            ControlKey = 17,    // The CTRL key.
            Menu = 18,          // The ALT key.
            Pause = 19,         // The PAUSE key.
            //Capital = 20,       // The CAPS LOCK key. (Same as CapsLock)
            //CapsLock = 20,      // The CAPS LOCK key.
            Escape = 27,        // The ESCAPE key.
            Space = 32,         // The SPACEBAR key.
            PageUp = 33,        // The PAGE UP key. (Same as Prior)
            Prior = 33,         // The PAGE UP key.
            PageDown = 34,      // The PAGE DOWN key. (Same as Next)
            Next = 34,          // The PAGE DOWN key.
            End = 35,           // The END key.
            Home = 36,          // The HOME key.
            Left = 37,          // The LEFT ARROW key.
            Up = 38,            // The UP ARROW key.
            Right = 39,         // The RIGHT ARROW key.
            Down = 40,          // The DOWN ARROW key.
            Select = 41,        // The SELECT key.
            Print = 42,         // The PRINT key.
            Execute = 43,       // The EXECUTE key.
            Snapshot = 44,      // The PRINT SCREEN key. (Same as PrintScreen)
            PrintScreen = 44,   // The PRINT SCREEN key.
            Insert = 45,        // The INS key.
            Delete = 46,        // The DEL key.
            Help = 47,          // The HELP key.

            // Number Keys (0-9)
            D0 = 48, D1 = 49, D2 = 50, D3 = 51, D4 = 52,
            D5 = 53, D6 = 54, D7 = 55, D8 = 56, D9 = 57,

            // Letter Keys (A-Z)
            A = 65, B = 66, C = 67, D = 68, E = 69, F = 70,
            G = 71, H = 72, I = 73, J = 74, K = 75, L = 76,
            M = 77, N = 78, O = 79, P = 80, Q = 81, R = 82,
            S = 83, T = 84, U = 85, V = 86, W = 87, X = 88,
            Y = 89, Z = 90,

            // Numeric Keypad Keys
            NumPad0 = 96, NumPad1 = 97, NumPad2 = 98, NumPad3 = 99, NumPad4 = 100,
            NumPad5 = 101, NumPad6 = 102, NumPad7 = 103, NumPad8 = 104, NumPad9 = 105,
            Multiply = 106, Add = 107, Separator = 108, Subtract = 109, Decimal = 110, Divide = 111,

            // Function Keys
            F1 = 112, F2 = 113, F3 = 114, F4 = 115, F5 = 116, F6 = 117,
            F7 = 118, F8 = 119, F9 = 120, F10 = 121, F11 = 122, F12 = 123,
            F13 = 124, F14 = 125, F15 = 126, F16 = 127, F17 = 128, F18 = 129,
            F19 = 130, F20 = 131, F21 = 132, F22 = 133, F23 = 134, F24 = 135,

            // Other Keys
            //NumLock = 144,      // The NUM LOCK key.
            //Scroll = 145,       // The SCROLL LOCK key.
            LShiftKey = 160,    // The left SHIFT key.
            RShiftKey = 161,    // The right SHIFT key.
            LControlKey = 162,  // The left CTRL key.
            RControlKey = 163,  // The right CTRL key.
            LMenu = 164,        // The left ALT key.
            RMenu = 165,        // The right ALT key.
            //ProcessKey = 229,   // The PROCESS KEY key.
            //Attn = 246,         // The ATTN key.
            //Crsel = 247,        // The CRSEL key.
            //Exsel = 248,        // The EXSEL key.
            //EraseEof = 249,     // The ERASE EOF key.
            //Play = 250,         // The PLAY key.
            //Zoom = 251,         // The ZOOM key.
            //Pa1 = 253,          // The PA1 key.
            //OemClear = 254      // The OEM CLEAR key.
        }

        private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const int KEYEVENTF_KEYUP = 0x0002;

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
        public static bool IsKeyDown(Keys key) {
            return (GetAsyncKeyState((int)key) & 0x8000) != 0;
        }

        [DllImport("user32.dll")]
        private static extern uint keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        private static bool IsExtendedKey(Keys key) {
            // List of extended keys
            return key == Keys.Up || key == Keys.Down || key == Keys.Left || key == Keys.Right ||
                   key == Keys.Insert || key == Keys.Delete || key == Keys.Home || key == Keys.End ||
                   key == Keys.PageUp || key == Keys.PageDown;
        }

        public static void KeyDown(Keys key) {
            if (!IsKeyDown(key)) {
                //DBug.Log($"Key down: {key}");
                keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
            }
        }

        public static void KeyUp(Keys key) {
            int flags = IsExtendedKey(key) ? KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP : KEYEVENTF_KEYUP;
            if (IsKeyDown(key)) {
                keybd_event((byte)key, 0, flags, 0); // Release key
                //DBug.Log($"Key up: {key}");
            }

            // If the key is a shift key, release all shift keys
            if (key == Keys.LShiftKey || key == Keys.RShiftKey || key == Keys.ShiftKey) {
                keybd_event((byte)Keys.LShiftKey, 0, KEYEVENTF_KEYUP, 0);
                keybd_event((byte)Keys.RShiftKey, 0, KEYEVENTF_KEYUP, 0);
                keybd_event((byte)Keys.ShiftKey, 0, KEYEVENTF_KEYUP, 0);
            }

            // Release all control keys if any control key is released
            if (key == Keys.LControlKey || key == Keys.RControlKey || key == Keys.ControlKey) {
                keybd_event((byte)Keys.LControlKey, 0, KEYEVENTF_KEYUP, 0);
                keybd_event((byte)Keys.RControlKey, 0, KEYEVENTF_KEYUP, 0);
                keybd_event((byte)Keys.ControlKey, 0, KEYEVENTF_KEYUP, 0);
            }

            // Release all alt keys if any alt key is released
            if (key == Keys.LMenu || key == Keys.RMenu || key == Keys.Menu) {
                keybd_event((byte)Keys.LMenu, 0, KEYEVENTF_KEYUP, 0);
                keybd_event((byte)Keys.RMenu, 0, KEYEVENTF_KEYUP, 0);
                keybd_event((byte)Keys.Menu, 0, KEYEVENTF_KEYUP, 0);
            }

        }

        public static void ReleaseAllKeys() {
            foreach (Keys key in Enum.GetValues(typeof(Keys))) {
                //if (key == Keys.None || key == Keys.Modifiers) continue; // Skip non-keys
                KeyUp(key);
            }
        }

        public static void SendKeys(string text) {
            foreach (char c in text) {
                // Handle uppercase letters (send Shift)
                bool shift = char.IsUpper(c) || char.IsPunctuation(c) || char.IsSymbol(c);

                Keys key;
                if (char.IsLetterOrDigit(c)) {
                    key = (Keys)char.ToUpper(c);
                }
                else {
                    // Handle space and common punctuation
                    switch (c) {
                        case ' ': key = Keys.Space; break;
                        case '\n': key = Keys.Return; break;
                        case '\r': key = Keys.Return; break;
                        default:
                            // Add more punctuation handling as needed
                            continue;
                    }
                }

                if (shift)
                    KeyDown(Keys.ShiftKey);

                KeyDown(key);
                KeyUp(key);

                if (shift)
                    KeyUp(Keys.ShiftKey);

                Thread.Sleep(10); // Small delay between releases
            }
        }
        public static void SendKeyCombo(params Keys[] keys) {
            // Press all keys (modifiers first)
            foreach (var key in keys) KeyDown(key);

            // Release all keys (reverse order)
            for (int i = keys.Length - 1; i >= 0; i--) KeyUp(keys[i]);
        }

    }
}
