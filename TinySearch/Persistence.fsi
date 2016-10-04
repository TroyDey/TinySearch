namespace TinySearch

module Persistence =

    open System.Collections.Generic

    open SearchTypes

    val keyExists : string -> bool
    val getDocumentString : string -> string
    val getDocument : string -> fieldIndex option
    val getDocuments : string -> (string * fieldIndex option) list
    val persistDocuments : string -> KeyValuePair<string,'a> list -> unit
    val clearDatabase : unit -> unit
    val persistDatum : string -> string -> 'a -> unit
    val getDatum : string -> string
