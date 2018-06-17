using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BatchRenamer
{
    public class FileRenamer
    {
        protected List<string> OriginalFiles { get; set; } = new List<string>();
        protected IEnumerable<string> FileNames => OriginalFiles.Select(
            path => ShowExtensions ? Path.GetFileName(path) : Path.GetFileNameWithoutExtension(path));
        protected IEnumerable<string> NewNames => FileNames.Select(name => ReplaceName(name));

        public string FindString { get; set; } = "";
        public string ReplaceString { get; set; } = "";

        public bool IsCaseSensitive { get; set; } = true;
        public bool UseRegex { get; set; } = true;
        public bool ShowExtensions { get; set; } = false;

        public BindingList<FileNameInfo> DisplayData { get; protected set; } = new BindingList<FileNameInfo>();

        public FileRenamer()
        {

        }

        public void AddFile(string path)
        {
            if (!OriginalFiles.Contains(path))
            {
                OriginalFiles.Add(path);
                string oldName = ShowExtensions ? Path.GetFileName(path) : Path.GetFileNameWithoutExtension(path);
                string newName = ReplaceName(oldName);
                DisplayData.Add(new FileNameInfo(path, oldName, newName));
            }
        }

        public void RemoveFile(string path)
        {
            int index = OriginalFiles.IndexOf(path);
            RemoveFile(index);
        }

        public void RemoveFile(int index)
        {
            OriginalFiles.RemoveAt(index);
            //DisplayData.RemoveAt(index);
        }

        public void RenameFiles()
        {

        }

        protected string ReplaceName(string original)
        {
            if (UseRegex)
            {
                if (IsCaseSensitive)
                    return Regex.Replace(original, FindString, ReplaceString);
                else
                    return Regex.Replace(original, FindString, ReplaceString, RegexOptions.IgnoreCase);
            }
            else
            {
                if (IsCaseSensitive)
                    return original.Replace(FindString, ReplaceString);
                else
                    return Regex.Replace(original, Regex.Escape(FindString), Regex.Escape(ReplaceString), RegexOptions.IgnoreCase);
            }
        }
    }
}
