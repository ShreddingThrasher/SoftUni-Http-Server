using SoftUniHttpServer.Server.Common;
using SoftUniHttpServer.Server.HTTP;
using SoftUniHttpServer.Server.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftUniHttpServer.Server.Routing
{
    public class RoutingTable : IRoutingTable
    {
        private readonly Dictionary<Method, Dictionary<string, Response>> routes;

        public RoutingTable()
        {
            this.routes = new Dictionary<Method, Dictionary<string, Response>>()
            {
                [Method.Get] = new Dictionary<string, Response>(StringComparer.InvariantCulture),
                [Method.Post] = new Dictionary<string, Response>(StringComparer.InvariantCulture),
                [Method.Put] = new Dictionary<string, Response>(StringComparer.InvariantCulture),
                [Method.Delete] = new Dictionary<string, Response>(StringComparer.InvariantCulture)
            };
        }

        public IRoutingTable Map(string url, Method method, Response response)
            => method switch
            {
                Method.Get => this.MapGet(url, response),
                Method.Post => this.MapPost(url, response),
                _ => throw new InvalidOperationException($"Method '{method}' is not supported.")
            };

        public IRoutingTable MapGet(string url, Response response)
        {
            Guard.AgainstNull(url, nameof(url));
            Guard.AgainstNull(response, nameof(response));

            this.routes[Method.Get][url] = response;

            return this;
        }

        public IRoutingTable MapPost(string url, Response response)
        {
            Guard.AgainstNull(url, nameof(url));
            Guard.AgainstNull(response, nameof(response));

            this.routes[Method.Post][url] = response;

            return this;
        }

        public Response MatchRequest(Request request)
        {
            var requestMethod = request.Method;
            var requestUrl = request.Url;

            if(!this.routes.ContainsKey(requestMethod)
                    || !this.routes[requestMethod].ContainsKey(requestUrl))
            {
                return new NotFoundResponse();
            }

            return this.routes[requestMethod][requestUrl];
        }
    }
}
