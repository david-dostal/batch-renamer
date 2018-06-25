using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRenamer
{
    public static class FileUtils
    {
        public static bool IsInvalidFileName(string s)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            return invalidChars.Any(c => s.Contains(c));
        }
    }
}
