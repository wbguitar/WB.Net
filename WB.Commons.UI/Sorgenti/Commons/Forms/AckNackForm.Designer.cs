namespace WB.Commons.Forms
{
    partial class AckNackForm
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
            this.btnAck = new System.Windows.Forms.Button();
            this.btnNack = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnAck
            // 
            this.btnAck.Location = new System.Drawing.Point(13, 11);
            this.btnAck.Name = "btnAck";
            this.btnAck.Size = new System.Drawing.Size(75, 23);
            this.btnAck.TabIndex = 0;
            this.btnAck.Text = "Ack";
            this.btnAck.UseVisualStyleBackColor = true;
            // 
            // btnNack
            // 
            this.btnNack.Location = new System.Drawing.Point(94, 11);
            this.btnNack.Name = "btnNack";
            this.btnNack.Size = new System.Drawing.Size(75, 23);
            this.btnNack.TabIndex = 0;
            this.btnNack.Text = "Nack";
            this.btnNack.UseVisualStyleBackColor = true;
            // 
            // AckNackForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(183, 45);
            this.Controls.Add(this.btnNack);
            this.Controls.Add(this.btnAck);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AckNackForm";
            this.Text = "AckNackForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAck;
        private System.Windows.Forms.Button btnNack;
    }
}