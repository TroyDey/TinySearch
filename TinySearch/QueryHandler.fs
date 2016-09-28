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
        |> Seq.map getFieldIndex 
        |> Seq.map (getSubIndex token) 
        |> Seq.filter (fun i -> i.IsSome) 
        |> Seq.map (fun i -> i.Value)

    let query (queryAnalyzer:analyzer) (defaultFields:string seq) (q:string)  =
        let tokenizedQuery = queryAnalyzer.tokenizer q
        queryAnalyzer.filters 
        |> Seq.fold (fun tokens filter -> filter tokens) tokenizedQuery
        |> Seq.map (fun t -> { token = t; indexes = getIndexesForToken defaultFields t })
        |> scoreResults