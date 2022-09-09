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

    [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern void GetSystemInfo(
        SYSTEM_INFO* lpSystemInfo
    );
}

unsafe struct SYSTEM_INFO
{
    //union {
    //    DWORD dwOemId;          // Obsolete field...do not use
    //    struct {
    //        WORD wProcessorArchitecture;
    //        WORD wReserved;
    //    }
    //    DUMMYSTRUCTNAME;
    //} DUMMYUNIONNAME;
    public uint dwOemId;
    public uint dwPageSize;
    public void* lpMinimumApplicationAddress;
    public void* lpMaximumApplicationAddress;
    public uint* dwActiveProcessorMask;
    public uint dwNumberOfProcessors;
    public uint dwProcessorType;
    public uint dwAllocationGranularity;
    public ushort wProcessorLevel;
    public ushort wProcessorRevision;
}