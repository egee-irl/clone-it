using System;
using Octokit;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Cloneit
{
    public class Rekal
    {
        private readonly GitHubClient _client;
        private readonly string _owner;
        private readonly string _token;
        private string _path;
        public Rekal(JToken json)
        {
            var username = json.SelectToken("username").ToString();
            
            _token = Environment.GetEnvironmentVariable("GIT_TOKEN");
            _owner = json.SelectToken("owner").ToString();
            _path = json.SelectToken("path").ToString();

            _client = new GitHubClient(new ProductHeaderValue(username));
            _client.Credentials = new Credentials(_token);
        }

        public async Task CloneRepos(string repoName, string branchName)
        {
            // /tmp/ is the default path. Oblviously this doesn't work on Windows (yet)...
            if (_path == "")
                _path = "/tmp/";
            var path = @_path + repoName + ".zip";

            // We use Octokit to authenticate and access to the repo
            var repo = await _client.Repository.Get(_owner, repoName);
            var downloadUrl = repo.SvnUrl + "/archive/" + branchName + ".zip";

            // The archive scraping happens with HttpClient
            using (var http = new HttpClient())
            {
                CredentialHelper(http);
                var contents = http.GetByteArrayAsync(downloadUrl).Result;
                try
                {
                    System.IO.File.WriteAllBytes(path, contents);
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message + " - Directory must exist first!");
                }                
            }
            try
            {
                ZipFile.ExtractToDirectory(path, _path);
            }
            catch (IOException e)
            {
                Console.WriteLine("WARNING: " + e.Message);
            }
        }
        // Since we can't pass authenticate from Octokit to the HttpClient, we have to auth a 2nd time 
        private void CredentialHelper(HttpClient http)
        {
            var credentials = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}:", _token);
            credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(credentials));
            http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
        }  
    }
}