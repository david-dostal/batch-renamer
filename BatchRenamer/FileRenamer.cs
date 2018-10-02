using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BatchRenamer
{
    public static class FileRenamer
    {
        public delegate string Renamer(string original, ReplaceOptions options);
        private static RegexUtils regexUtils = new RegexUtils();

        public static string StringReplace(string original, ReplaceOptions options)
        {
            string findEscaped = Regex.Escape(options.Find);
            string replaceEscaped = Regex.Escape(options.Replace);
            RegexOptions regexOptions = options.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
            string replaced = Regex.Replace(original, findEscaped, replaceEscaped, regexOptions);
            return Regex.Unescape(replaced);
        }

        public static string RegexReplace(string original, ReplaceOptions options)
        {
            RegexOptions regexOptions = options.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
            // cache last failed pattern to prevent excessive exceptions
            return regexUtils.CachedTryReplace(original, options.Find, options.Replace, regexOptions, out bool success);
        }

        public static string RenamePath(string path, ReplaceOptions options, Renamer renamer, bool renameExtension)
        {
            string directory = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);
            return $"{directory}{Path.DirectorySeparatorChar}{RenameFile(fileName, options, renamer, renameExtension)}";
            // can't use Path.Combine, because it throws an exception when fileName is invalid
        }

        public static string RenameFile(string fileName, ReplaceOptions options, Renamer renamer, bool renameExtension)
        {
            string name = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);

            return renameExtension ?
                renamer(name + extension, options) :
                renamer(name, options) + extension;
        }

        public static string DisplayName(string path, bool showExtension)
        {
            return showExtension ?
                Path.GetFileName(path) :
                Path.GetFileNameWithoutExtension(path);
        }

        public static bool IsInvalidFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars()
                .Any(c => fileName.Contains(c));
        }
    }
}
