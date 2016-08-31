namespace TinySearch

module Analyzers =

    type analyzer = { tokenizer: (string -> string list); filters:(string list -> string list) list }

    val defaultAnalyzer : analyzer
    val englishPossesiveAnalyzer : analyzer
    val stopFilterAnalyzer : analyzer
    val allFiltersAnalyzer : analyzer
