using System.Runtime.CompilerServices;

namespace Titan.Memory;

public static class MemoryUtils
{
    /// <summary>
    /// Aligns to 8 bytes.
    /// </summary>
    /// <param name="size">The size of the memory block</param>
    /// <returns>The 8 bytes aligned size</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint Align(nuint size)
        => size & ~(nuint)7u;

    public static uint Align(uint size)
        => size & ~7u;

    public static uint AlignUp(uint size)
        => (size & ~7u) + 8u;

    public static nuint Align(nuint size, uint alignment)
    {
        var mask = alignment - 1u;
        var alignedMemory = size & ~mask;
        return alignedMemory;
    }

    public static nuint AlignUp(nuint size, uint alignment)
    {
        var alignedMemory = Align(size, alignment);
        return alignedMemory < size ? alignedMemory + alignment : alignedMemory;
    }
}