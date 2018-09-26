using System.IO;
using System.Linq;

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
