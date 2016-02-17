using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics; // Trace
using Nordicsemi;

namespace TDnRFcontroller_LIB
{


    //=========================================================================
    //=========================================================================
    //=========================================================================
    //=========================================================================
    //=========================================================================
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
            BtUuid uartOverBtleUuid = new BtUuid("6e400001b5a3f393e0a9e50e24dcca42");
            masterEmulator.SetupAddService(uartOverBtleUuid, PipeStore.Remote);

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
            return (0);
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
            return (r);
        }
        //---------------------------------------------------------------------
        public Int32 Dn_Send_Ddat(byte[] p_buf, int len)//  array<System::Byte,1> p_buf, int len)
        {
            Int32 r;
            r = 0;

            masterEmulator.SendData(pipeSetup.DdatPipe, p_buf);
            return (r);
        }
        //---------------------------------------------------------------------
        public void On_Dcfm(byte[] p_buf, int len)//  array<System::Byte,1> p_buf, int len )
        {
            if (p_buf[0] == 1)
            {
                if (p_buf[1] == 0)
                    Console.WriteLine("Dcfm: OK");
                if (p_buf[1] == 1)
                    Console.WriteLine("Dcfm: NG");
                if (p_buf[1] == 2)
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
        byte[] /*array<Byte,1>*/ m_blkDn_buf;
        byte[] /*array<Byte,1>*/ m_blkDn_chk;
        UInt16 m_blkDn_len;
        UInt16 m_blkDn_blkCnt;
        UInt16 m_blkDn_rxBlkCnt;

        //static Byte  m_Dcfm_buf[20];
        //static UInt16 m_Dcfm_len;
        byte[] /*array<Byte,1>*/ m_Dcfm_buf;
        UInt16 m_Dcfm_len;

        //-----------------------------------------------------------------------------
        Int32 blk_dn_start(byte[] pkt)// array<Byte,1> pkt )
        {
            Byte j;
            UInt16 i;

            //----- BOGUS INIT -----
            BLK_DN_COUNT = 128;
            m_blkDn_buf = new byte[(16 * 128)];
            m_blkDn_chk = new byte[(1 * 128)];

            m_Dcfm_buf = new byte[(20)];
            //----- BOGUS INIT -----

            m_blkDn_rxBlkCnt = 0;
            m_blkDn_blkCnt = 0;

            m_blkDn_len = (UInt16)(pkt[2] | (pkt[3] << 8));
            if (m_blkDn_len == 0)
                return (1);

            m_blkDn_blkCnt = (UInt16)(((m_blkDn_len - 1) / 16) + 1);


            for (i = 0; i < m_blkDn_blkCnt; i++)
            {
                m_blkDn_chk[i] = 0x00;
                for (j = 0; j < 16; j++)
                {
                    m_blkDn_buf[i * 16 + j] = 0x00;
                }
            }

            return (0);
        }


        Int32 blk_dn_add(byte[] pkt, UInt16 len)//  array<Byte,1> pkt, UInt16 len )
        {
            Byte position;
            Byte j;
            //    UInt16 i;

            if (len != 20)
                return (1);

            position = pkt[0];
            if (position >= BLK_DN_COUNT)
                return (1);

            m_blkDn_rxBlkCnt++;
            m_blkDn_chk[position] += 1;
            for (j = 0; j < 16; j++)
            {
                m_blkDn_buf[position * 16 + j] = pkt[4 + j];
            }
            return (0);
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

            if (m_blkDn_blkCnt == 0)
                return (1);

            if (m_blkDn_rxBlkCnt < m_blkDn_blkCnt)
                return (1);

            missing_blk_cnt = 0;
            for (i = 0; i < m_blkDn_blkCnt; i++)
            {
                if (m_blkDn_chk[i] == 0x00)
                    missing_blk_cnt++;
            }
            if (missing_blk_cnt > 0)
                return (2);

            cs_pkt = (UInt16)(m_blkDn_buf[m_blkDn_len - 2] | (m_blkDn_buf[m_blkDn_len - 1] << 8));
            cs_now = 0;
            for (i = 0; i < m_blkDn_len - 2; i++)
            {
                cs_now += m_blkDn_buf[i];
            }
            if (cs_now != cs_pkt)
                return (DN_CHK_CHKSUM_NG);

            return (DN_CHK_OK);
        }

        Int32 blk_dn_missing_count()
        {
            Int16 i;
            Int16 missing_blk_cnt;

            if (m_blkDn_blkCnt == 0)
                return (0);

            missing_blk_cnt = 0;
            for (i = 0; i < m_blkDn_blkCnt; i++)
            {
                if (m_blkDn_chk[i] == 0x00)
                    missing_blk_cnt++;
            }
            return (missing_blk_cnt);
        }

        Int32 blk_dn_get_missing(byte[] buf/*array<Byte,1> buf*/, Byte ofst3, Byte count)
        {
            Int16 i;
            Int16 missing_blk_cnt;

            if (m_blkDn_blkCnt == 0)
                return (0);

            missing_blk_cnt = 0;
            for (i = 0; i < m_blkDn_blkCnt; i++)
            {
                if (m_blkDn_chk[i] == 0x00)
                {
                    missing_blk_cnt++;
                    if (missing_blk_cnt <= count)
                    {
                        buf[ofst3 + missing_blk_cnt - 1] = (Byte)i;
                    }
                    if (missing_blk_cnt == count)
                        break;
                }
            }
            return (missing_blk_cnt);
        }


        bool app_uart_put(char c)
        {
            return (true);
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
            return (0);

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
            return (0);

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
            if (n > max_entries)
                n = max_entries;
            m_Dcfm_buf[2] = (byte)n;
            //blk_dn_get_missing( &m_Dcfm_buf[3], n );
            blk_dn_get_missing(m_Dcfm_buf, (byte)3, (byte)n);
            m_Dcfm_len = (UInt16)(3 + n);

            Un_Send_Ucfm(m_Dcfm_buf, m_Dcfm_len);
            return (0);

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
        public Int32 On_Ucmd(byte[] buf, int len) //array<System::Byte,1> buf, int len )
        {
            Int32 r = 0;

            if ((buf[0] == 1) && (buf[1] == 1))
            {
                ///m_BlkDn_packetWaitTimeCount = 0;
                ///while(app_uart_put('S') != NRF_SUCCESS);
                r = blk_dn_start(buf);
                ///m_BlkDn_sm = eBlkDn_WAIT_PACKET;

            }
            return (r);

        }

        //DUMMY
        public delegate Int32 L6_OnDataRxCallback_DEL(byte[] theUpPacket);

        static Int32 L6_OnDataRxCallback_DUMMY(byte[] theUpPacket)
        {
            return (42);
        }

        L6_OnDataRxCallback_DEL rxCallback = L6_OnDataRxCallback_DUMMY;

        public void set_rxCallback(L6_OnDataRxCallback_DEL f)
        {
            rxCallback = f;
        }


        public Int32 On_Udat(byte[] buf, int len) // array<System::Byte,1> buf, int len )
        {
            Int32 r;

            ///stop_Dn_timer();
            ///m_BlkDn_packetWaitTimeCount = 0;
            r = blk_dn_add(buf, (UInt16)len);

            r = blk_dn_chk();
            if (r == DN_CHK_OK)
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
                if (r == DN_CHK_CHKSUM_NG)
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

            return (r);
        }



        public Int32 Un_Send_Ucfm(byte[] p_buf, int len) // array<System::Byte,1> p_buf, int len)
        {
            Int32 r;
            r = 0;

            masterEmulator.SendData(pipeSetup.UcfmPipe, p_buf);
            return (r);
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



    //=========================================================================
    //=========================================================================
    //=========================================================================
    //=========================================================================
    //=========================================================================
    public class PipeSetup_Ctrl
    {
        MasterEmulator masterEmulator;
        public PipeSetup_Ctrl(MasterEmulator master)
        {
            masterEmulator = master;
        }

        //===== Ctrl =====
        //----------------------------------------
        public int WctrlPipe { get; private set; }
        public int RctrlPipe { get; private set; }


        //===== ACtrl =====
        //----------------------------------------
        public int WActrlPipe { get; private set; }
        public int RActrlPipe { get; private set; }


        public void PerformPipeSetup_Ctrl()
        {
            // GAP service 
            BtUuid TDctrl_OverBtleUuid = new BtUuid("6e400001b5a3f393e0a9e50e24dcca43");
            masterEmulator.SetupAddService(TDctrl_OverBtleUuid, PipeStore.Remote);

            //===== Ctrl =====
            // WCTRL characteristic (Write Link Command 0x00xx) 
            BtUuid WctrlUuid = new BtUuid("6e400008b5a3f393e0a9e50e24dcca43");
            int WctrlMaxLength = 20;
            byte[] WctrlData = null;
            masterEmulator.SetupAddCharacteristicDefinition(WctrlUuid, WctrlMaxLength, WctrlData);
            // Using pipe type Transmit to enable write operations 
            //DcmdPipe = masterEmulator.SetupAssignPipe(PipeType.Transmit);
            WctrlPipe = masterEmulator.SetupAssignPipe(PipeType.Transmit);

            // RCTRL characteristic (Read Link Command 0x00xx) 
            BtUuid RctrlUuid = new BtUuid("6e400009b5a3f393e0a9e50e24dcca43");
            int RctrlMaxLength = 20;
            byte[] RctrlData = null;
            masterEmulator.SetupAddCharacteristicDefinition(RctrlUuid, RctrlMaxLength, RctrlData);
            // Using pipe type Transmit to enable write operations 
            RctrlPipe = masterEmulator.SetupAssignPipe(PipeType.ReceiveRequest);
            //RctrlPipe = masterEmulator.SetupAssignPipe(PipeType.Receive); NG


            //===== ACtrl =====
            // WACTRL characteristic (Write Link Command 0x00xx) 
            BtUuid WActrlUuid = new BtUuid("6e40000ab5a3f393e0a9e50e24dcca43");
            int WActrlMaxLength = 20;
            byte[] WActrlData = null;
            masterEmulator.SetupAddCharacteristicDefinition(WActrlUuid, WActrlMaxLength, WActrlData);
            // Using pipe type Transmit to enable write operations 
            WActrlPipe = masterEmulator.SetupAssignPipe(PipeType.Transmit);

            // RACTRL characteristic (Read Link Command 0x00xx) 
            BtUuid RActrlUuid = new BtUuid("6e40000bb5a3f393e0a9e50e24dcca43");
            int RActrlMaxLength = 20;
            byte[] RActrlData = null;
            masterEmulator.SetupAddCharacteristicDefinition(RActrlUuid, RActrlMaxLength, RActrlData);
            // Using pipe type Transmit to enable write operations 
            RActrlPipe = masterEmulator.SetupAssignPipe(PipeType.ReceiveRequest);
        }

    }

    public class CtrlEngine
    {

        public Nordicsemi.MasterEmulator masterEmulator;
        //PPP PipeSetup pipeSetup;
        PipeSetup_Ctrl pipeSetup;

        public void CtrlEngine_Setup(Nordicsemi.MasterEmulator master)//PPP , PipeSetup pipe)
        {
            masterEmulator = master;
            //PPP pipeSetup = pipe;
            pipeSetup = new PipeSetup_Ctrl(master);
        }

        public void PerformPipeSetup()
        {
            //this.pipeSetup.PerformPipeSetup(); TODO  
            this.pipeSetup.PerformPipeSetup_Ctrl(); //TODO change it generic PerformPipeSetup
        }


        public CtrlEngine()
        {
        }

        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        Int32 UpDn_Write_Wctrl_test(Byte[] p_buf, int len)//array<System::Byte,1> p_buf, int len)
        {
            bool bret;
            Int32 r;
            r = 0;

            Console.WriteLine("UpDn_Write_Wctrl_test...");
            bret = masterEmulator.SendData(pipeSetup.WctrlPipe, p_buf);
            if (bret == true)
            {
                Console.WriteLine("UpDn_Write_Wctrl_test: TRUE");
            }
            if (bret == false)
            {
                Console.WriteLine("UpDn_Write_Wctrl_test: FALSE");
            }

            return (r);
        }

        Int32 UpDn_Read_Rctrl_test()//(array<System::Byte,1> p_buf, int len)
        {
            Int32 i;
            Int32 r;
            r = 0;
            Byte[] r_buf; //array<System::Byte,1> r_buf;


            Console.WriteLine("UpDn_Read_Rctrl_test...");
            r_buf = masterEmulator.RequestData(pipeSetup.RctrlPipe);
            if (r_buf == null)//nullptr)
            {
                Console.WriteLine("UpDn_Read_Rctrl_test: r_buf == nullptr");
            }

            Console.WriteLine("r_buf.Length = {0}", r_buf.Length);
            if (r_buf.Length > 0)
            {
                for (i = 0; i < r_buf.Length; i++)
                    Console.Write(" {0:X2}", r_buf[i]);
                Console.WriteLine("");

            }
            return (r);
        }

        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        public Int32 On_Wctrl(byte[] buf, int len) // array<System::Byte,1> buf, int len )
        {
            /*
            System.Text.StringBuilder stringBuffer = new System.Text.StringBuilder();
            foreach (Byte element in arguments.PipeData)
            {
                stringBuffer.AppendFormat(" 0x{0:X2}", element);
            }
            if (DebugMessagesEnabled)
            {
                AddToLog(String.Format("Wctrl: Data received on pipe number {0}:{1}", arguments.PipeNumber,  stringBuffer.ToString()));
            }
            */
            return (42);
        }
        public Int32 On_Rctrl(byte[] buf, int len) // array<System::Byte,1> buf, int len )
        {
            /*
            System.Text.StringBuilder stringBuffer = new System.Text.StringBuilder();
            foreach (Byte element in arguments.PipeData)
            {
                stringBuffer.AppendFormat(" 0x{0:X2}", element);
            }
            if (DebugMessagesEnabled)
            {
                AddToLog(String.Format("Rctrl: Data received on pipe number {0}:{1}", arguments.PipeNumber,  stringBuffer.ToString()));
            }
            */
            return (42);
        }



        public Int32 OnAnyPipe(Object sender, Nordicsemi.PipeDataEventArgs arguments)
        {
            if (arguments.PipeNumber == pipeSetup.WctrlPipe)
                OnDataReceived_Wctrl(sender, arguments);
            if (arguments.PipeNumber == pipeSetup.RctrlPipe)
                OnDataReceived_Rctrl(sender, arguments);
            return (42);
        }

        void OnDataReceived_Wctrl(Object sender, Nordicsemi.PipeDataEventArgs arguments)
        {
            if (arguments.PipeNumber == pipeSetup.WctrlPipe)
                On_Wctrl(arguments.PipeData, arguments.PipeData.Length);
        }
        void OnDataReceived_Rctrl(Object sender, Nordicsemi.PipeDataEventArgs arguments)
        {
            if (arguments.PipeNumber == pipeSetup.RctrlPipe)
                On_Rctrl(arguments.PipeData, arguments.PipeData.Length);
        }

    }








    //=========================================================================
    //=========================================================================
    //=========================================================================

    /// <summary>
    /// Provides data for the OutputReceived event.
    /// </summary>
    public class OutputReceivedEventArgs : EventArgs
    {
        public string Message { get; set; }

        public OutputReceivedEventArgs(string message)
        {
            Message = message;
        }
    }

    public class MinDeviceInfoEventArgs : EventArgs
    {
        public string DeviceName { get; set; }
        public string DeviceAddr { get; set; }
        public string Rssi { get; set; }

        public MinDeviceInfoEventArgs(String deviceName, String deviceAddr, String rssi)
        {
            DeviceName = deviceName;
            DeviceAddr = deviceAddr;
            Rssi = rssi;
        }
    }

    public class udPacketEventArgs : EventArgs
    {
        public Byte[] Packet { get; set; }
        public Int32 Length { get; set; }

        public udPacketEventArgs(Byte[] packet, Int32 length)
        {
            Packet = packet;
            Length = length;
        }
    }


    //=========================================================================
    //=========================================================================
    //=========================================================================
    //=========================================================================
    //=========================================================================
    /// <summary>
    /// This class controls all calls to MasterEmulator DLL and implements the nRF UART 
    /// logic.
    /// </summary>
    public class TDnRFcontroller
    {
        public bool Flag_Initialised;
        public bool FLAG_connectionInProgress;
        public bool FLAG_autoConnect;
        public String m_autoConnect_sDeviceId;

        /* Event declarations */
        public event EventHandler<OutputReceivedEventArgs> LogMessage;
        public event EventHandler<EventArgs> appEv_Initialized;

        public event EventHandler<EventArgs> appEv_ScanningStarted;
        public event EventHandler<MinDeviceInfoEventArgs> appEv_DeviceInfoUpdate;
        public event EventHandler<EventArgs> appEv_ScanningCancelled;

        public event EventHandler<EventArgs> appEv_ConnectionStarted;
        public event EventHandler<EventArgs> appEv_ConnectionCancelled;
        public event EventHandler<EventArgs> appEv_Connected;
        public event EventHandler<EventArgs> appEv_ServiceDiscoveryCompleted;
        public event EventHandler<EventArgs> appEv_Disconnected;

        public event EventHandler<udPacketEventArgs> userEv_udPacket;


        public event EventHandler<EventArgs> SendDataStarted;
        public event EventHandler<EventArgs> SendDataCompleted;
        //public event EventHandler<ValueEventArgs<int>> ProgressUpdated;

        /* Public properties */
        public bool DebugMessagesEnabled { get; set; }

        /* Instance variables */
        public MasterEmulator masterEmulator;

        List<BtDevice> m_deviceList;

        public UpDnEngine udEngine;
        public CtrlEngine ctrlEngine;

        const int maxPacketLength = 20;
        const int counterFieldLength = 2;
        const int maxPayloadLength = maxPacketLength - counterFieldLength;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TDnRFcontroller()
        {
            m_deviceList = new List<BtDevice>();

            Flag_Initialised = false;
            FLAG_connectionInProgress = false;
            FLAG_autoConnect = false;
        }

        /// <summary>
        /// Create MasterEmulator instance.
        /// </summary>
        void InitializeMasterEmulator()
        {
            AddToLog("Loading...");
            masterEmulator = new MasterEmulator();
        }

        /// <summary>
        /// Register event handlers for MasterEmulator events.
        /// </summary>
        void RegisterEventHandlers() 
        {
            masterEmulator.LogMessage += meEv_OnLogMessage;                           //public event EventHandler<ValueEventArgs<string>> LogMessage;

            masterEmulator.DeviceDiscovered += meEv_OnDeviceDiscovered;               //public event EventHandler<ValueEventArgs<BtDevice>> DeviceDiscovered;

            masterEmulator.Connected += meEv_OnConnected;                             //public event EventHandler<EventArgs> Connected;
            masterEmulator.Disconnected += meEv_OnDisconnected;                       //public event EventHandler<ValueEventArgs<DisconnectReason>> Disconnected;

            masterEmulator.ConnectionUpdateRequest += meEv_OnConnectionUpdateRequest; //public event EventHandler<ConnectionUpdateRequestEventArgs> ConnectionUpdateRequest;


            masterEmulator.DataReceived += meEv_OnDataReceived;                       //public event EventHandler<PipeDataEventArgs> DataReceived;
            //public event EventHandler<DataRequestedEventArgs> DataRequested;
            //public event EventHandler<ValueEventArgs<int>> DisplayPasskey;
            //public event EventHandler<OobKeyRequestEventArgs> OobKeyRequest;
            //public event EventHandler<PasskeyRequestEventArgs> PasskeyRequest;
            //public event EventHandler<PipeErrorEventArgs> PipeError;
            //public event EventHandler<SecurityRequestEventArgs> SecurityRequest;
        }

        /// <summary>
        /// Searching for master emulator devices attached to the pc. 
        /// If more than one is connected it will simply return the first in the list.
        /// </summary>
        /// <returns>Returns the first master emulator device found.</returns>
        string FindUsbDevice() 
        {
            /* The UsbDeviceType argument is used for filtering master emulator device types. */
            var devices = masterEmulator.EnumerateUsb(UsbDeviceType.AnyMasterEmulator);

            if (devices.Count > 0)
            {
                return devices[0];
            }
            else
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// Tell the api to use the given master emulator device.
        /// </summary>
        /// <param name="device"></param>
        void OpenMasterEmulatorDevice(string device) 
        {
            if (masterEmulator.IsOpen)
            {
                return;
            }
            masterEmulator.Open(device);
            masterEmulator.Reset();
        }

        /// <summary>
        /// By calling Run, the pipesetup is processed and the stack engine is started.
        /// </summary>
        void Run()
        {
            if (masterEmulator.IsRunning)
            {
                return;
            }
            masterEmulator.Run();
        }

        /// <summary>
        /// Collection of method calls to start and setup MasterEmulator.
        /// The calls are placed in a background task for not blocking the gui thread.
        /// </summary>
        public void Controller_Initialize()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    InitializeMasterEmulator();
                    RegisterEventHandlers();
                    
                    String device = FindUsbDevice();

                    OpenMasterEmulatorDevice(device);

                    //----- PipeSetup -----
                    udEngine = new UpDnEngine();
                    udEngine.UpDnEngine_Setup(masterEmulator);
                    udEngine.PerformPipeSetup();

                    set_default_local_udPacketHandler();


                    ctrlEngine = new CtrlEngine();
                    ctrlEngine.CtrlEngine_Setup(masterEmulator);
                    ctrlEngine.PerformPipeSetup();

                    //----- Start the MasterEmulator -----
                    Run();

                    //----- Send Out Initialized Event -----
                    appEv_Initialized(this, EventArgs.Empty);

                }
                catch (Exception ex)
                {
                    LogErrorMessage(string.Format("Exception in StartMasterEmulator: {0}", ex.Message),
                    ex.StackTrace);
                }
            });
        }

        /// <summary>
        /// Close MasterEmulator.
        /// </summary>
        public void Controller_Close()
        {
            if (!masterEmulator.IsOpen)
            {
                return;
            }

            masterEmulator.Close();
        }




        //=====================================================================
        //=====================================================================
        //=====================================================================
        //=====================================================================

        /// <summary>
        /// Device discovery is started with the given scan parameters.
        /// By stating active scan, we will be receiving data from both advertising
        /// and scan repsonse packets.
        /// </summary>
        /// <returns></returns>
        public bool StartDeviceScanning()
        {
            FLAG_autoConnect = false;
            m_deviceList.Clear();
 
            if (!masterEmulator.IsRunning)
            {
                AddToLog("Not ready.");
                return false;
            }

            BtScanParameters scanParameters = new BtScanParameters();
            scanParameters.ScanType = BtScanType.ActiveScanning;
            bool startSuccess = masterEmulator.StartDeviceDiscovery(scanParameters);

            if (startSuccess)
            {
                appEv_ScanningStarted(this, EventArgs.Empty);
            }

            return startSuccess;
        }

        public bool InitiateAutoConnect(String sDeviceId)
        {
            bool bStatus;

            FLAG_autoConnect = true;
            m_autoConnect_sDeviceId = sDeviceId;

            bStatus = StartDeviceScanning();
            if (bStatus == false)
            {
                FLAG_autoConnect = false;
                m_autoConnect_sDeviceId = "";
            }
            return (bStatus);
        }

        /// <summary>
        /// Stop scanning for devices.
        /// </summary>
        public void StopDeviceScanning() //S
        {
            if (!masterEmulator.IsDeviceDiscoveryOngoing)
            {
                return;
            }

            bool success = masterEmulator.StopDeviceDiscovery();

            if (success)
            {
                appEv_ScanningCancelled(this, EventArgs.Empty);
            }
        }






        //=====================================================================
        //=====================================================================
        //=====================================================================
        //=====================================================================
        //=====================================================================
        public bool Connect_to_device(BtDevice device)
        {
            bool bStatus;

            //if (masterEmulator.IsDeviceDiscoveryOngoing)
            while (masterEmulator.IsDeviceDiscoveryOngoing) // TODO (maybe need this during debug run
            {
                masterEmulator.StopDeviceDiscovery();
            }

            karelDumpDevice(device); //karel

            string deviceName = GetDeviceName(device.DeviceInfo);
            AddToLog(string.Format("Connecting to {0}, Device name: {1}",
                device.DeviceAddress.ToString(), deviceName));

            BtConnectionParameters connectionParams = new BtConnectionParameters();
            //TODO _nRFTD1 connection parameters may be different than for _nRFUart
            connectionParams.ConnectionIntervalMs = 11.25;
            connectionParams.ScanIntervalMs = 250;
            connectionParams.ScanWindowMs = 200;
            //TODO _nRFTD1 connection parameters may be different than for _nRFUart

            FLAG_connectionInProgress = true;
            bStatus = masterEmulator.Connect(device.DeviceAddress, connectionParams);
            if (bStatus == false)
            {
                FLAG_connectionInProgress = false;
            }
            return (bStatus);
        }

        public bool Connect_to_device_address(String sDeviceAddr)
        {
            bool bStatus;
            BtDevice device;
            int i;

            device = null;
            for ( i= 0; i < m_deviceList.Count; i++)
            {
                if (m_deviceList[i].DeviceAddress.Value == sDeviceAddr)
                {
                    device = m_deviceList[i];
                    break;
                }
            }
            if (device == null)
            {
                return (false);
            }

            bStatus = Connect_to_device(device);

            return (bStatus);
        }


        /// <summary>
        /// By discovering pipes, the pipe setup we have specified will be matched up
        /// to the remote device's ATT table by ATT service discovery.
        /// </summary>
        bool DiscoverPipes()
        {
            bool bStatus;

            bStatus = masterEmulator.DiscoverPipes();
            if (bStatus == false)
            {
                AddToLog("DiscoverPipes did not succeed.");
            }
            return(bStatus);
        }
        /// <summary>
        /// Pipes of type PipeType.Receive must be opened before they will start receiving notifications.
        /// This maps to ATT Client Configuration Descriptors.
        /// </summary>
        void OpenRemotePipes()
        {
            var openedPipesEnumeration = masterEmulator.OpenAllRemotePipes();
            List<int> openedPipes = new List<int>(openedPipesEnumeration);
        }

        /// <summary>
        /// This event handler is called when a connection has been successfully established.
        /// </summary>
        void meEv_OnConnected(object sender, EventArgs arguments)
        {
            if (appEv_Connected != null)
            {
                appEv_Connected(this, EventArgs.Empty);
            }

            /* NG here
            //karel - from Documentation
            karelLog("\r\n");
            karelLog("\r\n");
            karelLog("========== TESTING masterEmulator.DiscoverServices() ================\r\n");
            karelLog("\r\n");

            var attributes = masterEmulator.DiscoverServices();
            foreach (var item in attributes)
            {
                Trace.WriteLine(item.ToString());
                karelLog("-----> " + item.ToString());
            }
            */

            /* The connection is up, proceed with pipe discovery. 
             * Using a background task in order not to block the event caller. */
            Task.Factory.StartNew(() =>
            {
                try
                {
                    DiscoverPipes();
                    OpenRemotePipes();
                    appEv_ServiceDiscoveryCompleted(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    LogErrorMessage(string.Format("Exception in On_meEv_Connected: {0}", ex.Message),
                        ex.StackTrace);
                }
            });
        }


        /// <summary>
        /// Disconnect from peer device.
        /// </summary>
        public void InitiateDisconnect()
        {
            if (!masterEmulator.IsConnected)
            {
                return;
            }
            masterEmulator.Disconnect();
        }

        /// <summary>
        /// This event handler is called when a connection has been terminated.
        /// </summary>
        void meEv_OnDisconnected(object sender, ValueEventArgs<DisconnectReason> arguments)
        {
            FLAG_connectionInProgress = false;
            appEv_Disconnected(this, EventArgs.Empty);
        }

        /// <summary>
        /// This event handler is called when a connection update request has been received.
        /// A connection update must be responded to in two steps: sending a connection update
        /// response, and performing the actual update.
        /// </summary>
        void meEv_OnConnectionUpdateRequest(object sender, ConnectionUpdateRequestEventArgs arguments)
        {
            Task.Factory.StartNew(() =>
            {
                masterEmulator.SendConnectionUpdateResponse(arguments.Identifier,
                    ConnectionUpdateResponse.Accepted);
                BtConnectionParameters updateParams = new BtConnectionParameters();
                updateParams.ConnectionIntervalMs = arguments.ConnectionIntervalMinMs;
                updateParams.SupervisionTimeoutMs = arguments.ConnectionSupervisionTimeoutMs;
                updateParams.SlaveLatency = arguments.SlaveLatency;
                masterEmulator.UpdateConnectionParameters(updateParams);
            });
        }


        //=====================================================================
        //=====================================================================
        //=====================================================================
        //=====================================================================
        //=====================================================================













#if false

        /// <summary>
        /// Start sending a data to the peer device.
        /// </summary>
        /// <param name="data">An arbitrarily large byte array of data to send.</param>
        /// <remarks>The method will continue to send until all data has been
        /// transmitted or the transmission has been stopped by <see cref="StopSendData"/>.
        /// </remarks>
        public void UARTStartSendData(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException();
            }

            IList<byte[]> splitData = SplitDataAndAddCounter(data, maxPayloadLength);

            UARTsendData = true;

            /* Starting a task to perform the sending of packets asynchronouly.
             * The SendDataCompleted event will notify the application when ready. */
            Task.Factory.StartNew(() =>
            {
                try
                {
                    SendDataStarted(this, EventArgs.Empty);

                    int numberOfPackets = splitData.Count;
                    int progressInPercent = 0;

                    for (int i = 0; i < numberOfPackets; i++)
                    {
                        if (!UARTsendData)
                        {
                            break;
                        }

                        /* Send one packet of data on the UartRx pipe. */
                        masterEmulator.SendData(pipeSetup.UartRxPipe, splitData[i]);

                        int currentProgressInPercent = ((i + 1) * 100) / numberOfPackets;

                        if (currentProgressInPercent > progressInPercent)
                        {
                            progressInPercent = currentProgressInPercent;
                            ProgressUpdated(this, new ValueEventArgs<int>(progressInPercent));
                        }
                    }
                }
                catch (Exception ex)
                {
                    AddToLog("Sending of data failed.");
                    Trace.WriteLine(ex.ToString());
                }

                SendDataCompleted(this, EventArgs.Empty);
            });
        }

        /// <summary>
        /// Start to send data to the peer device.
        /// </summary>
        /// <param name="data">A byte array no larger than max packet size.</param>
        /// <param name="numberOfRepetitions">The number of times to repeat the packet.</param>
        public void StartSendData(byte[] data, int numberOfRepetitions)
        {
            if (data == null)
            {
                throw new ArgumentNullException();
            }

            if (data.Length > maxPayloadLength)
            {
                throw new ArgumentException(string.Format("Length of data must not exceed {0}.",
                    maxPayloadLength));
            }

            int totalPacketSize = data.Length * numberOfRepetitions;

            var aggregatedData = new List<byte>(totalPacketSize);

            for (int i = 0; i < numberOfRepetitions; i++)
            {
                aggregatedData.AddRange(data);
            }

            StartSendData(aggregatedData.ToArray());
        }

        /// <summary>
        /// Split to max length packets and leave room for counter values.
        /// </summary>
        IList<byte[]> SplitDataAndAddCounter(byte[] data, int partSize)
        {
            /* Collection of packets split out from source array. */
            IList<byte[]> packets = new List<byte[]>();

            /* Counter value, incremented for each new packet. */
            int counter = 0;

            /* Index of counter field in packet. */
            const int counterIndex = 0;

            /* Last index where a packet of full length may start. */
            /* Note const and readonly won't work here since const is compile-time const and */
            /* readonly requires this to be a member. Since SplitDataAndAddCounter is called multiple*/
            /* times, it breaks the readonly usage as well. */
            int lastFullPacketIndex = (data.Length - partSize);

            /* Current index in source array. */
            int index;

            for (index = 0; index < lastFullPacketIndex; index += partSize)
            {
                byte[] packet = new byte[maxPacketLength];
                Array.Copy(data, index, packet, counterFieldLength, partSize);

                InjectCounter(counter, counterIndex, packet);

                packets.Add(packet);

                counter += 1;
            }

            /* Special treatment of last packet. */
            int lastPacketPayloadSize = (data.Length - index);
            byte[] lastPacket = new byte[maxPacketLength];
            Array.Copy(data, index, lastPacket, counterFieldLength, lastPacketPayloadSize);
            InjectCounter(counter, counterIndex, lastPacket);

            packets.Add(lastPacket);

            return packets;
        }

        /// <summary>
        /// Insert 16 bit counter in Least Significant Byte order.
        /// </summary>
        void InjectCounter(int counter, int index, byte[] packet)
        {
            byte leastSignificantByte = (byte)(counter & 0xFF);
            byte mostSignificantByte = (byte)((counter >> 8) & 0xFF);

            packet[index] = leastSignificantByte;
            packet[index + 1] = mostSignificantByte;
        }
#endif
        /// <summary>
        /// Signal StartSendData task to cancel sending of data.
        /// </summary>
        //public void UARTStopSendData()  
        //{
        //    UARTsendData = false;
        //}


        /// <summary>
        ///  Method for adding text to the textbox and logfile.
        ///  When called on the main thread, invoke is not required.
        ///  For other threads, the invoke is required.
        /// </summary>
        /// <param name="message">The message string to add to the log.</param>
        void AddToLog(string message) 
        {
            if (LogMessage != null)
            {
                LogMessage(this, new OutputReceivedEventArgs(message));
            }

            // Writing to trace also, which causes the message to be put in the log file.
            Trace.WriteLine(message);
        }

        void karelLog(string message) 
        {
            AddToLog(message);
        }

        /// <summary>
        /// Convenience method for logging exception messages.
        /// </summary>
        void LogErrorMessage(string errorMessage, string stackTrace) 
        {
            AddToLog(errorMessage);
            Trace.WriteLine(stackTrace);
        }

        /// <summary>
        /// Get the path to the log file of MasterEmulator.
        /// </summary>
        /// <returns>Returns path to log file.</returns>
        public string GetLogfilePath() 
        {
            return masterEmulator.GetLogFilePath();

        }







        void karelDumpDevice(BtDevice device) 
        {

            IDictionary<DeviceInfoType, string> deviceInfo;
            BtDeviceAddress deviceAddress;
            string str = string.Empty;
            //string deviceName = string.Empty;
            //bool hasNameField;

            /*
             * $$$$ karelDumpDevice $$$$
             * --Device Address: 3C2DB785F142
             * $$$$ karelDumpDevice $$$$
             * --Flags: GeneralDiscoverable, BrEdrNotSupported
             * --ServicesCompleteListUuid128: 0xD0D0D0D00000000000000000DEADF154
             * --CompleteLocalName: SimpleBLEPeripheral
             * --TxPowerLevel: 0dBm
             * --SlaveConnectionIntervalRange: 10-00-20-00
             * --RandomTargetAddress: A0-A1-A2-A3-A4-A5
             * --Rssi: -65
             * $$$$ karelDumpDevice $$$$ [END]
             * 
             * $$$$ karelDumpDevice $$$$
             * --Device Address: DCB326B6E893
             * $$$$ karelDumpDevice $$$$
             * --Flags: LimitedDiscoverable, BrEdrNotSupported
             * --ServicesCompleteListUuid128: 0x6E400001B5A3F393E0A9E50E24DCCA9E
             * --CompleteLocalName: Nondick_UART
             * --Rssi: -45
             * $$$$ karelDumpDevice $$$$ [END]
             * 
             */
            deviceInfo = device.DeviceInfo;

            deviceAddress = device.DeviceAddress;


            karelLog("$$$$ karelDumpDevice $$$$");

            str = string.Format("--Device Address: {0}", deviceAddress.Value);
            karelLog(str);

            karelLog("$$$$ karelDumpDevice $$$$");

            //http://stackoverflow.com/questions/972307/can-you-loop-through-all-enum-values
            //var Avalues = Enum.GetValues(typeof(DeviceInfoType));
            var Bvalues = Enum.GetValues(typeof(DeviceInfoType)).Cast<DeviceInfoType>();

            foreach (var p in Bvalues)
            {
                if (deviceInfo.ContainsKey(p))
                {
                    str = string.Format("--{0}: {1}", p, deviceInfo[p]);
                    karelLog(str);
                }
            }

            karelLog("$$$$ karelDumpDevice $$$$ [END]");

        }


#if false
        /// <summary>
        /// Connecting to the given device, and with the given connection parameters.
        /// </summary>
        /// <param name="device">Device to connect to.</param>
        /// <returns>Returns success indication.</returns>
        //bool Connect(BtDevice device) 
        bool Connect_nRFUart(BtDevice device) 
        {
            if (masterEmulator.IsDeviceDiscoveryOngoing)
            {
                masterEmulator.StopDeviceDiscovery();
            }

            karelDumpDevice(device); //karel

            /* NG here*/
            //karel - from Documentation
            karelLog("\r\n");
            karelLog("\r\n");
            karelLog("========== TESTING masterEmulator.DiscoverServices() ================\r\n");
            karelLog("\r\n");

            var attributes = masterEmulator.DiscoverServices();
            foreach (var item in attributes)
            {
                Trace.WriteLine(item.ToString());
                karelLog("-----> " + item.ToString());
            }
            /**/

            string deviceName = GetDeviceName(device.DeviceInfo);
            AddToLog(string.Format("Connecting to {0}, Device name: {1}",
                device.DeviceAddress.ToString(), deviceName));

            BtConnectionParameters connectionParams = new BtConnectionParameters();
            connectionParams.ConnectionIntervalMs = 11.25;
            connectionParams.ScanIntervalMs = 250;
            connectionParams.ScanWindowMs = 200;
            bool connectSuccess = masterEmulator.Connect(device.DeviceAddress, connectionParams);
            return connectSuccess;
        }
#endif



        /// <summary>
        /// Event handler for DeviceDiscovered. This handler will be called when devices
        /// are discovered during asynchronous device discovery.
        /// </summary>
        void meEv_OnDeviceDiscovered(object sender, ValueEventArgs<BtDevice> arguments) 
        {
            /* Avoid call after a connect procedure is being started,
             * and the discovery procedure hasn't yet been stopped. */
            if (FLAG_connectionInProgress)
            {
                return;
            }

            BtDevice device = arguments.Value;

            if (!IsEligibleForConnection(device))
            {
                return;
            }
            
            //FLAG_connectionInProgress = true;

            bool devInList = false;
            int i;
            for (i = 0; i < m_deviceList.Count; i++)
            {
                BtDevice d = m_deviceList[i];
                if (d.DeviceAddress.Equals(device.DeviceAddress))
                {
                    m_deviceList[i] = device; // Update (Rssi)
                    Console.WriteLine("meEv_OnDeviceDiscovered - UPDATE");
                    devInList = true;
                    break;
                }
            }

            // Get Latest Info In String Form
            String sDeviceAddress = device.DeviceAddress.Value;
            String sDevInfo_Rssi = device.DeviceInfo[DeviceInfoType.Rssi];
            String sDevInfo_CompleteLocalName;
            try
            {
                sDevInfo_CompleteLocalName = device.DeviceInfo[DeviceInfoType.CompleteLocalName];
            }
            catch //(Exception ex)
            {
                sDevInfo_CompleteLocalName = "...";
            }
            if (!devInList)
            {
                m_deviceList.Add(device);
                Console.WriteLine("meEv_OnDeviceDiscovered - Added NEW device");
            }
            if ((!devInList) || (true))//(false))
            {
                if (appEv_DeviceInfoUpdate != null)
                    appEv_DeviceInfoUpdate(this, new MinDeviceInfoEventArgs(sDevInfo_CompleteLocalName, sDeviceAddress, sDevInfo_Rssi));
            }

            if (FLAG_autoConnect == false)
                return;

            //------------------------
            //----- Auto Connect -----
            devInList = false;
            for (i = 0; i < m_deviceList.Count; i++)
            {
                device = m_deviceList[i];
                if (device.DeviceAddress.Value == m_autoConnect_sDeviceId)
                {
                    devInList = true;
                    break;
                }
            }
            if(devInList == false)
                return;

            /* Start the connection procedure in a background task to avoid 
             * blocking the event caller. */
            Task.Factory.StartNew(() =>
            {
                try
                {
                    FLAG_connectionInProgress = true;
                    appEv_ConnectionStarted(this, EventArgs.Empty); // Connecting event to MainWindow
                    bool bStatus;
                    bStatus = this.Connect_to_device(device);
                    if (!bStatus)
                    {
                        FLAG_connectionInProgress = false;
                        appEv_ConnectionCancelled(this, EventArgs.Empty); // ConnectionCanceled event to MainWindow
                        return;
                    }
                }
                catch (Exception ex)
                {
                    LogErrorMessage(string.Format("Exception in OnDeviceDiscovered: {0}", ex.Message), ex.StackTrace);
                    FLAG_connectionInProgress = false;
                    appEv_ConnectionCancelled(this, EventArgs.Empty); // ConnectionCanceled event to MainWindow
                }
            });
        }

        /// <summary>
        /// Check if a device has the advertising data we are looking for.
        /// </summary>
        bool IsEligibleForConnection(BtDevice device) 
        {
            IDictionary<DeviceInfoType, string> deviceInfo = device.DeviceInfo;

            karelDumpDevice(device);
            /*
            bool hasServicesCompleteAdField =
                deviceInfo.ContainsKey(DeviceInfoType.ServicesCompleteListUuid128);

            if (!hasServicesCompleteAdField)
            {
                return false;
            }
            */
            bool hasHidServiceUuid;
            try
            {
                //#define NUS_BASE_UUID  {{0x9E, 0xCA, 0xDC, 0x24, 0x0E, 0xE5, 0xA9, 0xE0, 0x93, 0xF3, 0xA3, 0xB5, 0x00, 0x00, 0x40, 0x6E}} /**< Used vendor specific UUID. */
                //const string bleUartUuid = "6E400001B5A3F393E0A9E50E24DCCA9E";
                const string bleUartUuid = "6E400001B5A3F393E0A9E50E24DCCA42"; // ... 42 == T&D
                hasHidServiceUuid = deviceInfo[DeviceInfoType.ServicesCompleteListUuid128].Contains(bleUartUuid);
            }
            catch //(Exception ex)
            {
                hasHidServiceUuid = false;
            }
            if (!hasHidServiceUuid)
            {
                return false;
            }

            /* If we have reached here it means all the criterias have passed. */
            return true;
        }
        /*
        bool IsEligibleForConnection_nRFTD1(BtDevice device) 
        {
            IDictionary<DeviceInfoType, string> deviceInfo = device.DeviceInfo;

            karelDumpDevice(device);

            bool hasServicesCompleteAdField =
                deviceInfo.ContainsKey(DeviceInfoType.ServicesCompleteListUuid128);

            if (!hasServicesCompleteAdField)
            {
                return false;
            }

            //const string bleTD1Uuid = ".....";
            bool hasHidServiceUuid =
                    deviceInfo[DeviceInfoType.ServicesCompleteListUuid128].Contains(bleTD1Uuid);

            if (!hasHidServiceUuid)
            {
                return false;
            }

            // If we have reached here it means all the criterias have passed. 
            return true;
        }
        */
        //=====================================================================
        //=====================================================================
        //=====================================================================
        //=====================================================================
        //=====================================================================

        /// <summary>
        /// Extract the device name from the advertising data.
        /// </summary>
        string GetDeviceName(IDictionary<DeviceInfoType, string> deviceInfo) 
        {
            string deviceName = string.Empty;
            bool hasNameField = deviceInfo.ContainsKey(DeviceInfoType.CompleteLocalName);
            if (hasNameField)
            {
                deviceName = deviceInfo[DeviceInfoType.CompleteLocalName];
            }
            return deviceName;
        }


        /// <summary>
        /// This event handler is called when data has been received on any of our pipes.
        /// </summary>
        void meEv_OnDataReceived(Object sender, Nordicsemi.PipeDataEventArgs arguments)
        {
            System.Text.StringBuilder stringBuffer = new System.Text.StringBuilder();
            foreach (Byte element in arguments.PipeData)
            {
                stringBuffer.AppendFormat(" 0x{0:X2}", element);
            }
            if (DebugMessagesEnabled)
            {
                AddToLog(String.Format("Data received on pipe number {0}:{1}", arguments.PipeNumber, stringBuffer.ToString()));
            }


            udEngine.OnAnyPipe(sender, arguments);
            ctrlEngine.OnAnyPipe(sender, arguments);

            //REF if (arguments.PipeNumber == pipeSetup.UartTxPipe)
            //REF     OnDataReceived_Uart( sender, arguments);
            /*
            //===== udEngine =====
            if (arguments.PipeNumber == pipeSetup.DcfmPipe)
                udEngine.OnAnyPipe(sender, arguments); // OnDataReceived_Dn( sender, arguments);
        
            if( arguments.PipeNumber == pipeSetup.UcmdPipe )
                udEngine.OnAnyPipe(sender, arguments); // OnDataReceived_Up( sender, arguments);
            if( arguments.PipeNumber == pipeSetup.UdatPipe )
                udEngine.OnAnyPipe(sender, arguments); // OnDataReceived_Up( sender, arguments);

            //===== ctrlEngine =====
            if (arguments.PipeNumber == pipeSetup.WctrlPipe)
                ctrlEngine.OnAnyPipe(sender, arguments); // OnDataReceived_Wctrl(sender, arguments);
            if (arguments.PipeNumber == pipeSetup.RctrlPipe)
                ctrlEngine.OnAnyPipe(sender, arguments); // OnDataReceived_Rctrl(sender, arguments);
            */
        }

        //===== udEngine =====
        /*
        void OnDataReceived_Dn(Object sender, Nordicsemi.PipeDataEventArgs arguments)
        {
            udEngine.On_Dcfm( arguments.PipeData, arguments.PipeData.Length );
        }
        void OnDataReceived_Up(Object sender, Nordicsemi.PipeDataEventArgs arguments)
        {
            if( arguments.PipeNumber == pipeSetup.UcmdPipe )
                udEngine.On_Ucmd( arguments.PipeData, arguments.PipeData.Length );
            if( arguments.PipeNumber == pipeSetup.UdatPipe )
                udEngine.On_Udat( arguments.PipeData, arguments.PipeData.Length );
        }
        */

        //===== ctrlEngine =====
        /*
        void OnDataReceived_Wctrl(Object sender, Nordicsemi.PipeDataEventArgs arguments)
        {
            if (arguments.PipeNumber == pipeSetup.WctrlPipe)
                ctrlEngine.On_Wctrl(arguments.PipeData, arguments.PipeData.Length);
        }
        void OnDataReceived_Rctrl(Object sender, Nordicsemi.PipeDataEventArgs arguments)
        {
            if (arguments.PipeNumber == pipeSetup.RctrlPipe)
                //ctrlEngine.On_Rctrl(sender, arguments);
                ctrlEngine.On_Rctrl(arguments.PipeData, arguments.PipeData.Length);
        }
        */

        /// <summary>
        /// This event handler is called when data has been received on any of our pipes.
        /// </summary>
        /* REF
        void OnDataReceived_Uart(object sender, PipeDataEventArgs arguments)
        {
            if (arguments.PipeNumber != pipeSetup.UartTxPipe)
            {
                AddToLog("Received data on unknown pipe.");
                return;
            }

            StringBuilder stringBuffer = new StringBuilder();
            foreach (byte element in arguments.PipeData)
            {
                stringBuffer.AppendFormat(" 0x{0:X2}", element);
            }

            if (DebugMessagesEnabled)
            {
                AddToLog(string.Format("Data received on pipe number {0}:{1}", arguments.PipeNumber,
                    stringBuffer.ToString()));
            }


            byte[] utf8Array = arguments.PipeData;
            string convertedText = Encoding.UTF8.GetString(utf8Array);
            AddToLog(string.Format("RX: {0}", convertedText));
        }
        REF */

        /// <summary>
        /// Relay received log message events to the log method.
        /// </summary>
        void meEv_OnLogMessage(object sender, ValueEventArgs<string> arguments) 
        {
            string message = arguments.Value;

            if (message.Contains("Connected to"))
            {
                /* Don't filter out */
            }
            else if (message.Contains("Disconnected"))
            {
                return;
            }
            else if (!DebugMessagesEnabled)
            {
                return;
            }

            AddToLog(string.Format("{0}", arguments.Value));
        }













        //=====================================================================
        public Int32 PC_Send_udPacket(Byte[] pkt)
        {
            udEngine.Dn_Send_CMD_11_Pkt(pkt);
            return (0);
        }

        public Int32 PC_Recv_udPacket(ref Byte[] pkt)
        {
            return (0);
        }
        //=====================================================================

        public delegate Int32 On_udPacketCallback_DEL(byte[] pkt);

        Int32 local_ON_udPacket(Byte[] packet)
        {
            if (userEv_udPacket != null)
                userEv_udPacket(this, new udPacketEventArgs(packet, packet.Length));
            return (0);
        }

        void set_default_local_udPacketHandler()
        {
            udEngine.set_rxCallback(local_ON_udPacket);
        }

        public Int32 PC_Set_callbackOn_udPacket(UpDnEngine.L6_OnDataRxCallback_DEL fn)
        {
            udEngine.set_rxCallback(fn);
            return (0);
        }


    }
}
