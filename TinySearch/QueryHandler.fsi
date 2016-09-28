namespace TinySearch

module QueryHandler = 
    
    open Analyzers
    open Scoring
    open SearchTypes

    val query : analyzer -> string seq -> string -> scoredResult list