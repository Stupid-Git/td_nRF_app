#pragma once

#include "MaikonCLI.h"

namespace MasterEm_TD2_a {

    using namespace nRFMaikon;
	using namespace System;
	using namespace System::ComponentModel;
	using namespace System::Collections;
	using namespace System::Windows::Forms;
	using namespace System::Data;
	using namespace System::Drawing;

	/// <summary>
	/// Summary for Form_Maikon
	/// </summary>
	public ref class Form_Maikon : public System::Windows::Forms::Form
	{
	public:
        MaikonCLI^ maikon;
	public:
		Form_Maikon(void)
		{
			InitializeComponent();
			//
			//TODO: Add the constructor code here
			//
            maikon = gcnew MaikonCLI(this->components);
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~Form_Maikon()
		{
			if (components)
			{
				delete components;
			}
		}
    private: System::IO::Ports::SerialPort^  serialPort1;
    private: System::Windows::Forms::Button^  btn_Test;
    private: System::Windows::Forms::Button^  btn_Test1;
    private: System::Windows::Forms::Button^  btnTest_getAddress;
    private: System::Windows::Forms::Button^  btnOpen41;
    private: System::Windows::Forms::Button^  btnClose41;
    private: System::Windows::Forms::Button^  btnMainInit;
    private: System::Windows::Forms::Button^  btnMainIntCt;
    private: System::Windows::Forms::Button^  btnMainLoop;
    protected: 
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
            this->components = (gcnew System::ComponentModel::Container());
            this->serialPort1 = (gcnew System::IO::Ports::SerialPort(this->components));
            this->btn_Test = (gcnew System::Windows::Forms::Button());
            this->btn_Test1 = (gcnew System::Windows::Forms::Button());
            this->btnTest_getAddress = (gcnew System::Windows::Forms::Button());
            this->btnOpen41 = (gcnew System::Windows::Forms::Button());
            this->btnClose41 = (gcnew System::Windows::Forms::Button());
            this->btnMainInit = (gcnew System::Windows::Forms::Button());
            this->btnMainIntCt = (gcnew System::Windows::Forms::Button());
            this->btnMainLoop = (gcnew System::Windows::Forms::Button());
            this->SuspendLayout();
            // 
            // serialPort1
            // 
            this->serialPort1->ErrorReceived += gcnew System::IO::Ports::SerialErrorReceivedEventHandler(this, &Form_Maikon::serialPort1_ErrorReceived);
            this->serialPort1->PinChanged += gcnew System::IO::Ports::SerialPinChangedEventHandler(this, &Form_Maikon::serialPort1_PinChanged);
            this->serialPort1->DataReceived += gcnew System::IO::Ports::SerialDataReceivedEventHandler(this, &Form_Maikon::serialPort1_DataReceived);
            // 
            // btn_Test
            // 
            this->btn_Test->Location = System::Drawing::Point(12, 12);
            this->btn_Test->Name = L"btn_Test";
            this->btn_Test->Size = System::Drawing::Size(75, 23);
            this->btn_Test->TabIndex = 0;
            this->btn_Test->Text = L"Test";
            this->btn_Test->UseVisualStyleBackColor = true;
            this->btn_Test->Click += gcnew System::EventHandler(this, &Form_Maikon::btn_Test_Click);
            // 
            // btn_Test1
            // 
            this->btn_Test1->Location = System::Drawing::Point(104, 12);
            this->btn_Test1->Name = L"btn_Test1";
            this->btn_Test1->Size = System::Drawing::Size(75, 23);
            this->btn_Test1->TabIndex = 1;
            this->btn_Test1->Text = L"Test 1";
            this->btn_Test1->UseVisualStyleBackColor = true;
            this->btn_Test1->Click += gcnew System::EventHandler(this, &Form_Maikon::btn_Test1_Click);
            // 
            // btnTest_getAddress
            // 
            this->btnTest_getAddress->Location = System::Drawing::Point(300, 93);
            this->btnTest_getAddress->Name = L"btnTest_getAddress";
            this->btnTest_getAddress->Size = System::Drawing::Size(125, 23);
            this->btnTest_getAddress->TabIndex = 2;
            this->btnTest_getAddress->Text = L"Test Get Addr";
            this->btnTest_getAddress->UseVisualStyleBackColor = true;
            this->btnTest_getAddress->Click += gcnew System::EventHandler(this, &Form_Maikon::btnTest_getAddress_Click);
            // 
            // btnOpen41
            // 
            this->btnOpen41->Location = System::Drawing::Point(300, 12);
            this->btnOpen41->Name = L"btnOpen41";
            this->btnOpen41->Size = System::Drawing::Size(125, 23);
            this->btnOpen41->TabIndex = 3;
            this->btnOpen41->Text = L"Open 41";
            this->btnOpen41->UseVisualStyleBackColor = true;
            this->btnOpen41->Click += gcnew System::EventHandler(this, &Form_Maikon::btnOpen41_Click);
            // 
            // btnClose41
            // 
            this->btnClose41->Location = System::Drawing::Point(300, 41);
            this->btnClose41->Name = L"btnClose41";
            this->btnClose41->Size = System::Drawing::Size(125, 23);
            this->btnClose41->TabIndex = 4;
            this->btnClose41->Text = L"Close 41";
            this->btnClose41->UseVisualStyleBackColor = true;
            this->btnClose41->Click += gcnew System::EventHandler(this, &Form_Maikon::btnClose41_Click);
            // 
            // btnMainInit
            // 
            this->btnMainInit->Location = System::Drawing::Point(12, 141);
            this->btnMainInit->Name = L"btnMainInit";
            this->btnMainInit->Size = System::Drawing::Size(75, 23);
            this->btnMainInit->TabIndex = 5;
            this->btnMainInit->Text = L"Main Init";
            this->btnMainInit->UseVisualStyleBackColor = true;
            this->btnMainInit->Click += gcnew System::EventHandler(this, &Form_Maikon::btnMainInit_Click);
            // 
            // btnMainIntCt
            // 
            this->btnMainIntCt->Location = System::Drawing::Point(12, 170);
            this->btnMainIntCt->Name = L"btnMainIntCt";
            this->btnMainIntCt->Size = System::Drawing::Size(75, 23);
            this->btnMainIntCt->TabIndex = 6;
            this->btnMainIntCt->Text = L"Main IntCt";
            this->btnMainIntCt->UseVisualStyleBackColor = true;
            this->btnMainIntCt->Click += gcnew System::EventHandler(this, &Form_Maikon::btnMainIntCt_Click);
            // 
            // btnMainLoop
            // 
            this->btnMainLoop->Location = System::Drawing::Point(12, 199);
            this->btnMainLoop->Name = L"btnMainLoop";
            this->btnMainLoop->Size = System::Drawing::Size(75, 23);
            this->btnMainLoop->TabIndex = 7;
            this->btnMainLoop->Text = L"Main Loop";
            this->btnMainLoop->UseVisualStyleBackColor = true;
            this->btnMainLoop->Click += gcnew System::EventHandler(this, &Form_Maikon::btnMainLoop_Click);
            // 
            // Form_Maikon
            // 
            this->AutoScaleDimensions = System::Drawing::SizeF(6, 12);
            this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
            this->ClientSize = System::Drawing::Size(538, 308);
            this->Controls->Add(this->btnMainLoop);
            this->Controls->Add(this->btnMainIntCt);
            this->Controls->Add(this->btnMainInit);
            this->Controls->Add(this->btnClose41);
            this->Controls->Add(this->btnOpen41);
            this->Controls->Add(this->btnTest_getAddress);
            this->Controls->Add(this->btn_Test1);
            this->Controls->Add(this->btn_Test);
            this->Name = L"Form_Maikon";
            this->Text = L"Form_Maikon";
            this->FormClosing += gcnew System::Windows::Forms::FormClosingEventHandler(this, &Form_Maikon::Form_Maikon_FormClosing);
            this->ResumeLayout(false);

        }
#pragma endregion

    private:
        System::Void Form_Maikon_FormClosing(System::Object^  sender, System::Windows::Forms::FormClosingEventArgs^  e)
        {
            //serialPort1->Close();
        }
        
        System::Void serialPort1_DataReceived(System::Object^  sender, System::IO::Ports::SerialDataReceivedEventArgs^  e)
        {
        }
        System::Void serialPort1_ErrorReceived(System::Object^  sender, System::IO::Ports::SerialErrorReceivedEventArgs^  e)
        {
        }
        System::Void serialPort1_PinChanged(System::Object^  sender, System::IO::Ports::SerialPinChangedEventArgs^  e) 
        {
        }
    private:
        System::Void btn_Test_Click(System::Object^  sender, System::EventArgs^  e)
        {
            Console::WriteLine("BaudRate = {0}", maikon->serialPort->BaudRate );
            Console::WriteLine("DataBits = {0}", maikon->serialPort->DataBits );
            Console::WriteLine("Parity   = {0}", maikon->serialPort->Parity );
            Console::WriteLine("StopBits = {0}", maikon->serialPort->StopBits );
        }
        
    private:
        System::Void btn_Test1_Click(System::Object^  sender, System::EventArgs^  e)
        {
            maikon->serialPort->BaudRate = 115200;
            maikon->serialPort->DataBits = 8;
            maikon->serialPort->Parity = System::IO::Ports::Parity::None;
            maikon->serialPort->StopBits = System::IO::Ports::StopBits::One;
        }

   
     private:
         System::Void btnOpen41_Click(System::Object^  sender, System::EventArgs^  e)
         {
             this->serialPort1->ReadTimeout = 2000;
             this->serialPort1->BaudRate = 115200;
             this->serialPort1->PortName = "COM41"; //TESTING - UART CHIP
             this->serialPort1->Parity = System::IO::Ports::Parity::None;
             this->serialPort1->StopBits = System::IO::Ports::StopBits::One;
             this->serialPort1->RtsEnable = true;
             this->serialPort1->DtrEnable = true;
             //this->serialPort1->ErrorReceived += gcnew System::IO::Ports::SerialErrorReceivedEventHandler(this, &MaikonCLI::On_serialPort_ErrorReceived);
             //this->serialPort1->PinChanged += gcnew System::IO::Ports::SerialPinChangedEventHandler(this, &MaikonCLI::On_serialPort_PinChanged);
             //this->serialPort1->DataReceived += gcnew System::IO::Ports::SerialDataReceivedEventHandler(this, &MaikonCLI::On_serialPort_DataReceived);

             serialPort1->Open();
         }
         System::Void btnClose41_Click(System::Object^  sender, System::EventArgs^  e)
         {
             serialPort1->Close();
         }

        System::Void btnTest_getAddress_Click(System::Object^  sender, System::EventArgs^  e)
        {
            int i;
            int countOut = 7;
            int ofstOut = 0;
            array<System::Byte,1>^ bufOut = gcnew array<System::Byte,1>(countOut);

            int countIn = 11;
            int ofstIn = 0;
            array<System::Byte,1>^ bufIn = gcnew array<System::Byte,1>(countIn);
            int nRead = 0;
            
            //-------------------------------------
            bufOut[0] = 0x01; // SOH
          //bufOut[1] = 0x58; // CMD
            bufOut[1] = 0x59; // CMD
            bufOut[2] = 0x00; // SUB
            bufOut[3] = 0x00; // L0
            bufOut[4] = 0x00; // L1
          //bufOut[5] = 0x59; // CS0
            bufOut[5] = 0x5A; // CS0
            bufOut[6] = 0x00; // CS1
 
            //serialPort1->Write( bufOut, ofstOut, countOut);

            serialPort1->Write( bufOut, 0, 1);  Threading::Thread::Sleep(100);
            serialPort1->Write( bufOut, 1, 1);  Threading::Thread::Sleep(100);
            serialPort1->Write( bufOut, 2, 1);  Threading::Thread::Sleep(100);
            serialPort1->Write( bufOut, 3, 1);  Threading::Thread::Sleep(100);
            serialPort1->Write( bufOut, 4, 1);  Threading::Thread::Sleep(100);
            serialPort1->Write( bufOut, 5, 1);  Threading::Thread::Sleep(100);
            serialPort1->Write( bufOut, 6, 1);  Threading::Thread::Sleep(100);

            Threading::Thread::Sleep(100);

            bufIn[0] = 0;
            try
            {
            nRead = serialPort1->Read( bufIn, ofstIn, countIn);

            //-------------------------------------
            /*
            bufIn[0] = 0x01; // SOH
            bufIn[1] = 0x58; // CMD
            bufIn[2] = 0x06; // ACK
            bufIn[3] = 0x04; // L0
            bufIn[4] = 0x00; // L1
            bufIn[5] = 0x44; // DATA
            bufIn[6] = 0x33; // DATA
            bufIn[7] = 0x22; // DATA
            bufIn[8] = 0x11; // DATA
            bufIn[9] = 0x0d; // CS0
            bufIn[10] = 0x01; // CS1
            */
            for( i=0; i<11; i++)
            {
                if( i < nRead )
                    Console::WriteLine(" bufIn[{0}] = 0x{1:x2}", i, bufIn[i]);
                else
                    Console::WriteLine(" bufIn[{0}] = xxxx", i);
            }
            }
            catch(Exception^ ex)
            {
                Console::WriteLine("TIMEOUT {0}", ex->ToString());
            }

        }
        

        System::Void btnMainInit_Click(System::Object^  sender, System::EventArgs^  e)
        {
            main_Init();
        }

        System::Void btnMainIntCt_Click(System::Object^  sender, System::EventArgs^  e)
        {
            main_IntCt();

        }
        System::Void btnMainLoop_Click(System::Object^  sender, System::EventArgs^  e)
        {
            main_Loop();
        }

};
}
