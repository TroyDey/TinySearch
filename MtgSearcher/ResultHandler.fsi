namespace MtgSearcher

module ResultHandler =

    open MtgSearcher.Scoring

    type pagination = { pageIdx: int; rows: int }
    type outputResult = { doc: string; score: double; termHitCount: int; maxTerms: int; coordScore: double; debug: scoreDebug list }

    val outputResults : (outputResult -> unit) -> (outputResult -> unit) -> pagination -> scoredResult list -> outputResult list
