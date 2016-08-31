namespace TinySearch

module TokenFilters =

    open System.Collections.Generic

    val lowerCaseFilter : string list -> string list
    val englishPossesiveFilter : string list -> string list
    val stopFilter : HashSet<string> -> string list -> string list
    val stopFilterDefault : string list -> string list