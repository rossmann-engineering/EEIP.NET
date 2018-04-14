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
