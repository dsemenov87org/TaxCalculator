open System
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging

open Giraffe
open Giraffe.HttpStatusCodeHandlers.Successful
open FSharp.Control.Tasks.V2

open TaxCalculator.BusinessLogic
open TaxCalculator.DataLayer
open TaxCalculator.BusinessLogic.Common
open FsConfig
open Microsoft.Extensions.Caching.Distributed

open Vostok.Instrumentation.AspNetCore
open Vostok.Hosting
open Vostok.Metrics
open Serilog
open Serilog.Events
open Vostok.Logging
open Vostok.Logging.Serilog

// ---------------------------------
// Prelude
// ---------------------------------

let inline dec (f: float) : Decimal = Convert.ToDecimal f

// ---------------------------------
// Configuration
// ---------------------------------

type PostgresConfig =
    {   Host        : string
    }

type RedisConfig =
    {   Host        : string
        Instance    : string
    }

type TaxCalculatorConfig =
    {   LogLevel: LogEventLevel
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
// Telemetry
// ---------------------------------

let private createHostLog() : ILog =
    let loggerConfiguration =
        LoggerConfiguration()
            .MinimumLevel.Debug()

    loggerConfiguration
        .WriteTo.Console(
            config.TaxCalculator.LogLevel,
            "{Timestamp:HH:mm:ss.fff} {Level:u3} [{Thread}] {Message:l}{NewLine}{Exception}") |> ignore

    let logger = loggerConfiguration.CreateLogger()

    in SerilogLog(logger) :> ILog
        

let configureApplication(app : Microsoft.AspNetCore.Builder.IApplicationBuilder) =
    // ---------------------------------
    // Composition root
    // ---------------------------------
    let storageService =
        TaxStorageService(
            TaxDatabaseSettings(config.Pg.Host, config.TaxCalculator.DbName),
            app.ApplicationServices.GetService<IDistributedCache>())

    let calculatorFactory =
        TaxCalculatorFactory (storageService, storageService)

    // ---------------------------------
    // Error handler
    // ---------------------------------

    let errorHandler (ex : Exception) _ =
        clearResponse >=> setStatusCode 500 >=> (ex |> string |> text) // for debug mode

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

    app.UseVostok() |> ignore
    app.UseGiraffeErrorHandler errorHandler |> ignore
    app.UseGiraffe webapp

type TelemetryApplication() =
    inherit AspNetCoreVostokApplication()

    override __.OnStarted (hostingEnvironment: IVostokHostingEnvironment) =
        hostingEnvironment.MetricScope.SystemMetrics(TimeSpan.FromMinutes 1.)

    override __.BuildWebHost (hostingEnvironment: IVostokHostingEnvironment) : IWebHost =
        WebHostBuilder()
            .UseKestrel()
            .AddVostokServices()
            .Configure(fun app -> configureApplication app |> ignore)
            .ConfigureServices(fun services ->
                services.AddGiraffe() |> ignore
                
                services.AddDistributedRedisCache(fun options ->
                    options.Configuration   <- config.Redis.Host
                    options.InstanceName    <- config.Redis.Instance) |> ignore)
            .Build()

[<EntryPoint>]
let main _ =
    VostokHostBuilder<TelemetryApplication>()
        .SetServiceInfo("Tax", "TaxCalculator")
        .ConfigureAppConfiguration(fun configurationBuilder ->
            configurationBuilder.AddEnvironmentVariables() |> ignore
            configurationBuilder.AddJsonFile("hostsettings.json") |> ignore)
        .ConfigureHost(fun hostConfigurator ->
            hostConfigurator.SetEnvironment("dev") |> ignore
            hostConfigurator.SetHostLog(createHostLog ()))
        .Build()
        .Run()
    0