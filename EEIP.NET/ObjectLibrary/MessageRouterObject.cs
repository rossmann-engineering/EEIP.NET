using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sres.Net.EEIP.ObjectLibrary
{
    class MessageRouterObject
    {
        public EEIPClient eeipClient;

        /// <summary>
        /// Constructor. </summary>
        /// <param name="eeipClient"> EEIPClient Object</param>
        public MessageRouterObject(EEIPClient eeipClient)
        {
            this.eeipClient = eeipClient;
        }

        public struct ObjectListStruct
        {
            public UInt16 Number;
            public UInt16[] Classes;
        }

        /// <summary>
        /// gets the Object List / Read "Message Router Object" Class Code 0x02 - Attribute ID 1
        /// </summary>
        public ObjectListStruct ObjectList
        {
            get
            {
                byte[] byteArray = eeipClient.GetAttributeSingle(2, 1, 1);
                ObjectListStruct returnValue;
                returnValue.Number = (UInt16)(byteArray[1] << 8 | byteArray[0]);
                returnValue.Classes = new UInt16[returnValue.Number];
                for (int i = 0; i < returnValue.Classes.Length; i++)
                {
                    returnValue.Classes[i] = (UInt16)(byteArray[i*2+3] << 8 | byteArray[i*2+2]);
                }
                return returnValue;
            }
        }

        /// <summary>
        /// gets the Maximum of connections supported / Read "Message Router Object" Class Code 0x02 - Attribute ID 2
        /// </summary>
        public UInt16 NumberAvailable
        {
            get
            {
                byte[] byteArray = eeipClient.GetAttributeSingle(2, 2, 1);
                UInt16 returnValue;
                returnValue = (UInt16)(byteArray[1] << 8 | byteArray[0]);
                return returnValue;
            }
        }

        /// <summary>
        /// gets the number of active connections / Read "Message Router Object" Class Code 0x02 - Attribute ID 3
        /// </summary>
        public UInt16 NumberActive
        {
            get
            {
                byte[] byteArray = eeipClient.GetAttributeSingle(2, 3, 1);
                UInt16 returnValue;
                returnValue = (UInt16)(byteArray[1] << 8 | byteArray[0]);
                return returnValue;
            }
        }

        /// <summary>
        /// gets the active connections / Read "Message Router Object" Class Code 0x02 - Attribute ID 4
        /// </summary>
        public UInt16[] ActiveConnections
        {
            get
            {
                byte[] byteArray = eeipClient.GetAttributeSingle(2, 4, 1);
                UInt16[] returnValue = new UInt16[byteArray.Length / 2];
                for (int i = 0; i < returnValue.Length; i++)
                {
                    returnValue[i] = (UInt16)(byteArray[1 + 2*i] << 8 | byteArray[0 + 2*i]);
                }
                return returnValue;
            
            }
        }
    }
}
