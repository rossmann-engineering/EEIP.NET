using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sres.Net.EEIP.ObjectLibrary
{    /// <summary>
     /// Connection Manager Object - Class Code: 06 Hex
     /// </summary>
     /// <remarks>
     /// </remarks>
    public class ConnectionManagerObject
    {
        /// <summary>
        /// Returns the Explanation of a given statuscode (Table 3-5-29) Page 3-75 Vol 1
        /// </summary>
        /// <param name="statusCode">Extended Status Code</param> 
        public static string GetExtendedStatus(uint statusCode)
        {
            switch(statusCode)
            {
                case 0x0100: return "Connection in use or duplicate forward open";
                case 0x0103: return "Transport class and trigger combination not supported";
                case 0x0106: return "Ownership conflict";
                case 0x0107: return "Target connection not found";
                case 0x0108: return "Invalid network connection parameter";
                case 0x0109: return "Invalid connection size";
                case 0x0110: return "Target for connection not configured";
                case 0x0111: return "RPI not supported";
                case 0x0113: return "Out of connections";
                case 0x0114: return "Vendor ID or product code missmatch";
                case 0x0115: return "Product type missmatch";
                case 0x0116: return "Revision mismatch";
                case 0x0117: return "Invalid produced or consumed application path";
                case 0x0118: return "Invalid or inconsistent configuration application path";
                case 0x0119: return "non-listen only connection not opened";
                case 0x011A: return "Target Obbject out of connections";
                case 0x011B: return "RPI is smaller than the production inhibit time";
                case 0x0203: return "Connection timed out";
                case 0x0204: return "Unconnected request timed out";
                case 0x0205: return "Parameter Error in unconnected request service";
                case 0x0206: return "Message too large for unconnected_send service";
                case 0x0207: return "Unconnected acknowledge without reply";
                case 0x0301: return "No Buffer memory available";
                case 0x0302: return "Network Bandwidth not available for data";
                case 0x0303: return "No consumed connection ID Filter available";
                case 0x0304: return "Not configured to send Scheduled priority data";
                case 0x0305: return "Schedule signature missmatch";
                case 0x0306: return "Schedule signature validation not possible";
                case 0x0311: return "Port not available";
                case 0x0312: return "Link address not valid";
                case 0x0315: return "Invalid segment in connection path";
                case 0x0316: return "Error in forward close service connection path";
                case 0x0317: return "Scheduling not specified";
                case 0x0318: return "Link address to self invalid";
                case 0x0319: return "Secondary resources unavailable";
                case 0x031A: return "Rack connation already established";
                case 0x031B: return "Module connection already established";
                case 0x031C: return "Miscellaneous";
                case 0x031D: return "Redundant connection Mismatch";
                case 0x031E: return "No more user configurable link consumer resources available in the producing module";
                case 0x0800: return "Network link in path module is offline";
                case 0x0810: return "No target application data available";
                case 0x0811: return "No originator application data available";
                case 0x0812: return "Node address has chnged since the network was scheduled";
                case 0x0813: return "Not configured for off-Subnet Multicast";
                default: return "unknown";
            }
        }
    }
}
