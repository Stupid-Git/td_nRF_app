#pragma once

#include "MaikonC.h"

using namespace System;

namespace nRFMaikon
{
    public ref class MaikonCLI
    {
    private:
        System::ComponentModel::IContainer^  components;
    public:
        System::IO::Ports::SerialPort^ serialPort;

    public:
        MaikonCLI(System::ComponentModel::IContainer^ param_components)
        {
            components = param_components;
            this->serialPort = (gcnew System::IO::Ports::SerialPort(this->components));
          //this->serialPort = (gcnew System::IO::Ports::SerialPort());
            serial_SetDefaultParameters();
            maikonC_Init();
        }

        void serial_SetDefaultParameters()
        {
            this->serialPort->BaudRate = 115200;
            this->serialPort->PortName = "COM56"; //REAL - nRF MODULE
          //this->serialPort->PortName = "COM59"; //TESTING - UART CHIP
            this->serialPort->RtsEnable = true;
            this->serialPort->DtrEnable = true;
            //this->serialPort->CtsHolding;
            this->serialPort->ErrorReceived += gcnew System::IO::Ports::SerialErrorReceivedEventHandler(this, &MaikonCLI::On_serialPort_ErrorReceived);
            this->serialPort->PinChanged += gcnew System::IO::Ports::SerialPinChangedEventHandler(this, &MaikonCLI::On_serialPort_PinChanged);
            this->serialPort->DataReceived += gcnew System::IO::Ports::SerialDataReceivedEventHandler(this, &MaikonCLI::On_serialPort_DataReceived);
        }

        bool serial_IsOpen()
        {
            return( this->serialPort->IsOpen );
        }
        bool serial_Open()
        {
            try
            {
                this->serialPort->Open();
            }
            catch(System::Exception^ ex)
            {
                Console::WriteLine("{0}", ex->ToString() );
                return(false);
            }
            return(true);
        }

        bool serial_Close()
        {
            try
            {
                this->serialPort->Close();
            }
            catch(Exception^ ex)
            {
                Console::WriteLine("{0}", ex->ToString() );
                return(false);
            }
            return(true);
        }

        Void serial_DataSend()
        {
            int32_t i;
            int32_t size;
            int32_t r;
            uint8_t c;
            try
            {
                size = maikonC_txStreamSize();
                if( size == 0 )
                    return;
                array<System::Byte,1>^ buffer = gcnew array<System::Byte,1>(size);
                
                for( i = 0; i < size; i++)
                {
                    r = maikonC_txStreamPop( &c );
                    if(r == 0)
                    {
                        Console::WriteLine("maikonC_txStreamPop: EMPTY");
                        break;
                    }
                    buffer[i] = c;
                }

                int offset = 0;
                int count = size;

                Console::WriteLine("\nmaikonCLI: serial_DataSend sent {0} bytes", count);
                serialPort->Write(buffer, offset, count);
            }
            catch(Exception^ ex)
            {
                Console::WriteLine("{0}", ex->ToString() );
            }

        }
        
        System::Void serialPort_Prod(void)
        {
            System::Object^  sender;
            System::IO::Ports::SerialDataReceivedEventArgs^  e;

            sender = nullptr;
            e = nullptr;

            On_serialPort_DataReceived(sender, e);
        }

        System::Void On_serialPort_DataReceived(System::Object^  sender, System::IO::Ports::SerialDataReceivedEventArgs^  e)
        {
            int i;
            array<System::Byte,1>^ buffer = gcnew array<System::Byte,1>(128);
            int offset = 0;
            int count = 128;
            int nRead;

            if( serialPort->BytesToRead == 0)
            {
                Console::WriteLine("\nSerialPort: nothing to read");
                return;
            }

            nRead = serialPort->Read(buffer, offset, count);
            /*
            if( e->EventType ==  System::IO::Ports::SerialData::Chars)
            {
            }
            */
            //Console::Write("\nnRead = {0} : 0x{1:x2},0x{2:x2},0x{3:x2},0x{4:x2}", nRead, buffer[0], buffer[1], buffer[2], buffer[3]);

            Console::Write("\nnRead = {0} : ", nRead);
            for( i=0; i< nRead; i++ )
            {
                Console::Write("{0:x2} ", buffer[i]);
            }
            Console::Write("\n");

            System::Text::Encoding^ enc = gcnew System::Text::UTF8Encoding();
            array<System::Byte>^ encodedBytes = gcnew array<System::Byte>(nRead);
            Array::Copy(buffer,0,encodedBytes,0, nRead);

            //array<System::Byte>^ encodedBytes = enc->GetBytes(value);
            //if (encodedBytes->Length > maxPacketLength)
            //{
            //    Array::Resize<System::Byte>( encodedBytes, maxPacketLength);
            //    AddToLog("Max packet size is 20 characters, text is truncated.");
            //}
            //masterEmulator->SendData(pipeSetup->DdatPipe, encodedBytes);
            String^ decodedString = enc->GetString(encodedBytes);
            //AddToLog(String::Format("TX Ddat: {0}", decodedString));
            Console::WriteLine("  --  {0}", decodedString);

            int r;
            for( i = 0; i < nRead; i++)
            {
                r = maikonC_rxStreamPush( buffer[i] );
                if(r == 0)
                {
                    Console::WriteLine("maikonC_rxStreamPush: FULL");
                    break;
                }
            }

            //OLD__maikonC_main_proc();
            maikonC_main_proc();

            serial_DataSend();
        }

        System::Void On_serialPort_ErrorReceived(System::Object^  sender, System::IO::Ports::SerialErrorReceivedEventArgs^  e)
        {
            Console::WriteLine("maikonCLI_On_serialPort_ErrorReceived: {0}", e->EventType);
        }
        System::Void On_serialPort_PinChanged(System::Object^  sender, System::IO::Ports::SerialPinChangedEventArgs^  e) 
        {
            Console::WriteLine("maikonCLI_On_serialPort_PinChanged: {0}", e->EventType);
        }



    };

}
