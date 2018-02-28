using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Cloneit
{
    internal static class Program
    {
        // Async all the things!
        private static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();

        private static async Task MainAsync(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("You must pass a valid json blob as a single arguement!");
                Environment.Exit(1);
            }
             
            var json = JObject.Parse(File.ReadAllText(args[0]));
            var repositories = json.SelectToken("repositories");

            var rekal = new Rekal(json);
            
            // Dynamic works here because laziness and the json blob is a known quantity
            foreach (dynamic repo in repositories)
            {
                await rekal.CloneRepos(repo.name.ToString(), repo.branch.ToString());
            }

            Console.WriteLine("All done. Thank you for choosing Rekal. 👍");
        }
    }
}
