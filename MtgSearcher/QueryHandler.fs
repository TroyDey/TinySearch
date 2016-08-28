module QueryHandler

open System.Linq
open System.Collections.Generic
open MtgSearcher.Indexer
open Scoring

let getSubIdx token =
    if cachedIndex.ContainsKey(token) then
        Some(new KeyValuePair<string, indexData>(token, cachedIndex.[token]))
    else
        None

let query (q:string) =
    q.Split(' ') |> Seq.toList |> List.map getSubIdx |> scoreResults