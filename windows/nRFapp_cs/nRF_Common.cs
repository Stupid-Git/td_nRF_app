using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Management;

namespace nRF_Common
{
    class ComPortUtil
    {
        class ComPortEntity
        {
            public String did = "";       // DeviceID：   USB\VID_1366&PID_1015&MI_00\9&2EB0DD94&0&0000
            public String pnp = "";       // PNPDeviceID：USB\VID_1366&PID_1015&MI_00\9&2EB0DD94&0&0000
            public String deviceName = "";// Name：       JLink CDC UART Port (COM55)

            /*
            Name：       JLink CDC UART Port (COM55)
            DeviceID：   USB\VID_1366&PID_1015&MI_00\9&2EB0DD94&0&0000
            PNPDeviceID：USB\VID_1366&PID_1015&MI_00\9&2EB0DD94&0&0000
            */
            public bool is_nRF()
            {
                if (pnp.IndexOf("VID_1366&PID_1015") != -1)
                    return (true);
                else
                    return (false);
            }
            public bool is_MCB()
            {
                if ((pnp.IndexOf("VID_C251&PID_2505") != -1) || (pnp.IndexOf("VID_0CCF&PID_0801") != -1))
                    return (true);
                else
                    return (false);
            }

            public int getPortNumber()
            {
                int sPort = 0;
                int index;
                index = deviceName.IndexOf("(COM");
                if (index != -1)
                {
                    String sub = deviceName.Substring(index + 4);
                    if (sub.Count() >= 2)
                    {
                        if (sub[1] == ')')
                            sub = sub.Substring(0, 1); // (COM1), (COM4)
                        else
                            sub = sub.Substring(0, 2); // (COM11), (COM42)
                    }
                    sPort = System.Convert.ToInt32( sub );
                    //sPort = System.Convert.ToInt32(deviceName.Substring(index + 4));
                    Console.WriteLine(sPort);
                }
                return (sPort);
            }

            public String getPortName()
            {
                int sPort = 0;
                
                sPort = getPortNumber();
                if (sPort == 0)
                    return ("");
                
                String namePort = "COM" + String.Format("{0}", sPort);

                return (namePort);
            }
        }


        public ComPortUtil()
        {
        }

        List<ComPortEntity> comPortList = null;

        void comPortList_Clear()
        {
            if (comPortList == null)
                comPortList = new List<ComPortEntity>();
            comPortList.Clear();
        }
        void comPortList_Add(String deviceName, String did, String pnp)
        {
            if (comPortList == null)
                comPortList = new List<ComPortEntity>();

            ComPortEntity newPort = new ComPortEntity();
            newPort.deviceName = deviceName;
            newPort.did = did;
            newPort.pnp = pnp;

            //TODO search for instance of the same entry before adding.
            comPortList.Add(newPort);
        }


        //---------------------------------------------------------------------
        public int nRF_PortCount()
        {
            int count = 0;
            if (comPortList == null)
                return (0);
            foreach (ComPortEntity e in comPortList)
            {
                if (e.is_nRF() )
                {
                    count++;
                }
            }
            return (count);
        }

        public string[] nRF_PortNames()
        {
            String[] portNames;
            int index = 0;
            int count = 0;

            count = nRF_PortCount();
            portNames = new String[count];
            if (count == 0)
                return (portNames);
            foreach (ComPortEntity e in comPortList)
            {
                if (e.is_nRF())
                {
                    portNames[index++] = e.getPortName();
                }
            }
            return (portNames);
        }

        //---------------------------------------------------------------------
        public int MCB_PortCount()
        {
            int count = 0;
            if (comPortList == null)
                return (0);
            foreach (ComPortEntity e in comPortList)
            {
                if (e.is_MCB())
                {
                    count++;
                }
            }
            return (count);
        }

        public string[] MCB_PortNames()
        {
            String[] portNames;
            int index = 0;
            int count = 0;

            count = MCB_PortCount();
            portNames = new String[count];
            if (count == 0)
                return (portNames);
            foreach (ComPortEntity e in comPortList)
            {
                if (e.is_MCB())
                {
                    portNames[index++] = e.getPortName();
                }
            }
            return (portNames);
        }

        public int enumeratePorts()
        {

            //nnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnn
            //nnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnn
            //http://stackoverflow.com/questions/11458835/finding-information-about-all-serial-devices-connected-through-usb-in-c-sharp
            String did = "";
            String pnp = "";
            String deviceName = "";

            int count = 0;

            comPortList_Clear();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "root\\CIMV2",
                "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\""
                );
            foreach (ManagementObject queryObj in searcher.Get())
            {
                // do what you like with the Win32_PnpEntity
                //queryObj->get...
                deviceName = (String)queryObj.GetPropertyValue("Name");
                did = (String)queryObj.GetPropertyValue("DeviceID");       // ex.COM3
                pnp = (String)queryObj.GetPropertyValue("PNPDeviceID");    // ex.USB\VID_0CCF&PID_DE00\0001

                Console.WriteLine("Name：       " + deviceName);
                Console.WriteLine("DeviceID：   " + did);
                Console.WriteLine("PNPDeviceID：" + pnp);

                comPortList_Add(deviceName, did, pnp);
                count++;
            }
            Console.WriteLine();

            return (count);
        }


        //********************************************************************/
        //********************************************************************/
        //********************************************************************/
        public int serialPortSearch()
        {

            //nnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnn
            //nnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnn
            //http://stackoverflow.com/questions/11458835/finding-information-about-all-serial-devices-connected-through-usb-in-c-sharp
            String did = "";
            String pnp = "";
            String deviceName = "";

            int sPort = 0;

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "root\\CIMV2",
                "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\""
                );
            foreach (ManagementObject queryObj in searcher.Get())
            {            
                // do what you like with the Win32_PnpEntity
                //queryObj->get...
                deviceName = (String)queryObj.GetPropertyValue("Name"); 
                did = (String)queryObj.GetPropertyValue("DeviceID");       // ex.COM3
                pnp = (String)queryObj.GetPropertyValue("PNPDeviceID");    // ex.USB\VID_0CCF&PID_DE00\0001

                Console.WriteLine("Name：       " + deviceName);
                Console.WriteLine("DeviceID：   " + did);
                Console.WriteLine("PNPDeviceID：" + pnp);

                comPortList_Add(deviceName, did, pnp);
            }
            Console.WriteLine();

            // The stackoverflow article also mentions 
            //   http://www.codeproject.com/Articles/32330/A-Useful-WMI-Tool-How-To-Find-USB-to-Serial-Adapto
            // which introduces WMICodeCreator
            /*
            Name：       JLink CDC UART Port (COM55)
            DeviceID：   USB\VID_1366&PID_1015&MI_00\9&2EB0DD94&0&0000
            PNPDeviceID：USB\VID_1366&PID_1015&MI_00\9&2EB0DD94&0&0000
            
            Name：       JLink CDC UART Port (COM56)
            DeviceID：   USB\VID_1366&PID_1015&MI_00\9&3B690065&0&0000
            PNPDeviceID：USB\VID_1366&PID_1015&MI_00\9&3B690065&0&0000
            The thread '<No Name>' (0x1718) has exited with code 0 (0x0).
             
            Name：       USB Serial Port (COM41)
            DeviceID：   FTDIBUS\VID_0403+PID_6001+AL00F4XXA\0000
            PNPDeviceID：FTDIBUS\VID_0403+PID_6001+AL00F4XXA\0000
             
            Name：       TandD General UsbUart Port (COM4)
            DeviceID：   USB\VID_0CCF&PID_0577\0001
            PNPDeviceID：USB\VID_0CCF&PID_0577\0001
             
            Name：       MCB1800 USB VCOM Port (COM61)
            DeviceID：   USB\VID_C251&PID_2505&MI_00\8&56AAAE5&0&0000
            PNPDeviceID：USB\VID_C251&PID_2505&MI_00\8&56AAAE5&0&0000
            */

            //return(0);

            //nnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnn
            //nnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnn
            System.Management.ManagementClass sp = new ManagementClass("Win32_SerialPort");

            /**/
            //using (System.Management.ManagementClass^ sp = gcnew ManagementClass("Win32_SerialPort") )
            {
                foreach (ManagementObject p in (sp.GetInstances()) )
                {
                    did = (String)p.GetPropertyValue("DeviceID");       // ex.COM3
                    pnp = (String)p.GetPropertyValue("PNPDeviceID");    // ex.USB\VID_0CCF&PID_DE00\0001

                    Console.WriteLine("DeviceID：" + did);
                    Console.WriteLine("PNPDeviceID：" + pnp);

                    if ((pnp.IndexOf("VID_C251&PID_2505") != -1) || (pnp.IndexOf("VID_0CCF&PID_0801") != -1))
                    {
                        sPort = System.Convert.ToInt32(did.Substring(3));
                        Console.WriteLine(sPort);
                        break;
                    }
                }
            }
            //*/
            return sPort;
            /*
            DeviceID：COM4
            PNPDeviceID：USB\VID_0CCF&PID_0577\0001
            DeviceID：COM61                
            PNPDeviceID：USB\VID_C251&PID_2505&MI_00\8&56AAAE5&0&0000
            61
            */


        }

        
        /* C#*/
        private int serialPortSearch_1()
        {
            string did = "", pnp = "";

            int sPort = 0;


            using (ManagementClass sp = new ManagementClass("Win32_SerialPort"))
            {
                foreach (ManagementObject p in sp.GetInstances())
                {
                    did = (string)p.GetPropertyValue("DeviceID");       // ex.COM3
                    pnp = (string)p.GetPropertyValue("PNPDeviceID");    // ex.USB\VID_0CCF&PID_DE00\0001

                    Console.WriteLine("DeviceID：" + did);
                    Console.WriteLine("PNPDeviceID：" + pnp);

                    if ((pnp.IndexOf("VID_C251&PID_2505") != -1) || (pnp.IndexOf("VID_0CCF&PID_0801") != -1))
                    {
                        sPort = System.Convert.ToInt32(did.Substring(3));
                        Console.WriteLine(sPort);
                        break;
                    }
                }
            }

            return sPort;
        }
        /*C# */

    }
}
