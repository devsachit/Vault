using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vault.Copy_Image;

namespace Vault
{
  public  class Controller
    {
        public static NodeController nodeController;
        public static List<NodeController> controllers;
        public static CopyController copycontroller;
        private static ControllerView _view = ControllerView.Menu;

        public enum ControllerView
        {
            NodeController,
            Lock,
            Menu,
        }
        static ControllerView view
        {
            get { return _view; }
            set
            {
                if (value == ControllerView.Lock)
                {
                    LockDesign.Initialize();
                    LockDesign.PreviousViewState = _view;
                }
                else if(value==ControllerView.Menu)
                {
                        Menu.Initialize();
                        Menu.PreviousViewState = _view;
                    
                }
                _view = value;
            }
        }
        internal static void OpenSelectedNodeController()
        {
           
            view = ControllerView.Lock;
          
            nodeController = new NodeController(Properties.Settings.Default.VaultPath);    
            nodeController.StartRemainingTask();
            GlobalClass.ChangeNodecontroller(Properties.Settings.Default.VaultPath);
            copycontroller = null;
            copycontroller = new CopyController();
            controllers = new List<NodeController>();
            controllers.Add(nodeController);
            GlobalClass.TextChanged += LockDesign.TextChanged;
         
        }

        internal static void SetView(ControllerView previousViewState)
        {
            view = previousViewState;
            GlobalClass.Invalidate();
        }

    
        internal static void OpenNodeController(NodeController nController)
        {
            nodeController = nController;
            nodeController.StartRemainingTask();
            GlobalClass.ChangeNodecontroller(Properties.Settings.Default.VaultPath);
            GlobalClass.Invalidate();
        }
        internal static void MouseMove(Point location)
        {
            if (view == ControllerView.NodeController)
                nodeController.MouseMove(location);
            else if (view == ControllerView.Lock)
                LockDesign.MouseMove(location);
            else if (view == ControllerView.Menu)
                Menu.MouseMove(location);
            GlobalClass.Invalidate();
        }

        internal static void MouseDown(Point location, MouseButtons button)
        {
            
            if (view == ControllerView.NodeController)
            {
                GlobalClass.Hidetextbox();
                nodeController.MouseDown(location, button);
            }
            else if(view==ControllerView.Lock)
            {
                LockDesign.MouseDown(location, button);
            }
            else if (view == ControllerView.Menu)
                Menu.MouseDown(location,button);
            GlobalClass.Invalidate();
        }

        internal static void MouseUp(Point location)
        {
            if (view == ControllerView.NodeController)
                nodeController.MouseUp(location);
            else if (view == ControllerView.Lock)
            {
                LockDesign.MouseUp(location);
            }
            else if (view == ControllerView.Menu)
                Menu.MouseUp(location);
            GlobalClass.Invalidate();
        }      

        internal static void MouseDoubleClick(Point location,MouseButtons btn)
        {

            if(btn==MouseButtons.Right)
            {
                if (GlobalClass.ParentForm.FormBorderStyle == FormBorderStyle.Fixed3D)
                    GlobalClass.ParentForm.FormBorderStyle = FormBorderStyle.None;
                else
                    GlobalClass.ParentForm.FormBorderStyle = FormBorderStyle.Fixed3D;
            }
            if (view == ControllerView.NodeController)
            {
                nodeController.MouseDoubleClick(location);
            
            }
            else if (view == ControllerView.Lock)
            {
                LockDesign.MouseDoubleClick(location);
            }
            else if (view == ControllerView.Menu)
                Menu.MouseDoubleClick(location);
            GlobalClass.Invalidate();
        }

        internal static void OpenSettings()
        {
         
        }

        internal static void MouseWheel(int delta,Point p)
        {
            
            if (view == ControllerView.NodeController)
                nodeController.MouseWheel(delta, p);
            else if (view == ControllerView.Lock)
            {
                LockDesign.MouseWheel(delta,p);
            }
            else if (view == ControllerView.Menu)
                Menu.MouseWheel(delta,p);
            GlobalClass.Invalidate();
        }

        internal static void Restart()
        {
            Application.Restart();
        }

        internal static void GoBack()
        {

            nodeController.GoBack();
            GlobalClass.Invalidate();
        }


        internal static void EnterDown()
        {
            nodeController.EnterDown();
            GlobalClass.Invalidate();
        }

        internal static void KeyDown(object sender, KeyEventArgs e)
        {
            if (view == ControllerView.NodeController)
            {
                if (e.KeyCode == Keys.Control || e.Modifiers == Keys.Control)
                {
                    GlobalClass.MultiSelect = true;
                    if (e.KeyCode == Keys.I)
                        Controller.AddNewImages();
                    else if (e.KeyCode == Keys.N)
                        Controller.AddNewNode();
                    else if (e.KeyCode == Keys.D)
                        Controller.AddFolders();
                    else if (e.KeyCode == Keys.R)
                        Controller.OpenRecycleBin();
                    else if (e.KeyCode == Keys.A)
                        Controller.SelectAll();
                    else if (e.KeyCode == Keys.C)
                        Controller.Copy();
                    else if (e.KeyCode == Keys.X)
                        Controller.Cut();
                    else if (e.KeyCode == Keys.V)
                        Controller.Paste();
                    else if (e.KeyCode == Keys.O)
                        Controller.GotoOriginalNodeFromLinkedEntity();
                    else if (e.KeyCode == Keys.Q)
                        Controller.ToggleGraphicsQuality();
                    else if (e.KeyCode == Keys.T)
                        Controller.SetCoverImage();
                    else if (e.KeyCode == Keys.F5)
                        Controller.Refresh(true);
                    else if (e.KeyCode == Keys.Right)
                        Controller.Right();
                    else if (e.KeyCode == Keys.Left)
                        Controller.Left();
                    else if (e.KeyCode == Keys.Up)
                        Controller.Up();
                    else if (e.KeyCode == Keys.Down)
                        Controller.Down();
                    else if (e.KeyCode == Keys.F)
                        Controller.ShowFavorite();
                    else if (e.KeyCode == Keys.Tab)
                        Controller.DeleteCustomFolder();
                    else if (e.KeyCode == Keys.W)
                        Controller.OpenWriter();
                    else if (e.KeyCode == Keys.P)
                        Controller.OpenPasswords();
                    else if (e.KeyCode == Keys.S)
                        Controller.OpenAllImages();
                }
                else if (e.KeyCode == Keys.Alt || e.Modifiers == Keys.Alt)
                {
                    if (e.KeyCode == Keys.Enter)
                        Controller.ShowProperties();
                }
                else if (e.KeyCode == Keys.Back)
                    Controller.GoBack();
                else if (e.KeyCode == Keys.Enter)
                    Controller.EnterDown();
                else if (e.KeyCode == Keys.Right)
                    Controller.Right();
                else if (e.KeyCode == Keys.Left)
                    Controller.Left();
                else if (e.KeyCode == Keys.F5)
                    Controller.Refresh();
                else if (e.KeyCode == Keys.Escape)
                {
                
                    Hide();
                }
                else if (e.KeyCode == Keys.Delete)
                    Controller.Delete();
                else if (e.KeyCode == Keys.Home)
                    Controller.RestoreFromRecyleBin();
                else if (e.KeyCode == Keys.F2)
                    Controller.Rename();
                else if (e.KeyCode == Keys.F6)
                    Controller.AddToFavorite();
                else if (e.KeyCode == Keys.Up)
                    Controller.Up();
                else if (e.KeyCode == Keys.Down)
                    Controller.Down();
                else if (e.KeyCode == Keys.Tab)
                    Controller.AddCustomFolder();
                else if (e.KeyCode == Keys.F12)
                {
                    OpenNodeControllerSettings();
                }
                else if (e.KeyCode == Keys.PageUp)
                {
                    Controller.ChangeOpenedNodeControllers(false);
                }
                else if (e.KeyCode == Keys.PageDown)
                {
                    Controller.ChangeOpenedNodeControllers(true);
                }
                else if (e.KeyCode == Keys.Space)
                {
                    view = ControllerView.Lock;
                }
            }
            else if (view == ControllerView.Lock)
            {
                LockDesign.KeyDown(e.KeyCode);
            }
            else if (view == ControllerView.Menu)
                Menu.KeyDown(e.KeyCode);
        }

        private static void OpenAllImages()
        {

            nodeController.OpenAllImages();
            GlobalClass.Invalidate();
        }

        private static void ShowProperties()
        {
            if(GlobalClass.SelectedEntity!=null || GlobalClass.SelectedNode!=null)
            {
                ObjectProperties p = new ObjectProperties(nodeController.ActiveFolder);
                p.LoadObject();
                p.Show();
               
            }
        }

        public static void ExitProgram()
        {
           
                GlobalClass.DoActionBeforeExiting();
                GlobalClass.ParentForm.Dispose();
            
        }

        public static void OpenPasswords()
        {
            
            if (view == ControllerView.NodeController || view==ControllerView.Menu)
                nodeController.OpenPasswords();
        }

        public static void OpenWriter()
        {
            if (view == ControllerView.NodeController || view == ControllerView.Menu)
                nodeController.openWriter();       
        }

        public static void OpenNodeControllerSettings()
        {
            NodeControllerSettings set = new NodeControllerSettings();
            set.ShowDialog();
        }

        private static void ChangeOpenedNodeControllers(bool next)
        {
            if(controllers.Count==1)
            {
                OpenNodeControllerSettings();
            }
            int currentindex = -1;
            if (controllers.Count >= 2)
            {
                for (int i = 0; i < controllers.Count; i++)
                {
                    if (controllers[i].ActiveFolder == Properties.Settings.Default.VaultPath)
                    {
                        currentindex = i;
                    }
                }
            }
            if (next)
            {
                currentindex++;
                if (currentindex >= controllers.Count)
                    currentindex = 0;
            }
            else
            {
                currentindex--;
                if(currentindex<=-1)
                {
                    currentindex = controllers.Count - 1;
                }
            }

            Properties.Settings.Default.VaultPath = controllers[currentindex].ActiveFolder;
            Properties.Settings.Default.Save();
            Controller.OpenNodeController(Controller.controllers[currentindex]);
            GlobalClass.Invalidate();
        }

        public static void DeleteCustomFolder()
        {
            nodeController.DeleteCustomFolder();
            GlobalClass.Invalidate();
        }

        public static void AddCustomFolder()
        {
            nodeController.AddCustomFolder();
            GlobalClass.Invalidate();
        }

        internal static void AddImageToDownloadFolder(Image image)
        {
            nodeController.AddToDownloadFolder(image);
            GlobalClass.Invalidate();
        }

        internal static void OpenRootNode()
        {
            nodeController.OpenRootNode();
            GlobalClass.Invalidate();
        }

        public static void Cut()
        {
            nodeController.Cut();
            GlobalClass.Invalidate();
        }

        public static void Down()
        {
            nodeController.Down();
            GlobalClass.Invalidate();
        }

        public static void Up()
        {
            nodeController.Up();
            GlobalClass.Invalidate();
        }

        internal static void KeyUp(object sender, KeyEventArgs e)
        {
            
            if (view == ControllerView.NodeController)
            {
                if (e.KeyCode != Keys.Left && e.KeyCode != Keys.Right)
                {
                    GlobalClass.MultiSelect = false;
                }
            }
            else if(view==ControllerView.Lock)
            {
                LockDesign.KeyUp(e.KeyCode);
            }
            else if (view == ControllerView.Menu)
                Menu.KeyUp(e.KeyCode);
         
        }

        internal static void Right()
        {
            nodeController.Right();
            GlobalClass.Invalidate();
        }

        internal static void Left()
        {
            nodeController.Left();
            GlobalClass.Invalidate();
        }
        internal static bool AddNode(string parentnodename, string displayName, string name,string activefolderpath)
        {
            if (nodeController.ActiveFolder != activefolderpath)
                return false;
            nodeController.AddNode(parentnodename, displayName, name);
            return true;
        }
        internal static bool AddEntity(string parentnodename, string fileToAdd, string activefolderpath)
        {
            if (nodeController.ActiveFolder != activefolderpath)
                return false;
            nodeController.AddEntity(parentnodename, fileToAdd);
            return true;
        }

        internal static void Hide()
        {
            Lock() ;
            GlobalClass.ParentForm.Hide();

        }

        internal static void Lock()
        {
            view = ControllerView.Lock;
            GlobalClass.Invalidate();
        }

        internal static void ShowMenu()
        {
            view = ControllerView.Menu;
            GlobalClass.Invalidate();
        }

        internal static void ClearBackground()
        {
            nodeController.ClearBackground();
            GlobalClass.Invalidate();
        }

        internal static void OpenNodeOrEntity()
        {
            nodeController.EnterDown();
            GlobalClass.Invalidate();
        }

        internal static void SetDesktopBackground()
        {
            nodeController.SetDesktopBackground();
            GlobalClass.Invalidate();
        }

        internal static void SetBackground()
        {
            nodeController.SetBackground();
            GlobalClass.Invalidate();
        }

        internal static void SelectAll()
        {
            nodeController.SelectAll();
            GlobalClass.Invalidate();
         }

        internal static void Paste()
        {
            nodeController.Paste();
            GlobalClass.Invalidate();
        }             

        internal static void GotoOriginalNodeFromLinkedEntity()
        {
            nodeController.GotoOriginalNodeFromLinkedEntity();
            GlobalClass.Invalidate();
        }

        internal static void SetCoverImage()
        {
            nodeController.SetCoverImage();
            GlobalClass.Invalidate();
        }

        internal static void SaveLastParentNodeUpdated()
        {
            if(controllers!=null)
            for(int i=0;i<controllers.Count;i++)
            controllers[i].SaveLastParentNodeUpdated();
        }

        internal static void ToggleGraphicsQuality()
        {
            if (GlobalClass.HighGraphicsQuality)
                GlobalClass.HighGraphicsQuality = false;
            else
                GlobalClass.HighGraphicsQuality = true;
            GlobalClass.Invalidate();
        }

        internal static void ShowFavorite()
        {
            nodeController.ShowFavorite();
            GlobalClass.Invalidate();
        }

        internal static void AddToFavorite()
        {
            nodeController.AddToFavorite();
            GlobalClass.Invalidate();
        }

        internal static void Copy()
        {
            nodeController.Copy();
            GlobalClass.Invalidate();
        }       

        internal static void OpenNode(Node node)
        {
            nodeController.Open(node);
            GlobalClass.Invalidate();
        }

        internal static void AddNewImages()
        {
            nodeController.AddEntityFromDialogBox();
            GlobalClass.Invalidate();
        }

        internal static void Rename()
        {
            nodeController.Rename();
            GlobalClass.Invalidate();
        }

        internal static void Refresh(bool Force = false)
        {
            nodeController.Refresh(Force);
            GlobalClass.Invalidate();
        }

        internal static void Delete()
        {
            nodeController.Delete();
            GlobalClass.Invalidate();
        }

        internal static void RestoreFromRecyleBin()
        {
            nodeController.RestoreFromRecyleBin();
            GlobalClass.Invalidate();
        }

        internal static void AddNewNode()
        {
            nodeController.AddNewNode();
            GlobalClass.Invalidate();
        }

        internal static void AddFolders()
        {
            nodeController.AddEntityFromFolders();
            GlobalClass.Invalidate();
        }

        internal static void Paint(Graphics graphics)
        {         
           
            if (view==ControllerView.NodeController)
            {
                nodeController.Paint(graphics);
            }
           else  if (view == ControllerView.Menu)
                Menu.Paint(graphics);
         else   if (view == ControllerView.Lock)
                LockDesign.Paint(graphics);

        }
        internal static void OpenRecycleBin()
        {
            nodeController.OpenRecycleBin();
            GlobalClass.Invalidate();
        }
  
        internal static void AddToRecyle(Node node)
        {
            nodeController.AddToRecyle(node);
            GlobalClass.Invalidate();                       
        }

        internal static void AddToRecycle(Entity entity)
        {
            nodeController.AddToRecyle(entity);
            GlobalClass.Invalidate();
        }

        internal static string GetActiveFolderPath()
        {
            return nodeController.ActiveFolder;
        }

        internal static void AddtoRootNode(Node destparent)
        {
            nodeController.AddToRootNode(destparent);
        }

        internal static int GetSelectionCount()
        {
            return nodeController.GetSelectionCout();
        }

        internal static bool IsOpenNodeManualRestrict()
        {
            if (nodeController == null)
                return true;
            return nodeController.RestrictManualAdd();
        }

        internal static void Flip()
        {
            nodeController.Flip();
            GlobalClass.Invalidate();
        }

        internal static void Extract()
        {
            nodeController.Extract();
            GlobalClass.Invalidate();
        }

        internal static void Rotate()
        {
            nodeController.Rotate();
            GlobalClass.Invalidate();
        }
    }
}
