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
  open RProvider.``base``
  open RProvider.graphics
  open RProvider.grDevices
    
  // disable default R graphics device
  R.graphics_off()

// add R printer
  App.AddFsiPrinter(fun (expr : RDotNet.SymbolicExpression) -> 
    //TODO : Add logic for other types of R objects via RDotNet.SymbolicExpressionExtension
    let png = R.eval(R.parse(text="png"))
    let file = System.IO.Path.GetTempFileName() + ".png"
    let args = namedParams [ "device", box png; "filename", box file ]    
    expr.Print() |> ignore
    try
      R.dev_off(R.dev_copy(args)) |> ignore  
    finally
      R.graphics_off() |> ignore
    Util.Image(file) |> Display
    System.IO.File.Delete(file)
    expr.ToString()
    )

  // add Deedle printer
  App.AddFsiPrinter(fun (printer:Deedle.Internal.IFsiFormattable) -> "\n" + (printer.Format()))

// open global namespaces
open FSharp.Charting

printfn "-- end custom init --"