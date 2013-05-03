using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lazy_Sniffer
{
    public class LazyTcpPacket
    {
        public PacketDotNet.TcpPacket tpPack;
        public string sourcePort;
        public string destinPort;
        public string len;
        public string sequence;
        public string ack;
        public string headerLen;
        public byte flags;
        public string windowSize;
        public string checkSum;
        public string data;

        public LazyTcpPacket(PacketDotNet.TcpPacket tcpPacket)
        {
            this.tpPack = tcpPacket;
            this.sourcePort = tpPack.SourcePort.ToString();
            this.destinPort = tpPack.DestinationPort.ToString();
            this.len = tpPack.Bytes.Length.ToString();
            this.sequence = tpPack.SequenceNumber.ToString();
            this.ack = tpPack.AcknowledgmentNumber.ToString();
            this.headerLen = tpPack.Header.Length.ToString();
            this.flags = tpPack.AllFlags;
            this.windowSize = tpPack.WindowSize.ToString();
            this.checkSum = tpPack.Checksum.ToString();
            this.data = byteToHexStr(tpPack.PayloadData);
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
