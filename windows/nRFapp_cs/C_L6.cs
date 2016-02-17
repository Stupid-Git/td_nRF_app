using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;


namespace TDnRF
{
    class C_L6
    {
        Protocol_01 protocol_01;
        Protocol_T2 protocol_T2;
        UpDnEngine udEngine;

        public C_L6()
        {
            protocol_01 = new Protocol_01();
            protocol_T2 = new Protocol_T2();
        }

        public void L6_Setup(UpDnEngine udEngine)
        {
            this.udEngine = udEngine;

            this.udEngine.set_OnRXcallbackdelegate(L6_OnDataRxCallback);
        }


        Int32 L6_OnDataRxCallback(byte[] theUpPacket)
        {

            blk_dn_check0x01(theUpPacket);
            blk_dn_checkT2(theUpPacket);

            serLow_OnBLEupPacket(theUpPacket);

            return (42);
        }




        public Int32 Dn_Send_CMD_12()
        {
            byte[] buf = new byte[20]; // array<Byte,1> buf = new array<Byte,1>(20);
            Int32 r;
            //--- Make the command ---
            buf[0] = 1;
            buf[1] = 2;
            r = udEngine.Dn_Send_Dcmd(buf, 20);
            return (r);
        }

        public Int32 Dn_Send_CMD_13()
        {
            byte[] buf = new byte[20]; // array<Byte,1> buf = new array<Byte,1>(20);
            Int32 r;
            //--- Make the command ---
            buf[0] = 1;
            buf[1] = 3;
            r = udEngine.Dn_Send_Dcmd(buf, 20);
            return (r);
        }





        //---------------------------------------------------------------------
        public Int32 Dn_Send_T2_DummyTEST1()
        {
            int i;
            byte[] buf = new byte[20]; // array<Byte,1> buf = new array<Byte,1>(20);

            UInt16 dataSize = 10;

            // fake make packet
            byte[] pktbuf = new byte[dataSize + 6]; // array<System::Byte,1> pktbuf = new array<Byte,1>(dataSize + 6);
            UInt16 cs_pkt;

            pktbuf[0] = (Byte)'T';
            pktbuf[1] = (Byte)'2';
            pktbuf[2] = (Byte)((dataSize >> 0) & 0x00FF); //Len LSB
            pktbuf[3] = (Byte)((dataSize >> 8) & 0x00FF); //Len MSB

            pktbuf[4] = (Byte)'a';
            pktbuf[5] = (Byte)'b';
            pktbuf[6] = (Byte)'c';
            pktbuf[7] = (Byte)'d';
            pktbuf[8] = (Byte)'e';
            pktbuf[9] = (Byte)'A';
            pktbuf[10] = (Byte)'B';
            pktbuf[11] = (Byte)'C';
            pktbuf[12] = (Byte)'D';
            pktbuf[13] = (Byte)'E';

            cs_pkt = 0;
            for (i = 4; i < pktbuf.Length - 2; i++)
            {
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf.Length - 2] = (byte)((cs_pkt >> 0) & 0x00FF);
            pktbuf[pktbuf.Length - 1] = (byte)((cs_pkt >> 8) & 0x00FF);


            udEngine.Dn_Send_CMD_11_Pkt(pktbuf);
            return (0);
        }

        public Int32 T2_RUINF()
        {
            byte[] pktbuf;

            pktbuf = protocol_T2.RUINF_get_send_packet();

            udEngine.Dn_Send_CMD_11_Pkt(pktbuf);

            return (0);

            /* TODO
            if (serialPortOpen() == false)
            {
                richTextBox1.AppendText("ポートオープン失敗\r\n");
                goto error_proc;
            }

            // ブレーク信号送出
            sData[0] = 0x00;
            serialPort1.Write(sData, 0, 1);
            delay(50);

            // コマンド送出
            string[] command = { "RUINF:" };

            command_make(command);

            UnUsed_sendCommand();
            UnUsed_t_recvCommand();

            extraction();   // パラメータ抽出

        error_proc: ;
            richTextBox1.AppendText("終了\r\n");
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
            TODO*/
        }

        //---------------------------------------------------------------------
        public Int32 Dn_Send_0x01_CMD_01_0x58()
        {
            byte[] pkt;

            pkt = protocol_01.makeReqPkt_0x58();

            udEngine.Dn_Send_CMD_11_Pkt(pkt);

            return (0);
        }

        //---------------------------------------------------------------------
        public Int32 Dn_Send_0x01_CMD_01_0x58_A()
        {
            int i;
            byte[] buf = new byte[20]; // array<Byte,1> buf = new array<Byte,1>(20);

            Byte cmd = 0x58; //CMD_GETSERIAL; CMD_01_0x58
            Byte subcmd = 0;
            UInt16 dataSize = 0;

            // fake make packet
            byte[] pktbuf = new byte[7]; //array<System::Byte,1> pktbuf = new array<Byte,1>(7);
            UInt16 cs_pkt;

            pktbuf[0] = 0x01;
            pktbuf[1] = cmd;
            pktbuf[2] = subcmd;
            pktbuf[3] = (byte)((dataSize >> 0) & 0x00FF); //Len LSB
            pktbuf[4] = (byte)((dataSize >> 8) & 0x00FF); //Len MSB

            cs_pkt = 0;
            for (i = 0; i < pktbuf.Length - 2; i++)
            {
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf.Length - 2] = (byte)((cs_pkt >> 0) & 0x00FF);
            pktbuf[pktbuf.Length - 1] = (byte)((cs_pkt >> 8) & 0x00FF);


            udEngine.Dn_Send_CMD_11_Pkt(pktbuf);
            return (0);
        }

        //---------------------------------------------------------------------
        public Int32 Dn_Send_0x01_CMD_01_0xF5(UInt16 recCount)
        {
            int i;
            Byte cmd = 0xF5; //CMD_01_0xF5;
            Byte subcmd = 0;
            UInt16 dataSize = 2;
            UInt16 cs_pkt;

            byte[] pktbuf = new byte[dataSize + 7]; //array<System::Byte,1> pktbuf = new array<Byte,1>(dataSize + 7);

            pktbuf[0] = 0x01;
            pktbuf[1] = cmd;
            pktbuf[2] = subcmd;
            pktbuf[3] = (byte)((dataSize >> 0) & 0x00FF); //Len LSB
            pktbuf[4] = (byte)((dataSize >> 8) & 0x00FF); //Len MSB
            pktbuf[5] = (byte)((recCount >> 0) & 0x00FF); //Len LSB
            pktbuf[6] = (byte)((recCount >> 8) & 0x00FF); //Len MSB

            cs_pkt = 0;
            for (i = 0; i < pktbuf.Length - 2; i++)
            {
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf.Length - 2] = (byte)((cs_pkt >> 0) & 0x00FF);
            pktbuf[pktbuf.Length - 1] = (byte)((cs_pkt >> 8) & 0x00FF);


            udEngine.Dn_Send_CMD_11_Pkt(pktbuf);
            return (0);
        }

        //---------------------------------------------------------------------
        public Int32 Dn_Send_0x01_CMD_01_0xF8()
        {
            int i;
            Byte cmd = 0xF8; //CMD_01_0xF8;
            Byte subcmd = 0;
            UInt16 dataSize = 0;
            UInt16 cs_pkt;

            byte[] pktbuf = new byte[dataSize + 7]; //array<System::Byte,1> pktbuf = new array<Byte,1>(dataSize + 7);

            pktbuf[0] = 0x01;
            pktbuf[1] = cmd;
            pktbuf[2] = subcmd;
            pktbuf[3] = (byte)((dataSize >> 0) & 0x00FF); //Len LSB
            pktbuf[4] = (byte)((dataSize >> 8) & 0x00FF); //Len MSB

            cs_pkt = 0;
            for (i = 0; i < pktbuf.Length - 2; i++)
            {
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf.Length - 2] = (byte)((cs_pkt >> 0) & 0x00FF);
            pktbuf[pktbuf.Length - 1] = (byte)((cs_pkt >> 8) & 0x00FF);


            udEngine.Dn_Send_CMD_11_Pkt(pktbuf);
            return (0);
        }

        //---------------------------------------------------------------------
        public Int32 Dn_Send_0x01_CMD_01_0xF9(Int32 size)
        {
            int i;
            Byte cmd = 0xF9; //CMD_01_0xF9
            Byte subcmd = 0;
            UInt16 dataSize = 2;
            UInt16 cs_pkt;

            byte[] pktbuf = new byte[dataSize + 7]; // array<System::Byte,1> pktbuf = new array<Byte,1>(dataSize + 7);

            pktbuf[0] = 0x01;
            pktbuf[1] = cmd;
            pktbuf[2] = subcmd;
            pktbuf[3] = (byte)((dataSize >> 0) & 0x00FF); //Len LSB
            pktbuf[4] = (byte)((dataSize >> 8) & 0x00FF); //Len MSB

            pktbuf[5] = (byte)((size >> 0) & 0x00ff); ;
            pktbuf[6] = (byte)((size >> 8) & 0x00ff);

            cs_pkt = 0;
            for (i = 0; i < pktbuf.Length - 2; i++)
            {
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf.Length - 2] = (byte)((cs_pkt >> 0) & 0x00FF);
            pktbuf[pktbuf.Length - 1] = (byte)((cs_pkt >> 8) & 0x00FF);


            udEngine.Dn_Send_CMD_11_Pkt(pktbuf);
            return (0);
        }

        //---------------------------------------------------------------------
        public Int32 Dn_Send_0x01_CMD_01_0x44(UInt16 byteCount)
        {
            int i;
            Byte cmd = 0x44; //CMD_01_0x44;
            Byte subcmd = 0;
            UInt16 dataSize = 2;
            UInt16 cs_pkt;

            byte[] pktbuf = new byte[dataSize + 7]; //array<System::Byte,1> pktbuf = new array<Byte,1>(dataSize + 7);

            pktbuf[0] = 0x01;
            pktbuf[1] = cmd;
            pktbuf[2] = subcmd;
            pktbuf[3] = (byte)((dataSize >> 0) & 0x00FF); //Len LSB
            pktbuf[4] = (byte)((dataSize >> 8) & 0x00FF); //Len MSB

            pktbuf[5] = (byte)((byteCount >> 0) & 0x00ff);
            pktbuf[6] = (byte)((byteCount >> 8) & 0x00ff);

            cs_pkt = 0;
            for (i = 0; i < pktbuf.Length - 2; i++)
            {
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf.Length - 2] = (byte)((cs_pkt >> 0) & 0x00FF);
            pktbuf[pktbuf.Length - 1] = (byte)((cs_pkt >> 8) & 0x00FF);


            udEngine.Dn_Send_CMD_11_Pkt(pktbuf);
            return (0);
        }

        //---------------------------------------------------------------------
        public Int32 Dn_Send_0x01_CMD_01_0x45(UInt16 blockNum)
        {
            int i;
            Byte cmd = 0x45; //CMD_01_0x45;
            Byte subcmd = 0x06;
            UInt16 dataSize = 2;
            UInt16 cs_pkt;

            byte[] pktbuf = new byte[dataSize + 7]; //array<System::Byte,1> pktbuf = new array<Byte,1>(dataSize + 7);

            pktbuf[0] = 0x01;
            pktbuf[1] = cmd;
            pktbuf[2] = subcmd;
            pktbuf[3] = (byte)((dataSize >> 0) & 0x00FF); //Len LSB
            pktbuf[4] = (byte)((dataSize >> 8) & 0x00FF); //Len MSB

            pktbuf[5] = (byte)((blockNum >> 0) & 0x00ff);
            pktbuf[6] = (byte)((blockNum >> 8) & 0x00ff);

            cs_pkt = 0;
            for (i = 0; i < pktbuf.Length - 2; i++)
            {
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf.Length - 2] = (byte)((cs_pkt >> 0) & 0x00FF);
            pktbuf[pktbuf.Length - 1] = (byte)((cs_pkt >> 8) & 0x00FF);


            udEngine.Dn_Send_CMD_11_Pkt(pktbuf);
            return (0);
        }




        void blk_dn_checkT2(byte[] theUpPacket)
        {
            UInt16 T2len;

            if ((theUpPacket[0] == 'T') && (theUpPacket[1] == '2'))
            {

                Console.WriteLine("blk_dn_checkT2");

                Console.WriteLine();
                Console.WriteLine();
                Console.Write("Got T2 Response\n");
                Console.WriteLine();
                Console.WriteLine();

                T2len = theUpPacket[2];
                T2len += (UInt16)(theUpPacket[3] << 8);
                T2len += 6;

                protocol_T2.RUINF_process_recv_packet(theUpPacket, T2len); // 'T'
            }

        }

        void blk_dn_check0x01(byte[] theUpPacket)
        {
            Int32 r = 0;

            if (theUpPacket[0] == 0x01)
            {
                Console.WriteLine("\nblk_dn_check0x01");

                if (theUpPacket[1] == 0x44)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.Write("Command 0x44: Status  = {0:x2}\n", theUpPacket[2]);
                    Console.Write("              Len     = {0}\n", (theUpPacket[4] << 8) + (theUpPacket[3]));
                    Console.Write("              data[0] = {0:x2}\n", theUpPacket[5]);
                    Console.Write("              data[1] = {0:x2}\n", theUpPacket[6]);
                    Console.WriteLine();
                    Console.WriteLine();
                }
                if (theUpPacket[1] == 0x45)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.Write("Command 0x45: Status  = {0:x2}\n", theUpPacket[2]);
                    Console.Write("              Len     = {0}\n", (theUpPacket[4] << 8) + (theUpPacket[3]));
                    Console.Write("              data[0] = {0:x2}\n", theUpPacket[5]);
                    Console.Write("              data[1] = {0:x2}\n", theUpPacket[6]);
                    Console.WriteLine();
                    Console.WriteLine();
                }


                if (theUpPacket[1] == 0x58)
                {
                    UInt32 serialNumber = 0;

                    r = protocol_01.procRspPkt_0x58(theUpPacket, ref serialNumber);
                    if (r == 0)
                    {

                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine("Serial Number = {0}", serialNumber);
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.Write("Serial Number = ");
                        Console.Write("{0:x2}", theUpPacket[8]);
                        Console.Write("{0:x2}", theUpPacket[7]);
                        Console.Write("{0:x2}", theUpPacket[6]);
                        Console.Write("{0:x2}", theUpPacket[5]);
                        Console.WriteLine();
                        Console.WriteLine();
                    }
                }
                if (theUpPacket[1] == 0xF8)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.Write("Command 0xF8 Response\n");
                    Console.WriteLine();
                    Console.WriteLine();
                }
                if (theUpPacket[1] == 0xF9)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.Write("Command 0xF9 Response\n");
                    Console.WriteLine();
                    Console.WriteLine();
                }
            }

        }


        //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
        //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
        //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
        //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
        //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&

        SerialPort serLow;

        public void serLow_Create()
        {
            if (serLow != null)
                return;
            serLow = new SerialPort("COM22", 115200);
            serLow.DataReceived += serLow_DataReceived;

        }

        public void serLow_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            maikonC_main_proc();
            //throw new NotImplementedException();
        }

        public void serLow_Open()
        {
            if (serLow == null)
                return;
            if (serLow.IsOpen == true)
                return;

            serLow.Open();
        }

        public void serLow_Close()
        {
            if (serLow == null)
                return;
            if (serLow.IsOpen == false)
                return;
            serLow.Close();
        }

        byte[] serLow_rData = new byte[20];
        byte[] _rData = new byte[20];

        /*TODO ??
        void serLowTask_XXX()
        {
            Task.Factory.StartNew(() =>
            {
                if (serLow.BytesToRead > 0)
                {
                    //serLow.Read(serLow_rData, 0, 1);
                }
                //System.Threading.Tasks.Sleep(1);
            }
            );
        }
        TODO */

        int serLow_rxStreamPop(ref Byte c)
        {
            int r = 0;
            Byte[] oneBytebuf = new Byte[1];

            if (serLow == null)
                return (-1);
            if (serLow.IsOpen == false)
                return(-1);
            
            if (serLow.BytesToRead > 0)
            {
                serLow.Read(oneBytebuf, 0, 1);
                c = oneBytebuf[0];
                r = 1;
            }
        
            return (r);
        }


        Int32 Is_0x01Frame(Byte[] data)
        {
            Int32 r = 0;
            int i;

            //Byte cmd = 0x58; //CMD_GETSERIAL; CMD_01_0x58
            //Byte subcmd = 0;
            UInt16 dataSize;

            UInt16 cs_calc;
            UInt16 cs_pkt;

            if (data.Length < 7)
                return (-42);

            if (data[0] != 0x01)
                return (-42);

            dataSize = data[3];
            dataSize += (UInt16)(data[4] * 256);

            if ((dataSize + 7) > data.Length)
                return (-42);

            cs_calc = 0;
            for (i = 0; i < (dataSize + 5)/*(rxPkt.Length - 2)*/; i++)
            {
                cs_calc += data[i];
            }

            cs_pkt = data[5 + dataSize + 0];
            cs_pkt += (UInt16)(data[5 + dataSize + 1] * 256);

            if (cs_pkt != cs_calc)
                return (-42);

            return (r);
        }




        Int32 sendToBLE_( Byte [] _rxB, UInt16 _rxB_rdPtr, UInt16 _rxB_wrPtr)
        {
            Int32 r = 0;
            UInt16 i;
            UInt16 dataSize = (UInt16)(_rxB_wrPtr - _rxB_rdPtr);
            byte[] pktbuf = new byte[dataSize]; //array<System::Byte,1> pktbuf = new array<Byte,1>(dataSize + 7);
            for (i = 0; i < dataSize; i++)
                pktbuf[i] = _rxB[_rxB_rdPtr + i];

            r = udEngine.Dn_Send_CMD_11_Pkt(pktbuf);

            return (r);
        }


        enum epktType_t
        {
            ePkt_Unknown,
            ePkt_0xC0,
            ePkt_0x01,
            ePkt_T2,
            ePkt_K2,
        };

        epktType_t guess_PktType = epktType_t.ePkt_Unknown;

        Byte[] rxB = new Byte[4096];
        UInt16 rxB_rdPtr = 0;
        UInt16 rxB_wrPtr = 0;

        bool g_Flag_frame01_rx_done = false;
        bool g_Flag_frameT2_rx_done = false;

        Byte[] txB = new Byte[4096];
        UInt16 txB_rdPtr = 0;
        UInt16 txB_wrPtr = 0;

        bool g_Flag_frame01_tx_ready = false;
        bool g_Flag_frameT2_tx_ready = false;


        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ////////// common /////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        //-----------------------------------------------------------------------------
        UInt16 get_checksum( Byte [] buf, UInt16 rp, UInt16 wp)
        {
            UInt16 cs = 0;
            UInt16 idx;

            for (idx = rp; idx < wp; idx++)
                cs += buf[idx];
            return (cs);
        }
        //-----------------------------------------------------------------------------
        // returns :  1 - Complete
        //            0 - Not complete
        //           -1 - Packet type error
        //           -2 - CheckSum error
        //-----------------------------------------------------------------------------
        Int32 procT2K2_checkPacketComplete(Byte [] buf, UInt16 rp, UInt16 wp, Byte TorK)
        {
            UInt16 size;
            UInt16 csum;
            UInt16 csum_calc;

            //printf("procT2K2_checkPacketComplete: buf = 0x%08x, rp = %d, wp = %d, TorK = %c\n", (int)buf, rp, wp, TorK );

            if ((rp + 1) >= wp)
                return (0);
            if (buf[rp + 0] != TorK)
                return (-1);
            if (buf[rp + 1] != '2')
                return (-1);

            if (rp + 6 > wp)
                return (0);

            size = (UInt16)(buf[rp + 3] << 8);
            size += buf[rp + 2];

            if ((rp + 4 + size + 2) > wp)
                return (0);

            csum = (UInt16)(buf[rp + 4 + size + 1] << 8);
            csum += buf[rp + 4 + size + 0];
            csum_calc = get_checksum(buf, (UInt16)(rp + 4), (UInt16)(rp + 4 + size));

            if (csum == csum_calc)
                return (1);

            return (-2);
        }


        //-----------------------------------------------------------------------------
        // returns :  1 - Complete
        //            0 - Not complete
        //           -1 - Packet type error
        //           -2 - CheckSum error
        //-----------------------------------------------------------------------------
        Int32 proc01_checkPacketComplete(Byte[] buf, UInt16 rp, UInt16 wp)
        {
            UInt16 size;
            UInt16 csum;
            UInt16 csum_calc;

            //printf("proc01_checkPacketComplete: buf = 0x%08x, rp = %d, wp = %d\n", (int)buf, rp, wp );

            if (rp == wp)
                return (0);
            if (buf[rp + 0] != 0x01)
                return (-1);
            if (rp + 7 > wp)
                return (0);

            size = (UInt16)(buf[rp + 4] << 8);
            size += buf[rp + 3];

            if ((rp + 5 + size + 2) > wp)
                return (0);

            csum = (UInt16)(buf[rp + 5 + size + 1] << 8);
            csum += buf[rp + 5 + size + 0];
            csum_calc = get_checksum(buf, (UInt16)(rp + 0), (UInt16)(rp + 5 + size));

            if (csum == csum_calc)
                return (1);

            return (-2);
            //return(r);
        }


        Int32 blk_uart_On_UartRx(Byte c)
        {
            Int32 r;
            r = 0;

            //printf("blk_uart_On_UartRx: c = 0x%02x, wp = %d\n", c, uartUni_buffer->wrPtr);
            /*
            if (uartUni_buffer->wrPtr > 20)
            {
                printf("FAKE HALT\n");
                printf(" FAKE HALT\n");
                printf("  FAKE HALT\n");
                printf("   FAKE HALT\n");

                while (1) ;
            }
            */

            //=================
            //=====  BIG  =====
            //=================
            if (guess_PktType == epktType_t.ePkt_0x01)
            {
                rxB[rxB_wrPtr++] = c;
                r = proc01_checkPacketComplete(rxB, rxB_rdPtr, rxB_wrPtr);
                if (r == 1)
                {
                    g_Flag_frame01_rx_done = true;
                    /*
                     * proc01_process_rxframe( (frame01_t*) &uartRxB_buffer[uartRxB_rp] );
                     * uartRxB_rp = 0;
                     * uartRxB_wp = 0;
                     * */
                }
                if (r < 0)
                {
                    //printf("01 BAD PACKET\n");
                    //printf("01 BAD PACKET\n");
                }
            }
            if (guess_PktType == epktType_t.ePkt_T2)
            {
                rxB[rxB_wrPtr++] = c;
                r = proc01_checkPacketComplete(rxB, rxB_rdPtr, rxB_wrPtr);
                r = procT2K2_checkPacketComplete(rxB, rxB_rdPtr, rxB_wrPtr, (Byte)'T');
                if (r == 1)
                {
                    g_Flag_frameT2_rx_done = true;
                    /*
                     * procT2_process_rxframe( (frameT2_t*) &uartRxB_buffer[uartRxB_rp] );
                     * uartRxB_rp = 0;
                     * uartRxB_wp = 0;
                     * */
                }
                if (r < 0)
                {
                    //printf("T2 BAD PACKET\n");
                    //printf("T2 BAD PACKET\n");
                }
            }

            //=================
            //===== SMALL =====
            //=================
            /*
            if( guess_PktType ==  epktType_t.ePkt_K2)
            {
                r = blk_uart_uartRxS_bufferPush( c );    
                r = procT2K2_checkPacketComplete( uartRxS_buffer, uartRxS_rp, uartRxS_wp, 'K' );
            }
            if( guess_PktType ==  epktType_t.ePkt_0xC0)
            {
                r = blk_uart_uartRxS_bufferPush( c );    
                r = procC0_checkPacketComplete( uartRxS_buffer, uartRxS_rp, uartRxS_wp );
            }
            */
            return (r);
        }


        
        Byte m_uartRx_c0 = 0;
        Byte m_uartRx_c1 = 0;
        Int32 processInputStreamISR()
        {
            Byte c = 0;
            Int32 r;

            do
            {
                //err_code = app_uart_get(&c);
                //if( err_code != NRF_SUCCESS)
                //    break;
                r = serLow_rxStreamPop(ref c); // r = maikonC_rxStreamPop(&c);
                if (r != 1)
                    break;
                r = 0;
                if (guess_PktType == epktType_t.ePkt_Unknown)
                {
                    m_uartRx_c1 = m_uartRx_c0;
                    m_uartRx_c0 = c;
                    if (m_uartRx_c0 == 0xC0)
                    {
                        guess_PktType = epktType_t.ePkt_0xC0;
                        blk_uart_On_UartRx(0xC0);
                    }
                    if (m_uartRx_c0 == 0x01)
                    {
                        guess_PktType = epktType_t.ePkt_0x01;
                        blk_uart_On_UartRx(0x01);
                    }
                    if ((m_uartRx_c1 == (Byte)'T') && (m_uartRx_c0 == (Byte)'2'))
                    {
                        guess_PktType = epktType_t.ePkt_T2;
                        blk_uart_On_UartRx((Byte)'T'); blk_uart_On_UartRx((Byte)'2');
                    }
                    if ((m_uartRx_c1 == 'K') && (m_uartRx_c0 == '2'))
                    {
                        guess_PktType = epktType_t.ePkt_K2;
                        blk_uart_On_UartRx((Byte)'K'); blk_uart_On_UartRx((Byte)'2');
                    }
                }
                else
                {
                    r = blk_uart_On_UartRx(c);
                    if (r == 1)
                        break;
                }

            } while (true);

            return (42);//( r );
        }


        Int32 proc_otherProcessing()
        {
            Int32 r = 0;
            return (r);
        }

        /*
        Int32 pullTxBuffer(Byte* pb)
        {
            Int32 r;
            r = 0;
            if (uartUni_buffer->rdPtr < uartUni_buffer->wrPtr)
            {
                *pb = uartUni_buffer->buffer[uartUni_buffer->rdPtr++];
                r = 1;
            }
            if (uartUni_buffer->rdPtr == uartUni_buffer->wrPtr)
            {
                uartUni_buffer->rdPtr = 0;
                uartUni_buffer->wrPtr = 0;
            }

            return (r);
        }
        */
        Int32 processOutputStreamISR()
        {
            Int32 r = 0;
            //Byte c;
            //Int32 debugCount = 0;

            if (g_Flag_frame01_tx_ready)
            {
                if (serLow != null)
                {
                    if( serLow.IsOpen == true)
                        serLow.Write(txB, txB_rdPtr, (txB_wrPtr - txB_rdPtr));
                }
                g_Flag_frame01_tx_ready = false;
                txB_rdPtr = 0;
                txB_wrPtr = 0;
                /*
                do
                {
                    r = pullTxBuffer(&c);
                    if (r == 1)
                    {
                        debugCount++;
                        r = maikonC_txStreamPush(c);
                        if (r == 0)
                            break;
                    }
                    else
                    {
                        uartUni_buffer->rdPtr = 0;
                        uartUni_buffer->wrPtr = 0;
                        g_Flag_frame01_tx_ready = false;
                    }
                } while (r != 0);
                */
            }

            if (g_Flag_frameT2_tx_ready)
            {
                if (serLow != null)
                {
                    if (serLow.IsOpen == true)
                        serLow.Write(txB, txB_rdPtr, (txB_wrPtr - txB_rdPtr));
                }
                g_Flag_frameT2_tx_ready = false;
                txB_rdPtr = 0;
                txB_wrPtr = 0;
                /*
                do
                {
                    r = pullTxBuffer(&c);
                    if (r == 1)
                    {
                        debugCount++;
                        r = maikonC_txStreamPush(c);
                        if (r == 0)
                            break;
                    }
                    else
                    {
                        uartUni_buffer->rdPtr = 0;
                        uartUni_buffer->wrPtr = 0;
                        g_Flag_frameT2_tx_ready = false;
                    }
                } while (r != 0);
                */
            }

            //printf("processOutputStreamISR: debugCount = %d", debugCount);
            //printf(", rp = %d, wp = %d, MAX = %d\n", g_txVStream_rp, g_txVStream_wp, TX_VSTREAM_MAX);

            //TX_VSTREAM_MAX
            //g_txVStream_rp;
            //g_txVStream_wp

            return (r);
        }

        Int32 serLow_OnBLEupPacket(byte[] theUpPacket)
        {
            Buffer.BlockCopy(theUpPacket, 0, txB, 0, theUpPacket.Length);

            txB_rdPtr = 0;
            txB_wrPtr = (UInt16)(theUpPacket.Length); // ?? -2

            g_Flag_frameT2_tx_ready = true;
            
            maikonC_main_proc();

            return (42);
        }

        //=============================================================================
        int maikonC_main_proc()
        {
            int r = 0;

            r = processInputStreamISR();

            if (g_Flag_frame01_rx_done)
            {
                g_Flag_frame01_rx_done = false;
                guess_PktType =epktType_t.ePkt_Unknown;
                //proc01_process_rxframe(uartUni_buffer);
                sendToBLE_(rxB, rxB_rdPtr, rxB_wrPtr);
                rxB_rdPtr = 0;
                rxB_wrPtr = 0;
            }

            if (g_Flag_frameT2_rx_done)
            {
                g_Flag_frameT2_rx_done = false;
                guess_PktType = epktType_t.ePkt_Unknown;
                //procT2_process_rxframe(uartUni_buffer);
                sendToBLE_(rxB, rxB_rdPtr, rxB_wrPtr);
                rxB_rdPtr = 0;
                rxB_wrPtr = 0;
            }


            proc_otherProcessing();

            r = processOutputStreamISR();

            return (r);
        }




    }
}
