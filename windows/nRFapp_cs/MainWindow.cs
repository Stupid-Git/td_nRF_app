using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//using System;
using System.IO;
using System.Windows;
//using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using System.Collections;
//using System.Collections.Generic;
using System.Collections.ObjectModel;

using nRFUart_TD;


using Maikon_ns;//using MasterEm_TD2_a;
using Maikon_lib;

namespace nRFUart_TDForms
{
    public partial class MainWindow : Form
    {
        public nRF_TD_Controller controller;
        bool isControllerInitialized = false;
        bool isControllerConnected = false;

        const string strConnect = "Connect";
        const string strScanning = "Stop scanning";
        const string strDisconnect = "Disconnect";
        const string strStopSendData = "Stop sending data";
        const string strStartSendData = "Send 100kB data";

        const UInt32 logHighWatermark = 10000;  // If we reach high watermark, we delete until we're
        // down to low watermark
        const UInt32 logLowWatermark = 5000;

        private ObservableCollection<String> _outputText = null;
        public ObservableCollection<string> OutputText
        {
            get { return _outputText ?? (_outputText = new ObservableCollection<string>()); }
            set { _outputText = value; }
        }


        public MainWindow()
        {
            InitializeComponent();
            InitializeNrfUartController();

            /* Retrieve persisted setting. */
            cbDebug.Checked/*.IsChecked*/ = Properties.Settings.Default.IsDebugEnabled;
            //TBR DataContext = this;
        }

        /*TODO
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            btnConnect.IsEnabled = false;
            Mouse.OverrideCursor = Cursors.Wait;
        }
        TODO*/

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                /* Persist user settings before the applications closes. */
                Properties.Settings.Default.Save();

                controller.Close();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }

            base.OnClosing(e);
        }

        void InitializeNrfUartController()
        {
            controller = new nRF_TD_Controller();

            /* Registering event handler methods for all nRF_TD_Controller events. */
            controller.LogMessage += OnLogMessage;
            controller.Initialized += OnControllerInitialized;
            controller.Scanning += OnScanning;
            controller.ScanningCanceled += OnScanningCanceled;
            controller.Connecting += OnConnecting;
            controller.ConnectionCanceled += OnConnectionCanceled;
            controller.Connected += OnConnected;
            controller.PipeDiscoveryCompleted += OnControllerPipeDiscoveryCompleted;
            controller.Disconnected += OnDisconnected;
            controller.SendDataStarted += OnSendDataStarted;
            controller.SendDataCompleted += OnSendDataCompleted;
            controller.ProgressUpdated += OnProgressUpdated;

            controller.Initialize();
        }

        void SetConnectButtonText(string text)
        {
            SetButtonText(btnConnect, text);
        }

        //se also Control::BeginInvoke Method 
        //https://msdn.microsoft.com/ja-jp/library/a06c0dc2%28v=vs.110%29.aspx?cs-save-lang=1&cs-lang=cpp#code-snippet-1

        void SetButtonText(Button button, string text)
        {
            /* Requesting GUI update to be done in main thread since this 
             * method will be called from a different thread. */
            Console.WriteLine("##TODO## Button Text = {0}", text);
            /*TODO
            Dispatcher.BeginInvoke((Action)delegate()
            {
                button.Content = text;
            });
            TODO*/
            if (button.InvokeRequired)
                //Invoke(new Action<Button,bool>(SetButtonIsEnabled),button, isEnabled);
                Invoke((MethodInvoker)(() => SetButtonText(button, text)));
            else
                button.Text = text;

        }

        void SetStartSendIsEnabled(bool isEnabled)
        {
            SetButtonIsEnabled(btnStartSend100K, isEnabled);
        }

        void SetStartSendFileIsEnabled(bool isEnabled)
        {
            SetButtonIsEnabled(btnStartSendFile, isEnabled);
        }

        void SetStopDataIsEnabled(bool isEnabled)
        {
            SetButtonIsEnabled(btnStopData, isEnabled);
        }

        /*REF
        public void UpdateMyTextBox(string NewText)
        {
            if (InvokeRequired)
                Invoke(new Action<string>(UpdateMyTextBox), NewText);
            else
                myTextBox.Text = NewText;
        }
        */

        void SetButtonIsEnabled(Button button, bool isEnabled)
        {
            /* Requesting GUI update to be done in main thread since this 
             * method will be called from a different thread. */

            Console.WriteLine("##TODO## Button Enabled = {0}", isEnabled);
            /*TODO
            Dispatcher.BeginInvoke((Action)delegate()
            {
                button.IsEnabled = isEnabled;
            });
            TODO*/
            if (button.InvokeRequired)
                //Invoke(new Action<Button,bool>(SetButtonIsEnabled),button, isEnabled);
                Invoke((MethodInvoker)(() => SetButtonIsEnabled(button, isEnabled)));
            else
                button.Enabled = isEnabled;
        }

        void SetProgressBarValue(int newValue)
        {
            /* Requesting GUI update to be done in main thread since this 
             * method will be called from a different thread. */

            Console.WriteLine("##TODO## progressBar = {0}", newValue);
            /*TODO
            Dispatcher.BeginInvoke((Action)delegate()
            {
                progressBar.Value = newValue;
            });
            TODO*/
            if (progressBar.InvokeRequired)
                //Invoke(new Action<Button,bool>(SetButtonIsEnabled),button, isEnabled);
                Invoke((MethodInvoker)(() => SetProgressBarValue(newValue)));
            else
                progressBar.Value = newValue;
        }

        void AddToOutputXXX(string text)
        {
            /* Need to call Invoke since method will be called from a background thread. */

            Console.WriteLine("##TODO## lbOutput = {0}", text);
            /*TODO
            Dispatcher.BeginInvoke((Action)delegate()
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss.ffff");
                text = String.Format("[{0}] {1}", timestamp, text);

                if (OutputText.Count >= logHighWatermark)
                {
                    UInt32 numToDelete = (UInt32)OutputText.Count - logLowWatermark;
                    for (UInt32 i = 0; i < numToDelete; i++)
                    {
                        OutputText.RemoveAt(0);
                    }
                }

                OutputText.Add(text);
                lbOutput.ScrollIntoView(text);
            });
            TODO*/
            if (richTextBox.InvokeRequired)
                Invoke((MethodInvoker)(() => AddToOutput(text)));
            else
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss.ffff");
                text = String.Format("[{0}] {1}", timestamp, text);

                /*TODO
                if (OutputText.Count >= logHighWatermark)
                {
                    UInt32 numToDelete = (UInt32)OutputText.Count - logLowWatermark;
                    for (UInt32 i = 0; i < numToDelete; i++)
                    {
                        OutputText.RemoveAt(0);
                    }
                }

                OutputText.Add(text);
                
                lbOutput.ScrollIntoView(text);
                TODO*/
                richTextBox.AppendText(text);
                richTextBox.SelectionStart = richTextBox.Text.Length;
                richTextBox.ScrollToCaret();
                
            }
        }

        delegate void SetTextCallback(string text);
        private void AddToOutput(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.richTextBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(AddToOutput);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.richTextBox.Text += text + "\r\n";
                //this.textBox.AppendText(text);
                richTextBox.SelectionStart = richTextBox.Text.Length;
                richTextBox.ScrollToCaret();
            }
        }


        #region nRFUart event handlers
        void OnControllerInitialized(object sender, EventArgs e)
        {
            isControllerInitialized = true;


            Console.WriteLine("##TODO## btnConnect.IsEnabled = true");
            /*TODO
            Dispatcher.BeginInvoke((Action)delegate()
            {
                btnConnect.IsEnabled = true;
                Mouse.OverrideCursor = null;
            });
            TODO*/
            if (InvokeRequired)
                Invoke((MethodInvoker)(() => OnControllerInitialized(sender, e)));
            else
            {
                btnConnect.Enabled = true;
                /*TODO
                Mouse.OverrideCursor = null;
                TODO*/
            }
            AddToOutput("Ready to connect");
        }

        void OnLogMessage(object sender, OutputReceivedEventArgs e)
        {
            AddToOutput(e.Message);
        }

        void OnScanning(object sender, EventArgs e)
        {
            AddToOutput("Scanning...");
            SetConnectButtonText(strScanning);
        }

        void OnScanningCanceled(object sender, EventArgs e)
        {
            AddToOutput("Stopped scanning");
            SetConnectButtonText(strConnect);
        }

        void OnConnectionCanceled(object sender, EventArgs e)
        {
            SetConnectButtonText(strConnect);
        }

        void OnConnecting(object sender, EventArgs e)
        {
            AddToOutput("Connecting...");
        }

        void OnConnected(object sender, EventArgs e)
        {
            isControllerConnected = true;
            SetConnectButtonText(strDisconnect);
        }

        void OnControllerPipeDiscoveryCompleted(object sender, EventArgs e)
        {
            AddToOutput("Ready to send");
        }

        void OnSendDataStarted(object sender, EventArgs e)
        {
            AddToOutput("Started sending data...");
            SetStopDataIsEnabled(true);
            SetStartSendIsEnabled(false);
            SetStartSendFileIsEnabled(false);
        }

        void OnSendDataCompleted(object sender, EventArgs e)
        {
            AddToOutput("Data transfer ended");
            SetStopDataIsEnabled(false);
            SetStartSendIsEnabled(true);
            SetStartSendFileIsEnabled(true);
            SetProgressBarValue(0);
        }

        void OnDisconnected(object sender, EventArgs e)
        {
            isControllerConnected = false;
            AddToOutput("Disconnected");
            SetConnectButtonText(strConnect);
            SetStopDataIsEnabled(false);
            SetStartSendIsEnabled(true);
            SetStartSendFileIsEnabled(true);
        }

        void OnProgressUpdated(object sender, Nordicsemi.ValueEventArgs<int> e)
        {
            int progress = e.Value;
            if (0 <= progress && progress <= 100)
            {
                SetProgressBarValue(progress);
            }
        }
        #endregion

        #region GUI event handlers
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!isControllerInitialized)
            {
                return;
            }

            if (btnConnect.Text == strConnect)
            {
                controller.InitiateConnection();
            }
            else if (btnConnect.Text == strScanning)
            {
                controller.StopScanning();
            }
            else if (btnConnect.Text == strDisconnect)
            {
                controller.InitiateDisconnect();
            }
        }

        private void btnNotifyON_Click(object sender, EventArgs e)
        {
            controller.EnableNotify_Dcfm();
        }

        private void btnNotifyOFF_Click(object sender, EventArgs e)
        {

        }




        //=====================================================================
        //=====================================================================
        //===== UART ==========================================================
        //=====================================================================
        //=====================================================================
        private void btnSend_Click(object sender, EventArgs e)
        {
            /* defunct
            if (!isControllerConnected)
            {
                return;
            }
            controller.UART_SendData(tbInput.Text);
            */
        }

        /// <summary>
        /// Adds ability to initiate send by hitting enter key when textbox has focus.
        /// </summary>
        void OnTbInputKeyDown(object sender, KeyEventArgs e)
        {
            /*TODO
            if (e.Key != Key.Enter)
            {
                return;
            }

            if (!isControllerConnected)
            {
                return;
            }

            controller.SendData(tbInput.Text);
            TODO*/
        }

        private void cbDebug_CheckedChanged(object sender, EventArgs e)
        {
            /* Store the state of the checkbox in application settings. */
            Properties.Settings.Default.IsDebugEnabled = (bool)cbDebug.Checked;
            controller.DebugMessagesEnabled = (bool)cbDebug.Checked;
        }


        void OnMenuItemLogfileClick(object sender, EventArgs e)
        {
            string logfilePath = controller.GetLogfilePath();
            Process.Start(logfilePath);
        }

        void OnMenuItemExitClick(object sender, EventArgs e)
        {
            Close();
        }

        void OnMenuItemAboutClick(object sender, EventArgs e)
        {
            /*TODO
            About aboutDialog = new About();
            aboutDialog.Owner = this;
            aboutDialog.ShowDialog();
            TODO*/
        }

        private void btnStartSendFile_Click(object sender, EventArgs e)
        {
            /*TODO
            string sendFilePath = String.Empty;

            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = "File";
            ofd.DefaultExt = "*.*";
            ofd.Filter = "All files (*.*)|*.*";
            ofd.FilterIndex = 0;
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

            ofd.Title = "Please select a file to send";

            bool? ofdResult = ofd.ShowDialog();

            if (ofdResult == false) //Failure
            {
                return;
            }

            SendFile(ofd.FileName);
            TODO*/
        }
        void SendFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            filePath = filePath.Replace("\\", "/");


            byte[] fileContent = File.ReadAllBytes(filePath);
            //controller.UARTStartSendData(fileContent);
        }

        private void btnStartSend100K_Click(object sender, EventArgs e)
        {
            Send100K();
        }
        private void btnStartSend1K_Click(object sender, EventArgs e)
        {
            Send1K();
        }

        void Send100K()
        {
            /* Instantiate byte array with 18 bytes of data. */
            byte[] data = new byte[] { 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A,
            0x6B, 0x6C, 0x6D, 0x6E, 0x6F, 0x70, 0x71, 0x72};

            /* Calculate number of packets required to send 100kB of data. */
            int maxBytesPerPacket = 18;
            int kibiBytes = 1024;
            int numberOfRepetitions = (100 * kibiBytes) / maxBytesPerPacket; /* 5120 packets */

            //controller.StartSendData(data, numberOfRepetitions);
        }

        void Send1K()
        {
            /* Instantiate byte array with 18 bytes of data. */
            byte[] data = new byte[] { 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A,
            0x6B, 0x6C, 0x6D, 0x6E, 0x6F, 0x70, 0x71, 0x72};

            /* Calculate number of packets required to send 100kB of data. */
            int maxBytesPerPacket = 18;
            int kibiBytes = 1024;
            int numberOfRepetitions = (1 * kibiBytes) / maxBytesPerPacket; /* 5120 packets */

            //controller.StartSendData(data, numberOfRepetitions);
        }

        //void OnBtnStopData(object sender, EventArgs e)
        private void btnStopData_Click(object sender, EventArgs e)
        {
            AddToOutput("Stop transfer");
            controller.UARTStopSendData();

        }

        #endregion


        
#if false

        //=====================================================================
        //=====================================================================
        //===== CMD ===========================================================
        //=====================================================================
        //=====================================================================
        System::Void btn_CMD13_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;
            controller.udEngine.Dn_Send_CMD_13();
        }


        //=====================================================================
        //=====================================================================
        //===== CMD testing ===================================================
        //=====================================================================
        //=====================================================================
        System::Void btnSendXX1_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;
            controller.udEngine.Dn_Send_T2_DummyTEST1();
            //controller.udEngine.Dn_Send_CMD_12();
        }

        System::Void btnSendXX4_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;
            //controller.udEngine.Dn_Dummy_Send42();
            //controller.Send_Dcmd(tbInput.Text);
        }

        System::Void btnSendXX5_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;
            //controller.Send_Ddat(tbInput.Text);
        }

        System::Void btnSendXX6_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;
            //controller.udEngine.Dn_Dummy_Send43();
        }


        //=====================================================================
        //=====================================================================
        //===== CMD T2 ========================================================
        //=====================================================================
        //=====================================================================
        System::Void btnT2_Test_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;
            controller.udEngine.Dn_Send_T2_DummyTEST1();
        }
#endif
        //=====================================================================
        //=====================================================================
        //===== CMD 0x01 ======================================================
        //=====================================================================
        //=====================================================================
        private void btn01_58_Click(object sender, EventArgs e)
        {
            if (!isControllerConnected)
                return;
            controller.udEngine.Dn_Send_0x01_CMD_01_0x58();
        }

        private void btn01_F5_Click(object sender, EventArgs e)
        {
            if (!isControllerConnected)
                return;
            UInt16 recCount;  
            recCount = Convert.ToUInt16( tbBlockNum.Text, 10);
            controller.udEngine.Dn_Send_0x01_CMD_01_0xF5(recCount);

        }


        private void btn01_F8_Click(object sender, EventArgs e)
        {
            if (!isControllerConnected)
                return;
            controller.udEngine.Dn_Send_0x01_CMD_01_0xF8();
        }

        private void btn01_F9_Click(object sender, EventArgs e)
        {
            if (!isControllerConnected)
                return;
            controller.udEngine.Dn_Send_0x01_CMD_01_0xF9(1024);
        }

        private void btn01_44_Click(object sender, EventArgs e)
        {
            if (!isControllerConnected)
                return;

            //tbBlockNum.Text = "0";
            UInt16 recCount;  
            recCount = Convert.ToUInt16( tbBlockNum.Text, 10);
            UInt16 byteCount;  
            byteCount = (UInt16)(recCount * 8);
            controller.udEngine.Dn_Send_0x01_CMD_01_0x44(byteCount);//2048 + 32);
        }
        private void btn01_45_Click(object sender, EventArgs e)
        {
            if (!isControllerConnected)
                return;

            UInt16 blkNum;  
            blkNum = Convert.ToUInt16( tbBlockNum.Text, 10);
            controller.udEngine.Dn_Send_0x01_CMD_01_0x45(blkNum);

        }

        Form_Maikon formMaikon = null;
        private void btnMaikon_Click(object sender, EventArgs e)
        {
            if( formMaikon == null )
            {
                formMaikon = new Form_Maikon();
            }

            if( formMaikon.IsDisposed == true )
            {
                formMaikon = new Form_Maikon();
            }

            if( formMaikon.Visible == false )
            {
                formMaikon.Show();
            }
            else
            {
                formMaikon.Hide();
            }

            if( formMaikon.maikon.serial_IsOpen() != true )
            {
                formMaikon.maikon.serial_Open();
            }

        }

#if false
        System::Void btnReadTest_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;
            controller.udEngine.UpDn_Read_Rctrl_test();
        }
        System::Void btnWriteTest_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;

            array<uint8_t,1>^ buf = gcnew array<uint8_t,1>(20);

            controller.udEngine.UpDn_Write_Wctrl_test(buf, buf.Length);
        }
#endif






    }
}
