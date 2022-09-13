using System;
using System.Collections.Generic;
using System.Text;

namespace SoftUniHttpServer.Server.HTTP
{
    public class Response
    {
        public Response(StatusCode statusCode)
        {
            this.StatusCode = statusCode;
            this.Headers.Add(Header.Server, "My Web Server");
            this.Headers.Add(Header.Date, $"{DateTime.UtcNow:r}");
        }

        public StatusCode StatusCode { get; }
        public HeaderCollection Headers { get; } = new HeaderCollection();
        public CookieCollection Cookies { get; } = new CookieCollection();
        public string Body { get; set; }
        public byte[] FileContent { get; set; }
        public Action<Request, Response> PreRenderAction { get; protected set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"HTTP/1.1 {(int)this.StatusCode} {this.StatusCode}");

            foreach (var header in this.Headers)
            {
                sb.AppendLine(header.ToString());
            }

            foreach (var cookie in this.Cookies)
            {
                sb.AppendLine($"{Header.SetCookie}: {cookie}");
            }

            sb.AppendLine();

            if (!string.IsNullOrEmpty(this.Body))
            {
                sb.AppendLine(this.Body);
            }

            return sb.ToString();
        }
    }
}
