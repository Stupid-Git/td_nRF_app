/*
// Maikon_exe.cpp : main project file.

#include "stdafx.h"

using namespace System;

int main(array<System::String ^> ^args)
{
    Console::WriteLine(L"Hello World");
    return 0;
}
*/
// Maikon_exe.cpp : main project file.

#include "stdafx.h"
#include "../Maikon/Form_Maikon.h"

using namespace System;

using namespace Maikon_ns;//MasterEm_TD2_a;

int main(array<System::String ^> ^args)
{
    Console::WriteLine(L"Hello World");
    
    // Enabling Windows XP visual effects before any controls are created
    Application::EnableVisualStyles();
    Application::SetCompatibleTextRenderingDefault(false); 

    // Create the main window and run it
    Application::Run(gcnew Form_Maikon());
    

    Console::WriteLine(L"Good Bye World");
    return 0;
}
