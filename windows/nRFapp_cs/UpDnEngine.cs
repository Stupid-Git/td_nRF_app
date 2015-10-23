using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nordicsemi;

namespace nRFUart_TD
{
    public class UpDnEngine
    {
        public    Nordicsemi.MasterEmulator masterEmulator;
        PipeSetup pipeSetup;


        public void UpDnEngine_Setup(Nordicsemi.MasterEmulator master, PipeSetup pipe)
        {
            masterEmulator = master;
            pipeSetup = pipe;
        }
        
        public UpDnEngine()
        {
        }

        //=====================================================================
        //=====================================================================
        //===== DN ============================================================
        //=====================================================================
        //=====================================================================
        Int32 Dn_Enable()
        {
            return(0);
        }

        Int32 Dn_Send_CMD_12()
        {
            byte[] buf = new byte[20]; // array<Byte,1> buf = new array<Byte,1>(20);
            Int32 r;
            //--- Make the command ---
            buf[0] = 1;
            buf[1] = 2;
            r = Dn_Send_Dcmd(buf, 20);
            return(r);
        }

        Int32 Dn_Send_CMD_13()
        {
            byte[] buf = new byte[20]; // array<Byte,1> buf = new array<Byte,1>(20);
            Int32 r;
            //--- Make the command ---
            buf[0] = 1;
            buf[1] = 3;
            r = Dn_Send_Dcmd(buf, 20);
            return(r);
        }


        Int32 Dn_Send_CMD_11_Pkt(byte [] pktbuf)//array<System::Byte,1> pktbuf)
        {            
            int  i;    
            byte[] buf = new byte[20]; // array<Byte,1> buf = new array<Byte,1>(20);
            //Byte length;
            Byte  blk_no;    
            UInt16 ofst;
            
            Int16 totalLength;
            Int16 total_cs;
            Byte total_cs0;
            Byte total_cs1;

            Int32 r;

            totalLength = (Int16)(pktbuf.Length + 2);
            total_cs = 0;
            for (i = 0; i < pktbuf.Length; i++)
            {
                total_cs += pktbuf[i];
            }
            total_cs0 = (Byte)((total_cs >> 0) & 0x00FF);
            total_cs1 = (Byte)((total_cs >> 8) & 0x00FF);
            
            //--- Make the command ---
            buf[0] = 1;
            buf[1] = 1;
            //buf[2] = (pktbuf.Length >> 0) & 0x00ff;
            //buf[3] = (pktbuf.Length >> 8) & 0x00ff;
            buf[2] = (Byte)(((totalLength) >> 0) & 0x00ff);
            buf[3] = (Byte)(((totalLength) >> 8) & 0x00ff);
            r = Dn_Send_Dcmd(buf, 20);

            ofst = 0;
            blk_no = 0;
            do
            {
                //--- Set the Data ---
                ofst = (UInt16)(blk_no * 16);
                if( ofst > pktbuf.Length)
                    break;

                buf[0] = blk_no++;
                buf[1] = 0x00;
                buf[2] = 0x00;
                buf[3] = 0x00;
                for(i=0; i<16; i++)
                {
                    if ((ofst + i) < pktbuf.Length)
                        buf[4 + i] = pktbuf[ofst + i];
                    else
                        if ((ofst + i) == (totalLength - 2))
                            buf[4 + i] = total_cs0;
                        else
                            if ((ofst + i) == (totalLength - 1))
                                buf[4 + i] = total_cs1;
                            else
                                buf[4 + i] = 0x00;
                }
                /* OLD
                for(i=0; i<16; i++)
                    if(ofst +i < pktbuf.Length)
                        buf[4 + i] = pktbuf[ofst + i];
                    else
                        buf[4 + i] = 0x00;
                */
                //length = 20;
                r = Dn_Send_Ddat(buf, 20);
                if(r != 0)
                    break;

            } while(true);

            return(0);
        }
#if true //TODO
        //---------------------------------------------------------------------
        Int32 Dn_Send_T2_DummyTEST1()
        {
            int  i;    
            byte[] buf = new byte[20]; // array<Byte,1> buf = new array<Byte,1>(20);

            Int16 dataSize = 10;

            // fake make packet
            byte[] pktbuf = new byte[dataSize + 6]; // array<System::Byte,1> pktbuf = new array<Byte,1>(dataSize + 6);
            Int16 cs_pkt;

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
            for( i=4; i<pktbuf.Length - 2 ; i++)
            {
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf.Length - 2] = (byte)((cs_pkt >> 0) & 0x00FF);
            pktbuf[pktbuf.Length - 1] = (byte)((cs_pkt >> 8) & 0x00FF);


            Dn_Send_CMD_11_Pkt(pktbuf);
            return(0);
        }

        //---------------------------------------------------------------------
        public Int32 Dn_Send_0x01_CMD_01_0x58()
        {
            int  i;    
            byte[] buf = new byte[20]; // array<Byte,1> buf = new array<Byte,1>(20);

            Byte cmd = 0x58; //CMD_GETSERIAL; CMD_01_0x58
            Byte subcmd = 0;
            Int16 dataSize = 0;

            // fake make packet
            byte[] pktbuf = new byte[7]; //array<System::Byte,1> pktbuf = new array<Byte,1>(7);
            Int16 cs_pkt;

            pktbuf[0] = 0x01;
            pktbuf[1] = cmd;
            pktbuf[2] = subcmd;
            pktbuf[3] = (byte)((dataSize >> 0) & 0x00FF); //Len LSB
            pktbuf[4] = (byte)((dataSize >> 8) & 0x00FF); //Len MSB

            cs_pkt = 0;
            for( i=0; i<pktbuf.Length - 2 ; i++)
            {
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf.Length - 2] = (byte)((cs_pkt >> 0) & 0x00FF);
            pktbuf[pktbuf.Length - 1] = (byte)((cs_pkt >> 8) & 0x00FF);


            Dn_Send_CMD_11_Pkt(pktbuf);
            return(0);
        }

        //---------------------------------------------------------------------
        public Int32 Dn_Send_0x01_CMD_01_0xF5(UInt16 recCount)
        {
            int  i;    
            Byte cmd = 0xF5; //CMD_01_0xF5;
            Byte subcmd = 0;
            Int16 dataSize = 2;
            Int16 cs_pkt;

            byte[] pktbuf = new byte[dataSize + 7]; //array<System::Byte,1> pktbuf = new array<Byte,1>(dataSize + 7);

            pktbuf[0] = 0x01;
            pktbuf[1] = cmd;
            pktbuf[2] = subcmd;
            pktbuf[3] = (byte)((dataSize >> 0) & 0x00FF); //Len LSB
            pktbuf[4] = (byte)((dataSize >> 8) & 0x00FF); //Len MSB
            pktbuf[5] = (byte)((recCount >> 0) & 0x00FF); //Len LSB
            pktbuf[6] = (byte)((recCount >> 8) & 0x00FF); //Len MSB

            cs_pkt = 0;
            for( i=0; i<pktbuf.Length - 2 ; i++)
            {
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf.Length - 2] = (byte)((cs_pkt >> 0) & 0x00FF);
            pktbuf[pktbuf.Length - 1] = (byte)((cs_pkt >> 8) & 0x00FF);


            Dn_Send_CMD_11_Pkt(pktbuf);
            return(0);
        }
        
        //---------------------------------------------------------------------
        public Int32 Dn_Send_0x01_CMD_01_0xF8()
        {
            int  i;    
            Byte cmd = 0xF8; //CMD_01_0xF8;
            Byte subcmd = 0;
            Int16 dataSize = 0;
            Int16 cs_pkt;

            byte[] pktbuf = new byte[dataSize + 7]; //array<System::Byte,1> pktbuf = new array<Byte,1>(dataSize + 7);

            pktbuf[0] = 0x01;
            pktbuf[1] = cmd;
            pktbuf[2] = subcmd;
            pktbuf[3] = (byte)((dataSize >> 0) & 0x00FF); //Len LSB
            pktbuf[4] = (byte)((dataSize >> 8) & 0x00FF); //Len MSB

            cs_pkt = 0;
            for( i=0; i<pktbuf.Length - 2 ; i++)
            {
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf.Length - 2] = (byte)((cs_pkt >> 0) & 0x00FF);
            pktbuf[pktbuf.Length - 1] = (byte)((cs_pkt >> 8) & 0x00FF);


            Dn_Send_CMD_11_Pkt(pktbuf);
            return(0);
        }

        //---------------------------------------------------------------------
        public Int32 Dn_Send_0x01_CMD_01_0xF9(Int32 size)
        {
            int  i;    
            Byte cmd = 0xF9; //CMD_01_0xF9
            Byte subcmd = 0;
            Int16 dataSize = 2;
            Int16 cs_pkt;

            byte[] pktbuf = new byte[dataSize + 7]; // array<System::Byte,1> pktbuf = new array<Byte,1>(dataSize + 7);

            pktbuf[0] = 0x01;
            pktbuf[1] = cmd;
            pktbuf[2] = subcmd;
            pktbuf[3] = (byte)((dataSize >> 0) & 0x00FF); //Len LSB
            pktbuf[4] = (byte)((dataSize >> 8) & 0x00FF); //Len MSB

            pktbuf[5] = (byte)((size >> 0) &0x00ff);;
            pktbuf[6] = (byte)((size >> 8) &0x00ff);

            cs_pkt = 0;
            for( i=0; i<pktbuf.Length - 2 ; i++)
            {
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf.Length - 2] = (byte)((cs_pkt >> 0) & 0x00FF);
            pktbuf[pktbuf.Length - 1] = (byte)((cs_pkt >> 8) & 0x00FF);


            Dn_Send_CMD_11_Pkt(pktbuf);
            return(0);
        }

        //---------------------------------------------------------------------
        public Int32 Dn_Send_0x01_CMD_01_0x44(UInt16 byteCount)
        {
            int  i;    
            Byte cmd = 0x44; //CMD_01_0x44;
            Byte subcmd = 0;
            Int16 dataSize = 2;
            Int16 cs_pkt;

            byte[] pktbuf = new byte[dataSize + 7]; //array<System::Byte,1> pktbuf = new array<Byte,1>(dataSize + 7);

            pktbuf[0] = 0x01;
            pktbuf[1] = cmd;
            pktbuf[2] = subcmd;
            pktbuf[3] = (byte)((dataSize >> 0) & 0x00FF); //Len LSB
            pktbuf[4] = (byte)((dataSize >> 8) & 0x00FF); //Len MSB

            pktbuf[5] = (byte)((byteCount >> 0) & 0x00ff);
            pktbuf[6] = (byte)((byteCount >> 8) & 0x00ff);

            cs_pkt = 0;
            for( i=0; i<pktbuf.Length - 2 ; i++)
            {
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf.Length - 2] = (byte)((cs_pkt >> 0) & 0x00FF);
            pktbuf[pktbuf.Length - 1] = (byte)((cs_pkt >> 8) & 0x00FF);


            Dn_Send_CMD_11_Pkt(pktbuf);
            return(0);
        }

        //---------------------------------------------------------------------
        public Int32 Dn_Send_0x01_CMD_01_0x45(UInt16 blockNum)
        {
            int  i;    
            Byte cmd = 0x45; //CMD_01_0x45;
            Byte subcmd = 0x06;
            Int16 dataSize = 2;
            Int16 cs_pkt;

            byte[] pktbuf = new byte[dataSize + 7]; //array<System::Byte,1> pktbuf = new array<Byte,1>(dataSize + 7);

            pktbuf[0] = 0x01;
            pktbuf[1] = cmd;
            pktbuf[2] = subcmd;
            pktbuf[3] = (byte)((dataSize >> 0) & 0x00FF); //Len LSB
            pktbuf[4] = (byte)((dataSize >> 8) & 0x00FF); //Len MSB

            pktbuf[5] = (byte)((blockNum >> 0) & 0x00ff);
            pktbuf[6] = (byte)((blockNum >> 8) & 0x00ff);

            cs_pkt = 0;
            for( i=0; i<pktbuf.Length - 2 ; i++)
            {
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf.Length - 2] = (byte)((cs_pkt >> 0) & 0x00FF);
            pktbuf[pktbuf.Length - 1] = (byte)((cs_pkt >> 8) & 0x00FF);


            Dn_Send_CMD_11_Pkt(pktbuf);
            return(0);
        }


        //---------------------------------------------------------------------
        Int32 Dn_Dummy_Send42()
        {            
            int  i;
            byte[] buf = new byte[20]; //array<Byte,1> buf = new array<Byte,1>(20);
            // fake make packet
            byte[] pktbuf = new byte[42]; //array<System::Byte,1> pktbuf = new array<Byte,1>(42);
            Int16 cs_pkt;
            cs_pkt = 0;
            for( i=0; i<pktbuf.Length - 2 ; i++)
            {
                pktbuf[i] = (Byte)(i & 0x00FF);
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf.Length - 2] = (byte)((cs_pkt >> 0) & 0x00FF);
            pktbuf[pktbuf.Length - 1] = (byte)((cs_pkt >> 8) & 0x00FF);


            Dn_Send_CMD_11_Pkt(pktbuf);
            return(0);
        }


        //---------------------------------------------------------------------
        Int32 Dn_Dummy_Send43()
        {            
            int  i;    
            byte [] buf = new byte [20]; //array<Byte,1> buf = new array<Byte,1>(20);
            //Byte length;
            Byte  blk_no;    
            Int16 ofst;

            Int32 r;
            // fake make packet
            byte[] pktbuf = new byte[42]; //array<System::Byte,1> pktbuf = new array<Byte,1>(42);
            Int16 cs_pkt;
            cs_pkt = 0;
            for( i=0; i<pktbuf.Length - 2 ; i++)
            {
                pktbuf[i] = (Byte)(i & 0x00FF);
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf.Length - 2] = (byte)((cs_pkt >> 0) & 0x00FF);
            pktbuf[pktbuf.Length - 1] = (byte)((cs_pkt >> 8) & 0x00FF);



            //--- Make the command ---
            buf[0] = 1;
            buf[1] = 1;
            buf[2] = (byte)((pktbuf.Length >> 0) & 0x00ff);
            buf[3] = (byte)((pktbuf.Length >> 8) & 0x00ff);
            r = Dn_Send_Dcmd(buf, 20);

            ofst = 0;
            blk_no = 0;
            do
            {
                //--- Set the Data ---
                ofst = (Int16)(blk_no * 16);
                if( ofst > pktbuf.Length)
                    break;

                buf[0] = blk_no++;
                buf[1] = 0x00;
                buf[2] = 0x00;
                buf[3] = 0x00;
                for(i=0; i<16; i++)
                    if(ofst +i < pktbuf.Length)
                        buf[4 + i] = pktbuf[ofst + i];
                    else
                        buf[4 + i] = 0x00;

                //length = 20;
                if(blk_no!=2)
                {
                    r = Dn_Send_Ddat(buf, 20);
                    if(r != 0)
                        break;
                }

            } while(true);

            return(0);
        }
#endif
        //---------------------------------------------------------------------
        Int32 Dn_Dummy_Send43B()
        {            
            int  i;    
            byte [] buf = new byte [20];
            //Byte length;
            Byte  blk_no;    
            Int16 ofst;

            Int32 r;
            // fake make packet
            byte [] pktbuf = new byte [42]; //         array<System::Byte,1> pktbuf = new array<Byte,1>(42);
            Int16 cs_pkt;
            cs_pkt = 0;
            for( i=0; i<pktbuf.Length - 2 ; i++)
            {
                pktbuf[i] = (Byte)(i & 0x00FF);
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf.Length - 2] = (byte)((cs_pkt >> 0) & 0x00FF);
            pktbuf[pktbuf.Length - 1] = (byte)((cs_pkt >> 8) & 0x00FF);


            ofst = 0;
            blk_no = 0;
            do
            {
                //--- Set the Data ---
                ofst = (Int16)(blk_no * 16);
                if( ofst > pktbuf.Length)
                    break;

                buf[0] = blk_no++;
                buf[1] = 0x00;
                buf[2] = 0x00;
                buf[3] = 0x00;
                for(i=0; i<16; i++)
                    if(ofst +i < pktbuf.Length)
                        buf[4 + i] = pktbuf[ofst + i];
                    else
                        buf[4 + i] = 0x00;

                //length = 20;
                if(blk_no == 2)
                {
                    r = Dn_Send_Ddat(buf, 20);
                    if(r != 0)
                        break;
                }

            } while(true);

            return(0);
        }


        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
#if false //TODO
        Int32 UpDn_Write_Wctrl_test(array<System::Byte,1> p_buf, int len)
        {
            bool bret;
            Int32 r;
            r = 0;

            Console.WriteLine("UpDn_Write_Wctrl_test...");
            bret = masterEmulator.SendData(pipeSetup.WctrlPipe, p_buf);
            if(bret == true)
            {
                Console.WriteLine("UpDn_Write_Wctrl_test: TRUE");
            }
            if(bret == false)
            {
                Console.WriteLine("UpDn_Write_Wctrl_test: FALSE");
            }

            return(r);
        }

        Int32 UpDn_Read_Rctrl_test()//(array<System::Byte,1> p_buf, int len)
        {
            Int32 i;
            Int32 r;
            r = 0;
            array<System::Byte,1> r_buf;


            Console.WriteLine("UpDn_Read_Rctrl_test...");
            r_buf = masterEmulator.RequestData(pipeSetup.RctrlPipe);
            if(r_buf == nullptr)
            {
                Console.WriteLine("UpDn_Read_Rctrl_test: r_buf == nullptr");
            }

            Console.WriteLine("r_buf.Length = {0}", r_buf.Length);
            if( r_buf.Length > 0)
            {
                for(i=0 ; i<r_buf.Length; i++)
                    Console.Write(" {0:X2}", r_buf[i]);
                Console.WriteLine("");

            }
            return(r);
        }
#endif
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        public Int32 Dn_Send_Dcmd(byte[] p_buf, int len)// array<System::Byte,1> p_buf, int len)
        {
            Int32 r;
            r = 0;

            masterEmulator.SendData(pipeSetup.DcmdPipe, p_buf);
            return(r);
        }
        //---------------------------------------------------------------------
        public Int32 Dn_Send_Ddat(byte[] p_buf, int len)//  array<System::Byte,1> p_buf, int len)
        {
            Int32 r;
            r = 0;

            masterEmulator.SendData(pipeSetup.DdatPipe, p_buf);
            return(r);
        }
        //---------------------------------------------------------------------
        public void On_Dcfm( byte [] p_buf, int len )//  array<System::Byte,1> p_buf, int len )
        {
            if(p_buf[0] == 1)
            {
                if(p_buf[1] == 0)
                    Console.WriteLine("Dcfm: OK");
                if(p_buf[1] == 1)
                    Console.WriteLine("Dcfm: NG");
                if(p_buf[1] == 2)
                {
                    Console.WriteLine("Dcfm: RESEND");
                    //[2]=1, [3]=1
                    Dn_Dummy_Send43B();
                }
            }
            else
            {
                //AddToLog(String::Format("Data received on pipe number {0}:{1}", arguments.PipeNumber, stringBuffer.ToString()));
            }
        }


        //=====================================================================
        //=====================================================================
        //===== UP ============================================================
        //=====================================================================
        //=====================================================================
        int BLK_DN_COUNT;
        /*
        #define BLK_DN_COUNT 128

        Byte m_blkDn_buf[ 16 * BLK_DN_COUNT ]; // 2048 @ BLK_DN_COUNT = 128
        Byte m_blkDn_chk[  1 * BLK_DN_COUNT ]; //  128 @ BLK_DN_COUNT = 128
        */
        byte [] /*array<Byte,1>*/ m_blkDn_buf;
        byte [] /*array<Byte,1>*/ m_blkDn_chk;
        Int16 m_blkDn_len;
        Int16 m_blkDn_blkCnt;
        Int16 m_blkDn_rxBlkCnt;

        //static Byte  m_Dcfm_buf[20];
        //static Int16 m_Dcfm_len;
        byte [] /*array<Byte,1>*/ m_Dcfm_buf;
        Int16 m_Dcfm_len;

        //-----------------------------------------------------------------------------
        Int32 blk_dn_start( byte [] pkt)// array<Byte,1> pkt )
        {
            Byte  j;
            Int16 i;

            //----- BOGUS INIT -----
            BLK_DN_COUNT = 128;
            m_blkDn_buf = new byte [(16 * 128)];
            m_blkDn_chk = new byte [( 1 * 128)];

            m_Dcfm_buf = new byte [(20)];
            //----- BOGUS INIT -----

            m_blkDn_rxBlkCnt = 0;
            m_blkDn_blkCnt = 0;    

            m_blkDn_len = (Int16)(pkt[2] | (pkt[3]<<8));
            if( m_blkDn_len == 0)
                return(1);

            m_blkDn_blkCnt = (Int16)(((m_blkDn_len - 1) / 16) + 1);


            for(i=0 ; i < m_blkDn_blkCnt; i++)
            {
                m_blkDn_chk[i] = 0x00;
                for(j=0 ; j < 16; j++)
                {
                    m_blkDn_buf[i*16 + j] = 0x00;
                }
            }

            return(0);
        }


        Int32 blk_dn_add( byte [] pkt, int len )//  array<Byte,1> pkt, Int16 len )
        {
            Byte  position;
            Byte  j;
            //    Int16 i;

            if(len != 20)
                return(1);

            position = pkt[0];
            if( position >= BLK_DN_COUNT)
                return(1);

            m_blkDn_rxBlkCnt++;
            m_blkDn_chk[position] += 1;
            for(j=0 ; j < 16; j++)
            {
                m_blkDn_buf[position*16 + j] = pkt[4 + j];
            }
            return(0);
        }

Int32 DN_CHK_OK = 0;
Int32 DN_CHK_CHKSUM_NG = 3;

        Int32 blk_dn_chk()
        {
            //    Byte  j;
            Int16 i;
            Int16 missing_blk_cnt;
            Int16 cs_pkt;
            Int16 cs_now;

            if(m_blkDn_blkCnt == 0)
                return(1);

            if(m_blkDn_rxBlkCnt < m_blkDn_blkCnt)
                return(1);

            missing_blk_cnt = 0;
            for(i=0 ; i < m_blkDn_blkCnt; i++)
            {
                if(m_blkDn_chk[i] == 0x00)
                    missing_blk_cnt++;
            }
            if(missing_blk_cnt>0)
                return(2);

            cs_pkt = (Int16)(m_blkDn_buf[m_blkDn_len - 2] | (m_blkDn_buf[m_blkDn_len - 1]<<8));
            cs_now = 0;
            for(i=0 ; i < m_blkDn_len - 2; i++)
            {
                cs_now += m_blkDn_buf[i];
            }
            if( cs_now != cs_pkt)
                return( DN_CHK_CHKSUM_NG );

            return(DN_CHK_OK);
        }

        Int32 blk_dn_missing_count()
        {
            Int16 i;
            Int16 missing_blk_cnt;

            if(m_blkDn_blkCnt == 0)
                return(0);

            missing_blk_cnt = 0;
            for(i=0 ; i < m_blkDn_blkCnt; i++)
            {
                if(m_blkDn_chk[i] == 0x00)
                    missing_blk_cnt++;
            }
            return(missing_blk_cnt);
        }

        Int32 blk_dn_get_missing(byte [] buf/*array<Byte,1> buf*/, Byte ofst3, Byte count)
        {
            Int16 i;
            Int16 missing_blk_cnt;

            if(m_blkDn_blkCnt == 0)
                return(0);

            missing_blk_cnt = 0;
            for(i=0 ; i < m_blkDn_blkCnt; i++)
            {
                if(m_blkDn_chk[i] == 0x00)
                {
                    missing_blk_cnt++;
                    if( missing_blk_cnt <= count )
                    {
                        buf[ofst3 + missing_blk_cnt - 1] = (Byte)i;
                    }
                    if( missing_blk_cnt == count )
                        break;
                }
            }
            return(missing_blk_cnt);
        }

        void blk_dn_checkT2()
        {
            if( (m_blkDn_buf[0] == 'T') && (m_blkDn_buf[1] == '2') )
            {

                Console.WriteLine("blk_dn_checkT2");
                
                Console.WriteLine();
                Console.WriteLine();
                Console.Write("Got T2 Response\n");
                Console.WriteLine();
                Console.WriteLine();
            }

        }
        void blk_dn_check0x01()
        {

            if( m_blkDn_buf[0] == 0x01 )
            {
                Console.WriteLine("\nblk_dn_check0x01");

                if(m_blkDn_buf[1] == 0x44)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.Write("Command 0x44: Status  = {0:x2}\n", m_blkDn_buf[2] );
                    Console.Write("              Len     = {0}\n"   , (m_blkDn_buf[4]<<8) + (m_blkDn_buf[3]) );
                    Console.Write("              data[0] = {0:x2}\n", m_blkDn_buf[5]);
                    Console.Write("              data[1] = {0:x2}\n", m_blkDn_buf[6]);
                    Console.WriteLine();
                    Console.WriteLine();
                }
                if(m_blkDn_buf[1] == 0x45)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.Write("Command 0x45: Status  = {0:x2}\n", m_blkDn_buf[2] );
                    Console.Write("              Len     = {0}\n"   , (m_blkDn_buf[4]<<8) + (m_blkDn_buf[3]) );
                    Console.Write("              data[0] = {0:x2}\n", m_blkDn_buf[5]);
                    Console.Write("              data[1] = {0:x2}\n", m_blkDn_buf[6]);
                    Console.WriteLine();
                    Console.WriteLine();
                }


                if(m_blkDn_buf[1] == 0x58)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.Write("Serial Number = ");
                    Console.Write("{0:x2}", m_blkDn_buf[8] );
                    Console.Write("{0:x2}", m_blkDn_buf[7] );
                    Console.Write("{0:x2}", m_blkDn_buf[6] );
                    Console.Write("{0:x2}", m_blkDn_buf[5] );
                    Console.WriteLine();
                    Console.WriteLine();
                }
                if(m_blkDn_buf[1] == 0xF8)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.Write("Command 0xF8 Response\n");
                    Console.WriteLine();
                    Console.WriteLine();
                }
                if(m_blkDn_buf[1] == 0xF9)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.Write("Command 0xF9 Response\n");
                    Console.WriteLine();
                    Console.WriteLine();
                }
            }

        }

        bool app_uart_put(char c)
        {
            return(true);
        }

        //-----------------------------------------------------------------------------
        //
        //  Controls
        //
        //-----------------------------------------------------------------------------
        UInt32 start_send_Dcfm_OK()
        {
            //            UInt32 err_code;

            m_Dcfm_buf[0] = 1;
            m_Dcfm_buf[1] = 0; // 0 = OK
            m_Dcfm_len = 2;

            Un_Send_Ucfm(m_Dcfm_buf, m_Dcfm_len);
            return(0);

            // should be this mode m_BlkDn_sm = eBlkDn_GOT_PACKET;

            /*
            err_code = ble_tuds_notify_Dcfm( &m_tuds, m_Dcfm_buf, &m_Dcfm_len);
            if(err_code == NRF_SUCCESS)
            {
            while(app_uart_put('o') != NRF_SUCCESS);
            m_BlkDn_sm = eBlkDn_CFM_SEND;
            start_Dn_timer(42);
            }
            else
            if(err_code == BLE_ERROR_NO_TX_BUFFERS)
            {
            while(app_uart_put('n') != NRF_SUCCESS);
            // should be this mode m_BlkDn_sm = eBlkDn_GOT_PACKET;
            start_Dn_timer(42);
            }
            else
            {
            while(app_uart_put('x') != NRF_SUCCESS);
            }

            return(err_code);    
            */
        }

        UInt32 start_send_Dcfm_NG()
        {
            //            UInt32 err_code;

            m_Dcfm_buf[0] = 1;
            m_Dcfm_buf[1] = 1; // 1 = NG (Checksum NG)
            m_Dcfm_len = 2;

            Un_Send_Ucfm(m_Dcfm_buf, m_Dcfm_len);
            return(0);

            /*
            // should be this mode m_BlkDn_sm = eBlkDn_GOT_PACKET;

            err_code = ble_tuds_notify_Dcfm( &m_tuds, m_Dcfm_buf, &m_Dcfm_len);
            if(err_code == NRF_SUCCESS)
            {
            m_BlkDn_sm = eBlkDn_CFM_SEND;
            start_Dn_timer(42);
            }
            if(err_code == BLE_ERROR_NO_TX_BUFFERS)
            {
            // should be this mode m_BlkDn_sm = eBlkDn_GOT_PACKET;
            start_Dn_timer(42);
            }

            return(err_code);
            */
        }

        UInt32 start_send_Dcfm_missing(int max_entries)
        {
            //            UInt32 err_code;

            m_Dcfm_buf[0] = 1;
            m_Dcfm_buf[1] = 2; // 2 = Missing

            int n = blk_dn_missing_count();
            if( n > max_entries )
                n = max_entries;
            m_Dcfm_buf[2] = (byte)n;
            //blk_dn_get_missing( &m_Dcfm_buf[3], n );
            blk_dn_get_missing(m_Dcfm_buf, (byte)3, (byte)n);
            m_Dcfm_len = (Int16)(3 + n);

            Un_Send_Ucfm(m_Dcfm_buf, m_Dcfm_len);
            return(0);

            // should be this mode m_BlkDn_sm = eBlkDn_GOT_PACKET;

            /*
            err_code = ble_tuds_notify_Dcfm( &m_tuds, m_Dcfm_buf, &m_Dcfm_len);
            if(err_code != NRF_SUCCESS)
            printf("ble_tuds_notify_Dcfm=%d\n\r", err_code);

            if(err_code == NRF_SUCCESS)
            {
            stop_Dn_timer();
            m_BlkDn_sm = eBlkDn_MISSING_SEND;
            start_Dn_timer(42);
            }
            if(err_code == BLE_ERROR_NO_TX_BUFFERS)
            {
            // should be this mode m_BlkDn_sm = eBlkDn_GOT_PACKET;
            stop_Dn_timer();
            m_BlkDn_sm = eBlkDn_MISSING_PRESEND;
            start_Dn_timer(42);
            }

            return(err_code);
            */
        }

        //-----------------------------------------------------------------------------
        //
        //  Events
        //
        //-----------------------------------------------------------------------------
        public Int32 On_Ucmd( byte[] buf, int len ) //array<System::Byte,1> buf, int len )
        {
            Int32 r = 0;

            if( (buf[0] == 1) && (buf[1] == 1) )
            {
                ///m_BlkDn_packetWaitTimeCount = 0;
                ///while(app_uart_put('S') != NRF_SUCCESS);
                r = blk_dn_start(buf);
                ///m_BlkDn_sm = eBlkDn_WAIT_PACKET;

            }
            return(r);

        }
        public Int32 On_Udat(byte[] buf, int len) // array<System::Byte,1> buf, int len )
        {
            Int32 r;

            ///stop_Dn_timer();
            ///m_BlkDn_packetWaitTimeCount = 0;
            r = blk_dn_add( buf, len);

            r = blk_dn_chk();
            if(r == DN_CHK_OK)
            {
                ///while(app_uart_put('O') != NRF_SUCCESS);
                //m_BlkDn_sm = eBlkDn_GOT_PACKET;
                start_send_Dcfm_OK();

                Console.Write("\nDN_CHK_OK: m_blkDn_len = {0}\n", m_blkDn_len);
                blk_dn_check0x01();
                blk_dn_checkT2();
            }
            else
                if(r == DN_CHK_CHKSUM_NG)
                {
                    ///while(app_uart_put('N') != NRF_SUCCESS);
                    ///m_BlkDn_sm = eBlkDn_GOT_PACKET;
                    start_send_Dcfm_NG();
                }
                else
                {
                    ///while(app_uart_put('D') != NRF_SUCCESS);
                    ///start_Dn_timer(42);
                }

                return(r);
        }



        public Int32 Un_Send_Ucfm(byte[] p_buf, int len) // array<System::Byte,1> p_buf, int len)
        {
            Int32 r;
            r = 0;

            masterEmulator.SendData(pipeSetup.UcfmPipe, p_buf);
            return(r);
        }


    }
}
