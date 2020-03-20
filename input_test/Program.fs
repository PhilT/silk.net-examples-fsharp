open System
open System.Drawing
open System.Linq
open System.Threading.Tasks
open Silk.NET.Input
open Silk.NET.Input.Common
open Silk.NET.Windowing
open Silk.NET.Windowing.Common

let main =
  let mutable options = WindowOptions.Default
  options.UseSingleThreadedWindow <- false
  options.UpdatesPerSecond <- 60.0
  options.FramesPerSecond <- 60.0

  let mutable window = Window.Create(options)

  let indent = "\n             "

  let inputGamepadOnButtonDown = Action<IGamepad, Button>(fun g b ->
    printfn "G%d> %d down. %A" g.Index b.Index b.Name
  )

  let inputGamepadOnButtonUp = Action<IGamepad, Button>(fun g b ->
    printfn "G%d> %d up. %A" g.Index b.Index b.Name
  )

  let gamepadOnTriggerMoved = Action<IGamepad, Trigger>(fun g t ->
    printfn "G%d> %d trigger moved: %A" g.Index t.Index t.Position
  )

  let gamepadOnThumbstickMoved = Action<IGamepad, Thumbstick>(fun g t ->
    printfn "G%d> %d thumbstick moved: %f, %f" g.Index t.Index t.X t.Y
  )

  let joystickOnHatMoved = Action<IJoystick, Hat>(fun j h ->
    printfn "J%d> %d hat moved: %A" j.Index h.Index h.Position
  )

  let joystickOnAxisMoved = Action<IJoystick, Axis>(fun j a ->
    printfn "J%d> %d axis moved: %A" j.Index a.Index a.Position
  )

  let joystickOnButtonUp = Action<IJoystick, Button>(fun j b ->
    printfn "J%d> %A up." j.Index b.Name
  )

  let joystickOnButtonDown = Action<IJoystick, Button>(fun j b ->
    printfn "J%d> %A down." j.Index b.Name
  )

  let keyboardOnKeyDown = Action<IKeyboard, Key, _>(fun k key _ ->
    printfn "K%d> %A down." k.Index key
  )

  let keyboardOnKeyUp = Action<IKeyboard, Key, _>(fun k key _ ->
    printfn "K%d> %A up." k.Index key
  )

  let keyboardOnKeyChar = Action<IKeyboard, Char>(fun k c ->
    printfn "K%d> %A received." k.Index c
  )

  let mouseOnMouseMove = Action<IMouse, PointF>(fun m p ->
    printfn "M%d> Moved: %A" m.Index p
  )

  let mouseOnScroll = Action<IMouse, ScrollWheel>(fun m s ->
    printfn "M%d> Scrolled: %A" m.Index s
  )

  let mouseOnMouseDown = Action<IMouse, MouseButton>(fun m b ->
    printfn "M%d> %A down." m.Index b
  )

  let mouseOnMouseUp = Action<IMouse, MouseButton>(fun m b ->
    printfn "M%d> %A up." m.Index b
  )

  let printButtonList (buttons: List<Button>) =
    printf "    Buttons: "

    buttons
    |> Seq.map (fun button -> sprintf "%A(%d)" button.Name (if button.Pressed then 1 else 0))
    |> String.concat indent
    |> printfn "%s"

  let connectGamepad (gamepad: IGamepad) isConnected =
    printfn "Discovered controller %d (Connected: %b)" gamepad.Index isConnected
    if isConnected then
      gamepad.add_ButtonDown(inputGamepadOnButtonDown)
      gamepad.add_ButtonUp(inputGamepadOnButtonUp)
      gamepad.add_ThumbstickMoved(gamepadOnThumbstickMoved)
      gamepad.add_TriggerMoved(gamepadOnTriggerMoved)
    else
      gamepad.remove_ButtonDown(inputGamepadOnButtonDown)
      gamepad.remove_ButtonUp(inputGamepadOnButtonUp)
      gamepad.remove_ThumbstickMoved(gamepadOnThumbstickMoved)
      gamepad.remove_TriggerMoved(gamepadOnTriggerMoved)

    printButtonList (gamepad.Buttons |> Seq.toList)

    printfn "    %d thumbsticks found." gamepad.Thumbsticks.Count
    printfn "    %d triggers found." gamepad.Triggers.Count

  let connectJoystick (joystick: IJoystick) isConnected =
    printfn "Discovered joystick %d (Connected: %b)" joystick.Index isConnected
    if isConnected then
      joystick.add_ButtonDown(joystickOnButtonDown)
      joystick.add_ButtonUp(joystickOnButtonUp)
      joystick.add_AxisMoved(joystickOnAxisMoved)
      joystick.add_HatMoved(joystickOnHatMoved)

    printButtonList (joystick.Buttons |> Seq.toList)

  let connectKeyboard (keyboard: IKeyboard) isConnected =
    printfn "Discovered keyboard %d (Connected: %b)" keyboard.Index isConnected
    if isConnected then
      keyboard.add_KeyDown(keyboardOnKeyDown)
      keyboard.add_KeyUp(keyboardOnKeyUp)
      keyboard.add_KeyChar(keyboardOnKeyChar)
    else
      keyboard.remove_KeyDown(keyboardOnKeyDown)
      keyboard.remove_KeyUp(keyboardOnKeyUp)
      keyboard.remove_KeyChar(keyboardOnKeyChar)

    printf "    Buttons: "
    keyboard.SupportedKeys
    |> Seq.map (fun key -> key.ToString())
    |> String.concat ", "
    |> printfn "%s"

  let connectMouse (mouse: IMouse) isConnected =
    printfn "Discovered mouse %d (Connected: %b)" mouse.Index isConnected
    if isConnected then
      mouse.add_MouseUp(mouseOnMouseUp)
      mouse.add_MouseDown(mouseOnMouseDown)
      mouse.add_Scroll(mouseOnScroll)
      mouse.add_MouseMove(mouseOnMouseMove)
    else
      mouse.remove_MouseUp(mouseOnMouseUp)
      mouse.remove_MouseDown(mouseOnMouseDown)
      mouse.remove_Scroll(mouseOnScroll)
      mouse.remove_MouseMove(mouseOnMouseMove)

    printf "    Buttons: "
    mouse.SupportedButtons
    |> Seq.map (fun button -> button.ToString())
    |> String.concat ", "
    |> printfn "%s"

    printfn "    %d scroll wheels." mouse.ScrollWheels.Count

  let doConnect (device: IInputDevice) isConnected =
    let status =
      if isConnected
      then "connected"
      else "disconnected"
    printfn "Device %s %s" device.Name status

    match device with
    | :? IGamepad as gamepad -> connectGamepad gamepad isConnected
    | :? IJoystick as joystick -> connectJoystick joystick isConnected
    | :? IKeyboard as keyboard -> connectKeyboard keyboard isConnected
    | :? IMouse as mouse -> connectMouse mouse isConnected
    | _ -> ()

  let load () =
    let input = window.CreateInput()
    input.add_ConnectionChanged (Action<IInputDevice, bool> doConnect)
    printfn "Now, go press buttons in the window and you'll see the feedback here."

    input.Gamepads
    |> Seq.filter (fun device -> device.IsConnected)
    |> Seq.iter (fun device -> doConnect device device.IsConnected)

    input.Joysticks
    |> Seq.filter (fun device -> device.IsConnected)
    |> Seq.iter (fun device -> doConnect device device.IsConnected)

    input.Keyboards
    |> Seq.filter (fun device -> device.IsConnected)
    |> Seq.iter (fun device -> doConnect device device.IsConnected)

    input.Mice
    |> Seq.filter (fun device -> device.IsConnected)
    |> Seq.iter (fun device -> doConnect device device.IsConnected)


  window.add_Load(fun () -> load ())
  window.Run()

  0
