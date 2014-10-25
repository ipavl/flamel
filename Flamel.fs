(* Flamel.fs *)

// Author:
//       ipavl <ipavl@users.sourceforge.net>
//
// Copyright (c) 2014 ipavl
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

open FSharp.Markdown

open System
open System.IO
open System.Collections.Generic

/// Functions that fetch files that should be included.
module Include =
    /// Reads the header include file.
    let header(dir : string) =
        File.ReadAllText (dir + "/_includes/header.inc.html")

    /// Reads the body include file.
    let body(dir : string) =
        File.ReadAllText (dir + "/_includes/body.inc.html")

    /// Reads the navigation include file.
    let navigation(dir : string) =
        File.ReadAllText (dir + "/_includes/nav.inc.html")

    /// Reads the footer include file.
    let footer(dir : string) =
        File.ReadAllText (dir + "/_includes/footer.inc.html")

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
    let markdown(dir : string) =
        let timer = Diagnostics.Stopwatch.StartNew()

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
                Include.header(dir)
                + metadata.Item("title")
                + Include.body(dir)
                + Include.navigation(dir)
                + html
                + Include.footer(dir)

            File.WriteAllText(htmlFile, page)

            printfn "%s -> %s" mdFile htmlFile

        timer.Stop()
        printfn "Done in %f ms" timer.Elapsed.TotalMilliseconds

[<EntryPoint>]
let main argv = 
    let src = new Text.StringBuilder()

    // parse args
    match argv with
    | [|dir|] -> src.Append(Environment.CurrentDirectory).Append("/").Append(dir)
    | _ -> src.Append(Environment.CurrentDirectory)
    |> ignore

    printfn "Flamel static site generator v0.3"
    printfn "Using source directory: %s" (src.ToString())

    Parse.markdown(src.ToString())

    0 // return an integer exit code
