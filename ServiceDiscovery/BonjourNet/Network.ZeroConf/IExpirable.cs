using System;
using System.Collections.Generic;
using System.Text;

namespace Network.ZeroConf
{
    internal interface IExpirable
    {
        bool IsOutDated { get; }
        uint Ttl { get; }
        void Renew(uint ttl);
    }
}
