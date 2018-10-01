using System;

namespace BatchRenamer
{
    [Flags]
    public enum ValidationResult
    {
        ProbablyValid = 0,
        DuplicateFileName = 1,
        InvalidFileName = 2,
    }
}