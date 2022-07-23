using System.Runtime.InteropServices;

namespace Titan.Memory.Win32;

unsafe class Kernel32
{
    [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern void* VirtualAlloc(
        void* lpAddress,
        nuint dwSize,
        AllocationType flAllocationType,
        AllocationProtect flProtect
    );

    [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool VirtualFree(
        void* lpAddress,
        nuint dwSize,
        AllocationType dwFreeType
    );
}