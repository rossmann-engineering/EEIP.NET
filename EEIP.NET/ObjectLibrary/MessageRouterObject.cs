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
        /// gets the Vendor ID / Read "Identity Object" Class Code 0x01 - Attribute ID 1
        /// </summary>
        public ObjectListStruct ObjectList
        {
            get
            {
                byte[] byteArray = eeipClient.getAttributeSingle(2, 1, 1);
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
    }
}
