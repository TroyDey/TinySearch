namespace TinySearch

module Tokenizers =

    open System
    open System.Linq
    open System.Collections.Generic
    open System.Text.RegularExpressions

    val whitespacetokenizer : string -> string list
