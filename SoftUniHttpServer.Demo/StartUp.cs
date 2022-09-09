using SoftUniHttpServer.Server;
using SoftUniHttpServer.Server.HTTP;
using SoftUniHttpServer.Server.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SoftUniHttpServer.Demo
{
    public class StartUp
    {
        private const string HtmlForm = @"<form action='/HTML' method='POST'>
            Name: <input type='text' name='Name' />
            Age: <input type='number' name='Age' />
            <input type='submit' valu='Save' />
        </form>";

        private const string DownloadForm = @"<form action='/Content' method='POST'>
            <input type='submit' value ='Download Sites Content' /> 
        </form>";


        private const string FileName = "content.txt";

        public static async Task Main(string[] args)
        {
            await DownloadSitesAsTextFile(StartUp.FileName,
                new string[] { "https://judge.softuni.org/", "https://softuni.org/" });

            var server = new HttpServer(routes =>
                routes
                    .MapGet("/", new TextResponse("Hello from the server! :)"))
                    .MapGet("/HTML", new HtmlResponse(StartUp.HtmlForm))
                    .MapGet("/Redirect", new RedirectResponse("https:softuni.org"))
                    .MapPost("/HTML", new TextResponse("", StartUp.AddFormDataAction))
                    .MapGet("/Content", new HtmlResponse(StartUp.DownloadForm))
                    .MapPost("/Content", new TextFileResponse(StartUp.FileName))
                    .MapGet("/Cookies", new HtmlResponse("", StartUp.AddCookiesAction)));
                    

            await server.Start();
        }

        private static void AddCookiesAction(Request request, Response response)
        {
            var requestHasCookies = request.Cookies.Any();
            var bodyText = "";

            if (requestHasCookies)
            {
                var cookiesText = new StringBuilder();
                cookiesText.AppendLine("<h1>Cookies</h1>");

                cookiesText.Append("<table border='1'><tr><th>Name</th><th>Value</th></tr>");

                foreach (var cookie in request.Cookies)
                {
                    cookiesText.Append("<tr>");

                    cookiesText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Name)}</td>");
                    cookiesText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Value)}</td>");
                    cookiesText.Append("</tr>");
                }

                cookiesText.Append("</table>");

                bodyText = cookiesText.ToString();
            }
            else
            {
                bodyText = "<h1>Cookies set!</h1>";
            }

            if (!requestHasCookies)
            {
                response.Cookies.Add("My-Cookie", "My-Value");
                response.Cookies.Add("My-Second-Cookie", "My-Second-Value");
            }

            response.Body = bodyText;
        }

        private static void AddFormDataAction(
            Request request, Response response)
        {
            response.Body = "";

            foreach (var (key, value) in request.Form)
            {
                response.Body += $"{key} - {value}";
                response.Body += Environment.NewLine;
            }
        }

        private static async Task DownloadSitesAsTextFile(string fileName, string[] urls)
        {
            var downloads = new List<Task<string>>();

            foreach (var url in urls)
            {
                downloads.Add(DownloadWebSiteContent(url));
            }

            var responses = await Task.WhenAll(downloads);

            var responsesString = string.Join(
                Environment.NewLine + new string('-', 100), responses);

            await File.WriteAllTextAsync(fileName, responsesString);
        }

        private static async Task<string> DownloadWebSiteContent(string url)
        {
            using var httpClient = new HttpClient();

            var response = await httpClient.GetAsync(url);

            var html = await response.Content.ReadAsStringAsync();

            return html.Substring(0, 2000);
        }
    }
}
