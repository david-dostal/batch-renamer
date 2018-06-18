using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace BatchRenamer
{
    public partial class Form1 : Form
    {
        private FileRenamer renamer = new FileRenamer();
        public Form1()
        {
            InitializeComponent();

            newFilenamesDgv.CellFormatting += NewFilenamesDgv_CellFormatting;
            newFilenamesDgv.AutoGenerateColumns = false;
            newFilenamesDgv.DataSource = renamer.FileNames;

            caseSensitiveCbx.DataBindings.Add(nameof(caseSensitiveCbx.Checked), renamer, nameof(renamer.IsCaseSensitive), false, DataSourceUpdateMode.OnPropertyChanged);
            useRegexCbx.DataBindings.Add(nameof(useRegexCbx.Checked), renamer, nameof(renamer.UseRegex), false, DataSourceUpdateMode.OnPropertyChanged);
            fileExtensionsCbx.DataBindings.Add(nameof(fileExtensionsCbx.Checked), renamer, nameof(renamer.ShowExtensions), false, DataSourceUpdateMode.OnPropertyChanged);

            findPatternTbx.DataBindings.Add(nameof(findPatternTbx.Text), renamer, nameof(renamer.FindString), false, DataSourceUpdateMode.OnPropertyChanged);
            replacePatternTbx.DataBindings.Add(nameof(replacePatternTbx.Text), renamer, nameof(renamer.ReplaceString), false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void NewFilenamesDgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.Value = renamer.GetFileName(renamer.FileNames[e.RowIndex]);
                e.FormattingApplied = true;
            }
            else if (e.ColumnIndex == 1)
            {
                e.Value = renamer.GetReplacedName(renamer.FileNames[e.RowIndex]);
                e.FormattingApplied = true;
            }
        }

        private void newFilenamesDgv_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
                renamer.AddFile(file);
        }

        private void newFilenamesDgv_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        private void newFilenamesDgv_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex != -1)
                e.ToolTipText = renamer.FileNames[e.RowIndex];
        }

        private void renameBtn_Click(object sender, EventArgs e) => renamer.RenameFiles();

        private void openFolderBtn_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.ShowHiddenItems = false;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string folderPath = dialog.FileName;
                FileInfo[] files = new DirectoryInfo(folderPath).GetFiles();
                foreach (FileInfo file in files)
                {
                    if (!file.Attributes.HasFlag(FileAttributes.Hidden))
                        renamer.AddFile(file.FullName);
                }
            }
        }
    }
}
