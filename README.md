# Silk.NET F# Examples

Updated to work with Silk.NET 1.0.1 and .NET Core 3.1.

The new OpenGL library from Ultz, [Silk.NET](https://github.com/Ultz/Silk.NET)
brings together bindings for OpenGL and Vulkan (Graphics), OpenAL (Audio), Input and 
Windowing (GLFW3) into a single, high speed library.

Some examples are provided in the project that target C#. Here I've converted them to F#
to test feasibility in this language.

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

This project tests various input devices. Keyboard, mouse, gamepads and joysticks
are all covered. Displays all input in the console.

```
cd input_test
dotnet run
```

## GLFW3

`lib/glfw3.dll` is provided by (https://www.glfw.org/) and is the 64-bit version.
If running on 32-bit machines this DLL will need to be replaced with the 32-bit version.
