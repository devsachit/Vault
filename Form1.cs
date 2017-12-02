using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vault
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            GlobalClass.ParentForm = this;
            GlobalClass.Initialize();
            Controller.OpenSelectedNodeController();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            Controller.MouseMove(e.Location);

        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            Controller.MouseDown(e.Location, e.Button);

        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            Controller.MouseUp(e.Location);

        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Controller.MouseDoubleClick(e.Location,e.Button);
        }


        protected override void OnMouseWheel(MouseEventArgs e)
        {
            Controller.MouseWheel(e.Delta,e.Location);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Controller.KeyDown(sender, e);
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Controller.KeyUp(sender, e);
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
           
          //  if (GlobalClass.HighGraphicsQuality)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
          //      e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
          ////  e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
             e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
           //    e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
           
            }
                Controller.Paint(e.Graphics);
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIcon1.Dispose();
            GlobalClass.DoActionBeforeExiting();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
          
        }

        private void backToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Controller.GoBack();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            notifyIcon1.ShowBalloonTip(1000, "Vault", "Vault is running.", ToolTipIcon.Info);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Controller.ExitProgram();
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            if (this.Visible)
                openToolStripMenuItem.Text = "Hide";
            else
                openToolStripMenuItem.Text = "Open";

            if (GlobalClass.AutomaticCopyImage)
                autoDownloadImageToolStripMenuItem.Checked = true;
            else
                autoDownloadImageToolStripMenuItem.Checked = false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Visible)
                Controller.Hide();
            else
                this.Show();

        }

        private void autoDownloadImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GlobalClass.AutomaticCopyImage)
                GlobalClass.AutomaticCopyImage = false;
            else
                GlobalClass.AutomaticCopyImage = true;
            this.Invalidate();
        }
    }
}
