# EEIP.NET
Ethernet/IP compatible library for .NET implementations
Supports IO Scanner and Explicit Message Client functionality
For Data Exchange with Ethernet/IP Devices

- Support of Explicit Messaging and Implicit Messaging
- Object Library with CIP-Definined Objects
- Provides a simple way to access Ethernet/IP Devices without special knowledge about Ethernet/IP

<a href="http://www.eeip-library.de">Implementation Guide and documentation</a>

## Installation

<a href="https://sourceforge.net/projects/eeip-net/files/latest/download" rel="nofollow"><img alt="Download EEIP.NET" src="https://a.fsdn.com/con/app/sf-download-button"></a>

## Usage

### Explicit Messaging: Write digital outputs to Ethernet/IP Device

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sres.Net.EEIP;

namespace Explicit_Messaging_Example
{
    class Program
    {
        static void Main(string[] args)
        {
            EEIPClient eeipClient = new EEIPClient();

            //Register Session (Wago-Device 750-352 IP-Address: 192.168.178.66)
            //we use the Standard Port for Ethernet/IP TCP-connections 0xAF12
            eeipClient.RegisterSession("192.168.178.66");

            //We write an Output to the Wago-Device; According to the Manual of the Device
            //Instance 0x66 of the Assembly Object contains the Digital Output data
            //The Documentation can be found at: http://www.wago.de/download.esm?file=%5Cdownload%5C00368362_0.pdf&name=m07500352_xxxxxxxx_0en.pdf

            //We set the first output "High"
            eeipClient.AssemblyObject.setInstance(0x66, new byte[] { 0x01 });

            System.Threading.Thread.Sleep(1000);

            //We set the secoond output "High"
            eeipClient.AssemblyObject.setInstance(0x66, new byte[] { 0x02 });

            System.Threading.Thread.Sleep(1000);

            //We set the secoond output "High"
            eeipClient.AssemblyObject.setInstance(0x66, new byte[] { 0x03 });

            System.Threading.Thread.Sleep(1000);

            //We reset the outputs
            eeipClient.AssemblyObject.setInstance(0x66, new byte[] { 0x00 });

            //When done, we unregister the session
            eeipClient.UnRegisterSession();
        }
    }
}
```

### Explicit Messaging: Read digital inputs from Ethernet/IP Device

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sres.Net.EEIP;

namespace Explicit_Message_Example2
{
    class Program
    {
        static void Main(string[] args)
        {
            EEIPClient eeipClient = new EEIPClient();

            //Register Session (Wago-Device 750-352 IP-Address: 192.168.178.66)
            //we use the Standard Port for Ethernet/IP TCP-connections 0xAF12
            eeipClient.RegisterSession("192.168.178.66");

            //Get the State of a digital Input According to the Manual
            //Instance 0x6C of the Assembly Object contains the Digital Input data
            //The Documentation can be found at: http://www.wago.de/download.esm?file=%5Cdownload%5C00368362_0.pdf&name=m07500352_xxxxxxxx_0en.pdf
            byte[] digitalInputs = eeipClient.AssemblyObject.getInstance(0x6c);

            Console.WriteLine("State of Digital Input 1: " + (EEIPClient.ToBool(digitalInputs[0], 0)));
            Console.WriteLine("State of Digital Input 2: " + (EEIPClient.ToBool(digitalInputs[0], 1)));
            Console.WriteLine("State of Digital Input 3: " + (EEIPClient.ToBool(digitalInputs[0], 2)));
            Console.WriteLine("State of Digital Input 4: " + (EEIPClient.ToBool(digitalInputs[0], 3)));

            //When done, we unregister the session
            eeipClient.UnRegisterSession();
            Console.ReadKey();
        }
    }
}
```

### Explicit Messaging: Read Analog Inputs from Ethernet/IP Device

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sres.Net.EEIP;

namespace Explicit_Message_Example_ReadAnalogInput
{
    class Program
    {
        static void Main(string[] args)
        {
            EEIPClient eeipClient = new EEIPClient();

            //Register Session (Wago-Device 750-352 IP-Address: 192.168.178.66)
            //we use the Standard Port for Ethernet/IP TCP-connections 0xAF12
            eeipClient.RegisterSession("192.168.178.66");

            //Get the State of Analog Inputs According to the Manual
            //Instance 0x6D of the Assembly Object contains the Analog Input data
            //The Documentation can be found at: http://www.wago.de/download.esm?file=%5Cdownload%5C00368362_0.pdf&name=m07500352_xxxxxxxx_0en.pdf
            //Page 202 shows the documentation for instance 6D hex
            byte[] analogInputs = eeipClient.AssemblyObject.getInstance(0x6D);

            Console.WriteLine("Temperature of Analog Input 1: " + (EEIPClient.ToUshort(new byte[] { analogInputs[0], analogInputs[1] }) / 10.0) + "°C");
            Console.WriteLine("Temperature of Analog Input 2: " + (EEIPClient.ToUshort(new byte[] { analogInputs[2], analogInputs[3] }) / 10.0) + "°C");

            //When done, we unregister the session
            eeipClient.UnRegisterSession();
            Console.ReadKey();
        }
    }
}
```

### Explicit Messaging: Read and Write Sensor parameters – Keyence NU-EP1

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sres.Net.EEIP;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            EEIPClient eeipClient = new EEIPClient();
            eeipClient.IPAddress = "192.168.0.123";
            eeipClient.RegisterSession();
            byte[] response = eeipClient.GetAttributeSingle(0x66, 1, 0x325);
            Console.WriteLine("Current Value Sensor 1: " + (response[1] * 255 + response[0]).ToString());
            response = eeipClient.GetAttributeSingle(0x66, 2, 0x325);
            Console.WriteLine("Current Value Sensor 2: " + (response[1] * 255 + response[0]).ToString());
            Console.WriteLine();
            Console.Write("Enter intensity for Sensor 1 [1..100]");
            int value = int.Parse(Console.ReadLine());
            Console.WriteLine("Set Light intensity Sensor 1 to "+value+"%");
            eeipClient.SetAttributeSingle(0x66, 1, 0x389,new byte [] {(byte)value,0 });
            Console.Write("Enter intensity for Sensor 2 [1..100]");
            value = int.Parse(Console.ReadLine());
            Console.WriteLine("Set Light intensity Sensor 2 to " + value + "%");
            eeipClient.SetAttributeSingle(0x66, 2, 0x389, new byte[] { (byte)value, 0 });
            Console.WriteLine();
            Console.WriteLine("Read Values from device to approve the value");
            response = eeipClient.GetAttributeSingle(0x66, 1, 0x389);
            Console.WriteLine("Current light Intensity Sensor 1 in %: " + (response[1] * 255 + response[0]).ToString());
            response = eeipClient.GetAttributeSingle(0x66, 2, 0x389);
            Console.WriteLine("Current light Intensity Sensor 2 in %: " + (response[1] * 255 + response[0]).ToString());
            eeipClient.UnRegisterSession();
            Console.ReadKey();
        }
    }
}
```

### Discover available Ethernet/IP Devices on the Network

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DescoverDevices
{
    class Program
    {
        static void Main(string[] args)
        {
            Sres.Net.EEIP.EEIPClient eipClient = new Sres.Net.EEIP.EEIPClient();
            List<Sres.Net.EEIP.Encapsulation.CIPIdentityItem> cipIdentityItem = eipClient.ListIdentity();

            for (int i = 0; i < cipIdentityItem.Count; i++)
            {
                Console.WriteLine("Ethernet/IP Device Found:");
                Console.WriteLine(cipIdentityItem[i].ProductName1);
                Console.WriteLine("IP-Address: " + Sres.Net.EEIP.Encapsulation.CIPIdentityItem.getIPAddress(cipIdentityItem[i].SocketAddress.SIN_Address));
                Console.WriteLine("Port: " + cipIdentityItem[i].SocketAddress.SIN_port);
                Console.WriteLine("Vendor ID: " + cipIdentityItem[i].VendorID1);
                Console.WriteLine("Product-code: " + cipIdentityItem[i].ProductCode1);
                Console.WriteLine("Type-Code: " + cipIdentityItem[i].ItemTypeCode);
            }
        }
    }
}
```

### Implicit Messaging connection to Allen-Bradley 1734 Point I/O

```csharp
using System;
using Sres.Net.EEIP;

//The Following Hardware Configuration is used in this example
// Allen-Bradley 1734-AENT Ethernet/IP Coupler
// Allen-Bradley 1734-IB4 4-Channel Digital Input Module
// Allen-Bradley 1734-IB4 4-Channel Digital Input Module
// Allen-Bradley 1734-IB4 4-Channel Digital Input Module
// Allen-Bradley 1734-IB4 4-Channel Digital Input Module
// Allen-Bradley 1734-OB4E 4-Channel Digital Output Module
// Allen-Bradley 1734-OB4E 4-Channel Digital Output Module
// Allen-Bradley 1734-OB4E 4-Channel Digital Output Module
// Allen-Bradley 1734-OB4E 4-Channel Digital Output Module
//IP-Address: 192.168.178.107 (By DHCP-Server)

namespace AllenBradleyPointIO
{
    class Program
    {
        static void Main(string[] args)
        {
            EEIPClient eeipClient = new EEIPClient();
            //Ip-Address of the Ethernet-IP Device (In this case Allen-Bradley 1734-AENT Point I/O)
            eeipClient.IPAddress = "192.168.178.107";
            //A Session has to be registered before any communication can be established
            eeipClient.RegisterSession();

            //Parameters from Originator -> Target
            eeipClient.O_T_InstanceID = 0x64;              //Instance ID of the Output Assembly
            eeipClient.O_T_Length = 4;                     //The Method "Detect_O_T_Length" detect the Length using an UCMM Message
            eeipClient.O_T_RealTimeFormat = Sres.Net.EEIP.RealTimeFormat.Header32Bit;   //Header Format
            eeipClient.O_T_OwnerRedundant = false;
            eeipClient.O_T_Priority = Sres.Net.EEIP.Priority.Scheduled;
            eeipClient.O_T_VariableLength = false;
            eeipClient.O_T_ConnectionType = Sres.Net.EEIP.ConnectionType.Point_to_Point;
            eeipClient.RequestedPacketRate_O_T = 500000;        //500ms is the Standard value

            //Parameters from Target -> Originator
            eeipClient.T_O_InstanceID = 0x65;
            eeipClient.T_O_Length = 16;
            eeipClient.T_O_RealTimeFormat = Sres.Net.EEIP.RealTimeFormat.Modeless;
            eeipClient.T_O_OwnerRedundant = false;
            eeipClient.T_O_Priority = Sres.Net.EEIP.Priority.Scheduled;
            eeipClient.T_O_VariableLength = false;
            eeipClient.T_O_ConnectionType = Sres.Net.EEIP.ConnectionType.Multicast;
            eeipClient.RequestedPacketRate_T_O = 500000;    //RPI in  500ms is the Standard value

            //Forward open initiates the Implicit Messaging
            eeipClient.ForwardOpen();

            while(true)
            {
                //Read the Inputs Transfered form Target -> Originator
                Console.WriteLine("State of first Input byte: " + eeipClient.T_O_IOData[8]);
                Console.WriteLine("State of second Input byte: " + eeipClient.T_O_IOData[9]);

                //write the Outputs Transfered form Originator -> Target
                eeipClient.O_T_IOData[0] = (byte)(eeipClient.O_T_IOData[0] + 1);
                eeipClient.O_T_IOData[1] = (byte)(eeipClient.O_T_IOData[1] - 1);
                eeipClient.O_T_IOData[2] = 1;
                eeipClient.O_T_IOData[3] = 8;

                System.Threading.Thread.Sleep(500);
            }

            //Close the Session
            eeipClient.ForwardClose();
            eeipClient.UnRegisterSession();
        }
    }
}
```

### Implicit Messaging connection to Turck FEN20-4DIP-4DXP

```csharp
using System;
using Sres.Net.EEIP;

//The Following Hardware Configuration is used in this example
// Turck FEN20-4DIP-4DXP
//Unisversal Digital Channels are used as Digital Outputs
// Manual: http://pdb2.turck.de/repo/media/_en/Anlagen/Datei_EDB/edb_6931090_gbr_en.pdf
//IP-Address: 192.168.1.254

namespace TurckFEN20
{
    class Program
    {
        static void Main(string[] args)
        {
            EEIPClient eeipClient = new EEIPClient();
            //Ip-Address of the Ethernet-IP Device (In this case Allen-Bradley 1734-AENT Point I/O)
            eeipClient.IPAddress = "192.168.1.254";
            //A Session has to be registered before any communication can be established
            eeipClient.RegisterSession();

            //Parameters from Originator -> Target
            eeipClient.O_T_InstanceID = 0x68;              //Instance ID of the Output Assembly
            eeipClient.O_T_Length = 4;                     //The Method "Detect_O_T_Length" detect the Length using an UCMM Message
            eeipClient.O_T_RealTimeFormat = Sres.Net.EEIP.RealTimeFormat.Header32Bit;   //Header Format
            eeipClient.O_T_OwnerRedundant = false;
            eeipClient.O_T_Priority = Sres.Net.EEIP.Priority.Scheduled;
            eeipClient.O_T_VariableLength = false;
            eeipClient.O_T_ConnectionType = Sres.Net.EEIP.ConnectionType.Point_to_Point;
            eeipClient.RequestedPacketRate_O_T = 500000;        //500ms is the Standard value

            //Parameters from Target -> Originator
            eeipClient.T_O_InstanceID = 0x67;
            eeipClient.T_O_Length = 8;
            eeipClient.T_O_RealTimeFormat = Sres.Net.EEIP.RealTimeFormat.Modeless;
            eeipClient.T_O_OwnerRedundant = false;
            eeipClient.T_O_Priority = Sres.Net.EEIP.Priority.Scheduled;
            eeipClient.T_O_VariableLength = false;
            eeipClient.T_O_ConnectionType = Sres.Net.EEIP.ConnectionType.Point_to_Point;
            eeipClient.RequestedPacketRate_T_O = 500000;    //RPI in  500ms is the Standard value

            //Forward open initiates the Implicit Messaging
            eeipClient.ForwardOpen();

            while (true)
            {
                //Read the Inputs Transfered form Target -> Originator
                Console.WriteLine("State of Input byte: " + eeipClient.T_O_IOData[2]);

                //write the Outputs Transfered form Originator -> Target
                eeipClient.O_T_IOData[2] = 0x0F;        //Set all Four digital Inputs to High

                System.Threading.Thread.Sleep(500);
            }

            //Close the Session
            eeipClient.ForwardClose();
            eeipClient.UnRegisterSession();
        }
    }
}
```

### Implicit Messaging connection to Keyence NU-EP1

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sres.Net.EEIP;


//This example demonstrates the usage of Implicit Messaging
//whith an Keyence NU-EP1 Network Unit. This is an Input Only connection.
//The two received bytes represents the output state of the sensors.
//Keyence Users Manual Page 3-6 No. 3
namespace Keyence_NU_RP1_Implicit
{
    class Program
    {
        static void Main(string[] args)
        {
            EEIPClient eeipClient = new EEIPClient();
            //Ip-Address of the Ethernet-IP Device (In this case Keyence-NU-EP1)
            eeipClient.IPAddress = "192.168.0.123";
            //A Session has to be registered before any communication can be established
            eeipClient.RegisterSession();

            //Parameters from Originator -> Target
            eeipClient.O_T_InstanceID = 0xfe;              //Instance ID of the Output Assembly
            eeipClient.O_T_Length = 0;
            eeipClient.O_T_RealTimeFormat = Sres.Net.EEIP.RealTimeFormat.Header32Bit;   //Header Format
            eeipClient.O_T_OwnerRedundant = false;
            eeipClient.O_T_Priority = Sres.Net.EEIP.Priority.Low;
            eeipClient.O_T_VariableLength = false;
            eeipClient.O_T_ConnectionType = Sres.Net.EEIP.ConnectionType.Point_to_Point;
            eeipClient.RequestedPacketRate_O_T = 500000;    //RPI in  500ms is the Standard value

            //Parameters from Target -> Originator
            eeipClient.T_O_InstanceID = 0x66;
            eeipClient.T_O_Length = 2;
            eeipClient.T_O_RealTimeFormat = Sres.Net.EEIP.RealTimeFormat.Modeless;
            eeipClient.T_O_OwnerRedundant = false;
            eeipClient.T_O_Priority = Sres.Net.EEIP.Priority.Scheduled;
            eeipClient.T_O_VariableLength = false;
            eeipClient.T_O_ConnectionType = Sres.Net.EEIP.ConnectionType.Multicast;
            eeipClient.RequestedPacketRate_T_O = 500000;    //RPI in  500ms is the Standard value

            //Forward open initiates the Implicit Messaging
            eeipClient.ForwardOpen();

            while (true)
            {
                //Read the Inputs Transfered from Target -> Originator
                Console.WriteLine("State of first Input byte: " + eeipClient.T_O_IOData[0]);
                Console.WriteLine("State of second Input byte: " + eeipClient.T_O_IOData[1]);

                System.Threading.Thread.Sleep(500);
            }

            //Close the Session
            eeipClient.ForwardClose();
            eeipClient.UnRegisterSession();
        }
    }
}
```

## License

[MIT](https://choosealicense.com/licenses/mit/)