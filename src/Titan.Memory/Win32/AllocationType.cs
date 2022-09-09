﻿using System;

namespace Titan.Memory.Win32;

[Flags]
internal enum AllocationType : uint
{
    MEM_COMMIT = 0x00001000,
    MEM_RESERVE = 0x00002000,
    MEM_REPLACE_PLACEHOLDER = 0x00004000,
    MEM_RESERVE_PLACEHOLDER = 0x00040000,
    MEM_RESET = 0x00080000,
    MEM_TOP_DOWN = 0x00100000,
    MEM_WRITE_WATCH = 0x00200000,
    MEM_PHYSICAL = 0x00400000,
    MEM_ROTATE = 0x00800000,
    MEM_DIFFERENT_IMAGE_BASE_OK = 0x00800000,
    MEM_RESET_UNDO = 0x01000000,
    MEM_LARGE_PAGES = 0x20000000,
    MEM_4MB_PAGES = 0x80000000,
    MEM_64K_PAGES = (MEM_LARGE_PAGES | MEM_PHYSICAL),
    MEM_UNMAP_WITH_TRANSIENT_BOOST = 0x00000001,
    MEM_COALESCE_PLACEHOLDERS = 0x00000001,
    MEM_PRESERVE_PLACEHOLDER = 0x00000002,
    MEM_DECOMMIT = 0x00004000,
    MEM_RELEASE = 0x00008000,
    MEM_FREE = 0x00010000
}