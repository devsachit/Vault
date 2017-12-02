namespace Vault
{
    partial class NodeTree
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // NodeTree
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.CausesValidation = false;
            this.DoubleBuffered = true;
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.Name = "NodeTree";
            this.Size = new System.Drawing.Size(220, 743);
            this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.NodeTree_Scroll);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.NodeTree_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NodeTree_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NodeTree_KeyUp);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NodeTree_MouseDoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.NodeTree_MouseDown);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
