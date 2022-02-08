module Env

open System

let tryString envVar =
    Environment.GetEnvironmentVariable(envVar)
    |> Option.ofObj

let tryInt envVar =
    tryString envVar
    |> Option.map int

let withDefault = Option.defaultValue
