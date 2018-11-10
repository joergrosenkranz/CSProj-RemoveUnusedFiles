using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Mono.Options;

namespace RemoveUnusedFiles
{
    public class ShowCommand : Command
    {
        public ShowCommand()
        : base("show", "Show files in the project directory that are not referenced by the project.")
        {
            Options = new OptionSet
            {
                "Usage: show <file1.csproj> <file2.csproj> ...",
                "",
                { "help|h|?", "Show this message and exit.", v => ShowHelp = v != null }
            };
        }

        public bool ShowHelp { get; set; }

        public override int Invoke(IEnumerable<string> args)
        {
            var projects = Options.Parse(args);
            if (ShowHelp)
            {
                Options.WriteOptionDescriptions(CommandSet.Out);
                return 0;
            }

            if (projects.Count == 0)
            {
                Log.Error("Missing mandatory project file parameter.");
                return 1;
            }

            var ret = 0;
            foreach (var proj in projects)
            {
                if ( ! Project.CheckProjectFile(proj) )
                {
                    ret = 1;
                    continue;
                }

                var unused = Project.EnumerateUnusedFiles(proj);

                foreach (var file in unused)
                    Console.WriteLine(Path.GetRelativePath(Environment.CurrentDirectory, file));
            }

            return ret;
        }
    }
}
