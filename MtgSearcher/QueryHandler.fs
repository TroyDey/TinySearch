module QueryHandler

open System.Linq
open System.Collections.Generic
open MtgSearcher.Indexer
open Scoring

let scoreMatches (rows:KeyValuePair<string, List<int64>> list) =
    //apply a list of scoring functions with their weights to the rows
    rows |> wordFrequencyScore |> Map.map (fun k v -> v * 1.5)


//Allow query to take in a list of scoring functions and their weights to apply
let query tokens =
    tokens |> List.map (function t -> cachedIndex.[t].ToList() |> List.ofSeq) |> List.fold (fun a c -> c @ a) [] |> scoreMatches |> Map.toList |> List.sortByDescending (fun x -> snd x)