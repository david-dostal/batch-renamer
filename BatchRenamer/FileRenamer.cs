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
        public event EventHandler<EventArgs> FilterUpdated;
        public BindingList<string> FileNames { get; set; } = new BindingList<string>();

        protected string _findString = "";
        public string FindString
        {
            get => _findString;
            set { _findString = value; OnFilterUpdated(); }
        }

        protected string _replacString = "";
        public string ReplaceString
        {
            get => _replacString;
            set { _replacString = value; OnFilterUpdated(); }
        }

        protected bool _isCaseSensitive = true;
        public bool IsCaseSensitive
        {
            get => _isCaseSensitive;
            set { _isCaseSensitive = value; OnFilterUpdated(); }
        }

        protected bool _useRegex = true;
        public bool UseRegex
        {
            get => _useRegex;
            set { _useRegex = value; OnFilterUpdated(); }
        }

        protected bool _showExtensions = false;
        public bool ShowExtensions
        {
            get => _showExtensions;
            set { _showExtensions = value; OnFilterUpdated(); }
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
            return Regex.Unescape(Regex.Replace(fileName, findRegex, replaceRegex, options));
        }

        public void OnFilterUpdated()
        {
            FilterUpdated?.Invoke(this, new EventArgs());
        }
    }
}
