using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BatchRenamer
{
    public static class FileRenamer
    {
        public delegate string Renamer(string original, ReplaceOptions options);

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
            return Regex.Replace(original, options.Find, options.Replace, regexOptions);
        }

        public static string RenameFile(string path, ReplaceOptions options, Renamer renamer, bool renameExtension)
        {
            string directory = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);

            return renameExtension ?
                 Path.Combine(directory, renamer(fileName + extension, options)) :
                 Path.Combine(directory, renamer(fileName, options) + extension);
        }

        public static string DisplayName(string path, bool showExtension)
        {
            return showExtension ?
                Path.GetFileName(path) :
                Path.GetFileNameWithoutExtension(path);
        }

        public static bool IsInvalidFileName(string path)
        {
            return Path.GetInvalidFileNameChars()
                .Any(c => path.Contains(c));
        }

        public static bool IsInvalidPath(string path)
        {
            return Path.GetInvalidPathChars()
                .Any(c => path.Contains(c));
        }
    }
}
