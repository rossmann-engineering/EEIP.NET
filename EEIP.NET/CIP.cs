using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sres.Net.EEIP
{

    /// <summary>
    /// Table A-3.1 Volume 1 Chapter A-3
    /// </summary>
    public enum CIPCommonServices : byte
    {
        Get_Attributes_All = 0x01,
        Set_Attributes_All_Request = 0x02,
        Get_Attribute_List = 0x03,
        Set_Attribute_List = 0x04,
        Reset = 0x05,
        Start = 0x06,
        Stop = 0x07,
        Create = 0x08,
        Delete = 0x09,
        Multiple_Service_Packet = 0x0A,
        Apply_Attributes = 0x0D,
        Get_Attribute_Single = 0x0E,
        Set_Attribute_Single = 0x10,
        Find_Next_Object_Instance = 0x11,
        Error_Response = 0x14,
        Restore = 0x15,
        Save = 0x16,
        NOP = 0x17,
        Get_Member = 0x18,
        Set_Member = 0x19,
        Insert_Member = 0x1A,
        Remove_Member = 0x1B,
        GroupSync = 0x1C    
    }


    public class CIPException : Exception
    {
        public CIPException()
        {
        }

        public CIPException(string message)
            : base(message)
        {
        }

        public CIPException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }


}
