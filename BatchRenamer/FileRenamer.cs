using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BatchRenamer
{
    public class FileRenamer
    {
        public BindingList<string> FileNames { get; set; } = new BindingList<string>();

        public string FindString { get; set; } = "";
        public string ReplaceString { get; set; } = "";

        public bool IsCaseSensitive { get; set; } = true;
        public bool UseRegex { get; set; } = true;
        public bool ShowExtensions { get; set; } = false;

        public FileRenamer()
        {
            FileNames.AllowEdit = false;
        }

        public void AddFile(string path)
        {
            if (!FileNames.Contains(path))
                FileNames.Add(path);
        }

        public void RemoveFile(int index) => FileNames.RemoveAt(index);

        public void RenameFiles()
        {
            throw new NotImplementedException();
        }

        public string GetFileName(string path) =>
            ShowExtensions ? Path.GetFileName(path) : Path.GetFileNameWithoutExtension(path);

        public string GetReplacedName(string path)
        {
            string findRegex = UseRegex ? FindString : Regex.Escape(FindString);
            string replaceRegex = UseRegex ? ReplaceString : Regex.Escape(ReplaceString);
            RegexOptions options = IsCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
            string fileName = GetFileName(path);
            return Regex.Replace(fileName, findRegex, replaceRegex, options);
        }
    }
}
