namespace TinySearch

module TokenFilters =

    open System
    open System.Collections.Generic

    let stopWords = new HashSet<string>(["a"; "an"; "and"; "are"; "as"; "at"; "be"; "but"; "by";
                                        "for"; "if"; "in"; "into"; "is"; "it";
                                        "no"; "not"; "of"; "on"; "or"; "such";
                                        "that"; "the"; "their"; "then"; "there"; "these";
                                        "they"; "this"; "to"; "was"; "will"; "with"])

    let lowerCaseFilter tokenStream =
        Seq.map (fun (token:string) -> token.ToLowerInvariant()) tokenStream

    //Note: this will also strip 's from contractions
    let englishPossesiveFilter tokenStream =
        Seq.map (fun (token:string) -> if token.EndsWith("'s") || token.EndsWith("'S") then token.Substring(0, token.Length-2) else token) tokenStream

    let stopFilter (stopWords:HashSet<string>) tokenStream =
        Seq.filter (fun (token:string) -> not (stopWords.Contains(token))) tokenStream

    let stopFilterDefault tokenStream =
        stopFilter stopWords tokenStream