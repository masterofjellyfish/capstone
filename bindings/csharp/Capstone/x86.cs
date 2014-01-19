// Capstone C# bindings
// By Matt Graeber <@mattifestation>, 2013>

using System;
using System.Runtime.InteropServices;

namespace Capstone.X86
{
    [StructLayout(LayoutKind.Sequential)]
    public struct OpMem
    {
        public REG Base;
        public REG Index;
        public int Scale;
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
        public double Fp;
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
    public struct CsX86
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] Prefix;
        public REG Segment;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Opcode;
        public byte OpSize;
        public byte AddrSize;
        public byte DispSize;
        public byte ImmSize;
        public byte ModRm;
        public byte Sib;
        public int Disp;
        public REG SibIndex;
        public byte SibScale;
        public REG SibBase;
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