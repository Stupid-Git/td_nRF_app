namespace nRFUart_TDForms
{
    partial class MainWindow
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnConnect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbDebug = new System.Windows.Forms.CheckBox();
            this.tbInput = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnStartSend100K = new System.Windows.Forms.Button();
            this.btnStopData = new System.Windows.Forms.Button();
            this.btnStartSendFile = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.richTextBox = new System.Windows.Forms.TextBox();
            this.btnStartSend1K = new System.Windows.Forms.Button();
            this.btnNotifyON = new System.Windows.Forms.Button();
            this.btnNotifyOFF = new System.Windows.Forms.Button();
            this.btn01_58 = new System.Windows.Forms.Button();
            this.btn01_F9 = new System.Windows.Forms.Button();
            this.btn01_45 = new System.Windows.Forms.Button();
            this.btn01_44 = new System.Windows.Forms.Button();
            this.btn01_F8 = new System.Windows.Forms.Button();
            this.btn01_F5 = new System.Windows.Forms.Button();
            this.tbBlockNum = new System.Windows.Forms.TextBox();
            this.btnMaikon = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(589, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(36, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(12, 27);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(85, 23);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "Console";
            // 
            // cbDebug
            // 
            this.cbDebug.AutoSize = true;
            this.cbDebug.Location = new System.Drawing.Point(520, 56);
            this.cbDebug.Name = "cbDebug";
            this.cbDebug.Size = new System.Drawing.Size(56, 16);
            this.cbDebug.TabIndex = 4;
            this.cbDebug.Text = "Debug";
            this.cbDebug.UseVisualStyleBackColor = true;
            this.cbDebug.CheckedChanged += new System.EventHandler(this.cbDebug_CheckedChanged);
            // 
            // tbInput
            // 
            this.tbInput.Location = new System.Drawing.Point(12, 211);
            this.tbInput.Name = "tbInput";
            this.tbInput.Size = new System.Drawing.Size(480, 19);
            this.tbInput.TabIndex = 5;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(498, 211);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(76, 19);
            this.btnSend.TabIndex = 7;
            this.btnSend.Text = "Send Text";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnStartSend100K
            // 
            this.btnStartSend100K.Location = new System.Drawing.Point(12, 236);
            this.btnStartSend100K.Name = "btnStartSend100K";
            this.btnStartSend100K.Size = new System.Drawing.Size(130, 19);
            this.btnStartSend100K.TabIndex = 8;
            this.btnStartSend100K.Text = "Send 100kB data";
            this.btnStartSend100K.UseVisualStyleBackColor = true;
            this.btnStartSend100K.Click += new System.EventHandler(this.btnStartSend100K_Click);
            // 
            // btnStopData
            // 
            this.btnStopData.Location = new System.Drawing.Point(304, 236);
            this.btnStopData.Name = "btnStopData";
            this.btnStopData.Size = new System.Drawing.Size(270, 19);
            this.btnStopData.TabIndex = 9;
            this.btnStopData.Text = "Stop Data Transfer";
            this.btnStopData.UseVisualStyleBackColor = true;
            this.btnStopData.Click += new System.EventHandler(this.btnStopData_Click);
            // 
            // btnStartSendFile
            // 
            this.btnStartSendFile.Location = new System.Drawing.Point(14, 261);
            this.btnStartSendFile.Name = "btnStartSendFile";
            this.btnStartSendFile.Size = new System.Drawing.Size(270, 19);
            this.btnStartSendFile.TabIndex = 10;
            this.btnStartSendFile.Text = "Send file";
            this.btnStartSendFile.UseVisualStyleBackColor = true;
            this.btnStartSendFile.Click += new System.EventHandler(this.btnStartSendFile_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(304, 261);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(270, 19);
            this.progressBar.TabIndex = 11;
            // 
            // richTextBox
            // 
            this.richTextBox.Location = new System.Drawing.Point(12, 301);
            this.richTextBox.Multiline = true;
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.richTextBox.Size = new System.Drawing.Size(562, 248);
            this.richTextBox.TabIndex = 12;
            // 
            // btnStartSend1K
            // 
            this.btnStartSend1K.Location = new System.Drawing.Point(154, 236);
            this.btnStartSend1K.Name = "btnStartSend1K";
            this.btnStartSend1K.Size = new System.Drawing.Size(130, 19);
            this.btnStartSend1K.TabIndex = 13;
            this.btnStartSend1K.Text = "Send 1kB data";
            this.btnStartSend1K.UseVisualStyleBackColor = true;
            this.btnStartSend1K.Click += new System.EventHandler(this.btnStartSend1K_Click);
            // 
            // btnNotifyON
            // 
            this.btnNotifyON.Location = new System.Drawing.Point(115, 27);
            this.btnNotifyON.Name = "btnNotifyON";
            this.btnNotifyON.Size = new System.Drawing.Size(85, 23);
            this.btnNotifyON.TabIndex = 14;
            this.btnNotifyON.Text = "Notify ON";
            this.btnNotifyON.UseVisualStyleBackColor = true;
            this.btnNotifyON.Click += new System.EventHandler(this.btnNotifyON_Click);
            // 
            // btnNotifyOFF
            // 
            this.btnNotifyOFF.Location = new System.Drawing.Point(206, 27);
            this.btnNotifyOFF.Name = "btnNotifyOFF";
            this.btnNotifyOFF.Size = new System.Drawing.Size(85, 23);
            this.btnNotifyOFF.TabIndex = 15;
            this.btnNotifyOFF.Text = "Notify OFF";
            this.btnNotifyOFF.UseVisualStyleBackColor = true;
            this.btnNotifyOFF.Click += new System.EventHandler(this.btnNotifyOFF_Click);
            // 
            // btn01_58
            // 
            this.btn01_58.Location = new System.Drawing.Point(115, 66);
            this.btn01_58.Name = "btn01_58";
            this.btn01_58.Size = new System.Drawing.Size(55, 19);
            this.btn01_58.TabIndex = 16;
            this.btn01_58.Text = "01 - 58";
            this.btn01_58.UseVisualStyleBackColor = true;
            this.btn01_58.Click += new System.EventHandler(this.btn01_58_Click);
            // 
            // btn01_F9
            // 
            this.btn01_F9.Location = new System.Drawing.Point(180, 116);
            this.btn01_F9.Name = "btn01_F9";
            this.btn01_F9.Size = new System.Drawing.Size(55, 19);
            this.btn01_F9.TabIndex = 17;
            this.btn01_F9.Text = "01 - F9";
            this.btn01_F9.UseVisualStyleBackColor = true;
            this.btn01_F9.Click += new System.EventHandler(this.btn01_F9_Click);
            // 
            // btn01_45
            // 
            this.btn01_45.Location = new System.Drawing.Point(115, 176);
            this.btn01_45.Name = "btn01_45";
            this.btn01_45.Size = new System.Drawing.Size(55, 19);
            this.btn01_45.TabIndex = 18;
            this.btn01_45.Text = "01 - 45";
            this.btn01_45.UseVisualStyleBackColor = true;
            this.btn01_45.Click += new System.EventHandler(this.btn01_45_Click);
            // 
            // btn01_44
            // 
            this.btn01_44.Location = new System.Drawing.Point(115, 151);
            this.btn01_44.Name = "btn01_44";
            this.btn01_44.Size = new System.Drawing.Size(55, 19);
            this.btn01_44.TabIndex = 19;
            this.btn01_44.Text = "01 - 44";
            this.btn01_44.UseVisualStyleBackColor = true;
            this.btn01_44.Click += new System.EventHandler(this.btn01_44_Click);
            // 
            // btn01_F8
            // 
            this.btn01_F8.Location = new System.Drawing.Point(115, 116);
            this.btn01_F8.Name = "btn01_F8";
            this.btn01_F8.Size = new System.Drawing.Size(55, 19);
            this.btn01_F8.TabIndex = 20;
            this.btn01_F8.Text = "01 - F8";
            this.btn01_F8.UseVisualStyleBackColor = true;
            this.btn01_F8.Click += new System.EventHandler(this.btn01_F8_Click);
            // 
            // btn01_F5
            // 
            this.btn01_F5.Location = new System.Drawing.Point(115, 91);
            this.btn01_F5.Name = "btn01_F5";
            this.btn01_F5.Size = new System.Drawing.Size(55, 19);
            this.btn01_F5.TabIndex = 21;
            this.btn01_F5.Text = "01 - F5";
            this.btn01_F5.UseVisualStyleBackColor = true;
            this.btn01_F5.Click += new System.EventHandler(this.btn01_F5_Click);
            // 
            // tbBlockNum
            // 
            this.tbBlockNum.Location = new System.Drawing.Point(14, 176);
            this.tbBlockNum.Name = "tbBlockNum";
            this.tbBlockNum.Size = new System.Drawing.Size(95, 19);
            this.tbBlockNum.TabIndex = 22;
            // 
            // btnMaikon
            // 
            this.btnMaikon.Location = new System.Drawing.Point(522, 29);
            this.btnMaikon.Name = "btnMaikon";
            this.btnMaikon.Size = new System.Drawing.Size(55, 19);
            this.btnMaikon.TabIndex = 23;
            this.btnMaikon.Text = "Maikon";
            this.btnMaikon.UseVisualStyleBackColor = true;
            this.btnMaikon.Click += new System.EventHandler(this.btnMaikon_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 561);
            this.Controls.Add(this.btnMaikon);
            this.Controls.Add(this.tbBlockNum);
            this.Controls.Add(this.btn01_F5);
            this.Controls.Add(this.btn01_F8);
            this.Controls.Add(this.btn01_44);
            this.Controls.Add(this.btn01_45);
            this.Controls.Add(this.btn01_F9);
            this.Controls.Add(this.btn01_58);
            this.Controls.Add(this.btnNotifyOFF);
            this.Controls.Add(this.btnNotifyON);
            this.Controls.Add(this.btnStartSend1K);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnStartSendFile);
            this.Controls.Add(this.btnStopData);
            this.Controls.Add(this.btnStartSend100K);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.tbInput);
            this.Controls.Add(this.cbDebug);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbDebug;
        private System.Windows.Forms.TextBox tbInput;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnStartSend100K;
        private System.Windows.Forms.Button btnStopData;
        private System.Windows.Forms.Button btnStartSendFile;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox richTextBox;
        private System.Windows.Forms.Button btnStartSend1K;
        private System.Windows.Forms.Button btnNotifyON;
        private System.Windows.Forms.Button btnNotifyOFF;
        private System.Windows.Forms.Button btn01_58;
        private System.Windows.Forms.Button btn01_F9;
        private System.Windows.Forms.Button btn01_45;
        private System.Windows.Forms.Button btn01_44;
        private System.Windows.Forms.Button btn01_F8;
        private System.Windows.Forms.Button btn01_F5;
        private System.Windows.Forms.TextBox tbBlockNum;
        private System.Windows.Forms.Button btnMaikon;
    }
}