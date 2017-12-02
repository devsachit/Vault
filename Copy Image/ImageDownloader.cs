using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vault.Copy_Image
{
    //public partial class ImageDownloader : Form
    //{
    //    public static List<string> LinksToDownload;
    //    public static List<Download> Downloades;
   
    //    private Image thumbnail;

    //    public ImageDownloader()
    //    {
    //        InitializeComponent();
          
    //        Download.DownloadCompleted += Download_DownloadCompleted;
    //        this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
    //        this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
    //        this.SetStyle(ControlStyles.ResizeRedraw, true);

    //        LinksToDownload = new List<string>();
    //        Downloades = new List<Download>();

    //        //   Rects = new List<RectangleF>();
    //    }
    //    int count = 0;
    //    float totalPercent = 0;
    //    private void Download_DownloadCompleted(object sender, EventArgs e)
    //    {
    //        int total = Downloades.Count;
    //        int completed = 0;
    //        for (int i = 0; i < Downloades.Count; i++)
    //        {
    //            if (Downloades[i].Status == "Download Completed")
    //            {
    //                completed++;
    //            }
    //        }
    //        if (total == 0 || completed == 0)
    //            totalPercent = 0;
    //        else
    //            totalPercent = (float)completed / (float)total;

    //        if (total == 0)
    //            GlobalClass.Notify("Another image is fetched.");
    //        else
    //            GlobalClass.Notify(completed + " of " + total + " images are fetched.");

    //    }

    //    internal void AddLinkText(string t)
    //    {
    //        if (!LinksToDownload.Contains(t))
    //        {
    //            LinksToDownload.Add(t);
    //            Download d = new Download(t);
    //            Downloades.Add(d);
    //        }
    //    }
    //    internal void AddImage(Image image)
    //    {
    //        LinksToDownload.Add("Raw Image...");
    //        Download d = new Download(image);
    //        Downloades.Add(d);

    //    }


    //    private void ImageDownloader_Load(object sender, EventArgs e)
    //    {
    //        this.Opacity = 0;
    //    }

    //    internal void Clear()
    //    {
    //        LinksToDownload = new List<string>();
    //        Downloades = new List<Download>();
    //    }
    //}

    public partial class ImageDownloader 
    {
        public static List<string> LinksToDownload;
        public static List<Download> Downloades;

        private Image thumbnail;

        public ImageDownloader()
        {
           
            Download.DownloadCompleted += Download_DownloadCompleted;
         
            LinksToDownload = new List<string>();
            Downloades = new List<Download>();

            //   Rects = new List<RectangleF>();
        }
        int count = 0;
        float totalPercent = 0;
        private void Download_DownloadCompleted(object sender, EventArgs e)
        {
            int total = Downloades.Count;
            int completed = 0;
            for (int i = 0; i < Downloades.Count; i++)
            {
                if (Downloades[i].Status == "Download Completed")
                {
                    completed++;
                }
            }
            if (total == 0 || completed == 0)
                totalPercent = 0;
            else
                totalPercent = (float)completed / (float)total;

            if (total == 0)
                GlobalClass.Notify("Another image is fetched.");
            else
                GlobalClass.Notify(completed + " of " + total + " images are fetched.");

        }

        internal void AddLinkText(string t)
        {
            if (!LinksToDownload.Contains(t))
            {
                LinksToDownload.Add(t);
                Download d = new Download(t);
                Downloades.Add(d);
            }
        }
        internal void AddImage(Image image)
        {
            LinksToDownload.Add("Raw Image...");
            Download d = new Download(image);
            Downloades.Add(d);

        }


        internal void Clear()
        {
            LinksToDownload = new List<string>();
            Downloades = new List<Download>();
        }
    }

    public class Download
    {
        public string Link { get; set; }
        public Image image { get; set; }
        public Image Thumbnail { get; private set; }
        public string Status { get; set; }
        BackgroundWorker worker;
        public static event EventHandler DownloadCompleted;

        public Download(string link )
        {
       
            Status = "Downloading...";
            this.Link = link;
            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            Thumbnail = Properties.Resources.fav;
            StartDownload();
        }

        public Download(Image image)
        { 
            Status = "Download Completed";
            this.Link = "Raw Image";
            this.image = image;
            Thumbnail = ImageLoad.GetThumbnailImage(image, 100);
            Vault.Controller.AddImageToDownloadFolder(this.image);
            this.image = null;
            SD.Garbage.ClearRAM.Clear();
            DownloadCompleted?.Invoke(null, EventArgs.Empty);

        }

        private void StartDownload()
        {
            if (CheckIFUri(this.Link))
            {
                worker.RunWorkerAsync();

            }
        }



        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            FetchImage();
        }
        private void FetchImage()
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(Link);
                HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream stream = httpWebReponse.GetResponseStream();
                this.image = Image.FromStream(stream);
                Thumbnail = ImageLoad.GetThumbnailImage(image, 100);
                Controller.AddImageToDownloadFolder(this.image);
                this.image = null;
                SD.Garbage.ClearRAM.Clear();
                Status = "Download Completed";
            }
            catch (Exception ee)
            {

                image = null;
                Thumbnail = new Bitmap(1, 1);

                Status = "Download Failed" + " " + ee.Message;
            }

        }
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (image != null)
                Status = "Download Completed";

            DownloadCompleted?.Invoke(null, EventArgs.Empty);

        }


        private bool CheckIFUri(string text)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(text, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }
    }
}
