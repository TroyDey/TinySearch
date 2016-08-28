namespace MtgSearcher
module Indexing =
    open System.Collections.Generic
    open MtgCard
    open Persistence

    type localData = { mutable termFreq: int64; fieldLength: int64; locations: List<int64> }
    type indexData = { term: string; mutable docFreq: int64; docs: Dictionary<string, localData> }

    val cachedIndex : Dictionary<string, indexData>
    val ParseCardDataFromJsonFile : string -> Dictionary<string, Card>
    val generateIndex : Dictionary<string,Card> -> unit

    val mutable totalDocuments : int64