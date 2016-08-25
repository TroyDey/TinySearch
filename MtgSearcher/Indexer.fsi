namespace MtgSearcher
module Indexer =
    open System.Collections.Generic
    open MtgCard

    val cachedIndex : Dictionary<string, Dictionary<string, List<int64>>>
    val ParseCardDataFromJsonFile : string -> Dictionary<string, Card>
    val generateIndex : Dictionary<string,Card> -> unit