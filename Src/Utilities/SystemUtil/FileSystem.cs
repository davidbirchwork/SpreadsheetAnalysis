using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Utilities.SystemUtil {
    public static class FileSystem {

        public static void CopyFilesRecursively(string source, string target, bool overwrite = true) {
            CopyFilesRecursively(new DirectoryInfo(source), new DirectoryInfo(target), overwrite);
        }

        /// <summary>
        /// Copies the files recursively.
        /// from http://stackoverflow.com/questions/58744/best-way-to-copy-the-entire-contents-of-a-directory-in-c
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="overwrite">overwrite files</param>
        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target, bool overwrite) {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name), overwrite);
            foreach (FileInfo file in source.GetFiles())        
                file.CopyTo(Path.Combine(target.FullName, file.Name),overwrite);
        }

        public static IEnumerable<Tuple<string,DateTime>> GetDirectoryLastModifiedTimes(string directory) {
            return
                Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories).Select(
                    file => new Tuple<string, DateTime>(file, File.GetLastWriteTime(file)));
        }

        public static void DeleteAllFilesInDirectory(string directory) {
            foreach (var file in Directory.EnumerateFiles(directory).ToArray()) {
                File.Delete(file);
            }
        }
    }
}
