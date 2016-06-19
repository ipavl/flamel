//
// The main source file for Flamel. Handles running the application and setting up services.
//
// Author:
//       ipavl <ipavl@users.sourceforge.net>
//

open System
open System.Text

[<EntryPoint>]
let main argv = 
    // The path of the source directory
    let src = new StringBuilder()

    // Parse args to get the source directory if applicable, otherwise use the current directory
    match argv with
    | [|dir|] -> src.Append(Environment.CurrentDirectory).Append("/").Append(dir)
    | _ -> src.Append(Environment.CurrentDirectory)
    |> ignore

    printfn "Flamel static site generator v0.4.2"
    printfn "Using source directory: %s" (src.ToString())

    // Do an initial parse
    Parser.Parse.markdown(src.ToString())

    // Launch a web server to serve the files
    WebServer.listener (fun req resp ->
    async {
        let data = Encoding.ASCII.GetBytes(WebServer.routeHandler (req, src.ToString()))
        resp.OutputStream.Write(data, 0, data.Length)
        resp.OutputStream.Close()
    })
    printfn "Started server at %s" WebServer.listenAddress

    // Set up the file watcher to reparse the source files on changes
    FileWatcher.setupFileWatcher(src.ToString())

    // Keeps the application running until the user presses a key to exit
    Console.ReadLine() |> ignore
    printfn "Received a keypress. Exiting..."

    0 // return an integer exit code
