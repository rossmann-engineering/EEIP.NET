using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Sres.Net.EEIP
{
    public class Encapsulation
    {
        public CommandsEnum Command { get; set; }
        public UInt16 Length { get; set; }
        public UInt32 SessionHandle { get; set; }
        public StatusEnum Status { get; }
        private byte[] senderContext = new byte[8];
        private UInt32 options = 0;
        public List<byte> CommandSpecificData = new List<byte>();

        /// <summary>
        /// Table 2-3.3 Error Codes
        /// </summary>
        public enum StatusEnum : UInt32
        {
            Success = 0x0000,
            InvalidCommand = 0x0001,
            InsufficientMemory = 0x0002,
            IncorrectData = 0x0003,
            InvalidSessionHandle = 0x0064,
            InvalidLength = 0x0065,
            UnsupportedEncapsulationProtocol = 0x0069
        }





        /// <summary>
        /// Table 2-3.2 Encapsulation Commands
        /// </summary>
        public enum CommandsEnum : UInt16
        {
            NOP = 0x0000,
            ListServices = 0x0004,
            ListIdentity = 0x0063,
            ListInterfaces = 0x0064,
            RegisterSession = 0x0065,
            UnRegisterSession = 0x0066,
            SendRRData = 0x006F,
            SendUnitData = 0x0070,
            IndicateStatus = 0x0072,
            Cancel = 0x0073
        }

        public byte[] toBytes()
        {
            byte[] returnValue = new byte[24 + CommandSpecificData.Count];
            returnValue[0] = (byte)this.Command;
            returnValue[1] = (byte)((UInt16)this.Command >> 8);
            returnValue[2] = (byte)this.Length;
            returnValue[3] = (byte)((UInt16)this.Length >> 8);
            returnValue[4] = (byte)this.SessionHandle;
            returnValue[5] = (byte)((UInt32)this.SessionHandle >> 8);
            returnValue[6] = (byte)((UInt32)this.SessionHandle >> 16);
            returnValue[7] = (byte)((UInt32)this.SessionHandle >> 24);
            returnValue[8] = (byte)this.Status;
            returnValue[9] = (byte)((UInt16)this.Status >> 8);
            returnValue[10] = (byte)((UInt16)this.Status >> 16);
            returnValue[11] = (byte)((UInt16)this.Status >> 24);
            returnValue[12] = senderContext[0];
            returnValue[13] = senderContext[1];
            returnValue[14] = senderContext[2];
            returnValue[15] = senderContext[3];
            returnValue[16] = senderContext[4];
            returnValue[17] = senderContext[5];
            returnValue[18] = senderContext[6];
            returnValue[19] = senderContext[7];
            returnValue[20] = (byte)this.options;
            returnValue[21] = (byte)((UInt16)this.options >> 8);
            returnValue[22] = (byte)((UInt16)this.options >> 16);
            returnValue[23] = (byte)((UInt16)this.options >> 24);
            for (int i = 0; i < CommandSpecificData.Count; i++)
            {
                returnValue[24 + i] = CommandSpecificData[i];
            }
            return returnValue;
        }


        /// <summary>
        /// Table 2-4.4 CIP Identity Item
        /// </summary>
        public class CIPIdentityItem
        {
            public UInt16 ItemTypeCode;                                     //Code indicating item type of CIP Identity (0x0C)
            public UInt16 ItemLength;                                       //Number of bytes in item which follow (length varies depending on Product Name string)
            public UInt16 EncapsulationProtocolVersion;                     //Encapsulation Protocol Version supported (also returned with Register Sesstion reply).
            public SocketAddress SocketAddress = new SocketAddress();       //Socket Address (see section 2-6.3.2)
            public UInt16 VendorID1;                                        //Device manufacturers Vendor ID
            public UInt16 DeviceType1;                                      //Device Type of product
            public UInt16 ProductCode1;                                     //Product Code assigned with respect to device type
            public byte[] Revision1 = new byte[2];                          //Device revision
            public UInt16 Status1;                                          //Current status of device
            public UInt32 SerialNumber1;                                      //Serial number of device
            public byte ProductNameLength;                          
            public string ProductName1;                                     //Human readable description of device
            public byte State1;                                             //Current state of device


            public static CIPIdentityItem getCIPIdentityItem(int startingByte, byte[] receivedData)
            {
                startingByte = startingByte + 2;            //Skipped ItemCount
                CIPIdentityItem cipIdentityItem = new CIPIdentityItem();
                cipIdentityItem.ItemTypeCode = Convert.ToUInt16(receivedData[0+startingByte]
                                                                    | (receivedData[1 + startingByte] << 8));
                cipIdentityItem.ItemLength = Convert.ToUInt16(receivedData[2 + startingByte]
                                                                    | (receivedData[3 + startingByte] << 8));
                cipIdentityItem.EncapsulationProtocolVersion = Convert.ToUInt16(receivedData[4 + startingByte]
                                                                    | (receivedData[5 + startingByte] << 8));
                cipIdentityItem.SocketAddress.SIN_family = Convert.ToUInt16(receivedData[7 + startingByte]
                                                    | (receivedData[6 + startingByte] << 8));
                cipIdentityItem.SocketAddress.SIN_port = Convert.ToUInt16(receivedData[9 + startingByte]
                                                    | (receivedData[8 + startingByte] << 8));
                cipIdentityItem.SocketAddress.SIN_Address = (UInt32)(receivedData[13 + startingByte]
                                                    | (receivedData[12 + startingByte] << 8)
                                                    | (receivedData[11 + startingByte] << 16)
                                                    | (receivedData[10 + startingByte] << 24)
                                                    );
                cipIdentityItem.VendorID1 = Convert.ToUInt16(receivedData[22 + startingByte]
                                    | (receivedData[23 + startingByte] << 8));
                cipIdentityItem.DeviceType1 = Convert.ToUInt16(receivedData[24 + startingByte]
                                    | (receivedData[25 + startingByte] << 8));
                cipIdentityItem.ProductCode1 = Convert.ToUInt16(receivedData[26 + startingByte]
                    | (receivedData[27 + startingByte] << 8));
                cipIdentityItem.Revision1[0] = receivedData[28 + startingByte];
                cipIdentityItem.Revision1[1] = receivedData[29 + startingByte];
                cipIdentityItem.Status1 = Convert.ToUInt16(receivedData[30 + startingByte]
                    | (receivedData[31 + startingByte] << 8));
                cipIdentityItem.SerialNumber1 = (UInt32)(receivedData[32 + startingByte]
                                                    | (receivedData[33 + startingByte] << 8)
                                                    | (receivedData[34 + startingByte] << 16)
                                                    | (receivedData[35 + startingByte] << 24));
                cipIdentityItem.ProductNameLength = receivedData[36 + startingByte];
                cipIdentityItem.ProductName1 = Encoding.ASCII.GetString(receivedData, 37 + startingByte, cipIdentityItem.ProductNameLength);
                cipIdentityItem.State1 = receivedData[receivedData.Length - 1];
                return cipIdentityItem;
            }
            /// <summary>
            /// Converts an IP-Address in UIint32 Format (Received by Device)
            /// </summary>
            public static string getIPAddress(UInt32 address)
            {
                return ((byte)(address >> 24)).ToString()+"." + ((byte)(address >> 16)).ToString()+"."+((byte)(address >> 8)).ToString()+"."+((byte)(address)).ToString();
            }


        }




        /// <summary>
        /// Socket Address (see section 2-6.3.2)
        /// </summary>
        public class SocketAddress
        {
            public UInt16 SIN_family;
            public UInt16 SIN_port;
            public UInt32 SIN_Address;
            public byte[] SIN_Zero = new byte[8];
        }

        public class CommonPacketFormat
        {
            public UInt16 ItemCount = 2;
            public UInt16 AddressItem = 0x0000;
            public UInt16 AddressLength = 0;
            public UInt16 DataItem = 0xB2; //0xB2 = Unconnected Data Item
            public UInt16 DataLength = 8;
            public List<byte> Data = new List<byte>();
            public UInt16 SockaddrInfoItem_O_T = 0x8001; //8000 for O->T and 8001 for T->O - Volume 2 Table 2-6.9
            public UInt16 SockaddrInfoLength = 16;
            public SocketAddress SocketaddrInfo_O_T = null;


            public byte[] toBytes()
            {
                if (SocketaddrInfo_O_T != null)
                    ItemCount=3;
                byte[] returnValue = new byte[10 + Data.Count + (SocketaddrInfo_O_T == null ? 0 : 20)];
                returnValue[0] = (byte)this.ItemCount;
                returnValue[1] = (byte)((UInt16)this.ItemCount >> 8);
                returnValue[2] = (byte)this.AddressItem;
                returnValue[3] = (byte)((UInt16)this.AddressItem >> 8);
                returnValue[4] = (byte)this.AddressLength;
                returnValue[5] = (byte)((UInt16)this.AddressLength >> 8);
                returnValue[6] = (byte)this.DataItem;
                returnValue[7] = (byte)((UInt16)this.DataItem >> 8);
                returnValue[8] = (byte)this.DataLength;
                returnValue[9] = (byte)((UInt16)this.DataLength >> 8);
                for (int i = 0; i < Data.Count; i++)
                {
                    returnValue[10 + i] = Data[i];
                }


                // Add Socket Address Info Item
                if (SocketaddrInfo_O_T != null)
                {
                    returnValue[10 + Data.Count + 0] = (byte)this.SockaddrInfoItem_O_T;
                    returnValue[10 + Data.Count + 1] = (byte)((UInt16)this.SockaddrInfoItem_O_T >> 8);
                    returnValue[10 + Data.Count + 2] = (byte)this.SockaddrInfoLength;
                    returnValue[10 + Data.Count + 3] = (byte)((UInt16)this.SockaddrInfoLength >> 8);
                    returnValue[10 + Data.Count + 5] = (byte)this.SocketaddrInfo_O_T.SIN_family;
                    returnValue[10 + Data.Count + 4] = (byte)((UInt16)this.SocketaddrInfo_O_T.SIN_family >> 8);
                    returnValue[10 + Data.Count + 7] = (byte)this.SocketaddrInfo_O_T.SIN_port;
                    returnValue[10 + Data.Count + 6] = (byte)((UInt16)this.SocketaddrInfo_O_T.SIN_port >> 8);
                    returnValue[10 + Data.Count + 11] = (byte)this.SocketaddrInfo_O_T.SIN_Address;
                    returnValue[10 + Data.Count + 10] = (byte)((UInt32)this.SocketaddrInfo_O_T.SIN_Address >> 8);
                    returnValue[10 + Data.Count + 9] = (byte)((UInt32)this.SocketaddrInfo_O_T.SIN_Address >> 16);
                    returnValue[10 + Data.Count + 8] = (byte)((UInt32)this.SocketaddrInfo_O_T.SIN_Address >> 24);
                    returnValue[10 + Data.Count + 12] = this.SocketaddrInfo_O_T.SIN_Zero[0];
                    returnValue[10 + Data.Count + 13] = this.SocketaddrInfo_O_T.SIN_Zero[1];
                    returnValue[10 + Data.Count + 14] = this.SocketaddrInfo_O_T.SIN_Zero[2];
                    returnValue[10 + Data.Count + 15] = this.SocketaddrInfo_O_T.SIN_Zero[3];
                    returnValue[10 + Data.Count + 16] = this.SocketaddrInfo_O_T.SIN_Zero[4];
                    returnValue[10 + Data.Count + 17] = this.SocketaddrInfo_O_T.SIN_Zero[5];
                    returnValue[10 + Data.Count + 18] = this.SocketaddrInfo_O_T.SIN_Zero[6];
                    returnValue[10 + Data.Count + 19] = this.SocketaddrInfo_O_T.SIN_Zero[7];
                }
                    return returnValue;
            }
        }
    }
}
