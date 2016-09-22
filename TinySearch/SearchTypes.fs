namespace TinySearch

module SearchTypes =
    open System.Collections.Generic

    type localData = { mutable termFreq: int64; fieldLength: int64; locations: List<int64> }
    type indexData = { term: string; mutable docFreq: int64; docs: Dictionary<string, localData> }
    type fieldIndex = Dictionary<string, indexData>
    type index = Dictionary<string, fieldIndex>

    type scoreDebug = {term: string; tf: double; idf: double; tfIdf: double; baseTf: int64; totalDocs: int64; docFreq: int64}
    type coordinationScore = {mutable termHitCount: int; mutable maxTerms: int option; mutable score: double option}
    type scoredResult = {docId: string; mutable score: double; mutable coordination: coordinationScore; mutable debug:scoreDebug list}

    type parsedQuery = { token: string; indexes: indexData list}

    type pagination = { pageIdx: int; rows: int }
    type outputResult = { doc: string; score: double; termHitCount: int; maxTerms: int; coordScore: double; debug: scoreDebug list }