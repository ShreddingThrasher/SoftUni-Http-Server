using SoftUniHttpServer.Server.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftUniHttpServer.Server.HTTP
{
    public class Cookie
    {
        public Cookie(string name, string value)
        {
            Guard.AgainstNull(name);
            Guard.AgainstNull(value);

            this.Name = name;
            this.Value = value;
        }
        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"{this.Name}={this.Value}";
        }
    }
}
