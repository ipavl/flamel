//
// Functions related to watching files for changes.
//
// Author:
//       ipavl <ipavl@users.sourceforge.net>
//

module FileWatcher
    open System
    open System.IO

    let setupFileWatcher (path : String) =
        let fileSystemWatcher = new FileSystemWatcher()

        fileSystemWatcher.Path <- path
        fileSystemWatcher.EnableRaisingEvents <- true
        fileSystemWatcher.IncludeSubdirectories <- true

        fileSystemWatcher.Changed.Add(fun _ -> Parser.Parse.markdown(path))
        fileSystemWatcher.Created.Add(fun _ -> Parser.Parse.markdown(path))
        fileSystemWatcher.Deleted.Add(fun _ -> Parser.Parse.markdown(path))
        fileSystemWatcher.Renamed.Add(fun _ -> Parser.Parse.markdown(path))