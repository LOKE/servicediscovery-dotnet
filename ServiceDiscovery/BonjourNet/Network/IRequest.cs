using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Network
{
    internal interface IRequest
    {
    }

    internal interface IClientRequest : IRequest
    {
        void WriteTo(Stream stream);
        byte[] GetBytes();
    }

    internal interface IClientRequestWriter : IClientRequest
    {
        void WriteTo(BinaryWriter writer);
    }

    internal interface IServerRequest<RequestType> : IRequest
    {
        RequestType GetRequest(Stream stream);
        RequestType GetRequest(byte[] requestBytes);
    }

    internal interface IServerRequestReader<TRequest> : IServerRequest<TRequest>
    {
        TRequest GetRequest(BinaryReader writer);
    }
}
