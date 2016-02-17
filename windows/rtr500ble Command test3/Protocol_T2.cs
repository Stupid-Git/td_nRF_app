
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Drawing;


using System.Threading;

using TDnRFLib; //TDnRFcontroller_LIB;


namespace rtr500ble_Command_test //OLD namespace TDnRFLib_Test1
{
    class Protocol_T2
    {

        public event EventHandler<OutputReceivedEventArgs> userEv_LogMessage;

        /*
        static class richTextBox1
        {
            public static void AppendText(String s)
            {        
                //if( userEv_LogMessage != null )
                //    userEv_LogMessage(this, new 
                Console.WriteLine(s);
            }
        };
        */

        // 送受信バッファ
        public Byte[] sData = new Byte[1024 * 10 + 128];
        public Byte[] rData = new Byte[1024 * 10 + 128];
        public Byte[] tData = new Byte[1024 * 10];

        public Protocol_T2()
        {
        }

        //public event EventHandler<TDnRFLib.OutputReceivedEventArgs> userEv_LogMessage;

        public void richTextBox1_AppendText(String s)
        {
            if (userEv_LogMessage != null)
                userEv_LogMessage(this, new OutputReceivedEventArgs(s));
            //Console.WriteLine(s);
        }
    
        private void UnUsed_sendCommand()
        {
            UInt16 i, len, sum, t;

            /*
            serialPort1.DiscardInBuffer();
            serialPort1.DiscardOutBuffer();
            */

            rData[0] = 0x00;

            // Ｔコマンドなら先頭バイトを削除する
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

            sData[len + 5] = (byte)sum;
            sData[len + 6] = (byte)(sum / 256);

            /*
            serialPort1.Write(sData, t, len + 7 - t);

            Application.DoEvents();
            */
        }



        // １バイトをｓｔｒｉｎｇへ変換

        String hex(Byte h)
        {
            char[] dummyChars = { Convert.ToChar(h) };
            return (new String(dummyChars));
        }



        // Ｔコマンド受信

        Boolean UnUsed_t_recvCommand()
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
            while ((j = (UInt16)serialPort1.BytesToRead) < (len - 4))         // 予定バイト数が受信されてからＲｅａｄすると早い
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
            // チェックサム計算
            if ((i < (len - 2)) && (i > 3)) sum += rData[i];
            }

            serialPort1.ReadTimeout = rt;
            serialPort1.WriteTimeout = wt;

            if ((sum & 0xffff) != (rData[len - 2] + (int)rData[len - 1] * 256)) return false;
            */
            return true;
        }



        // パラメータ名を探す

        int key_find(String key)
        {

            System.Text.Encoding enc = new System.Text.ASCIIEncoding();

            /*
            System.Text.Encoding^ enc = new System.Text.UTF8Encoding();
            array<System.Byte>^ utf8Array = arguments.PipeData;  //byte[] utf8Array = arguments.PipeData;
            String convertedText = enc->GetString(utf8Array);     //String convertedText = Encoding.UTF8.GetString(utf8Array);
            AddToLog(String.Format("RX: {0}", convertedText));
            */

            if (rData[0] != 'T' || rData[1] != '2') return (0);

            // 受信数
            int rlen = rData[2] + (UInt16)rData[3] * 256;

            // パラメータを検索
            for (int i = 9; i < rlen; i++)
            {
                for (int j = 0; j < key.Length; j++)
                {
                    if (rData[i + j] != key[j]) goto lookup_loop;
                }

                // Ｋｅｙが一致
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

                    richTextBox1_AppendText(key + enc.GetString(rData, i + key.Length + 2, rData[i + key.Length] + (UInt16)rData[i + key.Length + 1]) + "\r\n");
                    //richTextBox1.AppendText(key + Encoding.ASCII.GetString(rData, i + key.Length + 2, rData[i + key.Length] + (UInt16)rData[i + key.Length + 1]) + "\r\n");
                }

                return (i);

lookup_loop: ;
            }

            return (0);
        }

        // ＤＡＴＡを探す

    
        void data_find()
        {
            String key = "DATA=";

            for (int i = 0; i < 1024; i++) tData[i] = 0x00;

            if (rData[0] != 'T' || rData[1] != '2') return;

            // 受信数
            int rlen = rData[2] + (UInt16)rData[3] * 256;

            // パラメータを検索
            for (int i = 4; i < rlen; i++)
            {
                for (int j = 0; j < key.Length; j++)
                {
                    if (rData[i + j] != key[j]) goto lookup_loop;
                }

                // キー一致
                int len = rData[i + 5] + (UInt16)rData[i + 6] * 256;
                for (int j = 0; j < len; j++) tData[j + 2] = rData[i + j + 7];

                // 先頭２バイトにバイト数を書き込み
                tData[0] = (Byte)len;
                tData[1] = (Byte)(len / 256);
                return;

lookup_loop: ;
            }
        }

        // ＬＥＶＥＬを探す

    
        void level_find()
        {
            String key = "LEVEL=";

            tData[0] = 0x00;
            tData[1] = 0x00;

            if (rData[0] != 'T' || rData[1] != '2') return;

            // 受信数
            int rlen = rData[2] + (UInt16)rData[3] * 256;

            // パラメータを検索
            for (int i = 4; i < rlen; i++)
            {
                for (int j = 0; j < key.Length; j++)
                {
                    if (rData[i + j] != key[j]) goto lookup_loop;
                }

                // キー一致
                int len = rData[i + 6] + (UInt16)rData[i + 7] * 256;
                for (int j = 0; j < len; j++) tData[j + 2] = rData[i + j + 8];

                // 先頭２バイトにバイト数を書き込み
                tData[0] = (Byte)len;
                tData[1] = (Byte)(len / 256);
                return;

lookup_loop: ;
            }
        }

        // ＳＴＡＴＵＳを探す

    
        int status_find()
        {
            String key = "STATUS=";

            if (rData[0] != 'T' || rData[1] != '2') return (0);

            // 受信数
            int rlen = rData[2] + (UInt16)rData[3] * 256;

            // パラメータを検索
            for (int i = 4; i < rlen; i++)
            {
                for (int j = 0; j < key.Length; j++)
                {
                    if (rData[i + j] != key[j]) goto lookup_loop;
                }

                // ＳＴＡＴＵＳキー一致
                System.Text.Encoding enc = new System.Text.ASCIIEncoding();
                String status = enc.GetString(rData, i + 9, rData[i + 7] + (UInt16)rData[i + 8] * 256);
              //String status = "Encoding.ASCII.GetString(rData, i + 9, rData[i + 7] + (UInt16)rData[i + 8] * 256)";

                richTextBox1_AppendText(key + status + "  （");

                if (status.Substring(0, 4) == "0000") richTextBox1_AppendText("全体－");
                else if (status.Substring(0, 4) == "0001")
                {
                    richTextBox1_AppendText("システム全般－");
                    if (status.Substring(5, 4) == "0000") richTextBox1_AppendText("ＯＫ");
                    else if (status.Substring(5, 4) == "0001") richTextBox1_AppendText("ＳＲＡＭエラー");
                    else if (status.Substring(5, 4) == "0002") richTextBox1_AppendText("ＥＥＰＲＯＭエラー");
                    else if (status.Substring(5, 4) == "0003") richTextBox1_AppendText("バッテリエラー");
                    else if (status.Substring(5, 4) == "0004") richTextBox1_AppendText("Ｂｕｓｙエラー");
                }
                else if (status.Substring(0, 4) == "0002")
                {
                    richTextBox1_AppendText("コマンド－");

                    if (status.Substring(5, 4) == "0000") richTextBox1_AppendText("ＯＫ");
                    else if (status.Substring(5, 4) == "0001") richTextBox1_AppendText("コマンドフォーマットエラー");
                    else if (status.Substring(5, 4) == "0002") richTextBox1_AppendText("チェックサムエラー");
                    else if (status.Substring(5, 4) == "0003") richTextBox1_AppendText("一定時間内にコマンドが終了しなかった");
                    else if (status.Substring(5, 4) == "0004") richTextBox1_AppendText("コマンド実行中");
                    else if (status.Substring(5, 4) == "0005") richTextBox1_AppendText("中継機設定時に親機のコマンドを受けた");
                    else if (status.Substring(5, 4) == "0006") richTextBox1_AppendText("その他のエラー");
                }
                else if (status.Substring(0, 4) == "0003") richTextBox1_AppendText("ＧＳＭ－");
                else if (status.Substring(0, 4) == "0004")
                {
                    richTextBox1_AppendText("ＲＦ－");

                    if (status.Substring(5, 4) == "0000") richTextBox1_AppendText("ＯＫ");
                    else if (status.Substring(5, 4) == "0001") richTextBox1_AppendText("無線通信中");
                    else if (status.Substring(5, 4) == "0002") richTextBox1_AppendText("子機記録開始プロテクト");
                    else if (status.Substring(5, 4) == "0003") richTextBox1_AppendText("無線通信途中キャンセルされた");
                    else if (status.Substring(5, 4) == "0004") richTextBox1_AppendText("中継機間通信エラー");
                    else if (status.Substring(5, 4) == "0005") richTextBox1_AppendText("子機間通信エラー");
                    else if (status.Substring(5, 4) == "0006") richTextBox1_AppendText("タイムアウトエラー");
                    else if (status.Substring(5, 4) == "0007") richTextBox1_AppendText("記録開始までの秒数が足りなかった");
                    else if (status.Substring(5, 4) == "0008") richTextBox1_AppendText("引き数が不正な値だった");
                    else if (status.Substring(5, 4) == "0009") richTextBox1_AppendText("データが存在しない場合");
                    else if (status.Substring(5, 4) == "000a") richTextBox1_AppendText("吸い上げが終了していない");
                    else richTextBox1_AppendText("その他のエラー");
                }
                else if (status.Substring(0, 4) == "0005")
                {
                    richTextBox1_AppendText("ＩＲ－");

                    if (status.Substring(5, 4) == "0000") richTextBox1_AppendText("ＯＫ");
                    else if (status.Substring(5, 4) == "0001") richTextBox1_AppendText("光通信の応答が無かった");
                    else if (status.Substring(5, 4) == "0002") richTextBox1_AppendText("引き数が不正な値だった");
                    else if (status.Substring(5, 4) == "0003") richTextBox1_AppendText("子機からの応答が不正だった");
                }
                else if (status.Substring(0, 4) == "0006") richTextBox1_AppendText("ＧＰＳ－");
                else if (status.Substring(0, 4) == "0007") richTextBox1_AppendText("ＬＡＮ－");
                else if (status.Substring(0, 4) == "0008")
                {
                    richTextBox1_AppendText("ＤＣ－");

                    if (status.Substring(5, 4) == "0000") richTextBox1_AppendText("ＯＫ");
                    else if (status.Substring(5, 4) == "0001") richTextBox1_AppendText("準備できていない");
                }
                else richTextBox1_AppendText("未定義－");

                richTextBox1_AppendText("）\r\n");

                return (i);

lookup_loop: ;
            }

            return (0);
        }

        // パラメータ抽出

    
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


        private int command_make_sub(string[] command, byte[] completion)
        {
            byte[] material = new Byte[512 + 128];

            int c = 0, e = 0, n = 0;


            for (int i = 0; i < command.Length; i++)
            {
                n = 0;

                for (int j = 0; j < command[i].Length; j++) material[j] = (Byte)command[i][j];

                for (int j = 0; j < command[i].Length; j++)
                {
                    if (n == 0 && material[j] == '=')
                    {
                        // ”＝”位置ラッチ
                        e = c;
                        n = j;

                        completion[c++] = (Byte)'=';
                        completion[c++] = 0x00;
                        completion[c++] = 0x00;
                    }
                    else completion[c++] = material[j];
                }

                // ”＝”が見つかったときだけ
                if (n != 0)
                {
                    completion[e + 1] = (Byte)(command[i].Length - n - 1);
                    completion[e + 2] = (Byte)((command[i].Length - n - 1) >> 8);
                }
            }

            return (c);
        }

        
        // Ｔコマンド作成
    

        private void command_make(string[] command)
        {
            int clen = 0;
            byte[] cd = new Byte[1024];


#if true// false
            //sData[0] = 0x00;

            sData[0] = (Byte)'T';
            sData[1] = (Byte)'2';

            clen = command_make_sub(command, cd);

            sData[2] = (Byte)clen;
            sData[3] = (Byte)(clen / 256);

            Buffer.BlockCopy(cd, 0, sData, 4, clen);
#else
            sData[0] = 0x00;

            sData[1] = (Byte)'T';
            sData[2] = (Byte)'2';

            clen = command_make_sub(command, cd);

            sData[3] = (Byte)clen;
            sData[4] = (Byte)(clen / 256);
            Buffer.BlockCopy(cd, 0, sData, 5, clen);
#endif
        }
        
        public  byte [] RUINF_get_send_packet() // 'T'
        {
            byte [] pkt;
            UInt16 i, len, sum;

            // コマンド送出
            String [] command = { "RUINF:" };

            command_make(command);
            

            rData[0] = 0x00;

            len = (UInt16)(sData[2] + (UInt16)sData[3] * 256);
            for (sum = 0, i = 4; i < len + 4; i++)
            {
                sum += sData[i];
            }
            sData[len + 4] = (Byte)sum;
            sData[len + 5] = (Byte)(sum / 256);

            pkt = new Byte[len + 6];
            Buffer.BlockCopy(sData, 0, pkt, 0, len + 6);

            UInt16 T2len = (UInt16)(len + 6);
            T2_print("RUINF_get_send_packet TX packet", pkt, T2len);

            return( pkt );
        }
        
        public  void RUINF_process_recv_packet( Byte [] pkt, UInt16 T2len) // 'T'
        {
            int i;

            //for (i = 0; i < pkt.Length; i++)
            for (i = 0; i < T2len; i++)
                rData[i] = pkt[i];


            T2_print("RUINF_process_recv_packet RX packet", pkt, T2len);

            extraction();   // パラメータ抽出

        }

        void T2_print(String s, Byte[] P,  UInt16 T2len)
        {
            UInt32     i;
            String S1 = "";
            String S2 = "";
            char C;
            UInt32     wpl; // White Space Length

            Console.Write("{0}\n", s);

            wpl = (16 * 3) + 8;
            for (i = 0; i < T2len; i++)                
            {
                S1 += String.Format("{0:X2} ", P[i]); //Console.Write("{0:X2} ", P[i]);
                wpl -= 3;

                C = (char)P[i];
                if( Char.IsLetterOrDigit(C) )
                    S2 += String.Format("{0}", C);
                else
                    S2 += ".";

                if ( ((i % 16) == 15) || ((i+1) == T2len) )
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

        public void UnUsed_RUINF_send_recv()
        {
            /*
            richTextBox1.Clear();

            if (serialPortOpen() == false)
            {
            richTextBox1_AppendText("ポートオープン失敗\r\n");
            goto error_proc;
            }
            */

            // ブレーク信号送出
            sData[0] = 0x00;
            //serialPort1.Write(sData, 0, 1);
            //delay(50);

            // コマンド送出
            String []  command = { "RUINF:" };

            command_make(command);

            UnUsed_sendCommand();

            UnUsed_t_recvCommand();

            extraction();   // パラメータ抽出

            /*
error_proc: ;
            richTextBox1_AppendText("終了\r\n");
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
            */
        }


    }
}
    

