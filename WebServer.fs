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

    /// The server address and port to listen on.
    let listenAddress = "http://localhost:8141/"

    /// The file that the server should return if the server root is specified.
    let indexFile = "index.html"

    /// Creates a HTTP server asynchronously to serve pages.
    let listener (handler:(HttpListenerRequest -> HttpListenerResponse -> Async<unit>)) =
        let httpListener = new HttpListener()
        httpListener.Prefixes.Add listenAddress
        httpListener.Start()

        let task = Async.FromBeginEnd(httpListener.BeginGetContext, httpListener.EndGetContext)
        async {
            while true do
                let! context = task
                Async.Start(handler context.Request context.Response)
        } |> Async.Start

    /// Handles page routing requests.
    let routeHandler (req : HttpListenerRequest, webRoot : String) =
        let homePage = webRoot + "/" + indexFile
        let file = Path.Combine(webRoot, Uri(listenAddress).MakeRelativeUri(req.Url).OriginalString)

        // TODO: Cleanup
        if file.Equals(webRoot) && File.Exists(homePage) then
            // If the file path is the web root, return the default index file
            dict["code", "200"; "body", File.ReadAllText(homePage)]
        else if File.Exists file then
            // Return the requested file
            dict["code", "200"; "body", File.ReadAllText(file)]
        else if Directory.Exists(file) && file.EndsWith("/") && File.Exists(file + indexFile) then
            // Directory specified with trailing slash and no specific file, return directory index file
            dict["code", "200"; "body", File.ReadAllText(file + indexFile)]
        else if Directory.Exists(file) && not (file.EndsWith("/")) && File.Exists(file + "/" + indexFile) then
            // Directory specified with no trailing slash and no specific file, return directory index file
            dict["code", "200"; "body", File.ReadAllText(file + "/" + indexFile)]
        else
            dict["code", "404"; "body", "File not found"]
