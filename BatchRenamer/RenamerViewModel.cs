using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BatchRenamer
{
    public class RenamerViewModel
    {
        public event EventHandler<EventArgs> RenamedNamesChanged;
        public event EventHandler<EventArgs> OriginalNamesChanged;

        public BindingList<string> FileNames { get; protected set; } = new BindingList<string>();

        private bool useRegex = false;
        public bool UseRegex { get => useRegex; set { useRegex = value; OnRenamedNamesChanged(); } }

        private bool showExtensions = false;
        public bool ShowExtensions { get => showExtensions; set { showExtensions = value; OnOriginalNamesChanged(); OnRenamedNamesChanged(); } }

        private bool ignoreCase = false;
        public bool IgnoreCase { get => ignoreCase; set { ignoreCase = value; OnRenamedNamesChanged(); } }

        private string findString = "";
        public string FindString { get => findString; set { findString = value; OnRenamedNamesChanged(); } }

        private string replaceString = "";
        public string ReplaceString { get => replaceString; set { replaceString = value; OnRenamedNamesChanged(); } }

        private FileRenamer.Renamer Renamer => UseRegex ? (FileRenamer.Renamer)FileRenamer.RegexReplace : FileRenamer.StringReplace;
        private ReplaceOptions Options => new ReplaceOptions(FindString, ReplaceString, IgnoreCase);

        public RenamerViewModel()
        {
            FileNames.AllowEdit = false;
        }

        public void AddFiles(IEnumerable<string> fileNames)
        {
            FileNames.RaiseListChangedEvents = false;
            foreach (string path in fileNames)
                if (!FileNames.Contains(path))
                    FileNames.Add(path);
            FileNames.RaiseListChangedEvents = true;
            FileNames.ResetBindings();
        }

        public string OriginalPath(int index) => FileNames[index];

        public string RenamedPath(int index) => RenamedPath(OriginalPath(index));
        private string RenamedPath(string path)
        {
            return FileRenamer.RenamePath(path, Options, Renamer, ShowExtensions);
        }

        public string DisplayName(int index)
        {
            string path = FileNames[index];
            return FileRenamer.DisplayName(path, ShowExtensions);
        }

        public string RenamedDisplayName(int index) => RenamedDisplayName(FileNames[index]);
        public string RenamedDisplayName(string path)
        {
            return FileRenamer.RenameFile(FileRenamer.DisplayName(path, ShowExtensions), Options, Renamer, ShowExtensions);
        }

        public Dictionary<string, ValidationResult> ValidateAll()
        {
            Dictionary<string, ValidationResult> results = new Dictionary<string, ValidationResult>();
            IEnumerable<string> renamedPaths = FileNames.Select(RenamedPath);

            foreach(string path in FileNames)
            {
                string renamedPath = RenamedPath(path);
                ValidationResult result = ValidationResult.ProbablyValid;
                if (FileRenamer.IsInvalidFileName(RenamedDisplayName(path))) result |= ValidationResult.InvalidFileName;
                if (renamedPaths.Count(f => f == renamedPath) > 1) result |= ValidationResult.DuplicateFileName;
                if (UseRegex && !FileRenamer.RegexValid(Options)) result |= ValidationResult.InvalidRegex;

                results.Add(path, result);
            }

            return results;
        }

        public Dictionary<string, RenameResult> RenameAll()
        {
            Dictionary<string, RenameResult> results = new Dictionary<string, RenameResult>();

            FileNames.RaiseListChangedEvents = false;
            for (int i = 0; i < FileNames.Count; i++)
            {
                RenameResult result = RenameResult.Success;
                string oldPath = FileNames[i];
                string newPath = RenamedPath(oldPath);

                if (!FileUtils.FileExistsCaseSensitive(oldPath)) result |= RenameResult.FileDoesntExist;
                if ((oldPath != newPath) && FileUtils.FileExistsCaseSensitive(newPath)) result |= RenameResult.FileNameAlreadyExists;

                if (result == RenameResult.Success && oldPath != newPath)
                {
                    FileUtils.MoveCaseSensitive(oldPath, newPath, out bool success);
                    if (success) FileNames[i] = newPath;
                    else result |= RenameResult.CannotRename;
                }

                results.Add(oldPath, result);
            }
            FileNames.RaiseListChangedEvents = true;
            FileNames.ResetBindings();
            return results;
        }

        public void OnRenamedNamesChanged() => RenamedNamesChanged?.Invoke(this, EventArgs.Empty);
        public void OnOriginalNamesChanged() => OriginalNamesChanged?.Invoke(this, EventArgs.Empty);
    }
}
