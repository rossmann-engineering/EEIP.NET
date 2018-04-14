using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sres.Net.EEIP;


//This example demonstrates the usage of Implicit Messaging 
//whith an Keyence NU-EP1 Network Unit. This is an Input Only connection.
//The 128 received bytes returns the state of the Sensors (Page 3-9 of Keyence Manual contains the assignment).
//Keyence Users Manual Page 3-6 No. 2
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
            eeipClient.T_O_InstanceID = 0x64;
            eeipClient.T_O_Length = 128;
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

                //Read the Inputs Transfered form Target -> Originator
                Console.WriteLine("Sensor Value 1: " + (eeipClient.T_O_IOData[30] | eeipClient.T_O_IOData[31] << 8));
                Console.WriteLine("Sensor Value 2: " + (eeipClient.T_O_IOData[32] | eeipClient.T_O_IOData[33] << 8));



                System.Threading.Thread.Sleep(500);
            }

            //Close the Session
            eeipClient.ForwardClose();
            eeipClient.UnRegisterSession();
        }
    }
}

