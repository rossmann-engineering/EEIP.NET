using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sres.Net.EEIP.ObjectLibrary
{
    /// <summary>
    /// Identity Object - Class Code: 01 Hex
    /// </summary>
    /// <remarks>
    /// This object provides identification of and general information about the device. The Identity Object shall be present in all CIP products.
    /// If autonomous components of a device exist, use multiple instances of the Identity Object.
    /// </remarks>
    class IdentityObject
    {
        public EEIPClient eeipClient;

        /// <summary>
        /// Constructor. </summary>
        /// <param name="eeipClient"> EEIPClient Object</param>
        public IdentityObject(EEIPClient eeipClient)
        {
            this.eeipClient = eeipClient;
        }

        /// <summary>
        /// gets the Vendor ID / Read "Identity Object" Class Code 0x01 - Attribute ID 1
        /// </summary>
        public UInt16 VendorID
        {
            get
            {
                byte[] byteArray = eeipClient.GetAttributeSingle(1, 1, 1);
                UInt16 returnValue = (UInt16)(byteArray[1] << 8 | byteArray[0]);
                return returnValue;
            }
        }

        /// <summary>
        /// gets the Device Type / Read "Identity Object" Class Code 0x01 - Attribute ID 2
        /// </summary>
        public UInt16 DeviceType
        {
            get
            {
                byte[] byteArray = eeipClient.GetAttributeSingle(1, 1, 2);
                UInt16 returnValue = (UInt16)(byteArray[1] << 8 | byteArray[0]);
                return returnValue;
            }
        }


        /// <summary>
        /// gets the Product code / Read "Identity Object" Class Code 0x01 - Attribute ID 3
        /// </summary>
        public UInt16 ProductCode
        {
            get
            {
                byte[] byteArray = eeipClient.GetAttributeSingle(1, 1, 3);
                UInt16 returnValue = (UInt16)(byteArray[1] << 8 | byteArray[0]);
                return returnValue;
            }
        }

        /// <summary>
        /// gets the Revision / Read "Identity Object" Class Code 0x01 - Attribute ID 4
        /// </summary>
        /// <returns>Revision</returns>
        public Revison Revision
        {
            get
            {

                byte[] byteArray = eeipClient.GetAttributeSingle(1, 1, 4);
                Revison returnValue = new Revison();
                returnValue.MajorRevision = (ushort)(byteArray[0]);
                returnValue.MinorRevision = (ushort)(byteArray[1]);
                return returnValue;
            }
        }

        public struct Revison
        {
            public ushort MajorRevision;
            public ushort MinorRevision;
        }

        /// <summary>
        /// gets the Status / Read "Identity Object" Class Code 0x01 - Attribute ID 5
        /// </summary>
        public UInt16 Status
        {
            get
            {
                byte[] byteArray = eeipClient.GetAttributeSingle(1, 1, 5);
                UInt16 returnValue = (UInt16)(byteArray[1] << 8 | byteArray[0]);
                return returnValue;
            }
        }

        /// <summary>
        /// gets the Serial number / Read "Identity Object" Class Code 0x01 - Attribute ID 6
        /// </summary>
        public UInt32 SerialNumber
        {
            get
            {
                byte[] byteArray = eeipClient.GetAttributeSingle(1, 1, 6);
                UInt32 returnValue = ((UInt32)byteArray[3] << 24 | (UInt32)byteArray[2] << 16 | (UInt32)byteArray[1] << 8 | (UInt32)byteArray[0]);
                return returnValue;
            }
        }

        /// <summary>
        /// gets the Product Name / Read "Identity Object" Class Code 0x01 - Attribute ID 7
        /// </summary>
        public string ProductName
        {
            get
            {
                byte[] byteArray = eeipClient.GetAttributeSingle(1, 1, 7);
                String returnValue = Encoding.UTF8.GetString(byteArray);
                return returnValue;
            }
        }

        public enum StateEnum
        {
            Nonexistent = 0,
            DeviceSelfTesting = 1,
            Standby = 2,
            Operational = 3,
            MajorRecoverableFault = 4,
            MajorUnrecoverableFault = 5,
            DefaultforGet_Attributes_All_service = 255
        }

        /// <summary>
        /// gets the State / Read "Identity Object" Class Code 0x01 - Attribute ID 8
        /// </summary>
        public StateEnum State
        {
            get
            {
                byte[] byteArray = eeipClient.GetAttributeSingle(1, 1, 8);
                StateEnum returnValue = (StateEnum) byteArray[0];
                return returnValue;
            }
        }

        /// <summary>
        /// gets the State / Read "Identity Object" Class Code 0x01 - Attribute ID 9
        /// </summary>
        public UInt16 ConfigurationConsistencyValue
        {
            get
            {
                byte[] byteArray = eeipClient.GetAttributeSingle(1, 1, 9);
                UInt16 returnValue = (UInt16)(byteArray[1] << 8 | byteArray[0]);
                return returnValue;
            }
        }

        /// <summary>
        /// gets the Heartbeat intervall / Read "Identity Object" Class Code 0x01 - Attribute ID 10
        /// </summary>
        public byte HeartbeatInterval
        {
            get
            {
                byte[] byteArray = eeipClient.GetAttributeSingle(1, 1, 10);
                byte returnValue = (byte)byteArray[0];
                return returnValue;
            }
        }

        /// <summary>
        /// gets the Supported Language List / Read "Identity Object" Class Code 0x01 - Attribute ID 12
        /// </summary>
        public string[] SupportedLanguageList
        {
            get
            {
                byte[] byteArray = eeipClient.GetAttributeSingle(1, 1, 12);
                string[] returnValue = new string[byteArray.Length / 3];
                for (int i = 0; i < returnValue.Length; i++)
                {
                    byte[] byteArray2 = new byte[3];
                    System.Buffer.BlockCopy(byteArray, i*3, byteArray2, 0, 3);
                    returnValue[i] = Encoding.UTF8.GetString(byteArray2);
                }
                return returnValue;
            }
        }

        public ClassAttributesStruct ClassAttributes
        {
            get
            {
                byte[] byteArray = eeipClient.GetAttributeAll(1, 0);
                ClassAttributesStruct returnValue;
                returnValue.Revision = (UInt16)(byteArray[1] << 8 | byteArray[0]);
                returnValue.MaxInstance = (UInt16)(byteArray[3] << 8 | byteArray[2]);
                returnValue.MaxIDNumberOfClassAttributes = (UInt16)(byteArray[5] << 8 | byteArray[4]);
                returnValue.MaxIDNumberOfInstanceAttributes = (UInt16)(byteArray[7] << 8 | byteArray[6]);
                return returnValue;
            }
        }

        public InstanceAttributesStruct InstanceAttributes
        {
            get
            {
                byte[] byteArray = eeipClient.GetAttributeAll(1, 1);
                InstanceAttributesStruct returnValue;
                returnValue.VendorID = (UInt16)(byteArray[1] << 8 | byteArray[0]);
                returnValue.DeviceType = (UInt16)(byteArray[3] << 8 | byteArray[2]);
                returnValue.ProductCode = (UInt16)(byteArray[5] << 8 | byteArray[4]);
                returnValue.Revision.MajorRevision = byteArray[6];
                returnValue.Revision.MinorRevision = byteArray[7];
                returnValue.Status = (UInt16)(byteArray[9] << 8 | byteArray[8]);
                returnValue.SerialNumber = ((UInt32)byteArray[13] << 24 | (UInt32)byteArray[12] << 16 | (UInt32)byteArray[11] << 8 | (UInt32)byteArray[10]);
                byte[] productName = new byte[byteArray[14]];
                System.Buffer.BlockCopy(byteArray, 15, productName, 0, productName.Length);
                returnValue.ProductName = Encoding.UTF8.GetString(productName);
                return returnValue;
            }
        }


        public struct ClassAttributesStruct
        {
            public UInt16 Revision;
            public UInt16 MaxInstance;
            public UInt16 MaxIDNumberOfClassAttributes;
            public UInt16 MaxIDNumberOfInstanceAttributes;
        }

        public struct InstanceAttributesStruct
        {
            public UInt16 VendorID;
            public UInt16 DeviceType;
            public UInt16 ProductCode;
            public Revison Revision;
            public UInt16 Status;
            public uint SerialNumber;
            public string ProductName;
        }


    }
}
