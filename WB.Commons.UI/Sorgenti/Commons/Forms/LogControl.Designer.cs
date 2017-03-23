namespace WB.Commons.Forms
{
    partial class LogControl
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
            this.lvLogs = new System.Windows.Forms.ListView();
            this.chTimeStamp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chLevel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chLog = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbWarning = new System.Windows.Forms.CheckBox();
            this.cbError = new System.Windows.Forms.CheckBox();
            this.cbDebug = new System.Windows.Forms.CheckBox();
            this.cbTrace = new System.Windows.Forms.CheckBox();
            this.cbInfo = new System.Windows.Forms.CheckBox();
            this.cboxShowAll = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvLogs
            // 
            this.lvLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvLogs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chTimeStamp,
            this.chLevel,
            this.chLog});
            this.lvLogs.GridLines = true;
            this.lvLogs.Location = new System.Drawing.Point(0, 0);
            this.lvLogs.Name = "lvLogs";
            this.lvLogs.Size = new System.Drawing.Size(752, 256);
            this.lvLogs.TabIndex = 0;
            this.lvLogs.UseCompatibleStateImageBehavior = false;
            this.lvLogs.View = System.Windows.Forms.View.Details;
            // 
            // chTimeStamp
            // 
            this.chTimeStamp.Text = "Time stamp";
            this.chTimeStamp.Width = 151;
            // 
            // chLevel
            // 
            this.chLevel.Text = "Level";
            this.chLevel.Width = 107;
            // 
            // chLog
            // 
            this.chLog.Text = "Log";
            this.chLog.Width = 323;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.cbWarning);
            this.groupBox1.Controls.Add(this.cbError);
            this.groupBox1.Controls.Add(this.cbDebug);
            this.groupBox1.Controls.Add(this.cbTrace);
            this.groupBox1.Controls.Add(this.cbInfo);
            this.groupBox1.Location = new System.Drawing.Point(3, 262);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(292, 45);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Show level";
            // 
            // cbWarning
            // 
            this.cbWarning.AutoSize = true;
            this.cbWarning.Location = new System.Drawing.Point(225, 18);
            this.cbWarning.Name = "cbWarning";
            this.cbWarning.Size = new System.Drawing.Size(66, 17);
            this.cbWarning.TabIndex = 2;
            this.cbWarning.Text = "Warning";
            this.cbWarning.UseVisualStyleBackColor = true;
            // 
            // cbError
            // 
            this.cbError.AutoSize = true;
            this.cbError.Location = new System.Drawing.Point(171, 18);
            this.cbError.Name = "cbError";
            this.cbError.Size = new System.Drawing.Size(48, 17);
            this.cbError.TabIndex = 2;
            this.cbError.Text = "Error";
            this.cbError.UseVisualStyleBackColor = true;
            // 
            // cbDebug
            // 
            this.cbDebug.AutoSize = true;
            this.cbDebug.Location = new System.Drawing.Point(111, 18);
            this.cbDebug.Name = "cbDebug";
            this.cbDebug.Size = new System.Drawing.Size(58, 17);
            this.cbDebug.TabIndex = 2;
            this.cbDebug.Text = "Debug";
            this.cbDebug.UseVisualStyleBackColor = true;
            // 
            // cbTrace
            // 
            this.cbTrace.AutoSize = true;
            this.cbTrace.Location = new System.Drawing.Point(56, 18);
            this.cbTrace.Name = "cbTrace";
            this.cbTrace.Size = new System.Drawing.Size(54, 17);
            this.cbTrace.TabIndex = 2;
            this.cbTrace.Text = "Trace";
            this.cbTrace.UseVisualStyleBackColor = true;
            // 
            // cbInfo
            // 
            this.cbInfo.AutoSize = true;
            this.cbInfo.Location = new System.Drawing.Point(6, 18);
            this.cbInfo.Name = "cbInfo";
            this.cbInfo.Size = new System.Drawing.Size(44, 17);
            this.cbInfo.TabIndex = 2;
            this.cbInfo.Text = "Info";
            this.cbInfo.UseVisualStyleBackColor = true;
            // 
            // cboxShowAll
            // 
            this.cboxShowAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cboxShowAll.AutoSize = true;
            this.cboxShowAll.Location = new System.Drawing.Point(308, 280);
            this.cboxShowAll.Name = "cboxShowAll";
            this.cboxShowAll.Size = new System.Drawing.Size(66, 17);
            this.cboxShowAll.TabIndex = 2;
            this.cboxShowAll.Text = "Show all";
            this.cboxShowAll.UseVisualStyleBackColor = true;
            // 
            // LogControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cboxShowAll);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lvLogs);
            this.Name = "LogControl";
            this.Size = new System.Drawing.Size(755, 307);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvLogs;
        private System.Windows.Forms.ColumnHeader chTimeStamp;
        private System.Windows.Forms.ColumnHeader chLevel;
        private System.Windows.Forms.ColumnHeader chLog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbError;
        private System.Windows.Forms.CheckBox cbTrace;
        private System.Windows.Forms.CheckBox cbInfo;
        private System.Windows.Forms.CheckBox cboxShowAll;
        private System.Windows.Forms.CheckBox cbDebug;
        private System.Windows.Forms.CheckBox cbWarning;
    }
}
