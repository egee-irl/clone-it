using System;
using Octokit;
using System.IO;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace cloneit
{
    class Program
    {
        static GitHubClient _client;
        static String _username;
        static String _owner;        
        static String _token;

        private static void Main() => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            var json = JObject.Parse(File.ReadAllText(@"appsettings.json"));
            _token = Environment.GetEnvironmentVariable("GIT_TOKEN");
            _username = json.SelectToken("username").ToString();
            _owner = json.SelectToken("owner").ToString();
            _client = new GitHubClient(new ProductHeaderValue(_username));
            
            _client.Credentials = new Credentials(_token);

            var repositories = json.SelectToken("repositories");
            
            foreach (dynamic repo in repositories)
            {
                await CloneRepos(repo.name.ToString(), repo.branch.ToString());
            }

            Console.WriteLine("Done.");
        }

        private static void CredentialHelper(HttpClient http)
        {
            var credentials = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}:", _token);
            credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(credentials));
            http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
        }

        private static async Task CloneRepos(string repoName, string branchName)
        {
            var path = @"/tmp/" + repoName + ".zip";
            var repo = await _client.Repository.Get(_owner, repoName);
            var downloadUrl = repo.SvnUrl + "/archive/" + branchName + ".zip";
            using (var http = new HttpClient())
            {
                CredentialHelper(http);
                var contents = http.GetByteArrayAsync(downloadUrl).Result;
                System.IO.File.WriteAllBytes(path, contents);
            }
            try
            {
                ZipFile.ExtractToDirectory(path, "/tmp/");
            }
            catch 
            {
                // System.IO.IOException: The file '/tmp/egeeio-master/.eslintrc.json' already exists.
            }
            
        }
    }
}
