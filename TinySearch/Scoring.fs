namespace TinySearch

module Scoring =

    open System
    open System.Collections.Generic
    open SearchTypes

    type internalScoredResult = {docId: string; mutable score: double; mutable debug:scoreDebug list}

    let updateScoreWithCoordinationFactor maxTerms scoredRes =
        let coordScore = double(scoredRes.coordination.termHitCount) / double(maxTerms)
        scoredRes.coordination.maxTerms <- Some(maxTerms)
        scoredRes.coordination.score <- Some(coordScore)
        scoredRes.score <- scoredRes.score * coordScore
        scoredRes

    let termFrequencyScore locData =
        Math.Sqrt(double(locData.termFreq))

    let inverseDocumentFrequencyScore totalDocs docFreq =
        Math.Pow(1.0 + Math.Log10(totalDocs / (docFreq + 1.0)), 2.0)

    let updateDocScore term docFreq (acc:Map<string,internalScoredResult>) (cur:KeyValuePair<string,localData>) =
        let tf = (termFrequencyScore cur.Value)
        let totalDocs = TinySearch.Indexing.totalDocuments ()
        let idf = inverseDocumentFrequencyScore (double(totalDocs)) (double(docFreq))
        let tfIdf = tf * idf
        let debug = { term = term; tf = tf; idf = idf; tfIdf = tfIdf; baseTf = cur.Value.termFreq; totalDocs = totalDocs; docFreq = docFreq }

        if acc.ContainsKey(cur.Key) then
            let scoredResult = acc.[cur.Key]
            if scoredResult.score < tfIdf then
                scoredResult.score <- tfIdf
            scoredResult.debug <- debug::scoredResult.debug
            acc.Add (cur.Key, scoredResult)
        else
            acc.Add (cur.Key, { docId = cur.Key; score = tfIdf; debug = [debug] })

    let calculateScore acc (cur:indexData) =
        let termUpdateDocScore = updateDocScore cur.term cur.docFreq
        cur.docs 
        |> Seq.fold termUpdateDocScore acc

    let scoreTerm (subIndex:parsedQuery) =
        Seq.fold calculateScore Map.empty subIndex.indexes

    let combineTermScores (acc:Map<string,scoredResult>) key cur =
        if acc.ContainsKey(key) then
            acc.[key].score <- acc.[key].score + cur.score
            acc.[key].coordination.termHitCount <- acc.[key].coordination.termHitCount + 1
            acc.[key].debug <- acc.[key].debug @ cur.debug
            acc
        else
            acc.Add(key, { docId = key; score = cur.score; coordination = { termHitCount = 1; maxTerms = None; score = None}; debug = cur.debug })

    let scoreResults (pquery:parsedQuery seq) =
        Seq.map scoreTerm pquery
        |> Seq.fold (fun a c -> Map.fold combineTermScores a c) Map.empty
        |> Seq.map (fun kvp -> kvp.Value)
        |> Seq.map (updateScoreWithCoordinationFactor (Seq.length pquery))
        |> Seq.sortByDescending (fun x -> x.score)

//        List.fold calculateScore Map.empty subIndex
//        |> Seq.map (fun kvp -> kvp.Value)
//        |> Seq.toList
//        |> List.map (updateScoreWithCoordinationFactor subIndex.Length)
//        |> List.sortByDescending (fun x -> x.score)