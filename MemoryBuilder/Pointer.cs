using System.Runtime.InteropServices;

namespace MemoryBuilder;

/// <summary>
/// Represents an opaque memory address (pointer), typically used for accessing memory in an external process.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly partial struct Pointer(UIntPtr address) : IEquatable<Pointer>, IComparable<Pointer>
{
    public Pointer(IntPtr address) : this((UIntPtr)address) { }

    public static Pointer InvalidPointer => new(UIntPtr.Zero);

    public readonly UIntPtr address = address;

    public bool IsNull => address == UIntPtr.Zero;
    public bool IsNotNull => address != UIntPtr.Zero;

    public static explicit operator Pointer(UIntPtr address) => new(address);
    public static explicit operator Pointer(IntPtr address) => new((UIntPtr)address);

    public static bool operator <(Pointer left, Pointer right) => left.address < right.address;
    public static bool operator >(Pointer left, Pointer right) => left.address > right.address;
    public static bool operator <=(Pointer left, Pointer right) => left.address <= right.address;
    public static bool operator >=(Pointer left, Pointer right) => left.address >= right.address;
    public static bool operator ==(Pointer left, Pointer right) => left.address == right.address;
    public static bool operator !=(Pointer left, Pointer right) => left.address != right.address;
    public int CompareTo(Pointer other) => address.CompareTo(other.address);
    public bool Equals(Pointer other) => address == other.address;
    public override bool Equals(object? obj) => obj is Pointer other && Equals(other);
    public override int GetHashCode() => address.GetHashCode();

    public override string ToString() => $"0x{address:X}";

    public Pointer Offset(short offset) => new(address + (UIntPtr)offset);
    public Pointer Offset(ushort offset) => new(address + offset);
    public Pointer Offset(int offset) => new(address + (UIntPtr)offset);
    public Pointer Offset(uint offset) => new(address + offset);
    public Pointer Offset(long offset) => new(address + (UIntPtr)offset);
    public Pointer Offset(ulong offset) => new(address + (UIntPtr)offset);
    public Pointer Offset(IntPtr offset) => new(address + (UIntPtr)offset);
    public Pointer Offset(UIntPtr offset) => new(address + offset);
    public static Pointer operator +(Pointer pointer, short offset) => pointer.Offset(offset);
    public static Pointer operator -(Pointer pointer, short offset) => pointer.Offset((short) -offset);
    public static Pointer operator +(Pointer pointer, ushort offset) => pointer.Offset(offset);
    public static Pointer operator -(Pointer pointer, ushort offset) => pointer.Offset((ushort)(0u - offset));
    public static Pointer operator +(Pointer pointer, int offset) => pointer.Offset(offset);
    public static Pointer operator -(Pointer pointer, int offset) => pointer.Offset(-offset);
    public static Pointer operator +(Pointer pointer, uint offset) => pointer.Offset(offset);
    public static Pointer operator -(Pointer pointer, uint offset) => pointer.Offset(0u - offset);
    public static Pointer operator +(Pointer pointer, long offset) => pointer.Offset(offset);
    public static Pointer operator -(Pointer pointer, long offset) => pointer.Offset(-offset);
    public static Pointer operator +(Pointer pointer, ulong offset) => pointer.Offset(offset);
    public static Pointer operator -(Pointer pointer, ulong offset) => pointer.Offset(0ul - offset);
    public static Pointer operator +(Pointer pointer, IntPtr offset) => pointer.Offset(offset);
    public static Pointer operator -(Pointer pointer, IntPtr offset) => pointer.Offset(-offset);
    public static Pointer operator +(Pointer pointer, UIntPtr offset) => pointer.Offset(offset);
    public static Pointer operator -(Pointer pointer, UIntPtr offset) => new(pointer.address - offset);

    public static IntPtr operator -(Pointer left, Pointer right) => (IntPtr)(left.address - right.address);

    public IntPtr ToIntPtr() => (IntPtr)address;
    public UIntPtr ToUIntPtr() => address;
}

public readonly partial struct Pointer
{
    /// <summary>
    /// Reads a value of unmanaged type <typeparamref name="T"/> from the specified process memory.
    /// </summary>
    /// <typeparam name="T">An unmanaged value type</typeparam>
    /// <param name="process">Handle to the target process</param>
    /// <param name="value">The variable to store the read value</param>
    /// <returns><c>true</c> if the read operation succeeds; otherwise, <c>false</c></returns>
    public unsafe partial bool TryRead<T>(Handle process, out T value) where T : unmanaged;

    /// <summary>
    /// Reads a value of unmanaged type <typeparamref name="T"/> from the specified process memory.
    /// Throws <see cref="InvalidOperationException"/> if the operation fails.
    /// </summary>
    /// <typeparam name="T">An unmanaged value type</typeparam>
    /// <param name="process">Handle to the target process</param>
    /// <returns>The value read from the target process</returns>
    /// <exception cref="InvalidOperationException">Thrown if the read fails</exception>
    public unsafe partial T Read<T>(Handle process) where T : unmanaged;

    /// <summary>
    /// Reads an array of unmanaged values from the specified process into the provided buffer.
    /// </summary>
    /// <typeparam name="T">An unmanaged value type</typeparam>
    /// <param name="process">Handle to the target process</param>
    /// <param name="buffer">The target buffer to store the read data; must not be empty</param>
    /// <returns><c>true</c> if the read operation succeeds; otherwise, <c>false</c></returns>
    public unsafe partial bool TryReadArray<T>(Handle process, T[] buffer) where T : unmanaged;

    /// <summary>
    /// Reads an array of unmanaged values from the specified process.
    /// </summary>
    /// <typeparam name="T">An unmanaged value type</typeparam>
    /// <param name="process">Handle to the target process</param>
    /// <param name="count">The number of elements to read; must be positive</param>
    /// <returns>A new array containing the values read</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is not positive</exception>
    /// <exception cref="InvalidOperationException">Thrown if the read fails</exception>
    public partial T[] ReadArray<T>(Handle process, int count) where T : unmanaged;

    /// <summary>
    /// Reads a <see cref="Pointer"/> value from the specified process memory address, with an optional byte offset.
    /// </summary>
    /// <param name="process">Handle to the target process.</param>
    /// <param name="offset">Optional byte offset to apply to this pointer before reading.</param>
    /// <returns>The <see cref="Pointer"/> value read from the computed address in the target process.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the read operation fails.
    /// </exception>
    public partial Pointer ReadPointer(Handle process, int offset = 0);

    /// <summary>
    /// Converts this pointer to a delegate of the specified type, treating the pointer as the address of a native function.
    /// </summary>
    /// <typeparam name="TFunc">The delegate type that matches the target native function signature.</typeparam>
    /// <returns>A delegate of type <typeparamref name="TFunc"/> bound to the native function address represented by this pointer.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the conversion fails due to an invalid or null pointer.
    /// </exception>
    public partial TFunc ToFunction<TFunc>() where TFunc : Delegate;
}

public readonly partial struct Pointer
{
    public unsafe partial bool TryRead<T>(Handle process, out T value) where T : unmanaged =>
        process.ReadProcessMemory(this, out value);

    public unsafe partial T Read<T>(Handle process) where T : unmanaged
    {
        if (!TryRead(process, out T v))
        {
            throw new InvalidOperationException($"ReadProcessMemory failed at address {this} from {process}.");
        }
        return v;
    }

    public unsafe partial bool TryReadArray<T>(Handle process, T[] buffer) where T : unmanaged
    {
        if (buffer.Length == 0)
        {
            return false;
        }

        fixed (T* ptr = buffer)
        {
            return process.ReadProcessMemory(this, ptr, sizeof(T) * buffer.Length);
        }
    }

    public partial T[] ReadArray<T>(Handle process, int count) where T : unmanaged
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        var buffer = new T[count];
        if (!TryReadArray(process, buffer))
        {
            throw new InvalidOperationException($"ReadProcessMemory failed at address {address} for {count} elements from {process}.");
        }
        return buffer;
    }

    public partial Pointer ReadPointer(Handle process, int offset) => (this + offset).Read<Pointer>(process);

    public partial TFunc ToFunction<TFunc>() where TFunc : Delegate
    {
        if (IsNull)
        {
            throw new InvalidOperationException($"Pointer is null and can not get function {typeof(TFunc)}.");
        }
        return Marshal.GetDelegateForFunctionPointer<TFunc>((IntPtr)address);
    }
}
