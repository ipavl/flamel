//
// The main source file for Flamel.
//
// Author:
//       ipavl <ipavl@users.sourceforge.net>
//

open System
open System.Text

[<EntryPoint>]
let main argv = 
    let src = new StringBuilder()

    // parse args
    match argv with
    | [|dir|] -> src.Append(Environment.CurrentDirectory).Append("/").Append(dir)
    | _ -> src.Append(Environment.CurrentDirectory)
    |> ignore

    printfn "Flamel static site generator v0.3"
    printfn "Using source directory: %s" (src.ToString())

    // Launch a web server to serve the files
    WebServer.listener (fun req resp ->
    async {
        let data = Encoding.ASCII.GetBytes(WebServer.routeHandler (req, src.ToString()))
        resp.OutputStream.Write(data, 0, data.Length)
        resp.OutputStream.Close()
    })
    printfn "Started server at %s" WebServer.httpServer

    // Set up the file watcher to reparse the source files on changes
    FileWatcher.setupFileWatcher(src.ToString())

    Console.ReadLine() |> ignore

    0 // return an integer exit code
