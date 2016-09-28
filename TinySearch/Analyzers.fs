namespace TinySearch

module Analyzers =
    open Tokenizers
    open TokenFilters

    type analyzer = { tokenizer: (string -> string seq); filters:(string seq -> string seq) seq }

    let defaultAnalyzer = { tokenizer = whitespacetokenizer; filters = [lowerCaseFilter] }
    let englishPossesiveAnalyzer = { tokenizer = whitespacetokenizer; filters = [lowerCaseFilter; englishPossesiveFilter] }
    let stopFilterAnalyzer = { tokenizer = whitespacetokenizer; filters = [lowerCaseFilter; stopFilterDefault] }
    let allFiltersAnalyzer = { tokenizer = whitespacetokenizer; filters = [lowerCaseFilter; stopFilterDefault; englishPossesiveFilter] }