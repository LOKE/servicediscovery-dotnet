﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    internal abstract class Command<RequestEventArgsType, RequestType, ResponseType>
        where RequestEventArgsType : RequestEventArgs<RequestType, ResponseType>
        where ResponseType : IResponse
    {
        public Command()
        {

        }

        public abstract Command<RequestEventArgsType, RequestType, ResponseType> Initialize(RequestType request, IServiceProvider server);

        protected abstract RequestType BuildRequest();

        public virtual ResponseType GetResponse()
        {
            return GetResponse(true);
        }

        public abstract ResponseType GetResponse(bool expectResponse);

        public abstract void Execute(RequestEventArgsType e);
    }

    internal abstract class Command<RequestType, ResponseType> :
        Command<RequestEventArgs<RequestType, ResponseType>, RequestType, ResponseType>
        where ResponseType : IResponse
    {
    }
}
