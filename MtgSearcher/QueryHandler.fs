namespace MtgSearcher
module QueryHandler =

    open Indexing
    open Scoring

    let query (q:string) =
        q.Split(' ')
        |> Seq.filter (fun t -> cachedIndex.ContainsKey(t))
        |> Seq.fold (fun a t -> cachedIndex.[t]::a) []
        |> Seq.toList
        |> scoreResults