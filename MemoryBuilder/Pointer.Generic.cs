namespace MemoryBuilder;

/// <summary>
/// Represents a typed pointer to a value of type <typeparamref name="T"/> in the memory of an external process.
/// </summary>
/// <typeparam name="T">An unmanaged type that this pointer refers to.</typeparam>
public readonly partial struct Pointer<T>(Pointer pointer) where T : unmanaged
{
    private readonly Pointer pointer = pointer;

    public bool IsNull => pointer.IsNull;
    public bool IsNotNull => pointer.IsNotNull;

    public static explicit operator Pointer<T>(Pointer pointer) => new(pointer);
    public static implicit operator Pointer(Pointer<T> pointer) => pointer.pointer;

    public override string ToString() => pointer.ToString();

    public IntPtr ToIntPtr() => pointer.ToIntPtr();
    public UIntPtr ToUIntPtr() => pointer.ToUIntPtr();
}

public readonly partial struct Pointer<T>
{
    /// <summary>
    /// Attempts to read the value of type <typeparamref name="T"/> from the target process memory.
    /// </summary>
    /// <param name="process">The handle to the target process.</param>
    /// <param name="value">The variable to store the read value.</param>
    /// <returns><c>true</c> if the read operation succeeds; otherwise, <c>false</c>.</returns>
    public bool TryRead(Handle process, out T value) => this.pointer.TryRead(process, out value);

    /// <summary>
    /// Reads the value of type <typeparamref name="T"/> from the target process memory.
    /// Throws an <see cref="InvalidOperationException"/> if the read fails.
    /// </summary>
    /// <param name="process">The handle to the target process.</param>
    /// <returns>The value read from the target process memory.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the read operation fails.</exception>
    public T Read(Handle process) => pointer.Read<T>(process);

    /// <summary>
    /// Reads a pointer value from the memory location and returns it as a <see cref="Pointer{T}"/>.
    /// </summary>
    /// <param name="process">The handle to the target process.</param>
    /// <param name="offset">An optional byte offset added before reading the pointer.</param>
    /// <returns>The <see cref="Pointer"/> value read from the computed address in the target process.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the read operation fails.</exception>
    public Pointer ReadPointer(Handle process, int offset = 0) => pointer.ReadPointer(process, offset);
}
