namespace TinySearch

module ResultHandler =

    open SearchTypes
    open Persistence

    //just return card json string and debug info potentially take other transformers
    let outputResults (page:pagination) (results:scoredResult list) =
        results 
        |> List.skip (page.pageIdx * page.rows) 
        |> List.take (if page.rows > results.Length then results.Length else page.rows)
        //coordination.score and coordination.maxTerms should be guaranteed to have a value of at least 1 by this point 
        |> List.map (fun r -> { doc = (getDocument r.docId); score = r.score; termHitCount = r.coordination.termHitCount; maxTerms = r.coordination.maxTerms.Value; coordScore = r.coordination.score.Value; debug = r.debug })