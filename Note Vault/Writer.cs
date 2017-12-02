using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vault.Note_Vault
{
    public partial class Writer : Form
    {
        Notes notes;
        int SelectedIndex = -1;
        Note CurrentNote;
        public string ActivePath;
        Point ShiftIndex
        {
            get
            {
                return _shiftindex;
            }
            set
            {
                if (value.Y > 0 || value.X > 0)

                {
                    if (value.Y > 0)

                        _shiftindex = new Point(_shiftindex.X, 0);
                    if (value.X > 0)

                        _shiftindex = new Point(0, _shiftindex.Y);
                }
                else
                    _shiftindex = value;


            }

        }
        public Writer(string activepath)
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.ActivePath = activepath;
            notes = new Notes(ActivePath);
            CurrentNote = notes.GetLastNote(ActivePath);
            SelectedIndex = notes.Entities.Count - 1;
            DisplayNote();
        }

        #region Write Mode
        private void btn_new_Click(object sender, EventArgs e)
        {
            CreateNewNote();
        }

        private void CreateNewNote()
        {
            CurrentNote = notes.AddNewNote(ActivePath);
            SelectedIndex = notes.Entities.Count - 1;
            DisplayNote();
        }

        void DisplayNote()
        {
            if (CurrentNote == null)
            {
                tb_content.Text = "Create New Note";
            }
            else
            {
                tb_content.Text = CurrentNote.GetPlainContent();
                tb_title.Text = CurrentNote.GetPlainTitle();
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            SaveNote();
        }

        private void SaveNote()
        {
            UpdateCurrentNote();
            SaveCurrentNote();
        }

        private void UpdateCurrentNote()
        {
            if (CurrentNote != null)
                CurrentNote.UpdateNote(tb_title.Text, tb_content.Text);
        }

        private void SaveCurrentNote()
        {
            if (CurrentNote != null)
            {
                notes.Save(ActivePath);
                CurrentNote.Save(ActivePath);
            }
        }

        private void btn_next_Click(object sender, EventArgs e)
        {
            NextNote();
        }


        private void btn_previous_Click(object sender, EventArgs e)
        {
            PreivousNote();
        }

        private void NextNote()
        {
            ShiftIndex = new Point(0, 0);
            SelectedIndex++;
            if (SelectedIndex >= notes.Entities.Count)
                SelectedIndex = 0;
            SaveNote();
            GetNoteOfSelectedIndex();
        }

        private void GetNoteOfSelectedIndex()
        {
            if (SelectedIndex != -1)
                CurrentNote = notes.GetNoteofIndex(SelectedIndex,ActivePath);
            else
                CurrentNote = null;
            DisplayNote();
        }

        private void PreivousNote()
        {
            ShiftIndex = new Point(0, 0);
            SelectedIndex--;
            if (SelectedIndex <= -1)
            {
                SelectedIndex = notes.Entities.Count - 1;
            }
            SaveNote();
            GetNoteOfSelectedIndex();
        }
        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (SelectedIndex != -1)
            {
                notes.Delete(SelectedIndex,ActivePath);

                SelectedIndex--;
                if (SelectedIndex <= -1)
                {
                    SelectedIndex = notes.Entities.Count - 1;
                }

                GetNoteOfSelectedIndex();

            }
        }
        #endregion

        #region Read Mode Region
        public enum View
        {
            Note,
            FullImage
        }
        View view = View.Note;
        List<string> TitleContents;
        List<RectangleF> ContentRects;
        List<Entity> Entities;
        List<RectangleF> EntityRects;
        Font fontTitle = new Font("Segoe UI", 12f, FontStyle.Bold);
        private void btn_readmode_Click(object sender, EventArgs e)
        {
            if (CurrentNote != null)
            {

                HideAllControls();
                this.AutoScroll = true;

                SetContentsAndThumbs();
                this.Invalidate();
            }
        }

        private void HideAllControls()
        {
            splitContainer2.Visible = false;
            splitContainer2.Enabled = false;

            menuStrip1.Visible = false;
            menuStrip1.Enabled = false;

            tb_content.Visible = false;
            tb_content.Enabled = false;

            tb_title.Visible = false;
            tb_title.Enabled = false;

            this.Focus();
            this.AutoScroll = true;
        }

        private void ShowAllControls()
        {
            splitContainer2.Visible = true;
            splitContainer2.Enabled = true;

            menuStrip1.Visible = true;
            menuStrip1.Enabled = true;

            tb_content.Visible = true;
            tb_content.Enabled = true;

            tb_title.Visible = true;
            tb_title.Enabled = true;
        }

        private void SetContentsAndThumbs()
        {
            if (splitContainer2.Visible == true)
                return;
            TitleContents = new List<string>();
            ContentRects = new List<RectangleF>();
            Entities = new List<Entity>();
            EntityRects = new List<RectangleF>();

            if (CurrentNote == null)
                return;
            //For Title
            string title = CurrentNote.GetPlainTitle();
            Graphics g = Graphics.FromImage(new Bitmap(100, 100));
            TitleContents.Add(title);
            SizeF TitleSize = g.MeasureString(title, fontTitle);
            ContentRects.Add(new RectangleF(new Point(10, 5), TitleSize));

            //For contents
            string content = CurrentNote.GetPlainContent();
            String OriginalContent = content;
            float y = TitleSize.Height + 10;
            if (content.Contains("~"))
            {
                string[] splits = content.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);
                float x = (this.Width % GlobalClass.ThumbnailSize.Width) / 2;


                for (int i = 0; i < splits.Length; i++)
                {
                    //Add Entity
                    if (splits[i].Contains("(") && splits[i].Contains(")"))
                    {
                        string entityname = splits[i].Replace('(', ' ').Replace(')', ' ');
                        Entity en = new Entity(entityname.Trim());
                        en.LoadPicture(ActivePath);
                        Entities.Add(en);
                        EntityRects.Add(new RectangleF(x, y, GlobalClass.ThumbnailSize.Width, GlobalClass.ThumbnailSize.Height));
                        x += GlobalClass.ThumbnailSize.Width;
                        if (x + GlobalClass.ThumbnailSize.Width + 5 > this.Width)
                        {
                            x = (this.Width % GlobalClass.ThumbnailSize.Width) / 2;
                            y += GlobalClass.ThumbnailSize.Height;
                        }
                        OriginalContent = OriginalContent.Replace("~" + splits[i] + "~", "");

                    }

                }
                y += GlobalClass.ThumbnailSize.Height;

            }
            content = OriginalContent;
            string[] contents = OriginalContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in contents)

            {
                TitleContents.Add(s);
                SizeF contentsize = g.MeasureString(s, this.Font, this.Width - 30);
                ContentRects.Add(new RectangleF(new PointF(10, y + 10), contentsize));
                y += contentsize.Height + 10;

            }
        }


        private void Writer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                ShowAllControls();
            }
            else if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Back)
            {
                if (view == View.FullImage)
                {
                    view = View.Note;
                }
                else if (view == View.Note)
                {
                    ShowAllControls();
                }
            }
            else if (e.KeyCode == Keys.Right)
            {
                if (splitContainer2.Visible == false)
                {
                    if (view == View.Note)
                    {
                        NextNote();
                        SetContentsAndThumbs();
                    }
                    else if (view == View.FullImage)
                    {
                        SelectedThumbIndex++;
                        if (SelectedThumbIndex >= Entities.Count)
                        {
                            SelectedThumbIndex = 0;
                        }
                        EnterDown();

                    }
                }
            }
            else if (e.KeyCode == Keys.Left)
            {
                if (splitContainer2.Visible == false)
                {
                    if (view == View.Note)
                    {
                        PreivousNote();
                        SetContentsAndThumbs();
                    }

                    else if (view == View.FullImage)
                    {
                        SelectedThumbIndex--;
                        if (SelectedThumbIndex <= -1)
                        {
                            SelectedThumbIndex = Entities.Count - 1;
                        }
                        EnterDown();

                    }
                }
            }

            else if (e.KeyCode == Keys.Enter)
            {
                if (SelectedThumbIndex != -1)
                    EnterDown();
            }

            this.Invalidate();
        }

        private void Writer_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            if (view == View.Note)
            {



                for (int i = 1; i < ContentRects.Count; i++)
                {
                    // e.Graphics.SetClip(new Rectangle(0, 0, this.Width, this.Height));
                    if (ContentRects[i].Y + ShiftIndex.Y < this.Height && ShiftIndex.Y + ContentRects[i].Y + ContentRects[i].Height > 0)
                    {

                        e.Graphics.DrawString(TitleContents[i], this.Font, Brushes.White, GetShiftedRect(ContentRects[i]));
                    }
                }
                for (int i = 0; i < EntityRects.Count; i++)
                {
                    if (EntityRects[i].Y + ShiftIndex.Y < this.Height && ShiftIndex.Y + EntityRects[i].Y + EntityRects[i].Height > 0)
                    {
                        if (i == SelectedThumbIndex)
                            e.Graphics.DrawImage(SD.Images.ChangeImage.ApplyColorMatrix(Entities[i].GetThumbnail(), SD.Images.ChangeImage.ImageProperty.Negative), GetShiftedRect(EntityRects[i]));
                        else
                            e.Graphics.DrawImage(Entities[i].GetThumbnail(), GetShiftedRect(EntityRects[i]));
                    }
                }

                if (TitleContents.Count > 0)
                {
                    e.Graphics.FillRectangle(Brushes.Gold, new RectangleF(0, 0, this.Width, 30));
                    e.Graphics.DrawString(TitleContents[0], fontTitle, Brushes.Black, ContentRects[0]);
                }
            }
            else if (view == View.FullImage)
            {

                FullImage.Paint(e.Graphics);
            }
        }

        private RectangleF GetShiftedRect(RectangleF rectangleF)
        {
            return new RectangleF(rectangleF.X + ShiftIndex.X, rectangleF.Y + ShiftIndex.Y, rectangleF.Width, rectangleF.Height);
        }

        bool drag = false;
        Point initial, initialmouseDown;
        private Point _shiftindex;
        int SelectedThumbIndex = -1;
        private void Writer_MouseDown(object sender, MouseEventArgs e)
        {
            if (view == View.Note)
            {
                drag = true;
                initialmouseDown = e.Location;
                initial = e.Location;
            }
            else if (view == View.FullImage)
            {
                FullImage.MouseDown(e.Location);
            }
        }

        private void Writer_MouseMove(object sender, MouseEventArgs e)
        {
            if (view == View.Note)
            {
                if (drag)
                {
                    ShiftIndex = new Point(ShiftIndex.X, ShiftIndex.Y + e.Location.Y - initial.Y);
                    initial = e.Location;
                }
                else
                {
                    SelectedThumbIndex = FindSelection(e.Location);
                }
            }
            else if (view == View.FullImage)
            {
                FullImage.MouseMove(e.Location);

            }
            this.Invalidate();
        }

        private int FindSelection(Point location)
        {
            RectangleF mouserect = new RectangleF(location, new Size(1, 1));
            for (int i = 0; i < EntityRects.Count; i++)
            {
                if (mouserect.IntersectsWith(GetShiftedRect(EntityRects[i])))
                    return i;
            }
            return -1;
        }

        private void Writer_MouseUp(object sender, MouseEventArgs e)
        {
            if (view == View.Note)
            {
                drag = false;
            }
            if (e.Location == initialmouseDown)
            {
                EnterDown();
            }
            if (view == View.FullImage)
            {
                FullImage.MouseUp(e.Location);
            }
        }

        private void Writer_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (view == View.FullImage)
            {
                FullImage.MouseDoubleClick(e.Location);
            }
            this.Invalidate();
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (view == View.Note)
            {
                if (e.Delta > 0)
                    ShiftIndex = new Point(ShiftIndex.X, ShiftIndex.Y + GlobalClass.ThumbnailSize.Width);
                else
                    ShiftIndex = new Point(ShiftIndex.X, ShiftIndex.Y - GlobalClass.ThumbnailSize.Width);


            }
            else if (view == View.FullImage)
            {
                FullImage.MouseWheel(e.Delta);

            }
            this.Invalidate();

        }

        private void EnterDown()
        {
            if (SelectedThumbIndex != -1)
            {
                view = View.FullImage;
                FullImage.LoadEntity(Entities[SelectedThumbIndex],ActivePath);
                this.Invalidate();
            }
        }

        #endregion

    }
}
