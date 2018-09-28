using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Renaming.FileRenamer;

namespace BatchRenamer
{
    public class RenamerViewModel
    {
        public event EventHandler<EventArgs> DisplayNamesChanged;
        public event EventHandler<EventArgs> RenamedNamesChanged;

        public BindingList<string> FileNames { get; protected set; }

        public bool UseRegex { get; set; }
        public bool ShowExtensions { get; set; }
        public bool IgnoreCase { get; set; }

        public string FindString { get; set; }
        public string ReplaceString { get; set; }

        public int FileCount => FileNames.Count;

        public RenamerViewModel()
        {
            FileNames = new BindingList<string>();
            FileNames.AllowEdit = false;
        }

        public bool CanRename()
        {
            throw new NotImplementedException();
            // TODO:
            // - check if all are valid
            // - check if files don't already exist (if the name changed)
        }

        public void RenameAll()
        {

        }

        public void AddFiles(IEnumerable<string> fileNames)
        {

        }

        public string GetDisplayName(string path)
        {
            throw new NotImplementedException();
        }

        public string GetRenamedName(string path)
        {
            throw new NotImplementedException();
        }

        public ValidationResult GetValidationResult(string path)
        {
            throw new NotImplementedException();
        }

        public void OnDisplayNamesChanged() => DisplayNamesChanged?.Invoke(this, EventArgs.Empty);
        public void OnRenamedNamesChanged() => RenamedNamesChanged?.Invoke(this, EventArgs.Empty);
    }
}
