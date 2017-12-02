using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vault.Copy_Image
{
    public class CurrentClipboard
    {
        public static event EventHandler ValueChanged;
        private string _text;
        private Image _image;


        public enum DataType
        {
            Image,
            Text,
            Empty

        }
        public DataType datatype = DataType.Text;
        public string text { get { return _text; } set { _text = value; datatype = DataType.Text; ValueChanged(this, EventArgs.Empty); } }
        public Image image { get { return _image; } set { _image = value; datatype = DataType.Image; ValueChanged(this, EventArgs.Empty); } }

        public CurrentClipboard()
        {
            _text = string.Empty;
            _image = new Bitmap(1, 1);
        }

        public override string ToString()
        {
            if (_text != null && _text.Length > 0)
                return text;
            else if (_image != null)
                return "Got Image with width" + _image.Width + " and height " + _image.Height;
            else
                return "Nothing in Clipboard";
        }

        public string SavedFilePath { get; set; }

        public string PlainText { get; set; }

        internal void Clear()
        {
            _image = null;
            _text = string.Empty;
            datatype = DataType.Empty;
            SD.Garbage.ClearRAM.Clear();
        }
    }

    sealed class ClipboardNotification
    {
        /// <summary>
        /// Occurs when the contents of the clipboard is updated.
        /// </summary>
        public static event EventHandler ClipboardUpdate;

        private static NotificationForm _form = new NotificationForm();

        /// <summary>
        /// Raises the <see cref="ClipboardUpdate"/> event.
        /// </summary>
        /// <param name="e">Event arguments for the event.</param>
        private static void OnClipboardUpdate(EventArgs e)
        {
            var handler = ClipboardUpdate;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        /// <summary>
        /// Hidden form to recieve the WM_CLIPBOARDUPDATE message.
        /// </summary>
        private class NotificationForm : Form
        {
            public NotificationForm()
            {
                NativeMethods.SetParent(Handle, NativeMethods.HWND_MESSAGE);
                NativeMethods.AddClipboardFormatListener(Handle);
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == NativeMethods.WM_CLIPBOARDUPDATE)
                {
                    OnClipboardUpdate(null);
                }

                base.WndProc(ref m);
            }
        }
    }

    internal static class NativeMethods
    {
        // See http://msdn.microsoft.com/en-us/library/ms649021%28v=vs.85%29.aspx
        public const int WM_CLIPBOARDUPDATE = 0x031D;
        public const int WM_CHANGECBCHAIN = 0x030D;
        public const int WM_ASKCBFORMATNAME = 0x030C;
        public const int WM_DESTROYCLIPBOARD = 0x0307;
        public const int WM_DRAWCLIPBOARD = 0x0308;
        public const int WM_HSCROLLCLIPBOARD = 0x030E;
        public const int WM_PAINTCLIPBOARD = 0x0309;
        public const int WM_RENDERALLFORMATS = 0x0306;
        public const int WM_RENDERFORMAT = 0x0305;
        public const int WM_SIZECLIPBOARD = 0x030B;
        public const int WM_VSCROLLCLIPBOARD = 0x030A;


        public static IntPtr HWND_MESSAGE = new IntPtr(-3);

        // See http://msdn.microsoft.com/en-us/library/ms632599%28VS.85%29.aspx#message_only
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);

        // See http://msdn.microsoft.com/en-us/library/ms633541%28v=vs.85%29.aspx
        // See http://msdn.microsoft.com/en-us/library/ms649033%28VS.85%29.aspx
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
    }
}
