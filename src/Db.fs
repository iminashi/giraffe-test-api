module Db

open Npgsql.FSharp
open System

type Test = { Id: int; Name: string }

let connectionString =
    let host =
        Env.tryString "POSTGRES_HOST"
        |> Env.withDefault "localhost"

    let port =
        Env.tryInt "POSTGRES_PORT"
        |> Env.withDefault 5432

    Sql.host host
    |> Sql.database "postgres"
    |> Sql.username "postgres"
    |> Sql.password "postgres"
    |> Sql.port port
    |> Sql.formatConnectionString

let getAll () =
    connectionString
    |> Sql.connect
    |> Sql.query "SELECT * FROM test"
    |> Sql.executeAsync (fun read ->
        { Id = read.int "id"
          Name = read.text "name" })
