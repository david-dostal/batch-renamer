using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRenamer
{
    public class FileNameInfo
    {
        public string FilePath { get; protected set; }
        public string OldFileName { get; protected set; }
        public string NewFileName { get; protected set; }

        public FileNameInfo(string path, string oldName, string newName)
        {
            FilePath = path;
            OldFileName = oldName;
            NewFileName = newName;
        }
    }
}
