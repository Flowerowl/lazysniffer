using System;
using System.Collections.Generic;
using System.Text;
using PacketDotNet;
namespace Lazy_Sniffer
{
    public class LazyIpPacket
    {
        public PacketDotNet.IpPacket iPack;
        public string sourceIpAddress;
        public string destinIpAddress;
        public string ipProtoVersion;
        public string ipHeaderLen;
        public string totalDataLen;
        public string ttl;
        public string protocol;
        public string data;
        public LazyIpPacket(PacketDotNet.IpPacket ipPacket)
        {
            this.iPack = ipPacket;
            this.sourceIpAddress = iPack.SourceAddress.ToString();
            this.destinIpAddress = iPack.DestinationAddress.ToString();
            this.ipProtoVersion = iPack.Version.ToString();
            this.ipHeaderLen = (iPack.HeaderLength * 4).ToString();
            this.totalDataLen = iPack.TotalLength.ToString();
            this.ttl = iPack.TimeToLive.ToString();
            this.protocol = iPack.Protocol.ToString();
            this.data=byteToHexStr(iPack.PayloadData);

        }
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr="";
            if(bytes!=null)
            {
                for(int i=0;i<bytes.Length;i++)
                {
                    returnStr+=bytes[i].ToString("X2")+" ";
                }
            }
            return returnStr;
        }
    }
}
