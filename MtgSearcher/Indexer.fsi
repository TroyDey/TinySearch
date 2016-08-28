namespace MtgSearcher
module Indexer =
    open System.Collections.Generic
    open MtgCard

    type localData = { term: string; mutable termFreq: int64; fieldLength: int64; locations: List<int64> }
    type indexData = { mutable docFreq: int64; docs: Dictionary<string, localData> }

    val cachedIndex : Dictionary<string, indexData>
    val ParseCardDataFromJsonFile : string -> Dictionary<string, Card>
    val generateIndex : Dictionary<string,Card> -> unit
    val getDoc : string -> Card

    val mutable totalDocuments : int64