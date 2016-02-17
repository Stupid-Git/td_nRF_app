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
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnConnect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbDebug = new System.Windows.Forms.CheckBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.richTextBox = new System.Windows.Forms.TextBox();
            this.btn01_58 = new System.Windows.Forms.Button();
            this.btn01_F9 = new System.Windows.Forms.Button();
            this.btn01_45 = new System.Windows.Forms.Button();
            this.btn01_44 = new System.Windows.Forms.Button();
            this.btn01_F8 = new System.Windows.Forms.Button();
            this.btn01_F5 = new System.Windows.Forms.Button();
            this.tbBlockNum = new System.Windows.Forms.TextBox();
            this.btnMaikon = new System.Windows.Forms.Button();
            this.btnTEST_BUTTON = new System.Windows.Forms.Button();
            this.btn_CMD12 = new System.Windows.Forms.Button();
            this.btn_CMD13 = new System.Windows.Forms.Button();
            this.btnT2_RUINF = new System.Windows.Forms.Button();
            this.btnSunaba = new System.Windows.Forms.Button();
            this.btnSerLow_Create = new System.Windows.Forms.Button();
            this.btnSerLow_Close = new System.Windows.Forms.Button();
            this.btnSerLow_Open = new System.Windows.Forms.Button();
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
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(36, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.Items.AddRange(new object[] {
            "nRF Board",
            "Real BLE",
            "Real Serial",
            "Real BLE and Serial"});
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 20);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.exitToolStripMenuItem.Text = "Exit";
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
            // btnTEST_BUTTON
            // 
            this.btnTEST_BUTTON.Location = new System.Drawing.Point(342, 130);
            this.btnTEST_BUTTON.Name = "btnTEST_BUTTON";
            this.btnTEST_BUTTON.Size = new System.Drawing.Size(75, 23);
            this.btnTEST_BUTTON.TabIndex = 24;
            this.btnTEST_BUTTON.Text = "Test Button";
            this.btnTEST_BUTTON.UseVisualStyleBackColor = true;
            this.btnTEST_BUTTON.Click += new System.EventHandler(this.btnTEST_BUTTON_Click);
            // 
            // btn_CMD12
            // 
            this.btn_CMD12.Location = new System.Drawing.Point(317, 29);
            this.btn_CMD12.Name = "btn_CMD12";
            this.btn_CMD12.Size = new System.Drawing.Size(65, 19);
            this.btn_CMD12.TabIndex = 53;
            this.btn_CMD12.Text = "CMD_12";
            this.btn_CMD12.UseVisualStyleBackColor = true;
            this.btn_CMD12.Click += new System.EventHandler(this.btn_CMD12_Click);
            // 
            // btn_CMD13
            // 
            this.btn_CMD13.Location = new System.Drawing.Point(397, 29);
            this.btn_CMD13.Name = "btn_CMD13";
            this.btn_CMD13.Size = new System.Drawing.Size(65, 19);
            this.btn_CMD13.TabIndex = 50;
            this.btn_CMD13.Text = "CMD_13";
            this.btn_CMD13.UseVisualStyleBackColor = true;
            this.btn_CMD13.Click += new System.EventHandler(this.btn_CMD13_Click);
            // 
            // btnT2_RUINF
            // 
            this.btnT2_RUINF.Location = new System.Drawing.Point(317, 66);
            this.btnT2_RUINF.Name = "btnT2_RUINF";
            this.btnT2_RUINF.Size = new System.Drawing.Size(100, 19);
            this.btnT2_RUINF.TabIndex = 54;
            this.btnT2_RUINF.Text = "T2_RUINF";
            this.btnT2_RUINF.UseVisualStyleBackColor = true;
            this.btnT2_RUINF.Click += new System.EventHandler(this.btnT2_RUINF_Click);
            // 
            // btnSunaba
            // 
            this.btnSunaba.Location = new System.Drawing.Point(522, 91);
            this.btnSunaba.Name = "btnSunaba";
            this.btnSunaba.Size = new System.Drawing.Size(55, 19);
            this.btnSunaba.TabIndex = 55;
            this.btnSunaba.Text = "Sunaba";
            this.btnSunaba.UseVisualStyleBackColor = true;
            this.btnSunaba.Click += new System.EventHandler(this.btnSunaba_Click);
            // 
            // btnSerLow_Create
            // 
            this.btnSerLow_Create.Location = new System.Drawing.Point(506, 126);
            this.btnSerLow_Create.Name = "btnSerLow_Create";
            this.btnSerLow_Create.Size = new System.Drawing.Size(71, 19);
            this.btnSerLow_Create.TabIndex = 56;
            this.btnSerLow_Create.Text = "SL_Create";
            this.btnSerLow_Create.UseVisualStyleBackColor = true;
            this.btnSerLow_Create.Click += new System.EventHandler(this.btnSerLow_Create_Click);
            // 
            // btnSerLow_Close
            // 
            this.btnSerLow_Close.Location = new System.Drawing.Point(506, 176);
            this.btnSerLow_Close.Name = "btnSerLow_Close";
            this.btnSerLow_Close.Size = new System.Drawing.Size(71, 19);
            this.btnSerLow_Close.TabIndex = 57;
            this.btnSerLow_Close.Text = "SL_Close";
            this.btnSerLow_Close.UseVisualStyleBackColor = true;
            this.btnSerLow_Close.Click += new System.EventHandler(this.btnSerLow_Close_Click);
            // 
            // btnSerLow_Open
            // 
            this.btnSerLow_Open.Location = new System.Drawing.Point(506, 151);
            this.btnSerLow_Open.Name = "btnSerLow_Open";
            this.btnSerLow_Open.Size = new System.Drawing.Size(71, 19);
            this.btnSerLow_Open.TabIndex = 58;
            this.btnSerLow_Open.Text = "SL_Open";
            this.btnSerLow_Open.UseVisualStyleBackColor = true;
            this.btnSerLow_Open.Click += new System.EventHandler(this.btnSerLow_Open_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 561);
            this.Controls.Add(this.btnSerLow_Open);
            this.Controls.Add(this.btnSerLow_Close);
            this.Controls.Add(this.btnSerLow_Create);
            this.Controls.Add(this.btnSunaba);
            this.Controls.Add(this.btnT2_RUINF);
            this.Controls.Add(this.btn_CMD12);
            this.Controls.Add(this.btn_CMD13);
            this.Controls.Add(this.btnTEST_BUTTON);
            this.Controls.Add(this.btnMaikon);
            this.Controls.Add(this.tbBlockNum);
            this.Controls.Add(this.btn01_F5);
            this.Controls.Add(this.btn01_F8);
            this.Controls.Add(this.btn01_44);
            this.Controls.Add(this.btn01_45);
            this.Controls.Add(this.btn01_F9);
            this.Controls.Add(this.btn01_58);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.cbDebug);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.Load += new System.EventHandler(this.MainWindow_Load);
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
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox richTextBox;
        private System.Windows.Forms.Button btn01_58;
        private System.Windows.Forms.Button btn01_F9;
        private System.Windows.Forms.Button btn01_45;
        private System.Windows.Forms.Button btn01_44;
        private System.Windows.Forms.Button btn01_F8;
        private System.Windows.Forms.Button btn01_F5;
        private System.Windows.Forms.TextBox tbBlockNum;
        private System.Windows.Forms.Button btnMaikon;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button btnTEST_BUTTON;
        private System.Windows.Forms.Button btn_CMD12;
        private System.Windows.Forms.Button btn_CMD13;
        private System.Windows.Forms.Button btnT2_RUINF;
        private System.Windows.Forms.Button btnSunaba;
        private System.Windows.Forms.Button btnSerLow_Create;
        private System.Windows.Forms.Button btnSerLow_Close;
        private System.Windows.Forms.Button btnSerLow_Open;
    }
}