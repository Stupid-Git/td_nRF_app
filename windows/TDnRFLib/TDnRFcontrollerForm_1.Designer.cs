
namespace TDnRFLib //TDnRFcontroller_LIB //namespace rtr500ble_Command_test
{
    partial class TDnRFcontrollerForm_1
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
            this.btnBLE_TestSend = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.tbDeviceAddress = new System.Windows.Forms.TextBox();
            this.btnBLE_ON = new System.Windows.Forms.Button();
            this.btnBLE_OFF = new System.Windows.Forms.Button();
            this.btnBLE_SCAN_start = new System.Windows.Forms.Button();
            this.btnBLE_SCAN_stop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBLE_CONNECT = new System.Windows.Forms.Button();
            this.btnBLE_DISCONNECT = new System.Windows.Forms.Button();
            this.label_NO_BLE = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnBLE_TestSend
            // 
            this.btnBLE_TestSend.Location = new System.Drawing.Point(651, 142);
            this.btnBLE_TestSend.Name = "btnBLE_TestSend";
            this.btnBLE_TestSend.Size = new System.Drawing.Size(76, 26);
            this.btnBLE_TestSend.TabIndex = 41;
            this.btnBLE_TestSend.Text = "Test Send";
            this.btnBLE_TestSend.UseVisualStyleBackColor = true;
            this.btnBLE_TestSend.Visible = false;
            this.btnBLE_TestSend.Click += new System.EventHandler(this.btnBLE_TestSend_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBox1.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.richTextBox1.HideSelection = false;
            this.richTextBox1.Location = new System.Drawing.Point(12, 174);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(720, 266);
            this.richTextBox1.TabIndex = 34;
            this.richTextBox1.Text = "";
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(162, 12);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(357, 156);
            this.listView1.TabIndex = 43;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // tbDeviceAddress
            // 
            this.tbDeviceAddress.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tbDeviceAddress.Location = new System.Drawing.Point(525, 12);
            this.tbDeviceAddress.Name = "tbDeviceAddress";
            this.tbDeviceAddress.Size = new System.Drawing.Size(119, 20);
            this.tbDeviceAddress.TabIndex = 48;
            // 
            // btnBLE_ON
            // 
            this.btnBLE_ON.Location = new System.Drawing.Point(12, 37);
            this.btnBLE_ON.Name = "btnBLE_ON";
            this.btnBLE_ON.Size = new System.Drawing.Size(69, 26);
            this.btnBLE_ON.TabIndex = 50;
            this.btnBLE_ON.Text = "BLE ON";
            this.btnBLE_ON.UseVisualStyleBackColor = true;
            this.btnBLE_ON.Click += new System.EventHandler(this.btnBLE_ON_Click);
            // 
            // btnBLE_OFF
            // 
            this.btnBLE_OFF.Location = new System.Drawing.Point(12, 69);
            this.btnBLE_OFF.Name = "btnBLE_OFF";
            this.btnBLE_OFF.Size = new System.Drawing.Size(69, 26);
            this.btnBLE_OFF.TabIndex = 51;
            this.btnBLE_OFF.Text = "BLE OFF";
            this.btnBLE_OFF.UseVisualStyleBackColor = true;
            this.btnBLE_OFF.Click += new System.EventHandler(this.btnBLE_OFF_Click);
            // 
            // btnBLE_SCAN_start
            // 
            this.btnBLE_SCAN_start.Location = new System.Drawing.Point(87, 37);
            this.btnBLE_SCAN_start.Name = "btnBLE_SCAN_start";
            this.btnBLE_SCAN_start.Size = new System.Drawing.Size(69, 26);
            this.btnBLE_SCAN_start.TabIndex = 52;
            this.btnBLE_SCAN_start.Text = "SCAN";
            this.btnBLE_SCAN_start.UseVisualStyleBackColor = true;
            this.btnBLE_SCAN_start.Click += new System.EventHandler(this.btnBLE_SCAN_start_Click);
            // 
            // btnBLE_SCAN_stop
            // 
            this.btnBLE_SCAN_stop.Location = new System.Drawing.Point(87, 69);
            this.btnBLE_SCAN_stop.Name = "btnBLE_SCAN_stop";
            this.btnBLE_SCAN_stop.Size = new System.Drawing.Size(69, 26);
            this.btnBLE_SCAN_stop.TabIndex = 53;
            this.btnBLE_SCAN_stop.Text = "STOP";
            this.btnBLE_SCAN_stop.UseVisualStyleBackColor = true;
            this.btnBLE_SCAN_stop.Click += new System.EventHandler(this.btnBLE_SCAN_stop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(648, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 54;
            this.label1.Text = "Device Address";
            // 
            // btnBLE_CONNECT
            // 
            this.btnBLE_CONNECT.Location = new System.Drawing.Point(525, 37);
            this.btnBLE_CONNECT.Name = "btnBLE_CONNECT";
            this.btnBLE_CONNECT.Size = new System.Drawing.Size(119, 26);
            this.btnBLE_CONNECT.TabIndex = 55;
            this.btnBLE_CONNECT.Text = "CONNECT";
            this.btnBLE_CONNECT.UseVisualStyleBackColor = true;
            this.btnBLE_CONNECT.Click += new System.EventHandler(this.btnBLE_CONNECT_Click);
            // 
            // btnBLE_DISCONNECT
            // 
            this.btnBLE_DISCONNECT.Location = new System.Drawing.Point(525, 69);
            this.btnBLE_DISCONNECT.Name = "btnBLE_DISCONNECT";
            this.btnBLE_DISCONNECT.Size = new System.Drawing.Size(119, 26);
            this.btnBLE_DISCONNECT.TabIndex = 56;
            this.btnBLE_DISCONNECT.Text = "DISCONNECT";
            this.btnBLE_DISCONNECT.UseVisualStyleBackColor = true;
            this.btnBLE_DISCONNECT.Click += new System.EventHandler(this.btnBLE_DISCONNECT_Click);
            // 
            // label_NO_BLE
            // 
            this.label_NO_BLE.AutoSize = true;
            this.label_NO_BLE.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_NO_BLE.Location = new System.Drawing.Point(21, 15);
            this.label_NO_BLE.Name = "label_NO_BLE";
            this.label_NO_BLE.Size = new System.Drawing.Size(50, 13);
            this.label_NO_BLE.TabIndex = 57;
            this.label_NO_BLE.Text = "NO BLE";
            // 
            // TDnRFcontrollerForm_1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 452);
            this.Controls.Add(this.label_NO_BLE);
            this.Controls.Add(this.btnBLE_DISCONNECT);
            this.Controls.Add(this.btnBLE_CONNECT);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBLE_SCAN_stop);
            this.Controls.Add(this.btnBLE_SCAN_start);
            this.Controls.Add(this.btnBLE_OFF);
            this.Controls.Add(this.btnBLE_ON);
            this.Controls.Add(this.tbDeviceAddress);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.btnBLE_TestSend);
            this.Name = "TDnRFcontrollerForm_1";
            this.Text = "TDnRFcontrollerForm_1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TDnRFcontrollerForm_1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TDnRFcontrollerForm_1_FormClosed);
            this.Shown += new System.EventHandler(this.TDnRFcontrollerForm_1_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBLE_TestSend;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.TextBox tbDeviceAddress;
        private System.Windows.Forms.Button btnBLE_ON;
        private System.Windows.Forms.Button btnBLE_OFF;
        private System.Windows.Forms.Button btnBLE_SCAN_start;
        private System.Windows.Forms.Button btnBLE_SCAN_stop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBLE_CONNECT;
        private System.Windows.Forms.Button btnBLE_DISCONNECT;
        private System.Windows.Forms.Label label_NO_BLE;
    }
}