using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Mono.Options;

namespace RemoveUnusedFiles
{
    public class DeleteCommand : Command
    {
        public DeleteCommand() 
        : base("clean", "Delete files in the project directory that are not referenced by the project.")
        {
            Options = new OptionSet 
            {
                "Usage: clean <file1.csproj> <file2.csproj> ...",
                "",
                { "help|h|?", "Show this message and exit.", v => ShowHelp = v != null },
                { "f", "Force delete. Do not request approval.", v => Force = v != null }
            };
        }

        public bool ShowHelp { get; set; }
        public bool Force { get; set; }

        public override int Invoke(IEnumerable<string> args)
        {
            var projects = Options.Parse(args);
            if (ShowHelp)
            {
                Options.WriteOptionDescriptions(CommandSet.Out);
                return 0;
            }

            if ( projects.Count == 0 )
            {
                Log.Error("Missing mandatory project file parameter.");
                return 1;
            }

            var unused = projects
                .Where(Project.CheckProjectFile)
                .SelectMany(Project.EnumerateUnusedFiles)
                .ToList();

            if ( unused.Count == 0 )
            {
                Log.Info("Nothing to delete.");
                return 0;
            }

            if ( ! Force )
            {
                Console.Write($"Do you really want to delete {unused.Count} files? (y/N): ");
                var answer = Console.Read();

                if ( answer != (int)'Y' && answer != (int)'y' )
                    return 1;
            }

            foreach (var file in unused)
            {
                Console.WriteLine("Deleting {0}...", Path.GetRelativePath(Environment.CurrentDirectory, file));
                File.Delete(file);
            }

            return 0;
        }
    }
}
