using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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

            // we want to prioritise IPs on our networks
            item.Addresses[0].Addresses = SortIps(item.Addresses[0].Addresses);
            foreach (var address in item.Addresses[0].Addresses)
            {
                if (address.IsIPv6Teredo) continue; // skip ipv6
                Debug.Print("Found a service on " + address + ":" + item.Addresses[0].Port);
            }
            _waitEvent.Set();
            OnServicesUpdated();
        }

        private class Ip
        {
            public IPAddress Address { get; set; }

            public IPAddress Mask { get; set; }

            public bool IsSameNetworkAs(IPAddress address)
            {
                IPAddress network1 = GetNetworkAddress(address, Mask);
                IPAddress network2 = GetNetworkAddress(Address, Mask);

                return network1.Equals(network2);
            }

            private IPAddress GetNetworkAddress(IPAddress address, IPAddress subnetMask)
            {
                byte[] ipAdressBytes = address.GetAddressBytes();
                byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

                if (ipAdressBytes.Length != subnetMaskBytes.Length)
                    throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

                byte[] broadcastAddress = new byte[ipAdressBytes.Length];
                for (int i = 0; i < broadcastAddress.Length; i++)
                {
                    broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
                }
                return new IPAddress(broadcastAddress);
            }
        }

        private List<Ip> _ips;

        private List<Ip> Ips
        {
            get { return _ips ?? GetIps(); }
        }

        private List<Ip> GetIps()
        {
            var ips = new List<Ip>();

            foreach (NetworkInterface iface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (iface.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (var address in iface.GetIPProperties().UnicastAddresses)
                    {
                        if (address.Address.IsIPv6LinkLocal || System.Net.IPAddress.IsLoopback(address.Address))
                            continue;

                        ips.Add(new Ip()
                        {
                            Address = address.Address, Mask = address.IPv4Mask
                        });
                    }
                }
            }

            return ips;
        }

        private IList<IPAddress> SortIps(IList<IPAddress> addresses)
        {
            var myIps = Ips;
            var priorityIps = new List<IPAddress>();
            var secondaryIps = new List<IPAddress>();

            foreach (var address in addresses)
            {
                if (!address.IsIPv6Teredo && IsInNetwork(myIps, address)) priorityIps.Add(address);
                else secondaryIps.Add(address);
            }

            priorityIps.AddRange(secondaryIps);

            return priorityIps;
        }

        private bool IsInNetwork(IEnumerable<Ip> myIps, IPAddress addressToCheck)
        {
            return myIps.Any(ip => ip.IsSameNetworkAs(addressToCheck));
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
