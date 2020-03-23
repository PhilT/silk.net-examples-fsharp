#nowarn "9"

open Silk.NET.Windowing
open Silk.NET.Windowing.Common
open System.Threading
open Silk.NET.OpenGL
open Microsoft.FSharp.NativeInterop
open System
open System.IO
open System.Drawing

let compileShader (shader: uint32) (gl: GL) =
  gl.CompileShader(shader)
  let code = gl.GetShader(shader, GLEnum.CompileStatus)
  if code <> int GLEnum.True then failwith (sprintf "Error compiling shader %d" shader)

let linkProgram (program: uint32) (gl: GL) =
  gl.LinkProgram(program)
  let code = gl.GetProgram(program, GLEnum.LinkStatus)
  if code <> int GLEnum.True then failwith (sprintf "Error linking program %d" program)

let createShader vertPath fragPath (gl: GL) =
  let vertexShaderSource = File.ReadAllText vertPath
  let fragmentShaderSource = File.ReadAllText fragPath
  let vertexShader = gl.CreateShader(GLEnum.VertexShader)
  let fragmentShader = gl.CreateShader(GLEnum.FragmentShader)
  gl.ShaderSource(vertexShader, vertexShaderSource)
  gl.ShaderSource(fragmentShader, fragmentShaderSource)
  compileShader vertexShader gl
  compileShader fragmentShader gl

  let handle = gl.CreateProgram()
  gl.AttachShader(handle, vertexShader)
  gl.AttachShader(handle, fragmentShader)

  linkProgram handle gl

  gl.DetachShader(handle, vertexShader)
  gl.DetachShader(handle, fragmentShader)
  gl.DeleteShader(vertexShader)
  gl.DeleteShader(fragmentShader)

  handle

let main =
  let vertices = [|
    -0.5f; -0.5f; 0.0f // Bottom-left vertex
    0.5f; -0.5f; 0.0f  // Bottom-right vertex
    0.0f;  0.5f; 0.0f  // Top vertex
  |]

  let window = Window.Create(WindowOptions.Default)
  window.Title <- "A Triangle"

  window.add_Load (fun () -> 
    let gl = GL.GetApi()

    let vertexBufferObject = gl.GenBuffer()
    let vertexArrayObject = gl.GenVertexArray()
    let shaderHandle = createShader "shader.vert" "shader.frag" gl

    gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f)
    gl.BindBuffer(GLEnum.ArrayBuffer, vertexBufferObject)
    use floatPtr = fixed vertices
    let voidPtr = floatPtr |> NativePtr.toVoidPtr
    let size = uint32 (vertices.Length * sizeof<float32>)
    gl.BufferData(GLEnum.ArrayBuffer, size, voidPtr, GLEnum.StaticDraw)

    gl.UseProgram(shaderHandle)
    gl.BindVertexArray(vertexArrayObject)
    gl.VertexAttribPointer(0u, 3, GLEnum.Float, false, uint32 (3 * sizeof<float32>), IntPtr.Zero.ToPointer())
    gl.EnableVertexAttribArray(0u)
    gl.BindBuffer(GLEnum.ArrayBuffer, vertexBufferObject)

    window.add_Render(fun _ -> 
      gl.Clear(uint32 GLEnum.ColorBufferBit)
      gl.UseProgram(shaderHandle)
      gl.BindVertexArray(vertexArrayObject)
      gl.DrawArrays(GLEnum.Triangles, 0, 3u)
    )

    window.add_Resize(fun size ->
      gl.Viewport(size: Size)
    )

    window.add_Closing(fun () -> 
      gl.BindBuffer(GLEnum.ArrayBuffer, 0u)
      gl.BindVertexArray(0u)
      gl.UseProgram(0u)
      gl.DeleteBuffer(vertexBufferObject)
      gl.DeleteVertexArray(vertexArrayObject)
      gl.DeleteProgram(shaderHandle)
    )
  )

  window.Run()
  0
