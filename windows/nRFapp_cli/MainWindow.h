#pragma once

//TTOO #include "Form_Maikon.h"
//using namespace TDnRF;
//TTOO #include "serial_nrf_transportCLI.h"

#include "nRF_TD_Controller.h" //#include "nRF_TD_Controller.h"

using namespace TDnRF;

//TTOO #include "../maikon_sim/zapp_test.h"

namespace nRFapp_cli {

    using namespace System;
    using namespace System::ComponentModel;
    using namespace System::Collections;
    using namespace System::Windows::Forms;
    using namespace System::Data;
    using namespace System::Drawing;

    /// <summary>
    /// Summary for MainWindow_CLI
    /// </summary>
    public ref class MainWindow_CLI : public System::Windows::Forms::Form
    {

        nRF_TD_Kontroller^ controller; //nRF_TD_Controller^ controller;

        bool isControllerInitialized; // = false;
        bool isControllerConnected; // = false;

        String^ strConnect; // = "Connect";
        String^ strScanning; // = "Stop scanning";
        String^ strDisconnect; // = "Disconnect";
        String^ strStopSendData; // = "Stop sending data";
        String^ strStartSendData; // = "Send 100kB data";

        UInt32 logHighWatermark; // = 10000;  // If we reach high watermark, we delete until we're
    private: System::Windows::Forms::Button^  btnSendXX4;
    private: System::Windows::Forms::Button^  btnSendXX5;
    private: System::Windows::Forms::Button^  btnSendXX6;
    private: System::Windows::Forms::Button^  btnSendXX1;
    private: System::Windows::Forms::Button^  btnMaikon;
    private: System::Windows::Forms::Button^  btnNormalStart;
    private: System::Windows::Forms::Button^  btn_CMD13;
    private: System::Windows::Forms::Button^  btn01_58;
    private: System::Windows::Forms::Button^  btn01_F8;
    private: System::Windows::Forms::Button^  btn01_F9;
    private: System::Windows::Forms::Button^  btnT2_Test;
    private: System::Windows::Forms::Button^  btn01_44;
    private: System::Windows::Forms::Button^  btn01_45;
    private: System::Windows::Forms::TextBox^  tbBlockNum;
    private: System::Windows::Forms::Button^  btn01_F5;
    private: System::Windows::Forms::Button^  btnReadTest;
    private: System::Windows::Forms::Button^  btnWriteTest;
    private: System::Windows::Forms::Button^  btnSim_Start;
    private: System::Windows::Forms::Button^  btnSim_1;
    private: System::Windows::Forms::Button^  btnSim_2;
    private: System::Windows::Forms::Button^  btnSim_3;
    private: System::Windows::Forms::Button^  btnSim_Stop;
    private: System::Windows::Forms::Button^  btnT2_Test1;
    private: System::Windows::Forms::Button^  btn_CMD12;

             // down to low watermark
             UInt32 logLowWatermark; // = 5000;
             /*
             private ObservableCollection<String> _outputText = null;
             public ObservableCollection<string> OutputText
             {
             get { return _outputText ?? (_outputText = new ObservableCollection<string>()); }
             set { _outputText = value; }
             }
             */

             void InitVars()
             {            
                 isControllerInitialized = false;
                 isControllerConnected = false;

                 strConnect = "Connect";
                 strScanning = "Stop scanning";
                 strDisconnect = "Disconnect";
                 strStopSendData = "Stop sending data";
                 strStartSendData = "Send 100kB data";

                 logHighWatermark = 10000;  // If we reach high watermark, we delete until we're
                 // down to low watermark
                 logLowWatermark = 5000;
             }

    public:
        MainWindow_CLI(void)
        {
            InitVars();

            InitializeComponent();

            //TTOO formMaikon = nullptr;
            //TTOO sntCLI = nullptr;

            // karel: Commented for now - Initialize_nRF_TD_Controller();
            // karel: Moved to manual start, so as to enable testing

            /* Retrieve persisted setting. */
            //cbDebug.IsChecked = Properties.Settings.Default.IsDebugEnabled;
            //DataContext = this;
        }

    protected:
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        ~MainWindow_CLI()
        {
            if (components)
            {
                delete components;
            }
        }

        void Initialize_nRF_TD_Controller()
        {
            controller = gcnew nRF_TD_Kontroller(); //controller = gcnew nRF_TD_Controller();

            /* Registering event handler methods for all nRF_TD_Controller events. */


            //REF controller->LogMessage += gcnew  EventHandler<OutputReceivedEventArgs^>(this, &nRFUart::Basics_Form::OnLogMessage);

            controller->LogMessage += gcnew  EventHandler<OutputReceivedEventArgs^>(this, &MainWindow_CLI::OnLogMessage);
            controller->Initialized += gcnew  EventHandler<EventArgs^>(this, &MainWindow_CLI::OnControllerInitialized );
            controller->Scanning += gcnew  EventHandler<EventArgs^>(this, &MainWindow_CLI::OnScanning );
            controller->ScanningCanceled += gcnew  EventHandler<EventArgs^>(this, &MainWindow_CLI::OnScanningCanceled );
            controller->Connecting += gcnew  EventHandler<EventArgs^>(this, &MainWindow_CLI::OnConnecting );
            controller->ConnectionCanceled += gcnew  EventHandler<EventArgs^>(this, &MainWindow_CLI::OnConnectionCanceled );
            controller->Connected += gcnew  EventHandler<EventArgs^>(this, &MainWindow_CLI::OnConnected );
            controller->PipeDiscoveryCompleted += gcnew  EventHandler<EventArgs^>(this, &MainWindow_CLI:: OnControllerPipeDiscoveryCompleted );
            controller->Disconnected += gcnew  EventHandler<EventArgs^>(this, &MainWindow_CLI::OnDisconnected );
            controller->SendDataStarted += gcnew  EventHandler<EventArgs^>(this, &MainWindow_CLI::OnSendDataStarted );
            controller->SendDataCompleted += gcnew  EventHandler<EventArgs^>(this, &MainWindow_CLI::OnSendDataCompleted );
            controller->ProgressUpdated += gcnew  EventHandler<Nordicsemi::ValueEventArgs<int>^>(this, &MainWindow_CLI::OnProgressUpdated );
            /*
            controller->LogMessage += OnLogMessage;
            controller->Initialized += OnControllerInitialized;
            controller->Scanning += OnScanning;
            controller->ScanningCanceled += OnScanningCanceled;
            controller->Connecting += OnConnecting;
            controller->ConnectionCanceled += OnConnectionCanceled;
            controller->Connected += OnConnected;
            controller->PipeDiscoveryCompleted += OnControllerPipeDiscoveryCompleted;
            controller->Disconnected += OnDisconnected;
            controller->SendDataStarted += OnSendDataStarted;
            controller->SendDataCompleted += OnSendDataCompleted;
            controller->ProgressUpdated += OnProgressUpdated;
            */
            controller->Initialize();
        }


        void SetConnectButtonText(String^ text)
        {
            SetButtonText(btnConnect, text);
        }


#if 0  
        void SetButtonText(Button^ button, String^ text)
        {
            /* Requesting GUI update to be done in main thread since this 
            * method will be called from a different thread. */
            Dispatcher.BeginInvoke((Action)delegate()
            {
                button.Content = text;
            });
        }
#else
        delegate Void SetButtonTextCallback(Button^ button, String^ text);
        void SetButtonText(Button^ button, String^ text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            ISynchronizeInvoke^ i = this;

            // If InvokeRequired returns True, the code
            // is executing on a worker thread.
            if (i->InvokeRequired)
            {
                //C# == SetTextCallback d = new SetTextCallback(AddToOutput);
                // ref http://www.functionx.com/cppcli/classes/Lesson15c.htm
                // at this line  "Addition ^ add = gcnew Addition(oper, &CMathOperations::Plus);"

                // Create a delegate to perform the thread switch.
                SetButtonTextCallback^ tempDelegate = gcnew SetButtonTextCallback(this, &MainWindow_CLI::SetButtonText);

                cli::array<System::Object^>^ args = gcnew cli::array<System::Object^>(2);
                args[0] = button;
                args[1] = text;

                /*
                cli::array<System::Object^>^ args = gcnew cli::array<System::Object^>(1);
                args[0] = text;
                */
                // Marshal the data from the worker thread
                // to the UI thread.
                i->BeginInvoke(tempDelegate, args);

                return;
            }
            button->Text = text;
        };
#endif

        void SetStartSendIsEnabled(bool isEnabled)
        {
            //SetButtonIsEnabled(btnStartSend, isEnabled);
            //SetButtonIsEnabled(btnUARTStartSend100K, isEnabled);
        }

        void SetStartSendFileIsEnabled(bool isEnabled)
        {
            //SetButtonIsEnabled(btnUARTStartSendFile, isEnabled);
        }

        void SetStopDataIsEnabled(bool isEnabled)
        {
            //SetButtonIsEnabled(btnUARTStopData, isEnabled);
        }
#if 0
        void SetButtonIsEnabled(Button^ button, bool isEnabled)
        {
            /* Requesting GUI update to be done in main thread since this 
            * method will be called from a different thread. */
            Dispatcher.BeginInvoke((Action)delegate()
            {
                button.IsEnabled = isEnabled;
            });
        }
#else
        delegate Void SetButtonIsEnabledCallback(Button^ button, bool isEnabled);
        void SetButtonIsEnabled(Button^ button, bool isEnabled)
        {
            ISynchronizeInvoke^ i = this;
            if (i->InvokeRequired)
            {
                SetButtonIsEnabledCallback^ tempDelegate = gcnew SetButtonIsEnabledCallback(this, &MainWindow_CLI::SetButtonIsEnabled);
                cli::array<System::Object^>^ args = gcnew cli::array<System::Object^>(2);
                args[0] = button;
                args[1] = isEnabled;
                i->BeginInvoke(tempDelegate, args);

                return;
            }
            button->Enabled = isEnabled;
        };
#endif

#if 0
        void SetProgressBarValue(int newValue)
        {
            /* Requesting GUI update to be done in main thread since this 
            * method will be called from a different thread. */
            Dispatcher.BeginInvoke((Action)delegate()
            {
                progressBar.Value = newValue;
            });
        }
#else
        delegate Void SetProgressBarValueCallback(int newValue);
        void SetProgressBarValue(int newValue)
        {
            ISynchronizeInvoke^ i = this;
            if (i->InvokeRequired)
            {
                SetProgressBarValueCallback^ tempDelegate = gcnew SetProgressBarValueCallback(this, &MainWindow_CLI::SetProgressBarValue);
                cli::array<System::Object^>^ args = gcnew cli::array<System::Object^>(1);
                args[0] = newValue;
                //args[1] = isEnabled;
                i->BeginInvoke(tempDelegate, args);

                return;
            }
            progressBar->Value = newValue;
        };
#endif

#if 0
        void AddToOutput(String^ text)
        {
            /*TODO
            // Need to call Invoke since method will be called from a background thread.
            Dispatcher.BeginInvoke((Action)delegate()
            {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.ffff");
            text = String.Format("[{0}] {1}", timestamp, text);

            if (OutputText.Count >= logHighWatermark)
            {
            UInt32 numToDelete = (UInt32)OutputText.Count - logLowWatermark;
            for (UInt32 i = 0; i < numToDelete; i++)
            {
            OutputText.RemoveAt(0);
            }
            }

            OutputText.Add(text);
            lbOutput.ScrollIntoView(text);
            });
            TODO*/
        }
#else
        delegate Void AddToOutputCallback(String^ text);
        void AddToOutput(String^ text)
        {            
            // return; //BOGUS

            ISynchronizeInvoke^ i = this;
            if (i->InvokeRequired)
            {
                AddToOutputCallback^ tempDelegate = gcnew AddToOutputCallback(this, &MainWindow_CLI::AddToOutput);
                cli::array<System::Object^>^ args = gcnew cli::array<System::Object^>(1);
                args[0] = text;
                //args[1] = isEnabled;
                i->BeginInvoke(tempDelegate, args);

                return;
            }

            String^ timestamp = DateTime::Now.ToString("HH:mm:ss.ffff");
            text = String::Format("[{0}] {1}", timestamp, text);
#if 0
            if (OutputText->Count >= logHighWatermark)
            {
                UInt32 numToDelete = (UInt32)OutputText.Count - logLowWatermark;
                for (UInt32 i = 0; i < numToDelete; i++)
                {
                    OutputText.RemoveAt(0);
                }
            }

            OutputText.Add(text);
            lbOutput.ScrollIntoView(text);
#else
            richTextBox->AppendText(text + "\r");
            richTextBox->SelectionStart = richTextBox->TextLength; //Set the current caret position at the end
            richTextBox->ScrollToCaret(); //Now scroll it automatically

#endif
        }
#endif


    private: System::Windows::Forms::Button^  btnNotifyOFF;
    protected: 
    private: System::Windows::Forms::Button^  btnNotifyON;
    private: System::Windows::Forms::ProgressBar^  progressBar;











    private: System::Windows::Forms::RichTextBox^  richTextBox;
    private: System::Windows::Forms::CheckBox^  cbDebug;
    private: System::Windows::Forms::Label^  label1;
    private: System::Windows::Forms::Button^  btnConnect;
private: System::ComponentModel::IContainer^  components;
    private:
        /// <summary>
        /// Required designer variable.
        /// </summary>


#pragma region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent(void)
        {
            this->btnNotifyOFF = (gcnew System::Windows::Forms::Button());
            this->btnNotifyON = (gcnew System::Windows::Forms::Button());
            this->progressBar = (gcnew System::Windows::Forms::ProgressBar());
            this->richTextBox = (gcnew System::Windows::Forms::RichTextBox());
            this->cbDebug = (gcnew System::Windows::Forms::CheckBox());
            this->label1 = (gcnew System::Windows::Forms::Label());
            this->btnConnect = (gcnew System::Windows::Forms::Button());
            this->btnSendXX4 = (gcnew System::Windows::Forms::Button());
            this->btnSendXX5 = (gcnew System::Windows::Forms::Button());
            this->btnSendXX6 = (gcnew System::Windows::Forms::Button());
            this->btnSendXX1 = (gcnew System::Windows::Forms::Button());
            this->btnMaikon = (gcnew System::Windows::Forms::Button());
            this->btnNormalStart = (gcnew System::Windows::Forms::Button());
            this->btn_CMD13 = (gcnew System::Windows::Forms::Button());
            this->btn01_58 = (gcnew System::Windows::Forms::Button());
            this->btn01_F8 = (gcnew System::Windows::Forms::Button());
            this->btn01_F9 = (gcnew System::Windows::Forms::Button());
            this->btnT2_Test = (gcnew System::Windows::Forms::Button());
            this->btn01_44 = (gcnew System::Windows::Forms::Button());
            this->btn01_45 = (gcnew System::Windows::Forms::Button());
            this->tbBlockNum = (gcnew System::Windows::Forms::TextBox());
            this->btn01_F5 = (gcnew System::Windows::Forms::Button());
            this->btnReadTest = (gcnew System::Windows::Forms::Button());
            this->btnWriteTest = (gcnew System::Windows::Forms::Button());
            this->btnSim_Start = (gcnew System::Windows::Forms::Button());
            this->btnSim_1 = (gcnew System::Windows::Forms::Button());
            this->btnSim_2 = (gcnew System::Windows::Forms::Button());
            this->btnSim_3 = (gcnew System::Windows::Forms::Button());
            this->btnSim_Stop = (gcnew System::Windows::Forms::Button());
            this->btnT2_Test1 = (gcnew System::Windows::Forms::Button());
            this->btn_CMD12 = (gcnew System::Windows::Forms::Button());
            this->SuspendLayout();
            // 
            // btnNotifyOFF
            // 
            this->btnNotifyOFF->Location = System::Drawing::Point(208, 12);
            this->btnNotifyOFF->Name = L"btnNotifyOFF";
            this->btnNotifyOFF->Size = System::Drawing::Size(72, 23);
            this->btnNotifyOFF->TabIndex = 25;
            this->btnNotifyOFF->Text = L"Notify OFF";
            this->btnNotifyOFF->UseVisualStyleBackColor = true;
            this->btnNotifyOFF->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btnNotifyOFF_Click);
            // 
            // btnNotifyON
            // 
            this->btnNotifyON->Location = System::Drawing::Point(130, 12);
            this->btnNotifyON->Name = L"btnNotifyON";
            this->btnNotifyON->Size = System::Drawing::Size(72, 23);
            this->btnNotifyON->TabIndex = 24;
            this->btnNotifyON->Text = L"Notify ON";
            this->btnNotifyON->UseVisualStyleBackColor = true;
            this->btnNotifyON->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btnNotifyON_Click);
            // 
            // progressBar
            // 
            this->progressBar->Location = System::Drawing::Point(317, 282);
            this->progressBar->Name = L"progressBar";
            this->progressBar->Size = System::Drawing::Size(261, 19);
            this->progressBar->TabIndex = 23;
            // 
            // richTextBox
            // 
            this->richTextBox->Location = System::Drawing::Point(12, 307);
            this->richTextBox->Name = L"richTextBox";
            this->richTextBox->Size = System::Drawing::Size(566, 270);
            this->richTextBox->TabIndex = 16;
            this->richTextBox->Text = L"";
            // 
            // cbDebug
            // 
            this->cbDebug->AutoSize = true;
            this->cbDebug->Location = System::Drawing::Point(467, 37);
            this->cbDebug->Name = L"cbDebug";
            this->cbDebug->Size = System::Drawing::Size(56, 16);
            this->cbDebug->TabIndex = 15;
            this->cbDebug->Text = L"Debug";
            this->cbDebug->UseVisualStyleBackColor = true;
            this->cbDebug->CheckedChanged += gcnew System::EventHandler(this, &MainWindow_CLI::cbDebug_CheckedChanged);
            // 
            // label1
            // 
            this->label1->AutoSize = true;
            this->label1->Location = System::Drawing::Point(12, 38);
            this->label1->Name = L"label1";
            this->label1->Size = System::Drawing::Size(58, 12);
            this->label1->TabIndex = 14;
            this->label1->Text = L"Console ....";
            // 
            // btnConnect
            // 
            this->btnConnect->Location = System::Drawing::Point(12, 12);
            this->btnConnect->Name = L"btnConnect";
            this->btnConnect->Size = System::Drawing::Size(100, 23);
            this->btnConnect->TabIndex = 13;
            this->btnConnect->Text = L"Connect";
            this->btnConnect->UseVisualStyleBackColor = true;
            this->btnConnect->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btnConnect_Click);
            // 
            // btnSendXX4
            // 
            this->btnSendXX4->Location = System::Drawing::Point(541, 38);
            this->btnSendXX4->Name = L"btnSendXX4";
            this->btnSendXX4->Size = System::Drawing::Size(38, 19);
            this->btnSendXX4->TabIndex = 26;
            this->btnSendXX4->Text = L"S4";
            this->btnSendXX4->UseVisualStyleBackColor = true;
            this->btnSendXX4->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btnSendXX4_Click);
            // 
            // btnSendXX5
            // 
            this->btnSendXX5->Location = System::Drawing::Point(541, 63);
            this->btnSendXX5->Name = L"btnSendXX5";
            this->btnSendXX5->Size = System::Drawing::Size(38, 19);
            this->btnSendXX5->TabIndex = 27;
            this->btnSendXX5->Text = L"S5";
            this->btnSendXX5->UseVisualStyleBackColor = true;
            this->btnSendXX5->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btnSendXX5_Click);
            // 
            // btnSendXX6
            // 
            this->btnSendXX6->Location = System::Drawing::Point(541, 88);
            this->btnSendXX6->Name = L"btnSendXX6";
            this->btnSendXX6->Size = System::Drawing::Size(38, 19);
            this->btnSendXX6->TabIndex = 28;
            this->btnSendXX6->Text = L"S6";
            this->btnSendXX6->UseVisualStyleBackColor = true;
            this->btnSendXX6->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btnSendXX6_Click);
            // 
            // btnSendXX1
            // 
            this->btnSendXX1->Location = System::Drawing::Point(541, 113);
            this->btnSendXX1->Name = L"btnSendXX1";
            this->btnSendXX1->Size = System::Drawing::Size(38, 19);
            this->btnSendXX1->TabIndex = 29;
            this->btnSendXX1->Text = L"S1";
            this->btnSendXX1->UseVisualStyleBackColor = true;
            this->btnSendXX1->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btnSendXX1_Click);
            // 
            // btnMaikon
            // 
            this->btnMaikon->Location = System::Drawing::Point(527, 12);
            this->btnMaikon->Name = L"btnMaikon";
            this->btnMaikon->Size = System::Drawing::Size(52, 19);
            this->btnMaikon->TabIndex = 30;
            this->btnMaikon->Text = L"Maikon";
            this->btnMaikon->UseVisualStyleBackColor = true;
            this->btnMaikon->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btnMaikon_Click);
            // 
            // btnNormalStart
            // 
            this->btnNormalStart->Location = System::Drawing::Point(467, 12);
            this->btnNormalStart->Name = L"btnNormalStart";
            this->btnNormalStart->Size = System::Drawing::Size(52, 19);
            this->btnNormalStart->TabIndex = 31;
            this->btnNormalStart->Text = L"Normal";
            this->btnNormalStart->UseVisualStyleBackColor = true;
            this->btnNormalStart->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btnNormalStart_Click);
            // 
            // btn_CMD13
            // 
            this->btn_CMD13->Location = System::Drawing::Point(386, 14);
            this->btn_CMD13->Name = L"btn_CMD13";
            this->btn_CMD13->Size = System::Drawing::Size(65, 19);
            this->btn_CMD13->TabIndex = 32;
            this->btn_CMD13->Text = L"CMD_13";
            this->btn_CMD13->UseVisualStyleBackColor = true;
            this->btn_CMD13->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btn_CMD13_Click);
            // 
            // btn01_58
            // 
            this->btn01_58->Location = System::Drawing::Point(130, 51);
            this->btn01_58->Name = L"btn01_58";
            this->btn01_58->Size = System::Drawing::Size(55, 19);
            this->btn01_58->TabIndex = 33;
            this->btn01_58->Text = L"01 - 58";
            this->btn01_58->UseVisualStyleBackColor = true;
            this->btn01_58->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btn01_58_Click);
            // 
            // btn01_F8
            // 
            this->btn01_F8->Location = System::Drawing::Point(130, 101);
            this->btn01_F8->Name = L"btn01_F8";
            this->btn01_F8->Size = System::Drawing::Size(55, 19);
            this->btn01_F8->TabIndex = 34;
            this->btn01_F8->Text = L"01 - F8";
            this->btn01_F8->UseVisualStyleBackColor = true;
            this->btn01_F8->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btn01_F8_Click);
            // 
            // btn01_F9
            // 
            this->btn01_F9->Location = System::Drawing::Point(191, 101);
            this->btn01_F9->Name = L"btn01_F9";
            this->btn01_F9->Size = System::Drawing::Size(55, 19);
            this->btn01_F9->TabIndex = 35;
            this->btn01_F9->Text = L"01 - F9";
            this->btn01_F9->UseVisualStyleBackColor = true;
            this->btn01_F9->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btn01_F9_Click);
            // 
            // btnT2_Test
            // 
            this->btnT2_Test->Location = System::Drawing::Point(306, 51);
            this->btnT2_Test->Name = L"btnT2_Test";
            this->btnT2_Test->Size = System::Drawing::Size(55, 19);
            this->btnT2_Test->TabIndex = 36;
            this->btnT2_Test->Text = L"T2 Test";
            this->btnT2_Test->UseVisualStyleBackColor = true;
            this->btnT2_Test->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btnT2_Test_Click);
            // 
            // btn01_44
            // 
            this->btn01_44->Location = System::Drawing::Point(130, 131);
            this->btn01_44->Name = L"btn01_44";
            this->btn01_44->Size = System::Drawing::Size(55, 19);
            this->btn01_44->TabIndex = 37;
            this->btn01_44->Text = L"01 - 44";
            this->btn01_44->UseVisualStyleBackColor = true;
            this->btn01_44->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btn01_44_Click);
            // 
            // btn01_45
            // 
            this->btn01_45->Location = System::Drawing::Point(130, 156);
            this->btn01_45->Name = L"btn01_45";
            this->btn01_45->Size = System::Drawing::Size(55, 19);
            this->btn01_45->TabIndex = 38;
            this->btn01_45->Text = L"01 - 45";
            this->btn01_45->UseVisualStyleBackColor = true;
            this->btn01_45->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btn01_45_Click);
            // 
            // tbBlockNum
            // 
            this->tbBlockNum->Location = System::Drawing::Point(45, 156);
            this->tbBlockNum->Name = L"tbBlockNum";
            this->tbBlockNum->Size = System::Drawing::Size(79, 19);
            this->tbBlockNum->TabIndex = 39;
            // 
            // btn01_F5
            // 
            this->btn01_F5->Location = System::Drawing::Point(130, 76);
            this->btn01_F5->Name = L"btn01_F5";
            this->btn01_F5->Size = System::Drawing::Size(55, 19);
            this->btn01_F5->TabIndex = 40;
            this->btn01_F5->Text = L"01 - F5";
            this->btn01_F5->UseVisualStyleBackColor = true;
            this->btn01_F5->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btn01_F5_Click);
            // 
            // btnReadTest
            // 
            this->btnReadTest->Location = System::Drawing::Point(271, 171);
            this->btnReadTest->Name = L"btnReadTest";
            this->btnReadTest->Size = System::Drawing::Size(100, 19);
            this->btnReadTest->TabIndex = 41;
            this->btnReadTest->Text = L"Read Test";
            this->btnReadTest->UseVisualStyleBackColor = true;
            this->btnReadTest->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btnReadTest_Click);
            // 
            // btnWriteTest
            // 
            this->btnWriteTest->Location = System::Drawing::Point(271, 146);
            this->btnWriteTest->Name = L"btnWriteTest";
            this->btnWriteTest->Size = System::Drawing::Size(100, 19);
            this->btnWriteTest->TabIndex = 42;
            this->btnWriteTest->Text = L"Write Test";
            this->btnWriteTest->UseVisualStyleBackColor = true;
            this->btnWriteTest->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btnWriteTest_Click);
            // 
            // btnSim_Start
            // 
            this->btnSim_Start->Location = System::Drawing::Point(419, 146);
            this->btnSim_Start->Name = L"btnSim_Start";
            this->btnSim_Start->Size = System::Drawing::Size(52, 19);
            this->btnSim_Start->TabIndex = 43;
            this->btnSim_Start->Text = L"Sim S";
            this->btnSim_Start->UseVisualStyleBackColor = true;
            this->btnSim_Start->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btnSim_Start_Click);
            // 
            // btnSim_1
            // 
            this->btnSim_1->Location = System::Drawing::Point(477, 146);
            this->btnSim_1->Name = L"btnSim_1";
            this->btnSim_1->Size = System::Drawing::Size(52, 19);
            this->btnSim_1->TabIndex = 44;
            this->btnSim_1->Text = L"Sim 1";
            this->btnSim_1->UseVisualStyleBackColor = true;
            this->btnSim_1->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btnSim_1_Click);
            // 
            // btnSim_2
            // 
            this->btnSim_2->Location = System::Drawing::Point(477, 171);
            this->btnSim_2->Name = L"btnSim_2";
            this->btnSim_2->Size = System::Drawing::Size(52, 19);
            this->btnSim_2->TabIndex = 45;
            this->btnSim_2->Text = L"Sim 2";
            this->btnSim_2->UseVisualStyleBackColor = true;
            this->btnSim_2->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btnSim_2_Click);
            // 
            // btnSim_3
            // 
            this->btnSim_3->Location = System::Drawing::Point(477, 196);
            this->btnSim_3->Name = L"btnSim_3";
            this->btnSim_3->Size = System::Drawing::Size(52, 19);
            this->btnSim_3->TabIndex = 46;
            this->btnSim_3->Text = L"Sim 3";
            this->btnSim_3->UseVisualStyleBackColor = true;
            this->btnSim_3->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btnSim_3_Click);
            // 
            // btnSim_Stop
            // 
            this->btnSim_Stop->Location = System::Drawing::Point(419, 171);
            this->btnSim_Stop->Name = L"btnSim_Stop";
            this->btnSim_Stop->Size = System::Drawing::Size(52, 19);
            this->btnSim_Stop->TabIndex = 47;
            this->btnSim_Stop->Text = L"Sim X";
            this->btnSim_Stop->UseVisualStyleBackColor = true;
            this->btnSim_Stop->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btnSim_Stop_Click);
            // 
            // btnT2_Test1
            // 
            this->btnT2_Test1->Location = System::Drawing::Point(306, 76);
            this->btnT2_Test1->Name = L"btnT2_Test1";
            this->btnT2_Test1->Size = System::Drawing::Size(100, 19);
            this->btnT2_Test1->TabIndex = 48;
            this->btnT2_Test1->Text = L"T2_Test1";
            this->btnT2_Test1->UseVisualStyleBackColor = true;
            this->btnT2_Test1->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btnT2_Test1_Click);
            // 
            // btn_CMD12
            // 
            this->btn_CMD12->Location = System::Drawing::Point(306, 14);
            this->btn_CMD12->Name = L"btn_CMD12";
            this->btn_CMD12->Size = System::Drawing::Size(65, 19);
            this->btn_CMD12->TabIndex = 49;
            this->btn_CMD12->Text = L"CMD_12";
            this->btn_CMD12->UseVisualStyleBackColor = true;
            this->btn_CMD12->Click += gcnew System::EventHandler(this, &MainWindow_CLI::btn_CMD12_Click);
            // 
            // MainWindow_CLI
            // 
            this->AutoScaleDimensions = System::Drawing::SizeF(6, 12);
            this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
            this->ClientSize = System::Drawing::Size(590, 589);
            this->Controls->Add(this->btn_CMD12);
            this->Controls->Add(this->btnT2_Test1);
            this->Controls->Add(this->btnSim_Stop);
            this->Controls->Add(this->btnSim_3);
            this->Controls->Add(this->btnSim_2);
            this->Controls->Add(this->btnSim_1);
            this->Controls->Add(this->btnSim_Start);
            this->Controls->Add(this->btnWriteTest);
            this->Controls->Add(this->btnReadTest);
            this->Controls->Add(this->btn01_F5);
            this->Controls->Add(this->tbBlockNum);
            this->Controls->Add(this->btn01_45);
            this->Controls->Add(this->btn01_44);
            this->Controls->Add(this->btnT2_Test);
            this->Controls->Add(this->btn01_F9);
            this->Controls->Add(this->btn01_F8);
            this->Controls->Add(this->btn01_58);
            this->Controls->Add(this->btn_CMD13);
            this->Controls->Add(this->btnNormalStart);
            this->Controls->Add(this->btnMaikon);
            this->Controls->Add(this->btnSendXX1);
            this->Controls->Add(this->btnSendXX6);
            this->Controls->Add(this->btnSendXX5);
            this->Controls->Add(this->btnSendXX4);
            this->Controls->Add(this->btnNotifyOFF);
            this->Controls->Add(this->btnNotifyON);
            this->Controls->Add(this->progressBar);
            this->Controls->Add(this->richTextBox);
            this->Controls->Add(this->cbDebug);
            this->Controls->Add(this->label1);
            this->Controls->Add(this->btnConnect);
            this->Name = L"MainWindow_CLI";
            this->Text = L"MainWindow_CLI";
            this->ResumeLayout(false);
            this->PerformLayout();

        }
#pragma endregion


#pragma region nRFUart event handlers
        void OnControllerInitialized(System::Object^ sender, EventArgs^ e)
        {
            isControllerInitialized = true;
            /*TODO
            Dispatcher.BeginInvoke((Action)delegate()
            {
            btnConnect.IsEnabled = true;
            Mouse.OverrideCursor = null;
            });
            */
            AddToOutput("Ready to connect");
        }

        void OnLogMessage(System::Object^ sender, OutputReceivedEventArgs^ e)
        {
            AddToOutput(e->Message);
        }

        void OnScanning(System::Object^ sender, EventArgs^ e)
        {
            AddToOutput("Scanning...");
            SetConnectButtonText(strScanning);
        }

        void OnScanningCanceled(System::Object^ sender, EventArgs^ e)
        {
            AddToOutput("Stopped scanning");
            SetConnectButtonText(strConnect);
        }

        void OnConnectionCanceled(System::Object^ sender, EventArgs^ e)
        {
            SetConnectButtonText(strConnect);
        }

        void OnConnecting(System::Object^ sender, EventArgs^ e)
        {
            AddToOutput("Connecting...");
        }

        void OnConnected(System::Object^ sender, EventArgs^ e)
        {
            isControllerConnected = true;
            SetConnectButtonText(strDisconnect);
        }

        void OnControllerPipeDiscoveryCompleted(System::Object^ sender, EventArgs^ e)
        {
            AddToOutput("Ready to send");
        }

        void OnSendDataStarted(System::Object^ sender, EventArgs^ e)
        {
            AddToOutput("Started sending data...");
            SetStopDataIsEnabled(true);
            SetStartSendIsEnabled(false);
            SetStartSendFileIsEnabled(false);
        }

        void OnSendDataCompleted(System::Object^ sender, EventArgs^ e)
        {
            AddToOutput("Data transfer ended");
            SetStopDataIsEnabled(false);
            SetStartSendIsEnabled(true);
            SetStartSendFileIsEnabled(true);
            SetProgressBarValue(0);
        }

        void OnDisconnected(System::Object^ sender, EventArgs^ e)
        {
            isControllerConnected = false;
            AddToOutput("Disconnected");
            SetConnectButtonText(strConnect);
            SetStopDataIsEnabled(false);
            SetStartSendIsEnabled(true);
            SetStartSendFileIsEnabled(true);
        }

        void OnProgressUpdated(System::Object^ sender, Nordicsemi::ValueEventArgs<int>^ e)
        {
            int progress = e->Value;
            if (0 <= progress && progress <= 100)
            {
                SetProgressBarValue(progress);
            }
        }
#pragma endregion

//TTOO        Form_Maikon^ formMaikon;
//TTOO        serial_nrf_transportCLI^ sntCLI;// = nullptr;



    private:
        System::Void btnNormalStart_Click(System::Object^  sender, System::EventArgs^  e)
        {
            Initialize_nRF_TD_Controller(); // Moved here for testing
        }

    private: 
        System::Void btnMaikon_Click(System::Object^  sender, System::EventArgs^  e)
        {
            /*TTOO
            if( formMaikon == nullptr )
            {
                formMaikon = gcnew Form_Maikon();
            }

            if( formMaikon->IsDisposed == true )
            {
                formMaikon = gcnew Form_Maikon();
            }

            if( formMaikon->Visible == false )
            {
                formMaikon->Show();
            }
            else
            {
                formMaikon->Hide();
            }

            if( formMaikon->maikon->serial_IsOpen() != true )
            {
                formMaikon->maikon->serial_Open();
            }
            TTOO*/
        }

        System::Void btnConnect_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerInitialized)
            {
                return;
            }

            if (btnConnect->Text == strConnect)
            {
                controller->InitiateConnection();
            }
            else if (btnConnect->Text == strScanning)
            {
                controller->StopScanning();
            }
            else if (btnConnect->Text == strDisconnect)
            {
                controller->InitiateDisconnect();
            }
        }

        System::Void btnNotifyON_Click(System::Object^  sender, System::EventArgs^  e)
        {
            controller->EnableNotify_Dcfm();
        }
        System::Void btnNotifyOFF_Click(System::Object^  sender, System::EventArgs^  e)
        {
        }

        System::Void cbDebug_CheckedChanged(System::Object^  sender, System::EventArgs^  e)
        {
            /* Store the state of the checkbox in application settings. */
            controller->DebugMessagesEnabled = (bool)cbDebug->Checked;
            /*TODO
            Properties.Settings.Default.IsDebugEnabled = (bool)cbDebug->Checked;
            controller->DebugMessagesEnabled = (bool)cbDebug->Checked;
            TODO*/
        }

        //=====================================================================
        //=====================================================================
        //===== UART ==========================================================
        //=====================================================================
        //=====================================================================
        System::Void XXXbtnUARTSend_Click(System::Object^  sender, System::EventArgs^  e) 
        {
            /*defunct
            if (!isControllerConnected)
            {
                return;
            }
            controller->UART_SendData(tbInput->Text);
            */
        }

        System::Void XXXbtnUARTStartSendFile_Click(System::Object^  sender, System::EventArgs^  e) 
        {

            String^ sendFilePath = String::Empty;

            /*TODO
            Microsoft::Win32::OpenFileDialog^ ofd = gcnew Microsoft::Win32::OpenFileDialog();
            ofd.FileName = "File";
            ofd.DefaultExt = "*.*";
            ofd.Filter = "All files (*.*)|*.*";
            ofd.FilterIndex = 0;
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

            ofd.Title = "Please select a file to send";

            bool? ofdResult = ofd.ShowDialog();

            if (ofdResult == false) //Failure
            {
            return;
            }

            UARTSendFile(ofd.FileName);
            */
        }

        void UARTSendFile(String^ filePath)
        {
            if (!System::IO::File::Exists(filePath))
            {
                return;
            }

            filePath = filePath->Replace("\\", "/");

            /*TODO
            byte[] fileContent = File.ReadAllBytes(filePath);
            controller->UARTStartSendData(fileContent);
            TODO*/
        }

        System::Void XXXbtnUARTStartSend100K_Click(System::Object^  sender, System::EventArgs^  e)
        {
            UARTSend100K();
        }
        System::Void XXXbtnUARTStartSend1K_Click(System::Object^  sender, System::EventArgs^  e)
        {
            //UARTSend1K();
        }

        void UARTSend100K()
        {
            /*TODO
            // Instantiate byte array with 18 bytes of data. 
            byte[] data = new byte[] { 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A,
            0x6B, 0x6C, 0x6D, 0x6E, 0x6F, 0x70, 0x71, 0x72};

            // Calculate number of packets required to send 100kB of data. 
            int maxBytesPerPacket = 18;
            int kibiBytes = 1024;
            int numberOfRepetitions = (100 * kibiBytes) / maxBytesPerPacket; // 5120 packets

            controller->UARTStartSendData(data, numberOfRepetitions);
            TODO*/
        }

        System::Void XXXbtnUARTStopData_Click(System::Object^  sender, System::EventArgs^  e)
        {
            AddToOutput("Stop transfer");
            controller->UARTStopSendData();
        }


        //=====================================================================
        //=====================================================================
        //===== CMD ===========================================================
        //=====================================================================
        //=====================================================================
        System::Void btn_CMD12_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;
            L6->Dn_Send_CMD_12(); //controller->udEngine->Dn_Send_CMD_12();
        }

        System::Void btn_CMD13_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;
            L6->Dn_Send_CMD_13(); //controller->udEngine->Dn_Send_CMD_13();
        }


        //=====================================================================
        //=====================================================================
        //===== CMD testing ===================================================
        //=====================================================================
        //=====================================================================
        System::Void btnSendXX1_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;
            L6->Dn_Send_T2_DummyTEST1(); //controller->udEngine->Dn_Send_T2_DummyTEST1();
        }

        System::Void btnSendXX4_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;
            //L6->Dn_Dummy_Send42(); //controller->udEngine->Dn_Dummy_Send42();
            //L6->?? //controller->Send_Dcmd(tbInput->Text);
        }

        System::Void btnSendXX5_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;
            //controller->Send_Ddat(tbInput->Text);
        }

        System::Void btnSendXX6_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;
            //controller->udEngine->Dn_Dummy_Send43();
        }


        //=====================================================================
        //=====================================================================
        //===== CMD T2 ========================================================
        //=====================================================================
        //=====================================================================
        System::Void btnT2_Test_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;
            L6->Dn_Send_T2_DummyTEST1(); //controller->udEngine->Dn_Send_T2_DummyTEST1();
        }

        System::Void btnT2_Test1_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;
            L6->T2_RUINF(); //controller->udEngine->T2_RUINF();
        }

        //=====================================================================
        //=====================================================================
        //===== CMD 0x01 ======================================================
        //=====================================================================
        //=====================================================================
        System::Void btn01_58_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;
            L6->Dn_Send_0x01_CMD_01_0x58(); //controller->udEngine->Dn_Send_0x01_CMD_01_0x58();
        }


        System::Void btn01_F5_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;
            uint16_t recCount;  
            recCount = Convert::ToInt16( tbBlockNum->Text, 10);
            L6->Dn_Send_0x01_CMD_01_0xF5(recCount); //controller->udEngine->Dn_Send_0x01_CMD_01_0xF5(recCount);
        }

        System::Void btn01_F8_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;
            L6->Dn_Send_0x01_CMD_01_0xF8(); //controller->udEngine->Dn_Send_0x01_CMD_01_0xF8();
        }

        System::Void btn01_F9_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;
            L6->Dn_Send_0x01_CMD_01_0xF9(1024); //controller->udEngine->Dn_Send_0x01_CMD_01_0xF9(1024);
        }

        System::Void btn01_44_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;

            //tbBlockNum->Text = "0";
            uint16_t recCount;  
            recCount = Convert::ToInt16( tbBlockNum->Text, 10);
            uint16_t byteCount;  
            byteCount = recCount * 8;
            L6->Dn_Send_0x01_CMD_01_0x44(byteCount);//2048 + 32); //controller->udEngine->Dn_Send_0x01_CMD_01_0x44(byteCount);//2048 + 32);
        }
        System::Void btn01_45_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;

            uint16_t blkNum;  
            blkNum = Convert::ToInt16( tbBlockNum->Text, 10);
            L6->Dn_Send_0x01_CMD_01_0x45(blkNum); //controller->udEngine->Dn_Send_0x01_CMD_01_0x45(blkNum);

        }


        System::Void btnReadTest_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;
            L6->UpDn_Read_Rctrl_test(); //controller->udEngine->UpDn_Read_Rctrl_test();
        }
        System::Void btnWriteTest_Click(System::Object^  sender, System::EventArgs^  e)
        {
            if (!isControllerConnected)
                return;

            array<uint8_t,1>^ buf = gcnew array<uint8_t,1>(20);

            L6->UpDn_Write_Wctrl_test(buf, buf->Length); //controller->udEngine->UpDn_Write_Wctrl_test(buf, buf->Length);
        }

        ///////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////
    private:
        void Go_Task()
        {
            /*TTOO
            try
            {
                zapp_test1_Start();
            }
            catch (Exception^ ex)
            {
                Console::WriteLine("Exception in OnConnected: {0}, {1}", ex->Message, ex->StackTrace);
            }
            TTOO*/
        }

        System::Void btnSim_Start_Click(System::Object^  sender, System::EventArgs^  e)
        {
            /*TTOO
            if( sntCLI == nullptr )
            {
                sntCLI = gcnew serial_nrf_transportCLI(this->components);

                sntCLI->serial_Open();
                sntCLI->Start_task();

                //zapp_test1_Start();
                System::Threading::Tasks::Task^ myTask = System::Threading::Tasks::Task::Factory->StartNew(gcnew Action(this, &MainWindow_CLI::Go_Task));
            }
            TTOO*/

        }
        System::Void btnSim_Stop_Click(System::Object^  sender, System::EventArgs^  e)
        {
            //TTOO zapp_test1_Stop();
        }

        System::Void btnSim_1_Click(System::Object^  sender, System::EventArgs^  e)
        {
            //TTOO zapp_test1_Trig1();
        }
        System::Void btnSim_2_Click(System::Object^  sender, System::EventArgs^  e)
        {
            //TTOO zapp_test1_Trig2();
        }
        System::Void btnSim_3_Click(System::Object^  sender, System::EventArgs^  e)
        {
        }



};
}
