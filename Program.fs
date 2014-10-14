(*
  Untitled static site generator in F#
  Author: ipavl <https://github.com/ipavl>
  Date: October 13, 2014
 *)

open FSharp.Markdown

open System
open System.IO

/// Functions that are used to parse files (e.g. Markdown, templates).
module Parse =
    /// Parses and converts Markdown files into HTML files.
    let markdown() =
        for mdFile in Directory.EnumerateFiles(Environment.CurrentDirectory, "*.md", SearchOption.AllDirectories) do
            let htmlFile = Path.ChangeExtension(mdFile, "html")
            let html = Markdown.TransformHtml(File.ReadAllText mdFile)

            File.WriteAllText(htmlFile, html)

            printfn "%s -> %s" mdFile htmlFile


[<EntryPoint>]
let main argv = 
    //printfn "%A" argv
    printfn "F# Static Site Generator v0.1"
    printfn "Current working directory: %s" Environment.CurrentDirectory 

    Parse.markdown()

    0 // return an integer exit code
