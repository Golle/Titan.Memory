using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Titan.Memory;
using Titan.Memory.Allocators;
using Titan.Memory.Posix;
using Titan.Memory.Win32;


Console.WriteLine($"Hello world, {typeof(Program).Assembly.GetName().Name}");


unsafe
{
    var type = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? typeof(Win32PlatformAllocator) : typeof(PosixPlatformAllocator);
    var allocator = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? PlatformAllocator.Create<Win32PlatformAllocator>() : PlatformAllocator.Create<PosixPlatformAllocator>();
    Console.WriteLine($"Using {type} for virtual memory allocations");
    var sizeToReserve = 1024 * 1024 * 1024;

    if (!GeneralAllocator.Create(&allocator, (nuint)sizeToReserve, 0, out var genAllocator))
    {
        Console.WriteLine($"Failed to create the {nameof(GeneralAllocator)}");
        return;
    }

    var mems = stackalloc void*[100];
    var count = 0;
    //var a =  genAllocator.Allocate(120);
    //var b = mems[count++] = genAllocator.Allocate(160);
    //var c = genAllocator.Allocate(200);
    ////genAllocator.Free(b);
    //genAllocator.Free(c);
    mems[count++] = genAllocator.Allocate(150);
    //genAllocator.Free(a);
    genAllocator.Free(genAllocator.Allocate(180));
    mems[count++] = genAllocator.Allocate(2010);
    //mems[count++] = genAllocator.Allocate(200);
    var tmpMem = genAllocator.Allocate(1010);
    genAllocator.PrintDebugInfo();

    genAllocator.Free(genAllocator.Allocate(3010));
    genAllocator.PrintDebugInfo();
    mems[count++] = genAllocator.Allocate(1020);
    genAllocator.PrintDebugInfo();
    mems[count++] = genAllocator.Allocate(10110);
    genAllocator.PrintDebugInfo();
    genAllocator.Free(tmpMem);
    //mems[count++] = genAllocator.Allocate(10110);
    //mems[count++] = genAllocator.Allocate(10110);
    //mems[count++] = genAllocator.Allocate(10110);
    //mems[count++] = genAllocator.Allocate(10110);

    for (var i = 0; i < count; ++i)
    {
        genAllocator.PrintDebugInfo();
        genAllocator.Free(mems[i]);
    }

    genAllocator.PrintDebugInfo();

    genAllocator.Release();
    genAllocator.Release();


    //if (!VirtualMemory.Create(&allocator, (nuint)sizeToReserve, out var vm))
    //{
    //    Console.WriteLine("Failed to create the Virtual memory");
    //    return;
    //}

    //vm.Resize(1);
    //*((byte*)vm.Mem + pageSize-1) = 10;
    //vm.Resize(4000);
    //*((byte*)vm.Mem + pageSize - 1) = 10;
    //vm.Resize(40090);
    //*((byte*)vm.Mem + 5*pageSize - 1) = 10;
    //vm.Resize(4009110);
    //*((byte*)vm.Mem + 875 * pageSize - 1) = 10;
    //vm.Resize(40090);
    //*((byte*)vm.Mem + 5*pageSize - 1) = 10;

}


namespace Titan.Memory
{
    internal unsafe struct PoolAllocator<T> where T : unmanaged { }
    internal struct StackAllocator { }
    internal struct LinearAllocator { }
}

struct ATestStruct
{
    public uint A, B, C, D, E;
}