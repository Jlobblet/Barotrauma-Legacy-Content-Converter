module Barotrauma_Legacy_Content_Converter.Arguments

open System.IO
open System.Reflection
open Argu

/// IExiter that does not crash on exit to provide a nicer exit message
type Exiter() =
    interface IExiter with
        member this.Exit(msg, errorCode) =
            match errorCode with
            | ErrorCode.HelpText ->
                printfn $"%s{msg}"
                exit 0
            | _ ->
                eprintfn $"%s{msg}"
                exit 1

        member this.Name = "Exiter"


type Arguments =
    | [<ExactlyOnce; AltCommandLine("-i")>] InputDirectory of path: string
    | [<ExactlyOnce; AltCommandLine("-o")>] OutputDirectory of path: string

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | InputDirectory _ -> "specify the directory to get files from"
            | OutputDirectory _ -> "specify the directory to place files (will be created if does not exist)"

let ParseArguments argv =
    let name =
        Path.GetFileName(Assembly.GetEntryAssembly().Location)

    let parser =
        ArgumentParser.Create<Arguments>(programName = name, errorHandler = Exiter())

    parser.Parse argv
