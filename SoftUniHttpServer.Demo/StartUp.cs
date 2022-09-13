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


        private const string FileName = "test.docx";

        private const string LoginForm = @"<form action='/Login' method='POST'>
            Username: <input type='text' name='Username'/>
            Password: <input type='text' name='Password'/>
            <input type='submit' value ='Log In' /> 
        </form>";

        private const string Username = "user";
        private const string Password = "user123";

        public static async Task Main(string[] args)
        {
            //await DownloadSitesAsTextFile(StartUp.FileName,
            //    new string[] { "https://judge.softuni.org/", "https://softuni.org/" });

            var server = new HttpServer(routes =>
                routes
                    .MapGet("/", new TextResponse("Hello from the server! :)"))
                    .MapGet("/HTML", new HtmlResponse(StartUp.HtmlForm))
                    .MapGet("/Redirect", new RedirectResponse("https:softuni.org"))
                    .MapPost("/HTML", new TextResponse("", StartUp.AddFormDataAction))
                    .MapGet("/Content", new HtmlResponse(StartUp.DownloadForm))
                    .MapPost("/Content", new FileResponse(StartUp.FileName))
                    .MapGet("/Cookies", new HtmlResponse("", StartUp.AddCookiesAction))
                    .MapGet("/Session", new TextResponse("", StartUp.DisplaySessionInfoAction))
                    .MapGet("/Login", new HtmlResponse(StartUp.LoginForm))
                    .MapPost("/Login", new HtmlResponse("", StartUp.LoginAction))
                    .MapGet("/Logout", new HtmlResponse("", StartUp.LogoutAction))
                    .MapGet("/UserProfile", new HtmlResponse("", StartUp.GetUserDataAction)));
                    

            await server.Start();
        }

        private static void GetUserDataAction(Request request, Response response)
        {
            if (request.Session.ContainsKey(Session.SessionUserKey))
            {
                response.Body = "";
                response.Body += $"<h3>Currently logged-in user " +
                    $"Is with username '{Username}'</h3>";
            }
            else
            {
                response.Body = "";
                response.Body += $"<h3>You should first log in " +
                    $"- <a href='/Login'>Login</a></h3>";
            }
        }

        private static void LogoutAction(Request request, Response response)
        {
            //user for debug
            var sessionBeforeClean = request.Session;

            request.Session.Clear();

            //user for debug
            var sessionAfterClean = request.Session;

            response.Body = "";
            response.Body += "<h3>Logged out successfully!</h3>";
        }

        private static void LoginAction(Request request, Response response)
        {
            request.Session.Clear();

            //used for debug
            var sessionBeforeLogin = request.Session;

            var bodyText = "";

            var usernameMatches = request.Form["Username"] == StartUp.Username;
            var passwordMatches = request.Form["Password"] == StartUp.Password;

            if(usernameMatches && passwordMatches)
            {
                request.Session[Session.SessionUserKey] = "MyUserId";
                response.Cookies.Add(Session.SessionCookieName, request.Session.Id);

                bodyText = "<h3>Logged successfully!</h3>";

                //user for debug
                var sessionAfterLogin = request.Session;
            }
            else
            {
                bodyText = StartUp.LoginForm;
            }

            response.Body = "";
            response.Body += bodyText;
        }

        private static void DisplaySessionInfoAction(Request request, Response response)
        {
            var sessionExists = request.Session.ContainsKey(Session.SessionCurrentDateKey);

            var bodyText = "";

            if (sessionExists)
            {
                var currentDate = request.Session[Session.SessionCurrentDateKey];
                bodyText = $"Stored date: {currentDate}";
            }
            else
            {
                bodyText = "Current date stored!";
            }

            response.Body = "";
            response.Body += bodyText;
        }

        private static void AddCookiesAction(Request request, Response response)
        {
            var requestHasCookies = request.Cookies
                .Any(c => c.Name != Session.SessionCookieName);

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
