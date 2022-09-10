//#define TEST_GEN
//#define TEST_LINEAR
#define TEST_LINEAR_DYNAMIC

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
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


#if TEST_GEN
    {
        if (!GeneralAllocator.Create(&allocator, (nuint)sizeToReserve, 0, out var genAllocator))
        {
            Console.WriteLine($"Failed to create the {nameof(GeneralAllocator)}");
            return -1;
        }

        var mems = stackalloc void*[100];
        var count = 0;
        mems[count++] = genAllocator.Allocate(150);
        genAllocator.Free(genAllocator.Allocate(180));
        mems[count++] = genAllocator.Allocate(2010);
        var tmpMem = genAllocator.Allocate(1010);
        genAllocator.PrintDebugInfo();

        genAllocator.Free(genAllocator.Allocate(3010));
        genAllocator.PrintDebugInfo();
        mems[count++] = genAllocator.Allocate(1020);
        genAllocator.PrintDebugInfo();
        mems[count++] = genAllocator.Allocate(10110);
        genAllocator.PrintDebugInfo();
        genAllocator.Free(tmpMem);

        for (var i = 0; i < count; ++i)
        {
            genAllocator.PrintDebugInfo();
            genAllocator.Free(mems[i]);
        }

        genAllocator.PrintDebugInfo();

        genAllocator.Release();
        genAllocator.Release();
    }
#endif

#if TEST_LINEAR
    {
        if (!GeneralAllocator.Create(&allocator, 1024 * 1024 * 1024, 0, out var genAllocator))
        {
            Console.WriteLine($"Failed to create a {nameof(GeneralAllocator)}.");
            return -1;
        }

        if (!LinearFixedSizeAllocator.Create(&genAllocator, 1024 * 1024, out var linearAllocator))
        {
            Console.WriteLine($"Failed to create a {nameof(LinearFixedSizeAllocator)}.");
            return -1;
        }

        const int max = 100;
        var testStructs = stackalloc ATestStruct*[max];
        for (var i = 1u; i < max; ++i)
        {
            testStructs[i] = linearAllocator.Allocate<ATestStruct>();
            testStructs[i]->A = 10 * i;
            testStructs[i]->C = 15 * i;
        }

        for (var i = 1; i < max; ++i)
        {
            var str = testStructs[i];
            Debug.Assert(str->A == i * 10);
            Debug.Assert(str->C == i * 15);
        }
        linearAllocator.DebugPrint();
        linearAllocator.Reset();
        linearAllocator.DebugPrint();

        for (var i = 1u; i < max; ++i)
        {
            testStructs[i] = linearAllocator.AllocateAligned<ATestStruct>();
            testStructs[i]->A = 10 * i;
            testStructs[i]->C = 15 * i;
        }

        for (var i = 1; i < max; ++i)
        {
            var str = testStructs[i];
            Debug.Assert(str->A == i * 10);
            Debug.Assert(str->C == i * 15);
        }
        linearAllocator.DebugPrint();
        linearAllocator.Release();
        linearAllocator.DebugPrint();

    }
#endif

#if TEST_LINEAR_DYNAMIC
    {
        if (!LinearDynamicSizeAllocator.Create(&allocator, 1024 * 1024, out var linearDynamic))
        {
            Console.WriteLine($"Failed to create the {nameof(LinearDynamicSizeAllocator)}");
            return -1;
        }

        const int maxCount = 10000;
        var allocs = stackalloc ATestStruct*[maxCount];

        for (var i = 0; i < maxCount; i++)
        {
            allocs[i] = linearDynamic.Allocate<ATestStruct>();
            allocs[i]->A = (uint)(10 * (i + 1));
            allocs[i]->E = (uint)(15 * (i + 1));
        }

        for (var i = 0; i < maxCount; i++)
        {
            Debug.Assert(allocs[i]->A == (uint)(10 * (i + 1)));
            Debug.Assert(allocs[i]->E == (uint)(15 * (i + 1)));
        }

        linearDynamic.Reset();

        for (var i = 0; i < maxCount; i++)
        {
            allocs[i] = linearDynamic.Allocate<ATestStruct>();
            allocs[i]->A = (uint)(10 * (i + 1));
            allocs[i]->E = (uint)(15 * (i + 1));
        }


        linearDynamic.Release();

    }
#endif

}

return 0;

namespace Titan.Memory
{
    internal unsafe struct PoolAllocator<T> where T : unmanaged { }
    internal struct StackAllocator { }

}

struct ATestStruct
{
    public uint A, B, C, D, E;
}