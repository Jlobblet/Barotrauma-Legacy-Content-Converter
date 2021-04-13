[<RequireQualifiedAccess>]
module Barotrauma_Legacy_Content_Converter.Xml

open System.Threading
open System.Xml
open System.Xml.Linq

let loadAsync (filepath: string) =
    let settings = XmlReaderSettings(Async = true)
    let reader = XmlReader.Create(filepath, settings)
    let source = new CancellationTokenSource()
    let token = source.Token

    async {
        let! doc =
            XDocument.LoadAsync(reader, LoadOptions.PreserveWhitespace, token)
            |> Async.AwaitTask

        reader.Close()
        reader.Dispose()
        return doc
    }


let saveAsync (filepath: string) (doc: XDocument) =
    let settings = XmlWriterSettings(Async = true)
    let writer = XmlWriter.Create(filepath, settings)
    let source = new CancellationTokenSource()
    let token = source.Token

    async {
        do! doc.SaveAsync(writer, token) |> Async.AwaitTask
        writer.Close()
        writer.Dispose()
    }
