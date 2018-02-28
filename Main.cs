using System;
using Octokit;
using System.IO;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Cloneit
{
    class Program
    {
        private static void Main() => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            var json = JObject.Parse(File.ReadAllText(@"appsettings.json"));
            var repositories = json.SelectToken("repositories");

            var rekal = new Rekal(json);
            
            foreach (dynamic repo in repositories)
            {
                await rekal.CloneRepos(repo.name.ToString(), repo.branch.ToString());
            }

            Console.WriteLine("All done. Thank you for choosing Rekal. 👍");
        }
    }
}
