namespace TinySearch

module Analyzers =

    type analyzer = { tokenizer: (string -> string seq); filters:(string seq -> string seq) seq }

    val defaultAnalyzer : analyzer
    val englishPossesiveAnalyzer : analyzer
    val stopFilterAnalyzer : analyzer
    val allFiltersAnalyzer : analyzer
