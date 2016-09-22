namespace TinySearch

module Indexing =

    open System.Collections.Generic
    open Analyzers
    open SearchTypes

    val generateIndex : analyzer -> ('a -> string) -> ('a -> (string * string) list) -> Dictionary<string,'a> -> unit
    val initalizeIndexInMemory : unit -> unit
    val isInIdx : string -> bool
    val getIdxSegment : string -> fieldIndex option
    val totalDocuments : unit -> int64

    val cachedIndex : index
