using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Sres.Net.EEIP.EEIPClient eipClient = new Sres.Net.EEIP.EEIPClient();
            List<Sres.Net.EEIP.Encapsulation.CIPIdentityItem> cipIdentityItem = eipClient.ListIdentity();
            UInt32 sessionHandle =  eipClient.RegisterSession("192.168.178.66", 0xAF12);
            eipClient.setAttributeSingle(0x4, 102, 3, new byte[] { 0xff});
            Console.WriteLine(eipClient.AssemblyObject.getInstance(105));
            Console.WriteLine(eipClient.MessageRouterObject.ObjectList.Number);
            Console.WriteLine(Sres.Net.EEIP.Encapsulation.CIPIdentityItem.getIPAddress(cipIdentityItem[0].SocketAddress.SIN_Address));
            Console.WriteLine(eipClient.IdentityObject.VendorID);
            Console.WriteLine(eipClient.IdentityObject.DeviceType);
            Console.WriteLine(eipClient.IdentityObject.ProductCode);
            Console.WriteLine("Major Revision: " + eipClient.IdentityObject.Revision.MajorRevision);
            Console.WriteLine("Minor Revision: " + eipClient.IdentityObject.Revision.MinorRevision);
            Console.WriteLine("Device Status: " + eipClient.IdentityObject.Status);
            Console.WriteLine("Serial Number: " + eipClient.IdentityObject.SerialNumber);
            Console.WriteLine("Product Name: " + eipClient.IdentityObject.ProductName);
            eipClient.UnRegisterSession();
            //Console.WriteLine("Revision: " + eipClient.IdentityObject.AllClassAttributes.Revision);
            //Console.WriteLine("State: " + eipClient.IdentityObject.State);
            //Console.WriteLine("supported language" + eipClient.IdentityObject.SupportedLanguageList[0]);
        }
    }
}
