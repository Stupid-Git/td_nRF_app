using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDnRF
{
    class Protocol_01
    {

        public Byte[] makeReqPkt_0x58()
        {
            int i;
            byte[] buf = new byte[20]; // array<Byte,1> buf = new array<Byte,1>(20);

            Byte cmd = 0x58; //CMD_GETSERIAL; CMD_01_0x58
            Byte subcmd = 0;
            UInt16 dataSize = 0;

            byte[] pktbuf = new byte[dataSize + 7]; //array<System::Byte,1> pktbuf = new array<Byte,1>(dataSize + 7);
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


            return (pktbuf);
        }

        /*public*/ Int32 procRspPkt_Check(Byte[] rxPkt)
        {
            Int32 r = 0;
            int i;
            byte[] buf = new byte[20]; // array<Byte,1> buf = new array<Byte,1>(20);

            //Byte cmd = 0x58; //CMD_GETSERIAL; CMD_01_0x58
            //Byte subcmd = 0;
            UInt16 dataSize;

            UInt16 cs_calc;
            UInt16 cs_pkt;

            if (rxPkt.Length < 7)
                return (-42);

            if (rxPkt[0] != 0x01)
                return (-42);

            dataSize = rxPkt[3];
            dataSize += (UInt16)(rxPkt[4] * 256);

            if( (dataSize + 7) > rxPkt.Length)
                return (-42);

            cs_calc = 0;
            for (i = 0; i < (dataSize + 5)/*(rxPkt.Length - 2)*/; i++)
            {
                cs_calc += rxPkt[i];
            }

            cs_pkt = rxPkt[5 + dataSize + 0];
            cs_pkt += (UInt16)(rxPkt[5 + dataSize + 1] * 256);

            if( cs_pkt != cs_calc)
                return (-42);

            return (r);
        }

        public Int32 procRspPkt_0x58(Byte[] rxPkt, ref UInt32 serialNumber)
        {
            Int32 r = 0;
            //Byte cmd = 0x58; //CMD_GETSERIAL; CMD_01_0x58
            //Byte subcmd = 0;
            UInt16 dataSize = 0;

            serialNumber = 0;

            r = procRspPkt_Check(rxPkt);
            if (r != 0)
                return (r);

            if (rxPkt[1] != 0x58)
                return (-42);
            if (rxPkt[2] != 0x06)
                return (-42);

            dataSize = rxPkt[3];
            dataSize += (UInt16)(rxPkt[4] * 256);
            if( dataSize != 4)
                return (-42);

            UInt32 serialNo;

            serialNo = 0;

            serialNo |= (UInt32)((rxPkt[5] << 0)); // LSB
            serialNo |= (UInt32)((rxPkt[6] << 8)); // 
            serialNo |= (UInt32)((rxPkt[7] << 16)); // 
            serialNo |= (UInt32)((rxPkt[8] << 24)); // MSB
            
            serialNumber = serialNo;

            return (r);
        }

    }
}
