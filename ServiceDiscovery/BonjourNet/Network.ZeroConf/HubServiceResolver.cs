﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Network.ZeroConf
{
    internal class HubServiceResolver : IServiceResolver
    {
        List<IServiceResolver> resolvers = new List<IServiceResolver>();

        public void AddResolver(IServiceResolver resolver)
        {
            resolvers.Add(resolver);
            resolver.ServiceFound += resolver_ServiceFound;
            resolver.ServiceRemoved += resolver_ServiceRemoved;
        }

        public void RemoveResolver(IServiceResolver resolver)
        {
            resolvers.Add(resolver);
            resolver.ServiceFound -= resolver_ServiceFound;
            resolver.ServiceRemoved -= resolver_ServiceRemoved;
        }

        void resolver_ServiceRemoved(IService item)
        {
            if (ServiceRemoved != null)
                ServiceRemoved(item);
        }

        void resolver_ServiceFound(IService item)
        {
            if (ServiceFound != null)
                ServiceFound(item);
        }

        #region IServiceResolver Members

        public event ObjectEvent<IService> ServiceFound;

        public event ObjectEvent<IService> ServiceRemoved;

        public void Resolve(string protocol)
        {
            resolvers.ForEach(delegate(IServiceResolver resolver) { resolver.Resolve(protocol); });
        }

        public IList<IService> Resolve(string protocol, TimeSpan timeout, int minCountServices, int maxCountServices)
        {
            return new ResolverHelper().Resolve(this, protocol, timeout, 0, 10);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            resolvers.ForEach(delegate(IServiceResolver resolver) { resolver.Dispose(); });
        }

        #endregion
    }
}
