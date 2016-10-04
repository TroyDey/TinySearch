namespace TinySearch

module QueryHandler =

    open Analyzers
    open Indexing
    open Scoring
    open SearchTypes

    let getSubIndex token (fieldIndex: fieldIndex option) =
        match fieldIndex with
        | Some(idx) when idx.ContainsKey(token) -> Some(idx.[token])
        | _ -> None

    let getIndexesForToken (defaultFields:string seq) (token:string) =
        defaultFields
        |> Seq.map getIdxSegment
        |> Seq.choose (getSubIndex token)

    let query (queryAnalyzer:analyzer) (defaultFields:string seq) (q:string)  =
        (queryAnalyzer.tokenizer q, queryAnalyzer.filters)
        ||> Seq.fold (fun tokens filter -> filter tokens)
        |> Seq.map (fun t -> { token = t; indexes = getIndexesForToken defaultFields t })
        |> scoreResults