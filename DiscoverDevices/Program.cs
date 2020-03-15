using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoverDevices
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
                Console.WriteLine("Serial Number: " + cipIdentityItem[i].SerialNumber1);


            }
            Console.ReadKey();
        }
    }
}
