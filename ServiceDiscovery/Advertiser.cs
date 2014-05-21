using System;
using System.Collections.Generic;
using Network.Bonjour;
using Network.ZeroConf;

namespace ServiceDiscovery
{
    public class Advertiser : IDisposable
    {
        //public Advertiser(string serviceName, ushort? port = null)
        public Advertiser(string serviceName, ushort port)
            : this(serviceName, Guid.NewGuid().ToString(), port)
        {
        }

        //public Advertiser(string serviceName, string name, ushort? port = null)
        public Advertiser(string serviceName, string name, ushort port)
        {
            _service = new Network.Bonjour.Service();
            var add = ResolverHelper.GetEndPoint();
            //if (port.HasValue) add.Port = port.Value;
            add.Port = port;
            _service.AddAddress(add);
            _service.Protocol = "_"+serviceName+"._tcp.local.";
            _service.Name = name;
            _service.HostName = _service.Addresses[0].DomainName;
        }

        //public Advertiser(string serviceName, string name, Dictionary<string, string> metadata, ushort? port = null)
        public Advertiser(string serviceName, string name, Dictionary<string, string> metadata, ushort port)
            : this(serviceName, name, port)
        {
            foreach (var item in metadata)
            {
                _service[item.Key] = item.Value;
            }
        }

        public void Start()
        {
            //After setting all this, the only thing left to do is to publish your service
            _service.Publish();            
        }

        public void Stop()
        {
            _service.Renew(0);
            //_service.Stop();
        }

        public void Dispose()
        {
            this.Stop();
        }

        private readonly Network.Bonjour.Service _service;
    }
}
