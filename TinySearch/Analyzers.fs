namespace TinySearch

module Analyzers =
    open Tokenizers
    open TokenFilters

    type analyzer = { tokenizer: (string -> string list); filters:(string list -> string list) list }

    let defaultAnalyzer = { tokenizer = whitespacetokenizer; filters = [lowerCaseFilter] }