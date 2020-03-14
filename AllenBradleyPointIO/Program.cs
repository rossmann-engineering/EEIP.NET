using System;
using Sres.Net.EEIP;

//The Following Hardware Configuration is used in this example
// Allen-Bradley 1734-AENT Ethernet/IP Coupler
// Allen-Bradley 1734-IB4 4-Channel Digital Input Module
// Allen-Bradley 1734-IB4 4-Channel Digital Input Module
// Allen-Bradley 1734-IB4 4-Channel Digital Input Module
// Allen-Bradley 1734-IB4 4-Channel Digital Input Module
// Allen-Bradley 1734-OB4E 4-Channel Digital Output Module
// Allen-Bradley 1734-OB4E 4-Channel Digital Output Module
// Allen-Bradley 1734-OB4E 4-Channel Digital Output Module
// Allen-Bradley 1734-OB4E 4-Channel Digital Output Module
// IP-Address: 192.168.178.107 (By DHCP-Server)
// This example also handles a reconnection procedure if the Impicit Messaging has Timed out 
// (If the Property "LastReceivedImplicitMessage" is more than one second ago)
namespace AllenBradleyPointIO
{
    class Program
    {
        static void Main(string[] args)
        {
            EEIPClient eeipClient = new EEIPClient();
            //Ip-Address of the Ethernet-IP Device (In this case Allen-Bradley 1734-AENT Point I/O)
            eeipClient.IPAddress = "192.168.178.107";
            //A Session has to be registered before any communication can be established
            eeipClient.RegisterSession();

            //Parameters from Originator -> Target
            eeipClient.O_T_InstanceID = 0x64;              //Instance ID of the Output Assembly
            eeipClient.O_T_Length = 4;                     //The Method "Detect_O_T_Length" detect the Length using an UCMM Message
            eeipClient.O_T_RealTimeFormat = Sres.Net.EEIP.RealTimeFormat.Header32Bit;   //Header Format
            eeipClient.O_T_OwnerRedundant = false;
            eeipClient.O_T_Priority = Sres.Net.EEIP.Priority.Scheduled;
            eeipClient.O_T_VariableLength = false;
            eeipClient.O_T_ConnectionType = Sres.Net.EEIP.ConnectionType.Point_to_Point;
            eeipClient.RequestedPacketRate_O_T = 500000;        //500ms is the Standard value

            //Parameters from Target -> Originator
            eeipClient.T_O_InstanceID = 0x65;
            eeipClient.T_O_Length = 16;
            eeipClient.T_O_RealTimeFormat = Sres.Net.EEIP.RealTimeFormat.Modeless;
            eeipClient.T_O_OwnerRedundant = false;
            eeipClient.T_O_Priority = Sres.Net.EEIP.Priority.Scheduled;
            eeipClient.T_O_VariableLength = false;
            eeipClient.T_O_ConnectionType = Sres.Net.EEIP.ConnectionType.Multicast;
            eeipClient.RequestedPacketRate_T_O = 500000;    //RPI in  500ms is the Standard value

            //Forward open initiates the Implicit Messaging
            eeipClient.ForwardOpen();

            while(true)
            {
                
                //Read the Inputs Transfered form Target -> Originator
                Console.WriteLine("State of first Input byte: " + eeipClient.T_O_IOData[8]);
                Console.WriteLine("State of second Input byte: " + eeipClient.T_O_IOData[9]);

                //write the Outputs Transfered form Originator -> Target
                eeipClient.O_T_IOData[0] = (byte)(eeipClient.O_T_IOData[0] + 1);
                eeipClient.O_T_IOData[1] = (byte)(eeipClient.O_T_IOData[1] - 1);
                eeipClient.O_T_IOData[2] = 1;
                eeipClient.O_T_IOData[3] = 8;

                System.Threading.Thread.Sleep(500);

                //Detect Timeout (Read last Received Message Property)
                if (DateTime.Now.Ticks > eeipClient.LastReceivedImplicitMessage.Ticks + (1000 * 10000))
                    {
                    try
                    {
                        eeipClient.ForwardClose();
                        eeipClient.UnRegisterSession();
                    
                        eeipClient.RegisterSession();
                        eeipClient.ForwardOpen();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Couldn't reconnect to Point I/O");
                    }
                    }

            }

            //Close the Session
            eeipClient.ForwardClose();
            eeipClient.UnRegisterSession();

        }
    }
}
