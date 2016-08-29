namespace MtgSearcher

module QueryHandler = 
    
    open Analyzers
    open Indexing
    open Scoring

    val query : analyzer -> string -> scoredResult list