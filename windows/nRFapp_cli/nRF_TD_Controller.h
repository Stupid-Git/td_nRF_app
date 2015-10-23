#pragma once


#include "PipeSetup.h"
#include "UpDnEngine.h"

//#using <MasterEmulator.dll>
#using <C:\ble_nrf51\Master Emulator\2.1.12.6\MasterEmulator.dll>
//#using Nordicsemi;








using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Collections::Generic;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;

using namespace System::Threading;



namespace nRFUart_TD
{

using System::Threading::Tasks::Task;
using System::Collections::Generic::List;
using System::Collections::Generic::IDictionary;
using System::Collections::Generic::Dictionary;
using System::Text::Encoding;   


    /// <summary>
    /// Provides data for the OutputReceived event.
    /// </summary>
    //http://stackoverflow.com/a/9700348/3579820 C# -> CLI example
    public ref class OutputReceivedEventArgs : EventArgs
    {
    public:
        String^ _Message;
    public:
        OutputReceivedEventArgs(String^ message)
        {
            _Message = message;
        };

        property String^ Message {
            String^ get() { return _Message; }
        }
    };

    /// <summary>
    /// This class controls all calls to MasterEmulator DLL and implements the nRF UART 
    /// logic.
    /// </summary>
    ref class nRF_TD_Kontroller
    {

    public:
        event EventHandler<OutputReceivedEventArgs^>^ LogMessage;
        event EventHandler<EventArgs^>^ Initialized;
        event EventHandler<EventArgs^>^ Scanning;
        event EventHandler<EventArgs^>^ ScanningCanceled;
        event EventHandler<EventArgs^>^ Connecting;
        event EventHandler<EventArgs^>^ ConnectionCanceled;
        event EventHandler<EventArgs^>^ Connected;
        event EventHandler<EventArgs^>^ PipeDiscoveryCompleted;
        event EventHandler<EventArgs^>^ Disconnected;
        event EventHandler<EventArgs^>^ SendDataStarted;
        event EventHandler<EventArgs^>^ SendDataCompleted;
        event EventHandler<Nordicsemi::ValueEventArgs<int>^>^ ProgressUpdated;


        /* Public properties */
        //bool DebugMessagesEnabled { get; set; }
        bool DebugMessagesEnabled;

        /* Instance variables */
        Nordicsemi::MasterEmulator^ masterEmulator;
        PipeSetup^ pipeSetup;
        UpDnEngine^ udEngine;

        bool connectionInProgress;
        bool UARTsendData;

        int maxPacketLength;
        int counterFieldLength;
        int maxPayloadLength;// = maxPacketLength - counterFieldLength;

        static nRF_TD_Kontroller^ pS_controller;

        //---------------------------------------------------------------------
    public:
        /// <summary>
        /// Constructor.
        /// </summary>
        nRF_TD_Kontroller(void)
        {
            DebugMessagesEnabled = true;

            connectionInProgress = false;
            UARTsendData = false;

            maxPacketLength = 20;
            counterFieldLength = 2;
            maxPayloadLength = maxPacketLength - counterFieldLength;

            pS_controller = this;
        };


        /// <summary>
        /// Connect to peer device.
        /// </summary>
        void InitiateConnection() //S
        {
            bool success = StartDeviceDiscovery();
        }

        /// <summary>
        /// Stop scanning for devices.
        /// </summary>
        void StopScanning() //S
        {
            if (!masterEmulator->IsDeviceDiscoveryOngoing)
            {
                return;
            }

            bool success = masterEmulator->StopDeviceDiscovery();

            if (success)
            {
                ScanningCanceled(this, EventArgs::Empty);
            }
        }

        /// <summary>
        /// Send data to peer device.
        /// </summary>
        /// <param name="value"></param>
#if 0 //false
        void UART_SendData(String^ value) //S
        {   
            System::Text::Encoding^ enc = gcnew System::Text::UTF8Encoding(); // byte[] encodedBytes =  System::Text::Encoding::UTF8::GetBytes( value );
            array<System::Byte>^ encodedBytes = enc->GetBytes(value);

            if (encodedBytes->Length > maxPacketLength)
            {
                Array::Resize<System::Byte>( encodedBytes, maxPacketLength);
                AddToLog("Max packet size is 20 characters, text is truncated.");
            }

            masterEmulator->SendData(pipeSetup->UartRxPipe, encodedBytes);

            String^ decodedString = enc->GetString(encodedBytes);  // String^ decodedString = Encoding.UTF8.GetString(encodedBytes);
            AddToLog(String::Format("TX: {0}", decodedString));
        }

        void Send_Dcmd(String^ value)
        {   
            System::Text::Encoding^ enc = gcnew System::Text::UTF8Encoding();
            array<System::Byte>^ encodedBytes = enc->GetBytes(value);
            if (encodedBytes->Length > maxPacketLength)
            {
                Array::Resize<System::Byte>( encodedBytes, maxPacketLength);
                AddToLog("Max packet size is 20 characters, text is truncated.");
            }
            masterEmulator->SendData(pipeSetup->DcmdPipe, encodedBytes);
            String^ decodedString = enc->GetString(encodedBytes);
            AddToLog(String::Format("TX Dcmd: {0}", decodedString));
        }
        void Send_Ddat(String^ value)
        {   
            System::Text::Encoding^ enc = gcnew System::Text::UTF8Encoding();
            array<System::Byte>^ encodedBytes = enc->GetBytes(value);
            if (encodedBytes->Length > maxPacketLength)
            {
                Array::Resize<System::Byte>( encodedBytes, maxPacketLength);
                AddToLog("Max packet size is 20 characters, text is truncated.");
            }
            masterEmulator->SendData(pipeSetup->DdatPipe, encodedBytes);
            String^ decodedString = enc->GetString(encodedBytes);
            AddToLog(String::Format("TX Ddat: {0}", decodedString));
        }

        void Send_Ucfm(String^ value)
        {   
            System::Text::Encoding^ enc = gcnew System::Text::UTF8Encoding();
            array<System::Byte>^ encodedBytes = enc->GetBytes(value);
            if (encodedBytes->Length > maxPacketLength)
            {
                Array::Resize<System::Byte>( encodedBytes, maxPacketLength);
                AddToLog("Max packet size is 20 characters, text is truncated.");
            }
            masterEmulator->SendData(pipeSetup->DdatPipe, encodedBytes);
            String^ decodedString = enc->GetString(encodedBytes);
            AddToLog(String::Format("TX Ddat: {0}", decodedString));
        }
#endif //0 //false


        void EnableNotify_Dcfm()
        {
            masterEmulator->GetCharacteristicProperties(pipeSetup->DcfmPipe);
        }

#if 0

        /// <summary>
        /// Start sending a data to the peer device.
        /// </summary>
        /// <param name="data">An arbitrarily large byte array of data to send.</param>
        /// <remarks>The method will continue to send until all data has been
        /// transmitted or the transmission has been stopped by <see cref="UARTStopSendData"/>.
        /// </remarks>
        void UARTStartSendData(byte[] data)
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
                        masterEmulator->SendData(pipeSetup.UartRxPipe, splitData[i]);

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
        void UARTStartSendData(byte[] data, int numberOfRepetitions)
        {
            if (data == null)
            {
                throw new ArgumentNullException();
            }

            if (data.Length > maxPayloadLength)
            {
                throw new ArgumentException(String^.Format("Length of data must not exceed {0}.",
                    maxPayloadLength));
            }

            int totalPacketSize = data.Length * numberOfRepetitions;

            var aggregatedData = new List<byte>(totalPacketSize);

            for (int i = 0; i < numberOfRepetitions; i++)
            {
                aggregatedData.AddRange(data);
            }

            UARTStartSendData(aggregatedData.ToArray());
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
        /// Signal UARTStartSendData task to cancel sending of data.
        /// </summary>
        void UARTStopSendData() //NNN  //YYY
        {
            UARTsendData = false;
        }

        /// <summary>
        /// Disconnect from peer device.
        /// </summary>
        void InitiateDisconnect() //YYY
        {
            if (!masterEmulator->IsConnected)
            {
                return;
            }

            masterEmulator->Disconnect();
        }

        /// <summary>
        /// Close MasterEmulator.
        /// </summary>
        void Close() //YYY
        {
            if (!masterEmulator->IsOpen)
            {
                return;
            }

            masterEmulator->Close();
        }

        /// <summary>
        ///  Method for adding text to the textbox and logfile.
        ///  When called on the main thread, invoke is not required.
        ///  For other threads, the invoke is required.
        /// </summary>
        /// <param name="message">The message String^ to add to the log.</param>
        void AddToLog(String^ message)
        {
            //TODO if (LogMessage != nullptr)
            {
                LogMessage(this, gcnew OutputReceivedEventArgs( message) );
            }

            // Writing to trace also, which causes the message to be put in the log file.
            Console::WriteLine("AddToLog : " + message); //TODO Trace::WriteLine(message);
        }

        void karelLog(String^ message) //YYY
        {
            AddToLog("karelLog : " + message);
        }

        /// <summary>
        /// Convenience method for logging exception messages.
        /// </summary>
        void LogErrorMessage(String^ errorMessage, String^ stackTrace) //YYY
        {
            AddToLog(errorMessage);
            Console::WriteLine(stackTrace); //TODO Trace.WriteLine(stackTrace);
        }

        /// <summary>
        /// Get the path to the log file of MasterEmulator.
        /// </summary>
        /// <returns>Returns path to log file.</returns>
        String^ GetLogfilePath() //YYY
        {
            return masterEmulator->GetLogFilePath();

        }

        /// <summary>
        /// Collection of method calls to start and setup MasterEmulator.
        /// The calls are placed in a background task for not blocking the gui thread.
        /// </summary>
        void Initialize_Task()
        {
            try
            {
                InitializeMasterEmulator();
                RegisterEventHandlers();
                String^ device = FindUsbDevice();
                OpenMasterEmulatorDevice(device);

                pipeSetup = gcnew PipeSetup(masterEmulator);
                //pipeSetup->PerformPipeSetup_nRFUart();
                pipeSetup->PerformPipeSetup();

                udEngine = gcnew UpDnEngine();
                udEngine->UpDnEngine_Setup(masterEmulator, pipeSetup);

                Run();
                Initialized(this, EventArgs::Empty);
            }
            catch (Exception^ ex)
            {
                LogErrorMessage(String::Format("Exception in StartMasterEmulator: {0}", ex->Message),
                    ex->StackTrace);
            }
        }
        void Initialize() //NNN //YYY ??
        {
#if 1
            AddToLog(gcnew String("Initialize is starting Up dude") );
            System::Threading::Tasks::Task^ myTask = System::Threading::Tasks::Task::Factory->StartNew(gcnew Action(this, &nRF_TD_Kontroller::Initialize_Task));
            // For non-static methods, specify the object.      ^^^^ 
            // Use the C++-style reference to a class method.         ^^^^^^^^^^^^^^^^^^^^

#else
            System::Threading::Tasks::Task::Factory::StartNew(() =>
            {
                try
                {
                    InitializeMasterEmulator();
                    RegisterEventHandlers();
                    String^ device = FindUsbDevice();
                    OpenMasterEmulatorDevice(device);
                    pipeSetup = gcnew PipeSetup(masterEmulator);
                    //pipeSetup->PerformPipeSetup_nRFUart();
                    pipeSetup->PerformPipeSetup();
                    Run();
                    Initialized(this, EventArgs::Empty);
                }
                catch (Exception^ ex)
                {
                    LogErrorMessage(String::Format("Exception in StartMasterEmulator: {0}", ex->Message),
                        ex->StackTrace);
                }
            });
#endif
        }

        /// <summary>
        /// Create MasterEmulator instance.
        /// </summary>
        void InitializeMasterEmulator() //YYY
        {
            AddToLog("Loading...");
            masterEmulator = gcnew Nordicsemi::MasterEmulator();
        }

        /// <summary>
        /// Register event handlers for MasterEmulator events.
        /// </summary>
        void RegisterEventHandlers() //YYY
        {
            masterEmulator->Connected += gcnew  EventHandler<EventArgs^>(this, &nRF_TD_Kontroller::OnConnected );
            masterEmulator->ConnectionUpdateRequest += gcnew  EventHandler<Nordicsemi::ConnectionUpdateRequestEventArgs^>(this, &nRF_TD_Kontroller::OnConnectionUpdateRequest );
            masterEmulator->DataReceived += gcnew  EventHandler<Nordicsemi::PipeDataEventArgs^>(this, &nRF_TD_Kontroller::OnDataReceived );
            masterEmulator->DeviceDiscovered += gcnew  EventHandler<Nordicsemi::ValueEventArgs<Nordicsemi::BtDevice^>^>(this, &nRF_TD_Kontroller::OnDeviceDiscovered );
            masterEmulator->Disconnected += gcnew  EventHandler<Nordicsemi::ValueEventArgs<Nordicsemi::DisconnectReason>^>(this, &nRF_TD_Kontroller::OnDisconnected );
            masterEmulator->LogMessage += gcnew  EventHandler<Nordicsemi::ValueEventArgs<System::String^>^>(this, &nRF_TD_Kontroller::OnLogMessage );
#if 0
            masterEmulator->Connected += OnConnected;
            masterEmulator->ConnectionUpdateRequest += OnConnectionUpdateRequest;
            masterEmulator->DataReceived += OnDataReceived;
            masterEmulator->DeviceDiscovered += OnDeviceDiscovered;
            masterEmulator->Disconnected += OnDisconnected;
            masterEmulator->LogMessage += OnLogMessage;
#endif
        }

        /// <summary>
        /// Searching for master emulator devices attached to the pc. 
        /// If more than one is connected it will simply return the first in the list.
        /// </summary>
        /// <returns>Returns the first master emulator device found.</returns>
        String^ FindUsbDevice() //YYY
        {
            /* The UsbDeviceType argument is used for filtering master emulator device types. */
            auto devices = masterEmulator->EnumerateUsb(Nordicsemi::UsbDeviceType::AnyMasterEmulator);
            //var devices = masterEmulator->EnumerateUsb(UsbDeviceType.AnyMasterEmulator);

            if (devices->Count > 0)
            {
                return devices[0];
            }
            else
            {
                return String::Empty;
            }
        }

        /// <summary>
        /// Tell the api to use the given master emulator device.
        /// </summary>
        /// <param name="device"></param>
        void OpenMasterEmulatorDevice(String^ device) //YYY
        {
            if (masterEmulator->IsOpen)
            {
                return;
            }

            masterEmulator->Open(device);
            masterEmulator->Reset();
        }

        /// <summary>
        /// By calling Run, the pipesetup is processed and the stack engine is started.
        /// </summary>
        void Run() //YYY
        {
            if (masterEmulator->IsRunning)
            {
                return;
            }

            masterEmulator->Run();
        }

        /// <summary>
        /// Device discovery is started with the given scan parameters.
        /// By stating active scan, we will be receiving data from both advertising
        /// and scan repsonse packets.
        /// </summary>
        /// <returns></returns>
        bool StartDeviceDiscovery() //YYY
        {
            if (!masterEmulator->IsRunning)
            {
                AddToLog("Not ready.");
                return false;
            }

            Nordicsemi::BtScanParameters^ scanParameters = gcnew Nordicsemi::BtScanParameters();
            scanParameters->ScanType = Nordicsemi::BtScanType::ActiveScanning;
            bool startSuccess = masterEmulator->StartDeviceDiscovery(scanParameters);

            if (startSuccess)
            {
                Scanning(this, EventArgs::Empty);
            }

            return startSuccess;
        }
        /*
        enum class Colors
        {
            Red, Green, Blue, Yellow
        };

        enum class Styles
        {
            Plaid = 0,
            Striped = 23,
            Tartan = 65,
            Corduroy = 78
        };
        */

        void karelDumpDevice(Nordicsemi::BtDevice^ device) //YYY
        {
            //###
            /*
            Console::WriteLine(  "The values of the Colors Enum are:" );
            Array^ a = Enum::GetValues( Colors::typeid );
            for ( Int32 i = 0; i < a->Length; i++ )
            {
            Object^ o = a->GetValue( i );
            Console::WriteLine(  "{0}", Enum::Format( Colors::typeid, o,  "D" ) );
            }
            Console::WriteLine();
            Console::WriteLine(  "The values of the Styles Enum are:" );
            Array^ b = Enum::GetValues( Styles::typeid );
            for ( Int32 i = 0; i < b->Length; i++ )
            {
            Object^ o = b->GetValue( i );
            Console::WriteLine(  "{0}", Enum::Format( Styles::typeid, o,  "D" ) );

            }
            */
            //###
            IDictionary<Nordicsemi::DeviceInfoType, String^>^ deviceInfo;
            Nordicsemi::BtDeviceAddress^ deviceAddress;

            String^ str = String::Empty;

            //String^ deviceName = String^.Empty;
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
            deviceInfo = device->DeviceInfo;

            deviceAddress = device->DeviceAddress;


            karelLog("$$$$ karelDumpDevice $$$$");

            str = String::Format("--Device Address: {0}", deviceAddress->Value);
            karelLog(str);

            karelLog("$$$$ karelDumpDevice $$$$");

            //http://stackoverflow.com/questions/972307/can-you-loop-through-all-enum-values
            //var Avalues = Enum.GetValues(typeof(Nordicsemi::DeviceInfoType));
            /*
            auto Avalues = Enum::GetValues(Nordicsemi::DeviceInfoType);
            auto Bvalues = Enum::GetValues(typeof(Nordicsemi::DeviceInfoType))->Cast<Nordicsemi::DeviceInfoType>();
            */
            //Array^ hga = Enum::GetValues( Nordicsemi::DeviceInfoType::typeid );
            //auto Avalues = Enum::GetValues( typeid(Nordicsemi::DeviceInfoType) );

          //array<Nordicsemi::DeviceInfoType>^arr = static_cast<array<Nordicsemi::DeviceInfoType>^>(Enum::GetValues( Nordicsemi::DeviceInfoType:typeid ));
            array<Nordicsemi::DeviceInfoType>^arr = static_cast<array<Nordicsemi::DeviceInfoType>^>(Enum::GetValues( Nordicsemi::DeviceInfoType::typeid ));

            //for each (auto p in Bvalues)
            //for each (auto p in Nordicsemi::DeviceInfoType)
            for each (auto p in arr)
            {
                if (deviceInfo->ContainsKey( p ))
                {
                    str = String::Format("--{0}: {1}", p, deviceInfo[ p ]);
                    karelLog(str);
                }
            }

            karelLog("$$$$ karelDumpDevice $$$$ [END]");

        }


#if 0
        /// <summary>
        /// Connecting to the given device, and with the given connection parameters.
        /// </summary>
        /// <param name="device">Device to connect to.</param>
        /// <returns>Returns success indication.</returns>
        //bool Connect(BtDevice device) //YYY
        bool Connect_nRFUart(BtDevice device) //YYY
        {
            if (masterEmulator->IsDeviceDiscoveryOngoing)
            {
                masterEmulator->StopDeviceDiscovery();
            }

            karelDumpDevice(device); //karel

            /* NG here*/
            //karel - from Documentation
            karelLog("\r\n");
            karelLog("\r\n");
            karelLog("========== TESTING masterEmulator->DiscoverServices() ================\r\n");
            karelLog("\r\n");

            var attributes = masterEmulator->DiscoverServices();
            foreach (var item in attributes)
            {
                Trace.WriteLine(item.ToString());
                karelLog("-----> " + item.ToString());
            }
            /**/

            String^ deviceName = GetDeviceName(device.DeviceInfo);
            AddToLog(String^.Format("Connecting to {0}, Device name: {1}",
                device.DeviceAddress.ToString(), deviceName));

            BtConnectionParameters connectionParams = new BtConnectionParameters();
            connectionParams.ConnectionIntervalMs = 11.25;
            connectionParams.ScanIntervalMs = 250;
            connectionParams.ScanWindowMs = 200;
            bool connectSuccess = masterEmulator->Connect(device.DeviceAddress, connectionParams);
            return connectSuccess;
        }
#endif

#if 1
        bool Connect/*_nRFTD1*/(Nordicsemi::BtDevice^ device) //YYY
        {
            //if (masterEmulator->IsDeviceDiscoveryOngoing)
            while (masterEmulator->IsDeviceDiscoveryOngoing)
            {
                masterEmulator->StopDeviceDiscovery();
            }

            karelDumpDevice(device); //karel

            String^ deviceName = GetDeviceName(device->DeviceInfo);
            AddToLog(String::Format("Connecting to {0}, Device name: {1}",
                device->DeviceAddress->ToString(), deviceName));

            Nordicsemi::BtConnectionParameters^ connectionParams = gcnew Nordicsemi::BtConnectionParameters();
            //TODO _nRFTD1 connection parameters may be different than for _nRFUart
            connectionParams->ConnectionIntervalMs = 11.25;
            connectionParams->ScanIntervalMs = 250;
            connectionParams->ScanWindowMs = 200;
            //TODO _nRFTD1 connection parameters may be different than for _nRFUart
            bool connectSuccess;
            connectSuccess = masterEmulator->Connect(device->DeviceAddress, connectionParams);
            return connectSuccess;
        }
#endif

        /// <summary>
        /// By discovering pipes, the pipe setup we have specified will be matched up
        /// to the remote device's ATT table by ATT service discovery.
        /// </summary>
        void DiscoverPipes() //NNN
        {
            bool success = masterEmulator->DiscoverPipes();

            if (!success)
            {
                AddToLog("DiscoverPipes did not succeed.");
            }
        }

        /// <summary>
        /// Pipes of type PipeType.Receive must be opened before they will start receiving notifications.
        /// This maps to ATT Client Configuration Descriptors.
        /// </summary>
        void OpenRemotePipes() //NNN
        {
            //var openedPipesEnumeration = masterEmulator->OpenAllRemotePipes();
            auto  openedPipesEnumeration = masterEmulator->OpenAllRemotePipes();

            List<int>^ openedPipes = gcnew List<int>(openedPipesEnumeration);
        }

        /// <summary>
        /// Event handler for DeviceDiscovered. This handler will be called when devices
        /// are discovered during asynchronous device discovery.
        /// </summary>

        delegate int i_FN_BTdev(Nordicsemi::BtDevice^);
        //void OnDeviceDiscovered_Task(Nordicsemi::BtDevice^ device)
        void OnDeviceDiscovered_Task(Object ^ o)
        {
            Nordicsemi::BtDevice^ device;

            device = static_cast<Nordicsemi::BtDevice^>(o);

            try
            {
                Connecting(this, EventArgs::Empty); // Connecting event to MainWindow

                bool success = Connect(device);
                /*
                bool success;
                if (USE_TD1 == false)
                success = Connect_nRFUart(device);
                if (USE_TD1 == true)
                success = Connect_nRFTD1(device);
                */
                if (!success)
                {
                    ConnectionCanceled(this, EventArgs::Empty); // ConnectionCanceled event to MainWindow
                    return;
                }
            }
            catch (Exception^ ex)
            {
                LogErrorMessage(String::Format("Exception in OnDeviceDiscovered: {0}",
                    ex->Message), ex->StackTrace);
                ConnectionCanceled(this, EventArgs::Empty); // ConnectionCanceled event to MainWindow
            }    
        }

        void OnDeviceDiscovered(Object^ sender, Nordicsemi::ValueEventArgs<Nordicsemi::BtDevice^>^ arguments) //YYY
        {
            /* Avoid call after a connect procedure is being started,
            * and the discovery procedure hasn't yet been stopped. */
            if (connectionInProgress)
            {
                return;
            }

            //karel - Results of Scan
            Nordicsemi::BtDevice^ device = arguments->Value;
            if (!IsEligibleForConnection(device))
            {
                return;
            }

            connectionInProgress = true;

            /* Start the connection procedure in a background task to avoid 
            * blocking the event caller. */
#if 1
            /*
            nRF_TD_Kontroller^ someWork = gcnew nRF_TD_Kontroller;
            Thread^ newThread;
            newThread = 
            gcnew Thread(
            gcnew ParameterizedThreadStart(someWork, 
            &DoMoreWork));
            // Pass an object containing data for the thread.
            //
            newThread->Start("The answer.");
            */
            Thread^ newThread;
            newThread = 
                gcnew Thread(
                gcnew ParameterizedThreadStart(this,
                &nRF_TD_Kontroller::OnDeviceDiscovered_Task));
            newThread->Start( device );
#else
            //Task.Factory.StartNew(() =>
            //{
            try
            {
                Connecting(this, EventArgs::Empty); // Connecting event to MainWindow

                //bool success = Connect(device);
                bool success;
                if (USE_TD1 == false)
                    success = Connect_nRFUart(device);
                if (USE_TD1 == true)
                    success = Connect_nRFTD1(device);

                if (!success)
                {
                    ConnectionCanceled(this, EventArgs::Empty); // ConnectionCanceled event to MainWindow
                    return;
                }
            }
            catch (Exception^ ex)
            {
                LogErrorMessage(String::Format("Exception in OnDeviceDiscovered: {0}",
                    ex->Message), ex->StackTrace);
                ConnectionCanceled(this, EventArgs::Empty); // ConnectionCanceled event to MainWindow
            }
            //});
#endif
        }

        /// <summary>
        /// Check if a device has the advertising data we are looking for.
        /// </summary>
        bool IsEligibleForConnection(Nordicsemi::BtDevice^ device) //YYY
        {
            IDictionary<Nordicsemi::DeviceInfoType, String^>^ deviceInfo = device->DeviceInfo;

            karelDumpDevice(device);
            /*
            bool hasServicesCompleteAdField =
            deviceInfo->ContainsKey(Nordicsemi::DeviceInfoType::ServicesCompleteListUuid128);

            if (!hasServicesCompleteAdField)
            {
            return false;
            }
            */

            try
            {
                //#define NUS_BASE_UUID  {{0x9E, 0xCA, 0xDC, 0x24, 0x0E, 0xE5, 0xA9, 0xE0, 0x93, 0xF3, 0xA3, 0xB5, 0x00, 0x00, 0x40, 0x6E}} /**< Used vendor specific UUID. */
                //String^ bleUartUuid = "6E400001B5A3F393E0A9E50E24DCCA9E";
                String^ bleUartUuid = "6E400001B5A3F393E0A9E50E24DCCA42"; // ... 42 == T&D
                bool hasHidServiceUuid = deviceInfo[Nordicsemi::DeviceInfoType::ServicesCompleteListUuid128]->Contains(bleUartUuid);

                if (!hasHidServiceUuid)
                {
                    return false;
                }
            }
            catch( Exception ^ ex)
            {
                return false;
            }

            /* If we have reached here it means all the criterias have passed. */
            return true;
        }

        /*
        bool IsEligibleForConnection_nRFTD1(BtDevice device) //YYY
        {
        IDictionary<Nordicsemi::DeviceInfoType, String^> deviceInfo = device.DeviceInfo;

        karelDumpDevice(device);

        bool hasServicesCompleteAdField =
        deviceInfo.ContainsKey(Nordicsemi::DeviceInfoType::ServicesCompleteListUuid128);

        if (!hasServicesCompleteAdField)
        {
        return false;
        }

        //const String^ bleTD1Uuid = ".....";
        bool hasHidServiceUuid =
        deviceInfo[Nordicsemi::DeviceInfoType::ServicesCompleteListUuid128].Contains(bleTD1Uuid);

        if (!hasHidServiceUuid)
        {
        return false;
        }

        // If we have reached here it means all the criterias have passed.
        return true;
        }
        */

        /// <summary>
        /// Extract the device name from the advertising data.
        /// </summary>
        String^ GetDeviceName(IDictionary<Nordicsemi::DeviceInfoType, String^>^ deviceInfo) //YYY
        {
            String^ deviceName = String::Empty;
            bool hasNameField = deviceInfo->ContainsKey(Nordicsemi::DeviceInfoType::CompleteLocalName);
            if (hasNameField)
            {
                deviceName = deviceInfo[Nordicsemi::DeviceInfoType::CompleteLocalName];
            }
            return deviceName;
        }

        /// <summary>
        /// This event handler is called when data has been received on any of our pipes.
        /// </summary>
        void OnDataReceived(Object^ sender, Nordicsemi::PipeDataEventArgs^ arguments)
        {
            System::Text::StringBuilder^ stringBuffer = gcnew System::Text::StringBuilder();
            for each (Byte element in arguments->PipeData)
            {
                stringBuffer->AppendFormat(" 0x{0:X2}", element);
            }
            if (DebugMessagesEnabled)
            {
                AddToLog(String::Format("Data received on pipe number {0}:{1}", arguments->PipeNumber, stringBuffer->ToString()));
            }

            if (arguments->PipeNumber == pipeSetup->WctrlPipe)
                OnDataReceived_Wctrl( sender, arguments);
            if (arguments->PipeNumber == pipeSetup->RctrlPipe)
                OnDataReceived_Rctrl( sender, arguments);

            //REF if (arguments->PipeNumber == pipeSetup->UartTxPipe)
            //REF     OnDataReceived_Uart( sender, arguments);

            if (arguments->PipeNumber == pipeSetup->DcfmPipe)
                OnDataReceived_Dn( sender, arguments);
            if( arguments->PipeNumber == pipeSetup->UcmdPipe )
                OnDataReceived_Up( sender, arguments);
            if( arguments->PipeNumber == pipeSetup->UdatPipe )
                OnDataReceived_Up( sender, arguments);
        }

        void OnDataReceived_Dn(Object^ sender, Nordicsemi::PipeDataEventArgs^ arguments)
        {
            udEngine->On_Dcfm( arguments->PipeData, arguments->PipeData->Length );
        }

        void OnDataReceived_Up(Object^ sender, Nordicsemi::PipeDataEventArgs^ arguments)
        {
            if( arguments->PipeNumber == pipeSetup->UcmdPipe )
                udEngine->On_Ucmd( arguments->PipeData, arguments->PipeData->Length );
            if( arguments->PipeNumber == pipeSetup->UdatPipe )
                udEngine->On_Udat( arguments->PipeData, arguments->PipeData->Length );
        }

        void OnDataReceived_Wctrl(Object^ sender, Nordicsemi::PipeDataEventArgs^ arguments)
        {
            System::Text::StringBuilder^ stringBuffer = gcnew System::Text::StringBuilder();
            for each (Byte element in arguments->PipeData)
            {
                stringBuffer->AppendFormat(" 0x{0:X2}", element);
            }
            if (DebugMessagesEnabled)
            {
                AddToLog(String::Format("Wctrl: Data received on pipe number {0}:{1}", arguments->PipeNumber,  stringBuffer->ToString()));
            }
        }
        void OnDataReceived_Rctrl(Object^ sender, Nordicsemi::PipeDataEventArgs^ arguments)
        {
            System::Text::StringBuilder^ stringBuffer = gcnew System::Text::StringBuilder();
            for each (Byte element in arguments->PipeData)
            {
                stringBuffer->AppendFormat(" 0x{0:X2}", element);
            }
            if (DebugMessagesEnabled)
            {
                AddToLog(String::Format("Rctrl: Data received on pipe number {0}:{1}", arguments->PipeNumber,  stringBuffer->ToString()));
            }
        }

        /// <summary>
        /// This event handler is called when data has been received on any of our pipes.
        /// </summary>
        /* REF
        void OnDataReceived_Uart(Object^ sender, Nordicsemi::PipeDataEventArgs^ arguments)
        {
            if (arguments->PipeNumber != pipeSetup->UartTxPipe)
            {
                AddToLog("Received data on unknown pipe.");
                return;
            }

            System::Text::StringBuilder^ stringBuffer = gcnew System::Text::StringBuilder();
            for each (Byte element in arguments->PipeData)
            {
                stringBuffer->AppendFormat(" 0x{0:X2}", element);
            }
            if (DebugMessagesEnabled)
            {
                AddToLog(String::Format("Data received on pipe number {0}:{1}", arguments->PipeNumber,
                    stringBuffer->ToString()));
            }

            System::Text::Encoding^ enc = gcnew System::Text::UTF8Encoding();
            array<System::Byte>^ utf8Array = arguments->PipeData;  //byte[] utf8Array = arguments.PipeData;
            String^ convertedText = enc->GetString(utf8Array);     //String^ convertedText = Encoding.UTF8.GetString(utf8Array);
            AddToLog(String::Format("RX: {0}", convertedText));
        }
        REF */

        /// <summary>
        /// This event handler is called when a connection has been successfully established.
        /// </summary>
        void OnConnected_Task()
        {
            try
            {
                DiscoverPipes();
                OpenRemotePipes();
                PipeDiscoveryCompleted(this, EventArgs::Empty);
            }
            catch (Exception^ ex)
            {
                LogErrorMessage(String::Format("Exception in OnConnected: {0}", ex->Message),
                    ex->StackTrace);
            }
        }
        void OnConnected(Object^ sender, EventArgs^ arguments) //NNN
        {
            //if (Connected != nullptr)
            {
                Connected(this, EventArgs::Empty);
            }

            /* NG here
            //karel - from Documentation
            karelLog("\r\n");
            karelLog("\r\n");
            karelLog("========== TESTING masterEmulator->DiscoverServices() ================\r\n");
            karelLog("\r\n");

            var attributes = masterEmulator->DiscoverServices();
            foreach (var item in attributes)
            {
            Trace.WriteLine(item.ToString());
            karelLog("-----> " + item.ToString());
            }
            */

#if 1
            AddToLog(gcnew String("Initialize is starting Up dude") );
            System::Threading::Tasks::Task^ myTask = System::Threading::Tasks::Task::Factory->StartNew(gcnew Action(this, &nRF_TD_Kontroller::OnConnected_Task));
            // For non-static methods, specify the object.      ^^^^ 
            // Use the C++-style reference to a class method.         ^^^^^^^^^^^^^^^^^^^^
#else
            /* The connection is up, proceed with pipe discovery. 
            * Using a background task in order not to block the event caller. */
            Task.Factory.StartNew(() =>
            {
                try
                {
                    DiscoverPipes();
                    OpenRemotePipes();
                    PipeDiscoveryCompleted(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    LogErrorMessage(String^.Format("Exception in OnConnected: {0}", ex.Message),
                        ex.StackTrace);
                }
            });
#endif
        }

        /// <summary>
        /// This event handler is called when a connection update request has been received.
        /// A connection update must be responded to in two steps: sending a connection update
        /// response, and performing the actual update.
        /// </summary>
        void OnConnectionUpdateRequest_Task(/*Object^ sender, Nordicsemi::ConnectionUpdateRequestEventArgs^ arguments*/ Object^ o) //NNN
        {

            Nordicsemi::ConnectionUpdateRequestEventArgs^ arguments = static_cast<Nordicsemi::ConnectionUpdateRequestEventArgs^>(o);

            masterEmulator->SendConnectionUpdateResponse(arguments->Identifier, Nordicsemi::ConnectionUpdateResponse::Accepted);
            Nordicsemi::BtConnectionParameters^ updateParams = gcnew Nordicsemi::BtConnectionParameters();
            updateParams->ConnectionIntervalMs = arguments->ConnectionIntervalMinMs;
            updateParams->SupervisionTimeoutMs = arguments->ConnectionSupervisionTimeoutMs;
            updateParams->SlaveLatency = arguments->SlaveLatency;
            masterEmulator->UpdateConnectionParameters(updateParams);
        }
        void OnConnectionUpdateRequest(Object^ sender, Nordicsemi::ConnectionUpdateRequestEventArgs^ arguments) //NNN
        {
#if 1
            AddToLog("OnConnectionUpdateRequest: Start Thread");

            Thread^ newThread;
            newThread = 
                gcnew Thread(
                gcnew ParameterizedThreadStart(this,
                &nRF_TD_Kontroller::OnConnectionUpdateRequest_Task));
            newThread->Start( arguments );

#else
            Task.Factory.StartNew(() =>
            {
                masterEmulator->SendConnectionUpdateResponse(arguments->Identifier, Nordicsemi::ConnectionUpdateResponse::Accepted);
                Nordicsemi::BtConnectionParameters^ updateParams = gcnew Nordicsemi::BtConnectionParameters();
                updateParams->ConnectionIntervalMs = arguments->ConnectionIntervalMinMs;
                updateParams->SupervisionTimeoutMs = arguments->ConnectionSupervisionTimeoutMs;
                updateParams->SlaveLatency = arguments->SlaveLatency;
                masterEmulator->UpdateConnectionParameters(updateParams);
            });
#endif
        }

        /// <summary>
        /// This event handler is called when a connection has been terminated.
        /// </summary>
        void OnDisconnected(Object^ sender, Nordicsemi::ValueEventArgs<Nordicsemi::DisconnectReason>^ arguments) //NNN
        {
            connectionInProgress = false;
            UARTsendData = false;
            Disconnected(this, EventArgs::Empty);
        }

        /// <summary>
        /// Relay received log message events to the log method.
        /// </summary>
        void OnLogMessage(Object^ sender, Nordicsemi::ValueEventArgs<String^>^ arguments) //YYY
        {
            String^ message = arguments->Value;

            if (message->Contains("Connected to"))
            {
                /* Don't filter out */
            }
            else if (message->Contains("Disconnected"))
            {
                return;
            }
            else if (!DebugMessagesEnabled)
            {
                return;
            }

            AddToLog(String::Format("{0}", arguments->Value));
        }




        void PPP( char * message )
        {
            String^ S = gcnew String( message );

            pS_controller->AddToLog(S);
        }


    };

} //namespace nRFUart_TD


