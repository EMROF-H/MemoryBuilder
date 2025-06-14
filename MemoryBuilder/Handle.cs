namespace MemoryBuilder;

public readonly struct Handle(IntPtr value) : IEquatable<Handle>
{
    public static Handle InvalidHandle => new(IntPtr.Zero);

    private readonly IntPtr value = value;
    
    public static explicit operator IntPtr(Handle handle) => handle.value;
    public static explicit operator Handle(IntPtr handle) => new(handle);
    
    public bool IsValid => value != IntPtr.Zero;
    
    public static bool operator ==(Handle left, Handle right) => left.value == right.value;
    public static bool operator !=(Handle left, Handle right) => left.value != right.value;
    public bool Equals(Handle other) => value == other.value;
    public override bool Equals(object? obj) => obj is Handle other && Equals(other);
    public override int GetHashCode() => value.GetHashCode();
}
