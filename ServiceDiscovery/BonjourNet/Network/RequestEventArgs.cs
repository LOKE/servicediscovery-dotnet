using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Network
{
    internal class RequestEventArgs : RequestEventArgs<BinaryRequest, BinaryResponse>
    {
    }

    internal class RequestEventArgs<RequestType, ResponseType> : EventArgs
        where ResponseType : IResponse
    {
        public RequestEventArgs()
        {
            //Response = new HttpResponse();
        }

        public RequestType Request { get; set; }
        public ResponseType Response { get; set; }

        public virtual IPEndPoint Host { get; set; }
    }

    internal class ServerEventArgs<RequestType, ResponseType> : RequestEventArgs<RequestType, ResponseType>
        where RequestType : IServerRequest<RequestType>
        where ResponseType : IServerResponse
    {

    }

    internal class ClientEventArgs<RequestType, ResponseType> : RequestEventArgs<RequestType, ResponseType>
        where RequestType : IClientRequest
        where ResponseType : IClientResponse<ResponseType>
    {

    }
}
