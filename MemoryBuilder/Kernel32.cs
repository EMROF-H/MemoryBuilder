using System.Runtime.InteropServices;

namespace MemoryBuilder;

internal static class Kernel32
{
    public const string DllName = "kernel32.dll";
    
    [DllImport(DllName)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern unsafe bool ReadProcessMemory(this Handle hProcess, Pointer lpBaseAddress,
        void* lpBuffer, int dwSize, IntPtr *lpNumberOfBytesRead = null);

    public static unsafe bool ReadProcessMemory<T>(this Handle hProcess, Pointer lpBaseAddress, out T value) where T : unmanaged
    {
        fixed (T* ptr = &value)
        {
            return ReadProcessMemory(hProcess, lpBaseAddress, ptr, sizeof(T));
        }
    }
}
