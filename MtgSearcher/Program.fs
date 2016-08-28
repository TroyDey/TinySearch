open System
open System.Linq
open MtgSearcher.Indexer
open QueryHandler
open MtgCard
open ResultsHandler

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    ParseCardDataFromJsonFile "AllCards-x.json" |> generateIndex
    let fullResult = query "Air Elemental"
    fullResult |> outputResults printCard printDebug { pageIdx = 0; rows = 10 } |> ignore
    Console.ReadKey() |> ignore    
    0 // return an integer exit code