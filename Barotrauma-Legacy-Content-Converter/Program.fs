open System.IO
open Barotrauma_Legacy_Content_Converter
open Fake.IO.Globbing.Operators
open Barotrauma_Legacy_Content_Converter.Arguments
open Barotrauma_Legacy_Content_Converter.Transformations


[<EntryPoint>]
let main argv =
    // Do nothing on no input, Unix-style
    if Array.isEmpty argv then exit 0

    let parsed = ParseArguments argv

    let inputDir = parsed.GetResult <@ InputDirectory @>
    let outputDir = parsed.GetResult <@ OutputDirectory @>

    let inputFiles = !!Path.Combine(inputDir, "**.xml")

    let transform filepath =
        let relativePath = Path.GetRelativePath(inputDir, filepath)
        let outputFile = Path.Combine(outputDir, relativePath)

        outputFile
        |> Path.GetDirectoryName
        |> Directory.CreateDirectory
        |> ignore

        async {
            printfn $"{Path.Combine(inputDir, relativePath)} -> {Path.Combine(outputDir, relativePath)}"

            let! doc = Xml.readAsync filepath
            let updated = AllTransformations doc
            do! Xml.writeAsync outputFile updated
        }

    inputFiles
    |> Seq.map transform
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore

    0 // return an integer exit code
