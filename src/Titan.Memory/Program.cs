using Titan.Memory;

unsafe
{

    var native = PlatformAllocator.Create<NativeMemoryAllocator>();
    var win32 = PlatformAllocator.Create<Win32VirtualAllocAllocator>();

    var arena = PoolArena.Create(&win32, 10, (uint)sizeof(TestStruct));

    var maxCount = 12;
    var test = stackalloc TestStruct*[maxCount];
    for (var i = 0; i < maxCount; ++i)
    {
        var test1 = arena.Allocate<TestStruct>();
        test1->A = 10 * i;
        test1->B = 12 * i;
        test1->C = 13 * i;
        test[i] = test1;
    }

    for (var i = 0; i < 5; ++i)
    {
        arena.Free(test[i]);
    }
    for (var i = 0; i < 5; ++i)
    {
        var test1 = arena.Allocate<TestStruct>();
        test1->A = 2 * i;
        test1->B = 3 * i;
        test1->C = 4 * i;
        test[i] = test1;
    }
    for (var i = 0; i < 10; ++i)
    {
        Console.WriteLine($"Test1 {test[i]->A} {test[i]->B} {test[i]->C}");
    }

    arena.Release();

    var mem1 = (byte*)native.Allocate(1024);
    var mem2 = (byte*)win32.Allocate(1024);

    mem1[10] = 111;
    mem2[10] = 112;

    Console.WriteLine($"{*(mem1 + 10)} {*(mem2 + 10)}");

    native.Free(mem1);
    win32.Free(mem2);

    Console.WriteLine("Hello, World!");
}

internal struct TestStruct
{
    public int A, B, C;
}


internal struct LinearArena { /*TBD*/ }
internal unsafe struct FixedSizeArena { /*TBD*/ }
internal unsafe struct FixedPoolSizeArea { /*TBD*/ }
