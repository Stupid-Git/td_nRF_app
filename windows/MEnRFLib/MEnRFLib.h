// MEnRFLib.h

#pragma once

using namespace System;

namespace MEnRFLib {

	public ref class MEnRFapp
	{
		
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

	};
}
