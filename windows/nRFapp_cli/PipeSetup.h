#pragma once



//#using <MasterEmulator.dll>
#using <C:\ble_nrf51\Master Emulator\2.1.12.6\MasterEmulator.dll>
//#using Nordicsemi;
//#using namespace Nordicsemi;


namespace TDnRF
{    
    public ref class PipeSetup
    {
    public:
        /* Public properties for accessing discovered pipe IDs */

        /* TODO
        int UartRxPipe { get; private set; }
        int UartTxPipe { get; private set; }
        */
        /* REF http://vene.wankuma.com/prog/CppCli_Property.aspx
        property int Value
        {
            int get() { return this->_value; }
        private:
            void set(int value)
            {
                this->_value = value;
            }
        }
        int _Value;
        */
        //----------------------------------------
        /* REF
    private:
        int _UartRxPipe;
    public:
        property int UartRxPipe
        {
            int get() { return this->_UartRxPipe; }
        private:
            void set(int value)
            {
                this->_UartRxPipe = value;
            }
        }

        //----------------------------------------
    private:
        int _UartTxPipe;
    public:
        property int UartTxPipe
        {
            int get() { return this->_UartTxPipe; }
        private:
            void set(int value)
            {
                this->_UartTxPipe = value;
            }
        }
        REF */
        //===== Dn =====
        //----------------------------------------
    private:
        int _DcmdPipe;
        int _DdatPipe;
        int _DcfmPipe;
    public:
        property int DcmdPipe
        {
            int get() { return this->_DcmdPipe; }
        private:
            void set(int value) { this->_DcmdPipe = value;  }
        }
        property int DdatPipe
        {
            int get() { return this->_DdatPipe; }
        private:
            void set(int value) { this->_DdatPipe = value;  }
        }
        property int DcfmPipe
        {
            int get() { return this->_DcfmPipe; }
        private:
            void set(int value) { this->_DcfmPipe = value;  }
        }
        
        //===== Up =====
        //----------------------------------------
    private:
        int _UcmdPipe;
        int _UdatPipe;
        int _UcfmPipe;
    public:
        property int UcmdPipe
        {
            int get() { return this->_UcmdPipe; }
        private:
            void set(int value) { this->_UcmdPipe = value;  }
        }
        property int UdatPipe
        {
            int get() { return this->_UdatPipe; }
        private:
            void set(int value) { this->_UdatPipe = value;  }
        }
        property int UcfmPipe
        {
            int get() { return this->_UcfmPipe; }
        private:
            void set(int value) { this->_UcfmPipe = value;  }
        }

        
        //===== Ctrl =====
        //----------------------------------------
    private:
        int _WctrlPipe;
        int _RctrlPipe;
    public:
        property int WctrlPipe
        {
            int get() { return this->_WctrlPipe; }
        private:
            void set(int value) { this->_WctrlPipe = value;  }
        }
        property int RctrlPipe
        {
            int get() { return this->_RctrlPipe; }
        private:
            void set(int value) { this->_RctrlPipe = value;  }
        }

        //===== ACtrl =====
        //----------------------------------------
    private:
        int _WActrlPipe;
        int _RActrlPipe;
    public:
        property int WActrlPipe
        {
            int get() { return this->_WActrlPipe; }
        private:
            void set(int value) { this->_WActrlPipe = value;  }
        }
        property int RActrlPipe
        {
            int get() { return this->_RActrlPipe; }
        private:
            void set(int value) { this->_RActrlPipe = value;  }
        }



        Nordicsemi::MasterEmulator^ masterEmulator;

        PipeSetup(Nordicsemi::MasterEmulator^ master)
        {
            masterEmulator = master;
        }

        /// <summary>
        /// Pipe setup is performed by sequentially adding services, characteristics and
        /// descriptors. Pipes can be added to the characteristics and descriptors one wants
        /// to have access to from the application during runtime. A pipe assignment must
        /// be stated directly after the characteristic or descriptor it shall apply for.
        /// The pipe type chosen will affect what operations can be performed on the pipe
        /// at runtime. <see cref="Nordicsemi.PipeType"/>.
        /// </summary>
        /// 
        /* REF
        void PerformPipeSetup_nRFUart()
        {
            // GAP service 
            Nordicsemi::BtUuid^ uartOverBtleUuid = gcnew Nordicsemi::BtUuid("6e400001b5a3f393e0a9e50e24dcca9e");
            masterEmulator->SetupAddService(uartOverBtleUuid, Nordicsemi::PipeStore::Remote);

            // UART RX characteristic (RX from peripheral's viewpoint) 
            Nordicsemi::BtUuid^ uartRxUuid = gcnew Nordicsemi::BtUuid("6e400002b5a3f393e0a9e50e24dcca9e");
            int uartRxMaxLength = 20;
            //byte[] uartRxData = null;
            array<unsigned char, 1>^ uartRxData = nullptr;
            masterEmulator->SetupAddCharacteristicDefinition(uartRxUuid, uartRxMaxLength, uartRxData);
            // Using pipe type Transmit to enable write operations 
            UartRxPipe = masterEmulator->SetupAssignPipe(Nordicsemi::PipeType::Transmit);

            // UART TX characteristic (TX from peripheral's viewpoint) 
            Nordicsemi::BtUuid^ UartTxUuid = gcnew Nordicsemi::BtUuid("6e400003b5a3f393e0a9e50e24dcca9e");
            int uartTxMaxLength = 20;
            //byte[] uartTxData = null;
            array<unsigned char, 1>^ uartTxData = nullptr;
            masterEmulator->SetupAddCharacteristicDefinition(UartTxUuid, uartTxMaxLength, uartTxData);
            // Using pipe type Receive to enable notify operations 
            UartTxPipe = masterEmulator->SetupAssignPipe(Nordicsemi::PipeType::Receive);

            //UartRxPipe = 666042;
            //UartTxPipe = 666042;
        }
        REF */
        void PerformPipeSetup()
        {
            // GAP service 
            Nordicsemi::BtUuid^ TDudOverBtleUuid = gcnew Nordicsemi::BtUuid("6e400001b5a3f393e0a9e50e24dcca42");
            masterEmulator->SetupAddService(TDudOverBtleUuid, Nordicsemi::PipeStore::Remote);

            //===== Dn =====
            // DCMD characteristic (Down Link Command 0x0002) 
            Nordicsemi::BtUuid^ DcmdUuid = gcnew Nordicsemi::BtUuid("6e400002b5a3f393e0a9e50e24dcca42");
            int DcmdMaxLength = 20;
            array<unsigned char, 1>^ DcmdData = nullptr;
            masterEmulator->SetupAddCharacteristicDefinition(DcmdUuid, DcmdMaxLength, DcmdData);
            // Using pipe type Transmit to enable write operations 
            DcmdPipe = masterEmulator->SetupAssignPipe(Nordicsemi::PipeType::Transmit);

            // DDAT characteristic (Down Link Data 0x0003) 
            Nordicsemi::BtUuid^ DdatUuid = gcnew Nordicsemi::BtUuid("6e400003b5a3f393e0a9e50e24dcca42");
            int DdatMaxLength = 20;
            array<unsigned char, 1>^ DdatData = nullptr;
            masterEmulator->SetupAddCharacteristicDefinition(DdatUuid, DdatMaxLength, DdatData);
            // Using pipe type Receive to enable notify operations 
            DdatPipe = masterEmulator->SetupAssignPipe(Nordicsemi::PipeType::Transmit);

            // DCFM characteristic (Down Link Confirm 0x0004)
            Nordicsemi::BtUuid^ DcfmUuid = gcnew Nordicsemi::BtUuid("6e400004b5a3f393e0a9e50e24dcca42");
            int DcfmMaxLength = 20;
            array<unsigned char, 1>^ DcfmData = nullptr;
            masterEmulator->SetupAddCharacteristicDefinition(DcfmUuid, DcfmMaxLength, DcfmData);
            // Using pipe type Receive to enable notify operations 
            DcfmPipe = masterEmulator->SetupAssignPipe(Nordicsemi::PipeType::Receive);

            //===== Up =====
            // UCMD characteristic (Up Link Command 0x0005) 
            Nordicsemi::BtUuid^ UcmdUuid = gcnew Nordicsemi::BtUuid("6e400005b5a3f393e0a9e50e24dcca42");
            int UcmdMaxLength = 20;
            array<unsigned char, 1>^ UcmdData = nullptr;
            masterEmulator->SetupAddCharacteristicDefinition(UcmdUuid, UcmdMaxLength, UcmdData);
            // Using pipe type Transmit to enable write operations 
            UcmdPipe = masterEmulator->SetupAssignPipe(Nordicsemi::PipeType::Receive);

            // DDAT characteristic (Up Link Data 0x0006) 
            Nordicsemi::BtUuid^ UdatUuid = gcnew Nordicsemi::BtUuid("6e400006b5a3f393e0a9e50e24dcca42");
            int UdatMaxLength = 20;
            array<unsigned char, 1>^ UdatData = nullptr;
            masterEmulator->SetupAddCharacteristicDefinition(UdatUuid, UdatMaxLength, UdatData);
            // Using pipe type Receive to enable notify operations 
            UdatPipe = masterEmulator->SetupAssignPipe(Nordicsemi::PipeType::Receive);

            // UCFM characteristic (Up Link Confirm 0x0007)
            Nordicsemi::BtUuid^ UcfmUuid = gcnew Nordicsemi::BtUuid("6e400007b5a3f393e0a9e50e24dcca42");
            int UcfmMaxLength = 20;
            array<unsigned char, 1>^ UcfmData = nullptr;
            masterEmulator->SetupAddCharacteristicDefinition(UcfmUuid, UcfmMaxLength, UcfmData);
            // Using pipe type Receive to enable notify operations 
            UcfmPipe = masterEmulator->SetupAssignPipe(Nordicsemi::PipeType::Transmit);


        }
        
        void PerformPipeSetup_TDctrl()
        {
            // GAP service 
            Nordicsemi::BtUuid^ TDctrl_OverBtleUuid = gcnew Nordicsemi::BtUuid("6e400001b5a3f393e0a9e50e24dcca43");
            masterEmulator->SetupAddService(TDctrl_OverBtleUuid, Nordicsemi::PipeStore::Remote);

            //===== Ctrl =====
            // WCTRL characteristic (Write Link Command 0x00xx) 
            Nordicsemi::BtUuid^ WctrlUuid = gcnew Nordicsemi::BtUuid("6e400008b5a3f393e0a9e50e24dcca43");
            int WctrlMaxLength = 20;
            array<unsigned char, 1>^ WctrlData = nullptr;
            masterEmulator->SetupAddCharacteristicDefinition(WctrlUuid, WctrlMaxLength, WctrlData);
            // Using pipe type Transmit to enable write operations 
            //DcmdPipe = masterEmulator->SetupAssignPipe(Nordicsemi::PipeType::Transmit);
            WctrlPipe = masterEmulator->SetupAssignPipe(Nordicsemi::PipeType::Transmit);

            // RCTRL characteristic (Read Link Command 0x00xx) 
            Nordicsemi::BtUuid^ RctrlUuid = gcnew Nordicsemi::BtUuid("6e400009b5a3f393e0a9e50e24dcca43");
            int RctrlMaxLength = 20;
            array<unsigned char, 1>^ RctrlData = nullptr;
            masterEmulator->SetupAddCharacteristicDefinition(RctrlUuid, RctrlMaxLength, RctrlData);
            // Using pipe type Transmit to enable write operations 
            RctrlPipe = masterEmulator->SetupAssignPipe(Nordicsemi::PipeType::ReceiveRequest);
            //RctrlPipe = masterEmulator->SetupAssignPipe(Nordicsemi::PipeType::Receive); NG


            //===== ACtrl =====
            // WACTRL characteristic (Write Link Command 0x00xx) 
            Nordicsemi::BtUuid^ WActrlUuid = gcnew Nordicsemi::BtUuid("6e40000ab5a3f393e0a9e50e24dcca43");
            int WActrlMaxLength = 20;
            array<unsigned char, 1>^ WActrlData = nullptr;
            masterEmulator->SetupAddCharacteristicDefinition(WActrlUuid, WActrlMaxLength, WActrlData);
            // Using pipe type Transmit to enable write operations 
            WActrlPipe = masterEmulator->SetupAssignPipe(Nordicsemi::PipeType::Transmit);

            // RACTRL characteristic (Read Link Command 0x00xx) 
            Nordicsemi::BtUuid^ RActrlUuid = gcnew Nordicsemi::BtUuid("6e40000bb5a3f393e0a9e50e24dcca43");
            int RActrlMaxLength = 20;
            array<unsigned char, 1>^ RActrlData = nullptr;
            masterEmulator->SetupAddCharacteristicDefinition(RActrlUuid, RActrlMaxLength, RActrlData);
            // Using pipe type Transmit to enable write operations 
            RActrlPipe = masterEmulator->SetupAssignPipe(Nordicsemi::PipeType::ReceiveRequest);


        }

    };


}
