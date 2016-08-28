open System
open MtgSearcher.Indexing
open MtgSearcher.QueryHandler
open MtgSearcher.ResultHandler

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    
    ParseCardDataFromJsonFile "AllCards-x.json" |> generateIndex
    
    //query "Air Elemental"
    query "Tap target creature with flying"
    |> outputResults printCard printDebug { pageIdx = 0; rows = 10 } 
    |> ignore

    Console.ReadKey() |> ignore
    0 // return an integer exit code