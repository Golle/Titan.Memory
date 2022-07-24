﻿using Titan.Memory;

unsafe
{

    var native = PlatformAllocator.Create<NativeMemoryAllocator>();
    var win32 = PlatformAllocator.Create<Win32VirtualAllocAllocator>();
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"****** {nameof(PoolArena)} ******");
        Console.ResetColor();
        Console.WriteLine($"{nameof(PoolArena)} size {sizeof(PoolArena)} bytes");
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
            Console.WriteLine($"Pool Arena[{i}]: {test[i]->A} {test[i]->B} {test[i]->C}");
        }

        arena.Release();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"****** {nameof(PoolArena)} ******");
        Console.ResetColor();
    }
    Console.WriteLine();
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"****** {nameof(LinearArena)} ******");
        Console.ResetColor();
        // linear arena
        Console.WriteLine($"{nameof(LinearArena)} size {sizeof(LinearArena)} bytes");
        var arena = LinearArena.Create(&win32, 320);
        var maxCount = 12;
        var test = stackalloc TestStruct*[maxCount];
        for (var i = 0; i < maxCount; ++i)
        {
            var value = arena.Allocate<TestStruct>();
            value->A = 10 * i;
            value->B = 12 * i;
            value->C = 13 * i;
            test[i] = value;
        }
        for (var i = 0; i < 10; ++i)
        {
            Console.WriteLine($"Linear Arena[{i}]: {test[i]->A} {test[i]->B} {test[i]->C}");
        }
        arena.Reset();
        for (var i = 0; i < maxCount*3; ++i)
        {
            var value = arena.Allocate<TestStruct>();
            value->A = 10 * i;
            value->B = 12 * i;
            value->C = 13 * i;
        }
        //arena.Reset(); 
        
        for (var i = 0; i < maxCount; ++i)
        {
            var value = arena.Allocate<TestStruct>();
            value->A = 10 * i;
            value->B = 12 * i;
            value->C = 13 * i;
            test[i] = value;
        }
        arena.Release();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"****** {nameof(LinearArena)} ******");
        Console.ResetColor();
    }

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


internal unsafe struct FixedSizeArena { /*TBD*/ }
internal unsafe struct FixedPoolSizeArea { /*TBD*/ }