using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BatchRenamer
{
    public partial class MainFrm : Form
    {
        private RenamerViewModel renamer = new RenamerViewModel();

        private DataGridViewCellStyle normalCellStyle;
        private DataGridViewCellStyle duplicateCellStyle;
        private DataGridViewCellStyle invalidCellStyle;

        private const int originalColumnIndex = 0;
        private const int renamedColumnIndex = 1;

        public MainFrm()
        {
            InitializeComponent();
            newFilenamesDgv.SetDoubleBuffering(true);
            newFilenamesDgv.CellFormatting += (s, e) => FormatCell(e);
            newFilenamesDgv.AutoGenerateColumns = false;
            newFilenamesDgv.DataSource = renamer.FileNames;
            newFilenamesDgv.RowsAdded += (s, e) => UpdateFilesCount();
            newFilenamesDgv.RowsRemoved += (s, e) => UpdateFilesCount();
            newFilenamesDgv.SelectionChanged += (s, e) => UpdateSelectedCount();

            normalCellStyle = newFilenamesDgv.DefaultCellStyle;
            duplicateCellStyle = new DataGridViewCellStyle(newFilenamesDgv.DefaultCellStyle) { ForeColor = Color.DarkBlue, SelectionForeColor = Color.DarkBlue };
            invalidCellStyle = new DataGridViewCellStyle(newFilenamesDgv.DefaultCellStyle) { ForeColor = Color.Firebrick, SelectionForeColor = Color.Firebrick };

            ignoreCaseCbx.DataBindings.Add(nameof(ignoreCaseCbx.Checked), renamer, nameof(renamer.IgnoreCase), false, DataSourceUpdateMode.OnPropertyChanged);
            useRegexCbx.DataBindings.Add(nameof(useRegexCbx.Checked), renamer, nameof(renamer.UseRegex), false, DataSourceUpdateMode.OnPropertyChanged);
            fileExtensionsCbx.DataBindings.Add(nameof(fileExtensionsCbx.Checked), renamer, nameof(renamer.ShowExtensions), false, DataSourceUpdateMode.OnPropertyChanged);

            findPatternTbx.DataBindings.Add(nameof(findPatternTbx.Text), renamer, nameof(renamer.FindString), false, DataSourceUpdateMode.OnPropertyChanged);
            replacePatternTbx.DataBindings.Add(nameof(replacePatternTbx.Text), renamer, nameof(renamer.ReplaceString), false, DataSourceUpdateMode.OnPropertyChanged);

            renamer.OriginalNamesChanged += (s, e) => newFilenamesDgv.InvalidateColumn(originalColumnIndex);
            renamer.RenamedNamesChanged += (s, e) => newFilenamesDgv.InvalidateColumn(renamedColumnIndex);
        }

        private void UpdateFilesCount()
        {
            fileCountStLbl.Text = $"Files: {newFilenamesDgv.RowCount}";
        }

        private void UpdateSelectedCount()
        {
            selectedCountTsLbl.Text = $"Selected: {newFilenamesDgv.Rows.GetRowCount(DataGridViewElementStates.Selected)}";
        }

        private void FormatCell(DataGridViewCellFormattingEventArgs e)
        {
            e.Value =
                e.ColumnIndex == originalColumnIndex ? renamer.DisplayName(e.RowIndex) :
                e.ColumnIndex == renamedColumnIndex ? renamer.RenamedDisplayName(e.RowIndex) :
                throw new ArgumentException($"Index {e.ColumnIndex} is not a valid column. This should never happen.");

            e.FormattingApplied = true;
        }

        private void CellToolTipNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex == -1) return;
            e.ToolTipText = renamer.OriginalPath(e.RowIndex);
        }

        private void RenameClick(object sender, EventArgs e)
        {
            Dictionary<string, ValidationResult> validationResults = renamer.ValidateAll();
            if (!validationResults.Values.All(r => r == ValidationResult.ProbablyValid))
            {
                StringBuilder message = new StringBuilder();
                if (validationResults.Values.Any(r => r.HasFlag(ValidationResult.DuplicateFileName)))
                    message.AppendLine("The new filenames contain duplicates.");
                if (validationResults.Values.Any(r => r.HasFlag(ValidationResult.InvalidFileName)))
                    message.AppendLine("Some filenames contain invalid characters (\\/:*?\"<>|).");
                if (validationResults.Values.Any(r => r.HasFlag(ValidationResult.InvalidRegex)))
                    message.AppendLine("The regex pattern is not valid.");
                message.AppendLine("No files were renamed.");

                MessageBox.Show(message.ToString(), "Cannot rename", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Dictionary<string, RenameResult> renamingResults = new Dictionary<string, RenameResult>();
            try { renamingResults = renamer.RenameAll(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Unexpected error while renaming.", MessageBoxButtons.OK, MessageBoxIcon.Error); return; } // This should not happen

            if (!renamingResults.Values.All(r => r == RenameResult.Success))
            {
                StringBuilder message = new StringBuilder();
                if (renamingResults.Values.Any(r => r.HasFlag(RenameResult.FileDoesntExist)))
                    message.AppendLine("Some files you're trying to rename don't exist anymore.");
                if (renamingResults.Values.Any(r => r.HasFlag(RenameResult.FileNameAlreadyExists)))
                    message.AppendLine("Some files couldn't be renamed, because a file with the same name already exists.");
                if (renamingResults.Values.Any(r => r.HasFlag(RenameResult.CannotRename)))
                    message.AppendLine("Couldn't rename some files.");

                MessageBox.Show(message.ToString(), "Errors while renaming", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ?
                DragDropEffects.All :
                DragDropEffects.None;
        }

        private void OnFileDrop(object sender, DragEventArgs e) =>
            AddFiles((string[])e.Data.GetData(DataFormats.FileDrop));


        private void OpenFolderClick(object sender, EventArgs e)
        {
            if (addFilesOfs.ShowDialog(this) == DialogResult.OK)
                AddFiles(addFilesOfs.FileNames);
        }

        private void AddFiles(IEnumerable<string> fileNames)
        {
            List<string> files = new List<string>();
            foreach (string fileName in fileNames)
            {
                if (File.Exists(fileName))
                    files.Add(fileName);

                else if (Directory.Exists(fileName))
                    Directory.EnumerateFiles(fileName)
                        .Where(f => File.Exists(f)).ToList()
                        .ForEach(f => files.Add(f));

                // else we ignore nonexistent path
            }
            renamer.AddFiles(files);
        }
    }
}
