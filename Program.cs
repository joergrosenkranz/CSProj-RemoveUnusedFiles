using System;
using Mono.Options;

namespace RemoveUnusedFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var options = new CommandSet("dotnet-unusedfiles")
                {
                    "Show or remove unused files in a VS 2015 or VS 2017 project",
                    "",
                    "Usage: dotnet-unused <command> [<args>]",
                    "",
                    new ShowCommand(),
                    new DeleteCommand()
                };

                options.Run(args);
            }
            catch (Exception ex)
            {
                Log.Error($"Error occured: {ex.Message}");
            }
        }
    }
}
