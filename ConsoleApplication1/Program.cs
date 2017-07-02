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
            eeipClient.IPAddress = "192.168.178.112";
            eeipClient.RegisterSession();
            Console.WriteLine("Product Name: " + eeipClient.IdentityObject.ProductName);
            Console.WriteLine("Product Code: " + eeipClient.IdentityObject.ProductCode);
            Console.WriteLine("State of Dig. Input 1 " + eeipClient.AssemblyObject.getInstance(101)[0].ToString());
            eeipClient.AssemblyObject.setInstance(100, new byte[] { 123 });

            Console.WriteLine("Path size " + eeipClient.TcpIpInterfaceObject.PhysicalLinkObject.PathSize);
            Console.ReadKey();
        }
    }
}
