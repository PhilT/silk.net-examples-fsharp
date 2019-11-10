# Silk.NET F# Examples

The new upcoming OpenGL library from Ultz, [Silk.NET](https://github.com/Ultz/Silk.NET)
promises to bring together bindings for OpenGL (Graphics), OpenAL (Audio), Input and 
Windowing (GLFW3) into a single, high speed library.

Some examples are provided in the project that target C#. Here I've converted them to F#
to test feasibility in this language.

### NOTE: Direct references to Silk.NET.Windowing and Silk.NET.OpenGL

Currently the project references the csproj files from Silk.NET directly as
the current release version (preview 2) has some issues with the current examples
in F#. preview 3 is due out in about a week so I'll update to Paket references
after that date.

## Blank Window

Provides a basic blank window with no other graphics. It tests that we can hook into
the available window events and doesn't rely on winforms so should be cross platform
however, I've only tested on Windows thus far.

```
cd blank_window
dotnet run
```

## Triangle

Builds on the previous example, drawing a triangle in the middle of the window.
This tests that we can interface with OpenGL through the Silk.NET bindings.

```
cd triangle
dotnet run
```

## InputTest

[to implement]

## GLFW3

`lib/glfw3.dll` is provided by (https://www.glfw.org/) and is the 64-bit version.
If running on 32-bit machines this DLL will need to be replaced with the 32-bit version.
