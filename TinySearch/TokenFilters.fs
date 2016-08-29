namespace TinySearch

module TokenFilters =
    open System

    let lowerCaseFilter (tokenStream:string list) =
        List.map (fun (token:string) -> token.ToLowerInvariant()) tokenStream
