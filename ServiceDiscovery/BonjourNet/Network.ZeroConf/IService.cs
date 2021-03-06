﻿using System;
using System.Collections.Generic;
using System.Text;
using Network.Dns;

namespace Network.ZeroConf
{
    internal interface IService : IExpirable
    {
        DomainName HostName { get; set; }
        IList<EndPoint> Addresses { get; }
        void AddAddress(EndPoint ep);
        string Protocol { get; set; }
        string Name { get; set; }
        string this[string key] { get; set; }
        IEnumerable<KeyValuePair<string, string>> Properties { get; }
        State State { get; }
        bool IsOutDated { get; }
        void Publish();
        void Stop();
        void Merge(IService service);
        void Resolve();
        /// <summary>
        /// Gets the best endpoint to connect to for the given service.
        /// </summary>
        /// <returns>An endpoint (IP + Port) that can be connected to</returns>
        EndPoint GetBestEndPoint();
    }

    internal enum State
    {
        Added,
        Removed,
        Updated,
        UpToDate
    }
}
