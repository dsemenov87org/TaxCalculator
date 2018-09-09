open System
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Caching.Distributed
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging

open Giraffe
open Giraffe.HttpStatusCodeHandlers.Successful
open FSharp.Control.Tasks.V2

open TaxCalculator.BusinessLogic
open TaxCalculator.DataLayer
open TaxCalculator.BusinessLogic.Common
open FluentMigrator.Runner
open FsConfig

// ---------------------------------
// Configuration
// ---------------------------------

type PostgresConfig =
    {   Host        : string
        Port        : int
        User        : string
        Password    : string
    }

type RedisConfig =
    {   Host        : string
        Instance    : string
    }

type TaxCalculatorConfig =
    {   LogLevel: LogLevel
        DbName  : string
    }

type Config = {
    Pg              : PostgresConfig
    Redis           : RedisConfig
    TaxCalculator   : TaxCalculatorConfig
}

let config = 
    match EnvConfig.Get<Config>() with
    | Ok config     -> config
    | Error error   -> 
        match error with
        | NotFound envVarName           -> failwithf "Environment variable %s not found" envVarName
        | BadValue (envVarName, value)  -> failwithf "Environment variable %s has invalid value %s" envVarName value
        | NotSupported msg              -> failwith msg

// ---------------------------------
// Utils
// ---------------------------------

let inline dec (f: float) : Decimal = Convert.ToDecimal f

let runMigrations (serviceProvider : IServiceProvider) =
    // Put the database update into a scope to ensure
    // that all resources will be disposed.
    use scope = serviceProvider.CreateScope()
    let runner =
        scope.ServiceProvider.GetRequiredService<IMigrationRunner>()
    in
        runner.MigrateUp()

[<EntryPoint>]
let main _ =
    WebHostBuilder()
        .UseKestrel()
        .Configure(fun app ->
            // ---------------------------------
            // Composition root
            // ---------------------------------
            let storageService =
                TaxStorageService(
                    TaxDatabaseSettings(config.Pg.Host,
                                        config.Pg.Port,
                                        config.Pg.User,
                                        config.Pg.Password,
                                        config.TaxCalculator.DbName),
                    app.ApplicationServices.GetService<IDistributedCache>())

            let calculatorFactory =
                TaxCalculatorFactory (storageService, storageService)

            // ---------------------------------
            // Migrations
            // ---------------------------------

            runMigrations app.ApplicationServices

            // ---------------------------------
            // Error handler
            // ---------------------------------

            let errorHandler (ex : Exception) (logger : ILogger) =
                logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
                clearResponse >=> setStatusCode 500 >=> (ex |> string |> text)

            // ---------------------------------
            // Web app
            // ---------------------------------
            
            let companyTypeRoute companyType =
                let paramRouting taxSystem =
                    choose [
                        routef "/%f/%f/%f" (fun (input, output, salary) next ctx ->
                            task {
                                let! calculator =
                                    calculatorFactory.CreateCalculator(companyType, taxSystem)

                                let! response =
                                    calculator.Invoke
                                        (CustomerTaxParameters(
                                            RurMoney (dec input),
                                            RurMoney (dec output),
                                            RurMoney (dec salary)))
                                
                                return! (response |> json |> ok) next ctx
                            })
                    ]

                choose [
                    subRoute "/osn" (paramRouting EAccountTaxationSystem.Osn)
                    subRoute "/usn6" (paramRouting EAccountTaxationSystem.Usn6)
                    subRoute "/usn15" (paramRouting EAccountTaxationSystem.Usn15)
                ]

            let webapp =
                GET >=> choose [
                    subRoute "/api/ip" (companyTypeRoute ECompanyType.IP)
                    subRoute "/api/ooo" (companyTypeRoute ECompanyType.OOO)

                    setStatusCode 404 >=> text "Not found"
                ]

            app.UseGiraffeErrorHandler errorHandler |> ignore
            app.UseGiraffe webapp)

        .ConfigureServices(fun services ->
            services
                .AddFluentMigratorCore()
                .ConfigureRunner(fun rb ->
                    rb.AddPostgres()
                        .WithGlobalConnectionString(
                            sprintf
                                "server=%s;userid=%s;port=%d;password=%s;database=%s;"
                                config.Pg.Host
                                config.Pg.User
                                config.Pg.Port
                                config.Pg.Password
                                config.TaxCalculator.DbName)
                        .ScanIn(typedefof<TaxStorageService>.Assembly)
                        .For.Migrations() |> ignore)
                .AddLogging(fun lb -> lb.AddFluentMigratorConsole() |> ignore)
                .BuildServiceProvider(false)
                |> ignore

            services.AddGiraffe() |> ignore
            
            services.AddDistributedRedisCache(fun options ->
                options.Configuration   <- config.Redis.Host
                options.InstanceName    <- config.Redis.Instance) |> ignore)
        
        .ConfigureLogging(fun builder ->
            builder
                .AddFilter((=) config.TaxCalculator.LogLevel)
                .AddConsole()
                |> ignore)
        .Build()
        .Run()
    0