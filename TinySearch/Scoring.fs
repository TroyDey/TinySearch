namespace TinySearch

module Scoring =

    open System
    open System.Collections.Generic
    open SearchTypes

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

    let updateDocScore term docFreq (acc:Map<string,scoredResult>) (cur:KeyValuePair<string,localData>) =
        let tf = (termFrequencyScore cur.Value)
        let totalDocs = TinySearch.Indexing.totalDocuments ()
        let idf = inverseDocumentFrequencyScore (double(totalDocs)) (double(docFreq))
        let tfIdf = tf * idf
        let debug = { term = term; tf = tf; idf = idf; tfIdf = tfIdf; baseTf = cur.Value.termFreq; totalDocs = totalDocs; docFreq = docFreq }

        if acc.ContainsKey(cur.Key) then
            let scoredResult = acc.[cur.Key]
            scoredResult.score <- scoredResult.score + tfIdf
            scoredResult.debug <- debug::scoredResult.debug
            scoredResult.coordination.termHitCount <- scoredResult.coordination.termHitCount + 1
            acc.Add (cur.Key, scoredResult)
        else
            acc.Add (cur.Key, { docId = cur.Key; score = tfIdf; coordination = { termHitCount = 1; maxTerms = None; score = None}; debug = [debug] })

    let calculateScore acc (cur:indexData) =
        let termUpdateDocScore = updateDocScore cur.term cur.docFreq
        cur.docs 
        |> List.ofSeq
        |> List.fold termUpdateDocScore acc

    let scoreResults subIndex =
        List.fold calculateScore Map.empty subIndex
        |> Seq.map (fun kvp -> kvp.Value)
        |> Seq.toList
        |> List.map (updateScoreWithCoordinationFactor subIndex.Length)
        |> List.sortByDescending (fun x -> x.score)