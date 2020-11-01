using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sres.Net.EEIP;

namespace Fanuc_R30iA_Implicit
{
    class Program
    {
        static void Main(string[] args)
        {
            EEIPClient eeipClient = new EEIPClient();
            eeipClient.IPAddress = "192.168.1.254";
            //A Session has to be registered before any communication can be established
            eeipClient.RegisterSession();

            //Parameters from Originator -> Target
            eeipClient.O_T_InstanceID = 151;              //Instance ID of the Output Assembly
            eeipClient.O_T_Length = 4;                     //The Method "Detect_O_T_Length" detect the Length using an UCMM Message
            eeipClient.O_T_RealTimeFormat = Sres.Net.EEIP.RealTimeFormat.Header32Bit;   //Header Format
            eeipClient.O_T_OwnerRedundant = false;
            eeipClient.O_T_Priority = Sres.Net.EEIP.Priority.Scheduled;
            eeipClient.O_T_VariableLength = false;
            eeipClient.O_T_ConnectionType = Sres.Net.EEIP.ConnectionType.Point_to_Point;
            eeipClient.RequestedPacketRate_O_T = 500000;        //500ms is the Standard value

            //Parameters from Target -> Originator
            eeipClient.T_O_InstanceID = 101;
            eeipClient.T_O_Length = 4;
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
                Console.WriteLine(eeipClient.T_O_IOData);

                //write the Outputs Transfered form Originator -> Target
                //eeipClient.O_T_IOData[2] = 0x0F;        //Set all Four digital Inputs to High

                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
