namespace TinySearch

module ResultHandler =

    open SearchTypes
    open Persistence

    //just return card json string and debug info potentially take other transformers
    let outputResults (page:pagination) (results:scoredResult seq) =
        //need to create a skip method that does not throw when out of elements, since calling length here enumerates the entire sequence
        let resultLength = results |> Seq.length
        let skip = page.pageIdx * page.rows

        if skip > resultLength then
            Seq.empty
        else
            results 
            |> Seq.skip skip
            |> Seq.truncate page.rows
            //coordination.score and coordination.maxTerms should be guaranteed to have a value of at least 1 by this point 
            |> Seq.map (fun r -> { doc = (getDocumentString r.docId); score = r.score; termHitCount = r.coordination.termHitCount; maxTerms = r.coordination.maxTerms.Value; coordScore = r.coordination.score.Value; debug = r.debug })