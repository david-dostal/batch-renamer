using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BatchRenamer
{
    public class FileRenamer
    {
        public event EventHandler<EventArgs> FileNamesUpdated;
        public BindingList<string> FileNames { get; set; } = new BindingList<string>();

        protected string _findString = "";
        public string FindString
        {
            get => _findString;
            set { _findString = value; OnFileNamesUpdated(); }
        }

        protected string _replacString = "";
        public string ReplaceString
        {
            get => _replacString;
            set { _replacString = value; OnFileNamesUpdated(); }
        }

        protected bool _isCaseSensitive = true;
        public bool IsCaseSensitive
        {
            get => _isCaseSensitive;
            set { _isCaseSensitive = value; OnFileNamesUpdated(); }
        }

        protected bool _useRegex = true;
        public bool UseRegex
        {
            get => _useRegex;
            set { _useRegex = value; OnFileNamesUpdated(); }
        }

        protected bool _showExtensions = false;
        public bool ShowExtensions
        {
            get => _showExtensions;
            set { _showExtensions = value; OnFileNamesUpdated(); }
        }

        public FileRenamer()
        {
            FileNames.AllowEdit = false;
        }

        public void AddFile(string path)
        {
            if (!FileNames.Contains(path))
                FileNames.Add(path);
        }

        public void AddFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
                AddFile(file);
        }

        public void RemoveFile(int index) => FileNames.RemoveAt(index);

        public void RenameFiles()
        {
            for (int i = 0; i < FileNames.Count; i++)
            {
                string oldPath = FileNames[i];
                string newPath = GetReplacedPath(oldPath);
                if (File.Exists(oldPath) && !File.Exists(newPath))
                {
                    File.Move(oldPath, newPath);
                    FileNames[i] = newPath;
                }

            }
            OnFileNamesUpdated();
        }

        public bool FileNamesValid()
            => !FileNames.Select(f => GetReplacedName(f)).Any(f => FileUtils.IsInvalidFileName(f));

        public bool FileNameValid(string fileName) => FileUtils.IsInvalidFileName(fileName);

        public string GetFileName(string path) =>
            ShowExtensions ? Path.GetFileName(path) : Path.GetFileNameWithoutExtension(path);

        public string GetReplacedName(string path)
        {
            string findRegex = UseRegex ? FindString : Regex.Escape(FindString);
            string replaceRegex = UseRegex ? ReplaceString : Regex.Escape(ReplaceString);
            RegexOptions options = IsCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
            string fileName = GetFileName(path);
            return Regex.Unescape(Regex.Replace(fileName, findRegex, replaceRegex, options));
        }

        public bool HasDuplicates()
            => FileNames.Select(f => GetReplacedPath(f)).GroupBy(f => f).Any(f => f.Count() > 1);

        public bool IsDuplicate(int index)
        {
            for (int i = 0; i < FileNames.Count; i++)
                if (i != index && GetReplacedPath(FileNames[i]) == GetReplacedPath(FileNames[index]))
                    return true;
            return false;
        }

        private string GetReplacedPath(string path)
        {
            string fileName = Path.GetFileName(path);
            string filePath = Path.GetDirectoryName(path);
            string newFileName = GetReplacedName(fileName);
            if (!ShowExtensions) newFileName += Path.GetExtension(path);
            string newPath = Path.Combine(filePath, newFileName);
            return newPath;
        }

        public void OnFileNamesUpdated()
            => FileNamesUpdated?.Invoke(this, new EventArgs());
    }
}
