namespace WB.Commons.Forms
{
    partial class LogTracerControl
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
			this.tbTrace = new System.Windows.Forms.TextBox();
			this.lbLogLevels = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnAll = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// tbTrace
			// 
			this.tbTrace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbTrace.Location = new System.Drawing.Point(3, 3);
			this.tbTrace.Multiline = true;
			this.tbTrace.Name = "tbTrace";
			this.tbTrace.ReadOnly = true;
			this.tbTrace.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbTrace.Size = new System.Drawing.Size(333, 304);
			this.tbTrace.TabIndex = 5;
			// 
			// lbLogLevels
			// 
			this.lbLogLevels.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lbLogLevels.FormattingEnabled = true;
			this.lbLogLevels.Location = new System.Drawing.Point(342, 22);
			this.lbLogLevels.Name = "lbLogLevels";
			this.lbLogLevels.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lbLogLevels.Size = new System.Drawing.Size(119, 238);
			this.lbLogLevels.TabIndex = 6;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(342, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(55, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "Log levels";
			// 
			// btnAll
			// 
			this.btnAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAll.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnAll.Location = new System.Drawing.Point(342, 282);
			this.btnAll.Name = "btnAll";
			this.btnAll.Size = new System.Drawing.Size(119, 25);
			this.btnAll.TabIndex = 9;
			this.btnAll.Text = "Check all";
			this.btnAll.UseVisualStyleBackColor = true;
			// 
			// LogTracerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnAll);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lbLogLevels);
			this.Controls.Add(this.tbTrace);
			this.Name = "LogTracerControl";
			this.Size = new System.Drawing.Size(464, 310);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		protected System.Windows.Forms.TextBox tbTrace;
		protected System.Windows.Forms.ListBox lbLogLevels;
		protected System.Windows.Forms.Label label1;
		protected System.Windows.Forms.Button btnAll;

	}
}
