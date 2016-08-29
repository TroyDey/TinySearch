﻿namespace TinySearch

module Indexing =

    open System.Collections.Generic
    open Analyzers

    type localData = { mutable termFreq: int64; fieldLength: int64; locations: List<int64> }
    type indexData = { term: string; mutable docFreq: int64; docs: Dictionary<string, localData> }

    val generateIndex : analyzer -> ('a -> string) -> Dictionary<string,'a> -> unit

    val cachedIndex : Dictionary<string, indexData>
    val mutable totalDocuments : int64