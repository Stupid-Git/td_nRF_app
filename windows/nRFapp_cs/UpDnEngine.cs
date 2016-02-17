using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nordicsemi;

namespace TDnRF
{

    public class PipeSetup_UpDn
    {
        MasterEmulator masterEmulator;
        public PipeSetup_UpDn(MasterEmulator master)
        {
            masterEmulator = master;
        }


        //===== Dn =====
        //----------------------------------------
        public int DcmdPipe { get; private set; }
        public int DdatPipe { get; private set; }
        public int DcfmPipe { get; private set; }

        //===== Up =====
        //----------------------------------------
        public int UcmdPipe { get; private set; }
        public int UdatPipe { get; private set; }
        public int UcfmPipe { get; private set; }


        public void PerformPipeSetup()
        {
            // GAP service 
            BtUuid TDudOverBtleUuid = new BtUuid("6e400001b5a3f393e0a9e50e24dcca42");
            masterEmulator.SetupAddService(TDudOverBtleUuid, PipeStore.Remote);

            //===== Dn =====
            // DCMD characteristic (Down Link Command 0x0002) 
            BtUuid DcmdUuid = new BtUuid("6e400002b5a3f393e0a9e50e24dcca42");
            int DcmdMaxLength = 20;
            byte[] DcmdData = null;
            masterEmulator.SetupAddCharacteristicDefinition(DcmdUuid, DcmdMaxLength, DcmdData);
            // Using pipe type Transmit to enable write operations 
            DcmdPipe = masterEmulator.SetupAssignPipe(PipeType.Transmit);

            // DDAT characteristic (Down Link Data 0x0003) 
            BtUuid DdatUuid = new BtUuid("6e400003b5a3f393e0a9e50e24dcca42");
            int DdatMaxLength = 20;
            byte[] DdatData = null;
            masterEmulator.SetupAddCharacteristicDefinition(DdatUuid, DdatMaxLength, DdatData);
            // Using pipe type Receive to enable notify operations 
            DdatPipe = masterEmulator.SetupAssignPipe(PipeType.Transmit);

            // DCFM characteristic (Down Link Confirm 0x0004)
            BtUuid DcfmUuid = new BtUuid("6e400004b5a3f393e0a9e50e24dcca42");
            int DcfmMaxLength = 20;
            byte[] DcfmData = null;
            masterEmulator.SetupAddCharacteristicDefinition(DcfmUuid, DcfmMaxLength, DcfmData);
            // Using pipe type Receive to enable notify operations 
            DcfmPipe = masterEmulator.SetupAssignPipe(PipeType.Receive);

            //===== Up =====
            // UCMD characteristic (Up Link Command 0x0005) 
            BtUuid UcmdUuid = new BtUuid("6e400005b5a3f393e0a9e50e24dcca42");
            int UcmdMaxLength = 20;
            byte[] UcmdData = null;
            masterEmulator.SetupAddCharacteristicDefinition(UcmdUuid, UcmdMaxLength, UcmdData);
            // Using pipe type Transmit to enable write operations 
            UcmdPipe = masterEmulator.SetupAssignPipe(PipeType.Receive);

            // DDAT characteristic (Up Link Data 0x0006) 
            BtUuid UdatUuid = new BtUuid("6e400006b5a3f393e0a9e50e24dcca42");
            int UdatMaxLength = 20;
            byte[] UdatData = null;
            masterEmulator.SetupAddCharacteristicDefinition(UdatUuid, UdatMaxLength, UdatData);
            // Using pipe type Receive to enable notify operations 
            UdatPipe = masterEmulator.SetupAssignPipe(PipeType.Receive);

            // UCFM characteristic (Up Link Confirm 0x0007)
            BtUuid UcfmUuid = new BtUuid("6e400007b5a3f393e0a9e50e24dcca42");
            int UcfmMaxLength = 20;
            byte[] UcfmData = null;
            masterEmulator.SetupAddCharacteristicDefinition(UcfmUuid, UcfmMaxLength, UcfmData);
            // Using pipe type Receive to enable notify operations 
            UcfmPipe = masterEmulator.SetupAssignPipe(PipeType.Transmit);
        }

    }


    public class UpDnEngine
    {
        public Nordicsemi.MasterEmulator masterEmulator;
        //PPP PipeSetup pipeSetup;
        PipeSetup_UpDn pipeSetup;

        public void UpDnEngine_Setup(Nordicsemi.MasterEmulator master)//PPP , PipeSetup pipe)
        {
            masterEmulator = master;
            //PPP pipeSetup = pipe;
            pipeSetup = new PipeSetup_UpDn(master);
        }

        public void PerformPipeSetup()
        {
            this.pipeSetup.PerformPipeSetup();
        }


        public UpDnEngine()
        {
        }

        public void EnableNotify_Dcfm()
        {
            masterEmulator.GetCharacteristicProperties(pipeSetup.DcfmPipe);
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


        public Int32 Dn_Send_CMD_11_Pkt(byte[] pktbuf)//array<System::Byte,1> pktbuf)
        {
            int i;
            byte[] buf = new byte[20]; // array<Byte,1> buf = new array<Byte,1>(20);
            //Byte length;
            Byte blk_no;
            UInt16 ofst;

            UInt16 totalLength;
            UInt16 total_cs;
            Byte total_cs0;
            Byte total_cs1;

            Int32 r;

            totalLength = (UInt16)(pktbuf.Length + 2);
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
                if (ofst > pktbuf.Length)
                    break;

                buf[0] = blk_no++;
                buf[1] = 0x00;
                buf[2] = 0x00;
                buf[3] = 0x00;
                for (i = 0; i < 16; i++)
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
                if (r != 0)
                    break;

            } while (true);

            return (0);
        }
        //---------------------------------------------------------------------
        Int32 Dn_Dummy_Send42()
        {
            int i;
            byte[] buf = new byte[20]; //array<Byte,1> buf = new array<Byte,1>(20);
            // fake make packet
            byte[] pktbuf = new byte[42]; //array<System::Byte,1> pktbuf = new array<Byte,1>(42);
            UInt16 cs_pkt;
            cs_pkt = 0;
            for (i = 0; i < pktbuf.Length - 2; i++)
            {
                pktbuf[i] = (Byte)(i & 0x00FF);
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf.Length - 2] = (byte)((cs_pkt >> 0) & 0x00FF);
            pktbuf[pktbuf.Length - 1] = (byte)((cs_pkt >> 8) & 0x00FF);


            Dn_Send_CMD_11_Pkt(pktbuf);
            return (0);
        }


        //---------------------------------------------------------------------
        Int32 Dn_Dummy_Send43()
        {
            int i;
            byte[] buf = new byte[20]; //array<Byte,1> buf = new array<Byte,1>(20);
            //Byte length;
            Byte blk_no;
            UInt16 ofst;

            Int32 r;
            // fake make packet
            byte[] pktbuf = new byte[42]; //array<System::Byte,1> pktbuf = new array<Byte,1>(42);
            UInt16 cs_pkt;
            cs_pkt = 0;
            for (i = 0; i < pktbuf.Length - 2; i++)
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
                ofst = (UInt16)(blk_no * 16);
                if (ofst > pktbuf.Length)
                    break;

                buf[0] = blk_no++;
                buf[1] = 0x00;
                buf[2] = 0x00;
                buf[3] = 0x00;
                for (i = 0; i < 16; i++)
                    if (ofst + i < pktbuf.Length)
                        buf[4 + i] = pktbuf[ofst + i];
                    else
                        buf[4 + i] = 0x00;

                //length = 20;
                if (blk_no != 2)
                {
                    r = Dn_Send_Ddat(buf, 20);
                    if (r != 0)
                        break;
                }

            } while (true);

            return (0);
        }
        //---------------------------------------------------------------------
        Int32 Dn_Dummy_Send43B()
        {
            int i;
            byte[] buf = new byte[20];
            //Byte length;
            Byte blk_no;
            UInt16 ofst;

            Int32 r;
            // fake make packet
            byte[] pktbuf = new byte[42]; //         array<System::Byte,1> pktbuf = new array<Byte,1>(42);
            UInt16 cs_pkt;
            cs_pkt = 0;
            for (i = 0; i < pktbuf.Length - 2; i++)
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
                ofst = (UInt16)(blk_no * 16);
                if (ofst > pktbuf.Length)
                    break;

                buf[0] = blk_no++;
                buf[1] = 0x00;
                buf[2] = 0x00;
                buf[3] = 0x00;
                for (i = 0; i < 16; i++)
                    if (ofst + i < pktbuf.Length)
                        buf[4 + i] = pktbuf[ofst + i];
                    else
                        buf[4 + i] = 0x00;

                //length = 20;
                if (blk_no == 2)
                {
                    r = Dn_Send_Ddat(buf, 20);
                    if (r != 0)
                        break;
                }

            } while (true);

            return (0);
        }


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
        UInt16 m_blkDn_len;
        UInt16 m_blkDn_blkCnt;
        UInt16 m_blkDn_rxBlkCnt;

        //static Byte  m_Dcfm_buf[20];
        //static UInt16 m_Dcfm_len;
        byte [] /*array<Byte,1>*/ m_Dcfm_buf;
        UInt16 m_Dcfm_len;

        //-----------------------------------------------------------------------------
        Int32 blk_dn_start( byte [] pkt)// array<Byte,1> pkt )
        {
            Byte  j;
            UInt16 i;

            //----- BOGUS INIT -----
            BLK_DN_COUNT = 128;
            m_blkDn_buf = new byte [(16 * 128)];
            m_blkDn_chk = new byte [( 1 * 128)];

            m_Dcfm_buf = new byte [(20)];
            //----- BOGUS INIT -----

            m_blkDn_rxBlkCnt = 0;
            m_blkDn_blkCnt = 0;    

            m_blkDn_len = (UInt16)(pkt[2] | (pkt[3]<<8));
            if( m_blkDn_len == 0)
                return(1);

            m_blkDn_blkCnt = (UInt16)(((m_blkDn_len - 1) / 16) + 1);


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


        Int32 blk_dn_add( byte [] pkt, UInt16 len )//  array<Byte,1> pkt, UInt16 len )
        {
            Byte  position;
            Byte  j;
            //    UInt16 i;

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
            UInt16 i;
            UInt16 missing_blk_cnt;
            UInt16 cs_pkt;
            UInt16 cs_now;

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

            cs_pkt = (UInt16)(m_blkDn_buf[m_blkDn_len - 2] | (m_blkDn_buf[m_blkDn_len - 1]<<8));
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
            m_Dcfm_len = (UInt16)(3 + n);

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

        //DUMMY
        public delegate Int32 L6_OnDataRxCallback_DEL(byte[] theUpPacket);

        static Int32 L6_OnDataRxCallback_DUMMY(byte[] theUpPacket)
        {
            return (42);
        }

        L6_OnDataRxCallback_DEL rxCallback = L6_OnDataRxCallback_DUMMY;

        public void set_OnRXcallbackdelegate(L6_OnDataRxCallback_DEL f)
        {
            rxCallback = f;
        }


        public Int32 On_Udat(byte[] buf, int len) // array<System::Byte,1> buf, int len )
        {
            Int32 r;

            ///stop_Dn_timer();
            ///m_BlkDn_packetWaitTimeCount = 0;
            r = blk_dn_add( buf, (UInt16)len);

            r = blk_dn_chk();
            if(r == DN_CHK_OK)
            {
                ///while(app_uart_put('O') != NRF_SUCCESS);
                //m_BlkDn_sm = eBlkDn_GOT_PACKET;
                start_send_Dcfm_OK();

                Console.Write("\nDN_CHK_OK: m_blkDn_len = {0}\n", m_blkDn_len);


                byte[] theUpPacket = new byte[m_blkDn_len];
                Buffer.BlockCopy(m_blkDn_buf, 0, theUpPacket, 0, m_blkDn_len);
                rxCallback(theUpPacket);
                //L6_OnDataRxCallback_DUMMY(theUpPacket); // blk_dn_check0x01(); blk_dn_checkT2();
            
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



        public Int32 OnAnyPipe(Object sender, Nordicsemi.PipeDataEventArgs arguments)
        {
            //===== udEngine =====
            if (arguments.PipeNumber == pipeSetup.DcfmPipe)
                OnDataReceived_Dn(sender, arguments);

            if (arguments.PipeNumber == pipeSetup.UcmdPipe)
                OnDataReceived_Up(sender, arguments);
            if (arguments.PipeNumber == pipeSetup.UdatPipe)
                OnDataReceived_Up(sender, arguments);
            return (42);
        }

        
        void OnDataReceived_Dn(Object sender, Nordicsemi.PipeDataEventArgs arguments)
        {
            On_Dcfm(arguments.PipeData, arguments.PipeData.Length);
        }
        void OnDataReceived_Up(Object sender, Nordicsemi.PipeDataEventArgs arguments)
        {
            if (arguments.PipeNumber == pipeSetup.UcmdPipe)
                On_Ucmd(arguments.PipeData, arguments.PipeData.Length);
            if (arguments.PipeNumber == pipeSetup.UdatPipe)
                On_Udat(arguments.PipeData, arguments.PipeData.Length);
        }
        


    }
}
