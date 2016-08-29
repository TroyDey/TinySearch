namespace TinySearch

module Indexing =

    open System.Collections.Generic
    open System.Linq
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

    //Should convert this to a more elegant F# solution
    let buildInvertedIndex (indexAnalyzer:analyzer) (textAggregator:('a -> string)) docs =
        //assume cards is already unique
        for (KeyValue(c,n)) in docs do
            let text = textAggregator n
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

    //add the ability to index specific fields of a document separately instead of having to smash them all into one
    let generateIndex (indexAnalyzer:analyzer) (textAggregator:('a -> string)) (docs:Dictionary<string,'a>) =
        //clear the existing index
        clearDatabase ()
        
        //This is our stored data, we could have fields that are either stored or indexed or both
        docs.ToList()
        |> List.ofSeq 
        |> persistDocuments "card"
        
        totalDocuments <- docs.LongCount()
        
        persistDatum "meta" "totalDocuments" totalDocuments
        
        //Create inverse index
        docs 
        |> buildInvertedIndex indexAnalyzer textAggregator 
        |> List.ofSeq 
        |> persistDocuments "word"