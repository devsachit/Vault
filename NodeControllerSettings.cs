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
    public partial class NodeControllerSettings : Form
    {
        public NodeControllerSettings()
        {
            InitializeComponent();
            textBox1.Text = Properties.Settings.Default.VaultPath;
            ReLoadListbox();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBox1.SelectedItem!=null)
            textBox1.Text = listBox1.SelectedItem.ToString();
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (btn_add.Text == "Add")
            {
                for (int i = 0; i < Properties.Settings.Default.VaultPaths.Count; i++)
                {
                    if (Properties.Settings.Default.VaultPaths[i].ToLower() == textBox1.Text.ToLower())
                    {
                        MessageBox.Show("Already Exists!");
                        return;
                    }
                }

                Properties.Settings.Default.VaultPaths.Add(textBox1.Text);
                Properties.Settings.Default.Save();
                ReLoadListbox();
                btn_add.Text = "Open";


            }
            else if (btn_add.Text == "Open")
            {
                Properties.Settings.Default.VaultPath = textBox1.Text;
                Properties.Settings.Default.Save();
                for (int i = 0; i < Controller.controllers.Count; i++)
                {
                    if (Controller.controllers[i].ActiveFolder == textBox1.Text)
                    {

                        Controller.OpenNodeController(Controller.controllers[i]);
                        this.Dispose();
                        return;
                    }
                }

                NodeController con = new NodeController(textBox1.Text);
                Controller.controllers.Add(con);
                Controller.OpenNodeController(con);
                this.Dispose();
            }
        }

        private void ReLoadListbox()
        {
            listBox1.Items.Clear();
            foreach (string path in Properties.Settings.Default.VaultPaths)
            {
                listBox1.Items.Add(path);
            }
            if(listBox1.Items.Contains(textBox1.Text))
            {
                listBox1.Items.Remove(textBox1.Text);
            }
            listBox1.Items.Add(textBox1.Text);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            bool contains = false;
            foreach (string path in Properties.Settings.Default.VaultPaths)
            {
                if (textBox1.Text.ToLower() == path.ToLower())
                {
                    contains = true;
                    break;
                }
            }
            if (contains)
                btn_add.Text = "Open";
            else
                btn_add.Text = "Add";
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Delete)
            {
                string text = "";
                if (listBox1.SelectedItem != null)
                     text = listBox1.SelectedItem.ToString();
                if(text!="" && Properties.Settings.Default.VaultPaths.Contains(text))
                {
                    if (Properties.Settings.Default.VaultPath == text)
                    {
                        Properties.Settings.Default.VaultPath = "Vault Folder";
                        textBox1.Text = "Vault Folder";
                    }
                        Properties.Settings.Default.VaultPaths.Remove(text);
                    Properties.Settings.Default.Save();
                    ReLoadListbox();
                }
            }
        }
    }
}
