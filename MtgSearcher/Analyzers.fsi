﻿namespace MtgSearcher

module Analyzers =

    type analyzer = { tokenizer: (string -> string list); filters:(string list -> string list) list }

    val defaultAnalyzer: analyzer