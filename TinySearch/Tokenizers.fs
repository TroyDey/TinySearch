namespace TinySearch

module Tokenizers =

    open System
    open System.Linq
    open System.Collections.Generic
    open System.Text.RegularExpressions

    let whitespacetokenizer source =
        Regex.Matches(source, "\S+").Cast<Match>() |> Seq.map (fun m -> m.Value) |> Seq.toList

