using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vault
{
    public class PasswordController
    {
        Font FontName = new Font("Segoe UI", 14f);
        Font FontInfo = new Font("Segoe UI", 12f);
        public List<RectangleF> namerects;
        public List<string> names;
        public List<RectangleF> InfoRects;
        public List<string> Infos;

        public List<RectangleF> BackgroundRects;
        public List<Brush> BackGroundBrushes;
        public List<RectangleF> Oldpassrects;
        public List<string> OldsPass;
        float NameListWidth = 0;

        PassWords passwords;
        int SelectedNameIndex = -1;
        int SelectedInfoIndex = -1;
        int HoveredNameIndex = -1;
        int HoveredInfoIndex = -1;

        public int SelectedOldPassListIndex = -1;

        public int SelectedOldPasswordParentIndex { get; private set; }

        int SelectedPasswordEntityIndex = -1;

        public int OldPassSelectionStartIndex = -1;

        int InfoSelectionStartIndex = -1;

        public int HoveredOldPassIndex = -1;
        public int SelectedOldPassIndex = -1;


        public Point ShiftIndex_Info
        {
            get { return _shiftindex_info; }
            set
            {
                if (value.Y > 0)
                    _shiftindex_info = new Point(0, 0);
                else if (InfoRects.Count > 0 && InfoRects[InfoRects.Count - 1].Y > Math.Abs(value.Y))
                {
                    _shiftindex_info = value;
                }

                //   else _shiftindex_info = value;
            }
        }

        internal void MouseWheel(int delta)
        {
            if (delta > 0)
                ShiftIndex_Info = new Point(ShiftIndex_Info.X, ShiftIndex_Info.Y + 25);
            else

                ShiftIndex_Info = new Point(ShiftIndex_Info.X, ShiftIndex_Info.Y - 25);
        }

        public enum Selection
        {
            Name,
            Info,
            None,
            OldPass,
        }
        public enum InfoSelection
        {
            Name,
            Details,
            ExtraInfo,
            Edit,
            Copy,
            Delete,
            None,
            CopyEmail,
            CopyPassword,
        }

        InfoSelection infoSelection = InfoSelection.None;

        internal void DeleteActivePerson()
        {
            if (passwords.ActivePerson != null)
            {
                if (MessageBox.Show("Sure to Delete Selected Person's Informations and Passwords?", "Delete Selected Person?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    passwords.DeleteActivePerson();
                    ResetSelections();
                    SetRectangles();
                }
            }
        }

        Selection selection = Selection.Name;


        public PasswordController(string activepasswordpath)
        {
            passwords = new PassWords(activepasswordpath);
            names = new List<string>(); ;
            namerects = new List<RectangleF>();
            Infos = new List<string>();
            InfoRects = new List<RectangleF>();
            Oldpassrects = new List<RectangleF>();
            OldsPass = new List<string>();
            BackGroundBrushes = new List<Brush>();
            BackgroundRects = new List<RectangleF>();

        }

        internal void AddNewPasswordField()
        {
            if (passwords.ActivePerson != null)
            {
                passwords.ActivePerson.AddNewField();
                SelectedPasswordEntityIndex = passwords.ActivePerson.PasswordEntities.Count - 1;

                EditSelectedPasswordEntity();

                SetInfoRects();

            }
        }

        internal void AddNewPerson()
        {
            passwords.AddNewPerson();
            SelectedNameIndex = passwords.Persons.Count - 1;
            SetRectangles();
            ResetSelections();
        }

        public void SetRectangles()
        {
            SetNameRects();
            SetInfoRects();
        }

        private void SetInfoRects()
        {
            Infos = new List<string>();
            InfoRects = new List<RectangleF>();

            OldsPass = new List<string>();
            Oldpassrects = new List<RectangleF>();

            BackGroundBrushes = new List<Brush>();
            BackgroundRects = new List<RectangleF>();

            float x = NameListWidth + 50;
            float y = 20;
            Graphics g = Graphics.FromImage(new Bitmap(100, 100));


            BackGroundBrushes.Add(new SolidBrush(Color.FromArgb(1, 65, 65)));
            BackgroundRects.Add(new RectangleF(10, 10, NameListWidth + 10, 768));

            if (passwords.ActivePerson == null)
                return;

            AddInfoRects(g, ref x, y, "Name: ");
            AddInfoRects(g, ref x, y, passwords.ActivePerson.GetName());
            AddButtons_Edit(g, x, ref y);
            x = NameListWidth + 50;


            AddInfoRects(g, ref x, y, "Details: ");
            float h = AddInfoRects(g, ref x, y, passwords.ActivePerson.GetDetails());
            AddButtons_Edit(g, ref x, y);
            x = NameListWidth + 50;
            y += h + 5;

            AddInfoRects(g, ref x, y, "Extra Info: ");
            h = AddInfoRects(g, ref x, y, passwords.ActivePerson.GetExtraInfo());
            AddButtons_Edit(g, x, ref y);
            y += 20 + h;
            x = NameListWidth + 50;

            BackgroundRects.Add(new RectangleF(x - 20, 10, 1366, y - 20));
            BackGroundBrushes.Add(new SolidBrush(Color.FromArgb(150, Color.FromArgb(72, 143, 119))));

            for (int i = 0; i < passwords.ActivePerson.PasswordEntities.Count; i++)
            {

                float y_t = y;
                AddInfoRects(g, ref x, y, passwords.ActivePerson.PasswordEntities[i].GetPlainLabel());

                AddInfoRects(g, ref x, y, passwords.ActivePerson.PasswordEntities[i].GetPlainEmail());
                AddInfoRects(g, ref x, y, passwords.ActivePerson.PasswordEntities[i].GetPlainPassword());

                AddButtons_Copy(g, ref x, y);
                AddButtons_Edit(g, ref x, y);
                AddButtons_Delete(g, x, ref y);

                BackgroundRects.Add(new RectangleF(NameListWidth + 30, y_t - 3, 1366, y - y_t + 2));
                if (i % 2 == 0)
                    BackGroundBrushes.Add(new SolidBrush(Color.FromArgb(150, Color.FromArgb(4, 115, 97))));
                else
                    BackGroundBrushes.Add(new SolidBrush(Color.FromArgb(150, Color.FromArgb(0, 87, 81))));
                // y += height;
                x += 30;
                float oldpass_start_x = x;
                //for old passwords
                List<string> olds = passwords.ActivePerson.PasswordEntities[i].GetOldPasswords();
                for (int j = 0; j < passwords.ActivePerson.PasswordEntities[i].OldPasswords.Count; j++)
                {

                    AddoldPassRects(g, ref x, y, olds[j]);

                    AddOldPassButtons_Copy(g, ref x, y);
                    AddOldPassButtons_Edit(g, ref x, y);
                    AddOldpassButtons_Delete(g, x, ref y);
                    x = oldpass_start_x;
                }



                x = NameListWidth + 50;



            }


        }

        private void AddOldpassButtons_Delete(Graphics g, float x, ref float y)
        {

            OldsPass.Add("[Delete]");
            Oldpassrects.Add(new RectangleF(x, y, 25, 25));
            y += 30;
        }

        private void AddOldPassButtons_Edit(Graphics g, ref float x, float y)
        {

            OldsPass.Add("[Edit]");
            Oldpassrects.Add(new RectangleF(x, y, 25, 25));
            x += 30;
        }

        private void AddOldPassButtons_Copy(Graphics g, ref float x, float y)
        {

            OldsPass.Add("[Copy]");
            Oldpassrects.Add(new RectangleF(x, y, 25, 25));
            x += 30;
        }

        private void AddoldPassRects(Graphics g, ref float x, float y, string oldpass)
        {

            SizeF size = g.MeasureString(oldpass, FontInfo);
            OldsPass.Add(oldpass);
            Oldpassrects.Add(new RectangleF(x, y, size.Width, size.Height));
            x += size.Width + 5;

        }

        private void AddButtons_Delete(Graphics g, float x, ref float y)
        {


            Infos.Add("[Delete]");
            InfoRects.Add(new RectangleF(x, y, 25, 25));
            y += 30;
        }
        private float AddButtons_Delete(Graphics g, ref float x, float y)
        {


            Infos.Add("[Delete]");
            InfoRects.Add(new RectangleF(x, y, 25, 25));
            x += 30;

            return 30;
        }


        private void AddButtons_Edit(Graphics g, ref float x, float y)
        {

            Infos.Add("[Edit]");
            InfoRects.Add(new RectangleF(x, y, 25, 25));
            x += 30;
        }
        private void AddButtons_Edit(Graphics g, float x, ref float y)
        {
            x += 30;
            Infos.Add("[Edit]");
            InfoRects.Add(new RectangleF(x, y, 25, 25));
            y += 30;
        }


        private void AddButtons_Copy(Graphics g, ref float x, float y)
        {
            x += 30;
            Infos.Add("[Copy]");
            InfoRects.Add(new RectangleF(x, y, 25, 25));
            x += 30;
        }


        void AddInfoRects(Graphics g, float x, ref float y, string info)
        {
            SizeF size = g.MeasureString(info, FontInfo);
            Infos.Add(info);
            InfoRects.Add(new RectangleF(x, y, size.Width, size.Height));
            y += size.Height + 5;

        }
        string[] PassWordLabels = new string[] { "Apple", "Facebook", "Gmail", "Instagram", "Linkedin", "Pinterest", "Twitter", "Yahoo", "Hotmail", "Skype" };
        float AddInfoRects(Graphics g, ref float x, float y, string info)
        {
            SizeF size;
            if (PassWordLabels.Contains(info))
                size = new SizeF(25, 25);
            else
                size = g.MeasureString(info, FontInfo);

            Infos.Add(info);
            InfoRects.Add(new RectangleF(x, y, size.Width, size.Height));
            x += size.Width + 5;
            return size.Height;
        }

        void AddInfoRects(Graphics g, float x, float y, string info)
        {
            SizeF size = g.MeasureString(info, FontInfo);
            Infos.Add(info);
            InfoRects.Add(new RectangleF(x, y, size.Width, size.Height));

        }

        internal void MouseDoubleClick(Point location)
        {
            Select(location);
        }

        private void Select(Point location)
        {

            int index = FindSelection(location);

            if (selection == Selection.Info)
            {
                SelectedInfoIndex = index;
                if (infoSelection == InfoSelection.Delete)
                {
                    DeleteSelectedPasswordEntity();
                }
                else if (infoSelection == InfoSelection.Edit)
                {
                    EditSelectedPasswordEntity();
                }
                else if (infoSelection == InfoSelection.CopyPassword)
                {
                    CopyToClipboard();
                }
                else if (infoSelection == InfoSelection.CopyEmail)
                {
                    CopyToClipboardEmail();
                }
                else if (infoSelection == InfoSelection.Name || infoSelection == InfoSelection.Details || infoSelection == InfoSelection.ExtraInfo)
                {
                    EditPersonInfo();
                    if (infoSelection == InfoSelection.Name)
                    {
                        SetRectangles();
                    }
                }
                return;
            }

            if (selection == Selection.OldPass)
            {
                SelectedOldPassIndex = index;

                if (infoSelection == InfoSelection.Delete)
                {
                    DeleteSelectedOldPass();
                }
                else if (infoSelection == InfoSelection.Edit)
                {
                    EditOldPassword();
                }
                else if (infoSelection == InfoSelection.Copy)
                {
                    CopyToClipboardOldPass();
                }
                return;

            }
            if (selection == Selection.Name)
            {
                SelectedNameIndex = index;
                SetActivePerson(SelectedNameIndex);

                _shiftindex_info = new Point(0, 0);
            }
        }

        private void CopyToClipboardEmail()
        {
            if (SelectedPasswordEntityIndex != -1)
                Clipboard.SetText(passwords.ActivePerson.PasswordEntities[SelectedPasswordEntityIndex].GetPlainEmail());

        }


        private void CopyToClipboard()
        {
            if (SelectedPasswordEntityIndex != -1)
                Clipboard.SetText(passwords.ActivePerson.PasswordEntities[SelectedPasswordEntityIndex].GetPlainPassword());
        }

        private void CopyToClipboardOldPass()
        {

            if (SelectedOldPasswordParentIndex != -1 && SelectedOldPassListIndex != -1)
                Clipboard.SetText(passwords.ActivePerson.PasswordEntities[SelectedOldPasswordParentIndex].GetOldPasswords()[SelectedOldPassListIndex]);

        }

        private void EditOldPassword()
        {
            Edit_old_Pass old = new Edit_old_Pass(ref passwords, SelectedOldPasswordParentIndex, SelectedOldPassListIndex);
            old.ShowDialog();
            SetInfoRects();
        }

        private void DeleteSelectedOldPass()
        {
            if (SelectedOldPasswordParentIndex != -1 && SelectedOldPassListIndex != -1)
            {
                passwords.ActivePerson.PasswordEntities[SelectedOldPasswordParentIndex].DeleteOldPassAt(SelectedOldPassListIndex);
                Savepassword();
                ResetSelections();
                SetInfoRects();
            }
        }

        private void EditPersonInfo()
        {
            Edit_Person_Info en = new Edit_Person_Info(ref passwords, SelectedPasswordEntityIndex, infoSelection);
            en.ShowDialog();
            SetInfoRects();
        }

        private void EditSelectedPasswordEntity()
        {
            EditPasswordEntity en = new EditPasswordEntity(ref passwords, SelectedPasswordEntityIndex);
            en.ShowDialog();
            // ResetSelections();
            SetInfoRects();
        }

        private void DeleteSelectedPasswordEntity()
        {
            passwords.ActivePerson.DeleteEntity(SelectedPasswordEntityIndex);
            Savepassword();
            ResetSelections();
            SetInfoRects();
        }

        private void SetActivePerson(int selectedNameIndex)
        {
            passwords.SetActivePeron(selectedNameIndex);
            SetInfoRects();
            ResetSelections();
        }

        private void ResetSelections()
        {
            HoveredNameIndex = -1;
            HoveredInfoIndex = -1;
            HoveredOldPassIndex = -1;
            SelectedPasswordEntityIndex = -1;
            SelectedOldPassListIndex = -1;
            SelectedOldPassIndex = -1;
            selection = Selection.None;
            infoSelection = InfoSelection.None;
        }

        bool drag = false;
        Point initial, InitialMouseDown;
        private Point _shiftindex_info;

        internal void MouseUp(Point location)
        {
            drag = false;
            if (InitialMouseDown == location)
                Select(location);

        }

        internal void MouseDown(Point location)
        {
            drag = true;
            initial = location;
            InitialMouseDown = location;


        }
        internal void MouseMove(Point location)
        {
            if (drag)
            {
                if (location.X <= NameListWidth)
                {

                }
                else
                {
                    ShiftIndex_Info = new Point(0, ShiftIndex_Info.Y + location.Y - initial.Y);
                    initial = location;
                }
            }
            else
            {
                int index = FindSelection(location);
                if (selection == Selection.Name)
                    HoveredNameIndex = index;
                else if (selection == Selection.Info)
                {
                    HoveredInfoIndex = index;
                    InfoHovered();
                }
                else if (selection == Selection.OldPass)
                {
                    HoveredOldPassIndex = index;
                    OldPassHovered();
                }
            }
        }

        private void OldPassHovered()
        {
            infoSelection = InfoSelection.None;
            SelectedOldPassListIndex = -1;
            SelectedOldPasswordParentIndex = -1;
            OldPassSelectionStartIndex = -1;


            if (HoveredOldPassIndex >= 0)
            {

                int oldpassrectindex = HoveredOldPassIndex / 4;
                int total = -1;
                for (int i = 0; i < passwords.ActivePerson.PasswordEntities.Count; i++)
                {
                    if (passwords.ActivePerson.PasswordEntities[i].OldPasswords.Count == 0)
                        continue;
                    total += passwords.ActivePerson.PasswordEntities[i].OldPasswords.Count;
                    if (oldpassrectindex <= total)
                    {
                        SelectedOldPasswordParentIndex = i;
                        total -= passwords.ActivePerson.PasswordEntities[i].OldPasswords.Count;
                        for (int j = 0; j < passwords.ActivePerson.PasswordEntities[i].OldPasswords.Count; j++)
                        {
                            total++;
                            if (total == oldpassrectindex)
                            {

                                SelectedOldPassListIndex = j;
                                break;
                            }
                        }

                        break;
                    }
                }
            }


            OldPassSelectionStartIndex = (HoveredOldPassIndex / 4) * 4;

            if (HoveredOldPassIndex % 4 == 0 || HoveredOldPassIndex % 4 == 1)
            {
                infoSelection = InfoSelection.Copy;
            }
            else if (HoveredOldPassIndex % 4 == 2)
            {
                infoSelection = InfoSelection.Edit;
            }
            else if (HoveredOldPassIndex % 4 == 3)
            {
                infoSelection = InfoSelection.Delete;
            }

        }


        private void InfoHovered()
        {
            infoSelection = InfoSelection.None;
            SelectedPasswordEntityIndex = -1;
            //Name Details and extra info Fields
            if (HoveredInfoIndex < 9)
            {
                if (HoveredInfoIndex == 1 || HoveredInfoIndex == 2)
                {
                    infoSelection = InfoSelection.Name;
                }
                else if (HoveredInfoIndex == 4 || HoveredInfoIndex == 5)
                {
                    infoSelection = InfoSelection.Details;
                }
                else if (HoveredInfoIndex == 7 || HoveredInfoIndex == 8)
                {
                    infoSelection = InfoSelection.ExtraInfo;
                }
            }
            // password Field Selection
            else
            {
                SelectedPasswordEntityIndex = (HoveredInfoIndex - 9) / 6;
                InfoSelectionStartIndex = (SelectedPasswordEntityIndex * 6 + 9);

                if (HoveredInfoIndex % 6 == 3 || HoveredInfoIndex % 6 == 1)
                {
                    infoSelection = InfoSelection.Edit;
                }
                else if (HoveredInfoIndex % 6 == 4)
                {

                    infoSelection = InfoSelection.CopyEmail;
                }
                else if (HoveredInfoIndex % 6 == 5 || HoveredInfoIndex % 6 == 0)
                {
                    infoSelection = InfoSelection.CopyPassword;
                }
                else if (HoveredInfoIndex % 6 == 2)
                {
                    infoSelection = InfoSelection.Delete;
                }
            }
        }



        private int FindSelection(Point location)
        {
            RectangleF mouserect = new RectangleF(location, new Size(1, 1));
            for (int i = 0; i < InfoRects.Count; i++)
            {
                if (mouserect.IntersectsWith(ShiftedInfoRect(InfoRects[i])))
                {
                    selection = Selection.Info;
                    return i;
                }
            }
            for (int i = 0; i < Oldpassrects.Count; i++)
            {
                if (mouserect.IntersectsWith(ShiftedInfoRect(Oldpassrects[i])))
                {
                    selection = Selection.OldPass;
                    return i;
                }
            }
            for (int i = 0; i < namerects.Count; i++)
            {
                if (mouserect.IntersectsWith(namerects[i]))
                {
                    selection = Selection.Name;
                    return i;
                }
            }
            selection = Selection.None;
            return -1;

        }


        public void Savepassword()
        {
            passwords.Save();
        }
        public void LoadPassword()
        {
            passwords.Load();
            if (passwords.ActivePerson != null)
                SelectedNameIndex = passwords.Persons.Count - 1;
        }

        private void SetNameRects()
        {
            namerects = new List<RectangleF>();
            names = new List<string>();
            SizeF size;
            Graphics g = Graphics.FromImage(new Bitmap(100, 100));
            float x = 10;
            float y = 10;
            NameListWidth = 0;
            for (int i = 0; i < passwords.Persons.Count; i++)
            {
                size = g.MeasureString(passwords.Persons[i].GetName(), FontName);
                namerects.Add(new RectangleF(x, y, size.Width, size.Height));
                names.Add(passwords.Persons[i].GetName());
                y += size.Height + 5;
                if (NameListWidth < size.Width)
                    NameListWidth = size.Width;
            }




        }


        public void Paint(Graphics g)
        {
            PaintBackGrounds(g);

            PaintNameSelection(g);
            PaintSelectedInfo(g);
            PaintPersons(g);
            PaintActivePerson(g);
            PaintOldPasswords(g);
            PaintSelectedOldPass(g);

            if (selection == Selection.Info)
                PaintInfoSelection(g);
            else if (selection == Selection.OldPass)
                PaintOldPassSelection(g);

            //g.DrawString("SelectedOldPasswordParentIndex " + SelectedOldPasswordParentIndex, FontInfo, Brushes.Yellow, new Point(0, 0));
            //g.DrawString("SelectedOldPassListIndex " + SelectedOldPassListIndex, FontInfo, Brushes.Yellow, new Point(300, 0));
            //  g.DrawString("Hovered Index " + HoveredInfoIndex, FontInfo, Brushes.Yellow, new Point(500, 0));

        }

        private void PaintBackGrounds(Graphics g)
        {
            for (int i = 0; i < BackgroundRects.Count; i++)
            {
                if (i == 0)
                    g.FillRectangle(BackGroundBrushes[i], BackgroundRects[i]);
                else
                    g.FillRectangle(BackGroundBrushes[i], ShiftedInfoRect(BackgroundRects[i]));
            }
        }

        private void PaintSelectedOldPass(Graphics g)
        {
            if (HoveredOldPassIndex != -1)
                g.DrawRectangle(Pens.White, Oldpassrects[HoveredOldPassIndex].X, ShiftIndex_Info.Y + Oldpassrects[HoveredOldPassIndex].Y, Oldpassrects[HoveredOldPassIndex].Width, Oldpassrects[HoveredOldPassIndex].Height);

        }

        private void PaintOldPassSelection(Graphics g)
        {
            switch (infoSelection)
            {


                case InfoSelection.Copy:
                    if (OldPassSelectionStartIndex != -1)
                        g.DrawRectangle(Pens.White, Oldpassrects[OldPassSelectionStartIndex].X, ShiftIndex_Info.Y + Oldpassrects[OldPassSelectionStartIndex].Y, Oldpassrects[OldPassSelectionStartIndex].Width, Oldpassrects[OldPassSelectionStartIndex].Height);
                    break;

                case InfoSelection.Edit:
                    if (OldPassSelectionStartIndex != -1)
                        g.DrawRectangle(Pens.Green, Oldpassrects[OldPassSelectionStartIndex].X, ShiftIndex_Info.Y + Oldpassrects[OldPassSelectionStartIndex].Y, Oldpassrects[OldPassSelectionStartIndex].Width, Oldpassrects[OldPassSelectionStartIndex].Height);
                    break;

                case InfoSelection.Delete:
                    if (OldPassSelectionStartIndex != -1)
                        g.DrawRectangle(Pens.Red, Oldpassrects[OldPassSelectionStartIndex].X, ShiftIndex_Info.Y + Oldpassrects[OldPassSelectionStartIndex].Y, Oldpassrects[OldPassSelectionStartIndex].Width, Oldpassrects[OldPassSelectionStartIndex].Height);
                    break;
            }

        }

        private void PaintOldPasswords(Graphics g)
        {


            for (int i = 0; i < OldsPass.Count; i++)
            {
                switch (OldsPass[i])
                {
                    case "[Copy]":
                        g.DrawImage(Properties.Resources.copyclip, ShiftedInfoRect(Oldpassrects[i]));
                        break;
                    case "[Edit]":
                        g.DrawImage(Properties.Resources.edit_2, ShiftedInfoRect(Oldpassrects[i]));
                        break;
                    case "[Delete]":
                        g.DrawImage(Properties.Resources.remove, ShiftedInfoRect(Oldpassrects[i]));
                        break;
                    default:
                        g.DrawString(OldsPass[i], FontInfo, Brushes.White, ShiftedInfoRect(Oldpassrects[i]));
                        break;
                }

            }

        }

        private void PaintInfoSelection(Graphics g)
        {
            switch (infoSelection)
            {
                case InfoSelection.Name:
                    g.DrawRectangle(Pens.Red, InfoRects[1].X, ShiftIndex_Info.Y + InfoRects[1].Y, InfoRects[1].Width, InfoRects[1].Height);
                    break;

                case InfoSelection.Details:
                    g.DrawRectangle(Pens.Red, InfoRects[4].X, ShiftIndex_Info.Y + InfoRects[4].Y, InfoRects[4].Width, InfoRects[4].Height);
                    break;

                case InfoSelection.ExtraInfo:
                    g.DrawRectangle(Pens.Red, InfoRects[7].X, ShiftIndex_Info.Y + InfoRects[7].Y, InfoRects[7].Width, InfoRects[7].Height);
                    break;

                case InfoSelection.CopyEmail:
                    if (InfoSelectionStartIndex != -1)
                        g.DrawRectangle(Pens.White, InfoRects[InfoSelectionStartIndex + 1].X, ShiftIndex_Info.Y + InfoRects[InfoSelectionStartIndex + 1].Y, InfoRects[InfoSelectionStartIndex + 1].Width, InfoRects[InfoSelectionStartIndex + 1].Height);
                    break;


                case InfoSelection.CopyPassword:
                    if (InfoSelectionStartIndex != -1)
                        g.DrawRectangle(Pens.White, InfoRects[InfoSelectionStartIndex + 2].X, ShiftIndex_Info.Y + InfoRects[InfoSelectionStartIndex + 2].Y, InfoRects[InfoSelectionStartIndex + 2].Width, InfoRects[InfoSelectionStartIndex + 2].Height);
                    break;
                case InfoSelection.Edit:
                    if (InfoSelectionStartIndex != -1)
                        g.DrawRectangle(Pens.Green, InfoRects[InfoSelectionStartIndex].X, ShiftIndex_Info.Y + InfoRects[InfoSelectionStartIndex].Y, 10 + InfoRects[InfoSelectionStartIndex].Width + InfoRects[InfoSelectionStartIndex + 1].Width + InfoRects[InfoSelectionStartIndex + 2].Width, InfoRects[InfoSelectionStartIndex].Height);
                    break;

                case InfoSelection.Delete:
                    if (InfoSelectionStartIndex != -1)
                        g.DrawRectangle(Pens.Red, InfoRects[InfoSelectionStartIndex].X, ShiftIndex_Info.Y + InfoRects[InfoSelectionStartIndex].Y, 10 + InfoRects[InfoSelectionStartIndex].Width + InfoRects[InfoSelectionStartIndex + 1].Width + InfoRects[InfoSelectionStartIndex + 2].Width, InfoRects[InfoSelectionStartIndex].Height);
                    break;
            }
        }

        private void PaintSelectedInfo(Graphics g)
        {
            if (SelectedInfoIndex != -1 && SelectedInfoIndex < InfoRects.Count)
                g.FillRectangle(Brushes.Blue, ShiftedInfoRect(InfoRects[SelectedInfoIndex]));
            if (HoveredInfoIndex != -1 && HoveredInfoIndex < InfoRects.Count)

                g.FillRectangle(Brushes.Black, ShiftedInfoRect(InfoRects[HoveredInfoIndex]));
        }

        private void PaintNameSelection(Graphics g)
        {
            if (SelectedNameIndex != -1 && SelectedNameIndex < namerects.Count)
                g.FillRectangle(Brushes.Black, namerects[SelectedNameIndex]);

            if (HoveredNameIndex != -1 && HoveredNameIndex < namerects.Count)

                g.FillRectangle(Brushes.White, namerects[HoveredNameIndex]);
        }

        private void PaintActivePerson(Graphics g)
        {
            for (int i = 0; i < InfoRects.Count; i++)
            {
                switch (Infos[i])
                {
                    case "[Copy]":
                        g.DrawImage(Properties.Resources.copyclip, ShiftedInfoRect(InfoRects[i]));
                        break;
                    case "[Edit]":
                        g.DrawImage(Properties.Resources.edit_2, ShiftedInfoRect(InfoRects[i]));
                        break;
                    case "[Delete]":
                        g.DrawImage(Properties.Resources.remove, ShiftedInfoRect(InfoRects[i]));
                        break;
                    case "Apple":
                        g.DrawImage(Properties.Resources.apple, ShiftedInfoRect(InfoRects[i]));
                        break;

                    case "Facebook":
                        g.DrawImage(Properties.Resources.fb, ShiftedInfoRect(InfoRects[i]));
                        break;

                    case "Gmail":
                        g.DrawImage(Properties.Resources.google, ShiftedInfoRect(InfoRects[i]));
                        break;

                    case "Instagram":
                        g.DrawImage(Properties.Resources.Instagram, ShiftedInfoRect(InfoRects[i]));
                        break;

                    case "Linkedin":
                        g.DrawImage(Properties.Resources.linkedin, ShiftedInfoRect(InfoRects[i]));
                        break;

                    case "Pinterest":
                        g.DrawImage(Properties.Resources.pininterest, ShiftedInfoRect(InfoRects[i]));
                        break;

                    case "Twitter":
                        g.DrawImage(Properties.Resources.twitter, ShiftedInfoRect(InfoRects[i]));
                        break;
                    case "Yahoo":
                        g.DrawImage(Properties.Resources.yahoo, ShiftedInfoRect(InfoRects[i]));
                        break;

                    case "Hotmail":
                        g.DrawImage(Properties.Resources.MSN, ShiftedInfoRect(InfoRects[i]));
                        break;

                    case "Skype":
                        g.DrawImage(Properties.Resources.skype, ShiftedInfoRect(InfoRects[i]));
                        break;
                    default:
                        g.DrawString(Infos[i], FontInfo, Brushes.White, ShiftedInfoRect(InfoRects[i]));
                        break;
                }

            }
        }

        private RectangleF ShiftedInfoRect(RectangleF rectangleF)
        {
            return new RectangleF(rectangleF.X, rectangleF.Y + ShiftIndex_Info.Y, rectangleF.Width, rectangleF.Height);
        }

        private void PaintPersons(Graphics g)

        {
            for (int i = 0; i < namerects.Count; i++)
            {
                g.DrawString(names[i], FontName, Brushes.Gold, namerects[i]);
            }
        }
    }

    [Serializable]
    public class PassWords
    {

        public List<Person> Persons { get; set; }
        [NonSerialized]
        public Person ActivePerson;
        [NonSerialized]
        string ActivePasswordPath;
        public PassWords(string text)
        {
            Persons = new List<Person>();
            ActivePerson = null;
            ActivePasswordPath = text;
        }
        public void Load()
        {
            if (File.Exists(Path.Combine(ActivePasswordPath, "Keys")))
            {
                PassWords pass = SD.Serialization.LoadObject<PassWords>(Path.Combine(ActivePasswordPath, "Keys"));
                if (pass != null)
                    this.Persons = pass.Persons;


                //SD.XML.SerializeDeSerialize seri = new SD.XML.SerializeDeSerialize();
                //PassWords pass = seri.DeSerializeObject<PassWords>(Path.Combine(ActivePasswordPath, "Keys"));
                //this.Persons = pass.Persons;

                if (this.Persons.Count > 0)
                    ActivePerson = this.Persons[this.Persons.Count - 1];
                else
                    ActivePerson = null;
            }
            else
                Persons = new List<Person>();
        }
        public void Save()
        {
            if (!Directory.Exists(ActivePasswordPath))
                Directory.CreateDirectory(ActivePasswordPath);


            //SD.XML.SerializeDeSerialize seri = new SD.XML.SerializeDeSerialize();
            //seri.SerializeObject<PassWords>(this, Path.Combine(ActivePasswordPath, "Keys"));
             SD.Serialization.SaveObject<PassWords>(Path.Combine(ActivePasswordPath, "Keys"), this);
        }
        public void AddNewPerson()
        {
            Person p = new Person();

            Persons.Add(p);
            SetActivePeron(Persons.Count - 1);

        }

        internal void SetActivePeron(int selectedNameIndex)
        {
            if (selectedNameIndex < Persons.Count)
                ActivePerson = Persons[selectedNameIndex];
        }

        internal void DeleteActivePerson()
        {
            if (ActivePerson != null)
            {
                Persons.Remove(ActivePerson);
                ActivePerson = null;
                Save();
            }
        }
    }

    [Serializable]
    public class Person
    {
        public String Name { get; set; }
        public String Details { get; set; }
        public String ExtraInfo { get; set; }
        public List<PasswordEntity> PasswordEntities { get; set; }

        public Person()
        {
            SetName("New Name");
            SetDetails("None");
            SetExtraInfo("None");
            PasswordEntities = new List<PasswordEntity>();
        }
        public void SetName(string name)
        {
            this.Name = SD.Encryption.Crypto.EncryptStringAES(name, "loca#2@31..");
        }
        public void SetDetails(string details)
        {

            this.Details = SD.Encryption.Crypto.EncryptStringAES(details, "loca#2@31..");
        }
        public void SetExtraInfo(string extrainfo)
        {

            this.ExtraInfo = SD.Encryption.Crypto.EncryptStringAES(extrainfo, "loca#2@31..");
        }
        public string GetName()
        {
            return SD.Encryption.Crypto.DecryptStringAES(this.Name, "loca#2@31..");
        }
        public string GetDetails()
        {

            return SD.Encryption.Crypto.DecryptStringAES(this.Details, "loca#2@31..");
        }
        public string GetExtraInfo()
        {

            return SD.Encryption.Crypto.DecryptStringAES(this.ExtraInfo, "loca#2@31..");
        }
        void AddNewPassword(PasswordEntity passentity)
        {
            PasswordEntities.Add(passentity);
        }
        public void SetEditPassword(int index, PasswordEntity entity)
        {
            PasswordEntities[index] = entity;
        }

        internal void AddNewField()
        {
            AddNewPassword(new PasswordEntity());
        }

        internal void DeleteEntity(int selectedPasswordEntityIndex)
        {
            PasswordEntities.RemoveAt(selectedPasswordEntityIndex);


        }
    }

    [Serializable]
    public class PasswordEntity
    {
        public String Label { get; set; }
        public String Password { get; set; }
        public String Email { get; set; }
        public List<String> OldPasswords { get; set; }

        public PasswordEntity()
        {
            SetLabel("New Label");
            SetPassword("New Password");
            SetEmail("New Email");
            OldPasswords = new List<String>();
        }



        public PasswordEntity(string label, string password, string email)
        {
            SetLabel(label);
            SetPassword(password);
            SetEmail(email);
        }
        public void SetLabel(string label)
        {
            this.Label = SD.Encryption.Crypto.EncryptStringAES(label, "loca#2@31..");
        }
        public void SetEmail(string email)
        {

            this.Email = SD.Encryption.Crypto.EncryptStringAES(email, "loca#2@31..");
        }
        public void SetPassword(string password)
        {
            // If there is alread a password, save it to oldpasswords before replacing with new
            if (GetPlainPassword() != null)
            {
                if (OldPasswords == null)
                    OldPasswords = new List<string>();
                OldPasswords.Add(this.Password);
            }
            this.Password = SD.Encryption.Crypto.EncryptStringAES(password, "loca#2@31..");

        }

        public string GetPlainLabel()
        {
            return SD.Encryption.Crypto.DecryptStringAES(this.Label, "loca#2@31..");
        }
        public string GetPlainPassword()
        {
            return SD.Encryption.Crypto.DecryptStringAES(this.Password, "loca#2@31..");
        }
        public string GetPlainEmail()
        {

            return SD.Encryption.Crypto.DecryptStringAES(this.Email, "loca#2@31..");
        }
        public List<string> GetOldPasswords()
        {
            List<string> olds = new List<string>();
            for (int i = 0; i < OldPasswords.Count; i++)
            {
                olds.Add(SD.Encryption.Crypto.DecryptStringAES(OldPasswords[i], "loca#2@31.."));
            }
            return olds;
        }

        internal void DeleteOldPassAt(int selectedOldPassListIndex)
        {
            if (selectedOldPassListIndex < OldPasswords.Count)
                OldPasswords.RemoveAt(selectedOldPassListIndex);
        }

        internal void SetOldPasswordAt(int selectedOldPassListIndex, string text)
        {
            if (selectedOldPassListIndex < OldPasswords.Count)
                OldPasswords[selectedOldPassListIndex] = SD.Encryption.Crypto.EncryptStringAES(text, "loca#2@31..");
        }
    }
}
