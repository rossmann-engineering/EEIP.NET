using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Sres.Net.EEIP
{
    class EEIPClient
    {
        /// <summary>
        /// List and identify potential targets. This command shall be sent as braodcast massage using UDP.
        /// </summary>
        /// <returns>List<Encapsulation.CIPIdentityItem> contains the received informations from all devices </returns>	
        public List<Encapsulation.CIPIdentityItem> ListIdentity()
        {
            List<Encapsulation.CIPIdentityItem> returnList = new List<Encapsulation.CIPIdentityItem>();
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {

                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            System.Net.IPAddress mask = ip.IPv4Mask;
                            System.Net.IPAddress address = ip.Address;

                            String multicastAddress = (address.GetAddressBytes()[0] | (~(mask.GetAddressBytes()[0])) & 0xFF).ToString() + "." + (address.GetAddressBytes()[1] | (~(mask.GetAddressBytes()[1])) & 0xFF).ToString() + "." + (address.GetAddressBytes()[2] | (~(mask.GetAddressBytes()[2])) & 0xFF).ToString() + "." + (address.GetAddressBytes()[3] | (~(mask.GetAddressBytes()[3])) & 0xFF).ToString();

                            byte[] sendData = new byte[24];
                            sendData[0] = 0x63;               //Command for "ListIdentity"
                            System.Net.Sockets.UdpClient udpClient = new System.Net.Sockets.UdpClient();
                            System.Net.IPEndPoint endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(multicastAddress), 44818);                           
                            udpClient.Send(sendData, sendData.Length, endPoint);

                            var asyncResult = udpClient.BeginReceive(null, null);
                            asyncResult.AsyncWaitHandle.WaitOne(500);

                            while (true)
                            {
                                if (asyncResult.IsCompleted)
                                {
                                    try
                                    {
                                        System.Net.IPEndPoint remoteEP = null;
                                        byte[] receivedData = udpClient.EndReceive(asyncResult, ref remoteEP);
                                        // EndReceive worked and we have received data and remote endpoint
                                        if (receivedData.Length > 0)
                                        {
                                            UInt16 command = Convert.ToUInt16(receivedData[0]
                                                                        | (receivedData[1] << 8));
                                            if (command == 0x63)
                                            {
                                                returnList.Add(Encapsulation.CIPIdentityItem.getCIPIdentityItem(24, receivedData));
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        break;
                                    }
                                }
                                else
                                    break;
                            }

                        }
                    }
                }
            }
            return returnList;
        }
    }


}
