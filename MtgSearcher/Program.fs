open MtgSearcher.Indexer
open QueryHandler

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    ParseCardDataFromJsonFile "AllCards-x.json" |> generateIndex
    let result = query ["Elemental"]
    0 // return an integer exit code