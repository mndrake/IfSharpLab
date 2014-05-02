module ClipboardHelper

// based on :
// http://dobon.net/vb/dotnet/graphics/getclipboardmetafile.html
// http://social.msdn.microsoft.com/Forums/vstudio/en-US/12a1c749-b320-4ce9-aff7-9de0d7fd30ea/how-to-save-or-serialize-a-metafile-solution-found?forum=csharpgeneral

open System
open System.Drawing
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
open System.Drawing
open System.Drawing.Imaging

let fixedSize(img : Image, format, width : int, height : int) =
    let sourceWidth = img.Width
    let sourceHeight = img.Height
    let sourceX = 0
    let sourceY = 0
    let mutable destX = 0
    let mutable destY = 0

    let mutable nPercent = 0.0
    let nPercentH = 0.0

    let nPercentW = float width / float sourceWidth
    let nPercentH =  float height / float sourceHeight

    if (nPercentH < nPercentW) then
        nPercent <- nPercentH
        destX <- System.Convert.ToInt32((float width - (float sourceWidth * nPercent)) / 2.0)
    else
        nPercent <- nPercentW
        destY <- System.Convert.ToInt32((float height - (float sourceHeight * nPercent)) / 2.0)

    let destWidth = int (float sourceWidth * nPercent)
    let destHeight = int (float sourceHeight * nPercent)

    let bmPhoto = new Bitmap(width, height, PixelFormat.Format24bppRgb)
    bmPhoto.SetResolution(img.HorizontalResolution, img.VerticalResolution)

    let grPhoto = Graphics.FromImage(bmPhoto)
    grPhoto.Clear(Color.White)
    grPhoto.InterpolationMode <- Drawing2D.InterpolationMode.HighQualityBicubic

    grPhoto.DrawImage(img,
        new Rectangle(destX, destY, destWidth, destHeight),
        new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
        GraphicsUnit.Pixel)

    grPhoto.Dispose()
    let ms = new MemoryStream()
    bmPhoto.Save(ms, format)
    ms.ToArray() 

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

let getImage(format, width, height) = 
    let mf = match tryGetEnhMetafileOnClipboard() with
             |Some v -> v
             |None -> failwith "could not get metafile"
    fixedSize(mf, format ,width, height) 

let getPng(width, height) = getImage(ImageFormat.Png, width, height)
let getJpeg(width, height) = getImage(ImageFormat.Jpeg, width, height)
let getBmp(width, height) = getImage(ImageFormat.Bmp, width, height)