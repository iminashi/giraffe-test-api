module Program

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe

let errorHandler (ex: Exception) (logger: ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")

    clearResponse
    >=> ServerErrors.INTERNAL_ERROR ex.Message

[<RequireQualifiedAccess>]
module Configure =
    let cors (builder: CorsPolicyBuilder) =
        builder
            .WithOrigins("http://localhost:5000", "https://localhost:5001")
            .AllowAnyMethod()
            .AllowAnyHeader()
        |> ignore

    let services (services: IServiceCollection) =
        services.AddCors().AddGiraffe() |> ignore

    let app (app: IApplicationBuilder) =
        let env = app.ApplicationServices.GetService<IWebHostEnvironment>()

        let app =
            match env.IsDevelopment() with
            | true -> app.UseDeveloperExceptionPage()
            | false ->
                app
                    .UseGiraffeErrorHandler(errorHandler)
                    .UseHttpsRedirection()

        app
            .UseCors(cors)
            .UseStaticFiles()
            .UseGiraffe(WebApp.app)

    let logging (builder: ILoggingBuilder) =
        builder.AddConsole().AddDebug() |> ignore

[<EntryPoint>]
let main args =
    let contentRoot = Directory.GetCurrentDirectory()
    //let webRoot = Path.Combine(contentRoot, "WebRoot")

    Host
        .CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(fun webHostBuilder ->
            webHostBuilder
                .UseContentRoot(contentRoot)
                //.UseWebRoot(webRoot)
                .ConfigureServices(
                    Configure.services
                )
                .Configure(Configure.app)
                .ConfigureLogging(Configure.logging)
            |> ignore)
        .Build()
        .Run()

    0
