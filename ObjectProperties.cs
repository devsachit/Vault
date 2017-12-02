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
    public partial class ObjectProperties : Form
    {
        public enum ObjectSelected
        {
            Entity,
            Node,
            None,
        }
        ObjectSelected selected = ObjectSelected.None;
        Image fullimage;
        string ActiveFoler;

        public ObjectProperties(string activefolder)
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            GlobalClass.SelectedEntityChanged += GlobalClass_SelectedEntityChanged;
            GlobalClass.SelectedNodeChanged += GlobalClass_SelectedNodeChanged;
            this.ActiveFoler = activefolder;
        }

        private void GlobalClass_SelectedNodeChanged(object sender, EventArgs e)
        {
            LoadObject();

        }

        private void GlobalClass_SelectedEntityChanged(object sender, EventArgs e)
        {
            LoadObject();
        }
        SizeF s;
        private string nodecount;
        private string entitycount;

        private void Properties_Paint(object sender, PaintEventArgs e)
        {
            float x = 10, y = 10;
            if (selected == ObjectSelected.Entity)
            {
                e.Graphics.DrawImage(GlobalClass.SelectedEntity.GetThumbnail(), new PointF(x, y));
                x = GlobalClass.ThumbnailSize.Width + 20;
                x += PaintStringBold(e.Graphics, "Image File Name: ", x, y).Width; y += PaintString(e.Graphics,  GlobalClass.SelectedEntity.ImageFileName, x, y).Height;
                x = GlobalClass.ThumbnailSize.Width + 20;
                x += PaintStringBold(e.Graphics,  "Thumb File Name: ", x, y).Width; y += PaintString(e.Graphics, GlobalClass.SelectedEntity.ThumbnailFileName, x, y).Height;
                x = GlobalClass.ThumbnailSize.Width + 20;
                x += PaintStringBold(e.Graphics, "Tags: " , x, y).Width; y += PaintString(e.Graphics,  GlobalClass.SelectedEntity.Tags, x, y).Height;
                x = GlobalClass.ThumbnailSize.Width + 20;
                x += PaintStringBold(e.Graphics, "Old Parent Name: ", x, y).Width; y += PaintString(e.Graphics,  GlobalClass.SelectedEntity.OldParentName, x, y).Height;
                x = GlobalClass.ThumbnailSize.Width + 20;
                x += PaintStringBold(e.Graphics, "Old Parent Display Name: " , x, y).Width; y += PaintString(e.Graphics,  GlobalClass.SelectedEntity.OldParentDisplayName, x, y).Height;
                x = GlobalClass.ThumbnailSize.Width + 20;
                x += PaintStringBold(e.Graphics, "Is Deleted ?  ", x, y).Width; y += PaintString(e.Graphics,   GlobalClass.SelectedEntity.IsDeleted.ToString(), x, y).Height;
                x = GlobalClass.ThumbnailSize.Width + 20;
                x += PaintStringBold(e.Graphics, "Is Link ?  " , x, y).Width; y += PaintString(e.Graphics,  GlobalClass.SelectedEntity.IsShortCut.ToString(), x, y).Height;
                x = GlobalClass.ThumbnailSize.Width + 20;
                x += PaintStringBold(e.Graphics, "Is Favorite ?  ", x, y).Width; y += PaintString(e.Graphics, GlobalClass.SelectedEntity.IsFavorite.ToString(), x, y).Height;
                x = GlobalClass.ThumbnailSize.Width + 20;
                x += PaintStringBold(e.Graphics, "Dimenstion: ", x, y).Width; y += PaintString(e.Graphics,  fullimage.Width + " X " + fullimage.Height, x, y).Height;


            }
            else if (selected == ObjectSelected.Node)
            {
                e.Graphics.DrawImage(GlobalClass.SelectedNode.CoverImage, new PointF(x, y));
                x = GlobalClass.ThumbnailSize.Width + 20;
                x += PaintStringBold(e.Graphics, "Node File Name: ", x, y).Width; y += PaintString(e.Graphics, GlobalClass.SelectedNode.Name, x, y).Height;
                x = GlobalClass.ThumbnailSize.Width + 20;
                x += PaintStringBold(e.Graphics, "Display Name: ", x, y).Width; y += PaintString(e.Graphics, GlobalClass.SelectedNode.DisplayName, x, y).Height;
                x = GlobalClass.ThumbnailSize.Width + 20;
                x += PaintStringBold(e.Graphics, "Parent Name: ", x, y).Width; y += PaintString(e.Graphics, GlobalClass.SelectedNode.ParentName, x, y).Height;
                x = GlobalClass.ThumbnailSize.Width + 20;
                x += PaintStringBold(e.Graphics, "Old Parent Name: ", x, y).Width; y += PaintString(e.Graphics, GlobalClass.SelectedNode.OldParentName, x, y).Height;
                x = GlobalClass.ThumbnailSize.Width + 20;
                x += PaintStringBold(e.Graphics, "Old Parent Display Name: ", x, y).Width; y += PaintString(e.Graphics, GlobalClass.SelectedNode.OldParentDisplayName, x, y).Height;
                x = GlobalClass.ThumbnailSize.Width + 20;
                x += PaintStringBold(e.Graphics, "Contained Nodes: ", x, y).Width; y += PaintString(e.Graphics, nodecount, x, y).Height;
                x = GlobalClass.ThumbnailSize.Width + 20;
                x += PaintStringBold(e.Graphics, "Contained Images  ", x, y).Width; y += PaintString(e.Graphics, entitycount, x, y).Height;
                x = GlobalClass.ThumbnailSize.Width + 20;
               
            }
        }

        private SizeF PaintString(Graphics graphics, string v, float x,  float y)
        {          
            graphics.DrawString(v, GlobalClass.PropertyNormalFont, Brushes.Black, x, y);
            if (v == "" || v==null)
                v = "something";
            s = graphics.MeasureString(v, GlobalClass.PropertyNormalFont);
            return s;
        }
        private SizeF PaintStringBold(Graphics graphics, string v, float x, float y)
        {
            graphics.DrawString(v, GlobalClass.PropertyBoldFont, Brushes.Gray, x, y);
            if (v == "" || v == null)
                v = "something";
            s = graphics.MeasureString(v, GlobalClass.PropertyBoldFont);
            return s;
        }
        internal void LoadObject()
        {
            if (GlobalClass.SelectedNode != null)
            {
                selected = ObjectSelected.Node;
                GlobalClass.SelectedNode.ActiveFolderPath = ActiveFoler;
                if (GlobalClass.SelectedNode.Read())
                {
                    nodecount = GlobalClass.SelectedNode.Nodes.Count.ToString();
                    entitycount = GlobalClass.SelectedNode.Entities.Count.ToString();
                }
                else
                {
                    nodecount = "0";
                    entitycount = "0";
                }
            }
            else if (GlobalClass.SelectedEntity != null)
            {
                selected = ObjectSelected.Entity;
                fullimage = GlobalClass.SelectedEntity.GetFullImage(ActiveFoler);
            }
            else
                selected = ObjectSelected.None;

            this.Invalidate();
          
        }
    }
}
