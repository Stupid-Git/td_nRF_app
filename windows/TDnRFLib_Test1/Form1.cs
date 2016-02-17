using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using TDnRFLib; 


namespace TDnRFLib_Test1
{
    public partial class Form1 : Form
    {
        FormScan m_formScan;
        //DD public TDnRFClass1 btc;
        Protocol_T2 protocol_T2;

        public Form1()
        {
            InitializeComponent();

            //DD btc = new TDnRFClass1();

            //DD btc.userEv_DeviceInfoUpdate += OnEvent_DeviceInfoUpdate;
            //DD btc.userEv_Connected += OnEvent_Connected;
            //DD btc.userEv_Disconnected += OnEvent_Disconnected;
            //DD btc.userEv_udPacket += OnEvent_udPacket;

            protocol_T2 = new Protocol_T2();
            protocol_T2.userEv_LogMessage += On_protocol_T2_LogMessage;

        }

        public void richTextBox1_AppendText(String S)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => richTextBox1_AppendText(S)));
            }
            else
            {
                richTextBox1.AppendText(S);
                richTextBox1.ScrollToCaret();
            }
        }




        //*********************************************************************
        //***** Library Initialisation ****************************************
        //*********************************************************************
        private void btnInitBLE_Click(object sender, EventArgs e)
        {
            //btc.upEV_Log += asasasasa;
            btc.PC_Initialize(this);
        }

        //*********************************************************************
        //***** Nordic Semiconductor USB Dongle control ***********************
        //*********************************************************************
        private void btnOpenBLE_Click(object sender, EventArgs e)
        {
            btc.PC_OpenController();
        }
        private void btnCloseBLE_Click(object sender, EventArgs e)
        {
            btc.PC_CloseController();
        }


        //*********************************************************************
        //***** Discovery (Scanning) ******************************************
        //*********************************************************************
        List<BtDeviceInfo> m_deviceInfoList = new List<BtDeviceInfo>();

        void Update_formScan()
        {
            Form m_parentForm = this;

            if (m_formScan == null)
                m_formScan = new FormScan(this);
            if (m_formScan.IsDisposed)
                m_formScan = new FormScan(this);

            if (m_formScan.Visible == false)
            {
                //m_formScan.Show();
                m_parentForm.Invoke((System.Windows.Forms.MethodInvoker)delegate()
                {
                    //m_formScan.Text = "my text";
                    m_formScan.Show();
                });
            }


            m_formScan.UpdateList(m_deviceInfoList);
        }

        void OnEvent_DeviceInfoUpdate(object sender, MinDeviceInfoEventArgs e)
        {
            richTextBox1_AppendText(String.Format("DeviceAddr = {0}, Rssi = {1}\r\n", e.DeviceAddr, e.Rssi));
            
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

        private void btnStartScanBLE_Click(object sender, EventArgs e)
        {
            m_deviceInfoList.Clear();
            btc.PC_StartScanning();
        }
        private void btnStopScanBLE_Click(object sender, EventArgs e)
        {
            btc.PC_StopScanning();
        }

        //*********************************************************************
        //***** Connect/Disconnect to/from peripheral *************************
        //*********************************************************************
        private void btnConnectBLE_Click(object sender, EventArgs e)
        {
            btc.PC_Connect("22");
        }
        void OnEvent_Connected(object sender, EventArgs e)
        {
            richTextBox1_AppendText(String.Format("Device Connected\r\n"));
        }

        private void btnDisconnectBLE_Click(object sender, EventArgs e)
        {
            btc.PC_Disconnect();
        }
        void OnEvent_Disconnected(object sender, EventArgs e)
        {
            richTextBox1_AppendText(String.Format("Device Disconnected\r\n"));
        }


        //*********************************************************************
        //***** Comminication with T&D UDService (udEngine) *******************
        //*********************************************************************
        private void btnTestSend_Click(object sender, EventArgs e)
        {
            Send_request_RUINF();
        }



        void On_protocol_T2_LogMessage(object sender, TDnRFLib.OutputReceivedEventArgs e)
        {
            //if (InvokeRequired)
            //{
            //    Invoke((MethodInvoker)(() => On_protocol_T2_LogMessage(sender, e)));
            //}
            //else
            //{
                richTextBox1_AppendText(e.Message /*+ "\r\n"*/);
            //}
        }


        void Send_request_RUINF()
        {
            Int32 r;
            byte[] pkt;
            pkt = protocol_T2.RUINF_get_send_packet();

            r = btc.PC_Send_udPacket(pkt);

        }

        void OnEvent_udPacket(object sender, udPacketEventArgs e)
        {
            richTextBox1_AppendText(String.Format("\r\nudPacket Received: Packet Length = {0}, {1}\r\n", e.Packet.Length, e.Length));

            protocol_T2.RUINF_process_recv_packet(e.Packet, (UInt16)e.Packet.Length);
        }


        /*
        Int32 TBR_XXXXXX_On_udPacketCallback(byte[] pkt)
        {
            protocol_T2.RUINF_process_recv_packet(pkt, (UInt16)pkt.Length);
            return (0);
        }
        void TBR_XXXXXX_Set_Local_udPacketCallback()
        {
            Int32 r;
            r = btc.PC_Set_callbackOn_udPacket(TBR_XXXXXX_On_udPacketCallback);
        }
        */

    }
}
