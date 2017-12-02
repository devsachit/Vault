using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vault
{
    public partial class NodeTree : UserControl
    {
    
        public string ActiveFolderPath;
        Tree tree;
        Node RootNode;
        TreeNode selectedTreeNode;
        public NodeTree()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            GlobalClass.NodeControllerChanged += GlobalClass_NodeControllerChanged;
 
        }

        private void GlobalClass_NodeControllerChanged(object sender, EventArgs e)
        {
            Initialize(sender.ToString());
        }

        public void Initialize(string ActiveFolder)
        {
            this.ActiveFolderPath = ActiveFolder;
            RootNode = new Node(ActiveFolder, "RootNode", "", "Root Node");
            TreeNode n = new TreeNode("Root Node");
            n.Tag = "RootNode";
            //treeView1.Nodes.Add(n);
            //selectedTreeNode = n;
            //AddChildsToSelectedTreeNode();

            //tree = new Tree(ActiveFolder);
            //this.Invalidate();

        }

        private void AddChildsToSelectedTreeNode()
        {
            Node node = new Node(ActiveFolderPath, selectedTreeNode.Tag.ToString(), "", selectedTreeNode.Text);
            node.Read(true, true);
            foreach (Node n in node.Nodes)
            {
                n.ActiveFolderPath = ActiveFolderPath;
                TreeNode treenode = new TreeNode(n.DisplayName);
                treenode.Tag = n.Name;
                selectedTreeNode.Nodes.Add(treenode);
            }
        }

        private void NodeTree_Paint(object sender, PaintEventArgs e)
        {
            //if(tree!=null)
            //{
            //    tree.Paint(e.Graphics);
            //}
        }

        private void NodeTree_KeyDown(object sender, KeyEventArgs e)
        {
            Controller.KeyDown(sender, e);
        }

        private void NodeTree_KeyUp(object sender, KeyEventArgs e)
        {
            Controller.KeyUp(sender, e);
        }

        private void NodeTree_MouseDown(object sender, MouseEventArgs e)
        {

        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            Controller.MouseWheel(e.Delta);
        }
        private void NodeTree_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //if(tree!=null)
            //{
            //    tree.MouseDoubleClick(e.Location, e.Button);
            //    this.Invalidate();
            //}
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            selectedTreeNode = treeView1.SelectedNode;
            selectedTreeNode.Nodes.Clear();
            AddChildsToSelectedTreeNode();
        }

        private void treeView1_MouseEnter(object sender, EventArgs e)
        {
            
                
        }

        private void treeView1_MouseLeave(object sender, EventArgs e)
        {
           
        }

        private void NodeTree_Scroll(object sender, ScrollEventArgs e)
        {
            Controller.MouseWheel(e.OldValue>e.NewValue?120:-120);
        }
    }

    public class Tree
    {
        List<TreeItem> Items;
        string ActiveFolderpath { get; }
        Node RootNode;
        public Tree(string ActiveFolder)
        {
            Items = new List<TreeItem>();
            this.ActiveFolderpath = ActiveFolder;
            RootNode = new Node(ActiveFolder, "RootNode", "", "Root Node");
            Items = new List<TreeItem>();
            Items.Add(new TreeItem(RootNode, ActiveFolder,new Point(0,0)));
        }

        internal void Paint(Graphics graphics)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Paint(graphics);
            }
        }

        internal void MouseDoubleClick(Point location, MouseButtons button)
        {
            FindSelection(location);
        }

        private void FindSelection(Point location)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].FindSelection(location))
                    break;
            }
        }
    }

    public class TreeItem
    {
        public string ActiveFolderPath { get; }
        public Node ParentNode;
        public List<TreeItem> ChildNodes;
        public List<RectangleF> NodeRects;
        public bool IsCollaped = false;
        public PointF StartLocation;
        public PointF LastLocation;
        public TreeItem(Node Parentnode,string Activefolder,PointF p)
        {
            this.ActiveFolderPath = Activefolder;
            this.ParentNode = Parentnode;
            this.ParentNode.Read(true, true);
            StartLocation = p; 
            ReloadChilds();
            SetRectangles();
        }

        private void SetRectangles()
        {
            NodeRects = new List<RectangleF>();
            NodeRects.Add(new RectangleF(StartLocation.X, StartLocation.Y, 200, 40));

            float x = StartLocation.X + 10;
            float y = StartLocation.Y + 40;
            if(IsCollaped==false)
            {
                for(int i=0;i<ChildNodes.Count;i++)
                {
                    NodeRects.Add(new RectangleF(x, y, 200, 40));
                    y += 40;
                }
            }
            LastLocation = NodeRects[NodeRects.Count - 1].Location;
           
        }
        public void ToggleCollepse()
        {
            if (IsCollaped)
                IsCollaped = false;
            else
                IsCollaped = true;
            SetRectangles();
        }
        public void Refresh()
        {
            SetRectangles();
        }
        private void ReloadChilds()
        {
            ChildNodes = new List<TreeItem>();
            int i = 0;
            foreach (Node n in this.ParentNode.Nodes)
            {
                n.ActiveFolderPath = ActiveFolderPath;
                i++;
                ChildNodes.Add(new TreeItem(n, ActiveFolderPath, new PointF(StartLocation.X, StartLocation.Y + 40 * i)));
            }


        }

        public void Paint(Graphics g)
        {
            g.DrawImage(ParentNode.CoverImage, new RectangleF(NodeRects[0].X, NodeRects[0].Y, 40, 40));
            g.DrawString(ParentNode.DisplayName, GlobalClass.NodeNameFont, Brushes.Black, NodeRects[0].X+40, NodeRects[0].Y);
            for (int i = 1; i < NodeRects.Count; i++)
            {
                ChildNodes[i - 1].Paint(g);

            }
        }

        internal bool FindSelection(Point location)
        {
            RectangleF rect = new RectangleF(location, new Size(1, 1));
            for (int i = 0; i < NodeRects.Count; i++)
            {
                if (rect.IntersectsWith(NodeRects[i]))
                {

                }
            }
            return true;
        }
    }
}
