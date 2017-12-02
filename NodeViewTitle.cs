using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault
{
    public class NodeViewTitle
    {
        public string ActiveFolder { get; }

        List<string> NodeDisplayNames;
        List<string> NodeNames;
        public string NodeDisplayPath,NodePath;
       // BackgroundWorker worker;
      
        public bool RequestNew { get; private set; }
        public bool NodeSelected { get; private set; }

        //int NodeCount = 0;
        //int EntityCount = 0;
        Node SelectedNode;
        string CurrentNodeDisplayName;
        
        public NodeViewTitle(string Activefolder)
        {
            this.ActiveFolder = Activefolder;
            NodeDisplayNames = new List<string>();
            NodeNames = new List<string>();
           
            //worker = new BackgroundWorker();
            //worker.DoWork += Worker_DoWork;
            //worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
       //     GlobalClass.SelectedNodeChanged += GlobalClass_SelectedNodeChanged;
            CurrentNodeDisplayName = "";
          
        }

        //private void GlobalClass_SelectedNodeChanged(object sender, EventArgs e)
        //{
        //    SelectedNode = GlobalClass.SelectedNode;
        //    if(SelectedNode==null)
        //    {
        //        NodeSelected = false;
        //        return;
        //    }
        //    NodeSelected = true;
        //    if (worker.IsBusy)
        //    {
        //        RequestNew = true;
        //    }
        //    else
        //        worker.RunWorkerAsync(SelectedNode);
        //}

        //private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    if(RequestNew)
        //    {
        //        worker.RunWorkerAsync(SelectedNode);
        //    }
        //}

        //private void Worker_DoWork(object sender, DoWorkEventArgs e)
        //{
          
        //    Node selected = (Node)e.Argument;
        //    if(selected==null)
        //    {
        //        NodeSelected = false;
        //        return;

        //    }
        //    RequestNew = false;
        //    selected.ActiveFolderPath = ActiveFolder;
        //    selected.Read();
        //    NodeCount = selected.Nodes.Count;
        //    EntityCount = selected.Entities.Count;
        //}

        internal void AddNodeName(string displayname,string nodename)
        {
            CurrentNodeDisplayName = displayname;
            NodeDisplayNames.Add(displayname);
            NodeNames.Add(nodename);
            SetNodePath();
        }

        private void SetNodePath()
        {
            NodeDisplayPath = "";
            for(int i=0;i<NodeDisplayNames.Count;i++)
            {
                NodeDisplayPath += NodeDisplayNames[i] +"/";
            }
            NodePath = "";
            for (int i = 0; i < NodeNames.Count; i++)
            {
                NodePath += NodeNames[i] + "/";
            }
        }
        internal void RemoveName(string displayName,string nodename)
        {
            NodeDisplayNames.Remove(displayName);
            NodeNames.Remove(nodename);
            SetNodePath();
            if (NodeNames.Count == 0)
                CurrentNodeDisplayName = "Root Node";
            else
                CurrentNodeDisplayName = NodeDisplayNames[NodeDisplayNames.Count - 1];
        }
        SizeF s;
        internal void Paint(Graphics graphics)
        {
            graphics.FillRectangle(GlobalClass.TitleBrush, new RectangleF(0, 0, GlobalClass.ParentBoundry.Width, GlobalClass.TitleHeight));
          //  graphics.DrawImage(Properties.Resources.line, new Rectangle(0, GlobalClass.TitleHeight-3, GlobalClass.ParentBoundry.Width, 5));
            if (GlobalClass.SelectedNode != null)
            {
                s = graphics.MeasureString(CurrentNodeDisplayName + " >> " + GlobalClass.SelectedNode.DisplayName, GlobalClass.SmallfontBold);
                graphics.DrawString(CurrentNodeDisplayName + " >> " + GlobalClass.SelectedNode.DisplayName, GlobalClass.SmallfontBold, Brushes.White, new RectangleF(5, 2, GlobalClass.ParentBoundry.Width, GlobalClass.TitleHeight));
            }
            else
            {
                s = graphics.MeasureString(CurrentNodeDisplayName, GlobalClass.SmallfontBold);
                graphics.DrawString(CurrentNodeDisplayName, GlobalClass.SmallfontBold, Brushes.Gray, new RectangleF(5, 2, GlobalClass.ParentBoundry.Width, GlobalClass.TitleHeight));

            }

            //Selected Folder Image
            GraphicsPath path = GlobalClass.GetRoundBoundry(new RectangleF(5, s.Height + 5, 60, 60), 10);
            graphics.SetClip(path);
            if (GlobalClass.SelectedNode != null)
                graphics.DrawImage(GlobalClass.SelectedNode.CoverImage, new RectangleF(5, s.Height + 5, 60, 60));
            else
            {
                graphics.FillRectangle(GlobalClass.BlankBrush, new RectangleF(5, s.Height + 5, 60, 60));
                if (CurrentNodeDisplayName == "Favorite")
                    graphics.DrawImage(Properties.Resources.fav, new RectangleF(5 + 60/2-36/2, s.Height + 5+60/2-36/2, 36, 36));
               else if (CurrentNodeDisplayName == "Recycle Bin")
                    graphics.DrawImage(Properties.Resources.deletedicon, new RectangleF(5 + 60 / 2 - 36 / 2, s.Height + 5 + 60 / 2 - 36 / 2, 36, 36));
                else if (CurrentNodeDisplayName == "Downloads")
                    graphics.DrawImage(Properties.Resources.downloads, new RectangleF(5 + 60 / 2 - 36 / 2, s.Height + 5 + 60 / 2 - 36 / 2, 36, 36));

            }
            graphics.ResetClip();
            graphics.DrawPath(GlobalClass.BorderColorPen, path);
            graphics.DrawString("Loaded From : ", GlobalClass.SmallfontBold, Brushes.LightGoldenrodYellow, new PointF(GlobalClass.LeftPanelWidth, s.Height + 5));

            graphics.DrawString(ActiveFolder, GlobalClass.SmallfontBold, Brushes.LightGray, new PointF(GlobalClass.LeftPanelWidth+70, s.Height+5));



            //Selected Node Image
            path = GlobalClass.GetRoundBoundry(new RectangleF(5 + 65, s.Height + 5, 60, 60),10);
            graphics.SetClip(path);
            if (GlobalClass.SelectedEntity!=null)
                graphics.DrawImage(GlobalClass.SelectedEntity.GetThumbnail(), new RectangleF(5+65, s.Height + 5, 60, 60));
            else
                graphics.FillRectangle(GlobalClass.BlankBrush, new RectangleF(5 + 65, s.Height + 5, 60, 60));
            graphics.ResetClip();
            graphics.DrawPath(GlobalClass.BorderColorPen, path);

            ////Select Nodes and Entity Count
            //if (NodeSelected)
            //{
            //    float startx = s.Width + 7;
            //    if (NodeCount > 0)
            //    {
            //        s = graphics.MeasureString(" " + NodeCount + " folders ", GlobalClass.SmallfontBold);
            //        graphics.DrawString(" " + NodeCount + " folders ", GlobalClass.SmallfontBold, Brushes.DeepPink, startx, 2);
            //        startx += s.Width+5;
            //    }
            //    if (EntityCount > 0)
            //        graphics.DrawString(" " + EntityCount + " images ", GlobalClass.SmallfontBold, Brushes.SkyBlue, startx, 2);
            //    if (EntityCount == 0 && NodeCount == 0)
            //        graphics.DrawString("Empty Folder", GlobalClass.SmallfontBold, Brushes.Gold, startx, 2);
            //}
            if(Controller.GetSelectionCount()!=0)
            graphics.DrawString(Controller.GetSelectionCount() + " Items Selected", GlobalClass.SmallfontBold, Brushes.Gold, new PointF(5, s.Height + 70));


            }

       
    }
}
