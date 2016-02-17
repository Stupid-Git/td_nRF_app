#pragma once


using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Collections::Generic;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


using namespace System::Threading;

#include <stdint.h>

namespace TDnRF
{

    ref class Protocol_T2
    {

        static ref class richTextBox1
        {
        public:
            static void AppendText(String^ s)
            {
                Console::WriteLine( s );
            }
        };

        array<uint8_t,1>^ sData;
        array<uint8_t,1>^ rData;
        array<uint8_t,1>^ tData;

    public:
        Protocol_T2(void)
        {
            sData = gcnew array<uint8_t>(1024 * 10 + 128);
            rData = gcnew array<uint8_t>(1024 * 10 + 128);
            tData = gcnew array<uint8_t>(1024 * 10);
        }

    private:
        void sendCommand()
        {
            UInt16 i, len, sum, t;

            /*
            serialPort1.DiscardInBuffer();
            serialPort1.DiscardOutBuffer();
            */
            rData[0] = 0x00;

            // �s�R�}���h�Ȃ�擪�o�C�g���폜����
            if (sData[1] == (Byte)'T') t = 1;
            else t = 0;

            len = (UInt16)(sData[3] + (UInt16)sData[4] * 256);

            for (sum = 0, i = t; i < len + 5; i++)
            {
                if (t == 1)
                {
                    if (i >= 5) sum += sData[i];
                }
                else sum += sData[i];
            }

            sData[len + 5] = (Byte)sum;
            sData[len + 6] = (Byte)(sum / 256);

            /*
            serialPort1.Write(sData, t, len + 7 - t);

            Application.DoEvents();
            */
        }

        // �P�o�C�g���������������֕ϊ�

    private:
        String^ hex(Byte h)
        {
            char C = (char)h;
            const char * dummyChars = &C;//{ Convert.ToChar(h) };
            return (gcnew String(dummyChars, 0, 1));

            //char[] dummyChars = { Convert.ToChar(h) };
            //return (gcnew String(dummyChars));
        }



        // �s�R�}���h��M

    private:
        Boolean t_recvCommand()
        {
            /*
            int n;

            UInt16 i, j, len, sum;

            int rt, wt;
            
            rt = serialPort1.ReadTimeout;
            wt = serialPort1.WriteTimeout;

            if(serial_port == 0)
            {
            serialPort1.ReadTimeout = 5000;
            serialPort1.WriteTimeout = 5000;
            }
            else
            {
            serialPort1.ReadTimeout = 3000;
            serialPort1.WriteTimeout = 3000;
            }

            for (i = 0; i < 4; i++)
            {
            try
            {
            rData[i] = (byte)serialPort1.ReadByte();
            }
            catch (TimeoutException)
            {
            serialPort1.ReadTimeout = rt;
            serialPort1.WriteTimeout = wt;
            return false;
            }

            Application.DoEvents();
            }
            */
            if (rData[0] != 'T')
            {
                /*
                serialPort1.ReadTimeout = rt;
                serialPort1.WriteTimeout = wt;
                */
                return false;
            }
            /*
            len = (UInt16)(BitConverter.ToUInt16(rData, 2) + 6);

            i = 0;
            time_out = serialPort1.ReadTimeout;

            try
            {
            while ((j = (UInt16)serialPort1.BytesToRead) < (len - 4))         // �\��o�C�g������M����Ă���q����������Ƒ���
            {
            if (time_out == 0) return false;
            Application.DoEvents();
            }

            while (i < (len - 4))
            {
            n = serialPort1.Read(rData, 4 + i, len - 4 - i);
            if (n > 0) i += (UInt16)n;

            Application.DoEvents();
            }
            }
            catch (TimeoutException)
            {
            serialPort1.ReadTimeout = rt;
            serialPort1.WriteTimeout = wt;
            return false;
            }

            sum = 0;

            for (i = 0; i < len; i++)
            {
            // �`�F�b�N�T���v�Z
            if ((i < (len - 2)) && (i > 3)) sum += rData[i];
            }

            serialPort1.ReadTimeout = rt;
            serialPort1.WriteTimeout = wt;

            if ((sum & 0xffff) != (rData[len - 2] + (int)rData[len - 1] * 256)) return false;
            */
            return true;
        }



        // �p�����[�^����T��

    private:
        int key_find(String^ key)
        {

            System::Text::Encoding^ enc = gcnew System::Text::ASCIIEncoding();

            /*
            System::Text::Encoding^ enc = gcnew System::Text::UTF8Encoding();
            array<System::Byte>^ utf8Array = arguments->PipeData;  //byte[] utf8Array = arguments.PipeData;
            String^ convertedText = enc->GetString(utf8Array);     //String^ convertedText = Encoding.UTF8.GetString(utf8Array);
            AddToLog(String::Format("RX: {0}", convertedText));
            */

            if (rData[0] != 'T' || rData[1] != '2') return (0);

            // ��M��
            int rlen = rData[2] + (UInt16)rData[3] * 256;

            // �p�����[�^������
            for (int i = 9; i < rlen; i++)
            {
                for (int j = 0; j < key->Length; j++)
                {
                    if (rData[i + j] != key[j]) goto lookup_loop;
                }

                // �j��������v
                if (key != "LEVEL=")
                {
                    if (key == "TIME=")
                    {
                        if (rData[i - 1] == 'D') return (0);
                    }

                    if (key == "DATA=")
                    {
                        if (rData[4] != 'R' || rData[5] != 'U' || rData[6] != 'S' || rData[7] != 'R' || rData[8] != 'D') return (0);
                    }

                    richTextBox1::AppendText(key +enc->GetString(rData, i + key->Length + 2, rData[i + key->Length] + (UInt16)rData[i + key->Length + 1]) + "\r\n");
                    //richTextBox1::AppendText(key + Encoding::ASCII::GetString(rData, i + key->Length + 2, rData[i + key->Length] + (UInt16)rData[i + key->Length + 1]) + "\r\n");
                }

                return (i);

lookup_loop: ;
            }

            return (0);
        }

        // �c�`�s�`��T��

    private:
        void data_find()
        {
            String^ key = "DATA=";

            for (int i = 0; i < 1024; i++) tData[i] = 0x00;

            if (rData[0] != 'T' || rData[1] != '2') return;

            // ��M��
            int rlen = rData[2] + (UInt16)rData[3] * 256;

            // �p�����[�^������
            for (int i = 4; i < rlen; i++)
            {
                for (int j = 0; j < key->Length; j++)
                {
                    if (rData[i + j] != key[j]) goto lookup_loop;
                }

                // �L�[��v
                int len = rData[i + 5] + (UInt16)rData[i + 6] * 256;
                for (int j = 0; j < len; j++) tData[j + 2] = rData[i + j + 7];

                // �擪�Q�o�C�g�Ƀo�C�g������������
                tData[0] = (Byte)len;
                tData[1] = (Byte)(len / 256);
                return;

lookup_loop: ;
            }
        }

        // �k�d�u�d�k��T��

    private:
        void level_find()
        {
            String^key = "LEVEL=";

            tData[0] = 0x00;
            tData[1] = 0x00;

            if (rData[0] != 'T' || rData[1] != '2') return;

            // ��M��
            int rlen = rData[2] + (UInt16)rData[3] * 256;

            // �p�����[�^������
            for (int i = 4; i < rlen; i++)
            {
                for (int j = 0; j < key->Length; j++)
                {
                    if (rData[i + j] != key[j]) goto lookup_loop;
                }

                // �L�[��v
                int len = rData[i + 6] + (UInt16)rData[i + 7] * 256;
                for (int j = 0; j < len; j++) tData[j + 2] = rData[i + j + 8];

                // �擪�Q�o�C�g�Ƀo�C�g������������
                tData[0] = (Byte)len;
                tData[1] = (Byte)(len / 256);
                return;

lookup_loop: ;
            }
        }

        // �r�s�`�s�t�r��T��

    private:
        int status_find()
        {
            String^key = "STATUS=";

            if (rData[0] != 'T' || rData[1] != '2') return (0);

            // ��M��
            int rlen = rData[2] + (UInt16)rData[3] * 256;

            // �p�����[�^������
            for (int i = 4; i < rlen; i++)
            {
                for (int j = 0; j < key->Length; j++)
                {
                    if (rData[i + j] != key[j]) goto lookup_loop;
                }

                // �r�s�`�s�t�r�L�[��v
                System::Text::Encoding^ enc = gcnew System::Text::ASCIIEncoding();
                String^status = enc->GetString(rData, i + 9, rData[i + 7] + (UInt16)rData[i + 8] * 256);
              //String^status = "Encoding.ASCII.GetString(rData, i + 9, rData[i + 7] + (UInt16)rData[i + 8] * 256)";

                richTextBox1::AppendText(key + status + "  �i");

                if (status->Substring(0, 4) == "0000") richTextBox1::AppendText("�S�́|");
                else if (status->Substring(0, 4) == "0001")
                {
                    richTextBox1::AppendText("�V�X�e���S�ʁ|");
                    if (status->Substring(5, 4) == "0000") richTextBox1::AppendText("�n�j");
                    else if (status->Substring(5, 4) == "0001") richTextBox1::AppendText("�r�q�`�l�G���[");
                    else if (status->Substring(5, 4) == "0002") richTextBox1::AppendText("�d�d�o�q�n�l�G���[");
                    else if (status->Substring(5, 4) == "0003") richTextBox1::AppendText("�o�b�e���G���[");
                    else if (status->Substring(5, 4) == "0004") richTextBox1::AppendText("�a�������G���[");
                }
                else if (status->Substring(0, 4) == "0002")
                {
                    richTextBox1::AppendText("�R�}���h�|");

                    if (status->Substring(5, 4) == "0000") richTextBox1::AppendText("�n�j");
                    else if (status->Substring(5, 4) == "0001") richTextBox1::AppendText("�R�}���h�t�H�[�}�b�g�G���[");
                    else if (status->Substring(5, 4) == "0002") richTextBox1::AppendText("�`�F�b�N�T���G���[");
                    else if (status->Substring(5, 4) == "0003") richTextBox1::AppendText("��莞�ԓ��ɃR�}���h���I�����Ȃ�����");
                    else if (status->Substring(5, 4) == "0004") richTextBox1::AppendText("�R�}���h���s��");
                    else if (status->Substring(5, 4) == "0005") richTextBox1::AppendText("���p�@�ݒ莞�ɐe�@�̃R�}���h���󂯂�");
                    else if (status->Substring(5, 4) == "0006") richTextBox1::AppendText("���̑��̃G���[");
                }
                else if (status->Substring(0, 4) == "0003") richTextBox1::AppendText("�f�r�l�|");
                else if (status->Substring(0, 4) == "0004")
                {
                    richTextBox1::AppendText("�q�e�|");

                    if (status->Substring(5, 4) == "0000") richTextBox1::AppendText("�n�j");
                    else if (status->Substring(5, 4) == "0001") richTextBox1::AppendText("�����ʐM��");
                    else if (status->Substring(5, 4) == "0002") richTextBox1::AppendText("�q�@�L�^�J�n�v���e�N�g");
                    else if (status->Substring(5, 4) == "0003") richTextBox1::AppendText("�����ʐM�r���L�����Z�����ꂽ");
                    else if (status->Substring(5, 4) == "0004") richTextBox1::AppendText("���p�@�ԒʐM�G���[");
                    else if (status->Substring(5, 4) == "0005") richTextBox1::AppendText("�q�@�ԒʐM�G���[");
                    else if (status->Substring(5, 4) == "0006") richTextBox1::AppendText("�^�C���A�E�g�G���[");
                    else if (status->Substring(5, 4) == "0007") richTextBox1::AppendText("�L�^�J�n�܂ł̕b��������Ȃ�����");
                    else if (status->Substring(5, 4) == "0008") richTextBox1::AppendText("���������s���Ȓl������");
                    else if (status->Substring(5, 4) == "0009") richTextBox1::AppendText("�f�[�^�����݂��Ȃ��ꍇ");
                    else if (status->Substring(5, 4) == "000a") richTextBox1::AppendText("�z���グ���I�����Ă��Ȃ�");
                    else richTextBox1::AppendText("���̑��̃G���[");
                }
                else if (status->Substring(0, 4) == "0005")
                {
                    richTextBox1::AppendText("�h�q�|");

                    if (status->Substring(5, 4) == "0000") richTextBox1::AppendText("�n�j");
                    else if (status->Substring(5, 4) == "0001") richTextBox1::AppendText("���ʐM�̉�������������");
                    else if (status->Substring(5, 4) == "0002") richTextBox1::AppendText("���������s���Ȓl������");
                    else if (status->Substring(5, 4) == "0003") richTextBox1::AppendText("�q�@����̉������s��������");
                }
                else if (status->Substring(0, 4) == "0006") richTextBox1::AppendText("�f�o�r�|");
                else if (status->Substring(0, 4) == "0007") richTextBox1::AppendText("�k�`�m�|");
                else if (status->Substring(0, 4) == "0008")
                {
                    richTextBox1::AppendText("�c�b�|");

                    if (status->Substring(5, 4) == "0000") richTextBox1::AppendText("�n�j");
                    else if (status->Substring(5, 4) == "0001") richTextBox1::AppendText("�����ł��Ă��Ȃ�");
                }
                else richTextBox1::AppendText("����`�|");

                richTextBox1::AppendText("�j\r\n");

                return (i);

lookup_loop: ;
            }

            return (0);
        }

        // �p�����[�^���o

    private:
        void extraction()
        {
            key_find("NAME=");
            key_find("DESC=");

            key_find("LANG=");
            key_find("UNITS=");

            key_find("FWV=");
            key_find("RFV=");
            key_find("RFS=");
            key_find("SER=");
            key_find("CODE=");
            key_find("COMP=");

            key_find("EXT=");
            key_find("BATT=");

            key_find("DTIME=");
            key_find("DIFF=");
            key_find("DST=");

            key_find("SIZE=");

            key_find("TIME=");
            key_find("LAST=");

            key_find("ID=");
            key_find("CH=");
            key_find("NUM=");

            key_find("DATA=");

            status_find();
            data_find();
        }

    private: int command_make_sub(array<String^>^ command, array<uint8_t>^ completion)
             {
                 array<uint8_t>^ material = gcnew array<uint8_t>(512 + 128);

                 int c = 0, e = 0, n = 0;


                 for (int i = 0; i < command->Length; i++)
                 {
                     n = 0;

                     for (int j = 0; j < command[i]->Length; j++) material[j] = (Byte)command[i][j];

                     for (int j = 0; j < command[i]->Length; j++)
                     {
                         if (
                             (n == 0) && 
                             (material[j] == ((uint8_t)'='))
                             )
                         {
                             // �h���h�ʒu���b�`
                             e = c;
                             n = j;

                             completion[c++] = (Byte)'=';
                             completion[c++] = 0x00;
                             completion[c++] = 0x00;
                         }
                         else completion[c++] = material[j];
                     }

                     // �h���h�����������Ƃ�����
                     if (n != 0)
                     {
                         completion[e + 1] = (Byte)(command[i]->Length - n - 1);
                         completion[e + 2] = (Byte)((command[i]->Length - n - 1) >> 8);
                     }
                 }

                 return (c);
             }

             // �s�R�}���h�쐬

    private:
        void command_make(array<String^>^ command)
        {
            int clen = 0;
            array<uint8_t>^ cd = gcnew array<uint8_t>(1024); //  byte[] cd = new Byte[1024];

#if 0
            //sData[0] = 0x00;

            sData[0] = (Byte)'T';
            sData[1] = (Byte)'2';

            clen = command_make_sub(command, cd);

            sData[2] = (Byte)clen;
            sData[3] = (Byte)(clen / 256);

            Buffer::BlockCopy(cd, 0, sData, 4, clen);
#else
            sData[0] = 0x00;

            sData[1] = (Byte)'T';
            sData[2] = (Byte)'2';

            clen = command_make_sub(command, cd);

            sData[3] = (Byte)clen;
            sData[4] = (Byte)(clen / 256);

            Buffer::BlockCopy(cd, 0, sData, 5, clen);
#endif
        }
        
    public:        
        array<uint8_t,1>^ RUINF_get_send_packet() // 'T'
        {
            array<uint8_t,1>^ pkt;

            // �R�}���h���o
            array<String^>^ command = { "RUINF:" };

            command_make(command);
            
            //sendCommand();
            //void sendCommand()
            //{
            UInt16 i, len, sum, t;

            /*
            serialPort1.DiscardInBuffer();
            serialPort1.DiscardOutBuffer();
            */
            rData[0] = 0x00;

#if 1
            //TODO
#else
            // �s�R�}���h�Ȃ�擪�o�C�g���폜����
            if (sData[1] == (Byte)'T') t = 1;
            else t = 0;

            len = (UInt16)(sData[3] + (UInt16)sData[4] * 256);

            for (sum = 0, i = t; i < len + 5; i++)
            {
                if (t == 1)
                {
                    if (i >= 5) sum += sData[i];
                }
                else sum += sData[i];
            }

            sData[len + 5] = (Byte)sum;
            sData[len + 6] = (Byte)(sum / 256);

            pkt = gcnew array<uint8_t,1>(len + 7);
            Buffer::BlockCopy(sData, 0, pkt, 0, len + 7);
#endif
            /*
            serialPort1.Write(sData, t, len + 7 - t);

            Application.DoEvents();
            */
            //}

            //pkt = sData;
            return( pkt );
        }

    public:        
        void RUINF_process_recv_packet(array<uint8_t,1>^ pkt) // 'T'
        {
            int i;

            for(i=0; i<pkt->Length; i++)
                rData[i] = pkt[i];

            extraction();   // �p�����[�^���o

        }

    public:        
        void RUINF_send_recv()
        {
            /*
            richTextBox1.Clear();

            if (serialPortOpen() == false)
            {
            richTextBox1::AppendText("�|�[�g�I�[�v�����s\r\n");
            goto error_proc;
            }
            */

            // �u���[�N�M�����o
            sData[0] = 0x00;
            //serialPort1.Write(sData, 0, 1);
            //delay(50);

            // �R�}���h���o
            array<String^>^ command = { "RUINF:" };

            command_make(command);

            sendCommand();

            t_recvCommand();

            extraction();   // �p�����[�^���o

            /*
error_proc: ;
            richTextBox1::AppendText("�I��\r\n");
            serialPort1.Close();

            // ����֎~����
            operation_lift_ban();
            */
        }


    };

}
