using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sres.Net.EEIP.ObjectLibrary;

namespace ConsoleApplication1
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



            UInt32 sessionHandle =  eipClient.RegisterSession("192.168.178.66", 0xAF12);
            //            eipClient.O_T_ConnectionType = Sres.Net.EEIP.ConnectionType.Null;
            //            eipClient.O_T_Length = 0;
            eipClient.O_T_InstanceID = 0x65;
            eipClient.O_T_Length = 1;//7;
            eipClient.O_T_RealTimeFormat = Sres.Net.EEIP.RealTimeFormat.Header32Bit;
            eipClient.O_T_OwnerRedundant = false;
            eipClient.O_T_Priority = Sres.Net.EEIP.Priority.High;
            eipClient.O_T_VariableLength = false;
            eipClient.O_T_ConnectionType = Sres.Net.EEIP.ConnectionType.Point_to_Point;
            eipClient.RequestedPacketRate_O_T = 1000000;
            eipClient.T_O_InstanceID = 0x68;
            eipClient.T_O_Length = 2;//4
            eipClient.T_O_RealTimeFormat = Sres.Net.EEIP.RealTimeFormat.Modeless;
            eipClient.T_O_OwnerRedundant = false;
            eipClient.T_O_Priority = Sres.Net.EEIP.Priority.High;
            eipClient.T_O_VariableLength = false;
            eipClient.T_O_ConnectionType = Sres.Net.EEIP.ConnectionType.Point_to_Point;
            eipClient.RequestedPacketRate_T_O = 1000000;
            eipClient.ForwardOpen();

            //            eipClient.setAttributeSingle(0x4, 102, 3, new byte[] { 0xff});
            Console.WriteLine("State of digital Input #1: "+(eipClient.AssemblyObject.getInstance(0x68)[0]));
            Console.WriteLine("State of digital Input #1: " + eipClient.GetAttributeSingle (0x65, 2,1)[0]);
            Sres.Net.EEIP.ObjectLibrary.MessageRouterObject.ObjectListStruct objectList = eipClient.MessageRouterObject.ObjectList;
            Console.WriteLine("Number of supported objects: " + objectList.Number);
            for (int i = 0; i < objectList.Number; i++)
            {
                Console.WriteLine("Supported Object " + objectList.Classes[i]);
            }

            Console.WriteLine(Sres.Net.EEIP.Encapsulation.CIPIdentityItem.getIPAddress(cipIdentityItem[0].SocketAddress.SIN_Address));
            Console.WriteLine(eipClient.IdentityObject.VendorID);
            Console.WriteLine(eipClient.IdentityObject.DeviceType);
            Console.WriteLine(eipClient.IdentityObject.ProductCode);
            Console.WriteLine("Major Revision: " + eipClient.IdentityObject.Revision.MajorRevision);
            Console.WriteLine("Minor Revision: " + eipClient.IdentityObject.Revision.MinorRevision);
            Console.WriteLine("Device Status: " + eipClient.IdentityObject.Status);
            Console.WriteLine("Serial Number: " + eipClient.IdentityObject.SerialNumber);
            Console.WriteLine("Product Name: " + eipClient.IdentityObject.ProductName);
            //      eipClient.UnRegisterSession();
            //Console.WriteLine("Revision: " + eipClient.IdentityObject.AllClassAttributes.Revision);
            //Console.WriteLine("State: " + eipClient.IdentityObject.State);
            //Console.WriteLine("supported language" + eipClient.IdentityObject.SupportedLanguageList[0]);

            for (int i = 0; i < 3; i++ )
            {
                eipClient.O_T_IOData[0] = 1;
                System.Threading.Thread.Sleep(1000);
                eipClient.O_T_IOData[0] = 2;
                System.Threading.Thread.Sleep(1000);
                eipClient.O_T_IOData[0] = 3;
                System.Threading.Thread.Sleep(1000);
                eipClient.O_T_IOData[0] = 0;
                System.Threading.Thread.Sleep(1000);


            }
            eipClient.ForwardClose();
            System.Threading.Thread.Sleep(1000);
            eipClient.ForwardOpen();


            Console.ReadKey();
        }
    }
}
