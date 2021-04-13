[<RequireQualifiedAccess>]
module Barotrauma_Legacy_Content_Converter.Xml

open System.Threading
open System.Xml
open System.Xml.Linq

let readAsync (filepath: string) =
    let settings = XmlReaderSettings(Async = true)
    let reader = XmlReader.Create(filepath, settings)
    let source = new CancellationTokenSource()
    let token = source.Token

    XDocument.LoadAsync(reader, LoadOptions.None, token)
    |> Async.AwaitTask

let writeAsync (filepath: string) (doc: XDocument) =
    let settings = XmlWriterSettings(Async = true)
    let writer = XmlWriter.Create(filepath, settings)
    let source = new CancellationTokenSource()
    let token = source.Token

    doc.WriteToAsync(writer, token) |> Async.AwaitTask
