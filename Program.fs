(*
  Untitled static site generator in F#
  Author: ipavl <https://github.com/ipavl>
  Date: October 13, 2014
 *)

open FSharp.Markdown

open System
open System.IO
open System.Collections.Generic

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
    /// Extracts metadata such as the date and title from the passed Markdown.
    let extract(markdown : string) =
        let dict = new Dictionary<string, string>();
            
        // Active pattern to match metadata prefixes
        let (|Prefix|_|) (p:string) (s:string) =
            if s.StartsWith(p) then
                Some(s.Substring(p.Length))
            else
                None

        // TODO: Make this only loop for the metadata section as this parses metadata in actual content
        for line in markdown.Split([|Environment.NewLine|], StringSplitOptions.RemoveEmptyEntries) do
            match line with
            | Prefix "title: " rest -> dict.Add("title", rest)
            | Prefix "date: " rest -> dict.Add("date", rest)
            | _ -> ()

        dict

/// Functions that are used to parse files (e.g. Markdown, templates).
module Parse =
    /// Parses and converts Markdown files into HTML files.
    let markdown() =
        for mdFile in Directory.EnumerateFiles(Environment.CurrentDirectory, "*.md", SearchOption.AllDirectories) do
            let htmlFile = Path.ChangeExtension(mdFile, "html")
            let mdArray = File.ReadAllLines mdFile

            // convert the array to a multiline string
            let lines =
                let re = Text.RegularExpressions.Regex(@"#(\d+)")
                [|for line in mdArray ->
                    re.Replace(line.Replace("{", "{{").Replace("}", "}}").Trim(), "$1", 1)|]
            let mdString = String.Join("\n", lines)

            let metadata = Metadata.extract(mdString)   // extract the metadata into a dictionary

            // rebuild the Markdown string without the metadata block to parse for the page content
            let markdown =
                let sb = new Text.StringBuilder()

                // metadata.Count is the number of items we read, and there are two separator lines
                for i in metadata.Count + 2 .. mdArray.Length - 1 do
                    sb.Append(Array.get mdArray i).Append("\n") |> ignore
                sb.ToString()

            let html = Markdown.TransformHtml(markdown)
            let page : string =
                Include.header()
                + "<title>" + metadata.Item("title") + "</title>"
                + Include.body()
                + Include.navigation()
                + html
                + Include.footer()

            File.WriteAllText(htmlFile, page)

            printfn "%s -> %s" mdFile htmlFile

[<EntryPoint>]
let main argv = 
    printfn "F# Static Site Generator v0.2"
    printfn "Current working directory: %s" Environment.CurrentDirectory 

    Parse.markdown()

    0 // return an integer exit code
