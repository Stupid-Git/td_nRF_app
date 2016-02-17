using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Xml;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO.Ports;
using Microsoft.VisualBasic;
using System.Threading;
using System.Management;


namespace nRFUart_TDForms
{
    public partial class AppWindow_1 : Form
    {


        // see below public int serial_port = 0;
        public int cancel = 1;
        public int time_out = 0;

        static public UInt64 Unique_ID;
        static public Byte Unique_Freq;
        static public Byte Unique_Numb;
        static public Byte parameter_max;

        static public int route;

        static public int Relay0;
        static public int Relay1;
        static public int Relay2;
        static public int Relay3;

        // データ吸い上げ設定用
        static public int mode = 2;
        static public int range = 0;
        static public Boolean advance = false;

        // 送受信バッファ
        public Byte[] sData = new Byte[1024 * 10 + 128];
        public Byte[] rData = new Byte[1024 * 10 + 128];
        public Byte[] tData = new Byte[1024 * 10];

        byte[] BinaryData1 = new byte[0x00080000];  // フラッシュバンクＡ用バッファ
        byte[] BinaryData2 = new byte[0x00080000];  // フラッシュバンクＢ用バッファ
        byte[] BinaryData3 = new byte[0x00004000];  // ブートローダー用バッファ

        public uint DataLength1 = 0;
        public uint DataLength2 = 0;
        public uint DataLength3 = 0;

        public UInt32 crc32;

        public UInt32 START_ADDRESS = 0x1a004000;
        public UInt32 BANK_A_ADDRESS = 0x1a000000;
        public UInt32 BANK_B_ADDRESS = 0x1b000000;
        public UInt32 AUTH_CODE_ADDRESS = 0x00000300;

        public UInt32 SERIAL_FLASH_UC_START = 0x00000000;
        public UInt32 SERIAL_FLASH_UC_END = 0x000FFFFF;

        Byte[] password_user = new Byte[8] { (Byte)'C', (Byte)'r', (Byte)'p', (Byte)'t', (Byte)'0', (Byte)'8', (Byte)'0', (Byte)'1' };   // ユーザーアプリケーション　ＵＳＢコマンドのパスワード（８バイト）

        // 高精度ｄｅｌａｙ用変数
        static public long freq;
        static public long start_count;
        static public long stop_count;

        // ［閉じる］ボタンを無効化するための値
        private const UInt32 SC_CLOSE = 0x0000F060;
        private const UInt32 MF_BYCOMMAND = 0x00000000;

        private const UInt32 MF_ENABLED = 0x00000000;
        private const UInt32 MF_GRAYED = 0x00000001;
        private const UInt32 MF_DISABLED = 0x00000002;

        // 通信コマンド
        public UInt32 CMD_USB_TEST = 0xe0;									// ＵＳＢ通信試験

        public UInt32 CMD_nRF_MODE = 0xe6;									// ｎＲＦ５１ブートモード設定
        public UInt32 CMD_nRF_SLIPWRAP = 0xe7;								// ｎＲＦ５１ＳＬＩＰブロックラップ

        public UInt32 CMD_CFM_ELASE = 0xe8;									// ＣＰＵフラッシュメモリ消去
        public UInt32 CMD_CFM_PROGRAM = 0xe9;								// ＣＰＵフラッシュメモリプログラム
        public UInt32 CMD_CFM_COMPARE = 0xea;								// ＣＰＵフラッシュメモリ比較

        public UInt32 CMD_SFM_ELASE = 0xeb;									// シリアルフラッシュメモリ消去
        public UInt32 CMD_SFM_WRITE = 0xec;									// シリアルフラッシュメモリプログラム
        public UInt32 CMD_SFM_READ = 0xed;									// シリアルフラッシュメモリ比較

        public UInt32 CMD_CPU_RESET = 0xee;									// ＣＰＵリセット
        public UInt32 CMD_ID = 0xef;										// パーツＩＤとシリアル番号を送信

        // ＤＬＬインポート

        [DllImport("kernel32.dll")]
        extern static short QueryPerformanceCounter(ref long x);
        [DllImport("kernel32.dll")]
        extern static short QueryPerformanceFrequency(ref long x);
        [DllImport("USER32.DLL")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, Boolean bRevert);
        [DllImport("USER32.DLL")]
        private static extern UInt32 RemoveMenu(IntPtr hMenu, UInt32 nPosition, UInt32 wFlags);
        [DllImport("USER32.DLL")]
        private static extern UInt32 DrawMenuBar(IntPtr hMenu);
        [DllImport("USER32.DLL")]
        private static extern UInt32 EnableMenuItem(IntPtr hMenu, UInt32 nPosition, UInt32 wFlags);

        // メッセージボックス表示位置を制御するためのＤＬＬインポート

        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(IntPtr classname, string title);
        [DllImport("user32.dll")]
        static extern void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool rePaint);
        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hwnd, out Rectangle rect);

        // メッセージボックス表示位置を移動するスレッド

        void FindAndMoveMsgBox(int x, int y, bool repaint, string title)
        {
            Thread thr = new Thread(() => // create a new thread
            {
                IntPtr msgBox = IntPtr.Zero;

                // while there's no MessageBox, FindWindow returns IntPtr.Zero
                while ((msgBox = FindWindow(IntPtr.Zero, title)) == IntPtr.Zero) ;

                // after the while loop, msgBox is the handle of your MessageBox
                Rectangle r = new Rectangle();

                // Gets the rectangle of the message box
                GetWindowRect(msgBox, out r);

                MoveWindow(msgBox, x, y, r.Width - r.X, r.Height - r.Y, repaint);
            });

            // starts the thread
            thr.Start();
        }

        // メッセージボックス表示

        private DialogResult MessageBoxShow(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            DateTime now = DateTime.Now;


            caption = caption + now.ToString(" (yyyy/mm/dd HH:mm:ss)"); // メッセージボックス識別用

            FindAndMoveMsgBox((this.Left + this.Right) / 2, (this.Top + this.Bottom) / 2, true, caption);
            return (MessageBox.Show(text, caption, buttons, icon));
        }

        public AppWindow_1()
        {
            InitializeComponent();

            // カウンタ周波数の取得
            QueryPerformanceFrequency(ref freq);

            toolStripStatusLabel1.Text = "";
        }


        private void btnTest_Click(object sender, EventArgs e)
        {
            rUINF機器情報の取得ToolStripMenuItem_Click(sender, e);
        }





        //=====================================================================
        //=====================================================================
        //=====================================================================
        //=====================================================================

        
        int serial_port = 2;
        SerialPort serialPort1 = new SerialPort();


        private void delay(UInt16 t)
        {
            QueryPerformanceCounter(ref start_count);

            while (true)
            {
                QueryPerformanceCounter(ref stop_count);

                if (((stop_count - start_count) * 1000.0 / freq) > (Double)(t * 1.0)) break;
            }
        }

        private void delay_us(UInt16 t)
        {
            QueryPerformanceCounter(ref start_count);

            while (true)
            {
                QueryPerformanceCounter(ref stop_count);

                if (((stop_count - start_count) * 1000000.0 / freq) > (Double)(t * 1.0)) break;
            }
        }

        // 操作禁止

        private void operation_ban()
        {
            /*
            IntPtr hMenu = GetSystemMenu(this.Handle, false);

            // コントロールボックスの［閉じる］ボタンの無効化
            EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
            DrawMenuBar(hMenu);

            menuStrip1.Enabled = false;
            */
            Application.DoEvents();
        }

        // 操作禁止解除

        private void operation_lift_ban()
        {
            /*
            IntPtr hMenu = GetSystemMenu(this.Handle, false);

            // コントロールボックスの［閉じる］ボタンの有効化
            EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_ENABLED);
            DrawMenuBar(hMenu);

            menuStrip1.Enabled = true;
            */
        }

        // シリアル通信オープン

        private Boolean serialPortOpen()
        {
            int sPort = 0;

            // オープン

            if (serial_port == 0)
            {
                sPort = 1;
                serialPort1.BaudRate = 115200;

                serialPort1.ReadTimeout = 3000;
                serialPort1.WriteTimeout = 3000;
            }
            else
            if (serial_port == 1)
            {
                //sPort = serialPortSearch();
                serialPort1.BaudRate = 500000;

                serialPort1.ReadTimeout = 500;
                serialPort1.WriteTimeout = 500;
            }
            else
            if (serial_port == 2)
            {
                sPort = 23; // COM22, COM23  COM62, COM63 serialPortSearch();
                serialPort1.BaudRate = 115200;

                serialPort1.ReadTimeout = 500;
                serialPort1.WriteTimeout = 500;
            }
            
            
            
            serialPort1.RtsEnable = true;
            serialPort1.DtrEnable = true;

            serialPort1.PortName = String.Format("COM{0:d}", sPort);

            serialPort1.ReadBufferSize = 1024 * 10;
            serialPort1.WriteBufferSize = 1024 * 10;

            toolStripStatusLabel1.Text = String.Format("COM{0:d}", sPort);

            try
            {
                serialPort1.Open();
            }
            catch (Exception ex)
            {
                MessageBoxShow("通信ポート　オープンエラー", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }


        private void sendCommand()
        {
            UInt16 i, len, sum, t;


            serialPort1.DiscardInBuffer();
            serialPort1.DiscardOutBuffer();

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

            serialPort1.Write(sData, t, len + 7 - t);

            Application.DoEvents();
        }

        // 受信

        private Boolean recvCommand()
        {
            int i, n;

            UInt16 len, sum;


            for (i = 0; i < 5; i++)
            {
                try
                {
                    rData[i] = (byte)serialPort1.ReadByte();
                }
                catch (TimeoutException)
                {
                    return false;
                }

                Application.DoEvents();
            }

            if (rData[0] != 0x01)
            {
                return false;
            }

            len = (UInt16)(BitConverter.ToUInt16(rData, 3) + 7);

            if (len > 4096) return false;

            i = 0;

            time_out = serialPort1.ReadTimeout;

            try
            {
                while (serialPort1.BytesToRead < (len - 5))         // 予定バイト数が受信されてからＲｅａｄすると早い
                {
                    if (time_out == 0) return false;
                    Application.DoEvents();
                }

                while (i < len - 5)
                {
                    n = serialPort1.Read(rData, 5 + i, len - 5 - i);
                    if (n > 0) i += (UInt16)n;

                    Application.DoEvents();
                }
            }
            catch (TimeoutException)
            {
                return false;
            }

            sum = 0;

            for (i = 0; i < len - 2; i++) sum += rData[i];

            if (sum != BitConverter.ToUInt16(rData, len - 2)) return false;

            return true;
        }

        // １バイトをｓｔｒｉｎｇへ変換

        private string hex(Byte h)
        {
            char[] dummyChars = { Convert.ToChar(h) };
            return (new String(dummyChars));
        }

        // Ｔコマンド作成

        private void command_make(string[] command)
        {
            int clen = 0;
            byte[] cd = new Byte[1024];


            sData[0] = 0x00;

            sData[1] = (Byte)'T';
            sData[2] = (Byte)'2';

            clen = command_make_sub(command, cd);

            sData[3] = (Byte)clen;
            sData[4] = (Byte)(clen / 256);

            Buffer.BlockCopy(cd, 0, sData, 5, clen);
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

        // Ｔコマンド受信

        private Boolean t_recvCommand()
        {
            int n;

            UInt16 i, j, len, sum;

            int rt, wt;



            rt = serialPort1.ReadTimeout;
            wt = serialPort1.WriteTimeout;

            // 光ポート系のコマンドは時間がかかる場合がある

            if (serial_port == 0)
            {
                serialPort1.ReadTimeout = 6500;
                serialPort1.WriteTimeout = 6500;
            }
            else
            {
                serialPort1.ReadTimeout = 6500;
                serialPort1.WriteTimeout = 6500;
            }

            //karel
            while (serialPort1.BytesToRead < 4)//karel
            {//karel
                Application.DoEvents();//karel
            }//karel

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

            if (rData[0] != 'T')
            {
                serialPort1.ReadTimeout = rt;
                serialPort1.WriteTimeout = wt;
                return false;
            }

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

            return true;
        }

        // パラメータ抽出

        private void extraction()
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

        // パラメータ名を探す

        private int key_find(string key)
        {
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

                    richTextBox1.AppendText(key + Encoding.ASCII.GetString(rData, i + key.Length + 2, rData[i + key.Length] + (UInt16)rData[i + key.Length + 1]) + "\r\n");
                }

                return (i);

            lookup_loop: ;
            }

            return (0);
        }

        // ＤＡＴＡを探す

        private void data_find()
        {
            string key = "DATA=";

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

        private void level_find()
        {
            string key = "LEVEL=";

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

        private int status_find()
        {
            string key = "STATUS=";

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
                string status = Encoding.ASCII.GetString(rData, i + 9, rData[i + 7] + (UInt16)rData[i + 8] * 256);

                richTextBox1.AppendText(key + status + "  （");

                if (status.Substring(0, 4) == "0000") richTextBox1.AppendText("全体－");
                else if (status.Substring(0, 4) == "0001")
                {
                    richTextBox1.AppendText("システム全般－");
                    if (status.Substring(5, 4) == "0000") richTextBox1.AppendText("ＯＫ");
                    else if (status.Substring(5, 4) == "0001") richTextBox1.AppendText("ＳＲＡＭエラー");
                    else if (status.Substring(5, 4) == "0002") richTextBox1.AppendText("ＥＥＰＲＯＭエラー");
                    else if (status.Substring(5, 4) == "0003") richTextBox1.AppendText("バッテリエラー");
                    else if (status.Substring(5, 4) == "0004") richTextBox1.AppendText("Ｂｕｓｙエラー");
                }
                else if (status.Substring(0, 4) == "0002")
                {
                    richTextBox1.AppendText("コマンド－");

                    if (status.Substring(5, 4) == "0000") richTextBox1.AppendText("ＯＫ");
                    else if (status.Substring(5, 4) == "0001") richTextBox1.AppendText("コマンドフォーマットエラー");
                    else if (status.Substring(5, 4) == "0002") richTextBox1.AppendText("チェックサムエラー");
                    else if (status.Substring(5, 4) == "0003") richTextBox1.AppendText("一定時間内にコマンドが終了しなかった");
                    else if (status.Substring(5, 4) == "0004") richTextBox1.AppendText("コマンド実行中");
                    else if (status.Substring(5, 4) == "0005") richTextBox1.AppendText("中継機設定時に親機のコマンドを受けた");
                    else if (status.Substring(5, 4) == "0006") richTextBox1.AppendText("その他のエラー");
                }
                else if (status.Substring(0, 4) == "0003") richTextBox1.AppendText("ＧＳＭ－");
                else if (status.Substring(0, 4) == "0004")
                {
                    richTextBox1.AppendText("ＲＦ－");

                    if (status.Substring(5, 4) == "0000") richTextBox1.AppendText("ＯＫ");
                    else if (status.Substring(5, 4) == "0001") richTextBox1.AppendText("無線通信中");
                    else if (status.Substring(5, 4) == "0002") richTextBox1.AppendText("子機記録開始プロテクト");
                    else if (status.Substring(5, 4) == "0003") richTextBox1.AppendText("無線通信途中キャンセルされた");
                    else if (status.Substring(5, 4) == "0004") richTextBox1.AppendText("中継機間通信エラー");
                    else if (status.Substring(5, 4) == "0005") richTextBox1.AppendText("子機間通信エラー");
                    else if (status.Substring(5, 4) == "0006") richTextBox1.AppendText("タイムアウトエラー");
                    else if (status.Substring(5, 4) == "0007") richTextBox1.AppendText("記録開始までの秒数が足りなかった");
                    else if (status.Substring(5, 4) == "0008") richTextBox1.AppendText("引き数が不正な値だった");
                    else if (status.Substring(5, 4) == "0009") richTextBox1.AppendText("データが存在しない場合");
                    else if (status.Substring(5, 4) == "000a") richTextBox1.AppendText("吸い上げが終了していない");
                    else richTextBox1.AppendText("その他のエラー");
                }
                else if (status.Substring(0, 4) == "0005")
                {
                    richTextBox1.AppendText("ＩＲ－");

                    if (status.Substring(5, 4) == "0000") richTextBox1.AppendText("ＯＫ");
                    else if (status.Substring(5, 4) == "0001") richTextBox1.AppendText("光通信の応答が無かった");
                    else if (status.Substring(5, 4) == "0002") richTextBox1.AppendText("引き数が不正な値だった");
                    else if (status.Substring(5, 4) == "0003") richTextBox1.AppendText("子機からの応答が不正だった");
                }
                else if (status.Substring(0, 4) == "0006") richTextBox1.AppendText("ＧＰＳ－");
                else if (status.Substring(0, 4) == "0007") richTextBox1.AppendText("ＬＡＮ－");
                else if (status.Substring(0, 4) == "0008")
                {
                    richTextBox1.AppendText("ＤＣ－");

                    if (status.Substring(5, 4) == "0000") richTextBox1.AppendText("ＯＫ");
                    else if (status.Substring(5, 4) == "0001") richTextBox1.AppendText("準備できていない");
                }
                else richTextBox1.AppendText("未定義－");

                richTextBox1.AppendText("）\r\n");

                return (i);

            lookup_loop: ;
            }

            return (0);
        }



        private void rUINF機器情報の取得ToolStripMenuItem_Click(object sender, EventArgs e)
        {
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

            // コマンド送出
            string[] command = { "RUINF:" };

            /*TODO*/
            command_make(command);

            sendCommand();
            if (t_recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            extraction();   // パラメータ抽出
            /*TODO*/

        error_proc: ;
            richTextBox1.AppendText("終了\r\n");
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
        }






    }
}
