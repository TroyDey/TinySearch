module Scoring

open System
open System.Linq
open System.Collections.Generic
open MtgSearcher.Indexer

type scoreDebug = {term: string; tf: double; idf: double; tfIdf: double; baseTf: int64; totalDocs: int64; docFreq: int64}
type coordinationScore = {mutable termHitCount: int; mutable maxTerms: int option; mutable score: double option}
type scoredResult = {cardId: string; mutable score: double; mutable coordination: coordinationScore; mutable debug:scoreDebug list}

let updateScoreWithCoordinationFactor maxTerms scoredRes =
    let coordScore = (double(scoredRes.coordination.termHitCount) / double(maxTerms))
    scoredRes.coordination.maxTerms <- Some(maxTerms)
    scoredRes.coordination.score <- Some(coordScore)
    scoredRes.score <- scoredRes.score * coordScore
    scoredRes

let termFrequencyScore (locData:localData) =
    Math.Sqrt(double(locData.termFreq))

let inverseDocumentFrequencyScore totalDocs docFreq =
    1.0 + Math.Log10(totalDocs / (docFreq + 1.0))

let updateDocScore term (docFreq:int64) (acc:Dictionary<string,scoredResult>) (cur:KeyValuePair<string,localData>) =
    let tf = (termFrequencyScore cur.Value)
    let totalDocs = MtgSearcher.Indexer.totalDocuments
    let idf = (Math.Pow((inverseDocumentFrequencyScore (double(MtgSearcher.Indexer.totalDocuments)) (double(docFreq))), 2.0))
    let tfIdf = tf * idf
    let debug = { term = term; tf = tf; idf = idf; tfIdf = tfIdf; baseTf = cur.Value.termFreq; totalDocs = totalDocs; docFreq = docFreq }

    if acc.ContainsKey(cur.Key) then
        let scoredResult = acc.[cur.Key]
        scoredResult.score <- scoredResult.score + tfIdf
        scoredResult.debug <- debug::scoredResult.debug
        scoredResult.coordination.termHitCount <- scoredResult.coordination.termHitCount + 1
        acc.[cur.Key] <- scoredResult
    else
        acc.Add(cur.Key, { cardId = cur.Key; score = tfIdf; coordination = { termHitCount = 1; maxTerms = None; score = None}; debug = [debug] })
    acc

let calculateScore term docFreq acc (docs:Dictionary<string,localData>) =
    let termUpdateDocScore = updateDocScore term docFreq
    List.fold termUpdateDocScore acc (docs.ToList() |> List.ofSeq)

let scoreResults (subIndex:KeyValuePair<string, indexData> option list) =
    let scoreResults' (acc:Dictionary<string,scoredResult>) (cur:KeyValuePair<string, indexData> option) =
        match cur with
        | None -> acc
        | Some(kvp) -> calculateScore kvp.Key kvp.Value.docFreq acc kvp.Value.docs
    (List.fold scoreResults' (new Dictionary<string,scoredResult>()) subIndex) 
    |> Seq.map (fun kvp -> kvp.Value) 
    |> Seq.toList
    |> List.map (updateScoreWithCoordinationFactor subIndex.Length) 
    |> List.sortByDescending (fun x -> x.score)