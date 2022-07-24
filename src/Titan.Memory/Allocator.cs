using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Memory.Win32;

namespace Titan.Memory;

internal unsafe interface IAllocator<TArguments> where TArguments : unmanaged
{
    static abstract void* CreateContext(in TArguments args);
    static abstract void ReleaseContext(void* context);
    static abstract void* Allocate(void* context, nuint size);
    static abstract void Free(void* context, void* ptr);
}
internal unsafe interface IAllocator
{
    static abstract void* Allocate(void* context, nuint size);
    static abstract void Free(void* context, void* ptr);
}

internal unsafe struct Allocator
{
    private void* _context;
    private delegate*<void*, nuint, void*> _allocate;
    private delegate*<void*, void*, void> _free;
    private delegate*<void*, void> _release;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void* Allocate(nuint size) => _allocate(_context, size);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Free(void* ptr) => _free(_context, ptr);
    public void Release()
    {
        if (_release != null)
        {
            _release(_context);
        }
    }
    /// <summary>
    /// Create an allocator without a context
    /// </summary>
    /// <typeparam name="T">The allocator type</typeparam>
    /// <returns>The allocator</returns>
    public static Allocator Create<T>() where T : unmanaged, IAllocator =>
        new()
        {
            _allocate = &T.Allocate,
            _free = &T.Free,
            _context = null,
            _release = null
        };
    /// <summary>
    /// Create an allocator with a context and arguments
    /// </summary>
    /// <typeparam name="TAllocator">The allocator type</typeparam>
    /// <typeparam name="TArguments">The argument type for the allocator</typeparam>
    /// <param name="args">The arguments</param>
    /// <returns>The allocator</returns>
    public static Allocator Create<TAllocator, TArguments>(in TArguments args)
        where TAllocator : unmanaged, IAllocator<TArguments>
        where TArguments : unmanaged =>
        new()
        {
            _context = TAllocator.CreateContext(args),
            _release = &TAllocator.ReleaseContext,
            _allocate = &TAllocator.Allocate,
            _free = &TAllocator.Free
        };
}

struct Win32PoolArgs
{
    public nuint Size;
}

internal unsafe struct Win32VirtualAllocFixedSizeAllocator : IAllocator<Win32PoolArgs>
{
    private byte* _memory;
    private nuint _size;
    private volatile int _offset;
    public static void* CreateContext(in Win32PoolArgs args)
    {
        var size = args.Size;
        var contextSize = (nuint)sizeof(Win32VirtualAllocAllocator);
        var mem = (byte*)Kernel32.VirtualAlloc(null, size + contextSize, AllocationType.MEM_COMMIT | AllocationType.MEM_RESERVE, AllocationProtect.PAGE_READWRITE);
        Debug.Assert(mem != null, "Failed to allocate memory");

        //TODO: Consider putting the context at the top.
        var context = (Win32VirtualAllocFixedSizeAllocator*)(mem + size);
        context->_memory = mem;
        context->_offset = 0;
        context->_size = size;

        return context;
    }

    public static void ReleaseContext(void* context)
    {
        var memoryContext = (Win32VirtualAllocFixedSizeAllocator*)context;
        Kernel32.VirtualFree(memoryContext->_memory, 0, AllocationType.MEM_RELEASE);
    }

    public static void* Allocate(void* context, nuint size)
    {
        var memoryContext = (Win32VirtualAllocFixedSizeAllocator*)context;
        var offset = Interlocked.Add(ref memoryContext->_offset, (int)size);
        Debug.Assert(offset < (int)memoryContext->_size, "Out of memory");
        return memoryContext->_memory + offset - size;
    }

    public static void Free(void* context, void* ptr)
    {
        // noop, can't free memory in the fixed context (can be solved with a FreeList)
    }
}

internal unsafe struct Win32VirtualAllocAllocator : IAllocator
{
    public static readonly nuint PageSize = (nuint)Environment.SystemPageSize;
    public static void* Allocate(void* context, nuint size)
    {
        var mem = Kernel32.VirtualAlloc(null, size, AllocationType.MEM_RESERVE | AllocationType.MEM_COMMIT, AllocationProtect.PAGE_READWRITE);
        Debug.Assert(mem != null, $"Failed to allocate {size} bytes of memory.");
        return mem;
    }
    public static void Free(void* context, void* ptr)
    {
        var result = Kernel32.VirtualFree(ptr, 0, AllocationType.MEM_RELEASE);
        Debug.Assert(result, "Failed to release memory");
    }

    public static void* CreateContext() => null;
    public static void ReleaseContext(void* context) { }
}

internal unsafe struct NativeMemoryAllocator : IAllocator
{
    public static void* Allocate(void* context, nuint size)
    {
        var mem = NativeMemory.Alloc(size);
        Debug.Assert(mem != null, $"Failed to allocate {size} bytes of memory.");
        return mem;
    }
    public static void Free(void* context, void* ptr) => NativeMemory.Free(ptr);
    public static void* CreateContext() => null;
    public static void ReleaseContext(void* context) { }
}