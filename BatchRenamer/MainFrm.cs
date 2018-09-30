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
            newFilenamesDgv.CellFormatting += (s, e) => FormatCell(e);
            newFilenamesDgv.AutoGenerateColumns = false;
            newFilenamesDgv.DataSource = renamer.FileNames;

            normalCellStyle = newFilenamesDgv.DefaultCellStyle;
            duplicateCellStyle = new DataGridViewCellStyle(newFilenamesDgv.DefaultCellStyle) { ForeColor = Color.DarkBlue, SelectionForeColor = Color.DarkBlue };
            invalidCellStyle = new DataGridViewCellStyle(newFilenamesDgv.DefaultCellStyle) { ForeColor = Color.Firebrick, SelectionForeColor = Color.Firebrick };

            ignoreCaseCbx.DataBindings.Add(nameof(ignoreCaseCbx.Checked), renamer, nameof(renamer.IgnoreCase), false, DataSourceUpdateMode.OnPropertyChanged);
            useRegexCbx.DataBindings.Add(nameof(useRegexCbx.Checked), renamer, nameof(renamer.UseRegex), false, DataSourceUpdateMode.OnPropertyChanged);
            fileExtensionsCbx.DataBindings.Add(nameof(fileExtensionsCbx.Checked), renamer, nameof(renamer.ShowExtensions), false, DataSourceUpdateMode.OnPropertyChanged);

            findPatternTbx.DataBindings.Add(nameof(findPatternTbx.Text), renamer, nameof(renamer.FindString), false, DataSourceUpdateMode.OnPropertyChanged);
            replacePatternTbx.DataBindings.Add(nameof(replacePatternTbx.Text), renamer, nameof(renamer.ReplaceString), false, DataSourceUpdateMode.OnPropertyChanged);

            renamer.FileNamesChanged += (s, e) => newFilenamesDgv.Refresh();
        }

        private void FormatCell(DataGridViewCellFormattingEventArgs e)
        {
            e.Value =
                e.ColumnIndex == originalColumnIndex ? renamer.DisplayName(e.RowIndex) :
                e.ColumnIndex == renamedColumnIndex ? renamer.RenamedDisplayName(e.RowIndex) :
                throw new ArgumentException($"Index {e.ColumnIndex} is not a valid column. This should never happen.");

            ValidationResult valid = renamer.Validate(e.RowIndex);
            if (valid.HasFlag(ValidationResult.InvalidFileName) || valid.HasFlag(ValidationResult.InvalidDirectoryName))
                e.CellStyle = invalidCellStyle;
            else if (valid.HasFlag(ValidationResult.DuplicateFileName))
                e.CellStyle = duplicateCellStyle;
            else
                e.CellStyle = normalCellStyle;

            e.FormattingApplied = true;
        }

        private void CellToolTipNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex == -1) return;
            e.ToolTipText = renamer.OriginalPath(e.RowIndex);
        }

        private void RenameClick(object sender, EventArgs e)
        {
            ValidationResult valid = renamer.ValidateAll();
            if (valid != ValidationResult.ProbablyValid)
            {
                StringBuilder builder = new StringBuilder();
                if (valid.HasFlag(ValidationResult.DuplicateFileName))
                    builder.AppendLine($"The new filenames contain duplicates (highlighted in blue).");
                if (valid.HasFlag(ValidationResult.InvalidFileName))
                    builder.AppendLine($"Some filenames contain invalid characters (highlighted in red).");
                if (valid.HasFlag(ValidationResult.InvalidDirectoryName))
                    builder.AppendLine($"Some directory names contain invalid characters (highlighted in red).");

                MessageBox.Show(builder.ToString(), "Cannot rename");
            }
            else if (renamer.RenamedNames.Any(File.Exists))
                MessageBox.Show("Cannot rename, because files with same name already exist.", "Cannot rename");
            else
            {
                try
                {
                    renamer.RenameAll();
                }
                catch(Exception ex) // TODO: don't catch generic exception
                {
                    MessageBox.Show(ex.Message, "Error while renaming");
                }
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
