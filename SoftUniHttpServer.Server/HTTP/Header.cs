using SoftUniHttpServer.Server.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftUniHttpServer.Server.HTTP
{
    public class Header
    {
        public const string ContentType = "Content-Type";
        public const string ContentLength = "Content-Length";
        public const string ContentDisposition = "Content-Disposition";
        public const string Cookie = "Cookie";
        public const string Date = "Date";
        public const string Location = "Location";
        public const string Server = "Server";
        public const string SetCookie = "Set-Cookie";

        public Header(string name, string value)
        {
            Guard.AgainstNull(name);
            Guard.AgainstNull(value);

            this.Name = name;
            this.Value = value;
        }

        public string Name { get; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"{this.Name}: {this.Value}";
        }
    }
}
