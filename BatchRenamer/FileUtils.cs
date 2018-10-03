using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BatchRenamer
{
    public static class FileUtils
    {
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool MoveFile(string lpExistingFileName, string lpNewFileName);

        public static void MoveCaseSensitive(string from, string to, out bool success)
        {
            success = MoveFile(from, to);
        }

        public static bool FileExistsCaseSensitive(string path)
        {
            string directory = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);

            bool exists = Directory.GetFiles(directory, fileName)
                .Any(s => s == Path.GetFullPath(path));
            return exists;
        }
    }
}