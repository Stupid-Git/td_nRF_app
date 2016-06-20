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

namespace Zed
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            textBox1.Text = "0xFFFFFFFF";
            textBox2.Text = "0x01234567";
            checkBox1.Checked = true;
            checkBox2.Checked = false;
            m_torokuCode = (UInt32)Convert.ToInt32(textBox1.Text, 16);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == false) return;
            m_torokuCode = (UInt32)Convert.ToInt32(textBox1.Text, 16);
            Console.WriteLine("m_torokuCode changed to {0:x8}", m_torokuCode);
            checkBox2.Checked = false;
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == false) return;
            m_torokuCode = (UInt32)Convert.ToInt32(textBox2.Text, 16);
            Console.WriteLine("m_torokuCode changed to {0:x8}", m_torokuCode);
            checkBox1.Checked = false;
        }



        //**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++*
        //**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++*
        //**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++*
        //**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++*
        //**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++*
        //**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++*
        //**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++*
        //**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++*
        //**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++*
        //**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++*
        //**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++*
        //**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++**--**++*
        TDnRFLib.SunaBLE sunable = null;
        Protocol_T2 protocol_T2 = null;

        void protocol_T2_userEv_LogMessage(object sender, OutputReceivedEventArgs e)
        {
            richTextBox1_AppendText(e.Message);
        }
        public void protocol_T2_Init()
        {
            if (protocol_T2 == null)
            {
                protocol_T2 = new Protocol_T2();
                protocol_T2.userEv_LogMessage += protocol_T2_userEv_LogMessage;
            }
        }


        Protocol_01 protocol_01 = null;
        void protocol_01_userEv_LogMessage(object sender, OutputReceivedEventArgs e)
        {
            richTextBox1_AppendText(e.Message);
        }
        public void protocol_01_Init()
        {
            if (protocol_01 == null)
            {
                protocol_01 = new Protocol_01();
                protocol_01.userEv_LogMessage += protocol_01_userEv_LogMessage;
            }
        }
        
        public void sunable_Init()
        {
            if (sunable == null)
                sunable = new SunaBLE(this);

            protocol_T2_Init();
            protocol_01_Init();
        }

        private void tDnRFBLEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sunable_Init();
            sunable.ShowForm();
        }

        //String m_lastDeviceAddress = "FA92D0BB1C98";
        //String m_lastDeviceAddress = "DEADBEEF0123";







        //*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*
        //*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*
        //*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*
        //*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*

        void dsdsdasafa()
        {
            Byte[] TxPacket;
            Byte[] RxPacket = new Byte[0];

            TxPacket = new Byte[42];

            sunable.rtrBle_Open();

            sunable.rtrBle_Write_Packet(TxPacket);

            sunable.rtrBle_Read_Packet(ref TxPacket);

            sunable.rtrBle_Close();

            //----------------------
            sunable.BLE_ON();

            /*TODO*/
            String sDevAddr = sunable.GetConnectedDeviceAddress();
            sunable.Connect("DEADBEEF0123");
            sunable.Disconnect();
            /**/

            sunable.BLE_OFF();

        }


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


        //=====================================================================
        //=====================================================================
        //===== SEND RECV =====================================================
        //=====================================================================
        //=====================================================================
        private void btnBLE_TestSend_Click(object sender, EventArgs e)
        {
            Byte[] txPacket = null;
            Byte[] rxPacket = new Byte[0];

            sunable_Init();

            sunable.rtrBle_Open();

            txPacket = protocol_T2.RUINF_get_send_packet();
            sunable.rtrBle_Write_Packet(txPacket);

            sunable.rtrBle_Read_Packet(ref rxPacket);
            while (false == sunable.rtrBle_Read_Packet(ref rxPacket))
            {
                Application.DoEvents();
            };

            protocol_T2.RUINF_process_recv_packet(rxPacket, (UInt16)rxPacket.Length);

            sunable.rtrBle_Close();
        }


        //*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*
        //*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*+*-*
        public String m_The_sDeviceAddress = "";

        private void btTest_1_Click(object sender, EventArgs e)
        {
            int TO;
            Byte[] txPacket = null;
            Byte[] rxPacket = new Byte[0];

            richTextBox1.AppendText("Start\r\n");

            sunable_Init();

            sunable.rtrBle_Open();
            
            txPacket = protocol_01.CMD_58_get_send_packet();

            sunable.rtrBle_Write_Packet(txPacket);

            TO = 1000;
            sunable.rtrBle_Read_Packet(ref rxPacket);
            while (false == sunable.rtrBle_Read_Packet(ref rxPacket))
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(1);
                TO--;
                if (TO <= 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Timed out");
                    sunable.rtrBle_Close();
                    richTextBox1.AppendText("Timeout\r\n");
                    return;
                }
            };

            //rxPacket = new byte[] { 0x01, 0x58, 0x06, 0x04, 0x00,    0x00, 0x00, 0x00, 0x00,   0x63, 0x00};
            protocol_01.CMD_58_process_recv_packet(rxPacket, (UInt16)rxPacket.Length);
                        
            sunable.rtrBle_Close();

            richTextBox1.AppendText("End\r\n");


            /*
            // 操作禁止
            operation_ban();
            button1.Enabled = true;

            richTextBox1.Clear();

            if (serialPortOpen() == false)
            {
                richTextBox1.AppendText("ポートオープン失敗\r\n");
                goto error_proc;
            }

            // ブレーク信号送出
            sData[0] = 0x00;
            serialPort1.Write(sData, 0, 1);
            delay(50);

            // 通信権利　確保
            sData[0] = 0x01;
            sData[1] = (Byte)CMD_nRF_MODE;
            sData[2] = 0x01;

            sData[3] = (Byte)0x08;
            sData[4] = (Byte)0x00;

            Buffer.BlockCopy(password_user, 0, sData, 5, 8);

            sendCommand();
            if (recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            richTextBox1.AppendText("通信権利　確保\r\n");
            Application.DoEvents();

            // ｎＲＦ５１をブートモードに入れる（ＲＥＳＥＴ：０　ＢＯＯＴ：０）
            sData[0] = 0x01;
            sData[1] = (Byte)CMD_nRF_MODE;
            sData[2] = 0x02;

            sData[3] = (Byte)0x08;
            sData[4] = (Byte)0x00;

            sData[5] = (Byte)0x00;
            sData[6] = (Byte)0x00;

            sendCommand();
            if (recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            richTextBox1.AppendText("ＲＥＳＥＴ：０　ＢＯＯＴ：０\r\n");
            Application.DoEvents();
            delay(1000);

            // ＲＥＳＥＴ：１　ＢＯＯＴ：０
            sData[0] = 0x01;
            sData[1] = (Byte)CMD_nRF_MODE;
            sData[2] = 0x02;

            sData[3] = (Byte)0x08;
            sData[4] = (Byte)0x00;

            sData[5] = (Byte)0x01;
            sData[6] = (Byte)0x00;

            sendCommand();
            if (recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            richTextBox1.AppendText("ＲＥＳＥＴ：１　ＢＯＯＴ：０\r\n");
            Application.DoEvents();
            delay(1000);

            // ＲＥＳＥＴ：１　ＢＯＯＴ：１
            sData[0] = 0x01;
            sData[1] = (Byte)CMD_nRF_MODE;
            sData[2] = 0x02;

            sData[3] = (Byte)0x08;
            sData[4] = (Byte)0x00;

            sData[5] = (Byte)0x01;
            sData[6] = (Byte)0x01;

            sendCommand();
            if (recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            richTextBox1.AppendText("ＲＥＳＥＴ：１　ＢＯＯＴ：１\r\n");
            Application.DoEvents();
            delay(500);

        error_proc: ;
            richTextBox1.AppendText("終了");
            Application.DoEvents();
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
            */
        }

        //UInt32 m_torokuCode = 0x01234567; //01 9F 06 07 00 01 9F 15 00 00 FF FF 60 03 C3 03         ...
        //UInt32 m_torokuCode = 0x12345678;
        UInt32 m_torokuCode = 0xFFFFFFFF;
        private void btTest_2_Click(object sender, EventArgs e)
        {
            int TO;
            Byte[] t2Packet = null;
            Byte[] txPacket = null;
            Byte[] rxPacket = new Byte[0];

            richTextBox1.AppendText("Start\r\n");


            ///////////////////////////////
            ///////////////////////////////
            /*
            sunable_Init();
            sunable.rtrBle_Open();
            txPacket = protocol_T2.RUINF_get_send_packet();
            sunable.rtrBle_Write_Packet(txPacket);

            sunable.rtrBle_Read_Packet(ref rxPacket);
            while (false == sunable.rtrBle_Read_Packet(ref rxPacket))
            {
                Application.DoEvents();
            };

            protocol_T2.RUINF_process_recv_packet(rxPacket, (UInt16)rxPacket.Length);

            sunable.rtrBle_Close();
            */
            ///////////////////////////////
            ///////////////////////////////

            sunable_Init();
            sunable.rtrBle_Open();

            //t2Packet = protocol_01.CMD_58_get_send_packet();
            t2Packet = protocol_T2.RUINF_get_send_packet();
            txPacket = protocol_01.wrapWith9F(t2Packet, m_torokuCode);

            protocol_01.CMD_print("String s", txPacket, (UInt16)txPacket.Length);
            //return;
            sunable.rtrBle_Write_Packet(txPacket);

            TO = 1000;
            sunable.rtrBle_Read_Packet(ref rxPacket);
            while (false == sunable.rtrBle_Read_Packet(ref rxPacket))
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(1);
                TO--;
                if (TO <= 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Timed out");
                    sunable.rtrBle_Close();
                    richTextBox1.AppendText("Timeout\r\n");
                    return;
                }
            };

            //rxPacket = new byte[] { 0x01, 0x58, 0x06, 0x04, 0x00,    0x00, 0x00, 0x00, 0x00,   0x63, 0x00};
            //protocol_01.CMD_58_process_recv_packet(rxPacket, (UInt16)rxPacket.Length);
            protocol_T2.RUINF_process_recv_packet(rxPacket, (UInt16)rxPacket.Length);

            sunable.rtrBle_Close();

            richTextBox1.AppendText("End\r\n");


        }

        private void btTest_3_Click(object sender, EventArgs e)
        {
            int TO;
            Byte[] txPacket = null;
            Byte[] rxPacket = new Byte[0];

            richTextBox1.AppendText("Start\r\n");

            sunable_Init();

            sunable.rtrBle_Open();

            txPacket = protocol_01.CMD_9E_get_send_packet();

            sunable.rtrBle_Write_Packet(txPacket);

            TO = 1000;
            sunable.rtrBle_Read_Packet(ref rxPacket);
            while (false == sunable.rtrBle_Read_Packet(ref rxPacket))
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(1);
                TO--;
                if (TO <= 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Timed out");
                    sunable.rtrBle_Close();
                    richTextBox1.AppendText("Timeout\r\n");
                    return;
                }
            };

            //rxPacket = new byte[] { 0x01, 0x58, 0x06, 0x04, 0x00,    0x00, 0x00, 0x00, 0x00,   0x63, 0x00};
            protocol_01.CMD_9E_process_recv_packet(rxPacket, (UInt16)rxPacket.Length);

            sunable.rtrBle_Close();

            richTextBox1.AppendText("End\r\n");

        }

        private void btTest_3A_Click(object sender, EventArgs e)
        {
            int TO;
            Byte[] txPacket = null;
            Byte[] rxPacket = new Byte[0];

            richTextBox1.AppendText("Start\r\n");

            sunable_Init();

            sunable.rtrBle_Open();

            txPacket = protocol_01.CMD_9E_01_get_send_packet();

            sunable.rtrBle_Write_Packet(txPacket);

            TO = 1000;
            sunable.rtrBle_Read_Packet(ref rxPacket);
            while (false == sunable.rtrBle_Read_Packet(ref rxPacket))
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(1);
                TO--;
                if (TO <= 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Timed out");
                    sunable.rtrBle_Close();
                    richTextBox1.AppendText("Timeout\r\n");
                    return;
                }
            };

            //rxPacket = new byte[] { 0x01, 0x58, 0x06, 0x04, 0x00,    0x00, 0x00, 0x00, 0x00,   0x63, 0x00};
            protocol_01.CMD_9E_01_process_recv_packet(rxPacket, (UInt16)rxPacket.Length);

            sunable.rtrBle_Close();

            richTextBox1.AppendText("End\r\n");

        }

        private void direct_0x69_01()
        {
            int TO;
            Byte[] txPacket = null;
            Byte[] rxPacket = new Byte[0];

            richTextBox1.AppendText("Start\r\n");

            sunable_Init();

            sunable.rtrBle_Open();

            //txPacket = protocol_01.CMD_33_00_get_send_packet();
            txPacket = protocol_01.CMD_69_01_get_send_packet();

            sunable.rtrBle_Write_Packet(txPacket);

            TO = 1000;
            sunable.rtrBle_Read_Packet(ref rxPacket);
            while (false == sunable.rtrBle_Read_Packet(ref rxPacket))
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(1);
                TO--;
                if (TO <= 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Timed out");
                    sunable.rtrBle_Close();
                    richTextBox1.AppendText("Timeout\r\n");
                    return;
                }
            };

            //rxPacket = new byte[] { 0x01, 0x58, 0x06, 0x04, 0x00,    0x00, 0x00, 0x00, 0x00,   0x63, 0x00};
            //protocol_01.CMD_33_00_process_recv_packet(rxPacket, (UInt16)rxPacket.Length);
            protocol_01.CMD_69_01_process_recv_packet(rxPacket, (UInt16)rxPacket.Length);

            sunable.rtrBle_Close();

            richTextBox1.AppendText("End\r\n");

        }

        void wrap_0xNN()
        {
            int TO;
            Byte[] t2Packet = null;
            Byte[] txPacket = null;
            Byte[] rxPacket = new Byte[0];

            richTextBox1.AppendText("Start\r\n");

            sunable_Init();
            sunable.rtrBle_Open();

            //t2Packet = protocol_01.CMD_58_get_send_packet();
            //OLD t2Packet = protocol_T2.RUINF_get_send_packet();
            //t2Packet = protocol_01.CMD_33_00_get_send_packet();
            t2Packet = protocol_01.CMD_69_01_get_send_packet();

            txPacket = protocol_01.wrapWith9F(t2Packet, m_torokuCode);

            protocol_01.CMD_print("String s", txPacket, (UInt16)txPacket.Length);
            //return;
            sunable.rtrBle_Write_Packet(txPacket);

            TO = 1000;
            sunable.rtrBle_Read_Packet(ref rxPacket);
            while (false == sunable.rtrBle_Read_Packet(ref rxPacket))
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(1);
                TO--;
                if (TO <= 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Timed out");
                    sunable.rtrBle_Close();
                    richTextBox1.AppendText("Timeout\r\n");
                    return;
                }
            };

            //rxPacket = new byte[] { 0x01, 0x58, 0x06, 0x04, 0x00,    0x00, 0x00, 0x00, 0x00,   0x63, 0x00};
            //protocol_01.CMD_58_process_recv_packet(rxPacket, (UInt16)rxPacket.Length);
            //OLD protocol_T2.RUINF_process_recv_packet(rxPacket, (UInt16)rxPacket.Length);
            //protocol_01.CMD_33_00_process_recv_packet(rxPacket, (UInt16)rxPacket.Length);
            protocol_01.CMD_69_01_process_recv_packet(rxPacket, (UInt16)rxPacket.Length);

            sunable.rtrBle_Close();

            richTextBox1.AppendText("End\r\n");
          
        }

        private void bt0xnn_Click(object sender, EventArgs e)
        {
            //direct_0x69_01();
            wrap_0xNN();
        }

        private void btTestSegi48_Click(object sender, EventArgs e)
        {

            int TO;
            Byte[] t2Packet = null;
            Byte[] txPacket = null;
            Byte[] rxPacket = new Byte[0];

            richTextBox1.AppendText("Start\r\n");

            sunable_Init();
            sunable.rtrBle_Open();

            //t2Packet = protocol_01.CMD_58_get_send_packet();
            //OLD t2Packet = protocol_T2.RUINF_get_send_packet();
            //t2Packet = protocol_01.CMD_33_00_get_send_packet();
            t2Packet = protocol_01.CMD_48_00_get_send_packet();

            txPacket = protocol_01.wrapWith9F(t2Packet, m_torokuCode);

            protocol_01.CMD_print("String s", txPacket, (UInt16)txPacket.Length);
            //return;
            sunable.rtrBle_Write_Packet(txPacket);

            TO = 1000;
            sunable.rtrBle_Read_Packet(ref rxPacket);
            while (false == sunable.rtrBle_Read_Packet(ref rxPacket))
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(1);
                TO--;
                if (TO <= 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Timed out");
                    sunable.rtrBle_Close();
                    richTextBox1.AppendText("Timeout\r\n");
                    return;
                }
            };

            //rxPacket = new byte[] { 0x01, 0x58, 0x06, 0x04, 0x00,    0x00, 0x00, 0x00, 0x00,   0x63, 0x00};
            //protocol_01.CMD_58_process_recv_packet(rxPacket, (UInt16)rxPacket.Length);
            //OLD protocol_T2.RUINF_process_recv_packet(rxPacket, (UInt16)rxPacket.Length);
            //protocol_01.CMD_33_00_process_recv_packet(rxPacket, (UInt16)rxPacket.Length);
            protocol_01.CMD_48_00_process_recv_packet(rxPacket, (UInt16)rxPacket.Length);

            sunable.rtrBle_Close();

            richTextBox1.AppendText("End\r\n");
        }
    }
}
