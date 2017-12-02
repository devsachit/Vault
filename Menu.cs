using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vault
{
    public class Menu
    {     

        public static Controller.ControllerView PreviousViewState { get; internal set; }
        public static List<NeoButton> buttons;
      
        static int x = 100, y = 100;
        static int selected = -1, hovered = -1;
        public static void Initialize()
        {                
         
            if (buttons == null)
            {
                InitializeButtons();
            }
        }

        private static void InitializeButtons()
        {
            buttons = new List<NeoButton>();
            buttons.Add(new NeoButton("Image Vault", new Size(150, 150), new PointF(x, y))); x += 200;
            buttons.Add(new NeoButton("Note Vault", new Size(150, 150), new PointF(x, y))); x += 200;
            buttons.Add(new NeoButton("Password Vault", new Size(150, 150), new PointF(x, y))); x= 100;y += 250;
            buttons.Add(new NeoButton("Copier", new Size(150, 75), new PointF(x, y))); x += 200;
            buttons.Add(new NeoButton("New Controller", new Size(150, 75), new PointF(x, y))); x += 200;
            buttons.Add(new NeoButton("Settings", new Size(150, 75), new PointF(x, y))); x = 100;y += 125;
            buttons.Add(new NeoButton("Hide", new Size(150, 75), new PointF(x, y))); x +=200;
            buttons.Add(new NeoButton("Restart", new Size(150, 75), new PointF(x, y))); x += 200;
            buttons.Add(new NeoButton("Exit", new Size(150, 75), new PointF(x, y))); x = 100;y += 100;
            //buttons.Add(new NeoButton("0", new Size(30, 30), new PointF(x, y))); x = 272; y += 30;

            //buttons.Add(new NeoButton("+", new Size(30, 30), new PointF(x, y))); x += 30;
            //buttons.Add(new NeoButton("-", new Size(30, 30), new PointF(x, y))); x += 30;
            //buttons.Add(new NeoButton("/", new Size(30, 30), new PointF(x, y))); x += 30;
            //buttons.Add(new NeoButton("*", new Size(30, 30), new PointF(x, y))); x += 30;
            //buttons.Add(new NeoButton("unlock", new Size(60, 30), new PointF(x, y))); x += 60;
            //buttons.Add(new NeoButton("clear", new Size(60, 30), new PointF(x, y))); x += 60;
            //buttons.Add(new NeoButton("exit", new Size(60, 30), new PointF(x, y)));
        }

        internal static void MouseMove(Point location)
        {
            hovered = FindSelection(location);
        }

        private static int FindSelection(Point location)
        {
            RectangleF mouserect = new RectangleF(location, new Size(1, 1));
            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i].rectangle.IntersectsWith(mouserect))
                {
                    return i;
                }
            }
            return -1;
        }

        internal static void KeyUp(Keys keyCode)
        {
        }

        internal static void MouseDown(Point location, MouseButtons button)
        {
            selected = FindSelection(location);
            if (selected != -1)
            {
                string text = buttons[selected].Value;
               switch(text)
                {
                    case "Image Vault": Controller.SetView(Controller.ControllerView.NodeController);
                            break;
                    case "Note Vault": Controller.OpenWriter();
                        break;
                    case "Password Vault": Controller.OpenPasswords();
                        break;
                    case "Copier":
                        if (GlobalClass.AutomaticCopyImage)
                            GlobalClass.AutomaticCopyImage = false;
                        else
                            GlobalClass.AutomaticCopyImage = true;
                        break;
                    case "New Controller":
                        Controller.OpenNodeControllerSettings();
                        break;
                    case "Settings":
                        Controller.OpenSettings();
                        break;
                    case "Hide": Controller.Hide();
                        break;
                    case "Restart":Controller.Restart(); break;
                    case "Exit": Controller.ExitProgram();break;




                }

            }
        }

        internal static void MouseUp(Point location)
        {
            selected = -1;
        }

        internal static void MouseDoubleClick(Point location)
        {

        }

        internal static void MouseWheel(int delta, Point p)
        {

        }

        internal static void KeyDown(Keys keyCode)
        {
            if(keyCode==Keys.Escape)
            {
                if (PreviousViewState == Controller.ControllerView.Lock)
                    PreviousViewState = Controller.ControllerView.NodeController;
                Controller.SetView(PreviousViewState);
            }
        }




        internal static void Paint(Graphics graphics)
        {
            if (Controller.nodeController. BackgroundImage != null)
                graphics.DrawImage(Controller.nodeController.BackgroundImage, new PointF(0, 0));
            if (hovered != -1)
                graphics.FillRectangle(Brushes.SkyBlue, buttons[hovered].rectangle);
            if (selected != -1)
                graphics.FillRectangle(Brushes.Gold, buttons[selected].rectangle);

            if (GlobalClass.AutomaticCopyImage)
                graphics.FillRectangle(Brushes.Blue, buttons[3].rectangle);
           // graphics.FillRectangle(new HatchBrush(HatchStyle.Percent50, Color.FromArgb(150, Color.Black), Color.FromArgb(150, Color.White)), new RectangleF(0, 0, GlobalClass.ParentBoundry.Width, GlobalClass.ParentBoundry.Height));
            graphics.DrawImage(Properties.Resources.globalmenu, new Rectangle(0, 0, 700, 700));

        }
    }
}
