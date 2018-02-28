using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Cloneit
{
    internal static class Program
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
