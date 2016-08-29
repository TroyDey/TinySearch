namespace TinySearch

module Persistence =
    
    open System
    open System.Collections.Generic
    open StackExchange.Redis
    open Newtonsoft.Json

    //Operator to make the F# type inference happy with strings being used for Redis keys and values
    let inline (~~) (x:^a) : ^b = ((^a or ^b) : (static member op_Implicit: ^a -> ^b) x)

    //This is expensive to create so cache it
    let conMux = ConnectionMultiplexer.Connect("localhost,allowAdmin=true");

    let keyExists (documentId:string) =
        let db = conMux.GetDatabase()
        db.KeyExists(~~documentId)

    //Break strong type to the card type
    let getDocument (documentId:string) =
        let db = conMux.GetDatabase()
        let doc = db.StringGet(~~documentId)
        if doc.IsNull then
            String.Empty
        else
            doc.ToString()

    let getDocuments (keyPattern:string) =
        let server = conMux.GetServer("localhost:6379")
        server.Keys(pattern = ~~keyPattern)
        |> Seq.map (fun rk -> rk.ToString())
        |> Seq.map (fun k -> (k, getDocument k))
        |> Seq.toList

    let storeData (db:IDatabase) key data =
        let serializedData = JsonConvert.SerializeObject(data)
        db.StringSet(~~key, ~~serializedData) |> ignore

    let persistDocuments prefix documents =
        let db = conMux.GetDatabase()
        let rec persistDocuments' db prefix (documents:KeyValuePair<string,_> list) =
            match documents with
            | [] -> ()
            | h::t -> (storeData db (prefix + ":" + h.Key) h.Value); persistDocuments' db prefix t
        persistDocuments' db prefix documents

    let clearDatabase () =
        let server = conMux.GetServer("localhost:6379")
        server.FlushDatabase()

    let persistDatum prefix key datum =
        conMux.GetDatabase().StringSet(~~(prefix + ":" + key), ~~(datum.ToString())) |> ignore

    let getDatum (key:string) =
        let db = conMux.GetDatabase()
        db.StringGet(~~key).ToString()