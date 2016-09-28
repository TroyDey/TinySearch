namespace TinySearch

module TokenFilters =

    open System.Collections.Generic

    val lowerCaseFilter : string seq -> string seq
    val englishPossesiveFilter : string seq -> string seq
    val stopFilter : HashSet<string> -> string seq -> string seq
    val stopFilterDefault : string seq -> string seq