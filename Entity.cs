using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault
{
    [Serializable]
    public class Entity
    {
        public string ImageFileName { get; set; }
        public string ThumbnailFileName { get; set; }
        public string Tags { get; set; }
        public string OldParentName { get; set; }
        public string OldParentDisplayName { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsShortCut { get; set; }
        public bool IsFavorite { get;  set; }

        [NonSerialized]
        public string ParentName = "";

        [NonSerialized]
        Picture picture;
        [NonSerialized]
        public bool FileFound = true;


        public Entity(String ImagePath, string ActiveFolderPath)
        {
            string Random = GlobalClass.GetRandomString();
            this.ImageFileName = "Image" + Random;
            this.ThumbnailFileName = "Thumbnail" + ImageFileName;
            picture = new Picture(ImageFileName, ThumbnailFileName);
            SetPicture(ImagePath, ActiveFolderPath);
            Tags = Metadata.ReadMetadata(ImagePath);
            OldParentName = null;
            IsShortCut = false;
            IsDeleted = false;
            IsFavorite = false;
        }
        public Entity(Image image, string ActiveFolderPath)
        {
            string Random = GlobalClass.GetRandomString();
            this.ImageFileName = "Image" + Random;
            this.ThumbnailFileName = "Thumbnail" + ImageFileName;
            picture = new Picture(ImageFileName, ThumbnailFileName);
            SetPicture(image, ActiveFolderPath);
            Tags = "";
            OldParentName = null;
            IsShortCut = false;
            IsDeleted = false;
            IsFavorite = false;
        }
        public Entity(Entity sourceentity, string ActiveFolder)
        {
            this.ImageFileName = sourceentity.ImageFileName;
            this.ThumbnailFileName = sourceentity.ThumbnailFileName;
            this.Tags = sourceentity.Tags;
            this.OldParentName = sourceentity.OldParentName;
            this.OldParentDisplayName = sourceentity.OldParentDisplayName;
            this.IsDeleted = sourceentity.IsDeleted;
            this.picture = new Picture(this.ImageFileName, this.ThumbnailFileName);
            this.picture.LoadThumbnail(ActiveFolder);
            this.IsDeleted = false;
            this.IsShortCut = true;
            this.IsFavorite = sourceentity.IsFavorite;
        }
        public Entity(Entity sourceentity, string ActiveFolder,bool IsFavorite)
        {
            this.ImageFileName = sourceentity.ImageFileName;
            this.ThumbnailFileName = sourceentity.ThumbnailFileName;
            this.Tags = sourceentity.Tags;
            this.OldParentName = sourceentity.OldParentName;
            this.OldParentDisplayName = sourceentity.OldParentDisplayName;
            this.IsDeleted = sourceentity.IsDeleted;
            this.picture = new Picture(this.ImageFileName, this.ThumbnailFileName);
            this.picture.LoadThumbnail(ActiveFolder);
            this.IsDeleted = sourceentity.IsDeleted;
            this.IsShortCut = true;
            this.IsFavorite = IsFavorite;
        }

        public Entity(string imagename)
        {
            this.ImageFileName = imagename;
            this.ThumbnailFileName = "Thumbnail" + imagename;
            picture = new Picture(ImageFileName, ThumbnailFileName);
        }

        private void SetPicture(string imagePath, string activefolder)
        {
            picture.SaveImage(imagePath, activefolder);
        }


        private void SetPicture(Image image, string activefolder)
        {
            picture.SaveImage(image, activefolder);
        }
        public void LoadPicture(string activefolder)
        {
            if (picture == null)
                picture = new Picture(ImageFileName, ThumbnailFileName);
         FileFound=   picture.LoadThumbnail(activefolder);
        }

        public Image GetThumbnail()
        {
            return picture.Thumbnail;
        }
        public Image GetFullImage(string activefolder)
        {
            if (picture == null)
                LoadPicture(activefolder);

            return picture.GetFullImage(activefolder);
        }
        [NonSerialized]
        SizeF size;

        [NonSerialized]
        GraphicsPath path;
    

        internal void Paint(Graphics graphics, PointF location)
        {
            path = GlobalClass.GetRoundBoundry(new RectangleF(location.X+1,location.Y+1, GlobalClass.ThumbnailSize.Width-2,GlobalClass.ThumbnailSize.Height-2),GlobalClass.RoundCornorRadius);
            graphics.SetClip(path);           
            graphics.DrawImage(GetThumbnail(), location);
            PaintShortCutInfo(graphics, location);
            PaintTags(graphics, location);
            graphics.ResetClip();
            graphics.DrawPath(GlobalClass.BorderColorPen, path);
        }
               

        private void PaintTags(Graphics graphics, PointF location)
        {
            if (Tags.Length > 0 && FileFound)
            {
                size = graphics.MeasureString(Tags, GlobalClass.TagFont, GlobalClass.ThumbnailSize.Width - 5);
                graphics.FillRectangle(GlobalClass.TagBGBrush, new RectangleF(location.X , location.Y + GlobalClass.ThumbnailSize.Height - size.Height - 5, GlobalClass.ThumbnailSize.Width, size.Height + 5));
                graphics.DrawString(Tags, GlobalClass.TagFont, Brushes.White, new RectangleF( location.X + GlobalClass.ThumbnailSize.Width / 2 - size.Width / 2, location.Y + GlobalClass.ThumbnailSize.Height - size.Height -5/2,GlobalClass.ThumbnailSize.Width-5,size.Height+5));
            }
        }

        private void PaintShortCutInfo(Graphics graphics, PointF location)
        {
            if (IsShortCut)
            {
                if (FileFound)
                {
                    size = graphics.MeasureString("Shortcut", GlobalClass.SmallfontBold);
                    graphics.FillPath(GlobalClass.ShortCutBGBrush, GlobalClass.GetRoundBoundry(new RectangleF(location.X + 5, location.Y + 5, size.Width + 3, size.Height + 3), 3f));
                    graphics.DrawString("Shortcut", GlobalClass.SmallfontBold, Brushes.White, location.X + 5 + 1.5f, location.Y + 5 + 1.5f);

                }
                else
                {
                    graphics.DrawString("Original File has been Deleted", GlobalClass.SmallfontBold, Brushes.White, new RectangleF(location.X + 5, location.Y + 5, GlobalClass.ThumbnailSize.Width - 4, GlobalClass.ThumbnailSize.Height - 4));
                    graphics.DrawString("Original Folder:", GlobalClass.Smallfont, Brushes.WhiteSmoke, new RectangleF(location.X + 5, location.Y + 25, GlobalClass.ThumbnailSize.Width - 4, GlobalClass.ThumbnailSize.Height - 4));
                    graphics.DrawString(OldParentDisplayName, GlobalClass.SmallfontBold, Brushes.Gold, new RectangleF(location.X + 5, location.Y + 38, GlobalClass.ThumbnailSize.Width - 4, GlobalClass.ThumbnailSize.Height - 4));
                    graphics.DrawString("To Delete this broken shortcut, Select and press Delete.", GlobalClass.Smallfont, Brushes.SkyBlue, new RectangleF(location.X + 5, location.Y + 75, GlobalClass.ThumbnailSize.Width - 4, GlobalClass.ThumbnailSize.Height - 4));
                    
                }

            }
        }

        internal bool CopyFilesToFolder(string sourceactivepath, string pasteactivepath)
        {
            if (!Directory.Exists(pasteactivepath))
                Directory.CreateDirectory(pasteactivepath);

            if (File.Exists(Path.Combine(sourceactivepath, ThumbnailFileName)))
            {
                if (File.Exists(Path.Combine(pasteactivepath, ThumbnailFileName)))
                    return false;
                File.Copy(Path.Combine(sourceactivepath, ThumbnailFileName), Path.Combine(pasteactivepath, ThumbnailFileName));
            }

            if (File.Exists(Path.Combine(sourceactivepath, ImageFileName)))
            {
                if (File.Exists(Path.Combine(pasteactivepath, ImageFileName)))
                    return false;
                    File.Copy(Path.Combine(sourceactivepath, ImageFileName), Path.Combine(pasteactivepath, ImageFileName));
                return true;
            }
            return false;
        }

        internal void PaintSelection(Graphics graphics, PointF location)
        {
            graphics.DrawPath(GlobalClass.HighlightPen, path);
            graphics.FillPath(GlobalClass.SelectionBrush, path);
            if (IsShortCut)
            {
                if (FileFound)
                {
                    size = graphics.MeasureString("Shortcut", GlobalClass.SmallfontBold);
                    graphics.FillPath(Brushes.Navy, GlobalClass.GetRoundBoundry(new RectangleF(location.X + 5, location.Y + 5, size.Width + 3, size.Height + 3), 3f));
                    graphics.DrawString("Shortcut", GlobalClass.SmallfontBold, Brushes.Gold, location.X + 5 + 1.5f, location.Y + 5 + 1.5f);
                }
            }
          
        }

        internal void PaintHovered(Graphics graphics, PointF location)
        {
            graphics.FillPath(new SolidBrush(Color.FromArgb(50, Color.Green)), path);

        }
        internal void PaintDeletedInfo(Graphics graphics, RectangleF rectangleF)
        {
            graphics.FillPath(new SolidBrush(Color.FromArgb(100, Color.Red)), path);
            size = graphics.MeasureString(OldParentDisplayName, GlobalClass.SmallfontBold,(int)rectangleF.Width);
            graphics.FillRectangle(Brushes.Navy, new RectangleF(rectangleF.X + 5, rectangleF.Y + 5, size.Width + 3, size.Height + 3));
            graphics.DrawString(OldParentDisplayName, GlobalClass.SmallfontBold, Brushes.Gold, new RectangleF(rectangleF.X + 5 + 3 / 2, rectangleF.Y + 5 + 3 / 2, rectangleF.Width, rectangleF.Height));

        }
        internal void LoadInitialPicture(string Active)
        {
            picture = new Picture(ImageFileName, ThumbnailFileName);
        }

        internal bool Delete(string oldparent,string olddisplaynode, string activefolder)
        {
            if (this.IsDeleted)
                DeleteEntityFile(activefolder);
            else if(this.IsShortCut)
            {
                //Do nothing - it will delete the entity from the list, doesn't move to recyle bin or doesn't delete entity file
            }
            else
            {
                OldParentName = oldparent;
                OldParentDisplayName = olddisplaynode;
                IsDeleted = true;
                Controller.AddToRecycle(this);
               
            }
            return true;
        }

        private void DeleteEntityFile(string ActiveFoldepath)
        {
            if (File.Exists(Path.Combine(ActiveFoldepath, ThumbnailFileName)))
            {
                File.Delete(Path.Combine(ActiveFoldepath, ThumbnailFileName));
            }

            if (File.Exists(Path.Combine(ActiveFoldepath, ImageFileName)))
            {
                File.Delete(Path.Combine(ActiveFoldepath, ImageFileName));
            }
        }

        internal void Rotate(string ActivePath)
        {
            picture.Rotate(ActivePath);
        }

        internal void Flip(string activeFolderPath)
        {
            picture.Flip(activeFolderPath);
        }
    }

    public class Picture
    {
        public Image Thumbnail;
        string imagename, thumbname;
        public Picture(string imagenam, string thumbname)
        {
            this.imagename = imagenam;
            this.thumbname = thumbname;
            Thumbnail = new Bitmap(1, 1);
        }

        internal bool SaveImage(string ImagePath, string activefolder)
        {
            if (!Directory.Exists(activefolder))
                Directory.CreateDirectory(activefolder);
            Image FullImage = ImageLoad.LoadBitmapImage(ImagePath);
            if (FullImage == null)
                return false;
            SD.StringImage.Conversion.ImageToString(FullImage, "", Path.Combine(activefolder, imagename));

            Thumbnail = ImageLoad.GetThumbnailImage(FullImage, GlobalClass.ThumbnailSize.Width);
            SD.StringImage.Conversion.ImageToString(Thumbnail, "", Path.Combine(activefolder, thumbname));
            return true;

        }
        internal void SaveImage(Image image, string activefolder)
        {
            if (!Directory.Exists(activefolder))
                Directory.CreateDirectory(activefolder);
           
            SD.StringImage.Conversion.ImageToString(image, "", Path.Combine(activefolder, imagename));

            Thumbnail = ImageLoad.GetThumbnailImage(image, GlobalClass.ThumbnailSize.Width);
            SD.StringImage.Conversion.ImageToString(Thumbnail, "", Path.Combine(activefolder, thumbname));

        }
        public bool LoadThumbnail(string activefolder)
        {
            if (File.Exists(Path.Combine(activefolder, thumbname)))
            {
                Thumbnail = SD.StringImage.Conversion.StringToImage(Path.Combine(activefolder, thumbname));
                return true;
            }
            else
            {
                Thumbnail = new Bitmap(1, 1);
                return false;
            }
        }
        public Image GetFullImage(string activefolder)
        {
            if (File.Exists(Path.Combine(activefolder, imagename)))
                return SD.StringImage.Conversion.StringToImage(Path.Combine(activefolder, imagename));
            return new Bitmap(1, 1);
        }

        internal void Rotate(string activePath)
        {
           
            //Image img = GetFullImage(activePath);

            //Matrix myMatrix = new Matrix();
            //myMatrix.RotateAt(90f, new PointF(img.Width / 2, img.Height / 2), MatrixOrder.Prepend);

            //int width = img.Width > img.Height ? img.Width : img.Height;
            //Bitmap bmp = new Bitmap(width, width);
            //Graphics g = Graphics.FromImage(bmp);
            
            ////  g.DrawImage(img, new PointF(0, 0));
            //g.DrawImage(img, new Rectangle(0, 0,width,width));
            // img = bmp;
            
            //// img.RotateFlip(RotateFlipType.Rotate90FlipNone);

            //SaveImage(img, activePath);

        }

        internal void Flip(string activeFolderPath)
        {

        }
    }
}
