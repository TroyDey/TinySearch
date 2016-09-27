open System
open TinySearch.Indexing
open TinySearch.QueryHandler
open TinySearch.ResultHandler
open TinySearch.Analyzers
open Utilities

[<EntryPoint>]
let main argv = 
    
    let reindex = (if argv.Length > 0 then argv.[0] else String.Empty)

    if reindex <> String.Empty && reindex.ToLowerInvariant() = "reindex" then
        printf "Generating index...\r\n"
        ParseCardDataFromJsonFile "AllCards-x.json" |> generateIndex allFiltersAnalyzer aggregateCardText parseCard
        printf "Index generation complete!\r\n\r\n"
    
    printf "Welcome to TinySearch with Magic cards.\r\n"
    printf "Input Magic card query and press enter.\r\n"
    printf "Press escape to exit.\r\n\r\n"

    let mutable allDone = false

    while not allDone do
        printf ">"

        let firstKeyPress = Console.ReadKey().Key

        if firstKeyPress = ConsoleKey.Escape then
            allDone <- true
        else
            let queryText = (firstKeyPress.ToString()) + (Console.ReadLine())

            printf "\r\n"

            query allFiltersAnalyzer ["Name"; "Colors"; "ManaCost"; "Type"; "Text"] queryText
            |> outputResults { pageIdx = 0; rows = 10 } 
            |> List.map (fun outRes -> (printCard outRes; outRes)) 
            |> List.map (fun outRes -> printDebug outRes)
            |> ignore

    0