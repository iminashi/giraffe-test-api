module Db

open Npgsql.FSharp

let connectionString =
    "Host=localhost; Database=postgres; Username=postgres; Password=postgres;"

type Test = { Id: int; Name: string }

let getAll () =
    connectionString
    |> Sql.connect
    |> Sql.query "SELECT * FROM test"
    |> Sql.executeAsync (fun read ->
        { Id = read.int "id"
          Name = read.text "name" })
