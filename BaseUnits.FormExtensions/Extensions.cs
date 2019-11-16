using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BaseUnits.FormExtensions
{
    public static class ControlExtensions
    {
        [DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);

        public static void Suspend(this Control control)
        {
            LockWindowUpdate(control.Handle);
        }

        public static void Resume(this Control control)
        {
            LockWindowUpdate(IntPtr.Zero);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const int WM_VSCROLL = 0x115;
        private const int SB_BOTTOM = 7;

        public static void ScrollToBottom(this Control control)
        {
            PostMessage(control.Handle, WM_VSCROLL, (IntPtr)SB_BOTTOM, IntPtr.Zero);
        }

        public static void DoubleBuffered(this Control c)
        {
            var doubleBufferPropertyInfo = c.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            doubleBufferPropertyInfo.SetValue(c, true, null);
        }

        public static void SetDoubleBuffered(this Control c)
        {
            foreach (var it in c.Controls)
            {
                (it as Control).SetDoubleBuffered();
            }

            c.DoubleBuffered();
        }

        public static string SetLength(this string s, int len, string appendWhenExceed = "..")
        {
            return len > 0 && s.Length > len ? s.Substring(0, len) + appendWhenExceed : s;
        }
    }
}
