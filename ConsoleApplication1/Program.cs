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
            eeipClient.IPAddress = "192.168.178.107";
            eeipClient.RegisterSession();
            Console.WriteLine("Product Name: " + eeipClient.IdentityObject.ProductName);
            Console.WriteLine("Product Code: " + eeipClient.IdentityObject.ProductCode);
            byte[] data =eeipClient.GetAttributeSingle(0x4, 0x65,3);
            for (int i = 0; i < data.Length; i++)
                Console.WriteLine(data[i]);
            Console.ReadKey();
            UInt32 sessionHandle = eeipClient.RegisterSession("192.168.178.107", 0xAF12);
            //            eipClient.O_T_ConnectionType = Sres.Net.EEIP.ConnectionType.Null;
            //            eipClient.O_T_Length = 0;


            eeipClient.O_T_InstanceID = 0x64;
            eeipClient.O_T_Length = eeipClient.Detect_O_T_Length();
            eeipClient.O_T_RealTimeFormat = Sres.Net.EEIP.RealTimeFormat.Header32Bit;
            eeipClient.O_T_OwnerRedundant = false;
            eeipClient.O_T_Priority = Sres.Net.EEIP.Priority.Scheduled;
            eeipClient.O_T_VariableLength = false;
            eeipClient.O_T_ConnectionType = Sres.Net.EEIP.ConnectionType.Point_to_Point;

            eeipClient.T_O_InstanceID = 0x65;
            eeipClient.T_O_Length = eeipClient.Detect_T_O_Length();
            eeipClient.T_O_RealTimeFormat = Sres.Net.EEIP.RealTimeFormat.Modeless;
            eeipClient.T_O_OwnerRedundant = false;
            eeipClient.T_O_Priority = Sres.Net.EEIP.Priority.Scheduled;
            eeipClient.T_O_VariableLength = false;
            eeipClient.T_O_ConnectionType = Sres.Net.EEIP.ConnectionType.Multicast;
            eeipClient.ForwardOpen();
            while (true)
            {
                Console.Write(eeipClient.LastReceivedImplicitMessage);
                Console.WriteLine(eeipClient.T_O_IOData[8]);
                eeipClient.O_T_IOData[0] = (byte)((byte)eeipClient.O_T_IOData[0] + (byte)1);
                eeipClient.O_T_IOData[1] = (byte)((byte)eeipClient.O_T_IOData[1] - (byte)1);
                System.Threading.Thread.Sleep(500);
            }



            Console.ReadKey();
            eeipClient.ForwardClose();
            eeipClient.UnRegisterSession();
        }
    }
}
