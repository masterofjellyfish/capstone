/* Capstone Disassembler Engine - C# Binding */
/* By Matt Graeber <@mattifestation>, 2014> */

using System;
using System.Text;
using System.Runtime.InteropServices;
using Capstone;

public class TestX86
{
    struct platform
    {
        public Architecture arch;
        public Mode mode;
        public OptionValue syntax;
        public byte[] code;
        public string comment;

        public platform(Architecture a, Mode m, OptionValue x, byte[] c, string s)
        {
            arch = a;
            mode = m;
            syntax = x;
            code = c;
            comment = s;
        }
    }

    internal static string PrintBytes(byte[] bytes)
    {
        string[] hexBytes = new string[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            hexBytes[i] = "0x" + bytes[i].ToString("X2");
        }

        return string.Join(", ", hexBytes);
    }

    internal static void PrintInsnDetail(Mode mode, Instruction insn)
    {
        Capstone.X86.CsX86 arch = (Capstone.X86.CsX86)insn.Arch;

        Console.WriteLine("\tPrefix: " + PrintBytes(arch.Prefix));
        if (arch.Segment != Capstone.X86.REG.INVALID)
        {
            Console.WriteLine("\tSegment Override: " + arch.Segment);
        }
        Console.WriteLine("\tOpcode: " + PrintBytes(arch.Opcode));
        Console.WriteLine("\tOpSize: " + arch.OpSize + ", AddrSize: " +
            arch.AddrSize + ", DispSize: " + arch.DispSize +
            ", ImmSize: " + arch.ImmSize);
        Console.WriteLine("\tModRM: " + arch.ModRm);
        Console.WriteLine("\tDisp: 0x" + arch.Disp.ToString("X"));
        if (mode != Mode.Mode16)
        {
            Console.WriteLine("\tSib: " + arch.Sib);

            if (arch.Sib != 0)
            {
                Console.WriteLine("\tSibIndex: " + arch.SibIndex + ", SibScale: " +
                    arch.SibScale + ", SibBase: " + arch.SibBase);
            }
        }

        int opcount = arch.Operands.Length;

        if (opcount > 0)
        {
            Console.WriteLine("\tOpCount: " + opcount);

            for (int i = 0; i < opcount; i++)
            {
                Console.WriteLine("\t\tOperand[" + i + "]: " + arch.Operands[i].Type.ToString());
                switch (arch.Operands[i].Type)
                {
                    case Capstone.X86.OP.REG:
                        Console.WriteLine("\t\t\tRegister: " + arch.Operands[i].Value.Reg);
                        break;
                    case Capstone.X86.OP.IMM:
                        Console.WriteLine("\t\t\tImmediate: 0x" +
                            arch.Operands[i].Value.Imm.ToString("X"));
                        break;
                    case Capstone.X86.OP.FP:
                        Console.WriteLine("\t\t\tFloating Point: " + arch.Operands[i].Value.Fp);
                        break;
                    case Capstone.X86.OP.MEM:
                        if (arch.Operands[i].Value.Mem.Base != 0)
                        {
                            Console.WriteLine("\t\t\tOperand[" + i + "].Mem.Base: " +
                                arch.Operands[i].Value.Mem.Base);
                        }
                        if (arch.Operands[i].Value.Mem.Index != 0)
                        {
                            Console.WriteLine("\t\t\tOperand[" + i + "].Mem.Index: " +
                                arch.Operands[i].Value.Mem.Index);
                        }
                        if (arch.Operands[i].Value.Mem.Scale != 1)
                        {
                            Console.WriteLine("\t\t\tOperand[" + i + "].Mem.Scale: " +
                                arch.Operands[i].Value.Mem.Scale);
                        }
                        if (arch.Operands[i].Value.Mem.Disp != 0)
                        {
                            Console.WriteLine("\t\t\tOperand[" + i + "].Mem.Disp: 0x" +
                                arch.Operands[i].Value.Mem.Disp.ToString("X"));
                        }
                        break;
                }
            }
        }
    }

    public static void Main()
    {
        byte[] code16 = new byte[] { 0x8d, 0x4c, 0x32, 0x08, 0x01, 0xd8, 0x81, 0xc6, 0x34, 0x12, 0x00, 0x00, 0x05, 0x23, 0x01, 0x00, 0x00, 0x36, 0x8b, 0x84, 0x91, 0x23, 0x01, 0x00, 0x00, 0x41, 0x8d, 0x84, 0x39, 0x89, 0x67, 0x00, 0x00, 0x8d, 0x87, 0x89, 0x67, 0x00, 0x00, 0xb4, 0xc6 };
        byte[] code32 = new byte[] { 0x8d, 0x4c, 0x32, 0x08, 0x01, 0xd8, 0x81, 0xc6, 0x34, 0x12, 0x00, 0x00, 0x05, 0x23, 0x01, 0x00, 0x00, 0x36, 0x8b, 0x84, 0x91, 0x23, 0x01, 0x00, 0x00, 0x41, 0x8d, 0x84, 0x39, 0x89, 0x67, 0x00, 0x00, 0x8d, 0x87, 0x89, 0x67, 0x00, 0x00, 0xb4, 0xc6 };
        byte[] code64 = new byte[] { 0x55, 0x48, 0x8b, 0x05, 0xb8, 0x13, 0x00, 0x00 };

        uint address = 0x1000;
        UIntPtr insnCount = UIntPtr.Zero;

        platform[] platforms = {
			new platform(
					Architecture.X86,
					Mode.Mode16,
                    OptionValue.SyntaxIntel,
					code16,
                    "X86 16bit (Intel syntax)"
			),
			new platform(
					Architecture.X86,
					Mode.Mode32,
                    OptionValue.SyntaxATT,
					code32,
                    "X86 32bit (AT&T syntax)"
					),
			new platform(
					Architecture.X86,
					Mode.Mode32,
                    OptionValue.SyntaxIntel,
					code32,
                    "X86 32bit (Intel syntax)"
					),
            new platform(
					Architecture.X86,
					Mode.Mode64,
                    OptionValue.SyntaxIntel,
					code64,
                    "X86 64bit (Intel syntax)"
					)
		};

        for (int j = 0; j < platforms.Length; j++)
        {
            System.Console.WriteLine();
            System.Console.WriteLine("************");
            System.Console.WriteLine("Platform: {0}", platforms[j].comment);
            System.Console.WriteLine();

            Capstone.Capstone cs = new Capstone.Capstone(platforms[j].arch, platforms[j].mode);
            cs.SetSyntax(platforms[j].syntax);
            Instruction[] insns = cs.Disassemble(platforms[j].code, address, insnCount);
            for (int i = 0; i < insns.Length; i++)
            {
                Console.WriteLine(insns[i]);
                PrintInsnDetail(platforms[j].mode, insns[i]);
                Console.WriteLine();
            }
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}