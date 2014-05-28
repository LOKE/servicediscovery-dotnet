using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Network.Bonjour;
using Network.ZeroConf;

namespace AstonClub.ServiceDiscovery
{
    public class Browser: IDisposable
    {
        private string _serviceName;
        private AutoResetEvent _waitEvent;

        public Browser(string serviceName)
        {
            _serviceName = serviceName;
            _services = new Dictionary<string, IService>();
            _waitEvent = new AutoResetEvent(false);
        }

        public void Start()
        {
            if (_resolver != null) Stop();
            _resolver = new BonjourServiceResolver();
            _resolver.ServiceFound += ServiceFound;
            _resolver.ServiceRemoved += ServiceRemoved;
            _resolver.Resolve("_" + _serviceName + "._tcp");
            _waitEvent.Reset();
        }

        public void Stop()
        {
            if (_resolver == null) return;
            _resolver.Dispose();
            _resolver = null;
            _waitEvent.Reset();
        }

        public event EventHandler ServicesUpdated;

        public Service WaitForService(int timeout)
        {
            bool ok = _waitEvent.WaitOne(timeout);

            if (!ok) return null;

            var item = _services.First().Value;
            return Service.FromIService(item);
        }

        private void OnServicesUpdated()
        {
            if (ServicesUpdated != null) ServicesUpdated(this, EventArgs.Empty);
        }

        private readonly Dictionary<string, IService> _services;

        public IEnumerable<Service> Services
        {
            get { return _services.Select(s => Service.FromIService(s.Value)); }
        }

        private void ServiceFound(IService item)
        {
            _services[item.Name + "." + item.Protocol] = item;
            Debug.Print("Found a service " + item.Name + " with hostname " + item.HostName);
            if (!item.Addresses.Any()) return;
            foreach (var address in item.Addresses[0].Addresses)
            {
                if (address.IsIPv6Teredo) continue; // skip ipv6
                Debug.Print("Found a service on " + address + ":" + item.Addresses[0].Port);
            }
            _waitEvent.Set();
            OnServicesUpdated();
        }
        private void ServiceRemoved(IService item)
        {
            string key = item.Name + "." + item.Protocol;
            if (!_services.ContainsKey(key)) return;

            Debug.Print("Removed a service " + " with hostname " + item.HostName);

            _services.Remove(key);
            if (!_services.Any()) _waitEvent.Reset();
            OnServicesUpdated();
        }

        public void Dispose()
        {
            Stop();
        }

        private BonjourServiceResolver _resolver;
    }
}
