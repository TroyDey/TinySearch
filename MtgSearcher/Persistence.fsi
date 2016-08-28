namespace MtgSearcher

module Persistence =
    open System.Collections.Generic
    open StackExchange.Redis
    open Newtonsoft.Json
    open MtgCard

    val getDocument : string -> Card
    val persistDocuments : string -> KeyValuePair<string,'a> list -> unit
    val clearDatabase : unit -> unit
    val persistDatum : string -> string -> 'a -> unit
    val getDatum : string -> string
