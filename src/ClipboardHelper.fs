module ClipboardHelper

// based on :
// http://dobon.net/vb/dotnet/graphics/getclipboardmetafile.html
// http://social.msdn.microsoft.com/Forums/vstudio/en-US/12a1c749-b320-4ce9-aff7-9de0d7fd30ea/how-to-save-or-serialize-a-metafile-solution-found?forum=csharpgeneral

open System
open System.Runtime.InteropServices

[<Literal>] 
let private CF_ENHMETAFILE = 14
[<DllImport("user32.dll")>] 
extern bool private OpenClipboard(IntPtr HWndNewOwner)
[<DllImport("user32.dll")>] 
extern int private IsClipboardFormatAvailable(int Wformat)
[<DllImport("user32.dll")>] 
extern IntPtr private GetClipboardData(int Wformat)
[<DllImport("user32.dll")>] 
extern int private CloseClipboard()
[<DllImport("user32")>] 
extern IntPtr private GetDesktopWindow()
[<DllImport("gdi32")>]
extern int private GetEnhMetaFileBits(int hemf, int cbBuffer, byte[] lpbBuffer)

open System.IO
open System.Drawing.Imaging

/// gets the data in the metafile format of clipboard
let tryGetEnhMetafileOnClipboard() =
    let hWnd = GetDesktopWindow()
    let mutable meta : Metafile option = None
    if OpenClipboard(hWnd) then
        try
            if IsClipboardFormatAvailable(CF_ENHMETAFILE) <> 0 then
                let hmeta = GetClipboardData(CF_ENHMETAFILE)
                meta <- Some <| new Metafile(hmeta, true)
        finally
            CloseClipboard() |> ignore
    meta
    
let getImageBytes(format) =
    let mf = match tryGetEnhMetafileOnClipboard() with
             |Some v -> v
             |None -> failwith "could not get metafile"
    let ms = new MemoryStream()
    mf.Save(ms, format)
    ms.ToArray()



let getJpeg() = getImageBytes(ImageFormat.Jpeg)
let getPng() = getImageBytes(ImageFormat.Png)
let getBmp() = getImageBytes(ImageFormat.MemoryBmp)




//let getMetaFileBytesFromClipboard() =
//    let mf = 
//        match tryGetEnhMetafileOnClipboard() with
//        | Some v -> v
//        | None -> failwith "could not retrieve image from clipboard"
//    let enhMetafileHandle = mf.GetHenhmetafile().ToInt32()
//    let bufferSize = GetEnhMetaFileBits(enhMetafileHandle, 0, null)
//    let buffer : byte[] = Array.zeroCreate bufferSize
//    if GetEnhMetaFileBits(enhMetafileHandle, bufferSize, buffer) <= 0 then
//        failwith "getMetaFile"
//    let ms = new MemoryStream()
//    ms.Write(buffer, 0, bufferSize)
//    ms.ToArray()