using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sres.Net.EEIP.ObjectLibrary
{
    public class TcpIpInterfaceObject
    {
        public EEIPClient eeipClient;

        /// <summary>
        /// Constructor. </summary>
        /// <param name="eeipClient"> EEIPClient Object</param>
        public TcpIpInterfaceObject(EEIPClient eeipClient)
        {
            this.eeipClient = eeipClient;
        }

        /// <summary>
        /// gets the Status / Read "TCP/IP Interface Object" Class Code 0xF5 - Attribute ID 1
        /// </summary>
        public InterfaceStatus Status
        {
            get
            {
                byte[] byteArray = eeipClient.GetAttributeSingle(0xF5, 1, 1);
                InterfaceStatus status = new InterfaceStatus();
                if ((byteArray[0] & 0x0F) == 0)
                    status.NotConfigured = true;
                if ((byteArray[0] & 0x0F) == 1)
                    status.ValidConfiguration = true;
                if ((byteArray[0] & 0x0F) == 2)
                    status.ValidManualConfiguration = true;
                if ((byteArray[0] & 0x10) != 0)
                    status.McastPending = true;
                return status;
            }
        }
    

        /// <summary>
        /// gets the Configuration capability / Read "TCP/IP Interface Object" Class Code 0xF5 - Attribute ID 2
        /// </summary>
        public InterfaceCapabilityFlags ConfigurationCapability
        {
            get
            {
                byte[] byteArray = eeipClient.GetAttributeSingle(0xF5, 1, 2);
                    InterfaceCapabilityFlags configurationCapability = new InterfaceCapabilityFlags();
                if ((byteArray[0] & 0x01) != 0)
                    configurationCapability.BootPClient = true;
                if ((byteArray[0] & 0x02) != 0)
                    configurationCapability.DNSClient = true;
                if ((byteArray[0] & 0x04) != 0)
                    configurationCapability.DHCPClient = true;
                if ((byteArray[0] & 0x08) != 0)
                    configurationCapability.DHCPClient = true;
                if ((byteArray[0] & 0x10) != 0)
                    configurationCapability.ConfigurationSettable = true;
                return configurationCapability;
            }
        }

        /// <summary>
        /// gets the Path to the Physical Link object / Read "TCP/IP Interface Object" Class Code 0xF5 - Attribute ID 4
        /// </summary>
        public PhysicalLink PhysicalLinkObject
        {
            get
            {
                byte[] byteArray = eeipClient.GetAttributeSingle(0xF5, 1, 4);
                PhysicalLink physicalLinkObject = new PhysicalLink();
                physicalLinkObject.PathSize = (UInt16)(byteArray[1] << 8 | byteArray[0]);
                if (byteArray.Length > 2)
                    System.Buffer.BlockCopy(byteArray, 2 , physicalLinkObject.Path, 0, byteArray.Length - 2);
                return physicalLinkObject;
            }
        }

        /// <summary>
        /// sets the Configuration control attribute / Write "TCP/IP Interface Object" Class Code 0xF5 - Attribute ID 3
        /// </summary>
        public InterfaceControlFlags ConfigurationControl
        {
            set
            {
                byte[] valueToWrite = new byte[4];
                if (value.EnableBootP)
                    valueToWrite[0] = 1;
                if (value.EnableDHCP)
                    valueToWrite[0] = 2;
                if (value.EnableDNS)
                    valueToWrite[0] = (byte)(valueToWrite[0] | 0x10);
                eeipClient.SetAttributeSingle(0xF5, 1, 3, valueToWrite);
            }
        }

        /// <summary>
        /// sets the TCP/IP Network interface Configuration / Write "TCP/IP Interface Object" Class Code 0xF5 - Attribute ID 5
        /// </summary>
        public NetworkInterfaceConfiguration InterfaceConfiguration
        {
            set
            {
                byte[] valueToWrite = new byte[68];
                valueToWrite[0] = (byte)value.IPAddress;
                valueToWrite[1] = (byte)(value.IPAddress >> 8);
                valueToWrite[2] = (byte)(value.IPAddress >> 16);
                valueToWrite[3] = (byte)(value.IPAddress >> 24);
                valueToWrite[4] = (byte)value.NetworkMask;
                valueToWrite[5] = (byte)(value.NetworkMask >> 8);
                valueToWrite[6] = (byte)(value.NetworkMask >> 16);
                valueToWrite[7] = (byte)(value.NetworkMask >> 24);
                valueToWrite[8] = (byte)value.GatewayAddress;
                valueToWrite[9] = (byte)(value.GatewayAddress >> 8);
                valueToWrite[10] = (byte)(value.GatewayAddress >> 16);
                valueToWrite[11] = (byte)(value.GatewayAddress >> 24);
                valueToWrite[12] = (byte)value.NameServer;
                valueToWrite[13] = (byte)(value.NameServer >> 8);
                valueToWrite[14] = (byte)(value.NameServer >> 16);
                valueToWrite[15] = (byte)(value.NameServer >> 24);
                valueToWrite[16] = (byte)value.NameServer2;
                valueToWrite[17] = (byte)(value.NameServer2 >> 8);
                valueToWrite[18] = (byte)(value.NameServer2 >> 16);
                valueToWrite[19] = (byte)(value.NameServer2 >> 24);
                if (value.DomainName != null)
                {
                    byte[] domainName = Encoding.ASCII.GetBytes(value.DomainName);
                    System.Buffer.BlockCopy(domainName, 0, valueToWrite, 20, domainName.Length);
                }
                eeipClient.SetAttributeSingle(0xF5, 1, 5, valueToWrite);
            }
        }

    }

    /// <summary>
    /// Chapter 5-3.2.2.1 Volume 2
    /// </summary>
    public struct InterfaceStatus
        {
            public bool NotConfigured;
            public bool ValidConfiguration;
            public bool ValidManualConfiguration;
            public bool McastPending;  
        }

    /// <summary>
    /// Chapter 5-3.2.2.2 Volume 2
    /// </summary>
    public struct InterfaceCapabilityFlags
    {
        public bool BootPClient;
        public bool DNSClient;
        public bool DHCPClient;
        public bool DHCP_DNSUpdate;
        public bool ConfigurationSettable;
    }
    /// <summary>
    /// Chapter 5-3.2.2.3 Volume 2
    /// </summary>
    public struct InterfaceControlFlags
    {
        public bool UsePreviouslyStored;
        public bool EnableBootP;
        public bool EnableDHCP;
        public bool EnableDNS;
    }

    /// <summary>
    /// Page 5.5 Volume 2
    /// </summary>
    public struct PhysicalLink
    {
        public UInt16 PathSize;
        public byte[] Path;
    }

    /// <summary>
    /// Chapter 5-3.2.2.5 Volume 2
    /// </summary>
    public struct NetworkInterfaceConfiguration
    {
        public UInt32 IPAddress;
        public UInt32 NetworkMask;
        public UInt32 GatewayAddress;
        public UInt32 NameServer;
        public UInt32 NameServer2;
        public string DomainName;
    }


}
