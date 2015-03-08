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
        let watcher = new FileSystemWatcher()

        watcher.Path <- path
        watcher.Filter <- "*.md"
        watcher.EnableRaisingEvents <- true
        watcher.IncludeSubdirectories <- true

        watcher.Changed.Add(fun _ -> Parser.Parse.markdown(path))
        watcher.Created.Add(fun _ -> Parser.Parse.markdown(path))
        watcher.Deleted.Add(fun _ -> Parser.Parse.markdown(path))
        watcher.Renamed.Add(fun _ -> Parser.Parse.markdown(path))