using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vault.Copy_Image
{
    public class CopyController
    {
        static CurrentClipboard clipboard;
      ImageDownloader downloader;

        public CopyController( )
        {
            clipboard = new CurrentClipboard();
            ClipboardNotification.ClipboardUpdate += ClipboardNotification_ClipboardUpdate;
            CurrentClipboard.ValueChanged += CurrentClipboard_ValueChanged;
            downloader = new ImageDownloader();
        }

        private void ClipboardNotification_ClipboardUpdate(object sender, EventArgs e)
        {
            GetImage();
        }


        public static CurrentClipboard GetImage()
        {
            try
            {

                if (Clipboard.ContainsText())
                {
                    if (Clipboard.GetText() != clipboard.text)
                    {
                        clipboard.text = Clipboard.GetText();
                        clipboard.PlainText = clipboard.text;
                    }
                }
                if (Clipboard.ContainsImage())
                {
                    if (Clipboard.GetImage() != clipboard.image)
                    {
                        clipboard.image = Clipboard.GetImage();
                    }
                }
                if (Clipboard.ContainsFileDropList())
                {
                    foreach (string s in Clipboard.GetFileDropList())
                    {
                        if (s.ToLower().Contains(".jpg") || s.ToLower().Contains(".bmp") || s.ToLower().Contains(".png"))
                        {
                            clipboard.image = ImageLoad.LoadBitmapImage(s);
                        }
                    }
                }

                return clipboard;
            }
            catch (Exception ee)
            {

                return new CurrentClipboard();
            }
        }


        private void CurrentClipboard_ValueChanged(object sender, EventArgs e)
        {
            if (GlobalClass.AutomaticCopyImage)
            {

                if ((sender as CurrentClipboard).datatype != CurrentClipboard.DataType.Empty)
                {
                    clipboard = (sender as CurrentClipboard);

                    if (clipboard.datatype == CurrentClipboard.DataType.Text)
                    {
                        string t = clipboard.text;
                        clipboard.Clear();
                        (sender as CurrentClipboard).Clear();

                        OpenFormWithLink(t);

                        //  downloadcontroller.AddNewDownloader(t);



                    }
                    else if (clipboard.datatype == CurrentClipboard.DataType.Image)
                    {
                        //  downloadcontroller.AddNewDownloader(clipboard.image);
                        OpenFormWithImage(clipboard.image);
                        clipboard.Clear();
                        (sender as CurrentClipboard).Clear();

                    }
                }
            }
        }


        private void OpenFormWithLink(string t)
        {
            if (downloader == null )
            {
                downloader = new ImageDownloader();
                downloader.AddLinkText(t);
              //  downloader.Show();
            }
            else
            {
                downloader.AddLinkText(t);
              //  downloader.Show();
            }
        }
        private void OpenFormWithImage(Image image)
        {
            if (downloader == null )
            {
                downloader = new ImageDownloader();
                downloader.AddImage(image);
               // downloader.Show();
            }
            else
            {
                downloader.AddImage(image);
              //  downloader.Show();
            }
        }

        internal void ClearDownloadList()
        {
            if (downloader != null)
                downloader.Clear();
        }
    }
}
