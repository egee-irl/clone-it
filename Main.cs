using System;
using Octokit;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace cloneit
{
    class Program
    {
        static GitHubClient _client;
        static String _owner;
        private static void Main()
        {
            _owner = "egee-irl";
            _client = new GitHubClient(new ProductHeaderValue(_owner));

            var repos = new string[]{
                "egeeio",
                "refbot-csharp"};

            var branchName = "master";
            
            foreach (string repoName in repos)
            {
                CloneRepos(repoName, branchName);
            }

            Console.WriteLine("Done.");
        }

        private static async void CloneRepos(string repoName, string branchName)
        {
            var path = @"/tmp/" + repoName + ".zip";
            var repo = await _client.Repository.Get(_owner, repoName);
            var downloadUrl = repo.SvnUrl + "/archive/" + branchName + ".zip";

            using (var http = new System.Net.Http.HttpClient())
            {
                var contents = http.GetByteArrayAsync(downloadUrl).Result;
                System.IO.File.WriteAllBytes(path, contents);
            }

            ZipFile.ExtractToDirectory(path, "/tmp/");
        }
    }
}
