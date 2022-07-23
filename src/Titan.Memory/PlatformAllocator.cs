using System.Diagnostics;
using System.Runtime.InteropServices;
using Titan.Memory.Win32;

namespace Titan.Memory;

internal unsafe interface IPlatformAllocator
{
    static abstract void* Allocate(nuint size);
    static abstract void Free(void* ptr);
}

internal unsafe struct PlatformAllocator
{
    private delegate*<nuint, void*> _allocate;
    private delegate*<void*, void> _free;
    public readonly void* Allocate(nuint size) => _allocate(size);
    public readonly void Free(void* ptr) => _free(ptr);


    public static PlatformAllocator Create<T>() where T : unmanaged, IPlatformAllocator =>
        new()
        {
            _allocate = &T.Allocate,
            _free = &T.Free
        };
}


internal unsafe struct Win32VirtualAllocAllocator : IPlatformAllocator
{
    public static readonly nuint PageSize = (nuint)Environment.SystemPageSize;
    public static void* Allocate(nuint size)
    {
        var mem = Kernel32.VirtualAlloc(null, size, AllocationType.MEM_RESERVE | AllocationType.MEM_COMMIT, AllocationProtect.PAGE_READWRITE);
        Debug.Assert(mem != null, $"Failed to allocate {size} bytes of memory.");
        return mem;
    }
    public static void Free(void* ptr)
    {
        var result = Kernel32.VirtualFree(ptr, 0, AllocationType.MEM_RELEASE);
        Debug.Assert(result, "Failed to release memory");
    }
}

internal unsafe struct NativeMemoryAllocator : IPlatformAllocator
{
    public static void* Allocate(nuint size)
    {
        var mem = NativeMemory.Alloc(size);
        Debug.Assert(mem != null, $"Failed to allocate {size} bytes of memory.");
        return mem;
    }
    public static void Free(void* ptr) => NativeMemory.Free(ptr);
}