using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network.ZeroConf;

namespace ServiceDiscovery
{
    public class Service
    {
        public Service()
        {
            EndPoints = new List<EndPoint>();
        }

        public string Name { get; set; }
        public List<EndPoint> EndPoints { get; internal set; }

        public EndPoint GetBestEndPoint()
        {
            var endpoint = this.EndPoints.FirstOrDefault();
            return endpoint;
        }

        internal static Service FromIService(IService from)
        {
            var to = new Service();
            to.Name = from.Name;
            foreach (var address in from.Addresses)
            {
                foreach (var ip in address.Addresses)
                {
                    to.EndPoints.Add(new EndPoint()
                    {
                        IpAddress = ip.ToString(),
                        Port = address.Port
                    });
                }
            }
            return to;
        }
    }
}
