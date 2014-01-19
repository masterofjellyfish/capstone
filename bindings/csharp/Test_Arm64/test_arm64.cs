/* Capstone Disassembler Engine - C# Binding */
/* By Matt Graeber <@mattifestation>, 2014> */

using System;
using System.Text;
using System.Runtime.InteropServices;
using Capstone;

public class TestArm64
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
        Capstone.Arm64.CsArm64 arch = (Capstone.Arm64.CsArm64) insn.Arch;

        int opcount = arch.Operands.Length;

        if (opcount > 0)
        {
            Console.WriteLine("\tOpCount: " + opcount);

            for (int i = 0; i < opcount; i++)
            {
                Console.WriteLine("\t\tOperand[" + i + "]: " + arch.Operands[i].Type);
                switch (arch.Operands[i].Type)
                {
                    case Capstone.Arm64.OP.REG:
                        Console.WriteLine("\t\t\tRegister: " + arch.Operands[i].Value.Reg);
                        break;
                    case Capstone.Arm64.OP.IMM:
                        Console.WriteLine("\t\t\tImmediate: 0x" +
                            arch.Operands[i].Value.Imm.ToString("X"));
                        break;
                    case Capstone.Arm64.OP.CIMM:
                        Console.WriteLine("\t\t\tC-Immediate: " +
                            arch.Operands[i].Value.Imm);
                        break;
                    case Capstone.Arm64.OP.FP:
                        Console.WriteLine("\t\t\tFloating Point: " + arch.Operands[i].Value.Fp);
                        break;
                    case Capstone.Arm64.OP.MEM:
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
                        if (arch.Operands[i].Value.Mem.Disp != 0)
                        {
                            Console.WriteLine("\t\t\tOperand[" + i + "].Mem.Disp: 0x" +
                                arch.Operands[i].Value.Mem.Disp.ToString("X"));
                        }
                        break;
                }

                if ((arch.Operands[i].Shift.Type != Capstone.Arm64.SFT.INVALID) &&
                    arch.Operands[i].Shift.Value != 0)
                {
                    Console.WriteLine("\t\tShift Type: " + arch.Operands[i].Shift.Type +
                        ", Value: " + arch.Operands[i].Shift.Value);
                }
                if (arch.Operands[i].Ext != Capstone.Arm64.EXT.INVALID)
                {
                    Console.WriteLine("\t\tExt: " + arch.Operands[i].Ext);
                }
            }
        }

        if (arch.UpdateFlags) { Console.WriteLine("\tUpdate-flags: True"); }
        if (arch.Writeback) { Console.WriteLine("\tWrite-back: True"); }
        if ((arch.Cc != Capstone.Arm64.CC.AL) && (arch.Cc != Capstone.Arm64.CC.INVALID))
        {
            Console.WriteLine("\tCode condition: " + arch.Cc);
        }
    }

    public static void Main()
    {
        byte[] arm64Code = new byte[] { 0x21, 0x7c, 0x02, 0x9b, 0x21, 0x7c, 0x00, 0x53, 0x00, 0x40, 0x21, 0x4b, 0xe1, 0x0b, 0x40, 0xb9, 0x20, 0x04, 0x81, 0xda, 0x20, 0x08, 0x02, 0x8b };

        uint address = 0x1000;
        UIntPtr insnCount = UIntPtr.Zero;

        platform[] platforms = {
			new platform(
					Architecture.Arm64,
					Mode.Arm,
                    OptionValue.Off,
					arm64Code,
                    "ARM-64"
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
            cs.SetDetail(true);
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