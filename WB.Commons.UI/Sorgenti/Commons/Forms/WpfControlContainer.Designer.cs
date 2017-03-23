using System.Windows.Forms.Integration;
using System.Windows.Forms;
namespace WB.Commons.Forms
{
    partial class WpfControlContainer<T>
        where T : System.Windows.UIElement // WPF control
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private ElementHost elementHost = new ElementHost();
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (elementHost != null))
                elementHost.Dispose();

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
            // WpfControlContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "WpfControlContainer";
            this.Size = new System.Drawing.Size(352, 209);

            elementHost = new ElementHost();
            elementHost.Dock = DockStyle.Fill;
            this.Controls.Add(elementHost);

            
            this.ResumeLayout(false);
        }

        #endregion
    }
}
