#r "paket:
nuget Fake.Core
nuget Fake.IO.FileSystem
nuget Fake.DotNet.Cli
nuget Fake.Core.Target //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.DotNet
open Fake.Core

// Properties
let pwd = Shell.pwd()
let buildDir = pwd  + @"\out\"

// Utils
let publish =
  DotNet.publish (fun opts ->
    {opts with
      Configuration = DotNet.BuildConfiguration.Release
      Runtime       = Some "debian-x64"
      OutputPath    = Some buildDir
    })

// Targets
Target.create "Clean" (fun _ -> Shell.cleanDirs [buildDir])

Target.create "UnitTest" (fun _ ->
   "./test/TaxCalculator/TaxCalculator.BusinessLogic.UnitTests/BusinessLogic.UnitTests.csproj" |> DotNet.test id)

Target.create "PublishApp" (fun _ ->
  for path in !! "./src/**/*.*sproj" do publish path)

Target.create "RunMigrations" (fun _ ->
  Shell.Exec("dotnet ", "/root/out/TaxCalculator.DataLayer.dll") |> ignore)

Target.create "IntegrationalTests" (fun _ ->
   for path in !! "./test/*/IntegrationalTests.csproj" do DotNet.test id path)

// Dependencies
open Fake.Core.TargetOperators
"Clean"
  ==> "UnitTest"
  ==> "PublishApp"
  ==> "RunMigrations"
  ==> "IntegrationalTests"

// start build
Target.runOrDefault "IntegrationalTests"