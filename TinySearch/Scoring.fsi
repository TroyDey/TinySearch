namespace TinySearch

module Scoring =

    open System
    open System.Collections.Generic
    open Indexing

    type scoreDebug = {term: string; tf: double; idf: double; tfIdf: double; baseTf: int64; totalDocs: int64; docFreq: int64}
    type coordinationScore = {mutable termHitCount: int; mutable maxTerms: int option; mutable score: double option}
    type scoredResult = {docId: string; mutable score: double; mutable coordination: coordinationScore; mutable debug:scoreDebug list}

    val scoreResults : indexData list -> scoredResult list

