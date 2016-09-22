namespace TinySearch

module Indexing =

    open System.Collections.Generic
    open Analyzers
    open SearchTypes

    val generateIndex : analyzer -> ('a -> string) -> Dictionary<string,'a> -> unit
    val initalizeIndexInMemory : unit -> unit
    val isInIdx : string -> bool
    val getIdxSegment : string -> indexData option
    val totalDocuments : unit -> int64

    val cachedIndex : Dictionary<string, indexData>
