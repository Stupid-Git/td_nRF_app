using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace TDnRF
{
    using System;
    using Nordicsemi;

    public class PipeSetup_Ctrl
    {
        MasterEmulator masterEmulator;
        public PipeSetup_Ctrl(MasterEmulator master)
        {
            masterEmulator = master;
        }

        //===== Ctrl =====
        //----------------------------------------
        public int WctrlPipe { get; private set; }
        public int RctrlPipe { get; private set; }


        //===== ACtrl =====
        //----------------------------------------
        public int WActrlPipe { get; private set; }
        public int RActrlPipe { get; private set; }


        public void PerformPipeSetup_Ctrl()
        {
            // GAP service 
            BtUuid TDctrl_OverBtleUuid = new BtUuid("6e400001b5a3f393e0a9e50e24dcca43");
            masterEmulator.SetupAddService(TDctrl_OverBtleUuid, PipeStore.Remote);

            //===== Ctrl =====
            // WCTRL characteristic (Write Link Command 0x00xx) 
            BtUuid WctrlUuid = new BtUuid("6e400008b5a3f393e0a9e50e24dcca43");
            int WctrlMaxLength = 20;
            byte[] WctrlData = null;
            masterEmulator.SetupAddCharacteristicDefinition(WctrlUuid, WctrlMaxLength, WctrlData);
            // Using pipe type Transmit to enable write operations 
            //DcmdPipe = masterEmulator.SetupAssignPipe(PipeType.Transmit);
            WctrlPipe = masterEmulator.SetupAssignPipe(PipeType.Transmit);

            // RCTRL characteristic (Read Link Command 0x00xx) 
            BtUuid RctrlUuid = new BtUuid("6e400009b5a3f393e0a9e50e24dcca43");
            int RctrlMaxLength = 20;
            byte[] RctrlData = null;
            masterEmulator.SetupAddCharacteristicDefinition(RctrlUuid, RctrlMaxLength, RctrlData);
            // Using pipe type Transmit to enable write operations 
            RctrlPipe = masterEmulator.SetupAssignPipe(PipeType.ReceiveRequest);
            //RctrlPipe = masterEmulator.SetupAssignPipe(PipeType.Receive); NG


            //===== ACtrl =====
            // WACTRL characteristic (Write Link Command 0x00xx) 
            BtUuid WActrlUuid = new BtUuid("6e40000ab5a3f393e0a9e50e24dcca43");
            int WActrlMaxLength = 20;
            byte[] WActrlData = null;
            masterEmulator.SetupAddCharacteristicDefinition(WActrlUuid, WActrlMaxLength, WActrlData);
            // Using pipe type Transmit to enable write operations 
            WActrlPipe = masterEmulator.SetupAssignPipe(PipeType.Transmit);

            // RACTRL characteristic (Read Link Command 0x00xx) 
            BtUuid RActrlUuid = new BtUuid("6e40000bb5a3f393e0a9e50e24dcca43");
            int RActrlMaxLength = 20;
            byte[] RActrlData = null;
            masterEmulator.SetupAddCharacteristicDefinition(RActrlUuid, RActrlMaxLength, RActrlData);
            // Using pipe type Transmit to enable write operations 
            RActrlPipe = masterEmulator.SetupAssignPipe(PipeType.ReceiveRequest);
        }

    }

    public class CtrlEngine
    {
        
        public    Nordicsemi.MasterEmulator masterEmulator;
        //PPP PipeSetup pipeSetup;
        PipeSetup_Ctrl pipeSetup;

        public void CtrlEngine_Setup(Nordicsemi.MasterEmulator master)//PPP , PipeSetup pipe)
        {
            masterEmulator = master;
            //PPP pipeSetup = pipe;
            pipeSetup = new PipeSetup_Ctrl(master);
        }

        public void PerformPipeSetup()
        {
            //this.pipeSetup.PerformPipeSetup(); TODO  
            this.pipeSetup.PerformPipeSetup_Ctrl(); //TODO change it generic PerformPipeSetup
        }


        public CtrlEngine()
        {
        }

        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        Int32 UpDn_Write_Wctrl_test( Byte [] p_buf, int len )//array<System::Byte,1> p_buf, int len)
        {
            bool bret;
            Int32 r;
            r = 0;

            Console.WriteLine("UpDn_Write_Wctrl_test...");
            bret = masterEmulator.SendData(pipeSetup.WctrlPipe, p_buf);
            if(bret == true)
            {
                Console.WriteLine("UpDn_Write_Wctrl_test: TRUE");
            }
            if(bret == false)
            {
                Console.WriteLine("UpDn_Write_Wctrl_test: FALSE");
            }

            return(r);
        }

        Int32 UpDn_Read_Rctrl_test()//(array<System::Byte,1> p_buf, int len)
        {
            Int32 i;
            Int32 r;
            r = 0;
            Byte [] r_buf; //array<System::Byte,1> r_buf;


            Console.WriteLine("UpDn_Read_Rctrl_test...");
            r_buf = masterEmulator.RequestData(pipeSetup.RctrlPipe);
            if(r_buf == null)//nullptr)
            {
                Console.WriteLine("UpDn_Read_Rctrl_test: r_buf == nullptr");
            }

            Console.WriteLine("r_buf.Length = {0}", r_buf.Length);
            if( r_buf.Length > 0)
            {
                for(i=0 ; i<r_buf.Length; i++)
                    Console.Write(" {0:X2}", r_buf[i]);
                Console.WriteLine("");

            }
            return(r);
        }

        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------
        public Int32 On_Wctrl(byte[] buf, int len) // array<System::Byte,1> buf, int len )
        {
            /*
            System.Text.StringBuilder stringBuffer = new System.Text.StringBuilder();
            foreach (Byte element in arguments.PipeData)
            {
                stringBuffer.AppendFormat(" 0x{0:X2}", element);
            }
            if (DebugMessagesEnabled)
            {
                AddToLog(String.Format("Wctrl: Data received on pipe number {0}:{1}", arguments.PipeNumber,  stringBuffer.ToString()));
            }
            */
            return (42);
        }
        public Int32 On_Rctrl(byte[] buf, int len) // array<System::Byte,1> buf, int len )
        {
            /*
            System.Text.StringBuilder stringBuffer = new System.Text.StringBuilder();
            foreach (Byte element in arguments.PipeData)
            {
                stringBuffer.AppendFormat(" 0x{0:X2}", element);
            }
            if (DebugMessagesEnabled)
            {
                AddToLog(String.Format("Rctrl: Data received on pipe number {0}:{1}", arguments.PipeNumber,  stringBuffer.ToString()));
            }
            */
            return (42);
        }



        public Int32 OnAnyPipe(Object sender, Nordicsemi.PipeDataEventArgs arguments)
        {
            if (arguments.PipeNumber == pipeSetup.WctrlPipe)
                OnDataReceived_Wctrl(sender, arguments);
            if (arguments.PipeNumber == pipeSetup.RctrlPipe)
                OnDataReceived_Rctrl(sender, arguments);
            return (42);
        }
        
        void OnDataReceived_Wctrl(Object sender, Nordicsemi.PipeDataEventArgs arguments)
        {
            if (arguments.PipeNumber == pipeSetup.WctrlPipe)
                On_Wctrl(arguments.PipeData, arguments.PipeData.Length);
        }
        void OnDataReceived_Rctrl(Object sender, Nordicsemi.PipeDataEventArgs arguments)
        {
            if (arguments.PipeNumber == pipeSetup.RctrlPipe)
                On_Rctrl(arguments.PipeData, arguments.PipeData.Length);
        }
        
    }
}
