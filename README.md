# MemoryBuilder

MemoryBuilder is a powerful source generator and attribute-based library designed to simplify external process memory modeling and access in C#.

With just a simple [MemoryTarget("TypeName")] annotation on your struct, MemoryBuilder generates safe, strongly-typed classes that mirror unmanaged memory structures. This is particularly useful for reverse engineering, game memory inspection, or building overlays.

## Features

* 🧠 Attribute-driven memory modeling using [MemoryTarget]

* ⚡ Source generator for generating .g.cs memory access classes at compile-time

* 🧵 Pointer<T> abstraction to safely wrap process memory addresses

* ✅ Supports Nullable, recursive types, and pointer-based memory references

* 🎯 Targets .NET 8.0, and 9.0

* 💻 Windows-only deployment (x64)

## Usage Example
```C#
[MemoryTarget("Player")]
public struct PlayerTemplate
{
    public int Health;
    public Pointer<InventoryTemplate> Inventory;
}
```
```C#
var player = Player.TryBuildFromMemory(processHandle, basePointer);
if (player != null)
{
    Console.WriteLine(player.Health);
}
```

## License

Licensed under the [MIT License](LICENSE).
© 2025 EMROF-H
