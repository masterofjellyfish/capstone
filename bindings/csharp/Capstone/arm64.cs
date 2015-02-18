// Capstone C# bindings
// By Matt Graeber <@mattifestation>, 2013>

using System;
using System.Runtime.InteropServices;

namespace Capstone.Arm64
{
    [StructLayout(LayoutKind.Sequential)]
    public struct OpMem
    {
        public REG Base;
        public REG Index;
        public int Disp;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct OpShift
    {
        public SFT Type;
        public uint Value;
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
        public OpShift Shift;
        public EXT Ext;
        public OP Type;
        public OperandValue Value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CsArm64
    {
        public CC Cc;
        [MarshalAs(UnmanagedType.U1)]
        public bool UpdateFlags;
        [MarshalAs(UnmanagedType.U1)]
        public bool Writeback;
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