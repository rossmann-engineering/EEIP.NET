using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sres.Net.EEIP.ObjectLibrary
{
    class AssemblyObject
    {
        public EEIPClient eeipClient;

        /// <summary>
        /// Constructor. </summary>
        /// <param name="eeipClient"> EEIPClient Object</param>
        public AssemblyObject(EEIPClient eeipClient)
        {
            this.eeipClient = eeipClient;
        }

        /// <summary>
        /// Reads the Instance of the Assembly Object (Instance 101 returns the bytes of the class ID 101)
        /// </summary>
        /// <param name="instanceNo"> Instance number to be returned</param>
        /// <returns>bytes of the Instance</returns>
        public byte[] getInstance(int instanceNo)
        {
            
                byte[] byteArray = eeipClient.getAttributeSingle(4, instanceNo, 3);
                return byteArray;
            
        }

    }
}
