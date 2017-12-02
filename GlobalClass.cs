using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vault
{
    internal class GlobalClass
    {
        public static Size ThumbnailSize = new Size(150, 150);

        public static Font InfoFont = new Font("Segoe UI", 10f);
        public static Font Smallfont = new Font("Segoe UI", 8f);
        public static Size ParentBoundry = new Size(1366, 768);
        public static Font SmallfontBold = new Font("Segoe UI", 7f,FontStyle.Bold);
        public static Form1 ParentForm { get; internal set; }
        public static bool MultiSelect { get; internal set; }
        public static Node SelectedNode { get { return _selectednode; } set { _selectednode = value; if (SelectedNodeChanged != null) SelectedNodeChanged(null, EventArgs.Empty); } }

        public static List<Entity> CopiedEntities { get; internal set; }
        public static List<Node> CopiedNodes { get; internal set; }
        public static string CopiedSourceActivePath { get; internal set; }
        public static string CopiedSourceParentPath { get; internal set; }
        public static string PasteDestinationPath { get; internal set; }
        public static Node CopiedSourceNode { get; internal set; }
        public static bool IsCut = false;
        public static bool HighGraphicsQuality { get; internal set; }
        public static Entity SelectedEntity { get { return _selectedentity; }set { _selectedentity = value; if (SelectedEntityChanged != null) SelectedEntityChanged(null, EventArgs.Empty); } }
        public static Node TextBoxRealatedNode { get; internal set; }
        public static bool IsTextboxVisible { get; private set; }
        public static Entity TextBoxRealatedEntity { get; internal set; }

        internal static void Notify(string v)
        {
            throw new NotImplementedException();
        }

        public static Brush SelectionBrush { get; internal set; }
        public static Color BorderColor { get; internal set; }
        public static Pen HighlightPen { get; internal set; }
        public static Pen BorderColorPen { get; internal set; }
        public static Brush LeftPanelBrush { get; internal set; }
        public static bool AutomaticCopyImage { get; internal set; }
        public static Brush DimSelectionBrush { get; internal set; }
        public static string PasteDestinationActivePath { get; internal set; }
        public static Font PropertyNormalFont { get; internal set; }
        public static Font PropertyBoldFont { get; internal set; }

        public static int LeftPanelWidth = 235;

        public static float RoundCornorRadius = 8.67f;

        public static Brush TitleBrush;

        public static TextBox textbox;
        public static Brush BlankBrush ;
        public static Font TagFont ;

        public  static Brush TagBGBrush ;

        public static Brush NodeNameBGBrush;
        public static Brush ShortCutBGBrush  ;

        public static int TitleHeight = 100;
        public static event EventHandler SelectedNodeChanged,SelectedEntityChanged;

        public static Font NodeNameFont;

        public static int RandomCount = 0;
        private static Node _selectednode;
        public static event EventHandler ProgramExiting;
        public static event EventHandler TextChanged;
        public static event EventHandler NodeControllerChanged;
        public enum TextBoxAction
        {
            Rename,
            EditTag,
            LockPassword
        }
        public static TextBoxAction textboxAction = TextBoxAction.Rename;
        private static Entity _selectedentity;

        public static void Initialize()
        {
         
            NodeNameFont = new Font("Segoe UI", 10f, FontStyle.Bold);
            NodeNameBGBrush = new SolidBrush(Color.FromArgb(150, Color.Gray));
            ShortCutBGBrush = new SolidBrush(Color.FromArgb(150, Color.Gray));
            TagBGBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
            TagFont = new Font("Segoe UI", 7f, FontStyle.Italic);
            PropertyNormalFont= new Font("Segoe UI", 9f, FontStyle.Regular);
            PropertyBoldFont = new Font("Segoe UI", 9f, FontStyle.Bold);

            BlankBrush = new HatchBrush(HatchStyle.Percent50, Color.FromArgb(100, Color.Black), Color.Transparent);
            TitleBrush = new LinearGradientBrush(new PointF(0, 0), new PointF(0, GlobalClass.TitleHeight), Color.Black, Color.Transparent); // new TextureBrush(Properties.Resources.titlebar);//  new LinearGradientBrush(new PointF(0, 0), new PointF(0, GlobalClass.TitleHeight),  Color.Transparent, Color.FromArgb(230, Color.Black));
            SelectionBrush = new SolidBrush(Color.FromArgb(100, Color.Navy));
            DimSelectionBrush= new SolidBrush(Color.FromArgb(30, Color.Black));
            LeftPanelBrush = new LinearGradientBrush(new PointF(0,0),new PointF(LeftPanelWidth,ParentBoundry.Height), Color.FromArgb(100, 80, 50, 80), Color.FromArgb(150, Color.Gray));
            HighlightPen = new Pen(Color.Gold, 2f);
            BorderColorPen = new Pen(Color.FromArgb(50, 50, 50), 2f);
            textbox = new TextBox();
            textbox.KeyDown += Textbox_KeyDown;
            textbox.TextChanged += Textbox_TextChanged;
            textbox.Multiline = true;
            AutomaticCopyImage = true;
        }

     
        public static void ChangeNodecontroller(string ActiveFolderPath )
        {
            if (NodeControllerChanged != null)
                NodeControllerChanged(ActiveFolderPath, EventArgs.Empty);
        }
        private static void Textbox_KeyDown(object sender, KeyEventArgs e)
        {


            if (e.KeyCode == Keys.Escape)
            {
                Hidetextbox();
                if (TextChanged != null)
                {
                    TextChanged(null, EventArgs.Empty);
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (textboxAction != TextBoxAction.LockPassword)
                {
                    Hidetextbox();
                }
                if (TextChanged != null)
                {
                    TextChanged(textbox.Text, EventArgs.Empty);
                }
                if (textboxAction == TextBoxAction.LockPassword)
                {
                    textbox.Text = "";
                }
            }

        }
        private static void Textbox_TextChanged(object sender, EventArgs e)
        {
            if (textboxAction == TextBoxAction.LockPassword)
            {
                if (TextChanged != null)
                {
                    TextChanged(textbox.Text, EventArgs.Empty);
                }
            }
        }


        public static void ShowTextBox(PointF Location, SizeF size, string text)
        {
            if (IsTextboxVisible == false)
            {
                if(textboxAction==TextBoxAction.LockPassword)
                {
                    textbox.PasswordChar = '*';
                }
                else
                {
                    textbox.PasswordChar = '\0';
                }

                IsTextboxVisible = true;
                textbox.Size = new Size((int)size.Width, (int)size.Height);
                textbox.Location = new Point((int)Location.X, (int)Location.Y);
                textbox.Text = text;
                ParentForm.Controls.Add(textbox);
                textbox.Focus();
            }
        }
        public static void Hidetextbox()
        {

            if (IsTextboxVisible)
            {
                IsTextboxVisible = false;
                ParentForm.Controls.Remove(textbox);
            }
        }

        internal static void DoActionBeforeExiting()
        {
            if (ProgramExiting != null)
                ProgramExiting(null, EventArgs.Empty);
        }
        internal static string GetRandomString()
        {
            RandomCount++;
            //return RandomCount.ToString();
            return "R"+ DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond+RandomCount;
        }

        internal static void Invalidate()
        {
            ParentForm.Invalidate();
        }

        internal static void ClearClipborar()
        {
            CopiedEntities = new List<Entity>();
            CopiedNodes = new List<Node>();
            CopiedSourceActivePath = "";
            CopiedSourceParentPath = "";
            PasteDestinationPath = "";
            CopiedSourceNode = null;
        }

        public static GraphicsPath GetRoundBoundry(RectangleF Boundry, float CornerRadius)
        {
            GraphicsPath p = new GraphicsPath();
            if (CornerRadius <= 0.0F)
            {
                p.AddRectangle(Boundry);
                p.CloseFigure();
                return p;
            }

            // if the corner radius is greater than or equal to 
            // half the width, or height (whichever is shorter) 
            // then return a capsule instead of a lozenge 
            if (CornerRadius >= (Math.Min(Boundry.Width, Boundry.Height)) / 2.0)
            {
                p = GetCapsule(Boundry);
                return p;

            }
            // create the arc for the rectangle sides and declare 
            // a graphics path object for the drawing 
            float diameter = CornerRadius * 2.0F;
            SizeF sizeF = new SizeF(diameter, diameter);
            RectangleF arc = new RectangleF(Boundry.Location, sizeF);

            // top left arc 
            p.AddArc(arc, 180, 90);

            // top right arc 
            arc.X = Boundry.Right - diameter;
            p.AddArc(arc, 270, 90);

            // bottom right arc 
            arc.Y = Boundry.Bottom - diameter;
            p.AddArc(arc, 0, 90);

            // bottom left arc
            arc.X = Boundry.Left;
            p.AddArc(arc, 90, 90);

            p.CloseFigure();
            return p;

        }

        static private GraphicsPath GetCapsule(RectangleF baseRect)
        {
            float diameter;
            RectangleF arc;
            GraphicsPath p = new System.Drawing.Drawing2D.GraphicsPath();
            try
            {
                if (baseRect.Width > baseRect.Height)
                {
                    // return horizontal capsule 
                    diameter = baseRect.Height;
                    SizeF sizeF = new SizeF(diameter, diameter);
                    arc = new RectangleF(baseRect.Location, sizeF);
                    p.AddArc(arc, 90, 180);
                    arc.X = baseRect.Right - diameter;
                    p.AddArc(arc, 270, 180);
                }
                else if (baseRect.Width < baseRect.Height)
                {
                    // return vertical capsule 
                    diameter = baseRect.Width;
                    SizeF sizeF = new SizeF(diameter, diameter);
                    arc = new RectangleF(baseRect.Location, sizeF);
                    p.AddArc(arc, 180, 180);
                    arc.Y = baseRect.Bottom - diameter;
                    p.AddArc(arc, 0, 180);
                }
                else
                {
                    // return circle 
                    p.AddEllipse(baseRect);
                }
            }
            catch (Exception ex)
            {
                p.AddEllipse(baseRect);
            }
            finally
            {
                p.CloseFigure();
            }
            return p;
        }



        public static class Wallpaper
        {


            const int SPI_SETDESKWALLPAPER = 20;
            const int SPIF_UPDATEINIFILE = 0x01;
            const int SPIF_SENDWININICHANGE = 0x02;

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

            public enum Style : int
            {
                Tiled,
                Centered,
                Stretched,
                Fill
            }

            public static void Set(Image img, Style style)
            {
                //System.IO.Stream s = new System.Net.WebClient().OpenRead(uri.ToString());

                //System.Drawing.Image img = System.Drawing.Image.FromStream(s);
                string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
                img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
                if (style == Style.Stretched)
                {
                    key.SetValue(@"WallpaperStyle", 2.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                }

                if (style == Style.Centered)
                {
                    key.SetValue(@"WallpaperStyle", 1.ToString());
                    key.SetValue(@"TileWallpaper", 0.ToString());
                }

                if (style == Style.Tiled)
                {
                    key.SetValue(@"WallpaperStyle", 1.ToString());
                    key.SetValue(@"TileWallpaper", 1.ToString());
                }

                if (style == Style.Fill)
                {

                }

                SystemParametersInfo(SPI_SETDESKWALLPAPER,
                    0,
                    tempPath,
                    SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            }
        }

        internal static void CopyEntityToClipBoard()
        {
            string textToCopy = "";
            for (int i = 0; i < CopiedEntities.Count; i++)
            {
                textToCopy += "~(" + CopiedEntities[i].ImageFileName + ")~";
            }
            if(textToCopy != "" )
            Clipboard.SetText(textToCopy);
        }
    }
}
