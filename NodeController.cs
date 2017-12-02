using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vault
{
    public class NodeController
    {
        public string ActiveFolder;
        Node RootNode, Recycle, Favorite,Downloads;
        public Node OpenedNode;
        Tasks tasks;
        NodeViewTitle nodetitle;
        RecectNodes recentnodes = new RecectNodes();
        ContextMenu contextmenu;
        public bool showcontextmenu = false;
       public Image BackgroundImage = null;
        BackgroundWorker worker_bgload;
        public enum view
        {
            Explorer,
            FullImage,
        }

        public enum ExplorerOption
        {
            Root,
            Recycle,
            Favorite
        }

        ExplorerOption option = ExplorerOption.Root;
       public view View = view.Explorer;
        CustomRootFolders customfolders;
        public NodeController(string activefolder)
        {
            this.ActiveFolder = activefolder;
            GlobalClass.TextChanged += GlobalClass_TextChanged;
            nodetitle = new NodeViewTitle(ActiveFolder);
            RootNode = new Node(ActiveFolder, "RootNode", "", "Root Node");      
            Recycle = new Node(ActiveFolder, "RecycleBin", "", "Recycle Bin");
            Recycle.Read();         
            Favorite = new Node(ActiveFolder, "Favorite", "", "Favorite");
            Favorite.Read();
            Downloads = new Node(ActiveFolder, "Downloads", "", "Downloads");
            Downloads.Read();
            OpenNode(RootNode);

            customfolders = new CustomRootFolders(ActiveFolder);
            
            recentnodes.AddNode(ref RootNode, RecentBlock.NodeAction.Default);
            recentnodes.AddNode(ref Favorite, RecentBlock.NodeAction.Default);
            recentnodes.AddNode(ref Recycle, RecentBlock.NodeAction.Default);
            recentnodes.AddNode(ref Downloads, RecentBlock.NodeAction.Default);

            for (int i = 0; i < customfolders.FolderNames.Count; i++)
            {
                Node n = new Node(ActiveFolder, customfolders.FolderNames[i], "", customfolders.FolderNames[i]);
                n.Read();
                recentnodes.AddNode(ref n, RecentBlock.NodeAction.Custom);
            }

            contextmenu = new ContextMenu();
            SetBackgrounImage(Properties.Settings.Default.BackgroundImageName);
     
        }
        bool BgChangeRequested = false;
        private void SetBackgrounImage(string backgroundImageName)
        {
            if (backgroundImageName == "")
                return;
            BackgroundImage = new Bitmap(1, 1);
               
            if (worker_bgload == null)
            {
                worker_bgload = new BackgroundWorker();
                worker_bgload.DoWork += Worker_bgload_DoWork;
                worker_bgload.RunWorkerCompleted += Worker_bgload_RunWorkerCompleted;
            }
            if (!worker_bgload.IsBusy)
            {
                worker_bgload.RunWorkerAsync(backgroundImageName);
            }
            else
            {
                BgChangeRequested = true;   
            }
        }

        private void Worker_bgload_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GlobalClass.Invalidate();
            if(BgChangeRequested)
            {
                worker_bgload.RunWorkerAsync(Properties.Settings.Default.BackgroundImageName);
            }
        }

        private void Worker_bgload_DoWork(object sender, DoWorkEventArgs e)
        {
            BgChangeRequested = false;
            Entity en = new Entity(e.Argument.ToString());
           
            BackgroundImage = en.GetFullImage(ActiveFolder);

            if (Properties.Settings.Default.BackgroundImageSourceRect.Width != 0)
            {
                Image Img = new Bitmap(GlobalClass.ParentBoundry.Width, GlobalClass.ParentBoundry.Height);
                Graphics g = Graphics.FromImage(Img);
                g.DrawImage(BackgroundImage, Properties.Settings.Default.BackgroundImageDestRect, Properties.Settings.Default.BackgroundImageSourceRect, GraphicsUnit.Pixel);
                g.Dispose();
                BackgroundImage = Img;
              
            }
        }

        internal void StartRemainingTask()
        {
            tasks = new Tasks(ActiveFolder);
        }
        public void AddEntityFromDialogBox()
        {
            if (RestrictManualAdd() == false)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "All Images|*.jpg;*.png";
                dialog.Multiselect = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string s in dialog.FileNames)
                        AddTask(new TaskInfo(OpenedNode, s,ActiveFolder));
                }

                tasks.Execute();
            }
        }

        internal void AddNewNode()
        {
            if (RestrictManualAdd() == false)
            {
                Node n = new Node(ActiveFolder, "Node" + GlobalClass.GetRandomString(), OpenedNode.Name, "New Node");
                n.Save();
                OpenedNode.Add(n);
                OpenedNode.Save();
                OpenedNode.Refresh(true);
            }
        }

        public bool RestrictManualAdd()
        {
            if (View != view.Explorer)
                return true;
            if (OpenedNode == null || OpenedNode.Name == "Favorite" || OpenedNode.Name == "RecycleBin")
                return true;
            else return false;
               
        }

        private void AddTask(TaskInfo taskInfo)
        {
            tasks.AddTask(taskInfo);
        }

        internal void SelectAll()
        {
            OpenedNode.SelectAll();
        }

        internal void Paint(Graphics graphics)
        {
            if (View == view.Explorer)
            {
                if (BackgroundImage != null)
                    graphics.DrawImage(BackgroundImage, new PointF(0, 0));

                OpenedNode.Paint(graphics);
                PaintTitle(graphics);
                recentnodes.Paint(graphics);        
                  
            }
            else if (View == view.FullImage)
                FullImage.Paint(graphics);

            if (showcontextmenu)
                contextmenu.Paint(graphics);
        }

        internal void GotoOriginalNodeFromLinkedEntity()
        {
            if (View == view.Explorer)
            {
                if (OpenedNode.GetSelectedEntity() != null && OpenedNode.GetSelectedEntity().IsShortCut)
                {
                    option = ExplorerOption.Root;
                    nodetitle = new NodeViewTitle(ActiveFolder);
                    OpenNode(new Node(ActiveFolder, OpenedNode.GetSelectedEntity().OldParentName, "", ""));
                }
            }
        }

        internal void DeleteCustomFolder()
        {
            if (OpenedNode != null)
            {
                if (customfolders.RemoveFolder(OpenedNode.Name, ActiveFolder))
                {
                    recentnodes.RemoveNode(OpenedNode, RecentBlock.NodeAction.Custom);
                }
            }
        }

        internal void AddCustomFolder()
        {
            GetStringForm st = new GetStringForm("");
            st.Text = "Enter New Folder Name";
            st.ShowDialog();
            if (st.IsTextEntered)
            {
                if (customfolders.AddFolderName(st.TextEntered))
                {
                    Node n = new Node(ActiveFolder, st.TextEntered, "", st.TextEntered);
                    n.Read();
                    recentnodes.AddNode(ref n, RecentBlock.NodeAction.Custom);
                }
                else
                {
                    MessageBox.Show("Oyee! Folder Already Exists !", "Folder Already Exists");
                }
            }
        }

        internal void OpenPasswords()
        {
            GlobalClass.ParentForm.Hide();
            Password_Vault vault = new Password_Vault(ActiveFolder);
            vault.ShowDialog();
            GlobalClass.ParentForm.Show();
        }

        internal void openWriter()
        {
            GlobalClass.ParentForm.Hide();
            Note_Vault.Writer writer = new Note_Vault.Writer(ActiveFolder);
            writer.ShowDialog();
            GlobalClass.ParentForm.Show();
        }

        internal void SetCoverImage()
        {
            if (OpenedNode.ParentName != null)
            {
                Entity en = OpenedNode.GetSelectedEntity();
                if (en != null)
                    OpenedNode.SetCover(en.GetThumbnail());
                else
                {
                    Node n = OpenedNode.GetSelectedNode();
                    if (n != null)
                        OpenedNode.SetCover(n.CoverImage);
                }


            }
        }

        internal void AddToDownloadFolder(Image image)
        {
            Downloads.Read();
            Entity en = new Entity(image, ActiveFolder);
            Downloads.Add(en);
            Downloads.Save();
        }

        internal void ShowFavorite()
        {
            if (View != view.Explorer)
            {
                View = view.Explorer;
                option = ExplorerOption.Recycle;
            }
            if (option != ExplorerOption.Favorite)
            {
                option = ExplorerOption.Favorite;
                OpenNode(Favorite);
            }
            else
            {
                option = ExplorerOption.Root;
                OpenNode(RootNode);
            }
        }

        internal void OpenAllImages()
        {
            AllImages images = new AllImages(ActiveFolder);
            images.ShowDialog();
        }

        internal void AddToFavorite()
        {
            Favorite.ActiveFolderPath = ActiveFolder;
            Favorite.Read();
            foreach (Entity en in OpenedNode.GetSelectedEntities())
            {
                if (en.IsFavorite)
                    en.IsFavorite = false;
                else
                {
                    en.IsFavorite = true;
                    Favorite.Add(new Entity(en, ActiveFolder, true));
                }

            }
            OpenedNode.Save();
            Favorite.Save();
        }

        internal void OpenRootNode()
        {         
            option = ExplorerOption.Root;
            nodetitle = new NodeViewTitle(ActiveFolder);
            OpenNode(RootNode);
        }

        internal void Up()
        {
            if (View == view.Explorer)
            {
                OpenedNode.Up(View);
            }
        }

        internal void Down()
        {
            if (View == view.Explorer)
            {
                OpenedNode.Down(View);
            }
        }       

        internal void SetDesktopBackground()
        {
            try
            {
                Entity en = OpenedNode.GetSelectedEntity();
                if (en != null)
                {
                    if (View == view.Explorer)
                        GlobalClass.Wallpaper.Set(en.GetFullImage(ActiveFolder), GlobalClass.Wallpaper.Style.Fill);
                    else
                    {
                        Image im = en.GetFullImage(ActiveFolder);
                        Image Img = new Bitmap(GlobalClass.ParentBoundry.Width, GlobalClass.ParentBoundry.Height);
                        Graphics g = Graphics.FromImage(Img);
                        g.DrawImage(im, FullImage.DisplayRect, FullImage.SourceRect, GraphicsUnit.Pixel);
                        g.Dispose();
                        GlobalClass.Wallpaper.Set(Img, GlobalClass.Wallpaper.Style.Fill);
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        internal void SetBackground()
        {
            Entity en = OpenedNode.GetSelectedEntity();
            if (en != null)
            {
                if (View == view.Explorer)
                {
                    Properties.Settings.Default.BackgroundImageDestRect = new Rectangle(0, 0, GlobalClass.ParentBoundry.Width, GlobalClass.ParentBoundry.Height);
                    Properties.Settings.Default.BackgroundImageSourceRect = new Rectangle(0, 0, 0, 0);
                }
                else
                {
                    Properties.Settings.Default.BackgroundImageDestRect =new Rectangle((int) FullImage.DisplayRect.X, (int)FullImage.DisplayRect.Y, (int)FullImage.DisplayRect.Width, (int)FullImage.DisplayRect.Height);
                    Properties.Settings.Default.BackgroundImageSourceRect = new Rectangle((int)FullImage.SourceRect.X, (int)FullImage.SourceRect.Y, (int)FullImage.SourceRect.Width, (int)FullImage.SourceRect.Height);

                }

                Properties.Settings.Default.BackgroundImageName = en.ImageFileName;
                Properties.Settings.Default.Save();

                SetBackgrounImage(en.ImageFileName);
            }
        }

        internal void Rename()
        {
            SizeF s = new Size();
            PointF p = new PointF();
            Node n = OpenedNode.GetSelectedNode(out s, out p);
            if (n != null)
            {
                GlobalClass.TextBoxRealatedNode = n;
                GlobalClass.textboxAction = GlobalClass.TextBoxAction.Rename;
                GlobalClass.ShowTextBox(p, s, n.DisplayName);
                return;
            }
            Entity en = OpenedNode.GetSelectedEntity(out s, out p);
            if (en != null)
            {
                GlobalClass.TextBoxRealatedEntity = en;
                GlobalClass.textboxAction = GlobalClass.TextBoxAction.EditTag;
                GlobalClass.ShowTextBox(p, s, en.Tags);
                return;
            }

        }

        internal void ClearBackground()
        {
            BackgroundImage = null;
            Properties.Settings.Default.BackgroundImageName = "";
            Properties.Settings.Default.Save();
        }

        private void GlobalClass_TextChanged(object sender, EventArgs e)
        {

            if (GlobalClass.textboxAction == GlobalClass.TextBoxAction.Rename)
            {
                if (sender != null)
                {
                    GlobalClass.TextBoxRealatedNode.ActiveFolderPath = ActiveFolder;
                    GlobalClass.TextBoxRealatedNode.Read();
                    GlobalClass.TextBoxRealatedNode.Rename(sender.ToString());
                    GlobalClass.TextBoxRealatedNode.Save();
                    OpenedNode.Save();
                }
            }
            else if (GlobalClass.textboxAction == GlobalClass.TextBoxAction.EditTag)
            {
                if (sender != null)
                {
                    GlobalClass.TextBoxRealatedEntity.Tags = sender.ToString();
                    OpenedNode.Save();
                }
            }

        }
        internal void Copy()
        {
            if (View == view.Explorer)
            {
                OpenedNode.Copy();
                GlobalClass.CopiedSourceParentPath = nodetitle.NodePath;
                recentnodes.AddNode(ref OpenedNode, RecentBlock.NodeAction.CopiedFrom);
            }
        }
        internal void Cut()
        {
            if (View == view.Explorer)
            {
                OpenedNode.Cut();
                GlobalClass.CopiedSourceParentPath = nodetitle.NodePath;
                recentnodes.AddNode(ref OpenedNode, RecentBlock.NodeAction.CopiedFrom);
            }
        }
        internal void Paste()
        {
            if (View == view.Explorer)
            {
                GlobalClass.PasteDestinationPath = nodetitle.NodePath;
                GlobalClass.PasteDestinationActivePath = ActiveFolder;
                if (OpenedNode.Paste())
                {
                    recentnodes.AddNode(ref OpenedNode, RecentBlock.NodeAction.DestinationParentNodeForMove);
                }
            }
        }

        internal void Flip()
        {
            OpenedNode.Flip();
            if (View == view.FullImage)
                FullImage.Reload(ActiveFolder);
        }

        internal void Extract()
        {
            ExtractImages extract = new ExtractImages(OpenedNode.GetSelectedEntities(), ActiveFolder);
            extract.ShowDialog();
            extract = null;
            SD.Garbage.ClearRAM.Clear();
        }

        internal void Open(Node node)
        {
            OpenNode(node);
        }

        internal void Rotate()
        {
            OpenedNode.Rotate();
            if (View == view.FullImage)
                FullImage.Reload(ActiveFolder);
        }

        internal void OpenRecycleBin()
        {
            if(View!=view.Explorer)
            {
                View = view.Explorer;
                option = ExplorerOption.Recycle;
            }
            if (option == ExplorerOption.Root)
            {
                option = ExplorerOption.Recycle;
                nodetitle = new NodeViewTitle(ActiveFolder);
                OpenNode(Recycle);
            }
            else
            {
                option = ExplorerOption.Root;
                nodetitle = new NodeViewTitle(ActiveFolder);
                OpenNode(RootNode);
            }
        }

        private void PaintTitle(Graphics graphics)
        {
            nodetitle.Paint(graphics);
        }

        internal void AddEntityFromFolders()
        {
            if (RestrictManualAdd() == false)
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.SelectedPath = Properties.Settings.Default.LastSelectedFolder;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.LastSelectedFolder = dialog.SelectedPath;
                    Properties.Settings.Default.Save();
                    List<string> Dirs = Directory.GetDirectories(dialog.SelectedPath, "*", SearchOption.AllDirectories).ToList();
                    Dirs.Add(dialog.SelectedPath);

                    foreach (string s in Dirs)
                    {
                        Node nodetoadd = new Node(ActiveFolder, "Node" + GlobalClass.GetRandomString(), OpenedNode.Name, new DirectoryInfo(s).Name);
                        AddTask(new TaskInfo(OpenedNode, nodetoadd,ActiveFolder));
                        List<FileInfo> files = new DirectoryInfo(s).GetFiles("*.jpg").ToList();
                        files.AddRange(new DirectoryInfo(s).GetFiles("*.png"));
                        foreach (FileInfo f in files)
                        {
                            AddTask(new TaskInfo(nodetoadd, f.FullName,ActiveFolder));
                        }
                    }
                }

                tasks.Execute();
            }
        }

        internal int GetSelectionCout()
        {
            if (OpenedNode.SelectedIndexes == null)
                return 0;
            else
                return OpenedNode.SelectedIndexes.Count;
        }

        internal void AddToRootNode(Node destparent)
        {
            if (RootNode.IsLoaded == false)
                RootNode.Read();
            destparent.ParentName = RootNode.Name;
            destparent.Save();
            RootNode.Add(destparent);
            RootNode.Save();
        }

        internal void RestoreFromRecyleBin()
        {
            OpenedNode.MoveToOldParent();
        }
        internal void AddToRecyle(Entity entity)
        {
            if (Recycle.IsLoaded == false)
                Recycle.Read();

            Recycle.Add(entity);
            Recycle.Save();

            if (option == ExplorerOption.Recycle)
                OpenedNode = Recycle;
        }

        internal void AddToRecyle(Node node)
        {
            if (Recycle.IsLoaded == false)
                Recycle.Read();

            node.ParentName = Recycle.Name;
            node.Save();
            Recycle.Add(node);
            Recycle.Save();
            if (option == ExplorerOption.Recycle)
                OpenedNode = Recycle;
        }

        internal void Delete()
        {
            if (View == view.Explorer)
            {
                OpenedNode.Delete();
            }
        }

        internal void Refresh(bool Force = false)
        {
            OpenedNode.Refresh(Force);
        }

        String ParentNodeName = "";
        Node parentNode;
        internal void AddNode(String parentnodename, string DisplayName, string Name)
        {
            if (parentnodename != "")
            {
                if (parentNode != null)
                    parentNode.Save();
                parentNode = GetNodeFromName(parentnodename, parentnodename == OpenedNode.Name);
                ParentNodeName = parentnodename;
            }
            Node n = new Node(ActiveFolder, Name, parentNode.Name, DisplayName);
            n.Save();
            parentNode.Add(n);
            // parentNode.Save();
            //if (parentNode.Name == OpenedNode.Name)
            //{
            //    if (OpenedNode.ThumbsLoaded == false)
            //        OpenedNode.ReadAndLoadThumbs();
            //    OpenedNode = parentNode;
            //    OpenedNode.SetRectangles();
            //    GlobalClass.Invalidate();
            //}

        }
        internal void AddEntity(String parentnodename, string fileToAdd)
        {


            if (parentnodename != ParentNodeName)
            {

                if (parentNode != null)
                    parentNode.Save();
                parentNode = GetNodeFromName(parentnodename, parentnodename == OpenedNode.Name);
                ParentNodeName = parentnodename;
            }
           parentNode.Add(new Entity(fileToAdd, ActiveFolder));
          
            // parentNode.Save();
            //if (parentNode.Name == OpenedNode.Name)
            //{
            //    OpenedNode = parentNode;
            //    OpenedNode.SetRectangles();
            //    GlobalClass.Invalidate();
            //}

        }

        internal void SaveLastParentNodeUpdated()
        {
            if (parentNode != null)
            {
                parentNode.Save();
                tasks.Save();
            }
        }
        internal void MouseMove(Point location)
        {

             if (showcontextmenu)
                {
                    contextmenu.MouseMove(location);
                    if(contextmenu.IsMenuSelected)
                    {
                        if (contextmenu.HoveredIndex == -1)
                            showcontextmenu = false;
                    }
                    return;
                }

            if (View == view.Explorer)
            {               
                if (location.X >= GlobalClass.LeftPanelWidth)
                    OpenedNode.MouseMove(location);
                else if (location.X < GlobalClass.LeftPanelWidth && location.Y > GlobalClass.TitleHeight)
                    recentnodes.MouseMove(location);
            }
            else if (View == view.FullImage)
                FullImage.MouseMove(location);
        }



        internal void MouseWheel(int delta,Point p)
        {
            if (View == view.Explorer)
            {
                if (p.X >= GlobalClass.LeftPanelWidth)
                    OpenedNode.MouseWheel(delta);
                else if (p.X < GlobalClass.LeftPanelWidth && p.Y > GlobalClass.TitleHeight)
                    recentnodes.MouseWheel(delta, p);
            }
            else if (View == view.FullImage)
                FullImage.MouseWheel(delta);
        }

        internal void MouseDown(Point location, MouseButtons button)
        {
            if (showcontextmenu)
            {
                if (new Rectangle(location, new Size(1, 1)).IntersectsWith(contextmenu.regionRect))
                {
                    contextmenu.MouseDown(location);
                    return;
                }
                showcontextmenu = false;
            }
            if (button == MouseButtons.Right)
            {
                contextmenu.SetLocation(location);
                showcontextmenu = true;
            }
            else
            {
                showcontextmenu = false;
            }
            if (View == view.Explorer)
            { 
               
                if (location.X >= GlobalClass.LeftPanelWidth)
                    OpenedNode.MouseDown(location, button);
                else if (location.X < GlobalClass.LeftPanelWidth && location.Y > GlobalClass.TitleHeight)
                    recentnodes.MouseDown(location);
                
            }
            else if (View == view.FullImage)
                FullImage.MouseDown(location);
        }

        internal void MouseUp(Point location)
        {
           if (showcontextmenu)
            {
                contextmenu.MouseUp();
            }     
              
            if (View == view.Explorer)
            {
                OpenedNode.MouseUp();
            }
            else if (View == view.FullImage)
                FullImage.MouseUp(location);
        }

        internal void MouseDoubleClick(Point location)
        {
            if (showcontextmenu)
            {
                if (new Rectangle(location, new Size(1, 1)).IntersectsWith(contextmenu.regionRect))
                {
                    contextmenu.MouseDown(location);
                    return;
                }
                showcontextmenu = false;
            }

            if (View == view.Explorer)
            {
                
                OpenNodeOrEntity();
            }
            else if (View == view.FullImage)
                FullImage.MouseDoubleClick(location);
        }

        private void OpenNodeOrEntity()
        {
            if (OpenedNode.SelectedIndexes != null && OpenedNode.SelectedIndexes.Count > 0)
            {
                if (OpenedNode.selection == Node.Selection.Entity)
                {
                    FullImage.LoadEntity(OpenedNode.GetSelectedEntity(), ActiveFolder);

                    View = view.FullImage;
                }
                else if (OpenedNode.selection == Node.Selection.Node)
                {
                    View = view.Explorer;
                    OpenNode(OpenedNode.GetSelectedNode());
                }
            }
        }
        private void OpenNode(Node node)
        {
            if (OpenedNode==null || OpenedNode.Name != node.Name)
            {
                GlobalClass.SelectedEntity = null;
                GlobalClass.SelectedNode = null;
                node.ActiveFolderPath = ActiveFolder;
                node.Read(true);
                nodetitle.AddNodeName(node.DisplayName, node.Name);

                if (OpenedNode != null)
                {
                    if (OpenedNode.Name != ParentNodeName)
                        OpenedNode.Destroy();
                }
                OpenedNode = node;
                OpenedNode.SelectedIndexes = new List<Index>();
                OpenedNode.SetRectangles();
                if (OpenedNode.Name != "Favorite" && OpenedNode.Name != "RecycleBin")
                    recentnodes.AddNode(ref node, RecentBlock.NodeAction.Opened);
                GlobalClass.Invalidate();
            }
        }

        internal void GoBack()
        {
            if (View == view.Explorer)
            {
                if (OpenedNode.ParentName != null && OpenedNode.ParentName != "")
                {
                    nodetitle.RemoveName(OpenedNode.DisplayName, OpenedNode.Name);
                    Node n = OpenedNode;
                    if (OpenedNode.Name != ParentNodeName)
                        OpenedNode.Destroy();

                    OpenedNode = GetNodeFromName(n.ParentName, true);
                    OpenedNode.SelectNode(n);

                    OpenedNode.SetRectangles();
                    OpenedNode.UpdateShiftIndex();
                }
            }
            else if (View == view.FullImage)
            {
                View = view.Explorer;
            }
        }

        public Node GetNodeFromName(string nodename, bool loadthumbs)
        {
            if (nodename == "")
                MessageBox.Show(nodename);
            Node n = new Node(ActiveFolder, nodename, "", "");
            n.Read(loadthumbs);
            return n;
        }

        internal void EnterDown()
        {
            if (View == view.Explorer)
            {
                OpenNodeOrEntity();
            }
        }

        internal void Left()
        {
            OpenedNode.Previous(View);
            if (View == view.FullImage)
                FullImage.LoadEntity(OpenedNode.GetSelectedEntity(), ActiveFolder);
        }

        internal void Right()
        {
            OpenedNode.Next(View);
            if (View == view.FullImage)
                FullImage.LoadEntity(OpenedNode.GetSelectedEntity(), ActiveFolder);
        }
    }
}
