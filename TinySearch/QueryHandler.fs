namespace TinySearch

module QueryHandler =

    open Analyzers
    open Indexing
    open Scoring

    let query (queryAnalyzer:analyzer) (q:string)  =
        let tokenizedQuery = queryAnalyzer.tokenizer q
        queryAnalyzer.filters
        |> List.fold (fun a c -> (c a)) tokenizedQuery
        |> Seq.filter (fun t -> cachedIndex.ContainsKey(t))
        |> Seq.fold (fun a t -> cachedIndex.[t]::a) []
        |> Seq.toList
        |> scoreResults