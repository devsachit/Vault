namespace Vault.Note_Vault
{
    partial class Writer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tb_title = new System.Windows.Forms.TextBox();
            this.tb_content = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createNewNoteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previousNoteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteNoteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tb_title
            // 
            this.tb_title.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_title.Location = new System.Drawing.Point(0, 0);
            this.tb_title.Multiline = true;
            this.tb_title.Name = "tb_title";
            this.tb_title.Size = new System.Drawing.Size(928, 27);
            this.tb_title.TabIndex = 0;
            // 
            // tb_content
            // 
            this.tb_content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_content.Location = new System.Drawing.Point(0, 0);
            this.tb_content.Multiline = true;
            this.tb_content.Name = "tb_content";
            this.tb_content.Size = new System.Drawing.Size(928, 467);
            this.tb_content.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.createNewNoteToolStripMenuItem,
            this.previousNoteToolStripMenuItem,
            this.deleteNoteToolStripMenuItem,
            this.readModeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(928, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // createNewNoteToolStripMenuItem
            // 
            this.createNewNoteToolStripMenuItem.Name = "createNewNoteToolStripMenuItem";
            this.createNewNoteToolStripMenuItem.Size = new System.Drawing.Size(109, 20);
            this.createNewNoteToolStripMenuItem.Text = "Create New Note";
            this.createNewNoteToolStripMenuItem.Click += new System.EventHandler(this.btn_new_Click);
            // 
            // previousNoteToolStripMenuItem
            // 
            this.previousNoteToolStripMenuItem.Name = "previousNoteToolStripMenuItem";
            this.previousNoteToolStripMenuItem.Size = new System.Drawing.Size(93, 20);
            this.previousNoteToolStripMenuItem.Text = "Previous Note";
            this.previousNoteToolStripMenuItem.Click += new System.EventHandler(this.btn_previous_Click);
            // 
            // deleteNoteToolStripMenuItem
            // 
            this.deleteNoteToolStripMenuItem.Name = "deleteNoteToolStripMenuItem";
            this.deleteNoteToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.deleteNoteToolStripMenuItem.Text = "Delete Note";
            this.deleteNoteToolStripMenuItem.Click += new System.EventHandler(this.btn_delete_Click);
            // 
            // readModeToolStripMenuItem
            // 
            this.readModeToolStripMenuItem.Name = "readModeToolStripMenuItem";
            this.readModeToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.readModeToolStripMenuItem.Text = "Read Mode";
            this.readModeToolStripMenuItem.Click += new System.EventHandler(this.btn_readmode_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 24);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tb_title);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tb_content);
            this.splitContainer2.Size = new System.Drawing.Size(928, 498);
            this.splitContainer2.SplitterDistance = 27;
            this.splitContainer2.TabIndex = 3;
            // 
            // Writer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(928, 522);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Writer";
            this.Text = "Writer";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Writer_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Writer_KeyDown);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Writer_MouseDoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Writer_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Writer_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Writer_MouseUp);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_title;
        private System.Windows.Forms.TextBox tb_content;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createNewNoteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previousNoteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteNoteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem readModeToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer2;
    }
}