﻿open System
open MtgSearcher.Indexing
open MtgSearcher.QueryHandler
open MtgSearcher.ResultHandler
open MtgSearcher.Analyzers
open Utilities

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    
    ParseCardDataFromJsonFile "AllCards-x.json" |> generateIndex defaultAnalyzer aggregateCardText
    
    query defaultAnalyzer "TAP    target    creature with flying"
    |> outputResults printCard printDebug { pageIdx = 0; rows = 10 } 
    |> List.map (fun outRes -> (printCard outRes; outRes)) 
    |> List.map (fun outRes -> printDebug outRes)
    |> ignore

    Console.ReadKey() |> ignore
    0 // return an integer exit code