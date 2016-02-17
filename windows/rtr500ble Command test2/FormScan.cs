using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace rtr500ble_Command_test
{
  
    public partial class FormScan : Form
    {
        Form1 m_parentForm1;
        //newController controller;
        //List<BtDevice> m_deviceList;
        public FormScan(Form1 parentForm1)//newController inst_controller)
        {
            InitializeComponent();

            m_parentForm1 = parentForm1;
            //this.controller = inst_controller;

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

        bool updateListBusy = false;
        public void UpdateList(List<BtDeviceInfo> deviceList)
        {
            if (InvokeRequired)
            {
                //Invoke((MethodInvoker)(() => UpdateList(deviceList)));
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
        


        private void btnStopStart_Click(object sender, EventArgs e)
        {
            //Int32 r;
            //r = controller.StopScanning();
            m_parentForm1.richTextBox1_AppendText(String.Format("btnStopStart_Click\r\n"));
            m_parentForm1.bleCtrl.StopDeviceScanning(); 
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_parentForm1.richTextBox1_AppendText( String.Format("listView1_SelectedIndexChanged e. = {0}\r\n", e.ToString() ) );
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            m_parentForm1.richTextBox1_AppendText(String.Format("listView1_DoubleClick\r\n"));
            int idx = -1;
            if (listView1.SelectedIndices.Count > 0)
            {
                idx = listView1.SelectedIndices[0];
            }
            m_parentForm1.richTextBox1_AppendText(String.Format("listView1_DoubleClick idx = {0}\r\n", idx));
        }

        private void btn_Connect_Click(object sender, EventArgs e)
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

            m_parentForm1.richTextBox1_AppendText(String.Format("btn_Connect_Click idx = {0}\r\n", idx));
            m_parentForm1.richTextBox1_AppendText(String.Format("    sDevInfo_CompleteLocalName = {0}\r\n", sDevInfo_CompleteLocalName));
            m_parentForm1.richTextBox1_AppendText(String.Format("    sDeviceAddress = {0}\r\n", sDeviceAddress));
            m_parentForm1.richTextBox1_AppendText(String.Format("    sDevInfo_Rssi = {0}\r\n", sDevInfo_Rssi));

            m_parentForm1.m_The_sDeviceAddress = sDeviceAddress;

            bool bStatus;
            //bStatus = m_parentForm1.bleCtrl.InitiateAutoConnect(m_parentForm1.m_The_sDeviceAddress);
            bStatus = m_parentForm1.bleCtrl.Connect_to_device_address(m_parentForm1.m_The_sDeviceAddress);

            this.Hide();
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
