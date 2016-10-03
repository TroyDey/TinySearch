namespace TinySearch

module Scoring =

    open System
    open System.Collections.Generic
    open SearchTypes

    val scoreResults : parsedQuery seq -> scoredResult seq

