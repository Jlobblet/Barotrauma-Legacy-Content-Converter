module Barotrauma_Legacy_Content_Converter.Transformations

open System.Xml.Linq
open System.Xml.Linq.FSharpExtensions

let ensureItemsAreWrapped (doc: XDocument) =
    match doc.Root.Name.LocalName with
    | "Item" -> XDocument(XElement(XName.Get "Items", doc.Root))
    | _ -> doc

let ensureIdentifiers (doc: XDocument) =
    let nameToIdentifier (name: string) = name.ToLowerInvariant().Replace(" ", "")

    let setIdentifierFromName (elt: XElement) =
        let identifier = elt.TryAttribute "identifier"
        let name = elt.TryAttribute "name"

        match identifier, name with
        | None, Some n -> elt.SetAttributeValue("identifier", n.Value |> nameToIdentifier)
        | _ -> ()

    let setIdentifierAndRemoveName elt =
        setIdentifierFromName elt
        elt.SetAttributeValue(XName.Get "name", null)

    let ensureIdentifier (elt: XElement) =
        // Set the identifier on the second-level element
        elt |> setIdentifierFromName

        // Look for things that might need an identifier
        elt.Descendants()
        |> Seq.filter (fun e -> e.TryAttribute "name" |> Option.isSome)
        |> Seq.iter setIdentifierAndRemoveName
        
        // Special case for the spawn crate
        elt.TryAttribute "cargocontainername"
        |> Option.iter (fun attr ->
            elt.SetAttributeValue("cargocontaineridentifier", nameToIdentifier attr.Value)
            elt.SetAttributeValue("cargocontainername", null))

    doc.Root.Elements() |> Seq.iter ensureIdentifier

    doc

let splitAttackToAfflictions (doc: XDocument) =
    let split (elt: XElement) =
        let mapping =
            [ "bleeding", "bleedingdamage"
              "stun", "stun"
              "damage", "damage" ]
            |> Map.ofList

        mapping
        |> Map.map (fun _ -> elt.TryAttribute)
        |> Map.iter (fun identifier ->
            Option.iter (fun attr ->
                elt.Add
                    (XElement
                        (XName.Get "Affliction",
                         XAttribute(XName.Get "identifier", identifier),
                         XAttribute(XName.Get "strength", attr.Value)))

                attr.Remove()))

    doc.Descendants "Attack" |> Seq.iter split

    doc

let AllTransformations doc =
    doc
    |> ensureItemsAreWrapped
    |> ensureIdentifiers
    |> splitAttackToAfflictions
