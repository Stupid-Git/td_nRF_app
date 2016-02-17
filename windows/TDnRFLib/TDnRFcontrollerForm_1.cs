using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


//using rtr500ble_Command_test; //protocol_T2

namespace TDnRFLib //TDnRFcontroller_LIB //namespace rtr500ble_Command_test
{
    public partial class TDnRFcontrollerForm_1 : Form
    {
        TDnRFcontroller bleCtrl;

        public String m_lastDeviceAddress;
        Form m_parentForm1;

        bool isControllerInitialized = false;
        bool isControllerConnected = false;


        public TDnRFcontrollerForm_1(Form parentForm1)
        {
            InitializeComponent();

            Initialize_listView1();

            m_parentForm1 = parentForm1;
            bleCtrl = TDnRFcontroller.getInstance();

            Initialize_thisForm();
        }

        public TDnRFcontrollerForm_1(Form parentForm1, TDnRFcontroller controller)
        {
            InitializeComponent();

            Initialize_listView1();

            m_parentForm1 = parentForm1;
            bleCtrl = controller;

            Initialize_thisForm();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TDnRFcontrollerForm_1_Shown(object sender, EventArgs e)
        {
            if (!bleCtrl.IsInitialized)
            {
                richTextBox1_AppendText("bleCtrl NOT Initialized" + "\r\n");

                // Buttons
                button_Enable(btnBLE_ON, true);
                button_Enable(btnBLE_OFF, false);

                //button_Enable(btnBLE_Init, true);
                //button_Enable(btnBLE_Close, false);
                button_Enable(btnBLE_TestSend, false);
            }
            else if (!bleCtrl.Initialized_OK)
            {
                richTextBox1_AppendText("bleCtrl Initialization has FAILED" + "\r\n");
                // Buttons
                button_Enable(btnBLE_ON, true);
                button_Enable(btnBLE_OFF, false);

                //button_Enable(btnBLE_Init, true);
                //button_Enable(btnBLE_Close, false);
                button_Enable(btnBLE_TestSend, false);
            }
            else// if (bleCtrl.Initialized_OK)
            {
                richTextBox1_AppendText("bleCtrl is Initialized OK" + "\r\n");
                // Buttons
                button_Enable(btnBLE_ON, false);
                button_Enable(btnBLE_OFF, true);

                //button_Enable(btnBLE_Init, false);
                //button_Enable(btnBLE_Close, false);
                button_Enable(btnBLE_TestSend, false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TDnRFcontrollerForm_1_FormClosing(object sender, FormClosingEventArgs e)
        {
            bleCtrl.appEv_ScanningStarted -= bleCtrl_appEv_ScanningStarted;
            bleCtrl.appEv_DeviceInfoUpdate -= bleCtrl_appEv_DeviceInfoUpdate;
            bleCtrl.appEv_ScanningCancelled -= bleCtrl_appEv_ScanningCancelled;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TDnRFcontrollerForm_1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }



        void Initialize_thisForm()
        {
            if (!bleCtrl.IsInitialized)
            {
                richTextBox1_AppendText("!bleCtrl.IsInitialized" + "\r\n");
                label_NO_BLE.Text = "";
            }
            else
            {
                if (bleCtrl.Initialized_OK)
                {
                    richTextBox1_AppendText("Initialized  ==OK==" + "\r\n");
                    label_NO_BLE.Text = "";
                }
                else
                {
                    richTextBox1_AppendText("Initialized  ==NG==" + "\r\n");
                    label_NO_BLE.Text = "BLE NG";
                }
            }
        }

        #region GUI event handlers

        void button_Enable_All(bool state)
        {
            button_Enable(btnBLE_ON, state);
            button_Enable(btnBLE_OFF, state);
            button_Enable(btnBLE_SCAN_start, state);
            button_Enable(btnBLE_SCAN_stop, state);
            button_Enable(btnBLE_CONNECT, state);
            button_Enable(btnBLE_DISCONNECT, state);
        }

        //#####################################################################
        //#####################################################################
        //##### GUI Event Handlers ############################################
        //#####################################################################
        //#####################################################################
        private void btnBLE_ON_Click(object sender, EventArgs e)
        {
            button_Enable_All(false);
            onEvent_BLE_ON(sender, e); //========== GO ==========

        }
        private void btnBLE_OFF_Click(object sender, EventArgs e)
        {
            button_Enable_All(false);
            onEvent_BLE_OFF(sender, e); //========== GO ==========

            button_Enable(btnBLE_ON, true);
        }

        private void btnBLE_SCAN_start_Click(object sender, EventArgs e)
        {
            button_Enable_All(false);
            onEvent_StartScan(sender, e); //========== GO ==========

            //ON OK
            button_Enable(btnBLE_SCAN_stop, true);
        }

        private void btnBLE_SCAN_stop_Click(object sender, EventArgs e)
        {
            button_Enable_All(false);
            onEvent_StopScan(sender, e); //========== GO ==========

            //ON OK
            button_Enable(btnBLE_SCAN_start, true);
            button_Enable(btnBLE_CONNECT, true);
            button_Enable(btnBLE_OFF, true);
        }

        private void btnBLE_CONNECT_Click(object sender, EventArgs e)
        {
            button_Enable_All(false);
            onEvent_Connect(sender, e); //========== GO ==========

            //ON OK           
            button_Enable(btnBLE_DISCONNECT, true);
        }

        private void btnBLE_DISCONNECT_Click(object sender, EventArgs e)
        {
            button_Enable_All(false);
            onEvent_Disconnect(sender, e); //========== GO ==========

            //ON OK
            button_Enable(btnBLE_SCAN_start, true);
            button_Enable(btnBLE_CONNECT, true);
            button_Enable(btnBLE_OFF, true);
        }

        
        //#####################################################################
        //#####################################################################
        //#####################################################################
        #endregion GUI event handlers



        //#####################################################################
        //#####################################################################
        //##### listView ######################################################
        //#####################################################################
        //#####################################################################
        void Initialize_listView1()
        {

            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;

            //Add column header
            listView1.Columns.Add("Name", 100);
            listView1.Columns.Add("Adddrrss", 100);
            listView1.Columns.Add("Rssi", 70);

            //Add items in the listview
            string[] arr = new string[4];
            ListViewItem itm;

            //Add first item
            arr[0] = "...";
            arr[1] = "...";
            arr[2] = "...";
            itm = new ListViewItem(arr);
            listView1.Items.Add(itm);

            //Add second item
            arr[0] = "...";
            arr[1] = "...";
            arr[2] = "...";
            itm = new ListViewItem(arr);
            listView1.Items.Add(itm);
        }

        bool updateListBusy = false; //Just to stop the screan from being updated like crazy
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceList"></param>
        public void UpdateList(List<BtDeviceInfo> deviceList)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => UpdateList(deviceList)));
            }
            else
            {
                if (updateListBusy)
                    return;

                updateListBusy = true;

                if (deviceList.Count <= 0)
                    goto L_end;

                listView1.Items.Clear();
                int i;
                for (i = 0; i < deviceList.Count; i++)
                {
                    BtDeviceInfo device = deviceList[i];

                    String sDeviceAddr = "...";
                    //String sDevInfo_Appearance = device.DeviceInfo[DeviceInfoType.Appearance];
                    String sDevInfo_Rssi = "...";
                    String sDevInfo_CompleteLocalName = "...";


                    sDeviceAddr = device.sDeviceAddr; //DeviceAddress.ToString();
                    sDevInfo_Rssi = device.sRssi;
                    sDevInfo_CompleteLocalName = device.sCompleteLocalName;

                    string[] arr = new string[4];
                    ListViewItem itm;

                    //Add first item
                    arr[0] = sDevInfo_CompleteLocalName;
                    arr[1] = sDeviceAddr;
                    arr[2] = sDevInfo_Rssi;
                    itm = new ListViewItem(arr);

                    listView1.Items.Add(itm);
                    //listView1.Items[0] = itm;
                }
            L_end:
                updateListBusy = false;
            }
        }

        //#####################################################################
        //#####################################################################
        //#####################################################################






        void buttons_Update()
        {
            if (!bleCtrl.IsInitialized)
            {
                button_Enable_All(false);
                button_Enable(btnBLE_ON, true);
                return;
            }

            if ((bleCtrl.IsInitialized) && (!bleCtrl.Initialized_OK))
            {
                button_Enable_All(false);
                label_NO_BLE.Text = "BLE FAILURE";
                button_Enable(btnBLE_ON, true);
                return;
            }


            
            return;
        }


        //=====================================================================
        //=====================================================================
        //===== Initialize === BLE_ON =========================================
        //=====================================================================
        //=====================================================================
        bool Flag_bleInitStarted = false;
        volatile bool Flag_bleInitDone = false;

        private void onEvent_BLE_ON(object sender, EventArgs e)
        {
            if (Flag_bleInitStarted == true)
                return;
            Flag_bleInitStarted = true;

            if ((bleCtrl.IsInitialized) && (bleCtrl.Initialized_OK))
                goto L_End;

            if (!bleCtrl.IsInitialized)
            {
                bleCtrl.appEv_InitializedResult += bleCtrl_appEv_InitializedResult;


                Flag_bleInitDone = false;

                bleCtrl.Initialize_Controller();

                while (!bleCtrl.IsInitialized) //while (Flag_bleInitDone == false)
                {
                    System.Threading.Thread.Sleep(10);
                    Application.DoEvents();
                    //TODO Time out

                }
                if ((bleCtrl.IsInitialized) && (bleCtrl.Initialized_OK))
                {
                    goto L_End;
                }
                else
                {
                    goto L_End;
                }
            }
        L_End:
            Flag_bleInitStarted = false;

        }
        //---------------------------------------------------------------------
        void bleCtrl_appEv_InitializedResult(object sender, Initialized_OK_EventArgs e)
        {
            if (e.Status == true)
            {
                isControllerInitialized = true;
                richTextBox1_AppendText("the Controller has just been Initialized" + "\r\n");

                //
                button_Enable(btnBLE_ON, false);
                button_Enable(btnBLE_OFF, true);
                //
                button_Enable(btnBLE_SCAN_start, true);
                button_Enable(btnBLE_SCAN_stop, false);
                //
                if( tbDeviceAddress.Text.Length == 12 )
                    button_Enable(btnBLE_CONNECT, true);
                else
                    button_Enable(btnBLE_CONNECT, false);
                button_Enable(btnBLE_DISCONNECT, false);
                //
            }
            else
            {
                isControllerInitialized = false;
                richTextBox1_AppendText("the Controller has FAILED Initialization" + "\r\n");

                //
                button_Enable(btnBLE_ON, true);
                button_Enable(btnBLE_OFF, false);
                //
                button_Enable(btnBLE_SCAN_start, false);
                button_Enable(btnBLE_SCAN_stop, false);
                //
                button_Enable(btnBLE_CONNECT, false);
                button_Enable(btnBLE_DISCONNECT, false);
            }
            bleCtrl.appEv_InitializedResult -= bleCtrl_appEv_InitializedResult;
            Flag_bleInitDone = true;
        }



        //=====================================================================
        //=====================================================================
        //===== Initialize === BLE_OFF ========================================
        //=====================================================================
        //=====================================================================
        private void onEvent_BLE_OFF(object sender, EventArgs e)
        {
            bleCtrl.Controller_Close();
            isControllerInitialized = false;

            //
            button_Enable(btnBLE_ON, true);
            button_Enable(btnBLE_OFF, false);
            //
            button_Enable(btnBLE_SCAN_start, false);
            button_Enable(btnBLE_SCAN_stop, false);
            //
            button_Enable(btnBLE_CONNECT, false);
            button_Enable(btnBLE_DISCONNECT, false);
            //
            button_Enable(btnBLE_TestSend, false);

        }


        //=====================================================================
        //=====================================================================
        //===== Scan ==========================================================
        //=====================================================================
        //=====================================================================
        bool Is_ScanStuff_Init = false;
        public void ScanStuff_Init()
        {
            if (!Is_ScanStuff_Init)
            {
                Is_ScanStuff_Init = true;
                bleCtrl.appEv_ScanningStarted += bleCtrl_appEv_ScanningStarted;
                bleCtrl.appEv_DeviceInfoUpdate += bleCtrl_appEv_DeviceInfoUpdate;
                bleCtrl.appEv_ScanningCancelled += bleCtrl_appEv_ScanningCancelled;
            }
        }
        public void ScanStuff_DeInit()
        {
            if (Is_ScanStuff_Init)
            {
                Is_ScanStuff_Init = false;
                bleCtrl.appEv_ScanningStarted -= bleCtrl_appEv_ScanningStarted;
                bleCtrl.appEv_DeviceInfoUpdate -= bleCtrl_appEv_DeviceInfoUpdate;
                bleCtrl.appEv_ScanningCancelled -= bleCtrl_appEv_ScanningCancelled;
            }
        }


        private void onEvent_StartScan(object sender, EventArgs e)
        {
            bool bStatus;

            ScanStuff_Init();
            bStatus = bleCtrl.StartDeviceScanning();
            if (bStatus == true)
            {
                // has started scanning
                button_Enable(btnBLE_SCAN_start, false);
                button_Enable(btnBLE_SCAN_stop, true);
            }
            else
            {
                // could not start scanning
                // OR
                // already scanning
                button_Enable(btnBLE_SCAN_start, true); //???
                button_Enable(btnBLE_SCAN_stop, false); //???
            }
        }


        private void onEvent_StopScan(object sender, EventArgs e)
        {
            ScanStuff_Init();
            bleCtrl.appEv_DeviceInfoUpdate -= bleCtrl_appEv_DeviceInfoUpdate;
            bleCtrl.StopDeviceScanning();

            // ?? Not Yet
            button_Enable(btnBLE_SCAN_start, true);
            button_Enable(btnBLE_SCAN_stop, false);
        }


        //FormScan m_formScan = null;
        List<BtDeviceInfo> m_deviceInfoList = new List<BtDeviceInfo>();

        void Update_formScan()
        {
            UpdateList(m_deviceInfoList);
        }

        void bleCtrl_appEv_DeviceInfoUpdate(object sender, MinDeviceInfoEventArgs e)
        {
            richTextBox1_AppendText("bleCtrl_appEv_DeviceInfoUpdate" + "\r\n");
            richTextBox1_AppendText(String.Format("    DeviceName = {0}", e.DeviceName) + "\r\n");
            richTextBox1_AppendText(String.Format("    DeviceAddr = {0}", e.DeviceAddr) + "\r\n");
            richTextBox1_AppendText(String.Format("    RSSI       = {0}", e.Rssi) + "\r\n");

            BtDeviceInfo device = new BtDeviceInfo();
            device.sCompleteLocalName = e.DeviceName;
            device.sDeviceAddr = e.DeviceAddr;
            device.sRssi = e.Rssi;


            bool devInfoInList = false;
            int i;
            for (i = 0; i < m_deviceInfoList.Count; i++)
            {
                BtDeviceInfo d = m_deviceInfoList[i];
                if (d.sDeviceAddr.Equals(device.sDeviceAddr))
                {
                    m_deviceInfoList[i] = device; // Update (Rssi)                    
                    devInfoInList = true;
                    break;
                }
            }


            if (!devInfoInList)
            {
                m_deviceInfoList.Add(device);
                //Console.WriteLine("meEv_OnDeviceDiscovered - NEW");
            }

            if ((!devInfoInList) || (true))//(false))
            {
                Update_formScan();
            }
            Update_formScan();
        }
        //---------------------------------------------------------------------
        void bleCtrl_appEv_ScanningStarted(object sender, EventArgs e)
        {
            richTextBox1_AppendText("bleCtrl_appEv_ScanningStarted" + "\r\n");

            // Buttons
            //button_Enable(btnBLE_Init, false);
            //button_Enable(btnBLE_Close, false);
            button_Enable(btnBLE_SCAN_start, false);
            button_Enable(btnBLE_SCAN_stop, true);
            //button_Enable(btnBLE_AutoConnect, false);
            //button_Enable(btnBLE_Connect, false);
            //button_Enable(btnBLE_Disconnect, false);
            //button_Enable(btnBLE_TestSend, false);
        }
        //---------------------------------------------------------------------
        void bleCtrl_appEv_ScanningCancelled(object sender, EventArgs e)
        {
            richTextBox1_AppendText("bleCtrl_appEv_ScanningCancelled" + "\r\n");

            ScanStuff_DeInit();

            // Buttons
            //button_Enable(btnBLE_Init, false);
            //button_Enable(btnBLE_Close, true);
            button_Enable(btnBLE_SCAN_start, true);
            button_Enable(btnBLE_SCAN_stop, false);
            //button_Enable(btnBLE_AutoConnect, true);
            //button_Enable(btnBLE_Connect, true);
            //button_Enable(btnBLE_Disconnect, false);
            //button_Enable(btnBLE_TestSend, false);
        }

        //---------------------------------------------------------------------






        //=====================================================================
        //=====================================================================
        //===== LISTVIEW ======================================================
        //=====================================================================
        //=====================================================================
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBox1_AppendText(String.Format("listView1_SelectedIndexChanged e. = {0}\r\n", e.ToString()));

            update_tbDeviceAddress(sender, e);
        }


        bool update_tbDeviceAddress(object sender, EventArgs e)
        {
            tbDeviceAddress.Text = "";

            int idx = -1;
            if (listView1.SelectedIndices.Count != 1)
                return false;

            idx = listView1.SelectedIndices[0];

            //Add items in the listview
            string[] arr = new string[4];
            ListViewItem itm;

            //arr[0] = "42";// sDevInfo_CompleteLocalName;
            //arr[1] = "42";// sDeviceAddr;
            //arr[2] = "42";// sDevInfo_Rssi;
            //itm = new ListViewItem(arr);

            String sDevInfo_CompleteLocalName;
            String sDeviceAddress;
            String sDevInfo_Rssi;
            itm = listView1.Items[idx];

            arr[0] = itm.SubItems[0].Text;
            arr[1] = itm.SubItems[1].Text;
            arr[2] = itm.SubItems[2].Text;

            sDevInfo_CompleteLocalName = arr[0];
            sDeviceAddress = arr[1];
            sDevInfo_Rssi = arr[2];

            richTextBox1_AppendText(String.Format("btn_Connect_Click idx = {0}\r\n", idx));
            richTextBox1_AppendText(String.Format("    sDevInfo_CompleteLocalName = {0}\r\n", sDevInfo_CompleteLocalName));
            richTextBox1_AppendText(String.Format("    sDeviceAddress = {0}\r\n", sDeviceAddress));
            richTextBox1_AppendText(String.Format("    sDevInfo_Rssi = {0}\r\n", sDevInfo_Rssi));


            tbDeviceAddress.Text = sDeviceAddress;
            m_The_sDeviceAddress = sDeviceAddress;

            return (true);
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            richTextBox1_AppendText(String.Format("listView1_DoubleClick\r\n"));
            int idx = -1;
            if (listView1.SelectedIndices.Count > 0)
            {
                idx = listView1.SelectedIndices[0];
            }
            richTextBox1_AppendText(String.Format("listView1_DoubleClick idx = {0}\r\n", idx));
        }


        //=====================================================================
        //=====================================================================
        //===== CONNECT DISCONNECT ============================================
        //=====================================================================
        //=====================================================================
        private void Unused_event__btn_ConnectText_Click(object sender, EventArgs e)
        {
            bool bStatus;

            if (tbDeviceAddress.TextLength != 12)
                return;

            ConnectStuff_Init();
            //bleCtrl.InitiateConnect();
            bStatus = bleCtrl.Connect_to_device_fromText(tbDeviceAddress.Text);//"FA92D0BB1C98");
        }

        private void Unused_event__btn_Connect_Click(object sender, EventArgs e)
        {
            int idx = -1;
            if (listView1.SelectedIndices.Count != 1)
                return;

            idx = listView1.SelectedIndices[0];

            //Add items in the listview
            string[] arr = new string[4];
            ListViewItem itm;

            //arr[0] = "42";// sDevInfo_CompleteLocalName;
            //arr[1] = "42";// sDeviceAddr;
            //arr[2] = "42";// sDevInfo_Rssi;
            //itm = new ListViewItem(arr);

            String sDevInfo_CompleteLocalName;
            String sDeviceAddress;
            String sDevInfo_Rssi;
            itm = listView1.Items[idx];

            arr[0] = itm.SubItems[0].Text;
            arr[1] = itm.SubItems[1].Text;
            arr[2] = itm.SubItems[2].Text;

            sDevInfo_CompleteLocalName = arr[0];
            sDeviceAddress = arr[1];
            sDevInfo_Rssi = arr[2];

            richTextBox1_AppendText(String.Format("btn_Connect_Click idx = {0}\r\n", idx));
            richTextBox1_AppendText(String.Format("    sDevInfo_CompleteLocalName = {0}\r\n", sDevInfo_CompleteLocalName));
            richTextBox1_AppendText(String.Format("    sDeviceAddress = {0}\r\n", sDeviceAddress));
            richTextBox1_AppendText(String.Format("    sDevInfo_Rssi = {0}\r\n", sDevInfo_Rssi));

            m_The_sDeviceAddress = sDeviceAddress;

            bool bStatus;
            //bStatus = m_parentForm1.bleCtrl.InitiateAutoConnect(m_parentForm1.m_The_sDeviceAddress);
            bStatus = bleCtrl.Connect_to_device_address(m_The_sDeviceAddress);

            this.Hide();
        }
        

        bool IsConnectStuff_Init = false;
        public void ConnectStuff_Init()
        {
            if (!IsConnectStuff_Init)
            {
                IsConnectStuff_Init = true;
                bleCtrl.appEv_ConnectionStarted += bleCtrl_appEv_ConnectionStarted;
                bleCtrl.appEv_ConnectionCancelled += bleCtrl_appEv_ConnectionCancelled;
                bleCtrl.appEv_Connected += bleCtrl_appEv_Connected;
                bleCtrl.appEv_ServiceDiscoveryCompleted += bleCtrl_appEv_ServiceDiscoveryCompleted;
                bleCtrl.appEv_Disconnected += bleCtrl_appEv_Disconnected;
            }
        }

        public void ConnectStuff_DeInit()
        {
            if (IsConnectStuff_Init)
            {
                IsConnectStuff_Init = false;
                bleCtrl.appEv_ConnectionStarted -= bleCtrl_appEv_ConnectionStarted;
                bleCtrl.appEv_ConnectionCancelled -= bleCtrl_appEv_ConnectionCancelled;
                bleCtrl.appEv_Connected -= bleCtrl_appEv_Connected;
                bleCtrl.appEv_ServiceDiscoveryCompleted -= bleCtrl_appEv_ServiceDiscoveryCompleted;
                bleCtrl.appEv_Disconnected -= bleCtrl_appEv_Disconnected;
            }
        }

        private void onEvent_Connect(object sender, EventArgs e)
        {
            bool bStatus;

            if (tbDeviceAddress.TextLength != 12)
                return;

            ConnectStuff_Init();
            //bleCtrl.InitiateConnect();
            bStatus = bleCtrl.Connect_to_device_address(tbDeviceAddress.Text);//"FA92D0BB1C98");
        }

        private void onEvent_Disconnect(object sender, EventArgs e)
        {
            ConnectStuff_Init();
            bleCtrl.InitiateDisconnect();
        }


        //---------------------------------------------------------------------
        //---------------------------------------------------------------------

        public String m_The_sDeviceAddress = "";


        void bleCtrl_appEv_ConnectionStarted(object sender, EventArgs e)
        {
            richTextBox1_AppendText("On_bleCtrl_ConnectionStarted" + "\r\n");

            // Buttons
            button_Enable(btnBLE_ON, false);
            button_Enable(btnBLE_OFF, false);

            //button_Enable(btnBLE_Init, false);
            //button_Enable(btnBLE_Close, false);
            button_Enable(btnBLE_TestSend, false);

        }
        void bleCtrl_appEv_ConnectionCancelled(object sender, EventArgs e)
        {
            richTextBox1_AppendText("On_bleCtrl_ConnectionCancelled" + "\r\n");
            // Buttons
            button_Enable(btnBLE_ON, false);
            button_Enable(btnBLE_OFF, true);

            //button_Enable(btnBLE_Init, false);
            //button_Enable(btnBLE_Close, true);
            button_Enable(btnBLE_TestSend, false);

        }
        void bleCtrl_appEv_Disconnected(object sender, EventArgs e)
        {
            richTextBox1_AppendText("On_bleCtrl_Disconnected" + "\r\n");

            // Buttons
            button_Enable(btnBLE_ON, false);
            button_Enable(btnBLE_OFF, true);
            
            //button_Enable(btnBLE_Init, false);
            //button_Enable(btnBLE_Close, true);
            button_Enable(btnBLE_TestSend, false);


        }
        void bleCtrl_appEv_Connected(object sender, EventArgs e)
        {
            richTextBox1_AppendText("ON_bleCtrl_Connected" + "\r\n");

            // Buttons
            button_Enable(btnBLE_ON, false);
            button_Enable(btnBLE_OFF, false);

            //button_Enable(btnBLE_Init, false);
            //button_Enable(btnBLE_Close, false);
            button_Enable(btnBLE_TestSend, false);

        }

        void bleCtrl_appEv_ServiceDiscoveryCompleted(object sender, EventArgs e)
        {
            richTextBox1_AppendText("On_bleCtrl_ServiceDiscoveryCompleted" + "\r\n");

            // Buttons
            button_Enable(btnBLE_ON, false);
            button_Enable(btnBLE_OFF, false);

            //button_Enable(btnBLE_Init, false);
            //button_Enable(btnBLE_Close, false);
            button_Enable(btnBLE_TestSend, true);

        }





        //=====================================================================
        //=====================================================================
        //===== SEND RECV =====================================================
        //=====================================================================
        //=====================================================================
        /*
        Protocol_T2 protocol_T2;
        void protocol_T2_userEv_LogMessage(object sender, OutputReceivedEventArgs e)
        {
            richTextBox1_AppendText(e.Message);
        }
        public void T2Stuff_Init()
        {
            if (protocol_T2 == null)
            {
                protocol_T2 = new Protocol_T2();
                protocol_T2.userEv_LogMessage += protocol_T2_userEv_LogMessage;
                bleCtrl.userEv_udPacket += OnEvent_udPacket;
            }
        }
        public void T2Stuff_DeInit()
        {
            bleCtrl.userEv_udPacket -= OnEvent_udPacket;
        }
        */
        private void btnBLE_TestSend_Click(object sender, EventArgs e)
        {
            /*
            Int32 r;
            byte[] pkt;

            T2Stuff_Init();//bogus

            pkt = protocol_T2.RUINF_get_send_packet();

            r = bleCtrl.PC_Send_udPacket(pkt);
            button_Enable(btnBLE_TestSend, false);
            //button_Enable(btnBLE_Disconnect, false);
            */
        }

        void OnEvent_udPacket(object sender, udPacketEventArgs e)
        {
            /*
            richTextBox1_AppendText(String.Format("\r\nudPacket Received: Packet Length = {0}, {1}\r\n", e.Packet.Length, e.Length));

            protocol_T2.RUINF_process_recv_packet(e.Packet, (UInt16)e.Packet.Length);
            button_Enable(btnBLE_TestSend, true);
            //button_Enable(btnBLE_Disconnect, true);
            */
        }



        //=====================================================================
        //===== Helpers =======================================================
        //=====================================================================
        public void richTextBox1_AppendText(String S)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => richTextBox1_AppendText(S)));
            }
            else
            {
                richTextBox1.AppendText(S /*+ "\r\n"*/);
                richTextBox1.ScrollToCaret();

            }
        }
        void button_Enable(Button B, bool state)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => button_Enable(B, state)));
            }
            else
            {
                B.Enabled = state;
            }
        }
        void button_Text(Button B, String text)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => button_Text(B, text)));
            }
            else
            {
                B.Text = text;
            }
        }


    }



    public class BtDeviceInfo
    {
        public String sDeviceAddr = "";
        //String sDevInfo_Appearance = device.DeviceInfo[DeviceInfoType.Appearance];
        public String sRssi = "";
        public String sCompleteLocalName = "";
    }
}
