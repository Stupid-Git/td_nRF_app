﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

using TDnRFLib;

namespace Zed
{
    class Protocol_01
    {
        public event EventHandler<OutputReceivedEventArgs> userEv_LogMessage;


        // 送受信バッファ
        public Byte[] sData = new Byte[1024 * 10 + 128];
        public Byte[] rData = new Byte[1024 * 10 + 128];
        public Byte[] tData = new Byte[1024 * 10];

        public Protocol_01()
        {
        }

        //public event EventHandler<TDnRFLib.OutputReceivedEventArgs> userEv_LogMessage;

        public void richTextBox1_AppendText(String s)
        {
            if (userEv_LogMessage != null)
                userEv_LogMessage(this, new OutputReceivedEventArgs(s));
            //Console.WriteLine(s);
        }


        UInt16 /*proto_01_*/getChecksum(Byte[] buffer)
        {
            //int r = 0;
            UInt16 i, len, sum;

            len = (UInt16)(buffer[3] + (UInt16)buffer[4] * 256);

            // 0x01 proto checksum is from [0] to [(len + 5) - 1]
            for (sum = 0, i = 0; i < (5 + len); i++)
            {
                sum += buffer[i];
            }
            return (sum);
        }

        bool /*proto_01_*/checkChecksum(Byte[] buffer)
        {
            //int r = 0;
            UInt16 i, len, sum;

            len = (UInt16)(buffer[3] + (UInt16)buffer[4] * 256);

            // 0x01 proto checksum is from [0] to [(len + 5) - 1]
            for (sum = 0, i = 0; i < len + 5; i++)
            {
                sum += buffer[i];
            }
            if (
                (buffer[len + 5] != (Byte)sum) ||
                (buffer[len + 6] != (Byte)(sum / 256))
            )
            {
                return (false);
            }
            return (true);
        }

        Byte[] /*proto_01_*/newPktwithChecksum(Byte[] buffer)
        {
            //int r = 0;
            UInt16 i, len, sum;
            Byte[] pkt;

            len = (UInt16)(buffer[3] + (UInt16)buffer[4] * 256);
            pkt = new Byte[len + 7];
            Buffer.BlockCopy(buffer, 0, pkt, 0, len + 5);

            // 0x01 proto checksum is from [0] to [(len + 5) - 1]
            for (sum = 0, i = 0; i < (len + 5); i++)
            {
                sum += pkt[i];
            }
            pkt[len + 5] = (Byte)sum;
            pkt[len + 6] = (Byte)(sum / 256);

            return (pkt);
        }

        public void CMD_print(String s, Byte[] P, UInt16 CMDlen)
        {
            UInt32 i;
            String S1 = "";
            String S2 = "";
            char C;
            UInt32 wpl; // White Space Length

            Console.Write("{0}\n", s);

            wpl = (16 * 3) + 8;
            for (i = 0; i < CMDlen; i++)
            {
                S1 += String.Format("{0:X2} ", P[i]); //Console.Write("{0:X2} ", P[i]);
                wpl -= 3;

                C = (char)P[i];
                if (Char.IsLetterOrDigit(C))
                    S2 += String.Format("{0}", C);
                else
                    S2 += ".";

                if (((i % 16) == 15) || ((i + 1) == CMDlen))
                {
                    while (wpl-- > 0)
                    {
                        S1 += " "; // Console.Write(" ");
                    }
                    Console.Write(S1);
                    Console.Write(S2);
                    Console.Write("\n");
                    S1 = "";
                    wpl = (16 * 3) + 8;
                    S2 = "";
                }
            }
            Console.Write("\n");
        }

        public byte[] CMD_58_get_send_packet()
        {
            //int r;
            byte[] pkt;
            //UInt16 i, len, sum;
            UInt16 pktLen;

            rData[0] = 0x00;

            
            sData[0] = 0x01;
            sData[1] = 0x58;
            sData[2] = 0x06;
            sData[3] = 0x04;
            sData[4] = 0x00;

            sData[5] = 0x00;
            sData[6] = 0x00;
            sData[7] = 0x00;
            sData[8] = 0x00;

            pkt = newPktwithChecksum(sData);

            pktLen = (UInt16)(pkt.Length);
            CMD_print("CMD_54_get_send_packet TX packet", pkt, pktLen);
            

            sData[0] = 0x01;
            sData[1] = 0x58;
            sData[2] = 0x00;
            sData[3] = 0x00;
            sData[4] = 0x00;

            pkt = newPktwithChecksum(sData);

            pktLen = (UInt16)(pkt.Length);
            CMD_print("CMD_58_get_send_packet TX packet", pkt, pktLen);


            return (pkt);
        }

        public void CMD_58_process_recv_packet(Byte[] pkt, UInt16 pktLen)
        {
            int i;

            //for (i = 0; i < pkt.Length; i++)
            for (i = 0; i < pktLen; i++)
                rData[i] = pkt[i];

            if( !checkChecksum(rData) )
            {
                Console.WriteLine("Checksum error");
            }

            CMD_print("CMD_58_process_recv_packet RX packet", pkt, pktLen);


        }

        //=====================================================================
        public byte[] CMD_33_00_get_send_packet()
        {
            byte[] pkt;
            UInt16 pktLen;

            rData[0] = 0x00;


            sData[0] = 0x01;
            sData[1] = 0x33;
            sData[2] = 0x00;
            sData[3] = 0x04;
            sData[4] = 0x00;

            sData[5] = 0x00;
            sData[6] = 0x00;
            sData[7] = 0x00;
            sData[8] = 0x00;

            pkt = newPktwithChecksum(sData);

            pktLen = (UInt16)(pkt.Length);
            CMD_print("CMD_33_00_get_send_packet TX packet", pkt, pktLen);


            return (pkt);
        }

        public void CMD_33_00_process_recv_packet(Byte[] pkt, UInt16 pktLen)
        {
            int i;

            //for (i = 0; i < pkt.Length; i++)
            for (i = 0; i < pktLen; i++)
                rData[i] = pkt[i];

            if (!checkChecksum(rData))
            {
                Console.WriteLine("Checksum error");
            }

            CMD_print("CMD_33_00_process_recv_packet RX packet", pkt, pktLen);


        }
        //=====================================================================
        public byte[] CMD_48_00_get_send_packet()
        {
            byte[] pkt;
            UInt16 pktLen;

            rData[0] = 0x00;


            sData[0] = 0x01;
            sData[1] = 0x48;
            sData[2] = 0x00;
            sData[3] = 0x04;
            sData[4] = 0x00;

            sData[5] = 0x00;
            sData[6] = 0x00;
            sData[7] = 0x00;
            sData[8] = 0x00;

            pkt = newPktwithChecksum(sData);

            pktLen = (UInt16)(pkt.Length);
            CMD_print("CMD_48_00_get_send_packet TX packet", pkt, pktLen);


            return (pkt);
        }

        public void CMD_48_00_process_recv_packet(Byte[] pkt, UInt16 pktLen)
        {
            int i;

            //for (i = 0; i < pkt.Length; i++)
            for (i = 0; i < pktLen; i++)
                rData[i] = pkt[i];

            if (!checkChecksum(rData))
            {
                Console.WriteLine("Checksum error");
            }

            CMD_print("CMD_48_00_process_recv_packet RX packet", pkt, pktLen);


        }


        //=====================================================================
        public byte[] CMD_69_01_get_send_packet()
        {
            byte[] pkt;
            UInt16 pktLen;

            rData[0] = 0x00;


            sData[0] = 0x01;
            sData[1] = 0x69;
            sData[2] = 0x01;
            sData[3] = 0x04;
            sData[4] = 0x00;

            sData[5] = 0x00;
            sData[6] = 0x00;
            sData[7] = 0x00;
            sData[8] = 0x00;

            pkt = newPktwithChecksum(sData);

            pktLen = (UInt16)(pkt.Length);
            CMD_print("CMD_69_01_get_send_packet TX packet", pkt, pktLen);


            return (pkt);
        }

        public void CMD_69_01_process_recv_packet(Byte[] pkt, UInt16 pktLen)
        {
            int i;

            //for (i = 0; i < pkt.Length; i++)
            for (i = 0; i < pktLen; i++)
                rData[i] = pkt[i];

            if (!checkChecksum(rData))
            {
                Console.WriteLine("Checksum error");
            }

            CMD_print("CMD_69_01_process_recv_packet RX packet", pkt, pktLen);


        }

        //=====================================================================
        public byte[] CMD_9E_get_send_packet()
        {
            //int r;
            byte[] pkt;
            //UInt16 i, len, sum;
            UInt16 pktLen;

            rData[0] = 0x00;

            /*
            sData[0] = 0x01;
            sData[1] = 0x9E;
            sData[2] = 0x06;
            sData[3] = 0x04;
            sData[4] = 0x00;

            sData[5] = 0x00;
            sData[6] = 0x00;
            sData[7] = 0x00;
            sData[8] = 0x00;
            
            pkt = newPktwithChecksum(sData);

            pktLen = (UInt16)(pkt.Length);
            CMD_print("CMD_54_get_send_packet TX packet", pkt, pktLen);
            */

            sData[0] = 0x01;
            sData[1] = 0x9E;
            sData[2] = 0x00;
            sData[3] = 0x00;
            sData[4] = 0x00;

            pkt = newPktwithChecksum(sData);

            pktLen = (UInt16)(pkt.Length);
            CMD_print("CMD_9E_get_send_packet TX packet", pkt, pktLen);


            return (pkt);
        }

        public void CMD_9E_process_recv_packet(Byte[] pkt, UInt16 pktLen)
        {
            int i;

            //for (i = 0; i < pkt.Length; i++)
            for (i = 0; i < pktLen; i++)
                rData[i] = pkt[i];

            if (!checkChecksum(rData))
            {
                Console.WriteLine("Checksum error");
            }

            CMD_print("CMD_9E_process_recv_packet RX packet", pkt, pktLen);


        }

        //=====================================================================
        public byte[] CMD_9E_01_get_send_packet()
        {
            byte[] pkt;
            UInt16 pktLen;

            rData[0] = 0x00;


            sData[0] = 0x01;
            sData[1] = 0x9E;
            sData[2] = 0x01;
            sData[3] = 0x00;
            sData[4] = 0x00;

            pkt = newPktwithChecksum(sData);

            pktLen = (UInt16)(pkt.Length);
            CMD_print("CMD_9E_01_get_send_packet TX packet", pkt, pktLen);


            return (pkt);
        }

        public void CMD_9E_01_process_recv_packet(Byte[] pkt, UInt16 pktLen)
        {
            int i;

            //for (i = 0; i < pkt.Length; i++)
            for (i = 0; i < pktLen; i++)
                rData[i] = pkt[i];

            if (!checkChecksum(rData))
            {
                Console.WriteLine("Checksum error");
            }

            CMD_print("CMD_9E_01_process_recv_packet RX packet", pkt, pktLen);


        }

        public Byte[] wrapWith9F(Byte[] rawPacket, UInt32 torokuCode)
        {
            Byte[] pkt9F;
            int i;
            UInt16 sum;
            UInt16 len;
            UInt16 rawLen;

            /*
             * 01, 9F, 00, LL, LL, [nn, nn, nn, nn], [rawPacket], CS, CS
             * Size of 9F  packet is 5 + 4 + rawPacket-length + 2
             */
            len = (UInt16)(4 + rawPacket.Length);
            pkt9F = new Byte[5 + 4 + rawPacket.Length + 2];
            pkt9F[0] = 0x01;
            pkt9F[1] = 0x9F;
            pkt9F[2] = 0x22; //0x00;
            pkt9F[3] = (Byte)(len & 0x00ff);
            pkt9F[4] = (Byte)((len>>8)&0x00ff);
            
            pkt9F[5] = (Byte)((torokuCode>>0)  & 0x000000ff); // security LSB
            pkt9F[6] = (Byte)((torokuCode>>8)  & 0x000000ff);
            pkt9F[7] = (Byte)((torokuCode>>16) & 0x000000ff);
            pkt9F[8] = (Byte)((torokuCode>>24) & 0x000000ff); // security LSB

            rawLen = (UInt16)(rawPacket.Length);
            for (i = 0; i < rawLen; i++)
                pkt9F[9 + i] = rawPacket[i];

            sum = getChecksum(pkt9F);

            pkt9F[5 + len + 0] = (Byte)(sum & 0x00ff);
            pkt9F[5 + len + 1] = (Byte)((sum >> 8) & 0x00ff);

            return (pkt9F);
        }

    }
}
