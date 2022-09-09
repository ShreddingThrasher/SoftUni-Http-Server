using SoftUniHttpServer.Server.HTTP;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftUniHttpServer.Server.Responses
{
    public class RedirectResponse : Response
    {
        public RedirectResponse(string location)
            : base(StatusCode.Found)
        {
            this.Headers.Add(Header.Location, location);
        }
    }
}
