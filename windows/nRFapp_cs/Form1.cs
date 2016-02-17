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

namespace rtr500ble_Command_test
{
    public partial class Form1 : Form
    {
        public int serial_port = 0;
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

        // メニュー操作が禁止されている間はフォーム移動禁止

        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const long SC_MOVE = 0xF010L;

            if (menuStrip1.Enabled == false)
            {
                if (m.Msg == WM_SYSCOMMAND && (m.WParam.ToInt64() & 0xFFF0L) == SC_MOVE)
                {
                    m.Result = IntPtr.Zero;
                    return;
                }
            }

            base.WndProc(ref m);
        }

        public Form1()
        {
            InitializeComponent();

            // カウンタ周波数の取得
            QueryPerformanceFrequency(ref freq);

            toolStripStatusLabel1.Text = "";
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // アプリケーション情報書き込み
            writeProperty();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // アプリケーションプロパティ読み込み
            readProperty(Application.StartupPath + "\\property.xml");

            // フォーム位置を検査 
            CheckFormLocation();

            // シリアルポート設定
            if (serial_port == 0) cOM1シリアルポートToolStripMenuItem.PerformClick();
            else USB仮想COMToolStripMenuItem.PerformClick();

            // データグリッド４行
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();
            dataGridView1.Rows.Add();

            // 行の名前
            dataGridView1.Rows[0].HeaderCell.Value = "0x00";
            dataGridView1.Rows[1].HeaderCell.Value = "0x10";
            dataGridView1.Rows[2].HeaderCell.Value = "0x20";
            dataGridView1.Rows[3].HeaderCell.Value = "0x30";

            // 左上隅ヘッダー色
            dataGridView1.TopLeftHeaderCell.Style.BackColor = SystemColors.Control;

            // 列ヘッダーの背景色
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;

            // 行ヘッダーの背景色
            dataGridView1.RowHeadersDefaultCellStyle.BackColor = SystemColors.Control;

            // 列ヘッダー
            for (int i = 0; i < 16; i++)
            {
                //dataGridView1.colum
                dataGridView1.Columns[i].HeaderText = string.Format("0x{0:X2}", i);
            }

            // 選択時のセル色
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.RoyalBlue;

            // 現在の選択セルなし
            dataGridView1.CurrentCell = null;

            // セルクリア
            for (int i = 0; i < 64; i++)
            {
                dataGridView1[i % 16, i / 16].Style.BackColor = Color.White;
                dataGridView1[i % 16, i / 16].Value = "";
            }

            // 子機情報表示
            label1.Text = string.Format("ID : {0,8:X8} {1,8:X8}", Unique_ID >> 32, Unique_ID & 0x00000000ffffffff);
            label2.Text = string.Format("Number : {0:d}", Unique_Numb);
            label3.Text = string.Format("Frequency : {0:d}", Unique_Freq);

            numericUpDown1.Value = parameter_max;

            numericUpDown2.Value = Relay0;
            numericUpDown3.Value = Relay1;
            numericUpDown4.Value = Relay2;
            numericUpDown5.Value = Relay3;

            numericUpDown6.Value = Unique_Numb;
        }

        // アプリケーション情報読み込み 

        private void readProperty(string filename)
        {
            XmlTextReader reader = null;

            try
            {
                reader = new XmlTextReader(filename);

                while (reader.Read())
                {
                    // プロパティを読み込む
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.LocalName.Equals("top")) this.Top = Convert.ToInt32(reader.ReadString());
                        if (reader.LocalName.Equals("left")) this.Left = Convert.ToInt32(reader.ReadString());
                        if (reader.LocalName.Equals("serial_port")) serial_port = Convert.ToInt32(reader.ReadString());
                        if (reader.LocalName.Equals("Unique_ID")) Unique_ID = (UInt64)Convert.ToInt64(reader.ReadString());
                        if (reader.LocalName.Equals("Unique_Numb")) Unique_Numb = (Byte)Convert.ToInt32(reader.ReadString());
                        if (reader.LocalName.Equals("Unique_Freq")) Unique_Freq = (Byte)Convert.ToInt32(reader.ReadString());
                        if (reader.LocalName.Equals("parameter_max")) parameter_max = (Byte)Convert.ToInt32(reader.ReadString());
                        if (reader.LocalName.Equals("Relay0")) Relay0 = Convert.ToInt32(reader.ReadString());
                        if (reader.LocalName.Equals("Relay1")) Relay1 = Convert.ToInt32(reader.ReadString());
                        if (reader.LocalName.Equals("Relay2")) Relay2 = Convert.ToInt32(reader.ReadString());
                        if (reader.LocalName.Equals("Relay3")) Relay3 = Convert.ToInt32(reader.ReadString());
                        if (reader.LocalName.Equals("mode")) mode = Convert.ToInt32(reader.ReadString());
                        if (reader.LocalName.Equals("range")) range = Convert.ToInt32(reader.ReadString());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        // アプリケーション情報書き込み 

        private void writeProperty()
        {
            XmlTextWriter writer = null;

            string filename = Application.StartupPath + "\\property.xml";

            try
            {
                // フォーム位置を書き込む
                writer = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument(true);
                writer.WriteStartElement("property");
                writer.WriteElementString("top", String.Format("{0:0}", this.Top));
                writer.WriteElementString("left", String.Format("{0:0}", this.Left));
                writer.WriteElementString("serial_port", String.Format("{0:0}", serial_port));
                writer.WriteElementString("Unique_ID", String.Format("{0:0}", Unique_ID));
                writer.WriteElementString("Unique_Numb", String.Format("{0:0}", Unique_Numb));
                writer.WriteElementString("Unique_Freq", String.Format("{0:0}", Unique_Freq));
                writer.WriteElementString("parameter_max", String.Format("{0:0}", parameter_max));
                writer.WriteElementString("Relay0", String.Format("{0:0}", Relay0));
                writer.WriteElementString("Relay1", String.Format("{0:0}", Relay1));
                writer.WriteElementString("Relay2", String.Format("{0:0}", Relay2));
                writer.WriteElementString("Relay3", String.Format("{0:0}", Relay3));
                writer.WriteElementString("mode", String.Format("{0:0}", mode));
                writer.WriteElementString("range", String.Format("{0:0}", range));
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

        // フォーム位置を検査 

        private void CheckFormLocation()
        {
            foreach (Screen sc in Screen.AllScreens)
            {
                // タイトルバーがスクリーンの中にあるか
                Rectangle mf = this.DesktopBounds;
                Rectangle cross = sc.Bounds;
                Console.WriteLine(mf);

                // タイトルバー高さ
                mf.Height = 30;
                cross.Intersect(mf);
                if (cross.IsEmpty) continue;

                Console.WriteLine(cross);
                return;
            }

            // フォーム位置がどのスクリーンに重ならないときデフォルト位置にする
            this.Top = 10;
            this.Left = 10;
        }

        // ＤＥＬＡＹ　高精度計測

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
            IntPtr hMenu = GetSystemMenu(this.Handle, false);

            // コントロールボックスの［閉じる］ボタンの無効化
            EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
            DrawMenuBar(hMenu);

            menuStrip1.Enabled = false;

            Application.DoEvents();
        }

        // 操作禁止解除

        private void operation_lift_ban()
        {
            IntPtr hMenu = GetSystemMenu(this.Handle, false);

            // コントロールボックスの［閉じる］ボタンの有効化
            EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_ENABLED);
            DrawMenuBar(hMenu);

            menuStrip1.Enabled = true;
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
            {
                sPort = serialPortSearch();
                serialPort1.BaudRate = 500000;

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

        // ＵＳＢ仮想ＣＯＭ検索

        private int serialPortSearch()
        {
            string did = "", pnp = "";

            int sPort = 0;


            using (ManagementClass sp = new ManagementClass("Win32_SerialPort"))
            {
                foreach (ManagementObject p in sp.GetInstances())
                {
                    did = (string)p.GetPropertyValue("DeviceID");       // ex.COM3
                    pnp = (string)p.GetPropertyValue("PNPDeviceID");    // ex.USB\VID_0CCF&PID_DE00\0001

                    Console.WriteLine("DeviceID：" + did);
                    Console.WriteLine("PNPDeviceID：" + pnp);

                    if ((pnp.IndexOf("VID_C251&PID_2505") != -1) || (pnp.IndexOf("VID_0CCF&PID_0801") != -1))
                    {
                        sPort = System.Convert.ToInt32(did.Substring(3));
                        Console.WriteLine(sPort);
                        break;
                    }
                }
            }

            return sPort;
        }

        // 送信

        /*
        private void sendCommand()
        {
            UInt16 i, len, sum;

            serialPort1.DiscardInBuffer();
            serialPort1.DiscardOutBuffer();

            len = BitConverter.ToUInt16(sData, 3);

            for (sum = 0, i = 0; i < len + 5; i++) sum += sData[i];

            sData[len + 5] = (byte)sum;
            sData[len + 6] = (byte)(sum / 256);

            serialPort1.Write(sData, 0, len + 7);
        }
        */

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

            if(serial_port == 0)
            {
                serialPort1.ReadTimeout = 6500;
                serialPort1.WriteTimeout = 6500;
            }
            else
            {
                serialPort1.ReadTimeout = 6500;
                serialPort1.WriteTimeout = 6500;
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

        private void cOM1シリアルポートToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cOM1シリアルポートToolStripMenuItem.Checked = true;
            USB仮想COMToolStripMenuItem.Checked = false;
            toolStripStatusLabel1.Text = "COM1 Port";
            serial_port = 0;
        }

        private void uSBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cOM1シリアルポートToolStripMenuItem.Checked = false;
            USB仮想COMToolStripMenuItem.Checked = true;
            toolStripStatusLabel1.Text = "USB Port";
            serial_port = 1;
        }

        private void USB仮想COMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cOM1シリアルポートToolStripMenuItem.Checked = false;
            USB仮想COMToolStripMenuItem.Checked = true;
            toolStripStatusLabel1.Text = "USB Port";
            serial_port = 1;
        }
        private void bLEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cOM1シリアルポートToolStripMenuItem.Checked = false;
            USB仮想COMToolStripMenuItem.Checked = true;
            toolStripStatusLabel1.Text = "BLE Port";
            serial_port = 2;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (cancel == 0)
            {
                cancel = 1;
                button1.Enabled = false;
            }

            dataGridView1.CurrentCell = null;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            parameter_max = (Byte)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Relay0 = (int)numericUpDown2.Value;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            Relay1 = (int)numericUpDown3.Value;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            Relay2 = (int)numericUpDown4.Value;
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            Relay3 = (int)numericUpDown5.Value;
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            Unique_Numb = (Byte)numericUpDown6.Value;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (time_out > 10) time_out -= 10;
            else time_out = 0;
        }

        private void DeepPowerDown解除ToolStripMenuItem_Click(object sender, EventArgs e)
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
            delay(500);

            // ＣＰＵ　ＩＤ確認
            sData[0] = 0x01;
            sData[1] = (Byte)CMD_ID;
            sData[2] = 0x00;
            sData[3] = 0x00;
            sData[4] = 0x00;

            sendCommand();
            if (recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

        error_proc: ;
            richTextBox1.AppendText("\n終了");
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
        }

        private void エコーバック通信EToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UInt16 len, sum;

            UInt32 i;

            int err;


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

            for (i = 0; i < 1024; i++) sData[5 + i] = (Byte)i;

            sData[0] = 0x01;
            sData[1] = (Byte)CMD_USB_TEST;
            sData[2] = 0x00;

            sData[3] = (Byte)0xf9;
            sData[4] = (Byte)0x03;

            sData[3] = (Byte)0x00;
            sData[4] = (Byte)0x04;

            len = BitConverter.ToUInt16(sData, 3);

            for (sum = 0, i = 0; i < (len + 5); i++) sum += sData[i];

            sData[len + 5] = (byte)sum;
            sData[len + 6] = (byte)(sum / 256);

            err = 0;
            cancel = 0;

            for (i = 0; i < 1000000; i++)
            {
                if (cancel != 0) break;

                rData[2] = 0x00;

                serialPort1.Write(sData, 0, len + 7);

                if ((recvCommand() == false) || (rData[2] != 0x06))
                {
                    err++;

                    serialPort1.Close();
                    delay(600);

                    if (serialPortOpen() == false) goto error_proc;

                    serialPort1.DiscardInBuffer();
                    serialPort1.DiscardOutBuffer();

                    // ブレーク信号送出
                    sData[0] = 0x00;
                    serialPort1.Write(sData, 0, 1);
                    delay(50);

                    sData[0] = 0x01;
                }

                richTextBox1.AppendText(String.Format("送信回数 : {0:d}  エラー回数 : {1:d}\r\n", i, err));
                Application.DoEvents();
            }

        error_proc: ;
            richTextBox1.AppendText("終了\r\n");
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
            button1.PerformClick();
        }

        private void ｎＲＦ５１ブートモード設定BToolStripMenuItem_Click(object sender, EventArgs e)
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

            // 通信権利　確保
            sData[0] = 0x01;
            sData[1] = (Byte)CMD_nRF_MODE;
            sData[2] = 0x01;            

            sData[3] = (Byte)0x08;
            sData[4] = (Byte)0x00;

            Buffer.BlockCopy(password_user, 0, sData, 5, 8);

            sendCommand();
            if (recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            richTextBox1.AppendText("通信権利　確保\r\n");
            Application.DoEvents();

            // ｎＲＦ５１をブートモードに入れる（ＲＥＳＥＴ：０　ＢＯＯＴ：０）
            sData[0] = 0x01;
            sData[1] = (Byte)CMD_nRF_MODE;
            sData[2] = 0x02;

            sData[3] = (Byte)0x08;
            sData[4] = (Byte)0x00;

            sData[5] = (Byte)0x00;
            sData[6] = (Byte)0x00;

            sendCommand();
            if (recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            richTextBox1.AppendText("ＲＥＳＥＴ：０　ＢＯＯＴ：０\r\n");
            Application.DoEvents();
            delay(1000);

            // ＲＥＳＥＴ：１　ＢＯＯＴ：０
            sData[0] = 0x01;
            sData[1] = (Byte)CMD_nRF_MODE;
            sData[2] = 0x02;

            sData[3] = (Byte)0x08;
            sData[4] = (Byte)0x00;

            sData[5] = (Byte)0x01;
            sData[6] = (Byte)0x00;

            sendCommand();
            if (recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            richTextBox1.AppendText("ＲＥＳＥＴ：１　ＢＯＯＴ：０\r\n");
            Application.DoEvents();
            delay(1000);

            // ＲＥＳＥＴ：１　ＢＯＯＴ：１
            sData[0] = 0x01;
            sData[1] = (Byte)CMD_nRF_MODE;
            sData[2] = 0x02;

            sData[3] = (Byte)0x08;
            sData[4] = (Byte)0x00;

            sData[5] = (Byte)0x01;
            sData[6] = (Byte)0x01;

            sendCommand();
            if (recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            richTextBox1.AppendText("ＲＥＳＥＴ：１　ＢＯＯＴ：１\r\n");
            Application.DoEvents();
            delay(500);

        error_proc: ;
            richTextBox1.AppendText("終了");
            Application.DoEvents();
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
        }

        private void ｎＲＦ５１ブートモード解除ToolStripMenuItem_Click(object sender, EventArgs e)
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

            // ＲＥＳＥＴ：０　ＢＯＯＴ：１
            sData[0] = 0x01;
            sData[1] = (Byte)CMD_nRF_MODE;
            sData[2] = 0x02;

            sData[3] = (Byte)0x08;
            sData[4] = (Byte)0x00;

            sData[5] = (Byte)0x00;
            sData[6] = (Byte)0x01;

            sendCommand();
            if (recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            richTextBox1.AppendText("ＲＥＳＥＴ：０　ＢＯＯＴ：１\r\n");
            Application.DoEvents();
            delay(1000);

            // ＲＥＳＥＴ：１　ＢＯＯＴ：１
            sData[0] = 0x01;
            sData[1] = (Byte)CMD_nRF_MODE;
            sData[2] = 0x02;

            sData[3] = (Byte)0x08;
            sData[4] = (Byte)0x00;

            sData[5] = (Byte)0x01;
            sData[6] = (Byte)0x01;

            sendCommand();
            if (recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            richTextBox1.AppendText("ＲＥＳＥＴ：１　ＢＯＯＴ：１\r\n");
            Application.DoEvents();
            delay(1000);

            // 通信権利　開放
            sData[0] = 0x01;
            sData[1] = (Byte)CMD_nRF_MODE;
            sData[2] = 0x00;

            sData[3] = (Byte)0x08;
            sData[4] = (Byte)0x00;

            Buffer.BlockCopy(password_user, 0, sData, 5, 8);

            sendCommand();
            if (recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            richTextBox1.AppendText("通信権利　開放\r\n");
            Application.DoEvents();
            delay(1000);

        error_proc: ;
            richTextBox1.AppendText("終了");
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
        }

        private void ｎＲＦ５１ＳＬＩＰブロックラップToolStripMenuItem_Click(object sender, EventArgs e)
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

            sData[0] = 0x01;
            sData[1] = (Byte)CMD_nRF_SLIPWRAP;
            sData[2] = 0x00;

            /*
            sData[3] = (Byte)0x10;
            sData[4] = (Byte)0x00;

            sData[5] = (Byte)0xc0;
            
            sData[6] = (Byte)0xc1;
            sData[7] = (Byte)0x4e;
            sData[8] = (Byte)0x00;
            sData[9] = (Byte)0xf1;

            sData[10] = (Byte)0x02;
            sData[11] = (Byte)0x00;
            sData[12] = (Byte)0x00;
            sData[13] = (Byte)0x00;

            sData[14] = (Byte)0xe8;
            sData[15] = (Byte)0x43;
            sData[16] = (Byte)0x00;
            sData[17] = (Byte)0x00;

            sData[18] = (Byte)0xc1;
            sData[19] = (Byte)0xa7;

            sData[20] = (Byte)0xc0;
            */

            sData[3] = (Byte)((32 - 5) + 1);
            sData[4] = (Byte)0x00;
            //
            sData[5] = (Byte)0xc0;

            sData[6] = (Byte)0xd1;
            sData[7] = (Byte)0x4e;
            sData[8] = (Byte)0x01;
            sData[9] = (Byte)0xe0;

            sData[10] = (Byte)0x03;
            sData[11] = (Byte)0x0;
            sData[12] = (Byte)0x0;
            sData[13] = (Byte)0x0;

            sData[14] = (Byte)0x04;
            sData[15] = (Byte)0x0;
            sData[16] = (Byte)0x0;
            sData[17] = (Byte)0x0;

            sData[18] = (Byte)0x0;
            sData[19] = (Byte)0x0;
            sData[20] = (Byte)0x0;
            sData[21] = (Byte)0x0;

            sData[22] = (Byte)0x0;
            sData[23] = (Byte)0x0;
            sData[24] = (Byte)0x0;
            sData[25] = (Byte)0x0;

            sData[26] = (Byte)0xf0;
            sData[27] = (Byte)0x51;
            sData[28] = (Byte)0x00;
            sData[29] = (Byte)0x00;

            sData[30] = (Byte)0xd8;
            sData[31] = (Byte)0x59;

            sData[32] = (Byte)0xc0;

            sendCommand();
            if (recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
            }

        error_proc: ;
            richTextBox1.AppendText("終了\r\n");
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
        }

        private void wUINF機器情報の設定ToolStripMenuItem_Click(object sender, EventArgs e)
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

            string[] command = { "WUINF:", "UNITS=", "NAME=", "DESC=", };

            command[1] += "0";
            command[2] += "RTR-500BLE\x00";
            command[3] += "Aug.7.2015 15:25:00\x00";

            command_make(command);

            sendCommand();
            if (t_recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            extraction();   // パラメータ抽出

        error_proc: ;
            richTextBox1.AppendText("終了\r\n");
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
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

            command_make(command);

            sendCommand();
            if (t_recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            extraction();   // パラメータ抽出

        error_proc: ;
            richTextBox1.AppendText("終了\r\n");
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
        }

        private void wDTIM時刻情報の設定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.TimeZoneInfo tzi = System.TimeZoneInfo.Local;


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
            string[] command = { "WDTIM:", "DTIME=", "DIFF=", "DST=" };

            DateTime dt = new DateTime();

            int zen = 60;

            while (true)
            {
                dt = System.DateTime.Now;
                richTextBox1.AppendText(dt.ToString("yyyy/MM/dd/ HH:mm:ss.") + dt.Millisecond.ToString() + "\r\n");
                Application.DoEvents();

                if ((zen != 60) && (dt.Second != zen)) break;
                zen = dt.Second;

                //if ((dt.Millisecond > 800) && (dt.Millisecond < 850)) break;

                System.Threading.Thread.Sleep(10);
            }

            int jisa = tzi.BaseUtcOffset.Hours * 3600 + tzi.BaseUtcOffset.Minutes * 60 + tzi.BaseUtcOffset.Seconds;
            int dst = tzi.GetUtcOffset(dt).Hours * 3600 + tzi.GetUtcOffset(dt).Minutes * 60 + tzi.GetUtcOffset(dt).Seconds - jisa;

            command[1] += dt.ToString("yyyyMMddHHmmss");

            if (jisa < 0) command[2] += string.Format("-{0:0#}{1:0#}", -jisa / 3600, (-jisa / 60) % 60);
            else command[2] += string.Format("+{0:0#}{1:0#}", jisa / 3600, (jisa / 60) % 60);

            if (dst < 0) command[3] += string.Format("-{0:0#}{1:0#}", -dst / 3600, (-dst / 60) % 60);
            else command[3] += string.Format("+{0:0#}{1:0#}", dst / 3600, (dst / 60) % 60);

            command_make(command);

            sendCommand();
            if (t_recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            extraction();   // パラメータ抽出

        error_proc: ;
            richTextBox1.AppendText("終了\r\n");
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
        }

        private void rDTIM時刻情報の取得ToolStripMenuItem_Click(object sender, EventArgs e)
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
            string[] command = { "RDTIM:" };

            command_make(command);

            sendCommand();
            if (t_recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            extraction();   // パラメータ抽出

        error_proc: ;
            richTextBox1.AppendText("終了\r\n");
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
        }

        private void wRUST中継機情報の設定ToolStripMenuItem_Click(object sender, EventArgs e)
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
            string[] command = { "WRUST:", "ID=", "NUM=", "CH=", };

            for (int i = 0; i < 8; i++) command[1] += hex((Byte)(Unique_ID >> (i * 8)));    // グループＩＤ
            command[2] += String.Format("{0,1:d1}", parameter_max);
            command[3] += String.Format("{0,1:d1}", Unique_Freq);

            command_make(command);

            sendCommand();
            if (t_recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            extraction();   // パラメータ抽出

        error_proc: ;
            richTextBox1.AppendText("終了\r\n");
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
        }

        private void rRUST中継機情報の取得ToolStripMenuItem_Click(object sender, EventArgs e)
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
            string[] command = { "RRUST:" };

            command_make(command);

            sendCommand();
            if (t_recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            extraction();   // パラメータ抽出

        error_proc: ;
            richTextBox1.AppendText("終了\r\n");
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
        }

        private void wUSRDユーザ定義情報の設定ToolStripMenuItem_Click(object sender, EventArgs e)
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
            string[] command = { "WUSRD:", "DATA=", };

            command[1] += "USER AREA 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ@\x00";

            command_make(command);

            sendCommand();
            if (t_recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            extraction();   // パラメータ抽出

        error_proc: ;
            richTextBox1.AppendText("終了\r\n");
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
        }

        private void rUSRDユーザ定義情報の取得ToolStripMenuItem_Click(object sender, EventArgs e)
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
            string[] command = { "RUSRD:" };

            command_make(command);

            sendCommand();
            if (t_recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            extraction();   // パラメータ抽出

        error_proc: ;
            richTextBox1.AppendText("終了\r\n");
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
        }

        private void eINIT本機の初期化と再起動ToolStripMenuItem_Click(object sender, EventArgs e)
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
            string[] command = { "EINIT:", "MODE=" };

            //command[1] += "RESET";
            command[1] += "FACTORY";

            command_make(command);

            sendCommand();
            if (t_recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            extraction();   // パラメータ抽出

        error_proc: ;
            richTextBox1.AppendText("終了\r\n");
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
        }

        private void eISERシリアル番号の取得ToolStripMenuItem_Click(object sender, EventArgs e)
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
            string[] command = { "EISER:" };

            command_make(command);

            sendCommand();
            if (t_recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            extraction();   // パラメータ抽出

        error_proc: ;
            richTextBox1.AppendText("終了\r\n");
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
        }

        private void eISET設定値情報の書き込みToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox1.AppendText("未作成\r\n");
        }

        private void eISET設定値情報の読み込みToolStripMenuItem_Click(object sender, EventArgs e)
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
            string[] command = { "EISET:", "ACT=1" };

            command_make(command);

            sendCommand();
            if (t_recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            extraction();   // パラメータ抽出

            if (BitConverter.ToInt16(tData, 0) > 0)
            {
                for (int adr = 0; adr < 64; adr++)
                {
                    dataGridView1[adr % 16, adr / 16].Value = String.Format(" 0x{0,2:X2}", tData[2 + adr]);
                    dataGridView1.CurrentCell = dataGridView1[adr % 16, adr / 16];
                }

                Unique_ID = 0x00;
                for (int i = 0; i < 8; i++) Unique_ID = (Unique_ID << 8) + tData[2 + 51 - i];

                Unique_Freq = tData[2 + 61];
                Unique_Numb = tData[2 + 60];
            }
            else
            {
                Unique_ID = 0x00;
                Unique_Freq = 0x00;
                Unique_Numb = 0x00;
            }

            label1.Text = string.Format("ID : {0,8:X8} {1,8:X8}", Unique_ID >> 32, Unique_ID & 0x00000000ffffffff);
            label2.Text = string.Format("Number : {0:d}", Unique_Numb);
            label3.Text = string.Format("Frequency : {0:d}", Unique_Freq);

        error_proc: ;
            richTextBox1.AppendText("終了\r\n");
            serialPort1.Close();

            dataGridView1.CurrentCell = null;

            // 操作禁止解除
            operation_lift_ban();
        }

        private void eISUCデータ吸い上げToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 操作禁止
            operation_ban();
            button1.Enabled = true;

            richTextBox1.Clear();

            // 吸い上げモード、レンジ指定
            using (Form3 fm3 = new Form3())
            {
                this.AddOwnedForm(fm3);
                fm3.Show();

                while (fm3.IsDisposed == false)
                {
                    Application.DoEvents();
                    delay(100);
                }
            }

            cancel = 0;

            if (advance == true)
            {
                if (serialPortOpen() == false)
                {
                    richTextBox1.AppendText("ポートオープン失敗\r\n");
                    goto error_proc;
                }

                if (cancel != 0)
                {
                    richTextBox1.AppendText("中断\r\n");
                    goto error_proc;
                }

                // ブレーク信号送出
                sData[0] = 0x00;
                serialPort1.Write(sData, 0, 1);
                delay(50);

                // コマンド送出
                string[] command = { "EISUC:", "MODE=", "RANGE=" };

                command[1] += String.Format("{0,1:d1}", mode);
                command[2] += String.Format("{0,1:d1}", range);

                command_make(command);

                sendCommand();
                if (t_recvCommand() == false)
                {
                    richTextBox1.AppendText("受信異常\r\n");
                    goto error_proc;
                }

                extraction();   // パラメータ抽出

                string key = "SIZE=";
                int i = key_find(key);
                if (i > 0) i = System.Convert.ToInt32(Encoding.ASCII.GetString(rData, i + key.Length + 2, rData[i + key.Length] + (UInt16)rData[i + key.Length + 1]) + "\r\n");
                richTextBox1.AppendText(String.Format("転送バイト数 : {0:d}\r\n", i));

                for (int j = 1; j < i; j += 0)
                {
                    command[0] = "EISUC:";
                    command[1] = "MODE=9";
                    command[2] = "";

                    command_make(command);

                    sendCommand();
                    if (t_recvCommand() == false) goto error_proc;

                    extraction();   // パラメータ抽出

                    j += 1024;

                    delay(50);
                }
            }

        error_proc:;

            richTextBox1.AppendText("終了\r\n");
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
        }

        private void eWSTR子機の設定値取得ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string status;


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
            string[] command = { "EWSTR:", "ACT=", "NUM=", "ID=", "CH=", "ROUTE=", "RELAY=" };

            command[1] += "0";

            command[2] += String.Format("{0,1:d1}", Unique_Numb);                           // 子機番号
            for (int i = 0; i < 8; i++) command[3] += hex((Byte)(Unique_ID >> (i * 8)));    // グループＩＤ
            command[4] += String.Format("{0,1:d1}", Unique_Freq);                           // 周波数

            route = 1;

            command[5] += hex((Byte)(route));                   // ルート　０：自動　１：ＲＥＬＡＹパラメータによる

            command[6] += hex((Byte)0x00);                      // ここは必ず０ｘ００

            if (Relay0 > 0) command[6] += hex((Byte)Relay1);    // １番目の中継機
            if (Relay0 > 1) command[6] += hex((Byte)Relay2);    // ２番目の中継機
            if (Relay0 > 2) command[6] += hex((Byte)Relay3);    // ３番目の中継機

            command_make(command);

            sendCommand();
            if (t_recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            extraction();   // パラメータ抽出

            // 終了待ち

            cancel = 0;

            while (true)
            {
                string[] act1 = { "EWSTR:", "ACT=1" };
                string[] act2 = { "EWSTR:", "ACT=2" };

                if (cancel != 0)
                {
                    command_make(act2);
                    sendCommand();
                    t_recvCommand();
                    break;
                }

                delay(1000);
                Application.DoEvents();

                command_make(act1);

                sendCommand();
                if (t_recvCommand() == false)
                {
                    richTextBox1.AppendText("受信異常\r\n");
                    goto error_proc;
                }

                // パラメータ抽出
                key_find("TIME=");
                key_find("LAST=");
                data_find();

                int len = (tData[0] + (UInt16)tData[1] * 256);

                if (len != 0)
                {
                    richTextBox1.AppendText("-------------------------------\r\n");

                    for (int j = 0; j < len; j++)
                    {
                        richTextBox1.AppendText(String.Format("0x{0,2:X2} ", tData[2 + j]));
                    }

                    richTextBox1.AppendText("\r\n-------------------------------\r\n");

                    for (int adr = 0; adr < 64; adr++)
                    {
                        dataGridView1[adr % 16, adr / 16].Value = String.Format(" 0x{0,2:X2}", tData[2 + adr]);
                        dataGridView1.CurrentCell = dataGridView1[adr % 16, adr / 16];
                    }

                    break;
                }

                int i = status_find();
                status = Encoding.ASCII.GetString(rData, i + 9, rData[i + 7] + (UInt16)rData[i + 8] * 256);
                if ((status.Substring(0, 4) == "0004") && (status.Substring(5, 4) == "0000")) break;
            }

        error_proc: ;
            richTextBox1.AppendText("終了\r\n");
            serialPort1.Close();

            dataGridView1.CurrentCell = null;

            // 操作禁止解除
            operation_lift_ban();
        }

        private void eWCUR子機の現在値読み込みToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UInt16 exponent;
            UInt16 mantissa;
            UInt16 a, pow, n;
            string status;

            
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
            string[] command = { "EWCUR:", "ACT=", "NUM=", "ID=", "CH=", "ROUTE=", "RELAY=", "FORMAT=" };

            command[1] += "0";

            command[2] += String.Format("{0,1:d1}", Unique_Numb);                           // 子機番号
            for (int i = 0; i < 8; i++) command[3] += hex((Byte)(Unique_ID >> (i * 8)));    // グループＩＤ
            command[4] += String.Format("{0,1:d1}", Unique_Freq);                           // 周波数

            route = 1;

            command[5] += hex((Byte)(route));                   // ルート　０：自動　１：ＲＥＬＡＹパラメータによる
            
            command[6] += hex((Byte)0x00);                      // ここは必ず０ｘ００

            if (Relay0 > 0) command[6] += hex((Byte)Relay1);    // １番目の中継機
            if (Relay0 > 1) command[6] += hex((Byte)Relay2);    // ２番目の中継機
            if (Relay0 > 2) command[6] += hex((Byte)Relay3);    // ３番目の中継機

            command[7] += String.Format("{0,1:d1}", 5);

            command_make(command);

            sendCommand();
            if (t_recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            extraction();   // パラメータ抽出

            // 終了待ち

            cancel = 0;

            while (true)
            {
                string[] act1 = { "EWCUR:", "ACT=1" };
                string[] act2 = { "EWCUR:", "ACT=2" };

                if (cancel != 0)
                {
                    command_make(act2);
                    sendCommand();
                    t_recvCommand();
                    break;
                }

                delay(1000);
                Application.DoEvents();

                command_make(act1);

                sendCommand();
                t_recvCommand();

                // パラメータ抽出
                int i = key_find("TIME=");
                key_find("LAST=");
                data_find();

                if (i != 0)
                {
                    richTextBox1.AppendText("-------------------------------\r\n");

                    Byte batt = (Byte)(tData[2 + 4] & 0x0f);

                    richTextBox1.AppendText(String.Format("子機番号 : {0:d}  RSSI : 0x{1,2:X2}({2:d}) BATT : 0x{3,2:X2} ", tData[2 + 0], tData[2 + 1], tData[2 + 1], batt));

                    if (batt >= 10) richTextBox1.AppendText("(データ消失恐れ有り) ");
                    else if (batt >= 8) richTextBox1.AppendText("(直ぐに電池交換を) ");
                    else if (batt >= 6) richTextBox1.AppendText("(早めに電池交換を) ");
                    else if (batt >= 4) richTextBox1.AppendText("(半分以下) ");
                    else if (batt >= 3) richTextBox1.AppendText("(少なくなり始めた) ");
                    else richTextBox1.AppendText("(十分あり) ");

                    //richTextBox1.AppendText(String.Format("???? : 0x{0,2:X2} ", (Byte)(tData[2 + 9])));

                    Byte rec = (Byte)(tData[2 + 9] & 0x03);
                    richTextBox1.AppendText(String.Format("記録状況 : 0x{0,2:X2} ", rec));

                    if (rec == 0x00) richTextBox1.AppendText("(記録停止)\r\n");
                    else if (rec == 0x01) richTextBox1.AppendText("(記録中)\r\n");
                    else if (rec == 0x02) richTextBox1.AppendText("(予約中)\r\n");

                    if ((tData[2 + 9] & 0x7c) == 0x10)
                    {
                        richTextBox1.AppendText(String.Format("温度 : {0:#0.0}[℃]\r\n", (tData[2 + 18] + (UInt16)tData[2 + 19] * 256 - 1000.0) / 10.0));
                        richTextBox1.AppendText(String.Format("湿度 : {0:#0.0}[％]\r\n", (tData[2 + 28] + (UInt16)tData[2 + 29] * 256 - 1000.0) / 10.0));
                    }
                    else if ((tData[2 + 9] & 0x7c) == 0x00)
                    {
                        richTextBox1.AppendText(String.Format("温度 : {0:#0.0}[℃]\r\n", (tData[2 + 28] + (UInt16)tData[2 + 29] * 256 - 1000.0) / 10.0));
                    }
                    else if ((tData[2 + 9] & 0x7c) == 0x18)
                    {
                        richTextBox1.AppendText(String.Format("温度 : {0:#0.0}[℃]\r\n", (tData[2 + 18] + (UInt16)tData[2 + 19] * 256 - 1000.0) / 10.0));
                        richTextBox1.AppendText(String.Format("湿度 : {0:#0.0}[％]\r\n", (tData[2 + 28] + (UInt16)tData[2 + 29] * 256 - 1000.0) / 10.0));
                    }
                    else if ((tData[2 + 9] & 0x7c) == 0x28)
                    {
                        a = (UInt16)(tData[2 + 18] + (UInt16)tData[2 + 19] * 256);
                        exponent = (UInt16)((a >> 12) & 0x00ff);						// 指数部
                        mantissa = (UInt16)(a & 0x0fff);								// 仮数部

                        for (pow = 1, n = 0; n < exponent; n++) pow *= 2;

                        richTextBox1.AppendText(String.Format("照度 : {0:#0.000}[lx]\r\n", (UInt64)mantissa * pow / 100.0));

                        a = (UInt16)(tData[2 + 28] + (UInt16)tData[2 + 29] * 256);
                        exponent = (UInt16)((a >> 12) & 0x00ff);						// 指数部
                        mantissa = (UInt16)(a & 0x0fff);								// 仮数部

                        for (pow = 1, n = 0; n < exponent; n++) pow *= 2;

                        richTextBox1.AppendText(String.Format("ＵＶ : {0:#0.000}[mW/cm^2]\r\n", (UInt64)mantissa * pow / 1000.0));
                    }
                    else if ((tData[2 + 9] & 0x7c) == 0x78)
                    {
                        a = (UInt16)(tData[2 + 18] + (UInt16)tData[2 + 19] * 256);
                        exponent = (UInt16)((a >> 12) & 0x000f);						// 指数部
                        mantissa = (UInt16)(a & 0x0fff);								// 仮数部

                        if ((exponent & 0x0008) == 0x0000)
                        {
                            for (pow = 1, n = 0; n < exponent; n++) pow *= 2;

                            richTextBox1.AppendText(String.Format("電流 : {0:#00.00}[mA]\r\n", (UInt64)mantissa * pow / 100.0));
                        }
                        else
                        {
                            exponent &= 0x0007;
                            mantissa |= 0xf000;

                            for (pow = 1, n = 0; n < exponent; n++) pow *= 2;

                            richTextBox1.AppendText(String.Format("電流 : {0:#0.00}[mA]\r\n", ((0x10000 - mantissa) * (-1) * pow / 100.0)));
                        }
                    }
                    else if ((tData[2 + 9] & 0x7c) == 0x58)
                    {
                        a = (UInt16)(tData[2 + 18] + (UInt16)tData[2 + 19] * 256);
                        exponent = (UInt16)((a >> 12) & 0x000f);						// 指数部
                        mantissa = (UInt16)(a & 0x0fff);								// 仮数部

                        if ((exponent & 0x0008) == 0x0000)
                        {
                            for (pow = 1, n = 0; n < exponent; n++) pow *= 2;

                            richTextBox1.AppendText(String.Format("電圧 : {0:#0.0000}[V]\r\n", (UInt64)mantissa * pow / 10000.0));
                        }
                        else
                        {
                            exponent &= 0x0007;
                            mantissa |= 0xf000;

                            for (pow = 1, n = 0; n < exponent; n++) pow *= 2;

                            richTextBox1.AppendText(String.Format("電圧 : {0:#0.0000}[V]\r\n", ((0x10000 - mantissa) * (-1) * pow / 10000.0)));
                        }

                    }
                    else if ((tData[2 + 9] & 0x7c) == 0x0c)
                    {
                        richTextBox1.AppendText(String.Format("温度 : {0:#0.0}[℃]\r\n", (tData[2 + 28] + (UInt16)tData[2 + 29] * 256 - 1000.0) / 10.0));
                    }
                    else if ((tData[2 + 9] & 0x0c) == 0x04)
                    {
                        richTextBox1.AppendText(String.Format("パルス　 : {0:0}[Pulse]\r\n", (tData[2 + 18] + (UInt16)tData[2 + 19] * 256)));
                        richTextBox1.AppendText(String.Format("総パルス : {0:0}[Pulse]\r\n", (tData[2 + 26] + (UInt32)tData[2 + 27] * 256 + (UInt32)tData[2 + 28] * 65536 + (UInt32)tData[2 + 29] * 16777216)));
                    }
                    else
                    {


                    }

                    /*
                    if ((tData[2 + 9] & 0x78) == 0x10)
                    {
                        richTextBox1.AppendText(String.Format("温度 : {0:#0.0}[℃]\r\n", (tData[2 + 18] + (UInt16)tData[2 + 19] * 256 - 1000.0) / 10.0));
                        richTextBox1.AppendText(String.Format("湿度 : {0:#0.0}[％]\r\n", (tData[2 + 28] + (UInt16)tData[2 + 29] * 256 - 1000.0) / 10.0));
                    }
                    else if ((tData[2 + 9] & 0x78) == 0x00)
                    {
                        richTextBox1.AppendText(String.Format("温度 : {0:#0.0}[℃]\r\n", (tData[2 + 28] + (UInt16)tData[2 + 29] * 256 - 1000.0) / 10.0));
                    }
                    else if ((tData[2 + 9] & 0x78) == 0x18)
                    {
                        richTextBox1.AppendText(String.Format("温度 : {0:#0.0}[℃]\r\n", (tData[2 + 18] + (UInt16)tData[2 + 19] * 256 - 1000.0) / 10.0));
                        richTextBox1.AppendText(String.Format("湿度 : {0:#0.0}[％]\r\n", (tData[2 + 28] + (UInt16)tData[2 + 29] * 256 - 1000.0) / 10.0));
                    }
                    else if ((tData[2 + 9] & 0x78) == 0x28)
                    {
                        UInt16 exponent;
                        UInt16 mantissa;
                        UInt16 a, pow, n;

                        a = (UInt16)(tData[2 + 18] + (UInt16)tData[2 + 19] * 256);
                        exponent = (UInt16)((a >> 12) & 0x00ff);						// 指数部
                        mantissa = (UInt16)(a & 0x0fff);								// 仮数部

                        for (pow = 1, n = 0; n < exponent; n++) pow *= 2;

                        richTextBox1.AppendText(String.Format("照度 : {0:#0.000}[lx]\r\n", (UInt64)mantissa * pow / 100.0));

                        a = (UInt16)(tData[2 + 28] + (UInt16)tData[2 + 29] * 256);
                        exponent = (UInt16)((a >> 12) & 0x00ff);						// 指数部
                        mantissa = (UInt16)(a & 0x0fff);								// 仮数部

                        for (pow = 1, n = 0; n < exponent; n++) pow *= 2;

                        richTextBox1.AppendText(String.Format("ＵＶ : {0:#0.000}[mW/cm^2]\r\n", (UInt64)mantissa * pow / 1000.0));
                    }
                    */

                    richTextBox1.AppendText("-------------------------------\r\n");

                    break;
                }

                i = status_find();
                status = Encoding.ASCII.GetString(rData, i + 9, rData[i + 7] + (UInt16)rData[i + 8] * 256);
                if ((status.Substring(0, 4) == "0004") && (status.Substring(5, 4) == "0000")) break;

            }

        error_proc: ;
            richTextBox1.AppendText("終了\r\n");
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
        }

        private void eWSUC子機のデータ吸い上げToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 操作禁止
            operation_ban();
            button1.Enabled = true;

            richTextBox1.Clear();

            // 吸い上げモード、レンジ指定
            using (Form3 fm3 = new Form3())
            {
                this.AddOwnedForm(fm3);
                fm3.Show();

                while (fm3.IsDisposed == false)
                {
                    Application.DoEvents();
                    delay(100);
                }
            }

            cancel = 0;

            if (advance == true)
            {
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
                string[] command = { "EWSUC:", "ACT=", "NUM=", "ID=", "CH=", "ROUTE=", "RELAY=", "MODE=", "RANGE=" };

                command[1] += "0";

                command[2] += String.Format("{0,1:d1}", Unique_Numb);                           // 子機番号
                for (int n = 0; n < 8; n++) command[3] += hex((Byte)(Unique_ID >> (n * 8)));    // グループＩＤ
                command[4] += String.Format("{0,1:d1}", Unique_Freq);                           // 周波数

                route = 1;

                command[5] += hex((Byte)(route));                   // ルート　０：自動　１：ＲＥＬＡＹパラメータによる

                command[6] += hex((Byte)0x00);                      // ここは必ず０ｘ００
                if (Relay0 > 0) command[6] += hex((Byte)Relay1);    // １番目の中継機
                if (Relay0 > 1) command[6] += hex((Byte)Relay2);    // ２番目の中継機
                if (Relay0 > 2) command[6] += hex((Byte)Relay3);    // ３番目の中継機

                command[7] += String.Format("{0,1:d1}", mode);
                command[8] += String.Format("{0,1:d1}", range);

                command_make(command);

                sendCommand();
                if (t_recvCommand() == false)
                {
                    richTextBox1.AppendText("受信異常\r\n");
                    goto error_proc;
                }

                extraction();   // パラメータ抽出

                while (true)
                {
                    if (cancel != 0)
                    {
                        richTextBox1.AppendText("中断\r\n");
                        goto error_proc;
                    }

                    delay(1100);
                    Application.DoEvents();

                    string[] act1 = { "EWSUC:", "ACT=3"};

                    command_make(act1);

                    sendCommand();
                    t_recvCommand();

                    //extraction();   // パラメータ抽出

                    string key = "SIZE=";
                    int s = key_find(key);

                    key_find("TIME=");
                    key_find("LAST=");
                    int i = status_find();

                    string status = Encoding.ASCII.GetString(rData, i + 9, rData[i + 7] + (UInt16)rData[i + 8] * 256);
                    if (status.Substring(0, 4) == "0004" && status.Substring(5, 4) == "0000")
                    {
                        if (s > 0) s = System.Convert.ToInt32(Encoding.ASCII.GetString(rData, s + key.Length + 2, rData[s + key.Length] + (UInt16)rData[s + key.Length + 1]) + "\r\n");
                        if (s == 0) break;
                    }
                    else if (status.Substring(0, 4) != "0004" || status.Substring(5, 4) != "0001") break;
                }
            }

        error_proc: ;
            richTextBox1.AppendText("終了\r\n");
            serialPort1.Close();

            // 操作禁止解除
            operation_lift_ban();
        }

        private void eWRSC子機からの一斉データ取得ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string status;

            
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
            string[] command = { "EWRSC:", "ACT=", "MAX=", "ID=", "CH=", "ROUTE=", "RELAY=", "FORMAT=" };

            command[1] += "0";

            command[2] += String.Format("{0,1:d1}", parameter_max);                         // 最大子機番号
            for (int i = 0; i < 8; i++) command[3] += hex((Byte)(Unique_ID >> (i * 8)));    // グループＩＤ
            command[4] += String.Format("{0,1:d1}", Unique_Freq);                           // 周波数

            route = 1;

            command[5] += hex((Byte)(route));                   // ルート　０：自動　１：ＲＥＬＡＹパラメータによる

            command[6] += hex((Byte)0x00);                      // ここは必ず０ｘ００

            if (Relay0 > 0) command[6] += hex((Byte)Relay1);    // １番目の中継機
            if (Relay0 > 1) command[6] += hex((Byte)Relay2);    // ２番目の中継機
            if (Relay0 > 2) command[6] += hex((Byte)Relay3);    // ３番目の中継機

            command[7] += String.Format("{0,1:d1}", 5);

            command_make(command);

            sendCommand();
            if (t_recvCommand() == false)
            {
                richTextBox1.AppendText("受信異常\r\n");
                goto error_proc;
            }

            extraction();   // パラメータ抽出

            // 終了待ち

            cancel = 0;

            while (true)
            {
                string[] act1 = { "EWRSC:", "ACT=1" };
                string[] act2 = { "EWRSC:", "ACT=2" };

                if (cancel != 0)
                {
                    command_make(act2);
                    sendCommand();
                    t_recvCommand();
                    break;
                }

                delay(1000);
                Application.DoEvents();

                command_make(act1);

                sendCommand();
                if (t_recvCommand() == false)
                {
                    richTextBox1.AppendText("受信異常\r\n");
                    goto error_proc;
                }

                // パラメータ抽出
                int i = key_find("TIME=");
                key_find("LAST=");
                data_find();

                if (i != 0)
                {
                    int len = (tData[2] + (UInt16)tData[3] * 256);

                    if(len == 0)
                    {
                        richTextBox1.AppendText("受信データ長が０\r\n");
                    }

                    richTextBox1.AppendText("-------------------------------\r\n");

                    //if ((sum & 0xffff) != (rData[len - 2] + (int)rData[len - 1] * 256)) return false;


                    for (int j = 2; j < (len + 2); j += 30)
                    {
                        Byte batt = (Byte)(tData[2 + j + 4] & 0x0f);

                        richTextBox1.AppendText(String.Format("子機番号 : {0:d}  RSSI : 0x{1,2:X2}({2:d}) BATT : 0x{3,2:X2} ", tData[2 + j + 0], tData[2 + j + 1], tData[2 + j + 1], batt));
                        richTextBox1.AppendText("\r\n");

                        if ((tData[2 + j + 9] & 0x7c) == 0x10)
                        {
                            richTextBox1.AppendText(String.Format("温度 : {0:#0.0}[℃]\r\n", (tData[2 + j + 18] + (UInt16)tData[2 + j + 19] * 256 - 1000.0) / 10.0));
                            richTextBox1.AppendText(String.Format("湿度 : {0:#0.0}[％]\r\n", (tData[2 + j + 28] + (UInt16)tData[2 + j + 29] * 256 - 1000.0) / 10.0));
                        }
                        else if ((tData[2 + j + 9] & 0x7c) == 0x00)
                        {
                            richTextBox1.AppendText(String.Format("温度 : {0:#0.0}[℃]\r\n", (tData[2 + j + 28] + (UInt16)tData[2 + j + 29] * 256 - 1000.0) / 10.0));
                        }
                        else if ((tData[2 + j + 9] & 0x7c) == 0x18)
                        {
                            richTextBox1.AppendText(String.Format("温度 : {0:#0.0}[℃]\r\n", (tData[2 + j + 18] + (UInt16)tData[2 + j + 19] * 256 - 1000.0) / 10.0));
                            richTextBox1.AppendText(String.Format("湿度 : {0:#0.0}[％]\r\n", (tData[2 + j + 28] + (UInt16)tData[2 + j + 29] * 256 - 1000.0) / 10.0));
                        }

                        Application.DoEvents();
                    }

                    richTextBox1.AppendText("-------------------------------\r\n");
                    break;
                }

                i = status_find();
                status = Encoding.ASCII.GetString(rData, i + 9, rData[i + 7] + (UInt16)rData[i + 8] * 256);
                if ((status.Substring(0, 4) == "0004") && (status.Substring(5, 4) == "0000")) break;
            }

        error_proc: ;
            richTextBox1.AppendText("終了\r\n");
            serialPort1.Close();

            dataGridView1.CurrentCell = null;

            // 操作禁止解除
            operation_lift_ban();
        }

















    }
}
