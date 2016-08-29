namespace MtgSearcher

module QueryHandler = 
    
    open Analyzers
    open Scoring

    val query : analyzer -> string -> scoredResult list