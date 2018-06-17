using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatchRenamer
{
    public partial class Form1 : Form
    {
        private FileRenamer renamer = new FileRenamer();
        public Form1()
        {
            InitializeComponent();

            BindingSource source = new BindingSource();
            source.DataSource = renamer.DisplayData;

            newFilenamesDgv.AutoGenerateColumns = false;
            newFilenamesDgv.Columns["oldName"].DataPropertyName = "OldFileName";
            newFilenamesDgv.Columns["newName"].DataPropertyName = "NewFileName";
            newFilenamesDgv.DataSource = source;

            renamer.AddFile(@"C:\test1.txt");
        }

        private void newFilenamesDgv_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {

        }

        private void newFilenamesDgv_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void newFilenamesDgv_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }
    }
}
