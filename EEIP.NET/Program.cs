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
            
            UInt32 sessionHandle =  eipClient.RegisterSession("192.168.178.112", 0xAF12);
            //            eipClient.O_T_ConnectionType = Sres.Net.EEIP.ConnectionType.Null;
            //            eipClient.O_T_Length = 0;
           
            
            eipClient.O_T_InstanceID = 100;
            eipClient.O_T_Length = 504;//7;
            eipClient.O_T_RealTimeFormat = Sres.Net.EEIP.RealTimeFormat.Header32Bit;
            eipClient.O_T_OwnerRedundant = false;
            eipClient.O_T_Priority = Sres.Net.EEIP.Priority.Scheduled;
            eipClient.O_T_VariableLength = true;
            eipClient.O_T_ConnectionType = Sres.Net.EEIP.ConnectionType.Point_to_Point;
            eipClient.T_O_InstanceID = 101;
            eipClient.T_O_Length = 504;
            eipClient.T_O_RealTimeFormat = Sres.Net.EEIP.RealTimeFormat.Header32Bit;
            eipClient.T_O_OwnerRedundant = false;
            eipClient.T_O_Priority = Sres.Net.EEIP.Priority.Scheduled;
            eipClient.T_O_VariableLength = true;
            eipClient.T_O_ConnectionType = Sres.Net.EEIP.ConnectionType.Point_to_Point;
            eipClient.ForwardOpen();
            
            for (int i = 0; i < 3; i++ )
            {
                eipClient.O_T_IOData[0] = 1;
                System.Threading.Thread.Sleep(1000);
                eipClient.O_T_IOData[0] = 2;
                System.Threading.Thread.Sleep(1000);
                eipClient.O_T_IOData[0] = 3;
                System.Threading.Thread.Sleep(1000);
                eipClient.O_T_IOData[0] = 3;
                System.Threading.Thread.Sleep(1000);


            }
            eipClient.O_T_IOData[503] = 3;
            Console.ReadKey();
            Console.WriteLine("State if Digitalinput 1: " + eipClient.T_O_IOData[0]);
            eipClient.ForwardClose();
            System.Threading.Thread.Sleep(1000);
     


            Console.ReadKey();
        }
    }
}
