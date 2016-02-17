namespace TDnRFLib_Test1
{
    partial class Form1
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
            this.btnOpenBLE = new System.Windows.Forms.Button();
            this.btnInitBLE = new System.Windows.Forms.Button();
            this.btnCloseBLE = new System.Windows.Forms.Button();
            this.btnStartScanBLE = new System.Windows.Forms.Button();
            this.btnStopScanBLE = new System.Windows.Forms.Button();
            this.btnConnectBLE = new System.Windows.Forms.Button();
            this.btnDisconnectBLE = new System.Windows.Forms.Button();
            this.btnTestSend = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnOpenBLE
            // 
            this.btnOpenBLE.Location = new System.Drawing.Point(12, 62);
            this.btnOpenBLE.Name = "btnOpenBLE";
            this.btnOpenBLE.Size = new System.Drawing.Size(100, 22);
            this.btnOpenBLE.TabIndex = 0;
            this.btnOpenBLE.Text = "Open BLE";
            this.btnOpenBLE.UseVisualStyleBackColor = true;
            this.btnOpenBLE.Click += new System.EventHandler(this.btnOpenBLE_Click);
            // 
            // btnInitBLE
            // 
            this.btnInitBLE.Location = new System.Drawing.Point(12, 12);
            this.btnInitBLE.Name = "btnInitBLE";
            this.btnInitBLE.Size = new System.Drawing.Size(100, 22);
            this.btnInitBLE.TabIndex = 1;
            this.btnInitBLE.Text = "Init BLE";
            this.btnInitBLE.UseVisualStyleBackColor = true;
            this.btnInitBLE.Click += new System.EventHandler(this.btnInitBLE_Click);
            // 
            // btnCloseBLE
            // 
            this.btnCloseBLE.Location = new System.Drawing.Point(12, 90);
            this.btnCloseBLE.Name = "btnCloseBLE";
            this.btnCloseBLE.Size = new System.Drawing.Size(100, 22);
            this.btnCloseBLE.TabIndex = 2;
            this.btnCloseBLE.Text = "Close BLE";
            this.btnCloseBLE.UseVisualStyleBackColor = true;
            this.btnCloseBLE.Click += new System.EventHandler(this.btnCloseBLE_Click);
            // 
            // btnStartScanBLE
            // 
            this.btnStartScanBLE.Location = new System.Drawing.Point(12, 144);
            this.btnStartScanBLE.Name = "btnStartScanBLE";
            this.btnStartScanBLE.Size = new System.Drawing.Size(100, 22);
            this.btnStartScanBLE.TabIndex = 3;
            this.btnStartScanBLE.Text = "StartScan BLE";
            this.btnStartScanBLE.UseVisualStyleBackColor = true;
            this.btnStartScanBLE.Click += new System.EventHandler(this.btnStartScanBLE_Click);
            // 
            // btnStopScanBLE
            // 
            this.btnStopScanBLE.Location = new System.Drawing.Point(12, 172);
            this.btnStopScanBLE.Name = "btnStopScanBLE";
            this.btnStopScanBLE.Size = new System.Drawing.Size(100, 22);
            this.btnStopScanBLE.TabIndex = 4;
            this.btnStopScanBLE.Text = "StopScan BLE";
            this.btnStopScanBLE.UseVisualStyleBackColor = true;
            this.btnStopScanBLE.Click += new System.EventHandler(this.btnStopScanBLE_Click);
            // 
            // btnConnectBLE
            // 
            this.btnConnectBLE.Location = new System.Drawing.Point(12, 244);
            this.btnConnectBLE.Name = "btnConnectBLE";
            this.btnConnectBLE.Size = new System.Drawing.Size(100, 22);
            this.btnConnectBLE.TabIndex = 5;
            this.btnConnectBLE.Text = "Connect BLE";
            this.btnConnectBLE.UseVisualStyleBackColor = true;
            this.btnConnectBLE.Click += new System.EventHandler(this.btnConnectBLE_Click);
            // 
            // btnDisconnectBLE
            // 
            this.btnDisconnectBLE.Location = new System.Drawing.Point(12, 272);
            this.btnDisconnectBLE.Name = "btnDisconnectBLE";
            this.btnDisconnectBLE.Size = new System.Drawing.Size(100, 22);
            this.btnDisconnectBLE.TabIndex = 6;
            this.btnDisconnectBLE.Text = "Disconnect BLE";
            this.btnDisconnectBLE.UseVisualStyleBackColor = true;
            this.btnDisconnectBLE.Click += new System.EventHandler(this.btnDisconnectBLE_Click);
            // 
            // btnTestSend
            // 
            this.btnTestSend.Location = new System.Drawing.Point(12, 345);
            this.btnTestSend.Name = "btnTestSend";
            this.btnTestSend.Size = new System.Drawing.Size(100, 22);
            this.btnTestSend.TabIndex = 7;
            this.btnTestSend.Text = "Test Send";
            this.btnTestSend.UseVisualStyleBackColor = true;
            this.btnTestSend.Click += new System.EventHandler(this.btnTestSend_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBox1.Location = new System.Drawing.Point(175, 144);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(549, 375);
            this.richTextBox1.TabIndex = 10;
            this.richTextBox1.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 531);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.btnTestSend);
            this.Controls.Add(this.btnDisconnectBLE);
            this.Controls.Add(this.btnConnectBLE);
            this.Controls.Add(this.btnStopScanBLE);
            this.Controls.Add(this.btnStartScanBLE);
            this.Controls.Add(this.btnCloseBLE);
            this.Controls.Add(this.btnInitBLE);
            this.Controls.Add(this.btnOpenBLE);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOpenBLE;
        private System.Windows.Forms.Button btnInitBLE;
        private System.Windows.Forms.Button btnCloseBLE;
        private System.Windows.Forms.Button btnStartScanBLE;
        private System.Windows.Forms.Button btnStopScanBLE;
        private System.Windows.Forms.Button btnConnectBLE;
        private System.Windows.Forms.Button btnDisconnectBLE;
        private System.Windows.Forms.Button btnTestSend;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}

