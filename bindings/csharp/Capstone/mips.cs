// Capstone C# bindings
// By Matt Graeber <@mattifestation>, 2013>

using System;
using System.Runtime.InteropServices;

namespace Capstone.Mips
{
    [StructLayout(LayoutKind.Sequential)]
    public struct OpMem
    {
        public REG Base;
        public long Disp;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct OperandValue
    {
        [FieldOffset(0)]
        public REG Reg;
        [FieldOffset(0)]
        public long Imm;
        [FieldOffset(0)]
        public OpMem Mem;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Operand
    {
        public OP Type;
        public OperandValue Value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CsMips
    {
        private byte OpCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        private Operand[] OperandsInternal;
        public Operand[] Operands
        {
            get
            {
                int count = this.OpCount;
                Operand[] OpArray = new Operand[count];
                Array.Copy(this.OperandsInternal, OpArray, count);

                return OpArray;
            }
        }
    }
}