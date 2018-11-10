using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;

namespace RemoveUnusedFiles
{
    public static class Project
    {
        private static readonly bool _replaceBackslash = Path.DirectorySeparatorChar != '\\';

        public static bool CheckProjectFile(string proj)
        {
            var fullpath = Path.GetFullPath(proj);

            if ( ! File.Exists(fullpath) )
            {
                Log.Error("The file {0} does not exist.", fullpath);
                return false;
            }
            
            var doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.Load(proj);

            if ( !string.Equals(doc.DocumentElement.Name, "Project", StringComparison.Ordinal) ||
                 !string.Equals(doc.DocumentElement.NamespaceURI, "http://schemas.microsoft.com/developer/msbuild/2003", StringComparison.Ordinal))
            {
                Log.Error("No valid project file: {0}", proj);
                return false;
            }

            return true;
        }

        public static IEnumerable<string> EnumerateUnusedFiles(string proj)
        {
            var fullpath = Path.GetFullPath(proj);
            var baseDir = Path.GetDirectoryName(fullpath);

            var existing = Project.EnumerateExisting(fullpath);
            var project = Project.EnumerateProjectFiles(fullpath);

            return existing
                .Except(project, StringComparer.OrdinalIgnoreCase);
        }

        public static IEnumerable<string> EnumerateProjectFiles(string proj)
        {
            var fullpath = Path.GetFullPath(proj);
            var baseDir = Path.GetDirectoryName(fullpath);

            var doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.Load(proj);

            var mgr = new XmlNamespaceManager(doc.NameTable);
            mgr.AddNamespace("ms", doc.DocumentElement.NamespaceURI);

            // Sources
            var types = new string[] { "Compile", "Content", "None", "EmbeddedResource", "TypeScriptCompile" };

            foreach (string type in types)
            {
                foreach (XmlAttribute attr in doc.SelectNodes("ms:Project/ms:ItemGroup/ms:" + type + "/@Include", mgr))
                    yield return _NormalizePath(Path.Combine(baseDir, attr.Value));
            }
        }

        public static IEnumerable<string> EnumerateExisting(string proj)
        {
            var fullpath = Path.GetFullPath(proj);
            var baseDir = Path.GetDirectoryName(fullpath);

            var binPrefix = Path.Combine(baseDir, "bin") + Path.DirectorySeparatorChar;
            var objPrefix = Path.Combine(baseDir, "obj") + Path.DirectorySeparatorChar;

            var files = Directory.EnumerateFiles(baseDir, "*.cs", SearchOption.AllDirectories)
                .Concat(Directory.EnumerateFiles(baseDir, "*.resx", SearchOption.AllDirectories));

            return files
                .Where(f => !f.StartsWith(binPrefix, StringComparison.OrdinalIgnoreCase) &&
                            !f.StartsWith(objPrefix, StringComparison.OrdinalIgnoreCase));
        }

        private static string _NormalizePath(string path)
        {
            if (_replaceBackslash)
                path = path.Replace('\\', Path.DirectorySeparatorChar);

            return Path.GetFullPath(path);
        }
    }
}