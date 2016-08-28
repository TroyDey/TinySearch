namespace MtgSearcher
module ResultHandler =

    open System
    open MtgSearcher.Scoring
    open MtgCard
    open MtgSearcher.Indexing

    type pagination = { pageIdx: int; rows: int }
    type outputResult = { doc: Card; score: double; termHitCount: int; maxTerms: int; coordScore: double; debug: scoreDebug list }

    val outputResults : (outputResult -> unit) -> (outputResult -> unit) -> pagination -> scoredResult list -> outputResult list
    val printCard : outputResult -> unit
    val printDebug : outputResult -> unit
