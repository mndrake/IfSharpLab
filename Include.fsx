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
  open RProvider.grDevices
    
  // disable default R graphics device
  R.graphics_off()

  // add Deedle printer
  App.AddFsiPrinter(fun (printer:Deedle.Internal.IFsiFormattable) -> "\n" + (printer.Format()))

  // add R printer
  App.AddFsiPrinter(fun (symexpr:RDotNet.SymbolicExpression) ->
    let png = R.eval(R.parse(text="png"))
    let file = System.IO.Path.GetTempFileName() + ".png"
    let args = namedParams [ "device", box png; "filename", box file ]
    symexpr |> ignore
    R.dev_off(R.dev_copy(args)) |> ignore
    R.graphics_off() |> ignore
    let img = Util.Image (file)
    System.IO.File.Delete(file)
    img |> Display
    symexpr.Print()
  )


// open global namespaces
open FSharp.Charting

printfn "-- end custom init --"