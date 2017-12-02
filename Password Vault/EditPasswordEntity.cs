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
    public partial class EditPasswordEntity : Form
    {
        PassWords password;
        int selectedPasswordEntityIndex;
        public EditPasswordEntity(ref PassWords passwords, int selectedPasswordEntityIndex)
        {
            InitializeComponent();
            this.password = passwords;
            this.selectedPasswordEntityIndex = selectedPasswordEntityIndex;


            tb_label.Text = password.ActivePerson.PasswordEntities[selectedPasswordEntityIndex].GetPlainLabel();
            if (comboBox1.Items.Contains(tb_label.Text))
            {
                comboBox1.SelectedItem = tb_label.Text;
            }
            else
            {
                comboBox1.SelectedItem = "Custom";
            }
            tb_pass.Text = password.ActivePerson.PasswordEntities[selectedPasswordEntityIndex].GetPlainPassword();
            tb_email.Text = password.ActivePerson.PasswordEntities[selectedPasswordEntityIndex].GetPlainEmail();
        }

        private void EditPasswordEntity_Load(object sender, EventArgs e)
        {

        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            password.ActivePerson.PasswordEntities[selectedPasswordEntityIndex].SetLabel(tb_label.Text);

            password.ActivePerson.PasswordEntities[selectedPasswordEntityIndex].SetEmail(tb_email.Text);

            if (password.ActivePerson.PasswordEntities[selectedPasswordEntityIndex].GetPlainPassword() != tb_pass.Text)
                password.ActivePerson.PasswordEntities[selectedPasswordEntityIndex].SetPassword(tb_pass.Text);

            password.Save();
            this.Dispose();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null && comboBox1.SelectedItem.ToString() != "Custom")
            {
                tb_label.Text = comboBox1.SelectedItem.ToString();
                tb_label.Enabled = false;
            }
            else
            {
                tb_label.Enabled = true;
            }
        }
        private void tb_email_TextChanged(object sender, EventArgs e)
        {
            if (tb_label.Text == "Custom" || tb_label.Text == "New Label")
            {
                if (tb_email.Text.ToLower().Contains("gmail"))
                {
                    tb_label.Text = "Gmail";
                }
                else if (tb_email.Text.ToLower().Contains("facebook"))
                {
                    tb_label.Text = "Facebook";
                }
                else if (tb_email.Text.ToLower().Contains("yahoo"))
                {
                    tb_label.Text = "Yahoo";
                }
                else if (tb_email.Text.ToLower().Contains("hotmail"))
                {
                    tb_label.Text = "MSN";
                }

            }
        }
    }
}
