module WebApp

open Giraffe

let getAll next ctx =
    task {
        let! data = Db.getAll ()
        return! json data next ctx
    }

let app: HttpHandler =
    choose [
        GET >=> route "/" >=> getAll
        setStatusCode 404 >=> text "Not Found"
    ]
