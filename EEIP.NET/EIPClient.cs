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
        TcpClient client;
        NetworkStream stream;
        UInt32 sessionHandle;
        public ushort Port { get; set; } = 0xAF12;
        public string IPAddress { get; set; } = "172.0.0.1";
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

        /// <summary>
        /// Sends a RegisterSession command to a target to initiate session
        /// </summary>
        /// <param name="address">IP-Address of the target device</param> 
        /// <param name="port">Port of the target device (default should be 0xAF12)</param> 
        /// <returns>Session Handle</returns>	
        public UInt32 RegisterSession(UInt32 address, UInt16 port)
        {
            Encapsulation encapsulation = new Encapsulation();
            encapsulation.Command = Encapsulation.CommandsEnum.RegisterSession;
            encapsulation.Length = 4;
            encapsulation.CommandSpecificData.Add(1);       //Protocol version (should be set to 1)
            encapsulation.CommandSpecificData.Add(0);
            encapsulation.CommandSpecificData.Add(0);       //Session options shall be set to "0"
            encapsulation.CommandSpecificData.Add(0);


            string ipAddress = Encapsulation.CIPIdentityItem.getIPAddress(address);
            client = new TcpClient(ipAddress, port);
            stream = client.GetStream();

            stream.Write(encapsulation.toBytes(), 0, encapsulation.toBytes().Length);
            byte[] data = new Byte[256];

            Int32 bytes = stream.Read(data, 0, data.Length);

            UInt32 returnvalue = (UInt32)data[4] + (((UInt32)data[5]) << 8) + (((UInt32)data[6]) << 16) + (((UInt32)data[7]) << 24);
            this.sessionHandle = returnvalue;
            return returnvalue;
        }

        /// <summary>
        /// Sends a UnRegisterSession command to a target to terminate session
        /// </summary> 
        public void UnRegisterSession()
        {
            Encapsulation encapsulation = new Encapsulation();
            encapsulation.Command = Encapsulation.CommandsEnum.UnRegisterSession;
            encapsulation.Length = 0;
            encapsulation.SessionHandle =  sessionHandle;
 
            stream.Write(encapsulation.toBytes(), 0, encapsulation.toBytes().Length);
            byte[] data = new Byte[256];
            client.Close();
            stream.Close();
            sessionHandle = 0;
        }

        /// <summary>
        /// Sends a RegisterSession command to a target to initiate session
        /// </summary>
        /// <param name="address">IP-Address of the target device</param> 
        /// <param name="port">Port of the target device (default should be 0xAF12)</param> 
        /// <returns>Session Handle</returns>	
        public UInt32 RegisterSession(string address, UInt16 port)
        {
            string[] addressSubstring = address.Split('.');
            UInt32 ipAddress = UInt32.Parse(addressSubstring[3]) + (UInt32.Parse(addressSubstring[2]) << 8) + (UInt32.Parse(addressSubstring[1]) << 16) + (UInt32.Parse(addressSubstring[0]) << 24);
            return RegisterSession(ipAddress, port);
        }

        /// <summary>
        /// Sends a RegisterSession command to a target to initiate session with the Standard or predefined Port (Standard: 0xAF12)
        /// </summary>
        /// <param name="address">IP-Address of the target device</param> 
        /// <returns>Session Handle</returns>	
        public UInt32 RegisterSession(string address)
        {
            string[] addressSubstring = address.Split('.');
            UInt32 ipAddress = UInt32.Parse(addressSubstring[3]) + (UInt32.Parse(addressSubstring[2]) << 8) + (UInt32.Parse(addressSubstring[1]) << 16) + (UInt32.Parse(addressSubstring[0]) << 24);
            return RegisterSession(ipAddress, this.Port);
        }

        /// <summary>
        /// Sends a RegisterSession command to a target to initiate session with the Standard or predefined Port and Predefined IPAddress (Standard-Port: 0xAF12)
        /// </summary>
        /// <returns>Session Handle</returns>	
        public UInt32 RegisterSession()
        {
            
            return RegisterSession(this.IPAddress, this.Port);
        }

        public byte[] getAttributeSingle(int classID, int instanceID, int attributeID)
        {
            if (sessionHandle == 0)             //If a Session is not Registers, Try to Registers a Session with the predefined IP-Address and Port
                this.RegisterSession();
            byte[] dataToSend = new byte[48];
            Encapsulation encapsulation = new Encapsulation();
            encapsulation.SessionHandle = sessionHandle;
            encapsulation.Command = Encapsulation.CommandsEnum.SendRRData;
            encapsulation.Length = 24;
            //---------------Interface Handle CIP
            encapsulation.CommandSpecificData.Add(0);
            encapsulation.CommandSpecificData.Add(0);
            encapsulation.CommandSpecificData.Add(0);
            encapsulation.CommandSpecificData.Add(0);
            //----------------Interface Handle CIP

            //----------------Timeout
            encapsulation.CommandSpecificData.Add(0);
            encapsulation.CommandSpecificData.Add(0);
            //----------------Timeout

            //Common Packet Format (Table 2-6.1)
            Encapsulation.CommonPacketFormat commonPacketFormat = new Encapsulation.CommonPacketFormat();
            commonPacketFormat.ItemCount = 0x02;

            commonPacketFormat.AddressItem = 0x0000;        //NULL (used for UCMM Messages)
            commonPacketFormat.AddressLength = 0x0000;

            commonPacketFormat.DataItem = 0xB2;
            commonPacketFormat.DataLength = 8;



            //----------------CIP Command "Get Attribute Single"
            commonPacketFormat.Data.Add((byte)Sres.Net.EEIP.CIPCommonServices.Get_Attribute_Single);
            //----------------CIP Command "Get Attribute Single"

            //----------------Requested Path size
            commonPacketFormat.Data.Add(3);
            //----------------Requested Path size

            //----------------Path segment for Class ID
            commonPacketFormat.Data.Add(0x20);
            commonPacketFormat.Data.Add((byte)classID);
            //----------------Path segment for Class ID

            //----------------Path segment for Instance ID
            commonPacketFormat.Data.Add(0x24);
            commonPacketFormat.Data.Add((byte)instanceID);
            //----------------Path segment for Instace ID

            //----------------Path segment for Attribute ID
            commonPacketFormat.Data.Add(0x30);
            commonPacketFormat.Data.Add((byte)attributeID);
            //----------------Path segment for Attribute ID

            byte[] dataToWrite = new byte[encapsulation.toBytes().Length + commonPacketFormat.toBytes().Length];
            System.Buffer.BlockCopy(encapsulation.toBytes(), 0, dataToWrite, 0, encapsulation.toBytes().Length);
            System.Buffer.BlockCopy(commonPacketFormat.toBytes(), 0, dataToWrite, encapsulation.toBytes().Length, commonPacketFormat.toBytes().Length);
            encapsulation.toBytes();

            stream.Write(dataToWrite, 0, dataToWrite.Length);
            byte[] data = new Byte[256];

            Int32 bytes = stream.Read(data, 0, data.Length);

            //--------------------------BEGIN Error?
            if (data[42] != 0)      //Exception codes see "Table B-1.1 CIP General Status Codes"
            {
                switch (data[42])
                {
                    case 0x14: throw new CIPException("CIP-Exception: Attribute not supported, General Status Code: " + data[42]);
                    case 0x5: throw new CIPException("CIP-Exception: Path destination unknown, General Status Code: " + data[42]);
                    case 0x16: throw new CIPException("CIP-Exception: Object does not exist: " + data[42]);
                    case 0x15: throw new CIPException("CIP-Exception: Too much data: " + data[42]);
                    default: throw new CIPException("CIP-Exception, General Status Code: " + data[42]); 
                }
            }
            //--------------------------END Error?

            byte[] returnData = new byte[bytes - 44];
            System.Buffer.BlockCopy(data, 44, returnData, 0, bytes-44);

            return returnData;
        }

        /// <summary>
        /// Implementation of Common Service "Get_Attribute_All" - Service Code: 0x01
        /// </summary>
        /// <param name="classID">Class id of requested Attributes</param> 
        /// <param name="instanceID">Instance of Requested Attributes (0 for class Attributes)</param> 
        /// <returns>Session Handle</returns>	
        public byte[] GetAttributesAll(int classID, int instanceID)
        {
            if (sessionHandle == 0)             //If a Session is not Registers, Try to Registers a Session with the predefined IP-Address and Port
                this.RegisterSession();
            byte[] dataToSend = new byte[46];
            Encapsulation encapsulation = new Encapsulation();
            encapsulation.SessionHandle = sessionHandle;
            encapsulation.Command = Encapsulation.CommandsEnum.SendRRData;
            encapsulation.Length = 22;
            //---------------Interface Handle CIP
            encapsulation.CommandSpecificData.Add(0);
            encapsulation.CommandSpecificData.Add(0);
            encapsulation.CommandSpecificData.Add(0);
            encapsulation.CommandSpecificData.Add(0);
            //----------------Interface Handle CIP

            //----------------Timeout
            encapsulation.CommandSpecificData.Add(0);
            encapsulation.CommandSpecificData.Add(0);
            //----------------Timeout

            //Common Packet Format (Table 2-6.1)
            Encapsulation.CommonPacketFormat commonPacketFormat = new Encapsulation.CommonPacketFormat();
            commonPacketFormat.ItemCount = 0x02;

            commonPacketFormat.AddressItem = 0x0000;        //NULL (used for UCMM Messages)
            commonPacketFormat.AddressLength = 0x0000;

            commonPacketFormat.DataItem = 0xB2;
            commonPacketFormat.DataLength = 6;



            //----------------CIP Command "Get Attribute Single"
            commonPacketFormat.Data.Add((byte)Sres.Net.EEIP.CIPCommonServices.Get_Attributes_All);
            //----------------CIP Command "Get Attribute Single"

            //----------------Requested Path size
            commonPacketFormat.Data.Add(2);
            //----------------Requested Path size

            //----------------Path segment for Class ID
            commonPacketFormat.Data.Add(0x20);
            commonPacketFormat.Data.Add((byte)classID);
            //----------------Path segment for Class ID

            //----------------Path segment for Instance ID
            commonPacketFormat.Data.Add(0x24);
            commonPacketFormat.Data.Add((byte)instanceID);
            //----------------Path segment for Instace ID


            byte[] dataToWrite = new byte[encapsulation.toBytes().Length + commonPacketFormat.toBytes().Length];
            System.Buffer.BlockCopy(encapsulation.toBytes(), 0, dataToWrite, 0, encapsulation.toBytes().Length);
            System.Buffer.BlockCopy(commonPacketFormat.toBytes(), 0, dataToWrite, encapsulation.toBytes().Length, commonPacketFormat.toBytes().Length);
           

            stream.Write(dataToWrite, 0, dataToWrite.Length);
            byte[] data = new Byte[256];

            Int32 bytes = stream.Read(data, 0, data.Length);
            //--------------------------BEGIN Error?
            if (data[42] != 0)      //Exception codes see "Table B-1.1 CIP General Status Codes"
            {
                switch (data[42])
                {
                    case 0x14: throw new CIPException("CIP-Exception: Attribute not supported, General Status Code: " + data[42]);
                    case 0x5: throw new CIPException("CIP-Exception: Path destination unknown, General Status Code: " + data[42]);
                    case 0x16: throw new CIPException("CIP-Exception: Object does not exist: " + data[42]);
                    case 0x15: throw new CIPException("CIP-Exception: Too much data: " + data[42]);
                    default: throw new CIPException("CIP-Exception, General Status Code: " + data[42]);
                }
            }
            //--------------------------END Error?

            byte[] returnData = new byte[bytes - 44];
            System.Buffer.BlockCopy(data, 44, returnData, 0, bytes - 44);

            return returnData;
        }

        public byte[] setAttributeSingle(int classID, int instanceID, int attributeID, byte[] value)
        {
            if (sessionHandle == 0)             //If a Session is not Registers, Try to Registers a Session with the predefined IP-Address and Port
                this.RegisterSession();
            byte[] dataToSend = new byte[48 + value.Length];
            Encapsulation encapsulation = new Encapsulation();
            encapsulation.SessionHandle = sessionHandle;
            encapsulation.Command = Encapsulation.CommandsEnum.SendRRData;
            encapsulation.Length = (UInt16)(24+value.Length);
            //---------------Interface Handle CIP
            encapsulation.CommandSpecificData.Add(0);
            encapsulation.CommandSpecificData.Add(0);
            encapsulation.CommandSpecificData.Add(0);
            encapsulation.CommandSpecificData.Add(0);
            //----------------Interface Handle CIP

            //----------------Timeout
            encapsulation.CommandSpecificData.Add(0);
            encapsulation.CommandSpecificData.Add(0);
            //----------------Timeout

            //Common Packet Format (Table 2-6.1)
            Encapsulation.CommonPacketFormat commonPacketFormat = new Encapsulation.CommonPacketFormat();
            commonPacketFormat.ItemCount = 0x02;

            commonPacketFormat.AddressItem = 0x0000;        //NULL (used for UCMM Messages)
            commonPacketFormat.AddressLength = 0x0000;

            commonPacketFormat.DataItem = 0xB2;
            commonPacketFormat.DataLength = (UInt16)(8 + value.Length);



            //----------------CIP Command "Set Attribute Single"
            commonPacketFormat.Data.Add((byte)Sres.Net.EEIP.CIPCommonServices.Set_Attribute_Single);
            //----------------CIP Command "Set Attribute Single"

            //----------------Requested Path size
            commonPacketFormat.Data.Add(3);
            //----------------Requested Path size

            //----------------Path segment for Class ID
            commonPacketFormat.Data.Add(0x20);
            commonPacketFormat.Data.Add((byte)classID);
            //----------------Path segment for Class ID

            //----------------Path segment for Instance ID
            commonPacketFormat.Data.Add(0x24);
            commonPacketFormat.Data.Add((byte)instanceID);
            //----------------Path segment for Instace ID

            //----------------Path segment for Attribute ID
            commonPacketFormat.Data.Add(0x30);
            commonPacketFormat.Data.Add((byte)attributeID);
            //----------------Path segment for Attribute ID

            //----------------Data
            for (int i = 0; i < value.Length; i++)
            {
                commonPacketFormat.Data.Add(value[i]);
            }
            //----------------Data

            byte[] dataToWrite = new byte[encapsulation.toBytes().Length + commonPacketFormat.toBytes().Length];
            System.Buffer.BlockCopy(encapsulation.toBytes(), 0, dataToWrite, 0, encapsulation.toBytes().Length);
            System.Buffer.BlockCopy(commonPacketFormat.toBytes(), 0, dataToWrite, encapsulation.toBytes().Length, commonPacketFormat.toBytes().Length);
            encapsulation.toBytes();

            stream.Write(dataToWrite, 0, dataToWrite.Length);
            byte[] data = new Byte[256];

            Int32 bytes = stream.Read(data, 0, data.Length);

            //--------------------------BEGIN Error?
            if (data[42] != 0)      //Exception codes see "Table B-1.1 CIP General Status Codes"
            {
                switch (data[42])
                {
                    case 0x14: throw new CIPException("CIP-Exception: Attribute not supported, General Status Code: " + data[42]);
                    case 0x5: throw new CIPException("CIP-Exception: Path destination unknown, General Status Code: " + data[42]);
                    case 0x16: throw new CIPException("CIP-Exception: Object does not exist: " + data[42]);
                    case 0x15: throw new CIPException("CIP-Exception: Too much data: " + data[42]);

                    default: throw new CIPException("CIP-Exception, General Status Code: " + data[42]);
                }
            }
            //--------------------------END Error?

            byte[] returnData = new byte[bytes - 44];
            System.Buffer.BlockCopy(data, 44, returnData, 0, bytes - 44);

            return returnData;
        }

        /// <summary>
        /// Implementation of Common Service "Get_Attribute_All" - Service Code: 0x01
        /// </summary>
        /// <param name="classID">Class id of requested Attributes</param> 
        public byte[] GetAttributesAll(int classID)
        {
            return this.GetAttributesAll(classID, 0);
        }

        ObjectLibrary.IdentityObject identityObject;
        public ObjectLibrary.IdentityObject IdentityObject
        {
            get
            {
                if (identityObject == null)
                    identityObject = new ObjectLibrary.IdentityObject(this);
                return identityObject;

            }
        }

        ObjectLibrary.MessageRouterObject messageRouterObject;
        public ObjectLibrary.MessageRouterObject MessageRouterObject
        {
            get
            {
                if (messageRouterObject == null)
                    messageRouterObject = new ObjectLibrary.MessageRouterObject(this);
                return messageRouterObject;

            }
        }

        ObjectLibrary.AssemblyObject assemblyObject;
        public ObjectLibrary.AssemblyObject AssemblyObject
        {
            get
            {
                if (assemblyObject == null)
                    assemblyObject = new ObjectLibrary.AssemblyObject(this);
                return assemblyObject;

            }
        }

    }


}
