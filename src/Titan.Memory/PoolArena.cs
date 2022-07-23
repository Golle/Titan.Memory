using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Memory;

internal unsafe struct PoolArena
{
    private PlatformAllocator* _allocator; // used to free and expand the pool
    private byte* _mem;
    private Block* _freeList;
    private ushort _blockSize;
    private ushort _count;
    public static PoolArena Create(PlatformAllocator* allocator, uint count, uint blockSize) =>
        new()
        {
            _allocator = allocator,
            _blockSize = (ushort)blockSize,
            _count = (ushort)count,
            _freeList = null,
            _mem = null
        };

    public void* Allocate(bool zeroMemory = true)
    {
        if (_freeList == null)
        {
            Expand();
        }
        var nextBlock = _freeList;
        Debug.Assert(nextBlock != null, "Not sure what happened here");
        _freeList = _freeList->Next;
        Console.WriteLine($"Allocate {(nuint)nextBlock}");
        if (zeroMemory)
        {
            Unsafe.InitBlockUnaligned(nextBlock, 0, _blockSize);
        }
        return nextBlock;
    }

    public void Free(void* ptr)
    {
        IsInRange(ptr);
        Console.WriteLine($"Free {(nuint)ptr}");
        var block = ((Block*)ptr);
        block->Next = _freeList;
        _freeList = block;
    }

    public void Release()
    {
        var mem = _mem;
        do
        {
            var prev = GetFooter(mem)->PreviousBlock;
            Console.WriteLine($"Release memory block: {(nuint)mem}");
            _allocator->Free(mem);
            mem = prev;
        } while (mem != null);
    }

    private void Expand()
    {
        var footerSize = sizeof(Footer);
        var poolSize = _blockSize * _count;
        var mem = (byte*)_allocator->Allocate((nuint)(poolSize + footerSize));
        for (var i = 0; i < _count - 1; ++i)
        {
            var block = (Block*)(mem + _blockSize * i);
            block->Next = (Block*)(mem + _blockSize * (i + 1));
        }
        Console.WriteLine($"Expanding pool, stor old pointer: {(nuint)_mem}");
        var footer = (Footer*)(mem + poolSize);
        footer->PreviousBlock = _mem;
        _mem = mem;
        _freeList = (Block*)_mem;
    }

    private struct Block
    {
        public Block* Next;
    }

    private struct Footer
    {
        public byte* PreviousBlock;
    }

    [Conditional("DEBUG")]
    private void IsInRange(void* ptr)
    {
        var memory = _mem;
        do
        {
            if (ptr >= memory && ptr <= memory + (_count * _blockSize))
            {
                return;
            }
            memory = GetFooter(memory)->PreviousBlock;
        } while (memory != null);
        Debug.Assert(false, "Trying to free a block of memory not allocated by this pool");
    }

    private Footer* GetFooter(byte* mem) => (Footer*)(mem + (_blockSize * _count));
}

internal static class PoolArenaExtensions
{
    public static unsafe T* Allocate<T>(this ref PoolArena arena) where T : unmanaged => (T*)arena.Allocate();
}
