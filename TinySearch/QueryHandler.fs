namespace TinySearch

module QueryHandler =

    open Analyzers
    open Indexing
    open Scoring
    open SearchTypes

    let getSubIndex token (fieldIndex: fieldIndex option) =
        if fieldIndex.IsSome && fieldIndex.Value.ContainsKey(token) then
            Some(fieldIndex.Value.[token])
        else
            None

    let getFieldIndex field =
        if cachedIndex.ContainsKey(field) then
            Some(cachedIndex.[field])
        else
            getIdxSegment field

    let getIndexesForToken defaultFields token =
        defaultFields 
        |> List.map getFieldIndex 
        |> List.map (getSubIndex token) 
        |> List.filter (fun i -> i.IsSome) 
        |> List.map (fun i -> i.Value)

    let query (queryAnalyzer:analyzer) (defaultFields:string list) (q:string)  =
        let tokenizedQuery = queryAnalyzer.tokenizer q
        queryAnalyzer.filters 
        |> List.fold (fun tokens filter -> filter tokens) tokenizedQuery
        |> List.map (fun t -> { token = t; indexes = getIndexesForToken defaultFields t })
        |> scoreResults