// Capstone C# bindings
// By Matt Graeber <@mattifestation>, 2013>

using System;

namespace Capstone
{
    public enum Architecture
    {
        Arm = 0,	// ARM architecture (including Thumb, Thumb-2)
        Arm64,		// ARM-64, also called AArch64
        Mips,		// Mips architecture
        X86,		// X86 architecture (including x86 & x86-64)
    }

    // These options can be binary OR'd together
    [Flags]
    public enum Mode
    {
        LittleEndian,	        // little endian mode (default mode)
        Mode16 = 1 << 1,	    // 16-bit mode
        Mode32 = 1 << 2,	    // 32-bit mode
        Mode64 = 1 << 3,	    // 64-bit mode
        Thumb = 1 << 4,         // ARM's Thumb mode, including Thumb-2
        Micro = 1 << 4,         // MicroMips mode (MIPS architecture)
        N64 = 1 << 5,           // Nintendo-64 mode (MIPS architecture)
        BigEndian = 1 << 31,    // big endian mode

        Arm = LittleEndian      // 32-bit ARM
    }

    public enum OptionType
    {
        Syntax = 1,	// Asssembly output syntax
        Detail	    // Break down instruction structure into details
    }

    public enum OptionValue
    {
        Off = 0,        // Turn OFF an option (DETAIL)
        SyntaxIntel,   // X86 Intel asm syntax - default syntax on X86 (SYNTAX).
        SyntaxATT,     // X86 ATT asm syntax (SYNTAX)
        On             // Turn ON an option - default option for DETAIL
    }

    public enum ErrorCode
    {
        Ok = 0,	// No error: everything was fine
        Mem,    // Out-Of-Memory error: cs_open(), cs_disasm_dyn()
        Arch,	// Unsupported architecture: cs_open()
        Handle,	// Invalid handle: cs_op_count(), cs_op_index()
        Csh,    // Invalid csh argument: cs_close(), cs_errno(), cs_option()
        Mode,	// Invalid/unsupported mode: cs_open()
        Option	// Invalid/unsupported option: cs_option()
    }
}