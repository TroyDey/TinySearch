namespace TinySearch

module QueryHandler =

    open Analyzers
    open Indexing
    open Scoring
    open SearchTypes

    let getSubIndex subIdx token =
        if cachedIndex.ContainsKey(token) then
            Some(cachedIndex.[token])::subIdx
        else
            (getIdxSegment token)::subIdx
            

    let query (queryAnalyzer:analyzer) (q:string)  =
        let tokenizedQuery = queryAnalyzer.tokenizer q
        queryAnalyzer.filters
        |> List.fold (fun a c -> (c a)) tokenizedQuery
        |> Seq.fold getSubIndex []
        |> Seq.filter (fun si -> si.IsSome)
        |> Seq.map (fun si -> si.Value)
        |> Seq.toList
        |> scoreResults