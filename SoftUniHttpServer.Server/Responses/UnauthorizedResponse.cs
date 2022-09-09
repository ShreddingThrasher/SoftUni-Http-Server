using SoftUniHttpServer.Server.HTTP;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftUniHttpServer.Server.Responses
{
    class UnauthorizedResponse : Response
    {
        public UnauthorizedResponse()
            : base(StatusCode.Unauthorized)
        {

        }
    }
}
