using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vault
{
    public partial class AllImages : Form
    {
        string Activefolder;
        List<Entity> Entities;
        List<Node> Nodes;
        List<RectangleF> EntityRects,Noderects;

        public float ShiftY { get; private set; }

        public AllImages(string ActiveFolder)
        {
            InitializeComponent();
            this.Activefolder = ActiveFolder;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
          
            LoadEntities();
            SetRectangles();
            this.Invalidate();
            backgroundWorker1.RunWorkerAsync();
         
       
        }

        private void SetRectangles()
        {
            int x = 10, y = 10;
            EntityRects = new List<RectangleF>();
            Noderects = new List<RectangleF>();
            for (int i=0;i<Nodes.Count;i++)
            {
                Noderects.Add(new RectangleF(x, y, GlobalClass.ThumbnailSize.Width+50, GlobalClass.ThumbnailSize.Height ));
                x += GlobalClass.ThumbnailSize.Width+50 ;
                if (x + GlobalClass.ThumbnailSize.Width +50 > this.Width)
                {
                    x = 10;
                    y += GlobalClass.ThumbnailSize.Height;
                }
            }
            for (int i = 0; i < Entities.Count; i++)
            {
                EntityRects.Add(new RectangleF(x, y, GlobalClass.ThumbnailSize.Width+50, GlobalClass.ThumbnailSize.Height));
                x += GlobalClass.ThumbnailSize.Width+50;
                if (x + GlobalClass.ThumbnailSize.Width+50 > this.Width)
                {
                    x = 10;
                    y += GlobalClass.ThumbnailSize.Height;
                }
            }
   
        }

        private void LoadEntities()
        {
            Entities = new List<Entity>();
            Nodes = new List<Node>();
            string[] allfiles = Directory.GetFiles(Activefolder);
            for (int i = 0; i < allfiles.Length; i++)
            {
                if (allfiles[i].Contains("Image") && !allfiles[i].Contains("Thumbnail"))
                    Entities.Add(new Entity(new FileInfo( allfiles[i]).Name));
                else if(allfiles[i].Contains("Thumbnail"))
                {

                }
                else if(allfiles[i].Contains("Cover"))
                {

                }
                else if(allfiles[i].Contains("RemainingTasks"))
                {

                }
                else if (allfiles[i].Contains("CustomRootFolders"))
                {

                }
                else
                {
                    Nodes.Add(new Node(Activefolder, new FileInfo(allfiles[i]).Name, "", ""));
                }
            }
        }


        SizeF size;
        private PointF scrollpoint;

        private void AllImages_Paint(object sender, PaintEventArgs e)
        {
           
            for (int i = 0; i < Noderects.Count; i++)
            {

                float y = 0;
                if (Noderects[i].Y + ShiftY <= this.Height && Noderects[i].Y + Noderects[i].Height + ShiftY >= 0)
                {
                    e.Graphics.DrawString(Nodes[i].Name, GlobalClass.PropertyNormalFont, Brushes.Gray, GetShiftedRect(Noderects[i]));
                    size = e.Graphics.MeasureString(Nodes[i].Name, GlobalClass.PropertyNormalFont, (int)Noderects[i].Width); y = size.Height;

                    e.Graphics.DrawString(Nodes[i].DisplayName, GlobalClass.PropertyBoldFont, Brushes.Black, new RectangleF(Noderects[i].X, Noderects[i].Y + ShiftY + y, Noderects[i].Width, Noderects[i].Height - y));
                    size = e.Graphics.MeasureString(Nodes[i].DisplayName, GlobalClass.PropertyBoldFont, (int)Noderects[i].Width); y += size.Height;

                    e.Graphics.DrawString("Contains "+Nodes[i].Entities.Count + " images, "+ Nodes[i].Nodes.Count +" folders", GlobalClass.PropertyNormalFont, Brushes.Black, new RectangleF(Noderects[i].X, Noderects[i].Y + ShiftY + y, Noderects[i].Width, Noderects[i].Height - y));
                    size = e.Graphics.MeasureString("Contains " + Nodes[i].Entities.Count + " images, " + Nodes[i].Nodes.Count + " folders", GlobalClass.PropertyNormalFont, (int)Noderects[i].Width); y += size.Height;
                    
                    if (Nodes[i].ParentName!=null)
                    e.Graphics.DrawString("Parent: " + Nodes[i].ParentName, GlobalClass.PropertyBoldFont, Brushes.Gray, new RectangleF(Noderects[i].X, Noderects[i].Y + ShiftY + y, Noderects[i].Width, Noderects[i].Height - y));

                }
            }
            for (int i = 0; i < EntityRects.Count; i++)
            {
                float y = 0;
                if (EntityRects[i].Y + ShiftY <= this.Height && EntityRects[i].Y + EntityRects[i].Height + ShiftY >= 0)
                {
                    e.Graphics.DrawString(Entities[i].ImageFileName,GlobalClass.PropertyNormalFont,Brushes.Black, GetShiftedRect(EntityRects[i]));
                    size = e.Graphics.MeasureString(Entities[i].ImageFileName, GlobalClass.PropertyNormalFont, (int)EntityRects[i].Width); y = size.Height;
                    if (Entities[i].ParentName != "")
                        e.Graphics.DrawString("Parent: " + Entities[i].ParentName, GlobalClass.PropertyBoldFont, Brushes.Black, new RectangleF(EntityRects[i].X, EntityRects[i].Y + ShiftY + y, EntityRects[i].Width, EntityRects[i].Height - y));

                }
            }

            e.Graphics.DrawLine(new Pen(Brushes.Black, 2f), scrollpoint, new PointF(scrollpoint.X, scrollpoint.Y + 10));
            
        }

        private RectangleF GetShiftedRect(RectangleF rectangleF)
        {
            return new RectangleF(rectangleF.X, rectangleF.Y + ShiftY, rectangleF.Width, rectangleF.Height);
        }

      

        private void AllImages_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Dispose();
        }

        private void AllImages_MouseMove(object sender, MouseEventArgs e)
        {
            this.Invalidate();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < Nodes.Count; i++)
                Nodes[i].Read(false, false);
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            for(int i=0; i<Entities.Count;i++)
            {
               for(int j=0;j<Nodes.Count;j++)
                {
                    for (int k = 0; k < Nodes[j].Entities.Count; k++)
                    {
                        if (Nodes[j].Entities[k].ImageFileName == Entities[i].ImageFileName)
                        {
                            Entities[i].ParentName = Nodes[j].DisplayName;
                            break;
                        }
                    }
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            backgroundWorker2.RunWorkerAsync();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e.Delta > 0)
                ShiftY += GlobalClass.ParentBoundry.Height-200;
            else
                ShiftY -= GlobalClass.ParentBoundry.Height - 200;
            float y = GlobalClass.ParentBoundry.Height * Math.Abs(ShiftY / EntityRects[EntityRects.Count - 1].Y);
            scrollpoint = new PointF(GlobalClass.ParentBoundry.Width - 3, y);
            this.Invalidate();
        }
    }
}
