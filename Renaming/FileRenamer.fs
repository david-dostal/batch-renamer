namespace Renaming
module FileRenamer =
    open System.Text.RegularExpressions
    open System.IO

    type ReplaceOptions = { Find: string; Replace: string; IgnoreCase: bool }
    type Renamer = ReplaceOptions -> string -> string

    let stringReplace { Find = findStr; Replace = replaceStr; IgnoreCase = ignoreCase } (fileName: string) =
        let findEscaped = Regex.Escape(findStr)
        let replaceEscaped = Regex.Escape(replaceStr)
        let regexOptions = if ignoreCase then RegexOptions.IgnoreCase else RegexOptions.None
        let replaced = Regex.Replace(fileName, findEscaped, replaceEscaped, regexOptions)
        Regex.Unescape(replaced)

    let regexReplace { Find = findRegex; Replace = replaceRegex; IgnoreCase = ignoreCase } (fileName: string) =
        let regexOptions = if ignoreCase then RegexOptions.IgnoreCase else RegexOptions.None
        Regex.Replace(fileName, findRegex, replaceRegex, regexOptions)

    type FileInfo = {Directory: string; Name: string; Extension: string}
    
    let fileInfo (path: string) =
        { Directory = Path.GetDirectoryName(path);
          Name = Path.GetFileNameWithoutExtension(path);
          Extension = Path.GetExtension(path) }

    let renamedFile renamer options renameExtension path =
        let file = fileInfo path
        let renamedFile =
            if renameExtension
            then renamer options (file.Name + file.Extension)
            else (renamer options file.Name) + file.Extension
        Path.Combine(file.Directory, renamedFile)

    let renamedFiles renamer files =
        Seq.map (fun original -> original, renamer original) files

    let displayName showExtension (path: string) =
        if showExtension
        then Path.GetFileName path
        else Path.GetFileNameWithoutExtension path

    let isInvalid (path: string) =
        let invalidFileNameChars = Path.GetInvalidFileNameChars() |> Seq.map string
        let invalidPathChars = Path.GetInvalidPathChars() |> Seq.map string
        let path = Path.GetDirectoryName(path)
        let fileName = Path.GetFileName(path)
        (Seq.exists (fun (invalid: string) -> fileName.Contains(invalid)) invalidFileNameChars)
        || (Seq.exists (fun (invalid: string) -> path.Contains(invalid)) invalidPathChars)

    type ValidationResult =
        | Valid=1
        | IsDuplicate=2
        | InvalidName=4

    let validate names file =
        if Seq.contains file names then ValidationResult.IsDuplicate
        else if isInvalid file then ValidationResult.InvalidName
        else ValidationResult.Valid
