//
// Functions related to the preview server.
//
// Author:
//       ipavl <ipavl@users.sourceforge.net>
//

module WebServer
    open System
    open System.IO
    open System.Net
    open System.Text

    let httpServer = "http://localhost:8141/"

    let listener (handler:(HttpListenerRequest -> HttpListenerResponse -> Async<unit>)) =
        let httpListener = new HttpListener()
        httpListener.Prefixes.Add httpServer
        httpListener.Start()

        let task = Async.FromBeginEnd(httpListener.BeginGetContext, httpListener.EndGetContext)
        async {
            while true do
                let! context = task
                Async.Start(handler context.Request context.Response)
        } |> Async.Start

    let routeHandler (req : HttpListenerRequest, webRoot : String) =
        let indexFile = webRoot + "/index.html"
        let file = Path.Combine(webRoot, Uri(httpServer).MakeRelativeUri(req.Url).OriginalString)
        printfn "Requested: '%s'" file

        if (file.Equals(webRoot) && File.Exists(indexFile)) then
            // If the file path is the web root, return the default index file
            File.ReadAllText(indexFile)
        else if (File.Exists file) then
            // Return the requested file
            File.ReadAllText(file)
        else
            "File not found"
