using System;
using Octokit;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace cloneit
{
    class Program
    {
        static GitHubClient _client;
        static String _owner;

        private static void Main() => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            var json = JObject.Parse(File.ReadAllText(@"appsettings.json"));
            _owner = json.SelectToken("username").ToString();
            _client = new GitHubClient(new ProductHeaderValue(_owner));

            var repositories = json.SelectToken("repositories");
            
            foreach (dynamic repo in repositories)
            {
                await CloneRepos(repo.name.ToString(), repo.branch.ToString());
            }

            Console.WriteLine("Done.");
        }

        private static async Task CloneRepos(string repoName, string branchName)
        {
            var path = @"/tmp/" + repoName + ".zip";
            var repo = await _client.Repository.Get(_owner, repoName);
            var downloadUrl = repo.SvnUrl + "/archive/" + branchName + ".zip";

            using (var http = new System.Net.Http.HttpClient())
            {
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
