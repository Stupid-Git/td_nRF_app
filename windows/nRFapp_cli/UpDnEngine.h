#pragma once

#include <stdint.h>

//#using <MasterEmulator.dll>
#using <C:\ble_nrf51\Master Emulator\2.1.12.6\MasterEmulator.dll>
//#using Nordicsemi;
//#using namespace Nordicsemi;
#include "PipeSetup.h"

#include "Protocol_T2.h"

using namespace System;
using namespace System::Threading;

namespace TDnRF
{

    using namespace System;
    using namespace System::ComponentModel;
    using namespace System::Collections;
    using namespace System::Collections::Generic;
    using namespace System::Windows::Forms;
    using namespace System::Data;
    using namespace System::Drawing;

    public ref class UpDnEngine
    {

    public:
        Nordicsemi::MasterEmulator^ masterEmulator;
        PipeSetup^ pipeSetup;

        Protocol_T2^ protocol_T2; // 'T'

        void UpDnEngine_Setup(Nordicsemi::MasterEmulator^ master, PipeSetup^ pipe)
        {
            masterEmulator = master;
            pipeSetup = pipe;
        }

    public:
        UpDnEngine(void)
        {
            protocol_T2 = gcnew Protocol_T2(); // 'T'
        };

        //=====================================================================
        //=====================================================================
        //===== DN ============================================================
        //=====================================================================
        //=====================================================================
        int32_t Dn_Enable()
        {
            return(0);
        }

        int32_t Dn_Send_CMD_12()
        {
            array<uint8_t,1>^ buf = gcnew array<uint8_t,1>(20);
            int32_t r;
            //--- Make the command ---
            buf[0] = 1;
            buf[1] = 2;
            r = Dn_Send_Dcmd(buf, 20);
            return(r);
        }

        int32_t Dn_Send_CMD_13()
        {
            array<uint8_t,1>^ buf = gcnew array<uint8_t,1>(20);
            int32_t r;
            //--- Make the command ---
            buf[0] = 1;
            buf[1] = 3;
            r = Dn_Send_Dcmd(buf, 20);
            return(r);
        }


        int32_t Dn_Send_CMD_11_Pkt(array<System::Byte,1>^ pktbuf)
        {            
            int  i;    
            array<uint8_t,1>^ buf = gcnew array<uint8_t,1>(20);
            uint8_t length;
            uint8_t  blk_no;    
            uint16_t ofst;
            
            uint16_t totalLength;
            uint16_t total_cs;
            uint8_t total_cs0;
            uint8_t total_cs1;

            int32_t r;

            totalLength = (UInt16)(pktbuf->Length + 2);
            total_cs = 0;
            for (i = 0; i < pktbuf->Length; i++)
            {
                total_cs += pktbuf[i];
            }
            total_cs0 = (uint8_t)((total_cs >> 0) & 0x00FF);
            total_cs1 = (uint8_t)((total_cs >> 8) & 0x00FF);
            
            //--- Make the command ---
            buf[0] = 1;
            buf[1] = 1;
            //buf[2] = (pktbuf->Length >> 0) & 0x00ff;
            //buf[3] = (pktbuf->Length >> 8) & 0x00ff;
            buf[2] = (uint8_t)(((totalLength) >> 0) & 0x00ff);
            buf[3] = (uint8_t)(((totalLength) >> 8) & 0x00ff);
            r = Dn_Send_Dcmd(buf, 20);

            ofst = 0;
            blk_no = 0;
            do
            {
                //--- Set the Data ---
                ofst = blk_no * 16;
                if( ofst > pktbuf->Length)
                    break;

                buf[0] = blk_no++;
                buf[1] = 0x00;
                buf[2] = 0x00;
                buf[3] = 0x00;
                for(i=0; i<16; i++)
                {
                    if ((ofst + i) < pktbuf->Length)
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
                    if(ofst +i < pktbuf->Length)
                        buf[4 + i] = pktbuf[ofst + i];
                    else
                        buf[4 + i] = 0x00;
                */
                length = 20;
                r = Dn_Send_Ddat(buf, 20);
                if(r != 0)
                    break;

            } while(1);

            return(0);
        }

        //---------------------------------------------------------------------
        int32_t Dn_Send_T2_DummyTEST1()
        {
            int  i;    
            array<uint8_t,1>^ buf = gcnew array<uint8_t,1>(20);

            uint16_t dataSize = 10;

            // fake make packet
            array<System::Byte,1>^ pktbuf = gcnew array<Byte,1>(dataSize + 6);
            uint16_t cs_pkt;

            pktbuf[0] = 'T';
            pktbuf[1] = '2';
            pktbuf[2] = (dataSize >> 0) & 0x00FF; //Len LSB
            pktbuf[3] = (dataSize >> 8) & 0x00FF; //Len MSB

            pktbuf[4] = 'a';
            pktbuf[5] = 'b';
            pktbuf[6] = 'c';
            pktbuf[7] = 'd';
            pktbuf[8] = 'e';
            pktbuf[9] = 'A';
            pktbuf[10] = 'B';
            pktbuf[11] = 'C';
            pktbuf[12] = 'D';
            pktbuf[13] = 'E';

            cs_pkt = 0;
            for( i=4; i<pktbuf->Length - 2 ; i++)
            {
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf->Length - 2] = (cs_pkt >> 0) & 0x00FF;
            pktbuf[pktbuf->Length - 1] = (cs_pkt >> 8) & 0x00FF;


            Dn_Send_CMD_11_Pkt(pktbuf);
            return(0);
        }
        
        int32_t T2_RUINF()
        {
 
            array<System::Byte,1>^ pktbuf;// = gcnew array<Byte,1>(dataSize + 6);

            pktbuf = protocol_T2->RUINF_get_send_packet();
            Dn_Send_CMD_11_Pkt(pktbuf);

            return(0);

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

            sendCommand();
            t_recvCommand();

            extraction();   // パラメータ抽出

        error_proc: ;
            richTextBox1.AppendText("終了\r\n");
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
            TODO*/
        }

        //---------------------------------------------------------------------
        int32_t Dn_Send_0x01_CMD_01_0x58()
        {
            int  i;    
            array<uint8_t,1>^ buf = gcnew array<uint8_t,1>(20);

            uint8_t cmd = 0x58; //CMD_GETSERIAL; CMD_01_0x58
            uint8_t subcmd = 0;
            uint16_t dataSize = 0;

            // fake make packet
            array<System::Byte,1>^ pktbuf = gcnew array<Byte,1>(7);
            uint16_t cs_pkt;

            pktbuf[0] = 0x01;
            pktbuf[1] = cmd;
            pktbuf[2] = subcmd;
            pktbuf[3] = (dataSize >> 0) & 0x00FF; //Len LSB
            pktbuf[4] = (dataSize >> 8) & 0x00FF; //Len MSB

            cs_pkt = 0;
            for( i=0; i<pktbuf->Length - 2 ; i++)
            {
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf->Length - 2] = (cs_pkt >> 0) & 0x00FF;
            pktbuf[pktbuf->Length - 1] = (cs_pkt >> 8) & 0x00FF;


            Dn_Send_CMD_11_Pkt(pktbuf);
            return(0);
        }

        //---------------------------------------------------------------------
        int32_t Dn_Send_0x01_CMD_01_0xF5(uint16_t recCount)
        {
            int  i;    
            uint8_t cmd = 0xF5; //CMD_01_0xF5;
            uint8_t subcmd = 0;
            uint16_t dataSize = 2;
            uint16_t cs_pkt;

            array<System::Byte,1>^ pktbuf = gcnew array<Byte,1>(dataSize + 7);

            pktbuf[0] = 0x01;
            pktbuf[1] = cmd;
            pktbuf[2] = subcmd;
            pktbuf[3] = (dataSize >> 0) & 0x00FF; //Len LSB
            pktbuf[4] = (dataSize >> 8) & 0x00FF; //Len MSB
            pktbuf[5] = (recCount >> 0) & 0x00FF; //Len LSB
            pktbuf[6] = (recCount >> 8) & 0x00FF; //Len MSB

            cs_pkt = 0;
            for( i=0; i<pktbuf->Length - 2 ; i++)
            {
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf->Length - 2] = (cs_pkt >> 0) & 0x00FF;
            pktbuf[pktbuf->Length - 1] = (cs_pkt >> 8) & 0x00FF;


            Dn_Send_CMD_11_Pkt(pktbuf);
            return(0);
        }
        
        //---------------------------------------------------------------------
        int32_t Dn_Send_0x01_CMD_01_0xF8()
        {
            int  i;    
            uint8_t cmd = 0xF8; //CMD_01_0xF8;
            uint8_t subcmd = 0;
            uint16_t dataSize = 0;
            uint16_t cs_pkt;

            array<System::Byte,1>^ pktbuf = gcnew array<Byte,1>(dataSize + 7);

            pktbuf[0] = 0x01;
            pktbuf[1] = cmd;
            pktbuf[2] = subcmd;
            pktbuf[3] = (dataSize >> 0) & 0x00FF; //Len LSB
            pktbuf[4] = (dataSize >> 8) & 0x00FF; //Len MSB

            cs_pkt = 0;
            for( i=0; i<pktbuf->Length - 2 ; i++)
            {
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf->Length - 2] = (cs_pkt >> 0) & 0x00FF;
            pktbuf[pktbuf->Length - 1] = (cs_pkt >> 8) & 0x00FF;


            Dn_Send_CMD_11_Pkt(pktbuf);
            return(0);
        }

        //---------------------------------------------------------------------
        int32_t Dn_Send_0x01_CMD_01_0xF9(int32_t size)
        {
            int  i;    
            uint8_t cmd = 0xF9; //CMD_01_0xF9
            uint8_t subcmd = 0;
            uint16_t dataSize = 2;
            uint16_t cs_pkt;

            array<System::Byte,1>^ pktbuf = gcnew array<Byte,1>(dataSize + 7);

            pktbuf[0] = 0x01;
            pktbuf[1] = cmd;
            pktbuf[2] = subcmd;
            pktbuf[3] = (dataSize >> 0) & 0x00FF; //Len LSB
            pktbuf[4] = (dataSize >> 8) & 0x00FF; //Len MSB

            pktbuf[5] = ((size >> 0) &0x00ff);
            pktbuf[6] = ((size >> 8) &0x00ff);

            cs_pkt = 0;
            for( i=0; i<pktbuf->Length - 2 ; i++)
            {
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf->Length - 2] = (cs_pkt >> 0) & 0x00FF;
            pktbuf[pktbuf->Length - 1] = (cs_pkt >> 8) & 0x00FF;


            Dn_Send_CMD_11_Pkt(pktbuf);
            return(0);
        }

        //---------------------------------------------------------------------
        int32_t Dn_Send_0x01_CMD_01_0x44(int16_t byteCount)
        {
            int  i;    
            uint8_t cmd = 0x44; //CMD_01_0x44;
            uint8_t subcmd = 0;
            uint16_t dataSize = 2;
            uint16_t cs_pkt;

            array<System::Byte,1>^ pktbuf = gcnew array<Byte,1>(dataSize + 7);

            pktbuf[0] = 0x01;
            pktbuf[1] = cmd;
            pktbuf[2] = subcmd;
            pktbuf[3] = (dataSize >> 0) & 0x00FF; //Len LSB
            pktbuf[4] = (dataSize >> 8) & 0x00FF; //Len MSB

            pktbuf[5] = ((byteCount >> 0) &0x00ff);
            pktbuf[6] = ((byteCount >> 8) &0x00ff);

            cs_pkt = 0;
            for( i=0; i<pktbuf->Length - 2 ; i++)
            {
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf->Length - 2] = (cs_pkt >> 0) & 0x00FF;
            pktbuf[pktbuf->Length - 1] = (cs_pkt >> 8) & 0x00FF;


            Dn_Send_CMD_11_Pkt(pktbuf);
            return(0);
        }

        //---------------------------------------------------------------------
        int32_t Dn_Send_0x01_CMD_01_0x45(int16_t blockNum)
        {
            int  i;    
            uint8_t cmd = 0x45; //CMD_01_0x45;
            uint8_t subcmd = 0x06;
            uint16_t dataSize = 2;
            uint16_t cs_pkt;

            array<System::Byte,1>^ pktbuf = gcnew array<Byte,1>(dataSize + 7);

            pktbuf[0] = 0x01;
            pktbuf[1] = cmd;
            pktbuf[2] = subcmd;
            pktbuf[3] = (dataSize >> 0) & 0x00FF; //Len LSB
            pktbuf[4] = (dataSize >> 8) & 0x00FF; //Len MSB

            pktbuf[5] = ((blockNum >> 0) &0x00ff);
            pktbuf[6] = ((blockNum >> 8) &0x00ff);

            cs_pkt = 0;
            for( i=0; i<pktbuf->Length - 2 ; i++)
            {
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf->Length - 2] = (cs_pkt >> 0) & 0x00FF;
            pktbuf[pktbuf->Length - 1] = (cs_pkt >> 8) & 0x00FF;


            Dn_Send_CMD_11_Pkt(pktbuf);
            return(0);
        }


        //---------------------------------------------------------------------
        int32_t Dn_Dummy_Send42()
        {            
            int  i;    
            array<uint8_t,1>^ buf = gcnew array<uint8_t,1>(20);
            // fake make packet
            array<System::Byte,1>^ pktbuf = gcnew array<Byte,1>(42);
            uint16_t cs_pkt;
            cs_pkt = 0;
            for( i=0; i<pktbuf->Length - 2 ; i++)
            {
                pktbuf[i] = (uint8_t)(i & 0x00FF);
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf->Length - 2] = (cs_pkt >> 0) & 0x00FF;
            pktbuf[pktbuf->Length - 1] = (cs_pkt >> 8) & 0x00FF;


            Dn_Send_CMD_11_Pkt(pktbuf);
            return(0);
        }


        //---------------------------------------------------------------------
        int32_t Dn_Dummy_Send43()
        {            
            int  i;    
            array<uint8_t,1>^ buf = gcnew array<uint8_t,1>(20);
            uint8_t length;
            uint8_t  blk_no;    
            uint16_t ofst;

            int32_t r;
            // fake make packet
            array<System::Byte,1>^ pktbuf = gcnew array<Byte,1>(42);
            uint16_t cs_pkt;
            cs_pkt = 0;
            for( i=0; i<pktbuf->Length - 2 ; i++)
            {
                pktbuf[i] = (uint8_t)(i & 0x00FF);
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf->Length - 2] = (cs_pkt >> 0) & 0x00FF;
            pktbuf[pktbuf->Length - 1] = (cs_pkt >> 8) & 0x00FF;



            //--- Make the command ---
            buf[0] = 1;
            buf[1] = 1;
            buf[2] = (pktbuf->Length >> 0) & 0x00ff;
            buf[3] = (pktbuf->Length >> 8) & 0x00ff;
            r = Dn_Send_Dcmd(buf, 20);

            ofst = 0;
            blk_no = 0;
            do
            {
                //--- Set the Data ---
                ofst = blk_no * 16;
                if( ofst > pktbuf->Length)
                    break;

                buf[0] = blk_no++;
                buf[1] = 0x00;
                buf[2] = 0x00;
                buf[3] = 0x00;
                for(i=0; i<16; i++)
                    if(ofst +i < pktbuf->Length)
                        buf[4 + i] = pktbuf[ofst + i];
                    else
                        buf[4 + i] = 0x00;

                length = 20;
                if(blk_no!=2)
                {
                    r = Dn_Send_Ddat(buf, 20);
                    if(r != 0)
                        break;
                }

            } while(1);

            return(0);
        }

        //---------------------------------------------------------------------
        int32_t Dn_Dummy_Send43B()
        {            
            int  i;    
            array<uint8_t,1>^ buf = gcnew array<uint8_t,1>(20);
            uint8_t length;
            uint8_t  blk_no;    
            uint16_t ofst;

            int32_t r;
            // fake make packet
            array<System::Byte,1>^ pktbuf = gcnew array<Byte,1>(42);
            uint16_t cs_pkt;
            cs_pkt = 0;
            for( i=0; i<pktbuf->Length - 2 ; i++)
            {
                pktbuf[i] = (uint8_t)(i & 0x00FF);
                cs_pkt += pktbuf[i];
            }
            pktbuf[pktbuf->Length - 2] = (cs_pkt >> 0) & 0x00FF;
            pktbuf[pktbuf->Length - 1] = (cs_pkt >> 8) & 0x00FF;


            ofst = 0;
            blk_no = 0;
            do
            {
                //--- Set the Data ---
                ofst = blk_no * 16;
                if( ofst > pktbuf->Length)
                    break;

                buf[0] = blk_no++;
                buf[1] = 0x00;
                buf[2] = 0x00;
                buf[3] = 0x00;
                for(i=0; i<16; i++)
                    if(ofst +i < pktbuf->Length)
                        buf[4 + i] = pktbuf[ofst + i];
                    else
                        buf[4 + i] = 0x00;

                length = 20;
                if(blk_no == 2)
                {
                    r = Dn_Send_Ddat(buf, 20);
                    if(r != 0)
                        break;
                }

            } while(1);

            return(0);
        }



        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        int32_t UpDn_Write_Wctrl_test(array<System::Byte,1>^ p_buf, int len)
        {
            bool bret;
            int32_t r;
            r = 0;

            Console::WriteLine("UpDn_Write_Wctrl_test...");
            bret = masterEmulator->SendData(pipeSetup->WctrlPipe, p_buf);
            if(bret == true)
            {
                Console::WriteLine("UpDn_Write_Wctrl_test: TRUE");
            }
            if(bret == false)
            {
                Console::WriteLine("UpDn_Write_Wctrl_test: FALSE");
            }

            return(r);
        }

        int32_t UpDn_Read_Rctrl_test()//(array<System::Byte,1>^ p_buf, int len)
        {
            int32_t i;
            int32_t r;
            r = 0;
            array<System::Byte,1>^ r_buf;


            Console::WriteLine("UpDn_Read_Rctrl_test...");
            r_buf = masterEmulator->RequestData(pipeSetup->RctrlPipe);
            if(r_buf == nullptr)
            {
                Console::WriteLine("UpDn_Read_Rctrl_test: r_buf == nullptr");
            }

            Console::WriteLine("r_buf->Length = {0}", r_buf->Length);
            if( r_buf->Length > 0)
            {
                for(i=0 ; i<r_buf->Length; i++)
                    Console::Write(" {0:X2}", r_buf[i]);
                Console::WriteLine("");

            }
            return(r);
        }

        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        int32_t Dn_Send_Dcmd( array<System::Byte,1>^ p_buf, int len)
        {
            int32_t r;
            r = 0;

            masterEmulator->SendData(pipeSetup->DcmdPipe, p_buf);
            return(r);
        }
        //---------------------------------------------------------------------
        int32_t Dn_Send_Ddat( array<System::Byte,1>^ p_buf, int len)
        {
            int32_t r;
            r = 0;

            masterEmulator->SendData(pipeSetup->DdatPipe, p_buf);
            return(r);
        }
        //---------------------------------------------------------------------
        System::Void On_Dcfm( array<System::Byte,1>^ p_buf, int len )
        {
            if(p_buf[0] == 1)
            {
                if(p_buf[1] == 0)
                    Console::WriteLine("Dcfm: OK");
                if(p_buf[1] == 1)
                    Console::WriteLine("Dcfm: NG");
                if(p_buf[1] == 2)
                {
                    Console::WriteLine("Dcfm: RESEND");
                    //[2]=1, [3]=1
                    Dn_Dummy_Send43B();
                }
            }
            else
            {
                //AddToLog(String::Format("Data received on pipe number {0}:{1}", arguments->PipeNumber, stringBuffer->ToString()));
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

        uint8_t m_blkDn_buf[ 16 * BLK_DN_COUNT ]; // 2048 @ BLK_DN_COUNT = 128
        uint8_t m_blkDn_chk[  1 * BLK_DN_COUNT ]; //  128 @ BLK_DN_COUNT = 128
        */
        array<uint8_t,1>^ m_blkDn_buf;
        array<uint8_t,1>^ m_blkDn_chk;
        uint16_t m_blkDn_len;
        uint16_t m_blkDn_blkCnt;
        uint16_t m_blkDn_rxBlkCnt;

        //static uint8_t  m_Dcfm_buf[20];
        //static uint16_t m_Dcfm_len;
        array<uint8_t,1>^ m_Dcfm_buf;
        uint16_t m_Dcfm_len;

        //-----------------------------------------------------------------------------
        int32_t blk_dn_start( array<uint8_t,1>^ pkt )
        {
            uint8_t  j;
            uint16_t i;

            //----- BOGUS INIT -----
            BLK_DN_COUNT = 128;
            m_blkDn_buf = gcnew array<uint8_t,1>(16 * 128);
            m_blkDn_chk = gcnew array<uint8_t,1>( 1 * 128);

            m_Dcfm_buf = gcnew array<uint8_t,1>(20);
            //----- BOGUS INIT -----

            m_blkDn_rxBlkCnt = 0;
            m_blkDn_blkCnt = 0;    

            m_blkDn_len = pkt[2] | (pkt[3]<<8);
            if( m_blkDn_len == 0)
                return(1);

            m_blkDn_blkCnt = ((m_blkDn_len - 1) / 16) + 1;


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


        int32_t blk_dn_add( array<uint8_t,1>^ pkt, uint16_t len )
        {
            uint8_t  position;
            uint8_t  j;
            //    uint16_t i;

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

#define DN_CHK_OK 0
#define DN_CHK_CHKSUM_NG 3

        int32_t blk_dn_chk()
        {
            //    uint8_t  j;
            uint16_t i;
            uint16_t missing_blk_cnt;
            uint16_t cs_pkt;
            uint16_t cs_now;

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

            cs_pkt = m_blkDn_buf[m_blkDn_len - 2] | (m_blkDn_buf[m_blkDn_len - 1]<<8);
            cs_now = 0;
            for(i=0 ; i < m_blkDn_len - 2; i++)
            {
                cs_now += m_blkDn_buf[i];
            }
            if( cs_now != cs_pkt)
                return( DN_CHK_CHKSUM_NG );

            return(DN_CHK_OK);
        }

        int32_t blk_dn_missing_count()
        {
            uint16_t i;
            uint16_t missing_blk_cnt;

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

        int32_t blk_dn_get_missing(array<uint8_t,1>^ buf, uint8_t ofst3, uint8_t count)
        {
            uint16_t i;
            uint16_t missing_blk_cnt;

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
                        buf[ofst3 + missing_blk_cnt - 1] = (uint8_t)i;
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

                Console::WriteLine("blk_dn_checkT2");
                
                Console::WriteLine();
                Console::WriteLine();
                Console::Write("Got T2 Response\n");
                Console::WriteLine();
                Console::WriteLine();

                protocol_T2->RUINF_process_recv_packet(m_blkDn_buf); // 'T'
            }

        }

        void blk_dn_check0x01()
        {

            if( m_blkDn_buf[0] == 0x01 )
            {
                Console::WriteLine("\nblk_dn_check0x01");

                if(m_blkDn_buf[1] == 0x44)
                {
                    Console::WriteLine();
                    Console::WriteLine();
                    Console::Write("Command 0x44: Status  = {0:x2}\n", m_blkDn_buf[2] );
                    Console::Write("              Len     = {0}\n"   , (m_blkDn_buf[4]<<8) + (m_blkDn_buf[3]) );
                    Console::Write("              data[0] = {0:x2}\n", m_blkDn_buf[5]);
                    Console::Write("              data[1] = {0:x2}\n", m_blkDn_buf[6]);
                    Console::WriteLine();
                    Console::WriteLine();
                }
                if(m_blkDn_buf[1] == 0x45)
                {
                    Console::WriteLine();
                    Console::WriteLine();
                    Console::Write("Command 0x45: Status  = {0:x2}\n", m_blkDn_buf[2] );
                    Console::Write("              Len     = {0}\n"   , (m_blkDn_buf[4]<<8) + (m_blkDn_buf[3]) );
                    Console::Write("              data[0] = {0:x2}\n", m_blkDn_buf[5]);
                    Console::Write("              data[1] = {0:x2}\n", m_blkDn_buf[6]);
                    Console::WriteLine();
                    Console::WriteLine();
                }


                if(m_blkDn_buf[1] == 0x58)
                {
                    Console::WriteLine();
                    Console::WriteLine();
                    Console::Write("Serial Number = ");
                    Console::Write("{0:x2}", m_blkDn_buf[8] );
                    Console::Write("{0:x2}", m_blkDn_buf[7] );
                    Console::Write("{0:x2}", m_blkDn_buf[6] );
                    Console::Write("{0:x2}", m_blkDn_buf[5] );
                    Console::WriteLine();
                    Console::WriteLine();
                }
                if(m_blkDn_buf[1] == 0xF8)
                {
                    Console::WriteLine();
                    Console::WriteLine();
                    Console::Write("Command 0xF8 Response\n");
                    Console::WriteLine();
                    Console::WriteLine();
                }
                if(m_blkDn_buf[1] == 0xF9)
                {
                    Console::WriteLine();
                    Console::WriteLine();
                    Console::Write("Command 0xF9 Response\n");
                    Console::WriteLine();
                    Console::WriteLine();
                }
            }

        }

        bool app_uart_put(char c)
        {
            return(true);
        };

        //-----------------------------------------------------------------------------
        //
        //  Controls
        //
        //-----------------------------------------------------------------------------
        uint32_t start_send_Dcfm_OK()
        {
            //            uint32_t err_code;

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

        uint32_t start_send_Dcfm_NG()
        {
            //            uint32_t err_code;

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

        uint32_t start_send_Dcfm_missing(int max_entries)
        {
            //            uint32_t err_code;

            m_Dcfm_buf[0] = 1;
            m_Dcfm_buf[1] = 2; // 2 = Missing

            int n = blk_dn_missing_count();
            if( n > max_entries )
                n = max_entries;
            m_Dcfm_buf[2] = n;
            //blk_dn_get_missing( &m_Dcfm_buf[3], n );
            blk_dn_get_missing( m_Dcfm_buf, 3, n );
            m_Dcfm_len = 3 + n;

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
        int32_t On_Ucmd( array<System::Byte,1>^ buf, int len )
        {
            int32_t r;

            if( (buf[0] == 1) && (buf[1] == 1) )
            {
                ///m_BlkDn_packetWaitTimeCount = 0;
                ///while(app_uart_put('S') != NRF_SUCCESS);
                r = blk_dn_start(buf);
                ///m_BlkDn_sm = eBlkDn_WAIT_PACKET;

            }
            return(r);

        }
        int32_t On_Udat( array<System::Byte,1>^ buf, int len )
        {
            int32_t r;

            ///stop_Dn_timer();
            ///m_BlkDn_packetWaitTimeCount = 0;
            r = blk_dn_add( buf, (uint16_t)len);

            r = blk_dn_chk();
            if(r == DN_CHK_OK)
            {
                ///while(app_uart_put('O') != NRF_SUCCESS);
                //m_BlkDn_sm = eBlkDn_GOT_PACKET;
                start_send_Dcfm_OK();

                Console::Write("\nDN_CHK_OK: m_blkDn_len = {0}\n", m_blkDn_len);
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



        int32_t Un_Send_Ucfm( array<System::Byte,1>^ p_buf, int len)
        {
            int32_t r;
            r = 0;

            masterEmulator->SendData(pipeSetup->UcfmPipe, p_buf);
            return(r);
        }


    };

}
