using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault
{
    public static class FullImage
    {


        public static Image image;
        public static RectangleF DisplayRect, SourceRect;
        public static Entity CurrentEntity;
        public static BackgroundWorker worker;
        public static bool RequestedNew = false;
        public static string Tags = "";
        static SizeF size;
        
        internal static void Paint(Graphics graphics)
        {
            graphics.DrawImage(image, DisplayRect, SourceRect, GraphicsUnit.Pixel);
            if (Tags != "")
            {
                size = graphics.MeasureString(Tags, GlobalClass.Smallfont);
                graphics.FillRectangle(GlobalClass.TagBGBrush, new RectangleF(0, GlobalClass.ParentBoundry.Height - 20, GlobalClass.ParentBoundry.Width, 20));
                graphics.DrawString(Tags, GlobalClass.TagFont, Brushes.White, new PointF(GlobalClass.ParentBoundry.Width/2-size.Width/2, GlobalClass.ParentBoundry.Height - 15));

            }
        }


        internal static void Reload(string Activefolder)
        {

            LoadEntity(CurrentEntity, Activefolder);

        }

        internal static void LoadEntity(Entity entity, string Activefolder)
        {
            if (entity == null)
                return;
            ActiveFolderPath = Activefolder;
            CurrentEntity = entity;
            Tags = entity.Tags;
            if (Tags != null)
                Tags = Tags.Replace(";", " ");
            Initialize();
            if (worker == null)
            {
                worker = new BackgroundWorker();
                worker.DoWork += Worker_DoWork;
                worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            }
            if (worker.IsBusy)
            {
                RequestedNew = true;
            }
            else
            {
                worker.RunWorkerAsync(CurrentEntity);
            }

        }

        private static void Initialize()
        {
            image = CurrentEntity.GetThumbnail() ;
            SourceRect = new RectangleF(0, 0, image.Width, image.Height);           
            DisplayRect = new RectangleF(GlobalClass.ParentBoundry.Width / 2 - image.Width / 2, GlobalClass.ParentBoundry.Height / 2 - image.Height / 2, image.Width, image.Height);

        }

        private static void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GlobalClass.Invalidate();
            if(RequestedNew)
            {             
                worker.RunWorkerAsync(CurrentEntity);
            }
        }

        private static void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            RequestedNew = false;
            image = ((Entity)e.Argument).GetFullImage(ActiveFolderPath);
            SourceRect = new RectangleF(0, 0, image.Width, image.Height);
            Size size = SD.Images.Resize.GetResizedSize(image.Size, GlobalClass.ParentBoundry);
            DisplayRect = new RectangleF(GlobalClass.ParentBoundry.Width / 2 - size.Width / 2, GlobalClass.ParentBoundry.Height / 2 - size.Height / 2, size.Width, size.Height);

        }

        internal static void MouseWheel(int delta)
        {
            if (delta > 0)
                DisplayRect = SD.Images.Zoom.ZoomImage(DisplayRect, CurrentMouse, .1f);
            else
                DisplayRect = SD.Images.Zoom.ZoomImage(DisplayRect, CurrentMouse, -.1f);


        }
        static bool drag = false;
        static Point Initial, CurrentMouse;

        public static string ActiveFolderPath { get; private set; }

        internal static void MouseMove(Point location)
        {
            CurrentMouse = location;
            if (drag)
            {
                DisplayRect = new RectangleF(DisplayRect.X + location.X - Initial.X, DisplayRect.Y + location.Y - Initial.Y, DisplayRect.Width, DisplayRect.Height);
                Initial = location;
            }
        }

        internal static void MouseDown(Point location)
        {
            Initial = location;
            drag = true;
        }

        internal static void MouseUp(Point location)
        {
            drag = false;

        }

        internal static void MouseDoubleClick(Point location)
        {
            float zfactor = SourceRect.Width / DisplayRect.Width;
            if (DisplayRect.Width < SourceRect.Width)
            {
                DisplayRect = SD.Images.Zoom.ZoomImage(DisplayRect, location, zfactor);
            }
            else
            {
                Size size = SD.Images.Resize.GetResizedSize(image.Size, GlobalClass.ParentBoundry);
                DisplayRect = new RectangleF(GlobalClass.ParentBoundry.Width / 2 - size.Width / 2, GlobalClass.ParentBoundry.Height / 2 - size.Height / 2, size.Width, size.Height);

            }
        }

      
    }
}
