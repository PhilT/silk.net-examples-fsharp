open System

open System.Threading
open Silk.NET.Windowing
open Silk.NET.Windowing.Common

let main =
  let mutable options = WindowOptions.Default
  options.UseSingleThreadedWindow <- true
  options.UpdatesPerSecond <- 60.0
  options.FramesPerSecond <- 60.0

  let mutable window = Window.Create(options)

  window.add_FileDrop (fun files -> files |> Seq.iter (fun file -> printfn "%s" file))
  window.add_Move (fun position -> printfn "Moving %A" position)
  window.add_Resize (fun size -> printfn "Resize %A" size)
  window.add_StateChanged (fun state -> printfn "%A" state)
  window.add_Load (fun () -> printfn "Finished loading")
  window.add_Closing (fun () -> printfn "Window is closing now")
  window.add_FocusChanged (fun isFocused -> printfn "Focused = %b" isFocused)
  window.add_Render (fun delta -> printfn "Render %A" (1.0 / delta))
  window.add_Update (fun delta -> printfn "Update %A" (1.0 / delta))

  window.VSync <- VSyncMode.Off

  printfn "Entry thread is %A" Thread.CurrentThread.ManagedThreadId
  window.Run()

  0
