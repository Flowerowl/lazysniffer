using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lazy_Sniffer
{
    public class LazyEthernetPacket
    {
        public PacketDotNet.EthernetPacket epacket;
        public string sourceHwAddress;
        public string destinHwAddress;
        public string ethProto;
        public LazyEthernetPacket(PacketDotNet.EthernetPacket packet)
        {
            this.epacket = packet;
            this.sourceHwAddress = epacket.SourceHwAddress.ToString();
            this.destinHwAddress = epacket.DestinationHwAddress.ToString();
            this.ethProto = epacket.Type.ToString();
        }
    }
}
