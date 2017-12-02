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
    public partial class GetStringForm : Form
    {
     public   bool IsTextEntered = false;
     public   string TextEntered = "";
        public GetStringForm(string text)
        {
            InitializeComponent();
            textBox1.Text = text;
            IsTextEntered = false;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox1.Text.Length > 0)
                {
                    TextEntered = textBox1.Text;
                    IsTextEntered = true;
                    this.Dispose();
                }
                else
                {
                    MessageBox.Show("Please Enter some value");
                    IsTextEntered = false;
                }
            }
            else if(e.KeyCode==Keys.Escape)
            {
                IsTextEntered = false;
                this.Dispose();
            }
        }
    }
}
