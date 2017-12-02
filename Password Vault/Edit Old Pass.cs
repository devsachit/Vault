using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vault
{
    public partial class Edit_old_Pass : Form
    {
        private PassWords passwords;
        private int selectedOldPasswordParentIndex;
        private int selectedOldPassListIndex;



        public Edit_old_Pass(ref PassWords passwords, int selectedOldPasswordParentIndex, int selectedOldPassListIndex)
        {
            InitializeComponent();
            this.passwords = passwords;
            this.selectedOldPasswordParentIndex = selectedOldPasswordParentIndex;
            this.selectedOldPassListIndex = selectedOldPassListIndex;

            textBox1.Text = passwords.ActivePerson.PasswordEntities[selectedOldPasswordParentIndex].GetOldPasswords()[selectedOldPassListIndex];
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            passwords.ActivePerson.PasswordEntities[selectedOldPasswordParentIndex].SetOldPasswordAt(selectedOldPassListIndex, textBox1.Text);
            passwords.Save();
            this.Dispose();
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
