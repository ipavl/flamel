Flamel
======

Flamel is a static-site generator written in [F#](http://fsharp.org). It is named after the scrivener and alchemist, [Nicolas Flamel](https://en.wikipedia.org/wiki/Nicolas_Flamel), as alchemy is the process of transmuting things into another form, much like a static site generator does.

Dependencies
------------

* Visual Studio or MonoDevelop with the [F# binding](https://github.com/fsharp/fsharpbinding)
* Mono/.NET 4.0 or higher

Usage
-----

Once compiled, run the generator from the command line as follows:

    flamel.exe [source-directory]

where `source-directory` is a child of the current working directory. If no `source-directory` is specified, the current working directory will be used.

Generated files are currently outputted to the same directory that the Markdown files they were generated from are located. There is (currently) no web server included with Flamel, so you will need to copy the files to one to see your changes, or view them via your browser in their local directories.

Templates
---------

Flamel does not (currently) use any sort of templating language. The output page structure is built using partial regular HTML files in the `_includes` directory of the source folder you specify. The required files are:

* `header.inc.html`: should include an opening `<html>` tag, and opening `<head>` tag, any other elements that need to be placed in the `<head>` section, and then the last line should be an opening `<title>` tag
* `body.inc.html`: should include a closing `<title>` tag as the first line, any other elements that need to be placed in the `<head>` section that you want after the `<title>`, a closing `<head>` tag, and an opening `<body>` tag
* `nav.inc.html`: should include whatever navigation you want, or you could leave it as an empty file if you want to put your navigation elsewhere (such as in your footer) without editing the order the pages are constructed in the generator
* `footer.inc.html`: any page footer content you want should be placed here

If you want a certain string to appear either before or after your page title, you should specify that with the respective `<title>` tag.

Pages are structured as follows:

1. header.inc.html
2. Page title extracted from Markdown files (will be between `<title>` tags and any text you specify)
3. body.inc.html
4. nav.inc.html
5. HTML page content extracted from Markdown files
6. footer.inc.html

A sample site can be found in the `sample` directory.

License
-------

This project's source code is licensed under the MIT License. See the LICENSE file for a copy of the full license.

It would be appreciated if you link back to this project page in the footer of your site or on a credits page of some sort if you use Flamel to build your website, however it is not required. The sample site's footer says "Alchemized by Flamel" for the link text, but you can choose to use something else if you want.

Credits
-------

This project uses Tomas Petricek's [F# Formatting library](https://github.com/tpetricek/FSharp.Formatting), which is licensed under the Apache License. A copy of the Apache License can be found in the `lib` directory, along with the library DLL.
