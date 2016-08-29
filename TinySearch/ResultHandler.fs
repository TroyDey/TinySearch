namespace TinySearch

module ResultHandler =

    open Scoring
    open Persistence

    type pagination = { pageIdx: int; rows: int }
    type outputResult = { doc: string; score: double; termHitCount: int; maxTerms: int; coordScore: double; debug: scoreDebug list }

    //just return card json string and debug info potentially take other transformers
    let outputResults resultPrinter debugPrinter (page:pagination) (results:scoredResult list) =
        results 
        |> List.skip (page.pageIdx * page.rows) 
        |> List.take page.rows
        //coordination.score and coordination.maxTerms should be guaranteed to have a value of at least 1 by this point 
        |> List.map (fun r -> { doc = (getDocument r.docId); score = r.score; termHitCount = r.coordination.termHitCount; maxTerms = r.coordination.maxTerms.Value; coordScore = r.coordination.score.Value; debug = r.debug })
        |> List.map (fun outRes -> (resultPrinter outRes; outRes)) 
        |> List.map (fun outRes -> (debugPrinter outRes; outRes))