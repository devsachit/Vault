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
    public partial class Edit_Person_Info : Form
    {
        private PassWords passwords;
        private int selectedPasswordEntityIndex;
        private PasswordController.InfoSelection infoSelection;


        public Edit_Person_Info(ref PassWords passwords, int selectedPasswordEntityIndex, PasswordController.InfoSelection infoSelection)
        {

            InitializeComponent();
            this.passwords = passwords;
            this.selectedPasswordEntityIndex = selectedPasswordEntityIndex;
            this.infoSelection = infoSelection;

            label1.Text = infoSelection.ToString();
            if (infoSelection == PasswordController.InfoSelection.Name)
                textBox1.Text = passwords.ActivePerson.GetName();
            else
            if (infoSelection == PasswordController.InfoSelection.Details)
            {
                textBox1.Text = passwords.ActivePerson.GetDetails();
                textBox1.Multiline = true;

                textBox1.Height = 45;
            }
            else
            if (infoSelection == PasswordController.InfoSelection.ExtraInfo)
            {
                textBox1.Text = passwords.ActivePerson.GetExtraInfo();
                textBox1.Multiline = true;
                textBox1.Height = 45;
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (infoSelection == PasswordController.InfoSelection.Name)
                passwords.ActivePerson.SetName(textBox1.Text);
            else
            if (infoSelection == PasswordController.InfoSelection.Details)
                passwords.ActivePerson.SetDetails(textBox1.Text);
            else
            if (infoSelection == PasswordController.InfoSelection.ExtraInfo)
                passwords.ActivePerson.SetExtraInfo(textBox1.Text);
            passwords.Save();
            this.Dispose();
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
