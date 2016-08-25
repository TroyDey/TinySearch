module Scoring

open System
open System.Collections.Generic

type scoreRecord = {cardId: string; score: double}

let normalizeScores smallerIsBetter (scores: scoreRecord list) =
    let min = 0.000001
    if smallerIsBetter then
        let minScore = scores |> List.map (fun s -> s.score) |> List.min
        scores |> List.map (fun s -> s.cardId, minScore/Math.Max(min, s.score)) |> Map.ofList
    else
        let maxScore = scores |> List.map (fun s -> s.score) |> List.max
        scores |> List.map (fun s -> s.cardId, s.score/maxScore) |> Map.ofList

//Score based on word frequency.  Does not deal with the same card having hits for
//multiple search terms.
let wordFrequencyScore rows =
    let rec wordFrequencyScore' (rows:KeyValuePair<string,List<int64>> list) scores =
        match rows with
        | [] -> scores
        | h::t -> wordFrequencyScore' t ({cardId = h.Key; score = (double(h.Value.Count))}::scores)
    wordFrequencyScore' rows [] |> normalizeScores false