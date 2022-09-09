using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftUniHttpServer.Server.HTTP
{
    public class Request
    {
        public Method Method { get; private set; }
        public string Url { get; private set; }
        public HeaderCollection Headers { get; private set; }
        public CookieCollection Cookies { get; private set; }
        public string Body { get; private set; }
        public IReadOnlyDictionary<string, string> Form { get; private set; }


        public static Request Parse(string request)
        {
            var lines = request.Split("\r\n");

            var firstLine = lines.First().Split(" ");

            Method method = ParseMethod(firstLine[0]);
            
            var url = firstLine[1];

            HeaderCollection headers = ParseHeaders(lines.Skip(1));

            CookieCollection cookies = ParseCookies(headers);

            var bodyLines = lines.Skip(headers.Count + 2).ToArray();

            var body = string.Join("\r\n", bodyLines);

            var form = ParseForm(headers, body);

            return new Request()
            {
                Method = method,
                Url = url,
                Headers = headers,
                Cookies = cookies,
                Body = body,
                Form = form
            };
        }

        private static Method ParseMethod(string method)
        {
            try
            {
                return Enum.Parse<Method>(method, true);
            }
            catch (Exception)
            {
                throw new InvalidOperationException($"Method '{method}' is not supported");
            }
        }

        private static CookieCollection ParseCookies(HeaderCollection headers)
        {
            var cookies = new CookieCollection();

            if (headers.Contains(Header.Cookie))
            {
                string cookieHeader = headers[Header.Cookie];
                string[] allCookies = cookieHeader.Split(";", 
                    StringSplitOptions.RemoveEmptyEntries);

                foreach (string cookie in allCookies)
                {
                    string[] values = cookie.Split('=', StringSplitOptions.RemoveEmptyEntries);

                    cookies.Add(values[0]?.Trim(), values[1]?.Trim());
                }
            }

            return cookies;
        }

        private static HeaderCollection ParseHeaders(IEnumerable<string> headerLines)
        {
            var headerCollection = new HeaderCollection();

            foreach (var headerLine in headerLines)
            {
                if(headerLine == string.Empty)
                {
                    break;
                }

                var headerParts = headerLine.Split(":", 2);

                if(headerParts.Length != 2)
                {
                    throw new InvalidOperationException("Request is not valid.");
                }

                var headerName = headerParts[0];
                var headerValue = headerParts[1].Trim();

                headerCollection.Add(headerName, headerValue);
            }

            return headerCollection;
        }

        private static Dictionary<string, string> ParseForm(
            HeaderCollection headers, string body)
        {
            var formCollection = new Dictionary<string, string>();

            if(headers.Contains(Header.ContentType)
                && headers[Header.ContentType] == ContentType.FormUrlEncoded)
            {
                var parsedResult = ParseFormData(body);

                foreach (var (name, value) in parsedResult)
                {
                    formCollection.Add(name, value);
                }
            }

            return formCollection;
        }

        private static Dictionary<string, string> ParseFormData(string bodyLines)
            => HttpUtility.UrlDecode(bodyLines)
            .Split('&')
            .Select(part => part.Split('='))
            .Where(part => part.Length == 2)
            .ToDictionary(
                part => part[0],
                part => part[1],
                StringComparer.InvariantCultureIgnoreCase);
    }
}
