using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vault
{
    public partial class ExtractImages : Form
    {
        List<Entity> EntityList;

        public string ActiveFolder { get; }
        public string ExtractionFolder { get; private set; }
        public int Completed { get; private set; }

        ImageFormat Imageformat;
        public ExtractImages(List<Entity> list,String ActiveFolderPath)
        {
            InitializeComponent();
            EntityList = list;
            this.ActiveFolder = ActiveFolderPath;
            Imageformat = ImageFormat.Jpeg;
            textBox1.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"Extracted Vault Images");
        }

        private void btn_extract_Click(object sender, EventArgs e)
        {
            if (btn_extract.Text == "Open Extraction Folder")
            {
                Process.Start(ExtractionFolder);
            }
            else
            {
                ExtractionFolder = textBox1.Text;
                if (!Directory.Exists(ExtractionFolder))
                    Directory.CreateDirectory(ExtractionFolder);
                if (Directory.Exists(ExtractionFolder))
                {
                    StartExtraction();
                }
                else
                {
                    MessageBox.Show("Please select other folder.", "Someting went wrong...");
                }
            }
        }

        private void StartExtraction()
        {
            btn_extract.Text = "Extracting...";
            btn_extract.Enabled = false;
            progressBar1.Visible = true;
            lbl_info.Visible = true;
            progressBar1.Maximum = EntityList.Count;
            backgroundWorker1.RunWorkerAsync();
         
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < EntityList.Count; i++)
            {
                Image img = EntityList[i].GetFullImage(ActiveFolder);
                if (img != null && img.Width > 1)
                {
                    if (Imageformat == ImageFormat.Png)
                        img.Save(Path.Combine(this.ExtractionFolder, "Image" + DateTime.Now.Second + DateTime.Now.Millisecond + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Year + DateTime.Now.Month + ".png"), Imageformat);

                    else if (Imageformat == ImageFormat.Jpeg)
                        img.Save(Path.Combine(this.ExtractionFolder, "Image" + DateTime.Now.Second + DateTime.Now.Millisecond + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Year + DateTime.Now.Month + ".jpg"), Imageformat);

                    backgroundWorker1.ReportProgress(i);
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Completed = (e.ProgressPercentage + 1);
            progressBar1.Value = Completed;
            this.Invalidate();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btn_extract.Enabled = true;
            btn_extract.Text = "Open Extraction Folder";
            progressBar1.Visible = false;
            lbl_info.Visible = false;
        }

        private void ExtractImages_Load(object sender, EventArgs e)
        {
            if (EntityList == null || EntityList.Count == 0)
                this.Dispose();
        }

        private void rb_jpeg_CheckedChanged(object sender, EventArgs e)
        {
            Imageformat = ImageFormat.Jpeg;

        }

        private void rb_png_CheckedChanged(object sender, EventArgs e)
        {
            Imageformat = ImageFormat.Png;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog filedialog = new FolderBrowserDialog();
            if(filedialog.ShowDialog()==DialogResult.OK)
            {
                textBox1.Text = filedialog.SelectedPath;
            }
        }
    }
}
