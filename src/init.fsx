// load base dlls
#I "../"
#r "IfSharp.Kernel.dll"
#r "System.Data.dll"
#r "System.Windows.Forms.DataVisualization.dll"
#r "FSharp.Data.TypeProviders.dll"
#r "FSharp.Charting.dll"
#r "fszmq.dll"

#r "NuGet.dll"

let m = new IfSharp.Kernel.NuGetManager(__SOURCE_DIRECTORY__)

let download package =
    try
        let p =
            package
            |> m.ParseNugetLine
            |> m.DownloadNugetPackage
        printfn "loaded: %s" package
    with
    |ex -> 
        printfn "error loading: %s" package
        printfn "%s" (ex.Message)

download "#N FsLab/0.0.13-beta/pre"

// a 'cheat' to throw away custom printers defined by FsLab
type IFsi = abstract AddPrinter : 'A -> unit
let fsi = { new IFsi with member x.AddPrinter(_) = () }