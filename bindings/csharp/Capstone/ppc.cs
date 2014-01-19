using System;
using System.Runtime.InteropServices;

namespace Capstone.PowerPC
{
    [StructLayout(LayoutKind.Sequential)]
    public struct OpMem
    {
        public REG Base;
        public int Disp;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct OperandValue
    {
        [FieldOffset(0)]
        public REG Reg;
        [FieldOffset(0)]
        public int Imm;
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
    public struct CsPowerPC
    {
        public BC Bc;
        public BH Bh;
        [MarshalAs(UnmanagedType.U1)]
        public bool UpdateCR0;
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