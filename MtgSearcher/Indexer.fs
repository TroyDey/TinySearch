namespace MtgSearcher

module Indexer =

    open System.IO
    open System.Collections.Generic
    open System.Text
    open System.Linq
    open Newtonsoft.Json.Serialization
    open StackExchange.Redis
    open MtgCard

    //Operator to make the F# type inference happy with strings being used for Redis keys and values
    let inline (~~) (x:^a) : ^b = ((^a or ^b) : (static member op_Implicit: ^a -> ^b) x)

    //This is expensive to create so cache it
    let conMux = ConnectionMultiplexer.Connect("localhost,allowAdmin=true");

    //Since this will be lookup heavy use the standard dictionary that has O(1) lookup
    //The F# Map uses a binary tree and therefore its lookup is O(log n) 
    //Should still experiment with it
    let cachedIndex = new Dictionary<string, Dictionary<string, List<int64>>>()

    let ParseCardDataFromJsonFile (fileName:string) =
        use sr = new StreamReader(fileName)
        let jsonStr = sr.ReadToEnd()
        Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,Card>>(jsonStr)

    let aggregateText (card:Card) =
            let sb = new StringBuilder(1000) //use string builder since we could be jamming alot of text together
            sb.Append(card.Name).Append(" ").Append(System.String.Join(" ", if card.Colors = null then new List<string>() else card.Colors)).Append(" ").Append(card.ManaCost).Append(" ").Append(card.Type).Append(" ").Append(card.Text).ToString().Split(' ').ToList()

    let storeData (db:IDatabase) key data =
        let serializedCard = Newtonsoft.Json.JsonConvert.SerializeObject(data)
        db.StringSet(~~key, ~~serializedCard) |> ignore

    let persistData prefix data =
        let db = conMux.GetDatabase()
        let rec persistData' db prefix (data:KeyValuePair<string,_> list) =
            match data with
            | [] -> ()
            | h::t -> (storeData db (prefix + ":" + h.Key) h.Value); persistData' db prefix t
        persistData' db prefix data

    let clearDatabase () =
        let server = conMux.GetServer("localhost:6379")
        server.FlushDatabase()

    //Should convert this to a more elegant F# solution
    let buildInvertedIndex cards =
        //assume cards is already unique
        for (KeyValue(c,n)) in cards do
            let textList = aggregateText n
            let redisKey = "card:" + c
            let mutable idx = 0L
            for w in textList do
                if not(cachedIndex.ContainsKey(w)) then
                    let locref = new Dictionary<string,List<int64>>()
                    let loclist = new List<int64>()
                    loclist.Add(idx)
                    locref.Add(redisKey, loclist)
                    cachedIndex.Add(w, locref)
                else
                    let wordloc = cachedIndex.[w]
                    if wordloc.ContainsKey(redisKey) then
                        let loclist = wordloc.[redisKey]
                        loclist.Add(idx)
                    else
                        let loclist = new List<int64>()
                        loclist.Add(idx)
                        wordloc.Add(redisKey, loclist)
                idx <- idx + 1L
        cachedIndex.ToList()

    let generateIndex (cards:Dictionary<string,Card>) =
        //clear the existing index
        clearDatabase ()
        //This is our stored data, we could have fields that are either stored or indexed or both
        cards.ToList() |> List.ofSeq |> persistData "card"
        //Create inverse index
        buildInvertedIndex cards |> List.ofSeq |> persistData "word"