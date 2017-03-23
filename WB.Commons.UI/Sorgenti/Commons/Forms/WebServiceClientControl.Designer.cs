namespace WB.Commons.Forms
{
    partial class WebServiceClientControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebServiceClientControl));
            this.label1 = new System.Windows.Forms.Label();
            this.tbWSAddress = new System.Windows.Forms.TextBox();
            this.cbConnect = new System.Windows.Forms.CheckBox();
            this.logTracerControl1 = new WB.Commons.Forms.LogTracerControl();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // tbWSAddress
            // 
            resources.ApplyResources(this.tbWSAddress, "tbWSAddress");
            this.tbWSAddress.Name = "tbWSAddress";
            // 
            // cbConnect
            // 
            resources.ApplyResources(this.cbConnect, "cbConnect");
            this.cbConnect.Name = "cbConnect";
            this.cbConnect.UseVisualStyleBackColor = true;
            // 
            // logTracerControl1
            // 
            resources.ApplyResources(this.logTracerControl1, "logTracerControl1");
            this.logTracerControl1.Name = "logTracerControl1";
            // 
            // WebServiceClientControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbWSAddress);
            this.Controls.Add(this.cbConnect);
            this.Controls.Add(this.logTracerControl1);
            this.Name = "WebServiceClientControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		protected System.Windows.Forms.Label label1;
		protected System.Windows.Forms.TextBox tbWSAddress;
		protected System.Windows.Forms.CheckBox cbConnect;
		protected LogTracerControl logTracerControl1;


	}
}
