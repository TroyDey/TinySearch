namespace MtgSearcher
module ResultHandler =

    open System
    open Scoring
    open MtgCard
    open Indexing

    type pagination = { pageIdx: int; rows: int }
    type outputResult = { doc: Card; score: double; termHitCount: int; maxTerms: int; coordScore: double; debug: scoreDebug list }

    let outputResults resultPrinter debugPrinter (page:pagination) (results:scoredResult list) =
        results 
        |> List.skip (page.pageIdx * page.rows) 
        |> List.take page.rows
        //coordination.score and coordination.maxTerms should be guaranteed to have a value of at least 1 by this point 
        |> List.map (fun r -> { doc = (getDoc r.cardId); score = r.score; termHitCount = r.coordination.termHitCount; maxTerms = r.coordination.maxTerms.Value; coordScore = r.coordination.score.Value; debug = r.debug })
        |> List.map (fun outRes -> (resultPrinter outRes; outRes)) 
        |> List.map (fun outRes -> (debugPrinter outRes; outRes))

    let printCard (result:outputResult) =
        let r = result.doc
        printfn "%s     %s\r\n%s\r\n%s\r\n%s%s%s\r\n" r.Name r.ManaCost r.Type r.Text r.Power (if System.String.IsNullOrWhiteSpace(r.Power) && String.IsNullOrWhiteSpace(r.Toughness) then "NA/NA" else "/") r.Toughness

    let printDebug (result:outputResult) =
        printfn "%s - Total Score: %e\r\n{" result.doc.Name result.score
        printfn "\tTerm Hit Count: %d" result.termHitCount
        printfn "\tTotal Terms: %d" result.maxTerms
        printfn "\tCoordination Factor: %e\r\n" result.coordScore
        List.map (fun (sd:scoreDebug) -> printfn "\tterm: %s\r\n\ttf: %e\r\n\tidf: %e\r\n\ttfidf: %e\r\n\tbasetf: %d\r\n\ttotalDocs: %d\r\n\tdocFreq: %d\r\n" sd.term sd.tf sd.idf sd.tfIdf sd.baseTf sd.totalDocs sd.docFreq) result.debug |> ignore
        printfn "}\r\n"