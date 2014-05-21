using System.Collections.Generic;

namespace ServiceDiscovery
{
    public class EndPoint
    {
        public string IpAddress { get; internal set; }
        public uint Port { get; internal set; }
    }
}
