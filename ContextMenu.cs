using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault
{
  public  class ContextMenu
    {
        public Image image;

        public bool IsMenuSelected { get; private set; }
        Point Location { get; set; }
        public Rectangle regionRect { get { return new Rectangle(Location, new Size(361, 304)); } }

        public int HoveredIndex = -1;
        public int SelectedIndex = -1;

        public Size EntitySize = new Size(90, 38);
        public List<MenuEntity> menus;
        public ContextMenu()
        {
            image = Properties.Resources.menu;
            menus = new List<MenuEntity>();
            menus.Add(new MenuEntity(0, 0, EntitySize, MenuEntity.MenuAction.GoBack));
            menus.Add(new MenuEntity(EntitySize.Width, 0, EntitySize, MenuEntity.MenuAction.Open));
            menus.Add(new MenuEntity(EntitySize.Width*2, 0, EntitySize, MenuEntity.MenuAction.GotoFavorite));
            menus.Add(new MenuEntity(EntitySize.Width*3, 0, EntitySize, MenuEntity.MenuAction.GotoRecyleBin));

            menus.Add(new MenuEntity(0, EntitySize.Height, EntitySize, MenuEntity.MenuAction.SelectAll));
            menus.Add(new MenuEntity(EntitySize.Width, EntitySize.Height, EntitySize, MenuEntity.MenuAction.Copy));
            menus.Add(new MenuEntity(EntitySize.Width * 2, EntitySize.Height, EntitySize, MenuEntity.MenuAction.Cut));
            menus.Add(new MenuEntity(EntitySize.Width * 3, EntitySize.Height, EntitySize, MenuEntity.MenuAction.Paste));


            menus.Add(new MenuEntity(0, EntitySize.Height*2, EntitySize, MenuEntity.MenuAction.SetCover));
            menus.Add(new MenuEntity(EntitySize.Width, EntitySize.Height*2, EntitySize, MenuEntity.MenuAction.SetBG));
            menus.Add(new MenuEntity(EntitySize.Width * 2, EntitySize.Height*2, EntitySize, MenuEntity.MenuAction.ClearBG));
            menus.Add(new MenuEntity(EntitySize.Width * 3, EntitySize.Height*2, EntitySize, MenuEntity.MenuAction.SetDesktopBG));

            menus.Add(new MenuEntity(0, EntitySize.Height * 3, EntitySize, MenuEntity.MenuAction.Left));
            menus.Add(new MenuEntity(EntitySize.Width, EntitySize.Height * 3, EntitySize, MenuEntity.MenuAction.Right));
            menus.Add(new MenuEntity(EntitySize.Width * 2, EntitySize.Height * 3, EntitySize, MenuEntity.MenuAction.Down));
            menus.Add(new MenuEntity(EntitySize.Width * 3, EntitySize.Height * 3, EntitySize, MenuEntity.MenuAction.Up));

            menus.Add(new MenuEntity(0, EntitySize.Height * 4, EntitySize, MenuEntity.MenuAction.NewNode));
            menus.Add(new MenuEntity(EntitySize.Width, EntitySize.Height * 4, EntitySize, MenuEntity.MenuAction.AddImages));
            menus.Add(new MenuEntity(EntitySize.Width * 2, EntitySize.Height * 4, EntitySize, MenuEntity.MenuAction.AddFolders));
            menus.Add(new MenuEntity(EntitySize.Width * 3, EntitySize.Height * 4, EntitySize, MenuEntity.MenuAction.Delete));


            menus.Add(new MenuEntity(0, EntitySize.Height * 5, EntitySize, MenuEntity.MenuAction.Rename));
            menus.Add(new MenuEntity(EntitySize.Width, EntitySize.Height * 5, EntitySize, MenuEntity.MenuAction.Refresh));
            menus.Add(new MenuEntity(EntitySize.Width * 2, EntitySize.Height * 5, EntitySize, MenuEntity.MenuAction.Reload));
            menus.Add(new MenuEntity(EntitySize.Width * 3, EntitySize.Height * 5, EntitySize, MenuEntity.MenuAction.Restore));

            menus.Add(new MenuEntity(0, EntitySize.Height * 6, EntitySize, MenuEntity.MenuAction.Rotate));
            menus.Add(new MenuEntity(EntitySize.Width, EntitySize.Height * 6, EntitySize, MenuEntity.MenuAction.Flip));
            menus.Add(new MenuEntity(EntitySize.Width * 2, EntitySize.Height * 6, EntitySize, MenuEntity.MenuAction.Extract));
            menus.Add(new MenuEntity(EntitySize.Width * 3, EntitySize.Height * 6, EntitySize, MenuEntity.MenuAction.Locate));


            menus.Add(new MenuEntity(0, EntitySize.Height * 7, EntitySize, MenuEntity.MenuAction.GotoMenu));
            menus.Add(new MenuEntity(EntitySize.Width, EntitySize.Height * 7, EntitySize, MenuEntity.MenuAction.Lock));
            menus.Add(new MenuEntity(EntitySize.Width * 2, EntitySize.Height *7, EntitySize, MenuEntity.MenuAction.Hide));
            menus.Add(new MenuEntity(EntitySize.Width * 3, EntitySize.Height * 7, EntitySize, MenuEntity.MenuAction.Exit));
            CheckAvailability();
        }

        private void CheckAvailability()
        {
            for (int i = 0; i < menus.Count; i++)
            {
                menus[i].CheckAvailability();
            }
        }

        internal void Paint(Graphics graphics)
        {
            if (HoveredIndex != -1 && HoveredIndex == SelectedIndex)
                menus[HoveredIndex].PaintSelection(graphics, GetLocatedRect(menus[HoveredIndex].Rectangle));
              
            else if (HoveredIndex != -1)
                menus[HoveredIndex].PaintHovered(graphics, GetLocatedRect(menus[HoveredIndex].Rectangle));
           
            graphics.DrawImage(image, regionRect);

        for(int i=0;i<menus.Count;i++)
            menus[i].PaintDisable(graphics, GetLocatedRect(menus[i].Rectangle));

        }
        public void SetLocation(Point location)
        {
            IsMenuSelected = false;
            this.Location = location;
            ValidateLocation();
            CheckAvailability();
        }

        private void ValidateLocation()
        {
            int x = Location.X;int y = Location.Y;
            int w = regionRect.Width; int h = regionRect.Height;


            if(x+w>GlobalClass.ParentBoundry.Width)
            {
                x = GlobalClass.ParentBoundry.Width - w;
            }
            if(y+h>GlobalClass.ParentBoundry.Height)
            {
                y = GlobalClass.ParentBoundry.Height - h;
            }
            this.Location = new Point(x, y);
        }

        internal void MouseDown(Point location)
        {
            SelectedIndex = FindSelection(location);
            if (SelectedIndex != -1)
            {
                menus[SelectedIndex].DoAction();
                IsMenuSelected = true;
                CheckAvailability();
            }
        }

        internal void MouseMove(Point location)
        {
            HoveredIndex = FindSelection(location);
        }

        private int FindSelection(Point location)
        {
            Rectangle rect = new Rectangle(location, new Size(1, 1));
            for(int i=0;i<menus.Count;i++)
            {
               if(rect.IntersectsWith(GetLocatedRect( menus[i].Rectangle)))
                {
                    return i;
                }
            }
            return -1;
        }

        private Rectangle GetLocatedRect(Rectangle rectangle)
        {
            return new Rectangle(rectangle.X + Location.X, rectangle.Y + Location.Y, rectangle.Width, rectangle.Height);
        }

        internal void MouseUp()
        {
            SelectedIndex = -1;
        }
    }

    public class MenuEntity
    {
        public Rectangle Rectangle;
        public MenuAction menuaction;
        bool enable = true;
        public enum MenuAction
        {
            GoBack,
            GotoFavorite,
            GotoRootNode,
            GotoRecyleBin,
            GotoMenu,
            SelectAll,
            Copy,
            Cut,
            Paste,
            SetCover,
            SetBG,
            SetDesktopBG,
            ClearBG,
            Left,
            Right,
            Down,
            Up,
            Open,
            NewNode,
            AddImages,
            AddFolders,
            Delete,
            Rename,
            Refresh,
            Reload,
            Restore,
            Rotate,
            Flip,
            Extract,
            Locate,
            Lock,
            Hide,
            Exit
        }
        public MenuEntity(Rectangle Rectangle, MenuAction action)
        {
            this.Rectangle = Rectangle;
            this.menuaction = action;
        }
        public MenuEntity(int x, int y, Size s, MenuAction action)
        {
            this.Rectangle = new Rectangle(new Point(x, y), s);
            this.menuaction = action;
        }
     
        internal void DoAction()
        {if (enable == false)
                return;
         
            switch (menuaction)
            {
                case MenuAction.GoBack:
                    Controller.GoBack();
                    break;
                case MenuAction.GotoRootNode:
                    Controller.OpenRootNode();
                    break;
                case MenuAction.Open:
                    Controller.OpenNodeOrEntity();
                    break;
                case MenuAction.GotoFavorite:
                    Controller.ShowFavorite();
                    break;
                case MenuAction.GotoRecyleBin:
                    Controller.OpenRecycleBin();
                    break;
                case MenuAction.SelectAll:
                    Controller.SelectAll();
                    break;
                case MenuAction.Copy:
                    Controller.Copy();
                 
                   
                    break;
                case MenuAction.Cut:
                    Controller.Cut();

                    break;
                case MenuAction.Paste:
                    Controller.Paste();
                            break;
                case MenuAction.SetCover:
                    Controller.SetCoverImage();
                            break;
                case MenuAction.SetBG:                    
                    Controller.SetBackground();
                            break;
                case MenuAction.ClearBG:
                    Controller.ClearBackground();
                    break;
                case MenuAction.SetDesktopBG:
                    Controller.SetDesktopBackground();
                                break;
                case MenuAction.Left:
                    Controller.Left();
                    break;
                case MenuAction.Right:
                    Controller.Right();
                    break;
                case MenuAction.Up:
                    Controller.Up();
                    break;
                case MenuAction.Down:
                    Controller.Down();
                    break;
                case MenuAction.NewNode:
                    Controller.AddNewNode();
                    break;
                case MenuAction.AddImages:
                    Controller.AddNewImages();
                    break;
                case MenuAction.AddFolders:
                    Controller.AddFolders();
                    break;
                case MenuAction.Delete:
                    Controller.Delete();
                    break;
                case MenuAction.Rename:
                    Controller.Rename();
                    break;

                case MenuAction.Refresh:
                    Controller.Refresh();
                    break;
                case MenuAction.Reload:
                    Controller.Refresh(true);
                    break;
                case MenuAction.Restore:
                    Controller.RestoreFromRecyleBin();
                    break;

                case MenuAction.Rotate:
                    Controller.Rotate();
                    break;
                case MenuAction.Flip:
                    Controller.Flip();
                    break;
                case MenuAction.Extract:
                    Controller.Extract();
                    break;
                case MenuAction.Locate:
                    Controller.GotoOriginalNodeFromLinkedEntity();
                    break;
                case MenuAction.GotoMenu:
                    Controller.ShowMenu();
                    break;
                case MenuAction.Lock:
                    Controller.Lock();
                    break;
                case MenuAction.Hide:
                    Controller.Hide();
                    break;
                case MenuAction.Exit:
                    Controller.ExitProgram();
                    break;
            }
        }

        internal void PaintSelection(Graphics graphics, Rectangle rectangle)
        {
            graphics.FillRectangle(Brushes.Gold, rectangle);
          }

        internal void PaintHovered(Graphics graphics, Rectangle rectangle)
        {
            graphics.FillRectangle(Brushes.Blue, rectangle);
           
        }

        internal void PaintDisable(Graphics graphics, Rectangle rectangle)
        {
            if(enable==false)
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(200,Color.Gray)), rectangle);

        }

        internal void CheckAvailability()
        {
           switch(menuaction)
            {
              
                case MenuAction.Copy:
                    if (Controller.nodeController == null || Controller.nodeController.View != NodeController.view.Explorer)
                        enable = false;
                   else if (GlobalClass.SelectedEntity==null && GlobalClass.SelectedNode==null)
                        enable = false;
                    else enable = true;
                    break;
                case MenuAction.Open:
                    if (Controller.nodeController == null || Controller.nodeController.View != NodeController.view.Explorer)
                        enable = false;
                  else  if (GlobalClass.SelectedEntity == null && GlobalClass.SelectedNode == null)
                        enable = false;
                    else enable = true;
                    break;
                case MenuAction.Delete:
                    if (Controller.nodeController == null || Controller.nodeController.View != NodeController.view.Explorer)
                        enable = false;
                else    if (GlobalClass.SelectedEntity == null && GlobalClass.SelectedNode == null)
                        enable = false;
                    else enable = true;
                    break;
                case MenuAction.Cut:
                    if (Controller.nodeController == null || Controller.nodeController.View != NodeController.view.Explorer)
                        enable = false;
                   else if (GlobalClass.SelectedEntity == null && GlobalClass.SelectedNode == null)
                        enable = false;
                    else enable = true;
                    break;
                case MenuAction.Paste:
                    if (Controller.nodeController == null || Controller.nodeController.View != NodeController.view.Explorer)
                        enable = false;
                  else  if (GlobalClass.CopiedEntities == null && GlobalClass.CopiedNodes == null || (GlobalClass.CopiedEntities.Count == 0 && GlobalClass.CopiedNodes.Count == 0))
                        enable = false;
                    else enable = true;
                    break;
                case MenuAction.SetCover:
                    if (GlobalClass.SelectedEntity == null && GlobalClass.SelectedNode == null)
                        enable = false;
                    else enable = true;
                    break;
                case MenuAction.SetDesktopBG:
                    if (GlobalClass.SelectedEntity == null)
                        enable = false;
                    else enable = true;
                    return;
                case MenuAction.SetBG:
                    if (GlobalClass.SelectedEntity == null)
                        enable = false;
                    else enable = true;
                    break;
                    
           

                case MenuAction.ClearBG:
                    if (Properties.Settings.Default.BackgroundImageName == "")
                        enable = false;
                    else enable = true;
                    break;

                case MenuAction.AddFolders:
                    if (Controller.IsOpenNodeManualRestrict())
                        enable = false;
                    else enable = true;
                    break;
                case MenuAction.AddImages:
                    if (Controller.IsOpenNodeManualRestrict())
                        enable = false;
                    else enable = true;
                    break;
                case MenuAction.NewNode:
                    if (Controller.IsOpenNodeManualRestrict())
                        enable = false;
                    else enable = true;
                    break;

                case MenuAction.Rename:
                    if (GlobalClass.SelectedNode == null)
                        enable = false;
                    else enable = true;
                    break;

                case MenuAction.Restore:
                    if (Controller.nodeController == null || Controller.nodeController.View != NodeController.view.Explorer)
                        enable = false;
                   else if (GlobalClass.SelectedEntity != null && GlobalClass.SelectedEntity.IsDeleted)
                        enable = true;
                    else enable = false;
                    break;

                case MenuAction.Locate:
                    if (Controller.nodeController == null || Controller.nodeController.View != NodeController.view.Explorer)
                        enable = false;
                  else  if (GlobalClass.SelectedEntity != null && GlobalClass.SelectedEntity.IsShortCut)
                        enable = true;
                    else enable = false;
                    break;

                case MenuAction.Rotate:
                    if (GlobalClass.SelectedEntity == null)
                        enable = false;
                    else enable = true;
                    break;

                case MenuAction.Flip:
                    if (GlobalClass.SelectedEntity == null)
                        enable = false;
                    else enable = true;
                    break;

                case MenuAction.Extract:
                    if (Controller.nodeController == null || Controller.nodeController.View != NodeController.view.Explorer)
                        enable = false;
                   else if (GlobalClass.SelectedEntity == null)
                        enable = false;
                    else enable = true;
                    break;

                case MenuAction.Refresh:
                    if (Controller.nodeController == null || Controller.nodeController.View != NodeController.view.Explorer)
                        enable = false;
                    else enable = true;
                    break;

                case MenuAction.Reload:
                    if (Controller.nodeController == null || Controller.nodeController.View != NodeController.view.Explorer)
                        enable = false;
                    else enable = true;
                    break;

                case MenuAction.SelectAll:
                    if (Controller.nodeController == null || Controller.nodeController.View != NodeController.view.Explorer)
                        enable = false;
                    else enable = true;
                    break;
            }
        }
    }
}
