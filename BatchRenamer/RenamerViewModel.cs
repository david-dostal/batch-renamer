using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BatchRenamer
{
    public class RenamerViewModel
    {
        public event EventHandler<EventArgs> FileNamesChanged;

        public BindingList<string> FileNames { get; protected set; } = new BindingList<string>();
        public IEnumerable<string> RenamedNames => FileNames.Select(RenamedPath);

        private bool useRegex = false;
        public bool UseRegex { get => useRegex; set { useRegex = value; OnFileNamesChanged(); } }

        private bool showExtensions = false;
        public bool ShowExtensions { get => showExtensions; set { showExtensions = value; OnFileNamesChanged(); } }

        private bool ignoreCase = false;
        public bool IgnoreCase { get => ignoreCase; set { ignoreCase = value; OnFileNamesChanged(); } }

        private string findString = "";
        public string FindString { get => findString; set { findString = value; OnFileNamesChanged(); } }

        private string replaceString = "";
        public string ReplaceString { get => replaceString; set { replaceString = value; OnFileNamesChanged(); } }

        public int FileCount => FileNames.Count;

        public RenamerViewModel()
        {
            FileNames.AllowEdit = false;
        }

        public void RenameAll()
        {

        }

        public void AddFiles(IEnumerable<string> fileNames)
        {
            FileNames.RaiseListChangedEvents = false;
            foreach (string path in fileNames)
                FileNames.Add(path);
            FileNames.RaiseListChangedEvents = true;
            FileNames.ResetBindings();
        }

        public string OriginalPath(int index) => FileNames[index];

        public string RenamedPath(int index) => RenamedPath(OriginalPath(index));
        private string RenamedPath(string path)
        {
            FileRenamer.Renamer renamer = UseRegex ? (FileRenamer.Renamer)FileRenamer.RegexReplace : FileRenamer.StringReplace;
            ReplaceOptions options = new ReplaceOptions(FindString, ReplaceString, IgnoreCase);
            return FileRenamer.RenameFile(path, options, renamer, ShowExtensions);
        }

        public string DisplayName(int index)
        {
            string path = FileNames[index];
            return FileRenamer.DisplayName(path, ShowExtensions);
        }

        public string RenamedDisplayName(int index)
        {
            string path = FileNames[index];
            return FileRenamer.DisplayName(RenamedPath(path), ShowExtensions);
        }

        public ValidationResult Validate(int index) => Validate(RenamedPath(FileNames[index]));
        private ValidationResult Validate(string path)
        {
            if (FileRenamer.IsInvalidPath(path)) return ValidationResult.InvalidDirectoryName;
            if (FileRenamer.IsInvalidFileName(path)) return ValidationResult.InvalidFileName;
            if (RenamedNames.Count(p => p == path) > 1) return ValidationResult.DuplicateFileName;

            return ValidationResult.ProbablyValid;
        }

        public ValidationResult ValidateAll()
        {
            return RenamedNames
                .Select(Validate)
                .Aggregate(ValidationResult.ProbablyValid,
                    (result, curent) => result | curent);
        }

        public void OnFileNamesChanged() => FileNamesChanged?.Invoke(this, EventArgs.Empty);
    }
}
