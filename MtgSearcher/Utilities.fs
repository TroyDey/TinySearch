module Utilities

open System
open System.IO
open System.Text
open System.Collections.Generic
open Newtonsoft.Json
open MtgCard
open TinySearch.Scoring
open TinySearch.ResultHandler

let aggregateCardText (card:Card) =
        let sb = new StringBuilder(1000) //use string builder since we could be jamming alot of text together
        sb.Append(card.Name).Append(" ").Append(String.Join(" ", if card.Colors = null then new List<string>() else card.Colors)).Append(" ").Append(card.ManaCost).Append(" ").Append(card.Type).Append(" ").Append(card.Text).ToString()

let ParseCardDataFromJsonFile (fileName:string) =
    use sr = new StreamReader(fileName)
    let jsonStr = sr.ReadToEnd()
    Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,Card>>(jsonStr)

//create a card utility module as we decouple this from being specific to cards
let printCard (result:outputResult) =
    let card = JsonConvert.DeserializeObject<Card>(result.doc)
    printfn "%s     %s\r\n%s\r\n%s\r\n%s%s%s\r\n" card.Name card.ManaCost card.Type card.Text card.Power (if System.String.IsNullOrWhiteSpace(card.Power) && String.IsNullOrWhiteSpace(card.Toughness) then "NA/NA" else "/") card.Toughness

//create debug module specifically for this that this module would leverage
let printDebug (result:outputResult) =
    let card = JsonConvert.DeserializeObject<Card>(result.doc)
    printfn "%s - Total Score: %e\r\n{" card.Name result.score
    printfn "\tTerm Hit Count: %d" result.termHitCount
    printfn "\tTotal Terms: %d" result.maxTerms
    printfn "\tCoordination Factor: %e\r\n" result.coordScore
    List.map (fun (sd:scoreDebug) -> printfn "\tterm: %s\r\n\ttf: %e\r\n\tidf: %e\r\n\ttfidf: %e\r\n\tbasetf: %d\r\n\ttotalDocs: %d\r\n\tdocFreq: %d\r\n" sd.term sd.tf sd.idf sd.tfIdf sd.baseTf sd.totalDocs sd.docFreq) result.debug |> ignore
    printfn "}\r\n"