namespace MtgSearcher

module QueryHandler = 
    open Indexing
    open Scoring

    val query : string -> scoredResult list