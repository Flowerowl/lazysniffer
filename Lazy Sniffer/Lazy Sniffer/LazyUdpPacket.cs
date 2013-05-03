using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lazy_Sniffer
{
    public class LazyUdpPacket
    {
        public PacketDotNet.UdpPacket uPack;
        public string sourcePort;
        public string destinPort;
        public string len;
        public string data;

        public LazyUdpPacket(PacketDotNet.UdpPacket udpPacket)
        {
            this.uPack = udpPacket;
            this.sourcePort = uPack.SourcePort.ToString();
            this.destinPort = uPack.DestinationPort.ToString();
            this.len = uPack.Bytes.Length.ToString();
            this.data = byteToHexStr(uPack.PayloadData);
        }

        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2") + " ";
                }
            }
            return returnStr;
        }
    }
}
