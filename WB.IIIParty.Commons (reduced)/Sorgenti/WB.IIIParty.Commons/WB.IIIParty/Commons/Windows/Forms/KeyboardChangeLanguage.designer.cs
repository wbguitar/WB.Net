namespace WB.IIIParty.Commons.Windows.Forms
{
    partial class KeyboardChangeLanguage
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeyboardChangeLanguage));
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItemLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.timerLanguage = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.BackColor = System.Drawing.Color.Gainsboro;
            this.toolStripMenuItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripMenuItem1.ForeColor = System.Drawing.Color.White;
            this.toolStripMenuItem1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(150, 20);
            this.toolStripMenuItem1.Text = "toolStripMenuItem1";
            this.toolStripMenuItem1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // menuStrip1
            // 
            this.menuStrip1.AutoSize = false;
            this.menuStrip1.BackColor = System.Drawing.Color.Black;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.ForeColor = System.Drawing.Color.White;
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(0);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemLanguage});
            this.menuStrip1.Location = new System.Drawing.Point(3, 3);
            this.menuStrip1.MaximumSize = new System.Drawing.Size(50, 20);
            this.menuStrip1.MinimumSize = new System.Drawing.Size(50, 20);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.menuStrip1.Size = new System.Drawing.Size(50, 20);
            this.menuStrip1.Stretch = false;
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItemLanguage
            // 
            this.toolStripMenuItemLanguage.AutoSize = false;
            this.toolStripMenuItemLanguage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.toolStripMenuItemLanguage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStripMenuItemLanguage.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripMenuItemLanguage.ForeColor = System.Drawing.Color.Black;
            this.toolStripMenuItemLanguage.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemLanguage.Image")));
            this.toolStripMenuItemLanguage.Name = "toolStripMenuItemLanguage";
            this.toolStripMenuItemLanguage.RightToLeftAutoMirrorImage = true;
            this.toolStripMenuItemLanguage.Size = new System.Drawing.Size(50, 20);
            this.toolStripMenuItemLanguage.Text = "EN";
            this.toolStripMenuItemLanguage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // timerLanguage
            // 
            this.timerLanguage.Enabled = true;
            this.timerLanguage.Interval = 1000;
            this.timerLanguage.Tick += new System.EventHandler(this.timerLanguage_Tick);
            // 
            // KeyboardChangeLanguage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.menuStrip1);
            this.MaximumSize = new System.Drawing.Size(56, 26);
            this.MinimumSize = new System.Drawing.Size(56, 26);
            this.Name = "KeyboardChangeLanguage";
            this.Size = new System.Drawing.Size(56, 26);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemLanguage;
        private System.Windows.Forms.Timer timerLanguage;
    }
}
