(*
  Untitled static site generator in F#
  Author: ipavl <https://github.com/ipavl>
  Date: October 13, 2014
 *)

open FSharp.Markdown

open System
open System.IO

/// Functions that fetch files that should be included.
module Include =
    /// Reads the header include file.
    let header() =
        File.ReadAllText "_includes/header.inc.html"

    /// Reads the body include file.
    let body() =
        File.ReadAllText "_includes/body.inc.html"

    /// Reads the navigation include file.
    let navigation() =
        File.ReadAllText "_includes/nav.inc.html"

    /// Reads the footer include file.
    let footer() =
        File.ReadAllText "_includes/footer.inc.html"

/// Functions that parse specific metadata elements, such as the page title and date.
module Metadata =
    /// Parse the page title.
    let title() =
        "    <title>" + "TODO" + "</title>"

/// Functions that are used to parse files (e.g. Markdown, templates).
module Parse =
    /// Parses and converts Markdown files into HTML files.
    let markdown() =
        for mdFile in Directory.EnumerateFiles(Environment.CurrentDirectory, "*.md", SearchOption.AllDirectories) do
            let htmlFile = Path.ChangeExtension(mdFile, "html")
            let html = Markdown.TransformHtml(File.ReadAllText mdFile)

            let page : string =
                Include.header() + Metadata.title() + Include.body() + Include.navigation() + html + Include.footer()

            File.WriteAllText(htmlFile, page)

            printfn "%s -> %s" mdFile htmlFile

[<EntryPoint>]
let main argv = 
    //printfn "%A" argv
    printfn "F# Static Site Generator v0.1"
    printfn "Current working directory: %s" Environment.CurrentDirectory 

    Parse.markdown()

    0 // return an integer exit code
