// nRFapp_cli.cpp : main project file.

#include "stdafx.h"

#include "MainWindow.h"

using namespace System;
using namespace nRFapp_cli;

int main(array<System::String ^> ^args)
{
    Console::WriteLine(L"Hello World");
    
    // Enabling Windows XP visual effects before any controls are created
    Application::EnableVisualStyles();
    Application::SetCompatibleTextRenderingDefault(false); 

    // Create the main window and run it
    Application::Run(gcnew MainWindow_CLI());
    

    Console::WriteLine(L"Good Bye World");
    return 0;
}
