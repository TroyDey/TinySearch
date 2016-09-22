﻿namespace TinySearch

module Indexing =

    open System
    open System.Collections.Generic
    open System.Linq
    open Newtonsoft.Json
    open Persistence
    open Analyzers
    open SearchTypes

    //Since this will be lookup heavy use the standard dictionary that has O(1) lookup
    //The F# Map uses a binary tree and therefore its lookup is O(log n) 
    //Should still experiment with it
    let cachedIndex = new index()

    let mutable internalTotal = -1L

    //Needs error handling
    let totalDocuments () =
        if internalTotal < 0L then
            let docTotal = getDatum "meta:totalDocuments"
            internalTotal <- int64(docTotal)
            internalTotal
        else
            internalTotal

    let deserializeIndexObject (idx:(string * string)) =
        let idxData = JsonConvert.DeserializeObject<fieldIndex>((snd idx))
        cachedIndex.Add((fst idx), idxData)
        idxData

    let updateFieldIndex filteredText cardName (fieldIndex: fieldIndex) =
        let redisKey = "card:" + cardName
        let mutable idx = 0L

        for w in filteredText do
            if not(fieldIndex.ContainsKey(w)) then
                let locref = new Dictionary<string,localData>()
                let loclist = new List<int64>()

                loclist.Add(idx)
                locref.Add(redisKey, { termFreq = 1L; fieldLength = filteredText.LongCount(); locations = loclist })
                fieldIndex.Add(w, { term = w; docFreq = 1L; docs = locref })
            else
                let wordloc = fieldIndex.[w]
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
        fieldIndex

    //Should convert this to a more elegant F# solution
    let buildInvertedIndex (indexAnalyzer:analyzer) (docParser:'a -> (string * string) list) docs =
        //assume cards is already unique
        for (KeyValue(cardName,card)) in docs do
            let fields = docParser card

            for f in fields do
                let tokenStream = indexAnalyzer.tokenizer (snd f)
                let filteredText = List.fold (fun a c -> (c a)) tokenStream indexAnalyzer.filters

                if cachedIndex.ContainsKey((fst f)) then
                    updateFieldIndex filteredText cardName cachedIndex.[(fst f)] |> ignore
                else
                    cachedIndex.[(fst f)] <- updateFieldIndex filteredText cardName (new fieldIndex())

        cachedIndex.ToList()

    //add the ability to index specific fields of a document separately instead of having to smash them all into one
    let generateIndex (indexAnalyzer:analyzer) (textAggregator:('a -> string)) (docParser:'a -> (string * string) list) (docs:Dictionary<string,'a>) =
        //clear the existing index
        clearDatabase ()
        
        //This is our stored data, we could have fields that are either stored or indexed or both
        docs.ToList()
        |> List.ofSeq 
        |> persistDocuments "card"
        
        internalTotal <- docs.LongCount()
        
        persistDatum "meta" "totalDocuments" totalDocuments
        
        //Create inverse index
        docs 
        |> buildInvertedIndex indexAnalyzer docParser
        |> List.ofSeq 
        |> persistDocuments "fieldIndex"

    let isInIdx (key:string) =
        keyExists key

    let getIdxSegment (key:string) =
        let redisKey = "fieldIndex:" + key
        let doc = getDocument redisKey

        if doc = String.Empty then
            None
        else
            Some(deserializeIndexObject (redisKey, doc))

    let initalizeIndexInMemory () =
        getDocuments "fieldIndex:*"
        |> List.map deserializeIndexObject
        |> ignore

        internalTotal <- cachedIndex.LongCount()