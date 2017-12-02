using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vault
{
    public partial class Password_Vault : Form
    {
        public string ActivePasswordPath;
        public Password_Vault(string activepath)
        {
            InitializeComponent();
            ActivePasswordPath = Path.Combine(activepath, "Passwords");
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            passController = new PasswordController(ActivePasswordPath);
            passController.LoadPassword();
            passController.SetRectangles();

        }
        PasswordController passController;

        private void Password_Vault_Paint(object sender, PaintEventArgs e)
        {
            passController.Paint(e.Graphics);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.N)
            {
                passController.AddNewPerson();

            }
            else if (e.KeyCode == Keys.P)
            {
                passController.AddNewPasswordField();

            }
            else if (e.KeyCode == Keys.S)
            {
                passController.Savepassword();
            }
            else if (e.KeyCode == Keys.Delete)
            {

                passController.DeleteActivePerson();

            }
          else  if (e.KeyCode == Keys.Escape)
            {

                passController.Savepassword();
                this.Dispose();
            }
            this.Invalidate();
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            passController.MouseWheel(e.Delta);
            this.Invalidate();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            passController.MouseDown(e.Location);
            this.Invalidate();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            passController.MouseUp(e.Location);
            this.Invalidate();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            passController.MouseMove(e.Location);
            this.Invalidate();
        }
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            passController.MouseDoubleClick(e.Location);
            this.Invalidate();
        }

        private void addPasswordFieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            passController.AddNewPasswordField();
            this.Invalidate();
        }

        private void addPersonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            passController.AddNewPerson();
            this.Invalidate();
        }

        private void Password_Vault_FormClosing(object sender, FormClosingEventArgs e)
        {
            passController.Savepassword();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            passController.Savepassword();
        }

        private void deleteThisPersonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            passController.DeleteActivePerson();
            this.Invalidate();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

            passController.Savepassword();
            this.Dispose();
        }
    }
}
