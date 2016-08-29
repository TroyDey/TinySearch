namespace TinySearch

module Persistence =

    open System.Collections.Generic

    val keyExists : string -> bool
    val getDocument : string -> string
    val getDocuments : string -> (string * string) list
    val persistDocuments : string -> KeyValuePair<string,'a> list -> unit
    val clearDatabase : unit -> unit
    val persistDatum : string -> string -> 'a -> unit
    val getDatum : string -> string
