namespace MtgSearcher

module Indexing =

    open System.IO
    open System.Collections.Generic
    open System.Text
    open System.Linq
    open Newtonsoft.Json.Serialization
    open MtgCard
    open Persistence
    open Analyzers

    type localData = { mutable termFreq: int64; fieldLength: int64; locations: List<int64> }
    type indexData = { term: string; mutable docFreq: int64; docs: Dictionary<string, localData> }

    //Since this will be lookup heavy use the standard dictionary that has O(1) lookup
    //The F# Map uses a binary tree and therefore its lookup is O(log n) 
    //Should still experiment with it
    let cachedIndex = new Dictionary<string, indexData>()

    //Get from persistence and memoize instead of being mutable
    let mutable totalDocuments = 0L

    let ParseCardDataFromJsonFile (fileName:string) =
        use sr = new StreamReader(fileName)
        let jsonStr = sr.ReadToEnd()
        Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,Card>>(jsonStr)

    let aggregateText (card:Card) =
            let sb = new StringBuilder(1000) //use string builder since we could be jamming alot of text together
            sb.Append(card.Name).Append(" ").Append(System.String.Join(" ", if card.Colors = null then new List<string>() else card.Colors)).Append(" ").Append(card.ManaCost).Append(" ").Append(card.Type).Append(" ").Append(card.Text).ToString()

    //Should convert this to a more elegant F# solution
    let buildInvertedIndex (indexAnalyzer:analyzer) cards =
        //assume cards is already unique
        for (KeyValue(c,n)) in cards do
            let text = aggregateText n
            let tokenStream = indexAnalyzer.tokenizer text
            let filteredText = List.fold (fun a c -> (c a)) tokenStream indexAnalyzer.filters
            let redisKey = "card:" + c
            let mutable idx = 0L
            for w in filteredText do
                if not(cachedIndex.ContainsKey(w)) then
                    let locref = new Dictionary<string,localData>()
                    let loclist = new List<int64>()
                    loclist.Add(idx)
                    locref.Add(redisKey, { termFreq = 1L; fieldLength = filteredText.LongCount(); locations = loclist })
                    cachedIndex.Add(w, { term = w; docFreq = 1L; docs = locref })
                else
                    let wordloc = cachedIndex.[w]
                    if wordloc.docs.ContainsKey(redisKey) then
                        let doc = wordloc.docs.[redisKey]
                        doc.locations.Add(idx)
                        doc.termFreq <- doc.termFreq + 1L
                    else
                        let loclist = new List<int64>()
                        loclist.Add(idx)
                        wordloc.docFreq <- wordloc.docFreq + 1L
                        wordloc.docs.Add(redisKey, { termFreq = 1L; fieldLength = filteredText.LongCount(); locations = loclist })
                idx <- idx + 1L
        cachedIndex.ToList()

    let generateIndex (indexAnalyzer:analyzer) (cards:Dictionary<string,Card>) =
        //clear the existing index
        clearDatabase ()
        
        //This is our stored data, we could have fields that are either stored or indexed or both
        cards.ToList() 
        |> List.ofSeq 
        |> persistDocuments "card"
        
        totalDocuments <- cards.LongCount()
        
        persistDatum "meta" "totalDocuments" totalDocuments
        
        //Create inverse index
        cards 
        |> buildInvertedIndex indexAnalyzer 
        |> List.ofSeq 
        |> persistDocuments "word"