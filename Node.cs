using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vault
{
    [Serializable]
    public class Node
    {
        public string Name { get; set; }
        public string ParentName { get; set; }
        public bool IsShortCut { get; set; }
        public string DisplayName { get; set; }
        public string OldParentName { get; private set; }
        public bool IsDeleted { get; set; }
        public string OldParentDisplayName { get; set; }
   
        public Point Shiftindex
        {
            get { return _shiftindex; }
            set
            {
                if (value.Y > 0)
                    _shiftindex = new Point(0, 0);



                else if (entityRects!=null && entityRects.Count > 0)
                {
                    if (entityRects[entityRects.Count - 1].Y > Math.Abs(value.Y))
                        _shiftindex = value;
                }
                else if (nodeRects != null && nodeRects.Count > 0)
                {
                    if (nodeRects[nodeRects.Count - 1].Y > Math.Abs(value.Y))
                        _shiftindex = value;
                }
                else
                    _shiftindex = value;
                
            }
        }
        public List<Node> Nodes;
        public List<Entity> Entities;
        public enum Selection
        {
            Node,
            Entity,
            None,
        }
        [NonSerialized]
        public List<Index> SelectedIndexes;
        [NonSerialized]
        public string ActiveFolderPath;
        [NonSerialized]
        List<RectangleF> nodeRects, entityRects;
        [NonSerialized]
        public int HoveredIndex = -1;
     
        [NonSerialized]
        public Selection selection = Selection.None;


        [NonSerialized]
        bool Drag = false;
        [NonSerialized]
        private Point initialPoint;
        [NonSerialized]
        private MouseButtons mousebutton;
        [NonSerialized]
        public bool IsLoaded = false;
        [NonSerialized]
        private Selection HoveredSelection;
        [NonSerialized]
        bool SettingRects = false;
        [NonSerialized]
        public bool ThumbsLoaded = false;
        [NonSerialized]
        public bool ComparingRects = false;
        [NonSerialized]
        public string ParentNodeDisplayName;
        [NonSerialized]
        public Image CoverImage;
        [NonSerialized]
        SizeF size;
        [NonSerialized]
        GraphicsPath path;
        [NonSerialized]
        private Point _shiftindex;

        public Node(string Activefolder, string name, string parentname, string displayname)
        {
            this.ActiveFolderPath = Activefolder;
            this.Name = name;
            this.ParentName = parentname;
            this.DisplayName = displayname;
            this.IsShortCut = false;
            Shiftindex = new Point(0, 0);
            SelectedIndexes = new List<Index>();
            nodeRects = new List<RectangleF>();
            entityRects = new List<RectangleF>();
            Nodes = new List<Node>();
            Entities = new List<Entity>();
            ThumbsLoaded = false;

        }



        public Node(Node node, string copiedSourceActivePath, string newparentname)
        {
            this.ActiveFolderPath = copiedSourceActivePath;
            this.Name = "NodeC" + GlobalClass.GetRandomString();
            this.DisplayName = node.DisplayName;
            this.ParentName = newparentname;
            this.IsShortCut = true;
            Shiftindex = new Point(0, 0);
            SelectedIndexes = new List<Index>();
            nodeRects = new List<RectangleF>();
            entityRects = new List<RectangleF>();
            Nodes = new List<Node>();
            Entities = new List<Entity>();
            ThumbsLoaded = false;

        }
        private Image GetDefaultCover()
        {
            return Properties.Resources.folderimage;
        }

        private void LoadCover(string activeFolderPath)
        {
            if (Name == "RecycleBin")
            {
                CoverImage = Properties.Resources.deletedicon;
                return;
            }
            else if (Name == "Favorite")
            {
                CoverImage = Properties.Resources.fav;
                return;
            }
            else if (Name == "RootNode")
            {
                CoverImage = Properties.Resources.home;
                return;
            }
            if (Name == "Downloads")
            {
                CoverImage = Properties.Resources.downloads;
                return;
            }
            if (File.Exists(Path.Combine(activeFolderPath, "Cover" + Name)))
            {

                CoverImage = SD.StringImage.Conversion.StringToImage(Path.Combine(activeFolderPath, "Cover" + Name));

            }
            else
            {
                CoverImage = Properties.Resources.folderimage;

            }
        }
        public void SetCover(Image image)
        {
            CoverImage = image;
            SD.StringImage.Conversion.ImageToString(CoverImage, "", Path.Combine(ActiveFolderPath, "Cover" + Name));
        }

      

        internal void SelectAll()
        {
            SelectedIndexes = new List<Index>();
            for (int i = 0; i < nodeRects.Count; i++)
            {
                SelectedIndexes.Add(new Index(i, Selection.Node));
            }
            for (int i = 0; i < entityRects.Count; i++)
            {

                SelectedIndexes.Add(new Index(i, Selection.Entity));
            }
            GlobalClass.SelectedNode = GetSelectedNode();
            GlobalClass.SelectedEntity = GetSelectedEntity();
        }


        internal bool Paste()
        {
            //If source node is null, return
            if (GlobalClass.CopiedSourceNode == null)
                return false;
            //Readning Source Node
            GlobalClass.CopiedSourceNode.Read(true);



            //copying entities
            for (int i = 0; i < GlobalClass.CopiedEntities.Count; i++)
            {
                bool Duplicatedfile = false;
                Entity en = new Entity(GlobalClass.CopiedEntities[i], GlobalClass.CopiedSourceActivePath);
               // if sourece active path and destination active path is not same, it means pasting in another nodecontroller
               // To paste in another nodecontroller, it is necessary to move entity files, so shortcut=false.
               // IsCut=false, because to paste in another nodecontroller files should not be removed from sourecenode
                if (GlobalClass.CopiedSourceActivePath != GlobalClass.PasteDestinationActivePath)
                {
                    en.IsShortCut = false;
                    Duplicatedfile = true;
                    GlobalClass.IsCut = false;
                }
                /// remove entity from source node and set added entity.Isshortcut=false             
                if (GlobalClass.IsCut)
                {
                    en.IsShortCut = false;
                    GlobalClass.CopiedSourceNode.RemoveEntity(GlobalClass.CopiedEntities[i]);
                }
                /// If pasting in another nodecontroller, copyfiles, if files are already in another nodecontroller donot add to entity list
                if (Duplicatedfile)
                {
                    if (en.CopyFilesToFolder(GlobalClass.CopiedSourceActivePath, GlobalClass.PasteDestinationActivePath))
                        Entities.Add(en);
                }
                ///Pasting in same nodecontroller : donot copy files
                else
                    Entities.Add(en);
            }
            //If Nodecontroller is different donot copy nodes 
            if (GlobalClass.CopiedSourceActivePath == GlobalClass.PasteDestinationActivePath)
            {

                for (int i = 0; i < GlobalClass.CopiedNodes.Count; i++)
                {
                    if (GlobalClass.PasteDestinationPath.Contains(GlobalClass.CopiedNodes[i].Name))
                    {
                        MessageBox.Show("Cann't paste to Chile node", GlobalClass.CopiedNodes[i].Name);
                    }
                    else if (GlobalClass.PasteDestinationPath == GlobalClass.CopiedSourceParentPath)
                    {
                        //   MessageBox.Show("Source and Destination Folder is same");
                    }
                    else
                    {
                        Node n = GlobalClass.CopiedNodes[i];
                        n.ActiveFolderPath = ActiveFolderPath;
                        n.Read();
                        n.OldParentName = n.ParentName;
                        n.ParentName = Name;
                        n.Save();
                        Nodes.Add(n);
                        GlobalClass.CopiedSourceNode.RemoveNode(GlobalClass.CopiedNodes[i]);
                    }

                }

                GlobalClass.CopiedSourceNode.Save();
            }


            Save();
            SetRectangles();

            if (GlobalClass.CopiedEntities.Count > 0 || GlobalClass.CopiedNodes.Count > 0)
            {
                GlobalClass.ClearClipborar();
                return true;
            }
            else return false;

        }

      
        internal void Rename(string newdisplayname)
        {
            this.DisplayName = newdisplayname;
        }

        private void RemoveNode(Node node)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Name == node.Name)
                {
                    Nodes.RemoveAt(i);
                    break;
                }
            }
        }
        private void RemoveEntity(Entity entity)
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i].ImageFileName == entity.ImageFileName)
                {
                    Entities.RemoveAt(i);
                    break;
                }
            }
        }

        internal void Copy()
        {
            GlobalClass.IsCut = false;
            GlobalClass.CopiedEntities = GetSelectedEntities();
            GlobalClass.CopiedNodes = GetSelectedNodes();
            GlobalClass.CopiedSourceActivePath = ActiveFolderPath;
            GlobalClass.CopiedSourceNode = this;
            GlobalClass.CopyEntityToClipBoard();
        
        
            
        }
        internal void Cut()
        {
            GlobalClass.IsCut = true;
            GlobalClass.CopiedEntities = GetSelectedEntities();
            GlobalClass.CopiedNodes = GetSelectedNodes();
            GlobalClass.CopiedSourceActivePath = ActiveFolderPath;
            GlobalClass.CopiedSourceNode = this;
            GlobalClass.CopyEntityToClipBoard();

        }
        internal void Refresh(bool Force = false)
        {
            if (Force)
            {
                Point temp = Shiftindex;
                IsLoaded = false;
                ThumbsLoaded = false;
                Read(true);
                Shiftindex = temp;
            }
            SetRectangles(true);
        }
        [NonSerialized]
        int columncount = 0;
        public void SetRectangles(bool Force = false)
        {
            //   if (ComparingRects == false || Force)
            {
                SettingRects = true;
                nodeRects = new List<RectangleF>();
                entityRects = new List<RectangleF>();

                float initial_x = ((GlobalClass.ParentBoundry.Width -GlobalClass.LeftPanelWidth)% GlobalClass.ThumbnailSize.Width) / 2+ GlobalClass.LeftPanelWidth;
                columncount = ((GlobalClass.ParentBoundry.Width - GlobalClass.LeftPanelWidth) / GlobalClass.ThumbnailSize.Width);
                float x = initial_x, y = GlobalClass.TitleHeight;

                for (int i = 0; i < Nodes.Count; i++)
                {
                    nodeRects.Add(new RectangleF(x, y, GlobalClass.ThumbnailSize.Width, GlobalClass.ThumbnailSize.Height));
                    x += GlobalClass.ThumbnailSize.Width;
                    if (x + GlobalClass.ThumbnailSize.Width >= GlobalClass.ParentBoundry.Width)
                    {
                        x = initial_x;
                        y += GlobalClass.ThumbnailSize.Height;
                    }
                }

                for (int i = 0; i < Entities.Count; i++)
                {
                    entityRects.Add(new RectangleF(x, y, GlobalClass.ThumbnailSize.Width, GlobalClass.ThumbnailSize.Height));
                    x += GlobalClass.ThumbnailSize.Width;
                    if (x + GlobalClass.ThumbnailSize.Width >= GlobalClass.ParentBoundry.Width)
                    {
                        x = initial_x;
                        y += GlobalClass.ThumbnailSize.Height;
                    }
                }
                SettingRects = false;
            }
        }

        internal void Flip()
        {
            List<Entity> ens = GetSelectedEntities();
            for (int i = 0; i < ens.Count; i++)
            {
                ens[i].Flip(ActiveFolderPath);
            }
        }

        internal void Rotate()
        {
            List<Entity> ens = GetSelectedEntities();
            for (int i = 0; i < ens.Count; i++)
            {
                ens[i].Rotate(ActiveFolderPath);
            }
        }

        internal void Delete()
        {
        
            List<Node> NodetoDelete = new List<Node>();
            List<Entity> EntitytoDelete = new List<Entity>();
            foreach (Index key in SelectedIndexes)
            {
                if (key.selection == Selection.Node)
                {
                    if (Nodes[key.index].DeleteNode(ActiveFolderPath))
                        NodetoDelete.Add(Nodes[key.index]);
                }
                else if (key.selection == Selection.Entity)
                {
                    if (Entities[key.index].Delete(Name, DisplayName, ActiveFolderPath))
                        EntitytoDelete.Add(Entities[key.index]);
                }
            }
            for (int i = 0; i < NodetoDelete.Count; i++)
            {
                Nodes.Remove(NodetoDelete[i]);
            }
            for (int i = 0; i < EntitytoDelete.Count; i++)
            {
                Entities.Remove(EntitytoDelete[i]);
            }
            SelectedIndexes = new List<Index>();
            SetRectangles(true);

            Save();
        }

        internal void MoveToOldParent()
        {

            List<Node> Nodetomove = new List<Node>();
            List<Entity> Entitytomove = new List<Entity>();
            foreach (Index key in SelectedIndexes)
            {
                if (key.selection == Selection.Node)
                {
                    Nodetomove.Add(Nodes[key.index]);
                }
                else if (key.selection == Selection.Entity)
                {
                    Entitytomove.Add(Entities[key.index]);
                }
            }

            Node destparent;

            for (int i = 0; i < Nodetomove.Count; i++)
            {
                if (Nodetomove[i].IsDeleted && Nodetomove[i].OldParentName != null)
                {
                    if (!File.Exists(Path.Combine(ActiveFolderPath, Nodetomove[i].OldParentName)))
                    {
                        destparent = new Node(ActiveFolderPath, Nodetomove[i].OldParentName, "", Nodetomove[i].OldParentDisplayName);
                        Controller.AddtoRootNode(destparent);
                        destparent.Read();

                    }
                    else
                    {
                        destparent = new Node(ActiveFolderPath, Nodetomove[i].OldParentName, "", Nodetomove[i].OldParentDisplayName);
                        destparent.Read();

                    }


                    Nodetomove[i].ParentName = Nodetomove[i].OldParentName;

                    Nodetomove[i].IsDeleted = false;
                    Nodetomove[i].Save();
                    destparent.Add(Nodetomove[i]);
                    destparent.Save();
                    Nodes.Remove(Nodetomove[i]);
                }
            }

            for (int i = 0; i < Entitytomove.Count; i++)
            {
                if (Entitytomove[i].IsDeleted && Entitytomove[i].OldParentName != null)
                {

                    if (!File.Exists(Path.Combine(ActiveFolderPath, Entitytomove[i].OldParentName)))
                    {
                        destparent = new Node(ActiveFolderPath, Entitytomove[i].OldParentName, "", Entitytomove[i].OldParentDisplayName);
                        Controller.AddtoRootNode(destparent);
                        destparent.Read();
                    }
                    else
                    {
                        destparent = new Node(ActiveFolderPath, Entitytomove[i].OldParentName, "", Entitytomove[i].OldParentDisplayName);
                        destparent.Read();

                    }
                    Entitytomove[i].IsDeleted = false;
                    destparent.Add(Entitytomove[i]);
                    destparent.Save();
                    Entities.Remove(Entitytomove[i]);
                }
            }

            Save();
            SelectedIndexes = new List<Index>();
            SetRectangles(true);

        }


        /// <summary>
        /// It will delete the nodes files of calling node
        /// </summary>
        /// <param name="activenodepath"></param>
        /// <returns></returns>
        public bool DeleteNode(string activenodepath)
        {
            this.ActiveFolderPath = activenodepath;
            if (IsEmptyNode())
            {

                // if (this.IsDeleted)
                DeleteNodeFile();
                //else
                //{
                //    OldParentName = ParentName;                 
                //    IsDeleted = true;
                //    Controller.AddToRecyle(this);
                //}
                return true;
            }
            else
            {
                MessageBox.Show("Cannot Delete Folder " + DisplayName);
                return false;
            }
        }

        private void DeleteNodeFile()
        {
            if (File.Exists(Path.Combine(ActiveFolderPath, Name)))
            {
                File.Delete(Path.Combine(ActiveFolderPath, Name));
      
            }
            if(File.Exists(Path.Combine(ActiveFolderPath, "Cover" + Name)))
            {
                File.Delete(Path.Combine(ActiveFolderPath, "Cover" + Name));
            }
        }

        private bool IsEmptyNode()
        {
            Read();
            if (Nodes.Count == 0 && Entities.Count == 0)
                return true;
            else return false;
        }

        private Node GetNodeFromName(string nodename)
        {
            Node n = new Node(ActiveFolderPath, nodename, "", "");
            //  n.Set("", nodename, "");
            n.Read(true);
            return n;
        }

        internal bool Read(bool loadthumbs = false,bool onlynodes=false)
        {

            try
            {
                if (File.Exists(Path.Combine(ActiveFolderPath, Name)))
                {
                    Node n = SD.Serialization.LoadObject<Node>(Path.Combine(ActiveFolderPath, Name));
                    this.Entities = n.Entities;
                    this.Nodes = n.Nodes;
                    this.Name = n.Name;
                    this.ParentName = n.ParentName;
                    if (n.DisplayName != "")
                        this.DisplayName = n.DisplayName;
                    this.OldParentName = n.OldParentName;
                    this.IsDeleted = n.IsDeleted;
                    this.IsShortCut = n.IsShortCut;
                    this.Shiftindex = n.Shiftindex;
                    LoadInitialThumbnails();
                }
                else
                {
                    this.Entities = new List<Entity>();
                    this.Nodes = new List<Node>();
                    ParentName = null;
                }
                IsLoaded = true;

                if (loadthumbs)
                    ReadAndLoadThumbs(onlynodes);

                return true;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message, "Stil working....");
                return false;
            }
        }



        internal void Add(Node n)
        {
            Nodes.Add(n);
            if (Entities.Count == 0 && Nodes.Count == 2)
            {
                Nodes[0].LoadCover(ActiveFolderPath);
                SetCover(Nodes[0].CoverImage);
            }
        }
        internal void Add(Entity entity)
        {
            Entities.Add(entity);
            if (Nodes.Count == 0 && Entities.Count == 1)
            {
                Entities[0].LoadPicture(ActiveFolderPath);
                SetCover(Entities[0].GetThumbnail());
            }
        }
        private void LoadInitialThumbnails()
        {
            LoadCover(ActiveFolderPath);
            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i].LoadInitialPicture(ActiveFolderPath);

            }
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].LoadInitialThumbnail();
            }
        }

        private void LoadInitialThumbnail()
        {
            CoverImage = GetDefaultCover();
        }

        public void ReadAndLoadThumbs(bool onlynode=false)
        {
            if (IsLoaded == false)
                Read();

            if (this.Entities.Count > 0 || this.Nodes.Count > 0)
            {
                BackgroundWorker bg_loadthumb = new BackgroundWorker();
                bg_loadthumb.DoWork += Bg_loadthumb_DoWork;
                bg_loadthumb.RunWorkerCompleted += Bg_loadthumb_RunWorkerCompleted;
                bg_loadthumb.RunWorkerAsync(onlynode);
            }
        }


        private void Bg_loadthumb_DoWork(object sender, DoWorkEventArgs e)
        {
           if((bool)e.Argument==false)
            LoadEntityThumbnails();           
            LoadNodeCoverThumbnails();

        }

        private void LoadNodeCoverThumbnails()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].LoadCover(ActiveFolderPath);
            }

            ThumbsLoaded = true;
        }



        private void LoadEntityThumbnails()
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i].LoadPicture(ActiveFolderPath);
            }
            ThumbsLoaded = true;
        }
        private void Bg_loadthumb_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GlobalClass.Invalidate();
        }

        public void Save()
        {
            if (ActiveFolderPath == null)
            {
                ActiveFolderPath = Controller.GetActiveFolderPath();
            }
            if (!Directory.Exists(ActiveFolderPath))
                Directory.CreateDirectory(ActiveFolderPath);

            SD.Serialization.SaveObject<Node>(Path.Combine(ActiveFolderPath, Name), this);
        }

        internal void Paint(Graphics graphics)
        {
            if (IsLoaded == false)
                graphics.DrawString("Node is Empty", GlobalClass.InfoFont, Brushes.Black, new Point(10, 10));
            else
            {
                //Paint Nodes
                PaintNodes(graphics);
                //Paint Entities
                PaintEntities(graphics);
                //Paint Selected Indexes : Selected Nodes and Entites
                PaintSelectedEntityAndNodes(graphics);
            }
        }

        internal void SelectNode(Node n)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (n.Name == Nodes[i].Name)
                {
                    SelectedIndexes.Add(new Index(i, Selection.Node));
                    selection = Selection.Node;
                    GlobalClass.SelectedNode = GetSelectedNode();
                    break;
                }
            }

        }

        private void PaintEntities(Graphics graphics)
        {
            for (int i = 0; i < entityRects.Count; i++)
            {
                if (entityRects[i].Y + Shiftindex.Y + GlobalClass.ThumbnailSize.Height > 0 && entityRects[i].Y + Shiftindex.Y < GlobalClass.ParentBoundry.Height)
                {
                    if(Entities.Count>i)
                    Entities[i].Paint(graphics, GetShiftedLocation(entityRects[i].Location));

                    //Paint Hovered Entity
                    if (i == HoveredIndex && HoveredSelection == Selection.Entity)
                        Entities[i].PaintHovered(graphics, GetShiftedLocation(entityRects[i].Location));

                    //paint Delete Entity Info
                    if (Entities[i].IsDeleted)
                        Entities[i].PaintDeletedInfo(graphics, GetShiftedRect(entityRects[i]));
                }
            }
        }

        private void PaintNodes(Graphics graphics)
        {

            for (int i = 0; i < nodeRects.Count; i++)
            {
                if (nodeRects[i].Y + Shiftindex.Y + GlobalClass.ThumbnailSize.Height > 0 && nodeRects[i].Y + Shiftindex.Y < GlobalClass.ParentBoundry.Height)
                {
                    Nodes[i].Paint(graphics, GetShiftedLocation(nodeRects[i].Location));

                    //Paint Hovered Index
                    if (i == HoveredIndex && HoveredSelection == Selection.Node)
                        Nodes[i].PaintHovered(graphics, GetShiftedLocation(nodeRects[i].Location));

                    

                }
            }
        }

        private void PaintDeletedInfo(Graphics graphics, RectangleF rect)
        {
          //  graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Red)), rect);
        }

        private void PaintSelectedEntityAndNodes(Graphics graphics)
        {
            if (SelectedIndexes != null)
            {
                foreach (Index key in SelectedIndexes)
                {
                    if (key.index == -1)
                       return;
                    if (key.selection == Selection.Entity)
                    {
                        PointF p = GetShiftedLocation(entityRects[key.index].Location);
                      if(p.Y+GlobalClass.ThumbnailSize.Height>0 && p.Y<GlobalClass.ParentBoundry.Height)
                        Entities[key.index].PaintSelection(graphics, p);
                    }
                    else if (key.selection == Selection.Node)
                    {
                        PointF p = GetShiftedLocation(nodeRects[key.index].Location);
                        if (p.Y + GlobalClass.ThumbnailSize.Height > 0 && p.Y < GlobalClass.ParentBoundry.Height)
                            Nodes[key.index].PaintSelection(graphics, p);

                    }
                }
            }
        }

        private PointF GetShiftedLocation(PointF location)
        {
            return new PointF(location.X, location.Y + Shiftindex.Y);
        }
        private RectangleF GetShiftedRect(RectangleF rectangleF)
        {
            return new RectangleF(rectangleF.X, rectangleF.Y + Shiftindex.Y, rectangleF.Width, rectangleF.Height);
        }



        private void PaintHovered(Graphics graphics, PointF location)
        {
            graphics.FillPath(new SolidBrush(Color.FromArgb(50, Color.Green)),path);

        }
        internal Entity GetSelectedEntity()
        {
            if (SelectedIndexes.Count >= 1)
            {
                for (int i = SelectedIndexes.Count - 1; i >= 0; i--)
                {
                    if (SelectedIndexes[i].selection == Selection.Entity && SelectedIndexes[i].index<Entities.Count)
                        return Entities[SelectedIndexes[i].index];
                }


            }
            return null;
        }
        /// <summary>
        /// Returns Selected single entity and size and location of entity tags
        /// </summary>
        /// <param name="s">Size of Entity Tags</param>
        /// <param name="p">Location of Entity Tags</param>
        /// <returns></returns>
        internal Entity GetSelectedEntity(out SizeF size, out PointF point)
        {
            size = new SizeF(1, 1);
            point = new PointF();
            if (SelectedIndexes.Count >= 1)
            {
                for (int i = SelectedIndexes.Count - 1; i >= 0; i--)
                {
                    if (SelectedIndexes[i].selection == Selection.Entity)
                    {
                        RectangleF rect = entityRects[SelectedIndexes[i].index];

                        Graphics g = Graphics.FromImage(new Bitmap(1, 1));
                        SizeF s;
                        if (Entities[SelectedIndexes[i].index].Tags == "")

                            s = g.MeasureString("New Entity", GlobalClass.textbox.Font, GlobalClass.ThumbnailSize.Width - 5);
                        else
                            s = g.MeasureString(Entities[SelectedIndexes[i].index].Tags, GlobalClass.textbox.Font, GlobalClass.ThumbnailSize.Width - 5);
                        point = new PointF(rect.X, rect.Y + GlobalClass.ThumbnailSize.Height - s.Height - 5);
                        size = new SizeF(GlobalClass.ThumbnailSize.Width, s.Height + 5);
                    }
                    return Entities[SelectedIndexes[i].index];
                }
            }
            return null;
        }

        internal Node GetSelectedNode()
        {
            if (SelectedIndexes.Count >= 1)
            {
                for (int i = SelectedIndexes.Count - 1; i >= 0; i--)
                {
                    if (SelectedIndexes[i].selection == Selection.Node)
                    {
                        if (Nodes.Count > SelectedIndexes[i].index)
                            return Nodes[SelectedIndexes[i].index];
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Returns Selected single node and size and location of node displayname
        /// </summary>
        /// <param name="s">Size of Node DisplayName</param>
        /// <param name="p">Location of Node DisplayName</param>
        /// <returns></returns>
        internal Node GetSelectedNode(out SizeF s, out PointF p)
        {
            s = new SizeF(1, 1);
            p = new PointF(0, 0);
            if (SelectedIndexes.Count >= 1)
            {
                for (int i = SelectedIndexes.Count - 1; i >= 0; i--)
                {
                    if (SelectedIndexes[i].selection == Selection.Node)
                    {
                        RectangleF rect = nodeRects[SelectedIndexes[i].index];
                        Graphics g = Graphics.FromImage(new Bitmap(10, 10));
                        SizeF size;
                        if (Nodes[SelectedIndexes[i].index].DisplayName == "")
                            size = g.MeasureString("New Node", GlobalClass.textbox.Font, GlobalClass.ThumbnailSize.Width - 5);
                        else
                            size = g.MeasureString(Nodes[SelectedIndexes[i].index].DisplayName, GlobalClass.textbox.Font, GlobalClass.ThumbnailSize.Width - 5);
                        p = new PointF(rect.X, rect.Y + GlobalClass.ThumbnailSize.Height + Shiftindex.Y - size.Height - 5);
                        s = new SizeF(GlobalClass.ThumbnailSize.Width, size.Height + 5);

                        return Nodes[SelectedIndexes[i].index];
                    }
                }
            }
            return null;
        }

        internal List<Node> GetSelectedNodes()
        {
            List<Node> nodes = new List<Node>();
            for (int i = 0; i < SelectedIndexes.Count; i++)
            {
                if (SelectedIndexes[i].selection == Selection.Node)
                    nodes.Add(Nodes[SelectedIndexes[i].index]);
            }
            return nodes;
        }
        internal List<Entity> GetSelectedEntities()
        {
            List<Entity> entity = new List<Entity>();
            for (int i = 0; i < SelectedIndexes.Count; i++)
            {
                if (SelectedIndexes[i].selection == Selection.Entity)
                {
                    if (GlobalClass.IsCut && Entities[SelectedIndexes[i].index].IsShortCut == false || GlobalClass.IsCut == false)
                    {
                        Entities[SelectedIndexes[i].index].OldParentName = this.Name;
                        Entities[SelectedIndexes[i].index].OldParentDisplayName = this.OldParentDisplayName;

                        entity.Add(Entities[SelectedIndexes[i].index]);
                    }
                }
            }
            return entity;
        }
        private void PaintSelection(Graphics graphics, PointF location)
        {
            graphics.DrawPath(GlobalClass.HighlightPen, path);
            graphics.FillPath(GlobalClass.SelectionBrush, path);
        }
        
        private void Paint(Graphics graphics, PointF location)
        {
            path = GlobalClass.GetRoundBoundry(new RectangleF(location.X + 1, location.Y + 1, GlobalClass.ThumbnailSize.Width - 2, GlobalClass.ThumbnailSize.Height - 2), GlobalClass.RoundCornorRadius);
          
            graphics.SetClip(path);
      
            graphics.DrawRectangle(Pens.Black, location.X, location.Y + Shiftindex.Y, GlobalClass.ThumbnailSize.Width, GlobalClass.ThumbnailSize.Height);
            if (CoverImage == null)
                graphics.DrawImage(GetDefaultCover(), location);
            else
            graphics.DrawImage(CoverImage, location);


            size = graphics.MeasureString(DisplayName, GlobalClass.NodeNameFont, GlobalClass.ThumbnailSize.Width - 5);
            graphics.FillRectangle(GlobalClass.NodeNameBGBrush, new RectangleF(location.X, location.Y + GlobalClass.ThumbnailSize.Height + Shiftindex.Y - size.Height - 5, GlobalClass.ThumbnailSize.Width, size.Height + 5));
            graphics.DrawString(DisplayName, GlobalClass.NodeNameFont, Brushes.White, new RectangleF(location.X + GlobalClass.ThumbnailSize.Width / 2 - size.Width / 2, location.Y + GlobalClass.ThumbnailSize.Height + Shiftindex.Y - size.Height - 5 / 2, GlobalClass.ThumbnailSize.Width - 5, size.Height + 5));
            graphics.ResetClip();

            graphics.DrawPath(GlobalClass.BorderColorPen, path);
        }


        internal void MouseMove(Point location)
        {
            if (Drag == false)
                HoveredIndex = FindSelection(location, out HoveredSelection);
            else
            {
                HoveredIndex = FindSelection(location, out selection);
                if (HoveredIndex != -1)
                {

                    AddToSelection(MouseButtons.Middle,true);
                    GlobalClass.SelectedNode = GetSelectedNode();
                    GlobalClass.SelectedEntity = GetSelectedEntity();
                }
            }
        }

     
        internal void MouseUp()
        {
            Drag = false;
        }

        internal void MouseDown(Point location, MouseButtons button)
        {
            Drag = true;
            initialPoint = location;
            mousebutton = button;

            HoveredIndex = FindSelection(location,out selection);
            if (GlobalClass.MultiSelect)
                button = MouseButtons.Middle;
            AddToSelection(button);
            GlobalClass.SelectedNode = GetSelectedNode();
            GlobalClass.SelectedEntity = GetSelectedEntity();
            UpdateShiftIndex();
        }
        private void AddToSelection(MouseButtons button,bool Addonly=false)
        {

            if (HoveredIndex == -1)
            {
                SelectedIndexes = new List<Index>();
                return;
            }
            if (HoveredIndex >= 0 && SelectedIndexes == null)
                SelectedIndexes = new List<Index>();
            if (button == MouseButtons.Left)
            {
                SelectedIndexes = new List<Index>();
                SelectedIndexes.Add(new Index(HoveredIndex, selection));
            }
            else if (button == MouseButtons.Middle)
            {

                Index toadd = new Index(HoveredIndex, selection);
                bool contains = false;
                int inde = -1;
                for (int i = 0; i < SelectedIndexes.Count; i++)
                {
                    if (SelectedIndexes[i].index == toadd.index && SelectedIndexes[i].selection == toadd.selection)
                    {
                        contains = true;
                        inde = i;
                        break;
                    }
                }
                if (contains )
                {
                    if(inde!=-1 && Addonly == false)
                    SelectedIndexes.RemoveAt(inde);
                }
                else
                    SelectedIndexes.Add(toadd);
            }

        }
        internal void MouseWheel(int delta)
        {
            if (delta > 0)
            {
                Shiftindex = new Point(Shiftindex.X, Shiftindex.Y + GlobalClass.ThumbnailSize.Height);
            }
            else

                Shiftindex = new Point(Shiftindex.X, Shiftindex.Y - GlobalClass.ThumbnailSize.Height);
        }
        private int FindSelection(Point location,out Selection selectoption)
        {
            if (SettingRects == false)
            {
                ComparingRects = true;
                RectangleF rect = new RectangleF(location, new Size(1, 1));

                for (int i = 0; i < nodeRects.Count; i++)
                {
                    if (nodeRects[i].Y + Shiftindex.Y + GlobalClass.ThumbnailSize.Height > 0 && nodeRects[i].Y + Shiftindex.Y < GlobalClass.ParentBoundry.Height)
                    {
                        if (rect.IntersectsWith(GetShiftedRect(nodeRects[i])))
                        {
                            selectoption = Selection.Node;
                            return i;
                        }
                    }
                }
                for (int i = 0; i < entityRects.Count; i++)
                {
                    if (entityRects[i].Y + Shiftindex.Y + GlobalClass.ThumbnailSize.Height > 0 && entityRects[i].Y + Shiftindex.Y < GlobalClass.ParentBoundry.Height)
                    {
                        if (rect.IntersectsWith(GetShiftedRect(entityRects[i])))
                        {
                            selectoption = Selection.Entity;
                            return i;
                        }
                    }
                }
                ComparingRects = false;
                selectoption = Selection.None;
                return -1;
            }
            selectoption = Selection.None;
            return -1;

        }
        internal void Next(NodeController.view view)
        {
            if (selection == Selection.None)
            {
                if (nodeRects.Count > 0)
                    selection = Selection.Node;
                else if (entityRects.Count > 0)
                    selection = Selection.Entity;
            }
            if (Nodes.Count == 0 && Entities.Count == 0)
            {
                selection = Selection.None;
                SelectedIndexes = new List<Index>();
                return;
            }
            int index = 0;
            if (SelectedIndexes == null)
                SelectedIndexes = new List<Index>();
            if (SelectedIndexes.Count > 0)
                index = SelectedIndexes.Last().index + 1;
            else
                index = 0;

         

            AddSelectedIndex(view, index);
        }

      
        internal void Previous(NodeController.view view)
        {

            if (selection == Selection.None)
            {
                if (nodeRects.Count > 0)
                    selection = Selection.Node;
                else if (entityRects.Count > 0)
                    selection = Selection.Entity;
            }
            if(Nodes.Count==0 && Entities.Count==0)
            {
                selection = Selection.None;
                SelectedIndexes = new List<Index>();
                return;
            }

            int index = 0;
            if (SelectedIndexes == null)
                SelectedIndexes = new List<Index>();
            if (SelectedIndexes.Count > 0)
                index = SelectedIndexes.Last().index - 1;
            else
                index = 0;

            AddSelectedIndex(view, index);

            
        }
        internal void Up(NodeController.view view)
        {
            if (selection == Selection.None)
            {
                if (nodeRects.Count > 0)
                    selection = Selection.Node;
                else if (entityRects.Count > 0)
                    selection = Selection.Entity;
            }
            if (Nodes.Count == 0 && Entities.Count == 0)
            {
                selection = Selection.None;
                SelectedIndexes = new List<Index>();
                return;
            }
            int index = 0;
            if (SelectedIndexes == null)
                SelectedIndexes = new List<Index>();
            if (SelectedIndexes.Count > 0)
                index = SelectedIndexes.Last().index - columncount;
            else
                index = 0;

            AddSelectedIndex(view, index);
        }
        internal void Down(NodeController.view view)
        {
            if (selection == Selection.None)
            {
                if (nodeRects.Count > 0)
                    selection = Selection.Node;
                else if (entityRects.Count > 0)
                    selection = Selection.Entity;
            }
            if (Nodes.Count == 0 && Entities.Count == 0)
            {
                selection = Selection.None;
                SelectedIndexes = new List<Index>();
                return;
            }
            int index = 0;
            if (SelectedIndexes == null)
                SelectedIndexes = new List<Index>();
            if (SelectedIndexes.Count > 0)
                index = SelectedIndexes.Last().index + columncount;
            else
                index = 0;

            AddSelectedIndex(view, index);
        }
        public void AddSelectedIndex(NodeController.view view, int index)
        {
            //if(index==-1)
            //{
            //    SelectedIndexes = new List<Index>();
            //    return;
            //}
            if (view == NodeController.view.FullImage)
            {
                if (index >= entityRects.Count)
                    index = 0;
                if (index <= -1)
                {
                    if (entityRects.Count > 0)
                        index = entityRects.Count - 1;
                    else
                    {
                        selection = Selection.None;
                        SelectedIndexes = new List<Index>();
                    }
                }
                selection = Selection.Entity;
            }

            else if (view == NodeController.view.Explorer)
            {
                if (selection == Selection.Node)
                {
                    if (index < 0)
                    {
                        if (entityRects.Count > 0)
                        {
                            index = entityRects.Count - 1;
                            selection = Selection.Entity;
                        }
                        else if (nodeRects.Count > 0)
                        {
                            index = nodeRects.Count - 1;
                            selection = Selection.Node;
                        }
                        else
                        {
                            index = -1;
                            selection = Selection.None;
                        }
                    }
                    else if (index >= nodeRects.Count)
                    {
                        index = 0;
                        if (entityRects.Count > 0)
                            selection = Selection.Entity;
                        else
                            selection = Selection.Node;
                    }
                }
                else if (selection == Selection.Entity)
                {
                    if (index < 0)
                    {
                        if (nodeRects.Count > 0)
                        {
                            index = nodeRects.Count - 1;
                            selection = Selection.Node;
                        }
                        else if (entityRects.Count > 0)
                        {
                            index = entityRects.Count - 1;
                            selection = Selection.Entity;
                        }

                        else
                        {
                            index = -1;
                            selection = Selection.None;
                        }

                    }
                    else if (index >= entityRects.Count)
                    {
                        index = 0;
                        if (nodeRects.Count > 0)
                            selection = Selection.Node;
                        else
                            selection = Selection.Entity;
                    }
                }
            }

            if (GlobalClass.MultiSelect)
            {

                Index toadd = new Index(index, selection);
                bool contains = false;
                int inde = -1;
                for (int i = 0; i < SelectedIndexes.Count; i++)
                {
                    if (SelectedIndexes[i].index == toadd.index && SelectedIndexes[i].selection == toadd.selection)
                    {
                        contains = true;
                        inde = i;
                        break;
                    }
                }
                if (contains)
                {
                    if (inde != -1)
                        SelectedIndexes.RemoveAt(inde);
                }
                else
                    SelectedIndexes.Add(toadd);


            }
            else
            {
                SelectedIndexes = new List<Index>();
                SelectedIndexes.Add(new Index(index, selection));
            }
            GlobalClass.SelectedNode = GetSelectedNode();
            GlobalClass.SelectedEntity = GetSelectedEntity();
            UpdateShiftIndex();
        }
        public void UpdateShiftIndex()
        {
            RectangleF selectedrect = new RectangleF(0, 0, 1, 1);
            if (SelectedIndexes.Count == 0)
                return;
            if (SelectedIndexes.Count > 0)
            {
                if (SelectedIndexes.Last().selection == Selection.Entity)
                    selectedrect = entityRects[SelectedIndexes.Last().index];
                else
                if (SelectedIndexes.Last().selection == Selection.Node)
                    selectedrect = nodeRects[SelectedIndexes.Last().index];


            }

            if (selectedrect.Y + Shiftindex.Y + GlobalClass.ThumbnailSize.Height + GlobalClass.TitleHeight > GlobalClass.ParentBoundry.Height)
                Shiftindex = new Point(0, -(int)selectedrect.Y + GlobalClass.ParentBoundry.Height - GlobalClass.ThumbnailSize.Height - GlobalClass.TitleHeight);
            else if (selectedrect.Y + Shiftindex.Y < GlobalClass.TitleHeight)
                Shiftindex = new Point(0, (int)-selectedrect.Y + GlobalClass.TitleHeight);


        }
        internal void Destroy()
        {
            IsLoaded = false;
            ThumbsLoaded = false;
            Nodes = new List<Node>();
            Entities = new List<Entity>();
            SD.Garbage.ClearRAM.Clear();
        }

    }
}
