namespace MtgSearcher

module Persistence =

    open System.Collections.Generic

    val getDocument : string -> string
    val persistDocuments : string -> KeyValuePair<string,'a> list -> unit
    val clearDatabase : unit -> unit
    val persistDatum : string -> string -> 'a -> unit
    val getDatum : string -> string
