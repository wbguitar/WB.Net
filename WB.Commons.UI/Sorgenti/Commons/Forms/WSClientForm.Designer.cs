namespace WB.Commons.Forms
{
    partial class WSClientForm
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
            this.cbConnect = new System.Windows.Forms.CheckBox();
            this.tbWSAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.logTracerControl1 = new WB.Commons.Forms.LogTracerControl();
            this.SuspendLayout();
            // 
            // cbConnect
            // 
            this.cbConnect.AutoSize = true;
            this.cbConnect.Location = new System.Drawing.Point(12, 12);
            this.cbConnect.Name = "cbConnect";
            this.cbConnect.Size = new System.Drawing.Size(66, 17);
            this.cbConnect.TabIndex = 1;
            this.cbConnect.Text = "Connect";
            this.cbConnect.UseVisualStyleBackColor = true;
            // 
            // tbWSAddress
            // 
            this.tbWSAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbWSAddress.Location = new System.Drawing.Point(98, 10);
            this.tbWSAddress.Name = "tbWSAddress";
            this.tbWSAddress.Size = new System.Drawing.Size(379, 20);
            this.tbWSAddress.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(483, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "WS address";
            // 
            // logTracerControl1
            // 
            this.logTracerControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logTracerControl1.Location = new System.Drawing.Point(12, 35);
            this.logTracerControl1.Name = "logTracerControl1";
            this.logTracerControl1.Size = new System.Drawing.Size(594, 406);
            this.logTracerControl1.TabIndex = 0;
            // 
            // WSClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 453);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbWSAddress);
            this.Controls.Add(this.cbConnect);
            this.Controls.Add(this.logTracerControl1);
            this.Name = "WSClientForm";
            this.Text = "WSClientForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LogTracerControl logTracerControl1;
        private System.Windows.Forms.CheckBox cbConnect;
        private System.Windows.Forms.TextBox tbWSAddress;
        private System.Windows.Forms.Label label1;
    }
}