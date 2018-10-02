using System;
using System.Text.RegularExpressions;

namespace BatchRenamer
{
    public class RegexUtils
    {
        private string lastFailedPattern = "";
        /// <summary>
        /// Warning: mild hack (not really that bad, but use only if other options are not viable).
        /// 
        /// Performance hack to prevent excessive exceptions being thrown for same invalid pattern.
        /// Caches last used invalid pattern and skips check if pattern is the same (which means the pattern would be invalid anyway).
        /// </summary>
        public string CachedTryReplace(string input, string pattern, string replacement, RegexOptions options, out bool success)
        {
            // Caching the last failed pattern reduces the number of exceptions to 1 exception for repeated application of the same pattern.
            // This is not a general solution, but more a small hack which is to be used in a specific situation.
            // Unfortunately there is no way to date to validate a regexp without throwing an exception.
            // Last edited on: 02. 10. 2018
            if (pattern == lastFailedPattern)
            {
                success = false;
                return input;
            }
            string result = TryReplace(input, pattern, replacement, options, out success);
            if (!success) lastFailedPattern = pattern;
            return result;
        }

        /// <summary>
        /// Try to perform a regex replace. Returns original input and false as out parameter if regex is invalid.
        /// </summary>
        public string TryReplace(string input, string pattern, string replacement, RegexOptions options, out bool success)
        {
            try
            {
                string result = Regex.Replace(input, pattern, replacement, options);
                success = true;
                return result;
            }
            catch (ArgumentException)
            {
                success = false;
                return input;
            }
        }
    }
}
