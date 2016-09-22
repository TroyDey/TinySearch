namespace TinySearch

module QueryHandler = 
    
    open Analyzers
    open Scoring
    open SearchTypes

    val query : analyzer -> string list -> string -> scoredResult list