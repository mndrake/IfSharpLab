// include directory, this will be replaced by the kernel
#I "{0}"
// load base dlls
#r "IfSharp.Kernel.dll"
#r "System.Data.dll"
#r "System.Windows.Forms.DataVisualization.dll"
#r "FSharp.Data.TypeProviders.dll"
#r "FSharp.Charting.dll"
#r "fszmq.dll"

// open the global functions and methods
open FSharp.Charting
open IfSharp.Kernel
open IfSharp.Kernel.Globals

// custom init

printfn "-- start custom init --"

// install and load FsLab libraries
#load "src/init.fsx"
open Init
#load "src/packages/FsLab.0.0.13-beta/FsLab.fsx"
#load "src/DeedleFormat.fs"

module FsiAutoShow =
  open RProvider
  App.AddFsiPrinter(fun (printer:Deedle.Internal.IFsiFormattable) -> "\n" + (printer.Format()))
  App.AddFsiPrinter(fun (synexpr:RDotNet.SymbolicExpression) -> synexpr.Print())

// open global namespaces
open FSharp.Charting

printfn "-- end custom init --"