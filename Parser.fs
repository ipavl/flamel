//
// Functions related to parsing files.
//
// Author:
//       ipavl <ipavl@users.sourceforge.net>
//

module Parser
    open System
    open System.IO
    open System.Collections.Generic
    open FSharp.Markdown

    /// Functions that fetch files that should be included.
    module Include =
        /// Reads the header include file.
        let header(dir : string) =
            let headerFile = dir + "/_includes/header.inc.html"

            if File.Exists(headerFile) then File.ReadAllText (headerFile)
            else ""

        /// Reads the body include file.
        let body(dir : string) =
            let bodyFile = dir + "/_includes/body.inc.html"

            if File.Exists(bodyFile) then File.ReadAllText (bodyFile)
            else ""

        /// Reads the navigation include file.
        let navigation(dir : string) =
            let navigationFile = dir + "/_includes/nav.inc.html"

            if File.Exists(navigationFile) then File.ReadAllText (navigationFile)
            else ""

        /// Reads the footer include file.
        let footer(dir : string) =
            let footerFile = dir + "/_includes/footer.inc.html"

            if File.Exists(footerFile) then File.ReadAllText (footerFile)
            else ""

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

            for mdFile in Directory.EnumerateFiles(dir, "*.md", SearchOption.AllDirectories) do
                let htmlFile = Path.ChangeExtension(mdFile, "html")
                let mdArray = File.ReadAllLines mdFile

                // Convert the array to a multiline string
                let lines =
                    let re = Text.RegularExpressions.Regex(@"#(\d+)")
                    [|for line in mdArray ->
                        re.Replace(line.Replace("{", "{{").Replace("}", "}}").Trim(), "$1", 1)|]
                let mdString = String.Join(Environment.NewLine, lines)

                // Extract the metadata into a dictionary
                let metadata = Metadata.extract(mdString)

                // Rebuild the Markdown string without the metadata block to parse for the page content
                let markdown =
                    let sb = new Text.StringBuilder()

                    // metadata.Count is the number of items we read, and there are two separator lines
                    for i in metadata.Count + 2 .. mdArray.Length - 1 do
                        sb.Append(Array.get mdArray i).Append(Environment.NewLine) |> ignore
                    sb.ToString()

                let pageTitle =
                    if metadata.ContainsKey("title") then metadata.Item("title")
                    else ""

                // Construct the page
                let page : string =
                    Include.header(dir)
                    + pageTitle
                    + Include.body(dir)
                    + Include.navigation(dir)
                    + Markdown.TransformHtml(markdown)
                    + Include.footer(dir)

                // Save the page to file
                File.WriteAllText(htmlFile, page)

                printfn "%s -> %s" mdFile htmlFile

            timer.Stop()
            printfn "Done in %f ms" timer.Elapsed.TotalMilliseconds
