using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sres.Net.EEIP;

namespace Fanuc_R30iA
{
    class Program
    {
        static void Main(string[] args)
        {
            //https://www.youtube.com/watch?v=fg58H-ZZit0
            EEIPClient eeipClient = new EEIPClient();

            eeipClient.RegisterSession("192.168.1.3");

            byte[] readAllRegisters = eeipClient.GetAttributeAll(0x6b, 1);

            Console.WriteLine(readAllRegisters);


            //When done, we unregister the session
            eeipClient.UnRegisterSession();
            Console.ReadKey();
        }
    }
}
