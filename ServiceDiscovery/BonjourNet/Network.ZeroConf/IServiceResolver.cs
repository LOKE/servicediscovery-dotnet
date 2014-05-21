using System;
using System.Collections.Generic;
using System.Text;

namespace Network.ZeroConf
{
    internal delegate void ObjectEvent<T>(T item);


    internal interface IServiceResolver : IDisposable
    {
        event ObjectEvent<IService> ServiceFound;
        event ObjectEvent<IService> ServiceRemoved;
        void Resolve(string protocol);
        IList<IService> Resolve(string protocol, TimeSpan timeout, int minCountServices, int maxCountServices);
    }
}
