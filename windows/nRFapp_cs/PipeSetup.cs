/* Copyright (c) 2013 Nordic Semiconductor. All Rights Reserved.
 *
 * The information contained herein is property of Nordic Semiconductor ASA.
 * Terms and conditions of usage are described in detail in NORDIC
 * SEMICONDUCTOR STANDARD SOFTWARE LICENSE AGREEMENT. 
 *
 * Licensees are granted free, non-transferable use of the information. NO
 * WARRANTY of ANY KIND is provided. This heading must NOT be removed from
 * the file.
 *
 */

using System;
using Nordicsemi;

namespace nRFUart_TD
{
    public class PipeSetup
    {
        /* Public properties for accessing discovered pipe IDs */
        public int UartRxPipe { get; private set; }
        public int UartTxPipe { get; private set; }

        
        //===== Dn =====
        //----------------------------------------
        public int DcmdPipe { get; private set; }
        public int DdatPipe { get; private set; }
        public int DcfmPipe { get; private set; }
                
        //===== Up =====
        //----------------------------------------
        public int UcmdPipe { get; private set; }
        public int UdatPipe { get; private set; }
        public int UcfmPipe { get; private set; }

        
        //===== Ctrl =====
        //----------------------------------------
        public int WctrlPipe { get; private set; }
        public int RctrlPipe { get; private set; }


        //===== ACtrl =====
        //----------------------------------------
        public int WActrlPipe { get; private set; }
        public int RActrlPipe { get; private set; }




        MasterEmulator masterEmulator;

        public PipeSetup(MasterEmulator master)
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
        public void PerformPipeSetup_nRFUart()
        {
            /* GAP service */
            BtUuid uartOverBtleUuid = new BtUuid("6e400001b5a3f393e0a9e50e24dcca9e");
            masterEmulator.SetupAddService(uartOverBtleUuid, PipeStore.Remote);

            /* UART RX characteristic (RX from peripheral's viewpoint) */
            BtUuid uartRxUuid = new BtUuid("6e400002b5a3f393e0a9e50e24dcca9e");
            int uartRxMaxLength = 20;
            byte[] uartRxData = null;
            masterEmulator.SetupAddCharacteristicDefinition(uartRxUuid, uartRxMaxLength,
                uartRxData);
            /* Using pipe type Transmit to enable write operations */
            UartRxPipe = masterEmulator.SetupAssignPipe(PipeType.Transmit);

            /* UART TX characteristic (TX from peripheral's viewpoint) */
            BtUuid UartTxUuid = new BtUuid("6e400003b5a3f393e0a9e50e24dcca9e");
            int uartTxMaxLength = 20;
            byte[] uartTxData = null;
            masterEmulator.SetupAddCharacteristicDefinition(UartTxUuid, uartTxMaxLength,
                uartTxData);
            /* Using pipe type Receive to enable notify operations */
            UartTxPipe = masterEmulator.SetupAssignPipe(PipeType.Receive);
        }


        public void PerformPipeSetup()
        {
            // GAP service 
            BtUuid uartOverBtleUuid = new BtUuid("6e400001b5a3f393e0a9e50e24dcca42");
            masterEmulator.SetupAddService(uartOverBtleUuid, PipeStore.Remote);

            //===== Dn =====
            // DCMD characteristic (Down Link Command 0x0002) 
            BtUuid DcmdUuid = new BtUuid("6e400002b5a3f393e0a9e50e24dcca42");
            int DcmdMaxLength = 20;
            byte[] DcmdData = null;
            masterEmulator.SetupAddCharacteristicDefinition(DcmdUuid, DcmdMaxLength, DcmdData);
            // Using pipe type Transmit to enable write operations 
            DcmdPipe = masterEmulator.SetupAssignPipe(PipeType.Transmit);

            // DDAT characteristic (Down Link Data 0x0003) 
            BtUuid DdatUuid = new BtUuid("6e400003b5a3f393e0a9e50e24dcca42");
            int DdatMaxLength = 20;
            byte[] DdatData = null;
            masterEmulator.SetupAddCharacteristicDefinition(DdatUuid, DdatMaxLength, DdatData);
            // Using pipe type Receive to enable notify operations 
            DdatPipe = masterEmulator.SetupAssignPipe(PipeType.Transmit);

            // DCFM characteristic (Down Link Confirm 0x0004)
            BtUuid DcfmUuid = new BtUuid("6e400004b5a3f393e0a9e50e24dcca42");
            int DcfmMaxLength = 20;
            byte[] DcfmData = null;
            masterEmulator.SetupAddCharacteristicDefinition(DcfmUuid, DcfmMaxLength, DcfmData);
            // Using pipe type Receive to enable notify operations 
            DcfmPipe = masterEmulator.SetupAssignPipe(PipeType.Receive);

            //===== Up =====
            // UCMD characteristic (Up Link Command 0x0005) 
            BtUuid UcmdUuid = new BtUuid("6e400005b5a3f393e0a9e50e24dcca42");
            int UcmdMaxLength = 20;
            byte[] UcmdData = null;
            masterEmulator.SetupAddCharacteristicDefinition(UcmdUuid, UcmdMaxLength, UcmdData);
            // Using pipe type Transmit to enable write operations 
            UcmdPipe = masterEmulator.SetupAssignPipe(PipeType.Receive);

            // DDAT characteristic (Up Link Data 0x0006) 
            BtUuid UdatUuid = new BtUuid("6e400006b5a3f393e0a9e50e24dcca42");
            int UdatMaxLength = 20;
            byte[] UdatData = null;
            masterEmulator.SetupAddCharacteristicDefinition(UdatUuid, UdatMaxLength, UdatData);
            // Using pipe type Receive to enable notify operations 
            UdatPipe = masterEmulator.SetupAssignPipe(PipeType.Receive);

            // UCFM characteristic (Up Link Confirm 0x0007)
            BtUuid UcfmUuid = new BtUuid("6e400007b5a3f393e0a9e50e24dcca42");
            int UcfmMaxLength = 20;
            byte[] UcfmData = null;
            masterEmulator.SetupAddCharacteristicDefinition(UcfmUuid, UcfmMaxLength, UcfmData);
            // Using pipe type Receive to enable notify operations 
            UcfmPipe = masterEmulator.SetupAssignPipe(PipeType.Transmit);


        }

        public void PerformPipeSetup_TDctrl()
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
}
