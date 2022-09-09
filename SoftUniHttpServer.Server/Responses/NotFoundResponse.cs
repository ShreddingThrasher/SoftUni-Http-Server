using SoftUniHttpServer.Server.HTTP;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftUniHttpServer.Server.Responses
{
    public class NotFoundResponse : Response
    {
        public NotFoundResponse()
            : base(StatusCode.NotFound)
        {

        }
    }
}
