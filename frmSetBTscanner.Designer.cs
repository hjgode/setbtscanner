namespace setBTscanner
{
    partial class setBTscanner
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.btnConnect = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.txtBTmacAddress = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(62, 66);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(114, 36);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.AcceptsReturn = true;
            this.textBox1.AcceptsTab = true;
            this.textBox1.Location = new System.Drawing.Point(0, 118);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(239, 147);
            this.textBox1.TabIndex = 1;
            // 
            // txtBTmacAddress
            // 
            this.txtBTmacAddress.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.txtBTmacAddress.Location = new System.Drawing.Point(17, 20);
            this.txtBTmacAddress.MaxLength = 12;
            this.txtBTmacAddress.Name = "txtBTmacAddress";
            this.txtBTmacAddress.Size = new System.Drawing.Size(205, 26);
            this.txtBTmacAddress.TabIndex = 2;
            // 
            // setBTscanner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.txtBTmacAddress);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnConnect);
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "setBTscanner";
            this.Text = "setBTscanner";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox txtBTmacAddress;
    }
}

