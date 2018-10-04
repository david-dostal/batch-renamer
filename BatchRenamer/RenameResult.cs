using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRenamer
{
    [Flags]
    public enum RenameResult
    {
        Success = 0,
        FileDoesntExist = 1,
        FileNameAlreadyExists = 2,
        CannotRename = 4,
    }
}
