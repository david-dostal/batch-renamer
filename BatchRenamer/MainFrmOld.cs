﻿//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Windows.Forms;

//namespace BatchRenamer
//{
//    public partial class MainFrmOld : Form
//    {
//        private RenamerViewModel renamer = new RenamerViewModel();

//        public MainFrmOld()
//        {
//            InitializeComponent();
//            newFilenamesDgv.RowsAdded += (s, e) => UpdateFilesCount();
//            newFilenamesDgv.RowsRemoved += (s, e) => UpdateFilesCount();
//            newFilenamesDgv.SelectionChanged += (s, e) => UpdateSelectedCount();
//            newFilenamesDgv.CellFormatting += (s, e) => FormatCell(e);
//            newFilenamesDgv.AutoGenerateColumns = false;
//            newFilenamesDgv.DataSource = renamer.FileNames;

//            caseSensitiveCbx.DataBindings.Add(nameof(caseSensitiveCbx.Checked), renamer, nameof(renamer.IsCaseSensitive), false, DataSourceUpdateMode.OnPropertyChanged);
//            useRegexCbx.DataBindings.Add(nameof(useRegexCbx.Checked), renamer, nameof(renamer.UseRegex), false, DataSourceUpdateMode.OnPropertyChanged);
//            fileExtensionsCbx.DataBindings.Add(nameof(fileExtensionsCbx.Checked), renamer, nameof(renamer.ShowExtensions), false, DataSourceUpdateMode.OnPropertyChanged);
//            findPatternTbx.DataBindings.Add(nameof(findPatternTbx.Text), renamer, nameof(renamer.FindString), false, DataSourceUpdateMode.OnPropertyChanged);
//            replacePatternTbx.DataBindings.Add(nameof(replacePatternTbx.Text), renamer, nameof(renamer.ReplaceString), false, DataSourceUpdateMode.OnPropertyChanged);

//            renamer.FileNamesUpdated += (s, e) => UpdateFileNames();
//        }

//        private void FormatCell(DataGridViewCellFormattingEventArgs e)
//        {
//            string fileName = GetFilename(e.ColumnIndex, e.RowIndex);
//            bool isInvalid = renamer.FileNameValid(fileName);
//            bool isDuplicate = renamer.IsDuplicate(e.RowIndex);
//            e.CellStyle.ForeColor = isInvalid ? Color.Firebrick : isDuplicate ? Color.DarkBlue : Color.Black;
//            e.CellStyle.SelectionForeColor = isInvalid ? Color.Firebrick : isDuplicate ? Color.DarkBlue : Color.Black;
//            e.Value = fileName;
//            e.FormattingApplied = true;
//        }

//        private void UpdateFileNames() => newFilenamesDgv.Refresh();
//        private void UpdateFilesCount() => fileCountStLbl.Text = $"Files: {newFilenamesDgv.RowCount}";
//        private void UpdateSelectedCount() => selectedCountTsLbl.Text = $"Selected: {newFilenamesDgv.SelectedRows.Count}";

//        private string GetFilename(int column, int row)
//        {
//            if (column == 0) return renamer.GetFileName(renamer.FileNames[row]);
//            else if (column == 1) return renamer.GetReplacedName(renamer.FileNames[row]);
//            else throw new Exception($"Unknown column no {column}");
//        }

//        private void newFilenamesDgv_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
//        {
//            if (e.RowIndex != -1)
//                e.ToolTipText = renamer.FileNames[e.RowIndex];
//        }

//        private void renameBtn_Click(object sender, EventArgs e)
//        {
//            if (renamer.FileNamesValid())
//            {
//                if (renamer.HasDuplicates())
//                    MessageBox.Show($"The new filenames contain duplicates.{Environment.NewLine}(highlighted in blue)", "Duplicate filenames");
//                else
//                    renamer.RenameFiles();
//            }
//            else
//                MessageBox.Show($"Some filenames contain invalid characters.{Environment.NewLine}(highlighted in red)", "Invalid filenames");
//        }

//        private void newFilenamesDgv_DragEnter(object sender, DragEventArgs e)
//        {
//            if (e.Data.GetDataPresent(DataFormats.FileDrop))
//                e.Effect = DragDropEffects.All;
//            else
//                e.Effect = DragDropEffects.None;
//        }

//        private void newFilenamesDgv_DragDrop(object sender, DragEventArgs e) =>
//            AddFiles((string[])e.Data.GetData(DataFormats.FileDrop));


//        private void openFolderBtn_Click(object sender, EventArgs e)
//        {
//            if (addFilesOfs.ShowDialog(this) == DialogResult.OK)
//                AddFiles(addFilesOfs.FileNames);
//        }

//        private void AddFiles(IEnumerable<string> fileNames)
//        {
//            List<string> files = new List<string>();
//            foreach (string fileName in fileNames)
//            {
//                if (File.Exists(fileName))
//                    files.Add(fileName);
//                else if (Directory.Exists(fileName))
//                    foreach (string file in Directory.EnumerateFiles(fileName))
//                    {
//                        if (File.Exists(file))
//                            files.Add(file);
//                    }
//            }
//            renamer.AddFiles(files);
//        }
//    }
//}
