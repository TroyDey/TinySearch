namespace TinySearch

module TokenFilters =
    open System

    let lowerCaseFilter (tokenStream:string list) =
        List.map (fun (token:string) -> token.ToLowerInvariant()) tokenStream

    //Note: this will also strip 's from contractions
    let englishPossesiveFilter (tokenStream:string list) =
        List.map (fun (token:string) -> if token.EndsWith("'s") || token.EndsWith("'S") then token.Substring(0, token.Length-2) else token) tokenStream
