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
            Console.WriteLine(cipIdentityItem[0].ProductName1);

        }
    }
}
