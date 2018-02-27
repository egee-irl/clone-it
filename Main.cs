using System;
using Octokit;
using System.IO.Compression;
using System.Threading.Tasks;

namespace cloneit
{
    class Program
    {
        private static void Main() => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            var owner = "egee-irl";
            var reponame = "egeeio";
            var branchName = "asp";
            var path = @"/tmp/test.zip";

            var client = new GitHubClient(new ProductHeaderValue(owner));
            var repo = await client.Repository.Get(owner, reponame);
            var downloadUrl = repo.SvnUrl + "/archive/" + branchName + ".zip";

            using (var http = new System.Net.Http.HttpClient())
            {
                var contents = http.GetByteArrayAsync(downloadUrl).Result;
                System.IO.File.WriteAllBytes(path, contents);
            }
            ZipFile.ExtractToDirectory(path, "/tmp/farts");
        }
    }
}
