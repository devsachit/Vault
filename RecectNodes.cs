using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault
{
    public class RecectNodes
    {
        public List<RecentBlock> recentblocks;
        public RecectNodes()
        {
            recentblocks = new List<RecentBlock>();
            recentblocks.Add(new RecentBlock(RecentBlock.NodeAction.Opened, new Rectangle(5, 122, GlobalClass.LeftPanelWidth - 5, GlobalClass.ThumbnailSize.Height / 2)));
            recentblocks.Add(new RecentBlock(RecentBlock.NodeAction.DestinationParentNodeForMove, new Rectangle(5, 122 + GlobalClass.ThumbnailSize.Height / 2 + 30, GlobalClass.LeftPanelWidth - 5, GlobalClass.ThumbnailSize.Height / 2)));
            recentblocks.Add(new RecentBlock(RecentBlock.NodeAction.CopiedFrom, new Rectangle(5, 122 + GlobalClass.ThumbnailSize.Height + 60, GlobalClass.LeftPanelWidth - 5, GlobalClass.ThumbnailSize.Height / 2)));
            recentblocks.Add(new RecentBlock(RecentBlock.NodeAction.Default, new Rectangle(5, 122 + GlobalClass.ThumbnailSize.Height * 3 / 2 + 90, GlobalClass.LeftPanelWidth - 5, GlobalClass.ThumbnailSize.Height / 2)));
            recentblocks.Add(new RecentBlock(RecentBlock.NodeAction.Custom, new Rectangle(5, 122 + GlobalClass.ThumbnailSize.Height * 2 + 120, GlobalClass.LeftPanelWidth - 5, GlobalClass.ThumbnailSize.Height / 2)));



        }
        public void AddNode(ref Node n, RecentBlock.NodeAction action)
        {
            RecentBlock block = null;
            bool contains = false;
            for (int i = 0; i < recentblocks.Count; i++)
            {
               
                if (recentblocks[i].nodeAction == action)
                {
                    block = recentblocks[i];
                }
                if (recentblocks[i].nodeAction == RecentBlock.NodeAction.Opened)
                    continue;
                for (int j = 0; j < recentblocks[i].Nodes.Count; j++)
                {
                    if (recentblocks[i].Nodes[j].Node.Name == n.Name)
                    {
                        contains = true;
                    }
                }
            }

            if (contains == false)
            {
                if (block != null)
                    block.AddNode(n);
            }
        }

        internal void Paint(Graphics graphics)
        {
            
            GraphicsPath path = GlobalClass.GetRoundBoundry(new RectangleF(2, 103, GlobalClass.LeftPanelWidth, GlobalClass.ParentBoundry.Height - GlobalClass.TitleHeight - 6), 15f);

            graphics.FillPath(GlobalClass.LeftPanelBrush, path);
            graphics.DrawPath(GlobalClass.BorderColorPen, path);
            for (int i = 0; i < recentblocks.Count; i++)
            {
                recentblocks[i].Paint(graphics);
            }
        }

        internal void MouseWheel(int delta, Point p)
        {
            RecentBlock b = GetRecentBlock(p);
            if (b != null)
                b.MouseWheel(delta);
        }

        private RecentBlock GetRecentBlock(Point p)
        {
            RectangleF rect = new RectangleF(p, new Size(1, 1));
            int index = -1;
            for (int i = 0; i < recentblocks.Count; i++)
            {
                if (rect.IntersectsWith(recentblocks[i].Region))
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
                return recentblocks[index];

            return null;
        }

        internal void MouseDown(Point location)
        {
            RecentBlock b = GetRecentBlock(location);
            if (b != null)
                b.MouseDown(location);
        }

        internal void MouseMove(Point location)
        {
            RecentBlock b = GetRecentBlock(location);
            if (b != null)
                b.MouseMove(location);
        }

        internal void RemoveNode(Node openedNode, RecentBlock.NodeAction action)
        {
            for (int i = 0; i < recentblocks.Count; i++)
            {
                recentblocks[i].RemoveNode(openedNode);
            }
        }
    }

    public class RecentBlock
    {
        public List<NodeBlock> Nodes;
        public Rectangle Region;
        public NodeAction nodeAction;
        private int _shiftindex;

        public int ShiftIndex
        {
            get { return _shiftindex; }
            set
            {
                if (value > 0) _shiftindex = 0;
                else if (Nodes.Count>0 && Nodes[0].NodeRect.X - Nodes[0].NodeRect.Width > Math.Abs(value))
                    _shiftindex = value;
                //   else _shiftindex = value;
            }
        }

        public int SelectedIndex = -1;
        public int HoveredIndex = -1;

        public enum NodeAction
        {
            Opened,
            DestinationParentNodeForMove,
            CopiedFrom,
            Default,
            Custom,
        }

        public RecentBlock(NodeAction action, Rectangle region)
        {
            Nodes = new List<NodeBlock>();
            this.Region = region;
            this.nodeAction = action;
        }
        public void AddNode(Node n)
        {
            bool contains = false;
            if (nodeAction == NodeAction.Opened)
            {
                for (int j = 0; j < Nodes.Count; j++)
                {
                    if (Nodes[j].Node.Name == n.Name)
                    {
                        Node temp = Nodes[j].Node;
                        Nodes[j].Node = Nodes[Nodes.Count-1].Node;
                        Nodes[Nodes.Count-1].Node = temp;
                        contains = true;
                    }
                }
            }
            if (contains == false)
            {
                Nodes.Add(new NodeBlock(n, nodeAction));
                
            }
            SetRectagles();
        }
        private void SetRectagles()
        {

            int x = Region.X, y = Region.Y;

            for (int i = Nodes.Count - 1; i >= 0; i--)
            {
                Nodes[i].SetRectangle(new RectangleF(x, y, GlobalClass.ThumbnailSize.Width / 2, GlobalClass.ThumbnailSize.Height / 2));
                x += GlobalClass.ThumbnailSize.Width / 2 + 2;
            }

        }

        internal void Paint(Graphics graphics)
        {
            PaintNodeAction(graphics);
            graphics.SetClip(Region);
          // graphics.FillRectangle(GlobalClass.SelectionBrush, Region);
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].Paint(graphics, ShiftIndex, HoveredIndex, SelectedIndex, i);

               
            }
            graphics.ResetClip();
        }

        private void PaintNodeAction(Graphics graphics)
        {
            switch (nodeAction)
            {
                case NodeAction.Opened:
                    graphics.DrawImage(Properties.Resources.recentlyopened, Region.X + 5, Region.Y - 15, 108, 17);
                    //graphics.DrawString("Recently Opened", GlobalClass.SmallfontBold, Brushes.Black, Region.X+5, Region.Y - 15);
                    break;
                case NodeAction.DestinationParentNodeForMove:
                  //  graphics.DrawLine(GlobalClass.BorderColorPen, new Point(Region.X-2, Region.Y - 20), new Point(Region.X + Region.Width+2 , Region.Y - 20));
                    graphics.DrawImage(Properties.Resources.movedto, Region.X + 5, Region.Y - 15, 108, 17);

                  //  graphics.DrawString("Recently Moved To... ", GlobalClass.SmallfontBold, Brushes.Black, Region.X+5, Region.Y - 15);
                    break;
                case NodeAction.CopiedFrom:
                 //   graphics.DrawLine(GlobalClass.BorderColorPen, new Point(Region.X - 2, Region.Y - 20), new Point(Region.X + Region.Width + 2, Region.Y - 20));
                    graphics.DrawImage(Properties.Resources.copiedfrom, Region.X + 5, Region.Y - 15, 108, 17);

                    //    graphics.DrawString("Recently Copied From... ", GlobalClass.SmallfontBold, Brushes.Black, Region.X+5, Region.Y - 15);
                    break;
                case NodeAction.Default:
                 //   graphics.DrawLine(GlobalClass.BorderColorPen, new Point(Region.X - 2, Region.Y - 20), new Point(Region.X + Region.Width + 2, Region.Y - 20));
                    graphics.DrawImage(Properties.Resources.systemfolders, Region.X + 5, Region.Y - 15, 108, 17);

                   // graphics.DrawString("System Folders", GlobalClass.SmallfontBold, Brushes.Black, Region.X + 5, Region.Y - 15);
                    break;
                case NodeAction.Custom:
                //    graphics.DrawLine(GlobalClass.BorderColorPen, new Point(Region.X - 2, Region.Y - 20), new Point(Region.X + Region.Width + 2, Region.Y - 20));
                    graphics.DrawImage(Properties.Resources.customfolders, Region.X + 5, Region.Y - 15, 108, 17);

                   // graphics.DrawString("Custom Folders", GlobalClass.SmallfontBold, Brushes.Black, Region.X + 5, Region.Y - 15);
                    break;
            }
        }

        internal void MouseWheel(int delta)
        {
            if (delta > 0)
            {
                ShiftIndex += GlobalClass.ThumbnailSize.Width / 2;
            }
            else
                ShiftIndex -= GlobalClass.ThumbnailSize.Width / 2;
        }
        bool isdeleteiconclicked = false;
        internal void MouseDown(Point location)
        {
            SelectedIndex = FindSelection(location);
            if (SelectedIndex != -1)
                Nodes[SelectedIndex].Open();
            if (isdeleteiconclicked)
            {
                Controller.DeleteCustomFolder();
            }

        }
        internal void MouseMove(Point location)
        {
            HoveredIndex = FindSelection(location);
            
                SelectedIndex = -1;
        }
        private int FindSelection(Point location)
        {
            isdeleteiconclicked = false;
            RectangleF rect = new RectangleF(location, new Size(1, 1));
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (rect.IntersectsWith(new RectangleF(Nodes[i].NodeRect.X + ShiftIndex, Nodes[i].NodeRect.Y, Nodes[i].NodeRect.Width, Nodes[i].NodeRect.Height)))
                {
                    if (Nodes[i].IsDeleteIconShown && rect.IntersectsWith(new RectangleF(Nodes[i].NodeRect.X + ShiftIndex + Nodes[i].NodeRect.Width - 17, Nodes[i].NodeRect.Y + 2, 15, 15)))
                    {
                        isdeleteiconclicked = true;
                    }
                    else isdeleteiconclicked = false;
                    return i;
                }
            }
            return -1;
        }

        internal void RemoveNode(Node openedNode)
        {
    
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Node.Name == openedNode.Name)
                {
                    Nodes.RemoveAt(i);
                }
            }
            SetRectagles();
        }
    }

    public class NodeBlock
    {
        public Node Node;

        public RecentBlock.NodeAction Nodeaction;

        public RectangleF NodeRect;

        public bool IsDeleteIconShown { get; internal set; }

        public NodeBlock(Node node,RecentBlock.NodeAction nodeaction)
        {
            this.Node = node;
            this.Nodeaction = nodeaction;
        }
        public void SetRectangle(RectangleF rect)
        {
            this.NodeRect = rect;
        }

        internal void Open()
        {
            Controller.OpenNode(Node);
        }

        internal void PaintImage(Graphics graphics,int ShiftIndex)
        {
            if(Node.CoverImage!=null)
            graphics.DrawImage(Node.CoverImage, new RectangleF(NodeRect.X + ShiftIndex+12.5f, NodeRect.Y+5, NodeRect.Width-25, NodeRect.Height-25));
            
          
        }

        internal void Paint(Graphics graphics, int shiftIndex, int hoveredIndex, int selectedIndex, int thisindex)
        {
            IsDeleteIconShown = false;
            //if (selectedIndex == thisindex)
            //{
            //    PaintSelection(graphics, shiftIndex);

            //}
            if (hoveredIndex == thisindex)
            {
                PaintHovered(graphics, shiftIndex);
            }

            PaintImage(graphics, shiftIndex);
            PaintNodeInfo(graphics, shiftIndex);

            if (selectedIndex == thisindex)
            {
                PaintNodeSelection(graphics, shiftIndex);
                PaintRemoveIcon(graphics, shiftIndex);
            }
        }

        private void PaintRemoveIcon(Graphics graphics, int shiftIndex)
        {
            if (Nodeaction == RecentBlock.NodeAction.Custom)
                graphics.DrawImage(Properties.Resources.removesmall, new RectangleF(NodeRect.X + shiftIndex + NodeRect.Width - 17, NodeRect.Y+2, 15, 15));
            IsDeleteIconShown = true;
        }

        internal void PaintHovered(Graphics graphics, int shiftIndex)
        {
        
            graphics.FillRectangle(GlobalClass.DimSelectionBrush, new RectangleF(NodeRect.X + shiftIndex, NodeRect.Y, NodeRect.Width, NodeRect.Height));

          
        }

        internal void PaintNodeInfo(Graphics graphics, int shiftIndex)
        {

            SizeF s = graphics.MeasureString(Node.DisplayName, GlobalClass.SmallfontBold, (int)NodeRect.Width);
            graphics.FillRectangle(Brushes.Gray, new RectangleF(NodeRect.X + shiftIndex, NodeRect.Y + GlobalClass.ThumbnailSize.Height / 2 - s.Height - 5, NodeRect.Width, s.Height + 5));

            graphics.DrawString(Node.DisplayName, GlobalClass.SmallfontBold, Brushes.White, new RectangleF(shiftIndex + NodeRect.X + GlobalClass.ThumbnailSize.Width / 4 - s.Width / 2, NodeRect.Y + GlobalClass.ThumbnailSize.Height / 2 - s.Height - 2.5f, NodeRect.Width, s.Height));
        }

        internal void PaintNodeSelection(Graphics graphics, int shiftIndex)
        {

            SizeF s = graphics.MeasureString(Node.DisplayName, GlobalClass.SmallfontBold, (int)NodeRect.Width);
            graphics.FillRectangle(Brushes.Gold, new RectangleF(NodeRect.X + shiftIndex, NodeRect.Y + GlobalClass.ThumbnailSize.Height / 2 - s.Height - 5, NodeRect.Width, s.Height + 5));
            graphics.DrawString(Node.DisplayName, GlobalClass.SmallfontBold, Brushes.Black, new RectangleF(shiftIndex + NodeRect.X + GlobalClass.ThumbnailSize.Width / 4 - s.Width / 2, NodeRect.Y + GlobalClass.ThumbnailSize.Height / 2 - s.Height - 2.5f, NodeRect.Width, s.Height));

        }
    }
}
