namespace TinySearch

module ResultHandler =

    open TinySearch.Scoring

    type pagination = { pageIdx: int; rows: int }
    type outputResult = { doc: string; score: double; termHitCount: int; maxTerms: int; coordScore: double; debug: scoreDebug list }

    val outputResults : pagination -> scoredResult list -> outputResult list
